#!/usr/bin/env python3
"""Confere os guias (content/<idioma>/guides): índice, arquivos, paridade pt-BR/en e diretivas.

Modo padrão: valida o que existe. --strict: exige também um guia por jogo de GameVersions com as 9 seções.
Uso: python3 scripts/check-guides.py [--strict] [--root PASTA]
"""
import argparse
import json
import re
import sys
from pathlib import Path

LANGS = ("pt-BR", "en")
SECTIONS = ("games", "mechanics", "vgc", "mythology")
REQUIRED_GAME_SECTIONS = ("overview", "walkthrough", "bosses", "tms", "exclusives", "trades", "events", "postgame",
                          "legendaries")
# Igual a DIRECTIVE_LINE de frontend/src/lib/guides.ts.
DIRECTIVE = re.compile(r"^[ \t]*:::([A-Za-z0-9_][\w-]*)(?:[ \t]+(.*?))?[ \t]*$")
FENCE = re.compile(r"^[ \t]*(```|~~~)")
COMMENT = re.compile(r"<!--.*?-->", re.S)


class Data:
    """Jogos (gameVersions.ts), ids dos chefes e das áreas (assets/data)."""

    def __init__(self, root: Path):
        source = (root / "frontend/src/lib/gameVersions.ts").read_text(encoding="utf-8")
        self.games = re.findall(r"^\s*g\('([a-z0-9]+)'", source, re.M)
        self.root = root

    def ids(self, kind: str, game: str) -> set[str] | None:
        path = self.root / "assets/data" / kind / f"{game}.json"
        if not path.exists():
            return None
        return {item["id"] for item in json.loads(path.read_text(encoding="utf-8"))["items"]}


def check_game(data: Data, game: str) -> str | None:
    return None if game in data.games else f"jogo desconhecido '{game}'"


def check_id(data: Data, kind: str, label: str):
    def check(args):
        if (err := check_game(data, args[0])) is not None:
            return err
        ids = data.ids(kind, args[0])
        if ids is None:
            return f"sem assets/data/{kind}/{args[0]}.json"
        return None if args[1] in ids else f"{label} '{args[1]}' não existe em {args[0]}"
    return check


def directive_specs(data: Data):
    """nome → (argumentos mínimos, máximos, conferência). Bloco novo no frontend = uma linha aqui."""
    return {
        "bosses": (1, 1, lambda a: check_game(data, a[0])),
        "boss": (2, 2, check_id(data, "bosses", "chefe")),
        "encounters": (2, 2, check_id(data, "areas", "área")),
        "tms": (1, 1, lambda a: check_game(data, a[0])),
        "shiny-methods": (0, 1, lambda a: check_game(data, a[0]) if a else None),
    }


def directives(text: str):
    """(linha, nome, argumentos) de cada diretiva, fora de blocos de código e comentários."""
    text = COMMENT.sub(lambda m: "\n" * m.group(0).count("\n"), text)
    fence = None
    for number, line in enumerate(text.split("\n"), 1):
        f = FENCE.match(line)
        if f:
            fence = f.group(1) if fence is None else (None if fence == f.group(1) else fence)
            continue
        if fence is None and (m := DIRECTIVE.match(line)):
            yield number, m.group(1), (m.group(2) or "").split()


def load_index(path: Path, errors: list[str]):
    try:
        index = json.loads(path.read_text(encoding="utf-8"))
    except FileNotFoundError:
        errors.append(f"{path}: não existe")
        return None
    except json.JSONDecodeError as e:
        errors.append(f"{path}: JSON inválido ({e})")
        return None
    if not isinstance(index, dict):
        errors.append(f"{path}: precisa ser um objeto")
        return None
    for key in index:
        if key not in SECTIONS:
            errors.append(f"{path}: seção desconhecida '{key}'")
    for section in SECTIONS:
        guides = index.get(section)
        if not isinstance(guides, list):
            errors.append(f"{path}: falta a lista '{section}'")
            index[section] = []
            continue
        seen = set()
        for guide in guides:
            key = guide.get("key") if isinstance(guide, dict) else None
            where = f"{path}: {section}/{key}"
            if not key or not guide.get("title") or not isinstance(guide.get("sections"), list) or not guide["sections"]:
                errors.append(f"{where}: guia precisa de key, title e sections (não vazia)")
                continue
            if key in seen:
                errors.append(f"{where}: key repetida")
            seen.add(key)
            ids = set()
            for page in guide["sections"]:
                if not all(isinstance(page.get(k), str) and page.get(k) for k in ("id", "title", "file")):
                    errors.append(f"{where}: seção precisa de id, title e file")
                    continue
                if page["id"] in ids:
                    errors.append(f"{where}: seção '{page['id']}' repetida")
                ids.add(page["id"])
                if not page["file"].endswith(".md") or ".." in page["file"] or page["file"].startswith("/"):
                    errors.append(f"{where}: arquivo inválido '{page['file']}'")
    return index


def shape(index) -> dict:
    """O que precisa ser igual nos dois idiomas: guias, seções e arquivos (os títulos são traduzidos)."""
    return {s: [(g.get("key"), [(p.get("id"), p.get("file")) for p in g.get("sections", []) if isinstance(p, dict)])
                for g in index[s] if isinstance(g, dict)] for s in SECTIONS}


def check(root: Path, strict: bool = False) -> list[str]:
    errors: list[str] = []
    data = Data(root)
    specs = directive_specs(data)
    indexes = {}
    for lang in LANGS:
        base = root / "content" / lang / "guides"
        index = load_index(base / "index.json", errors)
        if index is None:
            continue
        indexes[lang] = index
        for guide in index["games"]:
            if isinstance(guide, dict) and guide.get("key") and guide["key"] not in data.games:
                errors.append(f"{base}/index.json: games/{guide['key']}: jogo desconhecido")
        referenced = set()
        for section in SECTIONS:
            for guide in index[section]:
                for page in guide.get("sections", []) if isinstance(guide, dict) else []:
                    if isinstance(page, dict) and isinstance(page.get("file"), str):
                        referenced.add(page["file"])
                        if not (base / page["file"]).is_file():
                            errors.append(f"{base / page['file']}: não existe (citado no index.json)")
        for md in sorted(base.rglob("*.md")):
            rel = md.relative_to(base).as_posix()
            if rel not in referenced:
                errors.append(f"{md}: fora do index.json")
            other = [l for l in LANGS if l != lang and not (root / "content" / l / "guides" / rel).is_file()]
            if other:
                errors.append(f"{md}: falta em {', '.join(other)}")
            for line, name, args in directives(md.read_text(encoding="utf-8")):
                where = f"{md}:{line}: :::{name}"
                if name not in specs:
                    errors.append(f"{where}: diretiva desconhecida")
                    continue
                low, high, validate = specs[name]
                if not low <= len(args) <= high:
                    errors.append(f"{where}: espera de {low} a {high} argumentos, veio {len(args)}")
                elif (err := validate(args)) is not None:
                    errors.append(f"{where}: {err}")
    if len(indexes) == len(LANGS) and shape(indexes["pt-BR"]) != shape(indexes["en"]):
        errors.append("index.json: guias, seções ou arquivos diferentes entre pt-BR e en")
    if strict:
        for lang, index in indexes.items():
            games = {g.get("key"): g for g in index["games"] if isinstance(g, dict)}
            for game in data.games:
                if game not in games:
                    errors.append(f"{lang}: falta o guia do jogo '{game}'")
                    continue
                ids = {p.get("id") for p in games[game].get("sections", []) if isinstance(p, dict)}
                missing = [s for s in REQUIRED_GAME_SECTIONS if s not in ids]
                if missing:
                    errors.append(f"{lang}: guia '{game}' sem as seções {', '.join(missing)}")
            for section in SECTIONS:
                if not index[section]:
                    errors.append(f"{lang}: seção '{section}' vazia")
    return errors


def main(argv=None) -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--strict", action="store_true", help="exige todos os jogos com as 9 seções")
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parent.parent)
    args = parser.parse_args(argv)
    errors = check(args.root, args.strict)
    for e in errors:
        print(e, file=sys.stderr)
    print(f"check-guides: {len(errors)} erro(s)" + (" (--strict)" if args.strict else ""))
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())

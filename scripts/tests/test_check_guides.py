"""Testes do scripts/check-guides.py com uma pasta mínima (dois jogos, um chefe, uma área)."""
import importlib.util
import json
import tempfile
import unittest
from pathlib import Path

SCRIPT = Path(__file__).resolve().parent.parent / "check-guides.py"
spec = importlib.util.spec_from_file_location("check_guides", SCRIPT)
check_guides = importlib.util.module_from_spec(spec)
spec.loader.exec_module(check_guides)

GAME_VERSIONS = """export const GAME_VERSIONS = [
  g('rb', 'Red / Blue', 1, 'kanto', 'red-blue', ['red', 'blue'], null, false, false),
  g('frlg', 'FireRed / LeafGreen', 3, 'kanto', 'firered-leafgreen', ['firered', 'leafgreen'], 'frlg', false, false),
  g('bdsp', 'Brilliant Diamond / Shining Pearl', 8, 'sinnoh', 'brilliant-diamond-shining-pearl', ['brilliant-diamond', 'shining-pearl'], 'bdsp', true, true),
]
"""
GAMES = ("rb", "frlg", "bdsp")


def guide(key, *pages):
    return {"key": key, "title": key, "sections": [{"id": p, "title": p, "file": f"games/{key}/{p}.md"} for p in pages]}


class CheckGuidesTests(unittest.TestCase):
    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.root = Path(self._tmp.name)
        self.write("frontend/src/lib/gameVersions.ts", GAME_VERSIONS)
        self.write("assets/data/bosses/frlg.json", json.dumps({"source": [], "items": [{"id": "brock"}]}))
        self.write("assets/data/areas/frlg.json", json.dumps({"source": [], "items": [{"id": "route-1"}]}))
        index = {"games": [guide("frlg", "overview")], "mechanics": [], "vgc": [], "mythology": []}
        body = "## Chefes\n\n- Dica\n  :::boss frlg brock\n:::bosses frlg\n:::encounters frlg route-1\n:::tms frlg\n:::shiny-methods\n"
        for lang in check_guides.LANGS:
            self.write(f"content/{lang}/guides/index.json", json.dumps(index))
            self.write(f"content/{lang}/guides/games/frlg/overview.md", body)

    def tearDown(self):
        self._tmp.cleanup()

    def write(self, rel, text):
        path = self.root / rel
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(text, encoding="utf-8")

    def errors(self, strict=False, warnings=None):
        return check_guides.check(self.root, strict, min_games=len(GAMES), warnings=warnings)

    def test_indice_valido(self):
        self.assertEqual(self.errors(), [])

    def test_arquivo_faltando_em_en(self):
        (self.root / "content/en/guides/games/frlg/overview.md").unlink()
        errors = self.errors()
        self.assertTrue(any("en/guides/games/frlg/overview.md: não existe" in e for e in errors), errors)
        self.assertTrue(any("overview.md: falta em en" in e for e in errors), errors)

    def test_arquivo_fora_do_indice(self):
        for lang in check_guides.LANGS:
            self.write(f"content/{lang}/guides/games/frlg/solto.md", "oi")
        self.assertTrue(any("solto.md: fora do index.json" in e for e in self.errors()))

    def test_indices_diferentes(self):
        index = {"games": [guide("frlg", "overview", "tms")], "mechanics": [], "vgc": [], "mythology": []}
        self.write("content/en/guides/index.json", json.dumps(index))
        self.write("content/en/guides/games/frlg/tms.md", "oi")
        self.assertTrue(any("diferentes entre pt-BR e en" in e for e in self.errors()))

    def test_diretiva_com_jogo_inexistente(self):
        for lang in check_guides.LANGS:
            self.write(f"content/{lang}/guides/games/frlg/overview.md", ":::bosses zz\n:::boss frlg misty\n:::encounters frlg route-9\n")
        errors = self.errors()
        self.assertTrue(any(":::bosses: jogo desconhecido 'zz'" in e for e in errors), errors)
        self.assertTrue(any("chefe 'misty' não existe" in e for e in errors), errors)
        self.assertTrue(any("área 'route-9' não existe" in e for e in errors), errors)

    def test_diretiva_desconhecida_e_argumentos(self):
        for lang in check_guides.LANGS:
            self.write(f"content/{lang}/guides/games/frlg/overview.md", ":::chefoes frlg\n:::tms\n```\n:::ignorada\n```\n")
        errors = self.errors()
        self.assertTrue(any(":::chefoes: diretiva desconhecida" in e for e in errors), errors)
        self.assertTrue(any(":::tms: espera de 1 a 1 argumentos" in e for e in errors), errors)
        self.assertFalse(any("ignorada" in e for e in errors), errors)

    def test_diretivas_do_vgc_sem_argumentos(self):
        for lang in check_guides.LANGS:
            self.write(f"content/{lang}/guides/games/frlg/overview.md", ":::vgc-regulation\n:::vgc-teams\n")
        self.assertEqual(self.errors(), [])
        for lang in check_guides.LANGS:
            self.write(f"content/{lang}/guides/games/frlg/overview.md", ":::vgc-teams sv\n")
        self.assertTrue(any(":::vgc-teams: espera de 0 a 0 argumentos" in e for e in self.errors()))

    def test_strict_com_jogo_sem_secao(self):
        self.assertEqual(self.errors(), [])
        errors = self.errors(strict=True)
        self.assertTrue(any("guia 'frlg' sem as seções walkthrough" in e for e in errors), errors)
        self.assertTrue(any("falta o guia do jogo 'rb'" in e for e in errors), errors)
        self.assertTrue(any("seção 'mechanics' vazia" in e for e in errors), errors)

    def test_strict_completo_passa(self):
        index = {
            "games": [guide(g, *check_guides.REQUIRED_GAME_SECTIONS) for g in GAMES],
            **{s: [{"key": "x", "title": "X", "sections": [{"id": "x", "title": "X", "file": f"{s}/x.md"}]}]
               for s in ("mechanics", "vgc", "mythology")},
        }
        for lang in check_guides.LANGS:
            self.write(f"content/{lang}/guides/index.json", json.dumps(index))
            for g in index["games"]:
                for p in g["sections"]:
                    self.write(f"content/{lang}/guides/{p['file']}", "- fato")
            for s in ("mechanics", "vgc", "mythology"):
                self.write(f"content/{lang}/guides/{s}/x.md", "- fato")
        self.assertEqual(self.errors(strict=True), [])

    def test_tms_incompleto_e_aviso(self):
        for lang in check_guides.LANGS:
            self.write(f"content/{lang}/guides/games/frlg/overview.md", ":::tms bdsp\n:::tms frlg\n")
        warnings = []
        self.assertEqual(self.errors(warnings=warnings), [])
        self.assertEqual(len(warnings), 2)  # um por idioma, só o bdsp
        self.assertTrue(all(":::tms" in w and "17" in w for w in warnings), warnings)

    def test_game_versions_ilegivel_e_erro(self):
        # Menos jogos que o mínimo: erro explícito (o --strict não passa à toa).
        self.assertIn("li 3 jogos", check_guides.check(self.root, strict=True, min_games=22)[0])
        self.write("frontend/src/lib/gameVersions.ts", "export const GAME_VERSIONS = [g(\n  'rb')]\n")
        errors = self.errors(strict=True)
        self.assertEqual(len(errors), 1)
        self.assertIn("li 0 jogos", errors[0])

    def test_conteudo_do_repositorio_passa(self):
        repo = Path(__file__).resolve().parent.parent.parent
        self.assertEqual(check_guides.check(repo), [])


if __name__ == "__main__":
    unittest.main()

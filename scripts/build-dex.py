#!/usr/bin/env python3
"""Gera assets/data/dex.sqlite, a Pokédex offline do RockAssistent (somente leitura no app).

Baixa os CSVs do PokeAPI (data/v2/csv, licença BSD-3) num commit fixo para
~/.cache/traderock/pokeapi/<COMMIT>/ (reusa o que já estiver lá) e monta um SQLite enxuto:
- nomes de espécie em en, ja-Hrkt, ja, fr, de, es, it, ko e pt-BR (se existir);
- textos da Pokédex em en e pt-BR (se existir); descrições (prose) e demais nomes só em en;
- métodos e condições de encontro, gatilhos de evolução, categorias etc. viram texto (identifier).

O arquivo gerado não vai para o git; o TradeRock.Core o embute como data/dex.sqlite.
Só usa a biblioteca padrão (csv, sqlite3, urllib). Precisa de internet só na primeira vez.
Uso: scripts/build-dex.py [--force] [--out caminho]
"""
import argparse
import concurrent.futures
import csv
import datetime
import hashlib
import os
import re
import sqlite3
import sys
import urllib.request
from pathlib import Path

COMMIT = "6ea97b5641faa12e090866e8144ae8f57764fc64"  # PokeAPI/pokeapi, 2026-11-05
BASE_URL = "https://raw.githubusercontent.com/PokeAPI/pokeapi/{commit}/data/v2/csv/{name}.csv"
ROOT = Path(__file__).resolve().parent.parent
OUT = ROOT / "assets" / "data" / "dex.sqlite"
CACHE = Path.home() / ".cache" / "traderock" / "pokeapi"

NAME_LANGUAGES = ("en", "ja-Hrkt", "ja", "fr", "de", "es", "it", "ko", "pt-BR")
FLAVOR_LANGUAGES = ("en", "pt-BR")

CSV_FILES = (
    "languages", "generations", "regions", "pokemon_colors", "growth_rates", "experience",
    "pokemon_species", "pokemon_species_names", "pokemon_species_flavor_text",
    "pokemon", "pokemon_forms", "pokemon_form_names", "pokemon_types", "types", "type_efficacy",
    "type_efficacy_past", "pokemon_types_past",
    "pokemon_stats", "stats", "stat_names",
    "abilities", "ability_names", "ability_prose", "pokemon_abilities", "pokemon_abilities_past",
    "moves", "move_names", "move_effect_prose", "move_damage_classes", "move_changelog",
    "pokemon_moves", "pokemon_move_methods", "machines",
    "items", "item_names", "item_prose", "item_categories",
    "pokemon_evolution", "evolution_triggers",
    "egg_groups", "egg_group_prose", "pokemon_egg_groups",
    "pokedexes", "pokedex_prose", "pokemon_dex_numbers",
    "version_groups", "versions", "version_names",
    "locations", "location_names", "location_areas", "location_area_prose", "location_game_indices",
    "encounters", "encounter_slots", "encounter_methods", "encounter_condition_values",
    "encounter_condition_value_map",
    "natures", "nature_names",
)

SCHEMA = """
CREATE TABLE meta(key TEXT PRIMARY KEY, value TEXT);
CREATE TABLE languages(id INTEGER PRIMARY KEY, identifier TEXT);
CREATE TABLE species(id INTEGER PRIMARY KEY, identifier TEXT, generation_id INTEGER, color TEXT,
  capture_rate INTEGER, base_happiness INTEGER, hatch_counter INTEGER, gender_rate INTEGER, growth_rate TEXT,
  evolution_chain_id INTEGER, evolves_from_species_id INTEGER, genus_en TEXT, genus_ptbr TEXT);
CREATE TABLE species_names(species_id INTEGER, language TEXT, name TEXT);
CREATE TABLE pokemon(id INTEGER PRIMARY KEY, species_id INTEGER, identifier TEXT, height INTEGER,
  weight INTEGER, base_experience INTEGER, is_default INTEGER, form_order INTEGER);
CREATE TABLE pokemon_forms(id INTEGER PRIMARY KEY, pokemon_id INTEGER, identifier TEXT, form_name_en TEXT,
  is_mega INTEGER, is_battle_only INTEGER, introduced_in_version_group INTEGER);
CREATE TABLE pokemon_types(pokemon_id INTEGER, slot INTEGER, type_id INTEGER);
CREATE TABLE types(id INTEGER PRIMARY KEY, identifier TEXT, generation_id INTEGER);
CREATE TABLE type_efficacy(damage_type_id INTEGER, target_type_id INTEGER, damage_factor INTEGER);
CREATE TABLE type_efficacy_past(generation_id INTEGER, damage_type_id INTEGER, target_type_id INTEGER,
  damage_factor INTEGER);
CREATE TABLE pokemon_types_past(pokemon_id INTEGER, generation_id INTEGER, slot INTEGER, type_id INTEGER);
CREATE TABLE pokemon_stats(pokemon_id INTEGER, stat_id INTEGER, base_stat INTEGER, effort INTEGER);
CREATE TABLE abilities(id INTEGER PRIMARY KEY, identifier TEXT, name_en TEXT, effect_en TEXT,
  short_effect_en TEXT, generation_id INTEGER);
CREATE TABLE pokemon_abilities(pokemon_id INTEGER, ability_id INTEGER, is_hidden INTEGER, slot INTEGER);
CREATE TABLE pokemon_abilities_past(pokemon_id INTEGER, generation_id INTEGER, ability_id INTEGER,
  is_hidden INTEGER, slot INTEGER);
CREATE TABLE moves(id INTEGER PRIMARY KEY, identifier TEXT, name_en TEXT, type_id INTEGER, power INTEGER,
  pp INTEGER, accuracy INTEGER, priority INTEGER, damage_class TEXT, effect_chance INTEGER, effect_en TEXT,
  generation_id INTEGER);
CREATE TABLE move_changelog(move_id INTEGER, changed_in_version_group_id INTEGER, type_id INTEGER,
  power INTEGER, pp INTEGER, accuracy INTEGER);
CREATE TABLE pokemon_moves(pokemon_id INTEGER NOT NULL, version_group_id INTEGER NOT NULL,
  move_id INTEGER NOT NULL, method TEXT NOT NULL, level INTEGER NOT NULL, "order" INTEGER,
  PRIMARY KEY (pokemon_id, version_group_id, method, level, move_id)) WITHOUT ROWID;
CREATE TABLE machines(version_group_id INTEGER, item_id INTEGER, move_id INTEGER, machine_number INTEGER);
CREATE TABLE items(id INTEGER PRIMARY KEY, identifier TEXT, name_en TEXT, category TEXT, cost INTEGER,
  effect_en TEXT, fling_power INTEGER);
CREATE TABLE evolution(evolved_species_id INTEGER, evolution_trigger TEXT, trigger_item_id INTEGER,
  minimum_level INTEGER, gender_id INTEGER, location_id INTEGER, held_item_id INTEGER, time_of_day TEXT,
  known_move_id INTEGER, known_move_type_id INTEGER, minimum_happiness INTEGER, minimum_beauty INTEGER,
  minimum_affection INTEGER, relative_physical_stats INTEGER, party_species_id INTEGER, party_type_id INTEGER,
  trade_species_id INTEGER, needs_overworld_rain INTEGER, turn_upside_down INTEGER);
CREATE TABLE flavor_text(species_id INTEGER, version_id INTEGER, language TEXT, text TEXT);
CREATE TABLE egg_groups(id INTEGER PRIMARY KEY, identifier TEXT, name_en TEXT);
CREATE TABLE pokemon_egg_groups(species_id INTEGER, egg_group_id INTEGER);
CREATE TABLE pokedexes(id INTEGER PRIMARY KEY, identifier TEXT, region TEXT, name_en TEXT);
CREATE TABLE pokemon_dex_numbers(species_id INTEGER, pokedex_id INTEGER, number INTEGER);
CREATE TABLE version_groups(id INTEGER PRIMARY KEY, identifier TEXT, generation_id INTEGER, "order" INTEGER);
CREATE TABLE versions(id INTEGER PRIMARY KEY, version_group_id INTEGER, identifier TEXT, name_en TEXT);
CREATE TABLE locations(id INTEGER PRIMARY KEY, region_id INTEGER, identifier TEXT, name_en TEXT);
CREATE TABLE location_areas(id INTEGER PRIMARY KEY, location_id INTEGER, identifier TEXT, name_en TEXT);
CREATE TABLE location_game_indices(location_id INTEGER, generation_id INTEGER, game_index INTEGER);
CREATE TABLE encounters(id INTEGER PRIMARY KEY, version_id INTEGER, location_area_id INTEGER,
  pokemon_id INTEGER, min_level INTEGER, max_level INTEGER, method TEXT, rarity INTEGER, condition_values TEXT);
CREATE TABLE natures(id INTEGER PRIMARY KEY, identifier TEXT, name_en TEXT, decreased_stat_id INTEGER,
  increased_stat_id INTEGER);
CREATE TABLE stats(id INTEGER PRIMARY KEY, identifier TEXT, name_en TEXT);
CREATE TABLE growth_rates(id INTEGER PRIMARY KEY, identifier TEXT);
CREATE TABLE experience(growth_rate_id INTEGER, level INTEGER, experience INTEGER);
"""

INDEXES = """
CREATE INDEX ix_species_names ON species_names(species_id);
CREATE INDEX ix_species_chain ON species(evolution_chain_id);
CREATE INDEX ix_pokemon_species ON pokemon(species_id);
CREATE INDEX ix_pokemon_forms ON pokemon_forms(pokemon_id);
CREATE INDEX ix_pokemon_types ON pokemon_types(pokemon_id);
CREATE INDEX ix_pokemon_stats ON pokemon_stats(pokemon_id);
CREATE INDEX ix_pokemon_abilities ON pokemon_abilities(pokemon_id);
CREATE INDEX ix_pokemon_abilities_past ON pokemon_abilities_past(pokemon_id);
CREATE INDEX ix_move_changelog ON move_changelog(move_id);
CREATE INDEX ix_machines ON machines(version_group_id, move_id);
CREATE INDEX ix_evolution ON evolution(evolved_species_id);
CREATE INDEX ix_flavor_text ON flavor_text(species_id);
CREATE INDEX ix_pokemon_types_past ON pokemon_types_past(pokemon_id);
CREATE INDEX ix_pokemon_egg_groups ON pokemon_egg_groups(species_id);
CREATE INDEX ix_dex_numbers ON pokemon_dex_numbers(species_id);
CREATE INDEX ix_encounters ON encounters(pokemon_id, version_id);
"""

MARKUP = re.compile(r"\[([^\]]*)\]\{[^}:]*:([^}]*)\}")


def download(commit, cache_dir):
    """Baixa os CSVs que faltam para cache_dir/<commit>/ e devolve essa pasta."""
    folder = Path(cache_dir) / commit
    folder.mkdir(parents=True, exist_ok=True)

    def fetch(name):
        target = folder / f"{name}.csv"
        if target.exists() and target.stat().st_size > 0:
            return
        part = target.with_suffix(".part")
        try:
            with urllib.request.urlopen(BASE_URL.format(commit=commit, name=name), timeout=120) as response:
                part.write_bytes(response.read())
        except OSError as error:  # URLError e HTTPError são OSError
            sys.exit(f"falha ao baixar {name}.csv: {error}")
        os.replace(part, target)

    with concurrent.futures.ThreadPoolExecutor(max_workers=8) as pool:
        list(pool.map(fetch, CSV_FILES))
    return folder


def language_tag(identifier):
    """ja-hrkt → ja-Hrkt, pt-br → pt-BR (como o NAME_LANGUAGES e os locales do app)."""
    parts = identifier.split("-")
    for i in range(1, len(parts)):
        if len(parts[i]) == 2:
            parts[i] = parts[i].upper()
        elif len(parts[i]) == 4 and parts[i].isalpha():
            parts[i] = parts[i].title()
    return "-".join(parts)


def clean_prose(text, effect_chance=None):
    """Tira a marcação do PokeAPI ([texto]{tipo:id}) e troca $effect_chance pelo valor."""
    if text is None:
        return None
    text = MARKUP.sub(lambda m: m.group(1) or m.group(2).replace("-", " "), text)
    if effect_chance:
        text = text.replace("$effect_chance", str(effect_chance))
    return text.strip()


def clean_flavor(text):
    """Os textos da Pokédex trazem quebras do jogo (\\n, \\f, hífen suave): viram espaços simples."""
    return " ".join(text.replace("\u00ad", "").replace("\f", " ").split())


def build(csv_dir, out, commit):
    csv_dir, out = Path(csv_dir), Path(out)

    def rows(name):
        with open(csv_dir / f"{name}.csv", encoding="utf-8-sig", newline="") as f:
            yield from csv.DictReader(f)

    def n(value):
        return int(value) if value not in (None, "") else None

    def lookup(name, key="id", value="identifier"):
        return {n(r[key]): r[value] for r in rows(name)}

    languages = {n(r["id"]): language_tag(r["identifier"]) for r in rows("languages")}
    en = next((i for i, tag in languages.items() if tag == "en"), None)

    def english(name, key, value="name"):
        return {n(r[key]): r[value] for r in rows(name) if n(r["local_language_id"]) == en}

    out.parent.mkdir(parents=True, exist_ok=True)
    tmp = out.with_suffix(".tmp")
    tmp.unlink(missing_ok=True)
    db = sqlite3.connect(tmp)
    db.executescript(SCHEMA)

    def insert(table, values):
        values = list(values)
        if values:
            marks = ",".join("?" * len(values[0]))
            db.executemany(f"INSERT INTO {table} VALUES ({marks})", values)

    insert("languages", languages.items())

    # Espécies e nomes
    colors = lookup("pokemon_colors")
    growth = lookup("growth_rates")
    genus_en, genus_pt, names = {}, {}, []
    for r in rows("pokemon_species_names"):
        tag = languages.get(n(r["local_language_id"]))
        species = n(r["pokemon_species_id"])
        if tag == "en":
            genus_en[species] = r["genus"] or None
        elif tag == "pt-BR":
            genus_pt[species] = r["genus"] or None
        if tag in NAME_LANGUAGES and r["name"]:
            names.append((species, tag, r["name"]))
    insert("species", ((n(r["id"]), r["identifier"], n(r["generation_id"]), colors.get(n(r["color_id"])),
                        n(r["capture_rate"]), n(r["base_happiness"]), n(r["hatch_counter"]), n(r["gender_rate"]),
                        growth.get(n(r["growth_rate_id"])), n(r["evolution_chain_id"]),
                        n(r["evolves_from_species_id"]), genus_en.get(n(r["id"])), genus_pt.get(n(r["id"])))
                       for r in rows("pokemon_species")))
    insert("species_names", names)
    insert("flavor_text", ((n(r["species_id"]), n(r["version_id"]), languages[n(r["language_id"])],
                            clean_flavor(r["flavor_text"]))
                           for r in rows("pokemon_species_flavor_text")
                           if languages.get(n(r["language_id"])) in FLAVOR_LANGUAGES))

    # Pokémon, formas, tipos e status
    # form_order = coluna "order" do pokemon.csv (ordem de exibição das formas).
    insert("pokemon", ((n(r["id"]), n(r["species_id"]), r["identifier"], n(r["height"]), n(r["weight"]),
                        n(r["base_experience"]), n(r["is_default"]), n(r["order"])) for r in rows("pokemon")))
    form_names = english("pokemon_form_names", "pokemon_form_id", "form_name")
    insert("pokemon_forms", ((n(r["id"]), n(r["pokemon_id"]), r["identifier"], form_names.get(n(r["id"])) or None,
                              n(r["is_mega"]), n(r["is_battle_only"]), n(r["introduced_in_version_group_id"]))
                             for r in rows("pokemon_forms")))
    insert("pokemon_types", ((n(r["pokemon_id"]), n(r["slot"]), n(r["type_id"])) for r in rows("pokemon_types")))
    insert("types", ((n(r["id"]), r["identifier"], n(r["generation_id"])) for r in rows("types")))
    insert("type_efficacy", ((n(r["damage_type_id"]), n(r["target_type_id"]), n(r["damage_factor"]))
                             for r in rows("type_efficacy")))
    # Tabelas antigas: a linha de geração G vale para os jogos até a G (Gen 1, Aço até a Gen 5, Clefairy Normal...).
    insert("type_efficacy_past", ((n(r["generation_id"]), n(r["damage_type_id"]), n(r["target_type_id"]),
                                   n(r["damage_factor"])) for r in rows("type_efficacy_past")))
    insert("pokemon_types_past", ((n(r["pokemon_id"]), n(r["generation_id"]), n(r["slot"]), n(r["type_id"]))
                                  for r in rows("pokemon_types_past")))
    insert("pokemon_stats", ((n(r["pokemon_id"]), n(r["stat_id"]), n(r["base_stat"]), n(r["effort"]))
                             for r in rows("pokemon_stats")))
    stat_names = english("stat_names", "stat_id")
    insert("stats", ((n(r["id"]), r["identifier"], stat_names.get(n(r["id"]))) for r in rows("stats")))
    insert("growth_rates", growth.items())
    insert("experience", ((n(r["growth_rate_id"]), n(r["level"]), n(r["experience"])) for r in rows("experience")))

    # Habilidades
    ability_names = english("ability_names", "ability_id")
    ability_prose = {n(r["ability_id"]): r for r in rows("ability_prose") if n(r["local_language_id"]) == en}
    insert("abilities", ((n(r["id"]), r["identifier"], ability_names.get(n(r["id"])),
                          clean_prose(ability_prose.get(n(r["id"]), {}).get("effect")),
                          clean_prose(ability_prose.get(n(r["id"]), {}).get("short_effect")), n(r["generation_id"]))
                         for r in rows("abilities")))
    insert("pokemon_abilities", ((n(r["pokemon_id"]), n(r["ability_id"]), n(r["is_hidden"]), n(r["slot"]))
                                 for r in rows("pokemon_abilities")))
    insert("pokemon_abilities_past", ((n(r["pokemon_id"]), n(r["generation_id"]), n(r["ability_id"]),
                                       n(r["is_hidden"]), n(r["slot"])) for r in rows("pokemon_abilities_past")))

    # Golpes
    move_names = english("move_names", "move_id")
    effects = english("move_effect_prose", "move_effect_id", "effect")
    classes = lookup("move_damage_classes")
    insert("moves", ((n(r["id"]), r["identifier"], move_names.get(n(r["id"])), n(r["type_id"]), n(r["power"]),
                      n(r["pp"]), n(r["accuracy"]), n(r["priority"]), classes.get(n(r["damage_class_id"])),
                      n(r["effect_chance"]), clean_prose(effects.get(n(r["effect_id"])), r["effect_chance"]),
                      n(r["generation_id"])) for r in rows("moves")))
    insert("move_changelog", ((n(r["move_id"]), n(r["changed_in_version_group_id"]), n(r["type_id"]),
                               n(r["power"]), n(r["pp"]), n(r["accuracy"])) for r in rows("move_changelog")))
    methods = lookup("pokemon_move_methods")
    insert("pokemon_moves", ((n(r["pokemon_id"]), n(r["version_group_id"]), n(r["move_id"]),
                              methods.get(n(r["pokemon_move_method_id"])), n(r["level"]), n(r["order"]))
                             for r in rows("pokemon_moves")))
    insert("machines", ((n(r["version_group_id"]), n(r["item_id"]), n(r["move_id"]), n(r["machine_number"]))
                        for r in rows("machines")))

    # Itens
    item_names = english("item_names", "item_id")
    item_effects = english("item_prose", "item_id", "effect")
    categories = lookup("item_categories")
    insert("items", ((n(r["id"]), r["identifier"], item_names.get(n(r["id"])), categories.get(n(r["category_id"])),
                      n(r["cost"]), clean_prose(item_effects.get(n(r["id"]))), n(r["fling_power"]))
                     for r in rows("items")))

    # Evolução
    triggers = lookup("evolution_triggers")
    insert("evolution", ((n(r["evolved_species_id"]), triggers.get(n(r["evolution_trigger_id"])),
                          n(r["trigger_item_id"]), n(r["minimum_level"]), n(r["gender_id"]), n(r["location_id"]),
                          n(r["held_item_id"]), r["time_of_day"] or None, n(r["known_move_id"]),
                          n(r["known_move_type_id"]), n(r["minimum_happiness"]), n(r["minimum_beauty"]),
                          n(r["minimum_affection"]), n(r["relative_physical_stats"]), n(r["party_species_id"]),
                          n(r["party_type_id"]), n(r["trade_species_id"]), n(r["needs_overworld_rain"]),
                          n(r["turn_upside_down"])) for r in rows("pokemon_evolution")))

    # Grupos de ovo e Pokédex regionais
    egg_names = english("egg_group_prose", "egg_group_id")
    insert("egg_groups", ((n(r["id"]), r["identifier"], egg_names.get(n(r["id"]))) for r in rows("egg_groups")))
    insert("pokemon_egg_groups", ((n(r["species_id"]), n(r["egg_group_id"])) for r in rows("pokemon_egg_groups")))
    regions = lookup("regions")
    dex_names = english("pokedex_prose", "pokedex_id")
    insert("pokedexes", ((n(r["id"]), r["identifier"], regions.get(n(r["region_id"])), dex_names.get(n(r["id"])))
                         for r in rows("pokedexes")))
    insert("pokemon_dex_numbers", ((n(r["species_id"]), n(r["pokedex_id"]), n(r["pokedex_number"]))
                                   for r in rows("pokemon_dex_numbers")))

    # Jogos
    insert("version_groups", ((n(r["id"]), r["identifier"], n(r["generation_id"]), n(r["order"]))
                              for r in rows("version_groups")))
    version_names = english("version_names", "version_id")
    insert("versions", ((n(r["id"]), n(r["version_group_id"]), r["identifier"], version_names.get(n(r["id"])))
                        for r in rows("versions")))

    # Locais e encontros
    location_names = english("location_names", "location_id")
    insert("locations", ((n(r["id"]), n(r["region_id"]), r["identifier"], location_names.get(n(r["id"])))
                         for r in rows("locations")))
    area_names = english("location_area_prose", "location_area_id")
    insert("location_areas", ((n(r["id"]), n(r["location_id"]), r["identifier"] or None,
                               area_names.get(n(r["id"])) or None) for r in rows("location_areas")))
    insert("location_game_indices", ((n(r["location_id"]), n(r["generation_id"]), n(r["game_index"]))
                                     for r in rows("location_game_indices")))
    encounter_methods = lookup("encounter_methods")
    slots = {n(r["id"]): (encounter_methods.get(n(r["encounter_method_id"])), n(r["rarity"]))
             for r in rows("encounter_slots")}
    condition_names = lookup("encounter_condition_values")
    conditions = {}
    for r in rows("encounter_condition_value_map"):
        conditions.setdefault(n(r["encounter_id"]), []).append(n(r["encounter_condition_value_id"]))

    def condition_text(encounter):
        ids = sorted(conditions.get(encounter, ()))
        return ",".join(condition_names[i] for i in ids) or None

    insert("encounters", ((n(r["id"]), n(r["version_id"]), n(r["location_area_id"]), n(r["pokemon_id"]),
                           n(r["min_level"]), n(r["max_level"]), *slots.get(n(r["encounter_slot_id"]), (None, None)),
                           condition_text(n(r["id"]))) for r in rows("encounters")))

    # Naturezas
    nature_names = english("nature_names", "nature_id")
    insert("natures", ((n(r["id"]), r["identifier"], nature_names.get(n(r["id"])), n(r["decreased_stat_id"]),
                        n(r["increased_stat_id"])) for r in rows("natures")))

    built_at = datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")
    insert("meta", [("pokeapi_commit", commit), ("built_at", built_at), ("script_sha256", script_hash())])
    db.executescript(INDEXES)
    db.commit()
    db.execute("VACUUM")
    db.close()
    os.replace(tmp, out)


def script_hash():
    return hashlib.sha256(Path(__file__).read_bytes()).hexdigest()


def up_to_date(out, commit):
    """O banco já existe, foi feito deste commit e por esta versão do script."""
    if not out.exists():
        return False
    try:
        db = sqlite3.connect(f"file:{out}?mode=ro", uri=True)
        try:
            meta = dict(db.execute("SELECT key, value FROM meta"))
        finally:
            db.close()
    except sqlite3.Error:
        return False
    return meta.get("pokeapi_commit") == commit and meta.get("script_sha256") == script_hash()


def main():
    parser = argparse.ArgumentParser(description="Gera a Pokédex offline (dex.sqlite) a partir do PokeAPI.")
    parser.add_argument("--out", type=Path, default=OUT)
    parser.add_argument("--force", action="store_true", help="refaz mesmo se o banco já estiver em dia")
    args = parser.parse_args()
    if not args.force and up_to_date(args.out, COMMIT):
        print(f"{args.out} já está em dia (PokeAPI {COMMIT[:7]})")
        return 0
    csv_dir = download(COMMIT, CACHE)
    build(csv_dir, args.out, COMMIT)
    db = sqlite3.connect(args.out)
    species = db.execute("SELECT count(*) FROM species").fetchone()[0]
    db.close()
    print(f"{species} espécies em {args.out} ({args.out.stat().st_size / 1e6:.1f} MB, PokeAPI {COMMIT[:7]})")
    return 0


if __name__ == "__main__":
    sys.exit(main())

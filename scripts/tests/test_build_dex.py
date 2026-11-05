"""Testes do scripts/build-dex.py com CSVs mínimos (Gastly, Haunter e Gengar, 2 golpes, 1 encontro)."""
import csv
import importlib.util
import sqlite3
import tempfile
import unittest
from pathlib import Path

SCRIPT = Path(__file__).resolve().parent.parent / "build-dex.py"
spec = importlib.util.spec_from_file_location("build_dex", SCRIPT)
build_dex = importlib.util.module_from_spec(spec)
spec.loader.exec_module(build_dex)

FIXTURES = {
    "languages": ("id,iso639,iso3166,identifier,official,order", [
        "1,ja,jp,ja-hrkt,1,1", "5,fr,fr,fr,1,8", "9,en,us,en,1,7", "10,cs,cz,cs,0,12"]),
    "generations": ("id,main_region_id,identifier", ["1,1,generation-i"]),
    "regions": ("id,identifier", ["1,kanto"]),
    "pokemon_colors": ("id,identifier", ["7,purple"]),
    "growth_rates": ("id,identifier,formula", ["4,medium-slow,x"]),
    "pokemon_species": (
        "id,identifier,generation_id,evolves_from_species_id,evolution_chain_id,color_id,shape_id,habitat_id,"
        "gender_rate,capture_rate,base_happiness,is_baby,hatch_counter,has_gender_differences,growth_rate_id,"
        "forms_switchable,is_legendary,is_mythical,order,conquest_order", [
            "92,gastly,1,,40,7,1,1,4,190,70,0,20,0,4,0,0,0,100,",
            "93,haunter,1,92,40,7,4,1,4,90,70,0,20,0,4,0,0,0,101,",
            "94,gengar,1,93,40,7,6,1,4,45,70,0,20,0,4,1,0,0,102,114"]),
    "pokemon_species_names": ("pokemon_species_id,local_language_id,name,genus", [
        "94,9,Gengar,Shadow Pokémon", "94,5,Ectoplasma,Pokémon Ombre", "94,1,ゲンガー,シャドーポケモン",
        "94,10,Gengar,Stín", "93,9,Haunter,Gas Pokémon", "92,9,Gastly,Gas Pokémon"]),
    "pokemon": ("id,identifier,species_id,height,weight,base_experience,order,is_default", [
        "92,gastly,92,13,1,62,147,1", "93,haunter,93,16,1,142,148,1", "94,gengar,94,15,405,225,149,1"]),
    "pokemon_forms": ("id,identifier,form_identifier,pokemon_id,introduced_in_version_group_id,is_default,"
                      "is_battle_only,is_mega,form_order,order", ["94,gengar,,94,1,1,0,0,1,154"]),
    "evolution_triggers": ("id,identifier", ["1,level-up", "2,trade"]),
    "pokemon_evolution": (
        "id,evolved_species_id,evolution_trigger_id,version_group_id,is_default,trigger_item_id,minimum_level,"
        "gender_id,location_id,held_item_id,time_of_day,known_move_id,known_move_type_id,minimum_happiness,"
        "minimum_beauty,minimum_affection,relative_physical_stats,party_species_id,party_type_id,trade_species_id,"
        "needs_overworld_rain,turn_upside_down,needs_multiplayer,near_special_rock,region_id,"
        "required_pokemon_form_id,evolved_pokemon_form_id,used_move_id,minimum_move_count,minimum_steps,"
        "minimum_damage_taken,nature_bitmask,condition_expression,percentage_chance", [
            "56,93,1,1,1,,25,,,,,,,,,,,,,,0,0,0,0,,,,,,,,,,",
            "57,94,2,1,1,,,,,,,,,,,,,,,,0,0,0,0,,,,,,,,,,"]),
    "move_damage_classes": ("id,identifier", ["1,status", "2,physical", "3,special"]),
    "moves": ("id,identifier,generation_id,type_id,power,pp,accuracy,priority,target_id,damage_class_id,effect_id,"
              "effect_chance,contest_type_id,contest_effect_id,super_contest_effect_id", [
                  "122,lick,1,8,30,30,100,0,10,2,7,30,,,", "325,shadow-punch,3,8,60,20,,0,10,2,18,,,,"]),
    "move_names": ("move_id,local_language_id,name", ["122,9,Lick", "325,9,Shadow Punch", "325,5,Poing Ombre"]),
    "move_effect_prose": ("move_effect_id,local_language_id,short_effect,effect", [
        "7,9,Has a $effect_chance% chance to [paralyze]{mechanic:paralysis} the target.,Long text."]),
    "versions": ("id,version_group_id,identifier", ["10,7,firered"]),
    "version_names": ("version_id,local_language_id,name", ["10,9,FireRed"]),
    "version_groups": ("id,identifier,generation_id,order", ["7,firered-leafgreen,3,8"]),
    "locations": ("id,region_id,identifier", ["240,1,lavender-town"]),
    "location_names": ("location_id,local_language_id,name,subtitle", ["240,9,Pokémon Tower,"]),
    "location_areas": ("id,location_id,game_index,identifier", ["300,240,1,3f"]),
    "location_area_prose": ("location_area_id,local_language_id,name", ["300,9,3F"]),
    "encounter_methods": ("id,identifier,order", ["1,walk,1"]),
    "encounter_slots": ("id,version_group_id,encounter_method_id,slot,rarity", ["500,7,1,1,90"]),
    "encounters": ("id,version_id,location_area_id,encounter_slot_id,pokemon_id,min_level,max_level",
                   ["900,10,300,500,92,13,15"]),
    "encounter_condition_values": ("id,encounter_condition_id,identifier,is_default",
                                   ["3,2,time-night,0", "9,3,season-winter,0"]),
    "encounter_condition_value_map": ("encounter_id,encounter_condition_value_id", ["900,9", "900,3"]),
    "pokemon_species_flavor_text": ("species_id,version_id,language_id,flavor_text", [
        '94,10,9,"Hides in\nshadows.\fIt is said..."', "94,10,5,Se cache."]),
}


class BuildDexTest(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.tmp = tempfile.TemporaryDirectory()
        root = Path(cls.tmp.name)
        csv_dir = root / "csv"
        csv_dir.mkdir()
        for name in build_dex.CSV_FILES:
            (csv_dir / f"{name}.csv").write_text("", encoding="utf-8")
        for name, (header, rows) in FIXTURES.items():
            with open(csv_dir / f"{name}.csv", "w", encoding="utf-8", newline="") as f:
                f.write(header + "\n")
                for row in rows:
                    f.write(row + "\n")
        cls.out = root / "dex.sqlite"
        build_dex.build(csv_dir, cls.out, build_dex.COMMIT)
        cls.db = sqlite3.connect(cls.out)

    @classmethod
    def tearDownClass(cls):
        cls.db.close()
        cls.tmp.cleanup()

    def one(self, sql, *args):
        return self.db.execute(sql, args).fetchall()

    def test_species_names_keep_listed_languages(self):
        names = dict(self.one("select language, name from species_names where species_id = 94"))
        self.assertEqual(names["en"], "Gengar")
        self.assertEqual(names["fr"], "Ectoplasma")
        self.assertEqual(names["ja-Hrkt"], "ゲンガー")
        self.assertNotIn("cs", names)

    def test_species_columns(self):
        row = self.one("select identifier, color, growth_rate, genus_en, genus_ptbr, evolves_from_species_id "
                       "from species where id = 94")[0]
        self.assertEqual(row, ("gengar", "purple", "medium-slow", "Shadow Pokémon", None, 93))

    def test_gengar_evolves_by_trade(self):
        self.assertEqual(self.one("select evolution_trigger from evolution where evolved_species_id = 94"),
                         [("trade",)])
        self.assertEqual(self.one("select evolution_trigger, minimum_level from evolution "
                                  "where evolved_species_id = 93"), [("level-up", 25)])

    def test_encounter_with_method_and_conditions(self):
        rows = self.one("select version_id, location_area_id, pokemon_id, min_level, max_level, method, rarity, "
                        "condition_values from encounters")
        self.assertEqual(rows, [(10, 300, 92, 13, 15, "walk", 90, "time-night,season-winter")])

    def test_moves(self):
        rows = self.one("select id, name_en, damage_class, power, accuracy, effect_en from moves order by id")
        self.assertEqual(rows[0][:5], (122, "Lick", "physical", 30, 100))
        self.assertEqual(rows[0][5], "Long text.")
        self.assertEqual(rows[1][:5], (325, "Shadow Punch", "physical", 60, None))

    def test_flavor_text_only_en_and_normalized(self):
        self.assertEqual(self.one("select language, text from flavor_text"),
                         [("en", "Hides in shadows. It is said...")])

    def test_meta(self):
        meta = dict(self.one("select key, value from meta"))
        self.assertEqual(meta["pokeapi_commit"], build_dex.COMMIT)
        self.assertIn("built_at", meta)
        self.assertEqual(build_dex.COMMIT, "6ea97b5641faa12e090866e8144ae8f57764fc64")


class MarkupTest(unittest.TestCase):
    def test_prose_markup_is_stripped(self):
        self.assertEqual(build_dex.clean_prose("[paralyze]{mechanic:paralysis} and []{move:shadow-punch}", "30"),
                         "paralyze and shadow punch")
        self.assertEqual(build_dex.clean_prose("$effect_chance% chance", "30"), "30% chance")


if __name__ == "__main__":
    unittest.main()

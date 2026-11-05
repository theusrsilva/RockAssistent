using System.Globalization;
using System.Text;
using Microsoft.Data.Sqlite;
using TradeRock.Core.Games;

namespace TradeRock.Core.Dex;

/// <summary>
/// Consultas da Pokédex offline (SQL direto no dex.sqlite). O que serve para todas as consultas (nomes, tipos, quem
/// existe em cada jogo) fica em memória depois da primeira chamada.
/// <para>Regras (o PokeAPI não tem tudo por geração):</para>
/// <list type="bullet">
/// <item>"Existe no jogo": a espécie é de uma geração até a do jogo e tem golpes no grupo de versões, ou número numa
/// Pokédex regional do jogo, ou um encontro numa versão do jogo (DLCs incluídas).</item>
/// <item>Fraquezas: a tabela de tipos de hoje, só com os tipos que existem na geração (Aço e Noturno a partir da Gen 2,
/// Fada a partir da Gen 6). O banco não tem as tabelas antigas (Gen 1, Aço resistindo a Fantasma/Noturno até a Gen 5)
/// nem os tipos antigos de cada Pokémon (Clefairy era Normal até a Gen 5).</item>
/// <item>Golpes: tipo, força, precisão e PP da geração do jogo, pelo <c>move_changelog</c>.</item>
/// <item>Habilidades: as da geração (<c>pokemon_abilities_past</c>), nenhuma antes da Gen 3, ocultas só da Gen 5.</item>
/// </list>
/// </summary>
public sealed class DexService(DexDatabase db)
{
    private const string English = "en";
    private const int NationalDex = 1;
    private static readonly string[] StatKeys = ["hp", "attack", "defense", "special-attack", "special-defense", "speed"];
    private static readonly HashSet<string> NoBreeding = ["lgpe", "pla", "za"];

    /// <summary>Grupos de versões das DLCs, que contam como parte do jogo.</summary>
    private static readonly Dictionary<string, string[]> ExtraGroups = new()
    {
        ["swsh"] = ["the-isle-of-armor", "the-crown-tundra"],
        ["sv"] = ["the-teal-mask", "the-indigo-disk"],
        ["za"] = ["mega-dimension"],
    };

    /// <summary>Pokédex regionais de cada jogo (identificadores de <c>pokedexes</c>).</summary>
    private static readonly Dictionary<string, string[]> RegionalDexes = new()
    {
        ["rb"] = ["kanto"], ["y"] = ["kanto"], ["frlg"] = ["kanto"], ["lgpe"] = ["letsgo-kanto"],
        ["gs"] = ["original-johto"], ["c"] = ["original-johto"], ["hgss"] = ["updated-johto"],
        ["rs"] = ["hoenn"], ["e"] = ["hoenn"], ["oras"] = ["updated-hoenn"],
        ["dp"] = ["original-sinnoh"], ["pt"] = ["extended-sinnoh"], ["bdsp"] = ["original-sinnoh"],
        ["bw"] = ["original-unova"], ["b2w2"] = ["updated-unova"],
        ["xy"] = ["kalos-central", "kalos-coastal", "kalos-mountain"],
        ["sm"] = ["original-alola", "original-melemele", "original-akala", "original-ulaula", "original-poni"],
        ["usum"] = ["updated-alola", "updated-melemele", "updated-akala", "updated-ulaula", "updated-poni"],
        ["swsh"] = ["galar", "isle-of-armor", "crown-tundra"],
        ["pla"] = ["hisui"],
        ["sv"] = ["paldea", "kitakami", "blueberry"],
        ["za"] = ["lumiose-city", "hyperspace"],
    };

    private readonly Lazy<Catalog> _catalog = new(() => Catalog.Load(db), LazyThreadSafetyMode.ExecutionAndPublication);

    private Catalog Data => _catalog.Value;

    public Task<IReadOnlyList<SpeciesSummary>> ListAsync(DexQuery q, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var data = Data;
        HashSet<int>? inGame = q.GameKey is null ? null : data.Game(q.GameKey).Exists;
        var text = string.IsNullOrWhiteSpace(q.Text) ? null : Normalize(q.Text.Trim());
        int? number = text is not null && int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var n) ? n : null;

        IEnumerable<SpeciesRow> rows = data.Species.Values;
        if (inGame is not null)
            rows = rows.Where(s => inGame.Contains(s.Id));
        if (q.Generation is { } gen)
            rows = rows.Where(s => s.Generation == gen);
        if (!string.IsNullOrWhiteSpace(q.Type))
            rows = rows.Where(s => s.Types.Contains(q.Type, StringComparer.OrdinalIgnoreCase));
        if (text is not null)
            rows = rows.Where(s => s.Id == number || s.Search.Any(k => k.Contains(text, StringComparison.Ordinal)));

        IReadOnlyList<SpeciesSummary> result = rows
            .Skip(Math.Max(0, q.Offset))
            .Take(Math.Clamp(q.Limit, 1, 2000))
            .Select(s => new SpeciesSummary(s.Id, s.Name, s.Generation, s.Types, s.National))
            .ToList();
        return Task.FromResult(result);
    }

    public Task<SpeciesDetail?> GetAsync(int speciesId, string gameKey, string lang, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var data = Data;
        var game = data.Game(gameKey);
        if (!data.Species.TryGetValue(speciesId, out var row))
            return Task.FromResult<SpeciesDetail?>(null);

        using var c = db.Open();
        var exists = game.Exists.Contains(speciesId);
        var language = data.Language(lang);

        var names = new Dictionary<string, string>();
        string? genusEn = null, genusPt = null, growth = null, color = null;
        int genderRate = 0, captureRate = 0;
        int? hatch = null, happiness = null;
        using (var cmd = Command(c, "SELECT language, name FROM species_names WHERE species_id = $s", ("$s", speciesId)))
        using (var r = cmd.ExecuteReader())
            while (r.Read())
                names[r.GetString(0)] = r.GetString(1);
        using (var cmd = Command(c, """
            SELECT genus_en, genus_ptbr, gender_rate, capture_rate, hatch_counter, base_happiness, growth_rate, color
            FROM species WHERE id = $s
            """, ("$s", speciesId)))
        using (var r = cmd.ExecuteReader())
            if (r.Read())
            {
                genusEn = Str(r, 0);
                genusPt = Str(r, 1);
                genderRate = Int(r, 2) ?? -1;
                captureRate = Int(r, 3) ?? 0;
                hatch = Int(r, 4) is { } h ? (h + 1) * 255 : null; // passos do PokeAPI: (hatch_counter + 1) × 255
                happiness = Int(r, 5);
                growth = Str(r, 6);
                color = Str(r, 7);
            }

        var eggGroups = Strings(c, """
            SELECT g.identifier FROM pokemon_egg_groups pg JOIN egg_groups g ON g.id = pg.egg_group_id
            WHERE pg.species_id = $s ORDER BY g.id
            """, ("$s", speciesId));

        var forms = Forms(c, data, row, game);
        var main = forms.FirstOrDefault(f => f.IsDefault) ?? forms.First();
        var (height, weight) = HeightWeight(c, main.Id);

        var detail = new SpeciesDetail(
            speciesId,
            names.GetValueOrDefault(language) ?? row.Name,
            names,
            language == "pt-BR" ? genusPt ?? genusEn : genusEn,
            main.Types,
            height,
            weight,
            genderRate,
            captureRate,
            hatch,
            happiness,
            growth,
            color,
            eggGroups,
            exists ? Abilities(c, main.Id, game.Generation) : [],
            Chain(c, data, speciesId),
            DamageTaken(data, main.Types, game.Generation),
            exists ? Flavor(c, speciesId, game, language) : [],
            exists ? Encounters(c, speciesId, game) : [],
            exists ? Moves(c, data, main.Id, game) : SpeciesMoves.Empty,
            main.Stats,
            forms,
            DexNumbers(c, speciesId),
            exists);
        return Task.FromResult<SpeciesDetail?>(detail);
    }

    public Task<MoveDetail?> GetMoveAsync(int id, string gameKey, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var data = Data;
        var game = data.Game(gameKey);
        using var c = db.Open();
        using var cmd = Command(c, """
            SELECT identifier, name_en, type_id, power, pp, accuracy, priority, damage_class, effect_chance, effect_en, generation_id
            FROM moves WHERE id = $m
            """, ("$m", id));
        using var r = cmd.ExecuteReader();
        if (!r.Read())
            return Task.FromResult<MoveDetail?>(null);

        var identifier = r.GetString(0);
        var stats = data.MoveAt(id, game.Order, Int(r, 2) ?? 0, Int(r, 3), Int(r, 4), Int(r, 5));
        var generation = Int(r, 10) ?? 1;
        var machines = Machines(c, game.MainGroupId);
        machines.TryGetValue(id, out var machine);

        var learners = new List<MoveLearner>();
        if (generation <= game.Generation)
        {
            using var lc = Command(c, """
                SELECT DISTINCT p.species_id, pm.method, pm.level FROM pokemon_moves pm JOIN pokemon p ON p.id = pm.pokemon_id
                WHERE pm.move_id = $m AND pm.version_group_id = $vg AND p.is_default = 1
                ORDER BY p.species_id, pm.method, pm.level
                """, ("$m", id), ("$vg", game.MainGroupId));
            using var lr = lc.ExecuteReader();
            while (lr.Read())
            {
                var species = lr.GetInt32(0);
                var method = lr.GetString(1);
                learners.Add(new MoveLearner(species, data.Species.TryGetValue(species, out var s) ? s.Name : $"#{species}",
                    method, method == "level-up" ? lr.GetInt32(2) : null));
            }
        }

        var detail = new MoveDetail(id, identifier, Str(r, 1) ?? identifier, data.TypeName(stats.Type), Str(r, 7), stats.Power,
            stats.Accuracy, stats.Pp, Int(r, 6) ?? 0, Int(r, 8), Str(r, 9), generation,
            generation <= game.Generation && (learners.Count > 0 || machine is not null), machine?.Number, machine?.Name, learners);
        return Task.FromResult<MoveDetail?>(detail);
    }

    public Task<AbilityDetail?> GetAbilityAsync(int id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var data = Data;
        using var c = db.Open();
        using var cmd = Command(c, "SELECT identifier, name_en, short_effect_en, effect_en, generation_id FROM abilities WHERE id = $a",
            ("$a", id));
        using var r = cmd.ExecuteReader();
        if (!r.Read())
            return Task.FromResult<AbilityDetail?>(null);

        var holders = new List<AbilityHolder>();
        using (var hc = Command(c, """
            SELECT p.species_id, p.id, pa.is_hidden FROM pokemon_abilities pa JOIN pokemon p ON p.id = pa.pokemon_id
            WHERE pa.ability_id = $a ORDER BY p.species_id, p.is_default DESC, p.id
            """, ("$a", id)))
        using (var hr = hc.ExecuteReader())
            while (hr.Read())
            {
                var species = hr.GetInt32(0);
                holders.Add(new AbilityHolder(species, hr.GetInt32(1),
                    data.Species.TryGetValue(species, out var s) ? s.Name : $"#{species}", hr.GetInt32(2) == 1));
            }

        var identifier = r.GetString(0);
        return Task.FromResult<AbilityDetail?>(new AbilityDetail(id, identifier, Str(r, 1) ?? identifier, Str(r, 2), Str(r, 3),
            Int(r, 4) ?? 3, holders));
    }

    public Task<ItemDetail?> GetItemAsync(int id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        using var c = db.Open();
        using var cmd = Command(c, "SELECT identifier, name_en, category, cost, effect_en, fling_power FROM items WHERE id = $i",
            ("$i", id));
        using var r = cmd.ExecuteReader();
        if (!r.Read())
            return Task.FromResult<ItemDetail?>(null);
        var identifier = r.GetString(0);
        return Task.FromResult<ItemDetail?>(new ItemDetail(id, identifier, Str(r, 1) ?? identifier, Str(r, 2), Int(r, 3), Str(r, 4),
            Int(r, 5)));
    }

    /// <summary>Tabela de tipos de uma geração (1 a 9). Veja as regras na descrição da classe.</summary>
    public Task<TypeChart> GetTypeChartAsync(int generation, CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(generation, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(generation, 9);
        ct.ThrowIfCancellationRequested();
        var data = Data;
        var types = data.TypesIn(generation);
        var factors = types.ToDictionary(a => a.Name, a => (IReadOnlyDictionary<string, double>)types.ToDictionary(
            d => d.Name, d => data.Efficacy.GetValueOrDefault((a.Id, d.Id), 100) / 100.0));
        return Task.FromResult(new TypeChart(generation, types.Select(t => t.Name).ToList(), factors));
    }

    /// <summary>
    /// Como conseguir a espécie em cada jogo, na ordem: Encounter (o PokeAPI tem um encontro numa versão do jogo),
    /// Evolution (evolui de uma espécie que existe no jogo), Egg (melhor esforço: forma base, o jogo tem criação, e
    /// alguma evolução dela existe no jogo e pode botar ovo), senão None. Gift fica para os nossos dados (Task 8).
    /// </summary>
    public Task<IReadOnlyList<GameAvailability>> AvailabilityAsync(int speciesId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var data = Data;
        data.Species.TryGetValue(speciesId, out var row);
        IReadOnlyList<GameAvailability> result = GameVersions.All.Select(g =>
        {
            var game = data.Game(g.Key);
            var kind = AvailabilityKind.None;
            if (row is not null && game.Exists.Contains(speciesId))
            {
                if (game.Encounters.Contains(speciesId))
                    kind = AvailabilityKind.Encounter;
                else if (row.EvolvesFrom is { } from && game.Exists.Contains(from))
                    kind = AvailabilityKind.Evolution;
                else if (row.EvolvesFrom is null && g.Generation >= 2 && !NoBreeding.Contains(g.Key)
                         && data.Descendants(speciesId).Any(d => game.Exists.Contains(d) && data.Breedable.Contains(d)))
                    kind = AvailabilityKind.Egg;
            }
            return new GameAvailability(g.Key, g.Name, kind);
        }).ToList();
        return Task.FromResult(result);
    }

    // ---- partes da ficha ----

    private static List<SpeciesForm> Forms(SqliteConnection c, Catalog data, SpeciesRow row, GameInfo game)
    {
        var pokemon = new List<(int Id, string Identifier, bool IsDefault)>();
        using (var cmd = Command(c, "SELECT id, identifier, is_default FROM pokemon WHERE species_id = $s ORDER BY is_default DESC, form_order, id",
                   ("$s", row.Id)))
        using (var r = cmd.ExecuteReader())
            while (r.Read())
                pokemon.Add((r.GetInt32(0), r.GetString(1), r.GetInt32(2) == 1));

        var ids = string.Join(",", pokemon.Select(p => p.Id));
        var formInfo = new Dictionary<int, (string? Name, bool Mega, int? Introduced)>();
        using (var cmd = Command(c, $"SELECT pokemon_id, form_name_en, is_mega, introduced_in_version_group FROM pokemon_forms WHERE pokemon_id IN ({ids}) ORDER BY id"))
        using (var r = cmd.ExecuteReader())
            while (r.Read())
                formInfo.TryAdd(r.GetInt32(0), (Str(r, 1), r.GetInt32(2) == 1, Int(r, 3)));

        var types = new Dictionary<int, List<string>>();
        using (var cmd = Command(c, $"SELECT pokemon_id, type_id FROM pokemon_types WHERE pokemon_id IN ({ids}) ORDER BY pokemon_id, slot"))
        using (var r = cmd.ExecuteReader())
            while (r.Read())
                types.GetOrAdd(r.GetInt32(0)).Add(data.TypeName(r.GetInt32(1)));

        var stats = new Dictionary<int, Dictionary<int, (int Base, int Effort)>>();
        using (var cmd = Command(c, $"SELECT pokemon_id, stat_id, base_stat, effort FROM pokemon_stats WHERE pokemon_id IN ({ids})"))
        using (var r = cmd.ExecuteReader())
            while (r.Read())
            {
                if (!stats.TryGetValue(r.GetInt32(0), out var map))
                    stats[r.GetInt32(0)] = map = [];
                map[r.GetInt32(1)] = (r.GetInt32(2), r.GetInt32(3));
            }

        var forms = new List<SpeciesForm>();
        foreach (var p in pokemon)
        {
            var info = formInfo.GetValueOrDefault(p.Id);
            // Formas que só chegaram depois do jogo (a Mega do Gengar no FRLG) ficam de fora; a padrão sempre fica.
            if (!p.IsDefault && info.Introduced is { } vg && data.GroupOrder(vg) > game.Order)
                continue;
            forms.Add(new SpeciesForm(p.Id, p.Identifier, info.Name ?? row.Name, [.. types.GetValueOrDefault(p.Id) ?? []],
                Stats(stats.GetValueOrDefault(p.Id)), info.Mega, p.IsDefault));
        }
        return forms;
    }

    private static BaseStats Stats(Dictionary<int, (int Base, int Effort)>? map)
    {
        if (map is null)
            return BaseStats.Empty;
        int B(int id) => map.TryGetValue(id, out var v) ? v.Base : 0;
        var evs = new Dictionary<string, int>();
        for (var i = 0; i < StatKeys.Length; i++)
            if (map.TryGetValue(i + 1, out var v) && v.Effort > 0)
                evs[StatKeys[i]] = v.Effort;
        return new BaseStats(B(1), B(2), B(3), B(4), B(5), B(6), Enumerable.Range(1, 6).Sum(B), evs);
    }

    private static (int Height, int Weight) HeightWeight(SqliteConnection c, int pokemonId)
    {
        using var cmd = Command(c, "SELECT height, weight FROM pokemon WHERE id = $p", ("$p", pokemonId));
        using var r = cmd.ExecuteReader();
        return r.Read() ? (Int(r, 0) ?? 0, Int(r, 1) ?? 0) : (0, 0);
    }

    private static List<SpeciesAbility> Abilities(SqliteConnection c, int pokemonId, int generation)
    {
        if (generation < 3)
            return [];
        var slots = new SortedDictionary<int, (int? Ability, bool Hidden)>();
        using (var cmd = Command(c, "SELECT slot, ability_id, is_hidden FROM pokemon_abilities WHERE pokemon_id = $p", ("$p", pokemonId)))
        using (var r = cmd.ExecuteReader())
            while (r.Read())
                slots[r.GetInt32(0)] = (r.GetInt32(1), r.GetInt32(2) == 1);
        // pokemon_abilities_past: a linha de geração G vale para os jogos até a G; por espaço, vence a menor G >= a do jogo.
        var overridden = new HashSet<int>();
        using (var cmd = Command(c, """
            SELECT slot, ability_id, is_hidden FROM pokemon_abilities_past
            WHERE pokemon_id = $p AND generation_id >= $g ORDER BY generation_id
            """, ("$p", pokemonId), ("$g", generation)))
        using (var r = cmd.ExecuteReader())
            while (r.Read())
                if (overridden.Add(r.GetInt32(0)))
                    slots[r.GetInt32(0)] = (Int(r, 1), r.GetInt32(2) == 1);

        var result = new List<SpeciesAbility>();
        foreach (var (ability, hidden) in slots.Values)
        {
            if (ability is null || (hidden && generation < 5))
                continue;
            using var cmd = Command(c, "SELECT identifier, name_en, short_effect_en, generation_id FROM abilities WHERE id = $a", ("$a", ability));
            using var r = cmd.ExecuteReader();
            if (r.Read() && (Int(r, 3) ?? 3) <= generation)
                result.Add(new SpeciesAbility(ability.Value, Str(r, 1) ?? r.GetString(0), hidden, Str(r, 2)));
        }
        return result;
    }

    private static List<EvolutionStep> Chain(SqliteConnection c, Catalog data, int speciesId)
    {
        var chainId = data.Species[speciesId].ChainId;
        var members = data.Species.Values.Where(s => chainId is not null && s.ChainId == chainId).ToList();
        if (members.Count == 0)
            return [];
        var ids = members.Select(m => m.Id).ToHashSet();
        var ordered = new List<SpeciesRow>();
        var queue = new Queue<SpeciesRow>(members.Where(m => m.EvolvesFrom is null || !ids.Contains(m.EvolvesFrom.Value)));
        while (queue.Count > 0)
        {
            var s = queue.Dequeue();
            ordered.Add(s);
            foreach (var child in members.Where(m => m.EvolvesFrom == s.Id))
                queue.Enqueue(child);
        }

        var conditions = new Dictionary<int, List<string>>();
        using (var cmd = Command(c, $"""
            SELECT evolved_species_id, evolution_trigger, trigger_item_id, minimum_level, gender_id, location_id, held_item_id,
                   time_of_day, known_move_id, known_move_type_id, minimum_happiness, minimum_beauty, minimum_affection,
                   relative_physical_stats, party_species_id, party_type_id, trade_species_id, needs_overworld_rain, turn_upside_down
            FROM evolution WHERE evolved_species_id IN ({string.Join(",", ids)})
            """))
        using (var r = cmd.ExecuteReader())
            while (r.Read())
            {
                var text = Condition(c, data, r);
                var list = conditions.GetOrAdd(r.GetInt32(0));
                if (!list.Contains(text))
                    list.Add(text);
            }

        return ordered.Select(s => new EvolutionStep(s.Id, s.Name, s.EvolvesFrom is { } f && ids.Contains(f) ? f : null,
            conditions.GetValueOrDefault(s.Id) ?? [])).ToList();
    }

    /// <summary>Frase curta, em inglês, para uma linha de <c>evolution</c>.</summary>
    private static string Condition(SqliteConnection c, Catalog data, SqliteDataReader r)
    {
        var trigger = Str(r, 1) ?? "level-up";
        var parts = new List<string>();
        switch (trigger)
        {
            case "level-up":
                parts.Add(Int(r, 3) is { } level ? $"level {level}" : "level up");
                break;
            case "trade":
                parts.Add("trade");
                break;
            case "use-item":
                parts.Add($"use {Name(c, "items", Int(r, 2))}");
                break;
            default:
                parts.Add(trigger.Replace('-', ' '));
                if (Int(r, 3) is { } min)
                    parts.Add($"level {min}");
                break;
        }
        if (trigger != "use-item" && Int(r, 2) is { } item)
            parts.Add($"with {Name(c, "items", item)}");
        if (Int(r, 4) is { } gender)
            parts.Add(gender == 1 ? "female" : "male");
        if (Int(r, 6) is { } held)
            parts.Add($"holding {Name(c, "items", held)}");
        if (Str(r, 7) is { Length: > 0 } time)
            parts.Add($"at {time}");
        if (Int(r, 8) is { } move)
            parts.Add($"knowing {Name(c, "moves", move)}");
        if (Int(r, 9) is { } moveType)
            parts.Add($"knowing a {data.TypeName(moveType)} move");
        if (Int(r, 10) is { } happiness)
            parts.Add($"friendship {happiness}");
        if (Int(r, 11) is { } beauty)
            parts.Add($"beauty {beauty}");
        if (Int(r, 12) is { } affection)
            parts.Add($"affection {affection}");
        if (Int(r, 13) is { } relative)
            parts.Add(relative switch { 1 => "Attack > Defense", -1 => "Attack < Defense", _ => "Attack = Defense" });
        if (Int(r, 14) is { } party)
            parts.Add($"with {(data.Species.TryGetValue(party, out var p) ? p.Name : $"#{party}")} in party");
        if (Int(r, 15) is { } partyType)
            parts.Add($"with a {data.TypeName(partyType)} type in party");
        if (Int(r, 16) is { } tradeFor)
            parts.Add($"for {(data.Species.TryGetValue(tradeFor, out var t) ? t.Name : $"#{tradeFor}")}");
        if (Int(r, 5) is { } location)
            parts.Add($"at {Name(c, "locations", location)}");
        if (Int(r, 17) == 1)
            parts.Add("in rain");
        if (Int(r, 18) == 1)
            parts.Add("upside down");
        return string.Join(" ", parts);
    }

    private static string Name(SqliteConnection c, string table, int? id)
    {
        using var cmd = Command(c, $"SELECT coalesce(name_en, identifier) FROM {table} WHERE id = $i", ("$i", id));
        return cmd.ExecuteScalar() as string ?? $"#{id}";
    }

    private static Dictionary<string, double> DamageTaken(Catalog data, string[] defender, int generation)
    {
        var defenderIds = defender.Select(data.TypeId).OfType<int>().ToList();
        return data.TypesIn(generation).ToDictionary(t => t.Name,
            t => defenderIds.Aggregate(1.0, (acc, d) => acc * data.Efficacy.GetValueOrDefault((t.Id, d), 100) / 100.0));
    }

    private static List<FlavorText> Flavor(SqliteConnection c, int speciesId, GameInfo game, string language)
    {
        var texts = new Dictionary<(int Version, string Lang), string>();
        using (var cmd = Command(c, $"SELECT version_id, language, text FROM flavor_text WHERE species_id = $s AND version_id IN ({game.VersionIdList})",
                   ("$s", speciesId)))
        using (var r = cmd.ExecuteReader())
            while (r.Read())
                texts.TryAdd((r.GetInt32(0), r.GetString(1)), r.GetString(2));

        var result = new List<FlavorText>();
        foreach (var (id, identifier) in game.Versions)
        {
            if (texts.TryGetValue((id, language), out var text))
                result.Add(new FlavorText(identifier, language, text));
            else if (texts.TryGetValue((id, English), out text))
                result.Add(new FlavorText(identifier, English, text));
        }
        return result;
    }

    private static List<EncounterInfo> Encounters(SqliteConnection c, int speciesId, GameInfo game)
    {
        var result = new List<EncounterInfo>();
        using var cmd = Command(c, $"""
            SELECT v.identifier, coalesce(l.name_en, l.identifier), a.name_en, e.method, min(e.min_level), max(e.max_level),
                   sum(coalesce(e.rarity, 0)), e.condition_values
            FROM encounters e
            JOIN pokemon p ON p.id = e.pokemon_id
            JOIN versions v ON v.id = e.version_id
            JOIN location_areas a ON a.id = e.location_area_id
            JOIN locations l ON l.id = a.location_id
            WHERE p.species_id = $s AND e.version_id IN ({game.VersionIdList})
            GROUP BY e.version_id, e.location_area_id, e.method, e.condition_values
            ORDER BY e.version_id, l.id, a.id, e.method
            """, ("$s", speciesId));
        using var r = cmd.ExecuteReader();
        while (r.Read())
            result.Add(new EncounterInfo(r.GetString(0), r.GetString(1), Str(r, 2), Str(r, 3) ?? "unknown", Int(r, 4) ?? 0,
                Int(r, 5) ?? 0, Int(r, 6) ?? 0, Str(r, 7)?.Split(',') ?? []));
        return result;
    }

    private static SpeciesMoves Moves(SqliteConnection c, Catalog data, int pokemonId, GameInfo game)
    {
        var machines = Machines(c, game.MainGroupId);
        var groups = new Dictionary<string, List<LearnedMove>>();
        using var cmd = Command(c, """
            SELECT pm.move_id, pm.method, pm.level, coalesce(m.name_en, m.identifier), m.type_id, m.power, m.pp, m.accuracy, m.damage_class
            FROM pokemon_moves pm JOIN moves m ON m.id = pm.move_id
            WHERE pm.pokemon_id = $p AND pm.version_group_id = $vg
            ORDER BY pm.method, pm.level, pm."order", m.name_en
            """, ("$p", pokemonId), ("$vg", game.MainGroupId));
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            var id = r.GetInt32(0);
            var method = r.GetString(1);
            var stats = data.MoveAt(id, game.Order, Int(r, 4) ?? 0, Int(r, 5), Int(r, 6), Int(r, 7));
            machines.TryGetValue(id, out var machine);
            var isMachine = method == "machine";
            groups.GetOrAdd(method).Add(new LearnedMove(id, r.GetString(3), data.TypeName(stats.Type), Str(r, 8), stats.Power,
                stats.Accuracy, stats.Pp, method == "level-up" ? r.GetInt32(2) : null,
                isMachine ? machine?.Number : null, isMachine ? machine?.Name : null));
        }
        List<LearnedMove> Get(string m) => groups.GetValueOrDefault(m) ?? [];
        return new SpeciesMoves(Get("level-up"), [.. Get("machine").OrderBy(m => m.MachineNumber ?? int.MaxValue)], Get("tutor"), Get("egg"));
    }

    private sealed record Machine(int Number, string Name);

    private static Dictionary<int, Machine> Machines(SqliteConnection c, int versionGroupId)
    {
        var result = new Dictionary<int, Machine>();
        using var cmd = Command(c, """
            SELECT mc.move_id, mc.machine_number, coalesce(i.name_en, i.identifier) FROM machines mc JOIN items i ON i.id = mc.item_id
            WHERE mc.version_group_id = $vg ORDER BY mc.machine_number
            """, ("$vg", versionGroupId));
        using var r = cmd.ExecuteReader();
        while (r.Read())
            result.TryAdd(r.GetInt32(0), new Machine(Int(r, 1) ?? 0, r.GetString(2)));
        return result;
    }

    private static Dictionary<string, int> DexNumbers(SqliteConnection c, int speciesId)
    {
        var result = new Dictionary<string, int>();
        using var cmd = Command(c, """
            SELECT d.identifier, n.number FROM pokemon_dex_numbers n JOIN pokedexes d ON d.id = n.pokedex_id
            WHERE n.species_id = $s ORDER BY d.id
            """, ("$s", speciesId));
        using var r = cmd.ExecuteReader();
        while (r.Read())
            result.TryAdd(r.GetString(0), r.GetInt32(1));
        return result;
    }

    // ---- utilitários ----

    private static SqliteCommand Command(SqliteConnection c, string sql, params (string Name, object? Value)[] args)
    {
        var cmd = c.CreateCommand();
        cmd.CommandText = sql;
        foreach (var (name, value) in args)
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        return cmd;
    }

    private static string[] Strings(SqliteConnection c, string sql, params (string, object?)[] args)
    {
        using var cmd = Command(c, sql, args);
        using var r = cmd.ExecuteReader();
        var list = new List<string>();
        while (r.Read())
            list.Add(r.GetString(0));
        return [.. list];
    }

    private static int? Int(SqliteDataReader r, int i) => r.IsDBNull(i) ? null : r.GetInt32(i);

    private static string? Str(SqliteDataReader r, int i) => r.IsDBNull(i) ? null : r.GetString(i);

    /// <summary>Minúsculas e sem acento, para a busca ("Flabébé" acha com "flabebe").</summary>
    internal static string Normalize(string text)
    {
        var decomposed = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);
        foreach (var ch in decomposed)
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    // ---- dados em memória ----

    private sealed record SpeciesRow(int Id, string Name, int Generation, int? EvolvesFrom, int? ChainId, string[] Types,
        int? National, string[] Search);

    private sealed record GameInfo(string Key, int Generation, int MainGroupId, int Order,
        IReadOnlyList<(int Id, string Identifier)> Versions, HashSet<int> Exists, HashSet<int> Encounters)
    {
        public string VersionIdList { get; } = string.Join(",", Versions.Select(v => v.Id));
    }

    private sealed record MoveStats(int Type, int? Power, int? Pp, int? Accuracy);

    private sealed class Catalog
    {
        public required SortedDictionary<int, SpeciesRow> Species { get; init; }
        public required Dictionary<int, (string Name, int Generation)> Types { get; init; }
        public required Dictionary<(int Attack, int Defense), int> Efficacy { get; init; }
        public required Dictionary<string, GameInfo> Games { get; init; }
        public required HashSet<int> Breedable { get; init; }
        public required HashSet<string> Languages { get; init; }
        private Dictionary<int, int> GroupOrders { get; init; } = [];
        private Dictionary<int, List<(int Order, int? Type, int? Power, int? Pp, int? Accuracy)>> Changelog { get; init; } = [];
        private Dictionary<string, int> TypeIds { get; init; } = [];

        public GameInfo Game(string key) =>
            Games.TryGetValue(key, out var g) ? g : throw new KeyNotFoundException($"Jogo desconhecido: {key}");

        public string TypeName(int id) => Types.TryGetValue(id, out var t) ? t.Name : "unknown";

        public int? TypeId(string name) => TypeIds.TryGetValue(name, out var id) ? id : null;

        public int GroupOrder(int versionGroupId) => GroupOrders.GetValueOrDefault(versionGroupId);

        public List<(int Id, string Name)> TypesIn(int generation) =>
            [.. Types.Where(t => t.Value.Generation <= generation).OrderBy(t => t.Key).Select(t => (t.Key, t.Value.Name))];

        /// <summary>A língua pedida se o banco tem nomes nela ("en-US" vira "en"); senão inglês.</summary>
        public string Language(string? lang)
        {
            if (string.IsNullOrWhiteSpace(lang))
                return English;
            if (lang == "pt-BR" || Languages.Contains(lang))
                return lang;
            var match = Languages.FirstOrDefault(l => string.Equals(l, lang, StringComparison.OrdinalIgnoreCase))
                        ?? Languages.FirstOrDefault(l => string.Equals(l, lang.Split('-')[0], StringComparison.OrdinalIgnoreCase));
            return match ?? (lang.StartsWith("pt", StringComparison.OrdinalIgnoreCase) ? "pt-BR" : English);
        }

        /// <summary>
        /// Tipo, força, PP e precisão no jogo de ordem <paramref name="order"/>: cada linha do <c>move_changelog</c>
        /// guarda os valores de antes da mudança; vale, campo a campo, a primeira mudança depois do jogo.
        /// </summary>
        public MoveStats MoveAt(int moveId, int order, int type, int? power, int? pp, int? accuracy)
        {
            if (!Changelog.TryGetValue(moveId, out var changes))
                return new MoveStats(type, power, pp, accuracy);
            int? t = null, p = null, q = null, a = null;
            foreach (var ch in changes.Where(ch => ch.Order > order)) // já em ordem crescente
            {
                t ??= ch.Type;
                p ??= ch.Power;
                q ??= ch.Pp;
                a ??= ch.Accuracy;
            }
            return new MoveStats(t ?? type, p ?? power, q ?? pp, a ?? accuracy);
        }

        public IEnumerable<int> Descendants(int speciesId)
        {
            var queue = new Queue<int>([speciesId]);
            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();
                foreach (var child in Species.Values.Where(s => s.EvolvesFrom == parent).Select(s => s.Id).ToList())
                {
                    yield return child;
                    queue.Enqueue(child);
                }
            }
        }

        public static Catalog Load(DexDatabase db)
        {
            using var c = db.Open();

            var types = new Dictionary<int, (string, int)>();
            using (var cmd = Command(c, "SELECT id, identifier, generation_id FROM types WHERE id < 10000 AND identifier <> 'stellar'"))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    types[r.GetInt32(0)] = (r.GetString(1), r.GetInt32(2));

            var efficacy = new Dictionary<(int, int), int>();
            using (var cmd = Command(c, "SELECT damage_type_id, target_type_id, damage_factor FROM type_efficacy"))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    efficacy[(r.GetInt32(0), r.GetInt32(1))] = r.GetInt32(2);

            var groups = new Dictionary<string, (int Id, int Order)>();
            var groupOrders = new Dictionary<int, int>();
            using (var cmd = Command(c, "SELECT id, identifier, \"order\" FROM version_groups"))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                {
                    groups[r.GetString(1)] = (r.GetInt32(0), r.GetInt32(2));
                    groupOrders[r.GetInt32(0)] = r.GetInt32(2);
                }

            var versions = new List<(int Id, int Group, string Identifier)>();
            using (var cmd = Command(c, "SELECT id, version_group_id, identifier FROM versions ORDER BY id"))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    versions.Add((r.GetInt32(0), r.GetInt32(1), r.GetString(2)));

            var names = new Dictionary<int, List<string>>();
            var english = new Dictionary<int, string>();
            var languages = new HashSet<string>();
            using (var cmd = Command(c, "SELECT species_id, language, name FROM species_names"))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                {
                    names.GetOrAdd(r.GetInt32(0)).Add(Normalize(r.GetString(2)));
                    languages.Add(r.GetString(1));
                    if (r.GetString(1) == English)
                        english[r.GetInt32(0)] = r.GetString(2);
                }

            var speciesTypes = new Dictionary<int, List<string>>();
            using (var cmd = Command(c, """
                SELECT p.species_id, pt.type_id FROM pokemon p JOIN pokemon_types pt ON pt.pokemon_id = p.id
                WHERE p.is_default = 1 ORDER BY p.species_id, pt.slot
                """))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    speciesTypes.GetOrAdd(r.GetInt32(0)).Add(types.TryGetValue(r.GetInt32(1), out var t) ? t.Item1 : "unknown");

            var national = new Dictionary<int, int>();
            using (var cmd = Command(c, "SELECT species_id, number FROM pokemon_dex_numbers WHERE pokedex_id = $d", ("$d", NationalDex)))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    national[r.GetInt32(0)] = r.GetInt32(1);

            var species = new SortedDictionary<int, SpeciesRow>();
            using (var cmd = Command(c, "SELECT id, identifier, generation_id, evolves_from_species_id, evolution_chain_id FROM species"))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                {
                    var id = r.GetInt32(0);
                    var identifier = r.GetString(1);
                    species[id] = new SpeciesRow(id, english.GetValueOrDefault(id) ?? identifier, r.GetInt32(2), Int(r, 3), Int(r, 4),
                        [.. speciesTypes.GetValueOrDefault(id) ?? []], national.TryGetValue(id, out var n) ? n : null,
                        [Normalize(identifier), .. names.GetValueOrDefault(id) ?? []]);
                }

            // Quem existe em cada grupo de versões (golpes e encontros) e em cada Pokédex regional.
            var byGroup = Pairs(c, "SELECT DISTINCT p.species_id, pm.version_group_id FROM pokemon_moves pm JOIN pokemon p ON p.id = pm.pokemon_id");
            var encounterByGroup = Pairs(c, """
                SELECT DISTINCT p.species_id, v.version_group_id FROM encounters e
                JOIN pokemon p ON p.id = e.pokemon_id JOIN versions v ON v.id = e.version_id
                """);
            var byDex = new Dictionary<string, HashSet<int>>();
            using (var cmd = Command(c, "SELECT d.identifier, n.species_id FROM pokemon_dex_numbers n JOIN pokedexes d ON d.id = n.pokedex_id"))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                {
                    if (!byDex.TryGetValue(r.GetString(0), out var set))
                        byDex[r.GetString(0)] = set = [];
                    set.Add(r.GetInt32(1));
                }

            var games = new Dictionary<string, GameInfo>();
            foreach (var g in GameVersions.All)
            {
                var groupNames = new[] { g.PokeApiVersionGroup }.Concat(ExtraGroups.GetValueOrDefault(g.Key) ?? []).ToList();
                var groupIds = groupNames.Select(n => groups.TryGetValue(n, out var vg)
                    ? vg.Id
                    : throw new InvalidOperationException($"Grupo de versões ausente no dex.sqlite: {n}")).ToList();
                var gameVersions = groupIds.SelectMany(id => versions.Where(v => v.Group == id))
                    .OrderBy(v => groupIds.IndexOf(v.Group)).ThenBy(v => v.Id).Select(v => (v.Id, v.Identifier)).ToList();

                var encounters = groupIds.SelectMany(id => encounterByGroup.GetValueOrDefault(id) ?? []).ToHashSet();
                var exists = groupIds.SelectMany(id => byGroup.GetValueOrDefault(id) ?? [])
                    .Concat(encounters)
                    .Concat(RegionalDexes[g.Key].SelectMany(d => byDex.TryGetValue(d, out var set)
                        ? set
                        : throw new InvalidOperationException($"Pokédex ausente no dex.sqlite: {d}")))
                    .Where(id => species.TryGetValue(id, out var s) && s.Generation <= g.Generation)
                    .ToHashSet();
                encounters.IntersectWith(exists);
                games[g.Key] = new GameInfo(g.Key, g.Generation, groupIds[0], groups[g.PokeApiVersionGroup].Order, gameVersions,
                    exists, encounters);
            }

            var breedable = new HashSet<int>();
            using (var cmd = Command(c, """
                SELECT DISTINCT pg.species_id FROM pokemon_egg_groups pg JOIN egg_groups g ON g.id = pg.egg_group_id
                WHERE g.identifier <> 'no-eggs'
                """))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    breedable.Add(r.GetInt32(0));

            var changelog = new Dictionary<int, List<(int, int?, int?, int?, int?)>>();
            using (var cmd = Command(c, "SELECT move_id, changed_in_version_group_id, type_id, power, pp, accuracy FROM move_changelog"))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    changelog.GetOrAdd(r.GetInt32(0)).Add((groupOrders.GetValueOrDefault(r.GetInt32(1)), Int(r, 2), Int(r, 3), Int(r, 4), Int(r, 5)));
            foreach (var list in changelog.Values)
                list.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            return new Catalog
            {
                Species = species,
                Types = types,
                Efficacy = efficacy,
                Games = games,
                Breedable = breedable,
                Languages = languages,
                GroupOrders = groupOrders,
                Changelog = changelog,
                TypeIds = types.ToDictionary(t => t.Value.Item1, t => t.Key),
            };
        }

        private static Dictionary<int, HashSet<int>> Pairs(SqliteConnection c, string sql)
        {
            var result = new Dictionary<int, HashSet<int>>();
            using var cmd = Command(c, sql);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                if (!result.TryGetValue(r.GetInt32(1), out var set))
                    result[r.GetInt32(1)] = set = [];
                set.Add(r.GetInt32(0));
            }
            return result;
        }
    }
}

internal static class DexDictionaryExtensions
{
    public static List<TValue> GetOrAdd<TKey, TValue>(this Dictionary<TKey, List<TValue>> map, TKey key) where TKey : notnull
    {
        if (!map.TryGetValue(key, out var list))
            map[key] = list = [];
        return list;
    }
}

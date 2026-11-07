using TradeRock.Core.Games;
using TradeRock.Core.Hunts;

namespace TradeRock.Core.GameData;

/// <summary>
/// Confere os dados dos jogos: espécies de 1 a 1025, golpes que existem no dex, áreas, chefes e nós de mapa que
/// existem, jogos e versões do <see cref="GameVersions"/> e os vocabulários fechados (tipo, método, raridade).
/// </summary>
public static class GameDataValidator
{
    public const int MaxSpecies = 1025;

    public static readonly IReadOnlySet<string> BossKinds = new HashSet<string> { "gym", "rival", "evil", "elite", "champion", "other" };
    public static readonly IReadOnlySet<string> AreaKinds = new HashSet<string> { "route", "city", "dungeon", "building", "sea" };
    public static readonly IReadOnlySet<string> Chances = new HashSet<string> { "common", "uncommon", "rare", "fixed" };
    public static readonly IReadOnlySet<string> ModifierKinds = new HashSet<string> { "toggle", "steps" };

    public static readonly IReadOnlySet<string> EncounterMethods = new HashSet<string>
    {
        "walk", "surf", "fish", "tree", "city", "horde", "dexnav", "sos", "max_raid", "tera_raid", "honey", "radar",
        "roaming", "rock_smash", "cave", "outbreak", "raid", "fixed", "gift", "trade",
    };

    public static IReadOnlyList<string> Validate(GameDataStore store, Func<string, bool>? moveExists)
    {
        var errors = new List<string>();
        ShinyMethods(store.ShinyMethods, errors);
        foreach (var map in store.Maps)
            Map(map, errors);
        foreach (var game in store.GameKeys.OrderBy(k => k))
            Game(store, GameVersions.ByKey(game), moveExists, errors);
        return errors;
    }

    private static void ShinyMethods(IReadOnlyList<ShinyMethod> methods, List<string> errors)
    {
        foreach (var dup in Duplicates(methods.Select(m => m.Id)))
            errors.Add($"shiny-methods: id repetido {dup}");
        foreach (var m in methods)
        {
            var at = $"shiny-methods/{m.Id}";
            if (string.IsNullOrWhiteSpace(m.Name) || string.IsNullOrWhiteSpace(m.NameEn))
                errors.Add($"{at}: name e name_en são obrigatórios");
            if (m.BaseOdds < 1)
                errors.Add($"{at}: base_odds {m.BaseOdds} (o denominador tem de ser >= 1)");
            if (m.Games.Count == 0)
                errors.Add($"{at}: sem jogos");
            foreach (var g in m.Games.Where(g => !IsGame(g)))
                errors.Add($"{at}: jogo desconhecido {g}");
            foreach (var dup in Duplicates(m.Modifiers.Select(x => x.Id)))
                errors.Add($"{at}: modificador repetido {dup}");
            var rolls = m.BaseRolls is not null;
            if (rolls != m.RollOdds is not null)
                errors.Add($"{at}: base_rolls e roll_odds vêm juntos (ou os dois null)");
            else if (rolls && (m.BaseRolls < 1 || m.RollOdds < 1))
                errors.Add($"{at}: base_rolls e roll_odds precisam ser >= 1");
            else if (rolls)
                SameOdds(at, m.BaseOdds, m, 0, errors);
            if (m.Modifiers.Count(x => x.Counter) > 1)
                errors.Add($"{at}: mais de um modificador com counter");
            foreach (var mod in m.Modifiers)
            {
                var mat = $"{at}/{mod.Id}";
                if (string.IsNullOrWhiteSpace(mod.Name) || string.IsNullOrWhiteSpace(mod.NameEn))
                    errors.Add($"{mat}: name e name_en são obrigatórios");
                if (!ModifierKinds.Contains(mod.Kind))
                    errors.Add($"{mat}: kind {mod.Kind} (toggle ou steps)");
                else if (mod.Kind == "toggle" && (mod.Odds is not >= 1 || mod.Steps is not null || mod.Counter))
                    errors.Add($"{mat}: toggle precisa de odds >= 1, steps null e counter false");
                else if (mod.Kind == "steps" && (mod.Steps is not { Count: > 0 } || mod.Odds is not null || mod.Rolls is not null))
                    errors.Add($"{mat}: steps precisa de degraus, odds null e rolls null (as rolagens ficam nos degraus)");
                foreach (var g in (mod.Games ?? []).Where(g => !m.Games.Contains(g)))
                    errors.Add($"{mat}: jogo {g} não é do método");
                if (mod.Kind == "toggle" && rolls != mod.Rolls is not null)
                    errors.Add($"{mat}: rolls {(rolls ? "obrigatório (método por rolagens)" : "tem de ser null (método sem rolagens)")}");
                else if (mod.Kind == "toggle" && mod.Rolls is { } r && mod.Odds is { } o)
                    SameOdds(mat, o, m, r, errors);
                foreach (var st in mod.Steps ?? [])
                {
                    if (st.Odds < 1 || st.Min < 0 || st.Max < st.Min)
                        errors.Add($"{mat}: degrau {st.Label} inválido");
                    if (rolls != st.Rolls is not null)
                        errors.Add($"{mat}: degrau {st.Label}: rolls {(rolls ? "obrigatório" : "tem de ser null")}");
                    else if (st.Rolls is { } sr)
                        SameOdds($"{mat}/{st.Label}", st.Odds, m, sr, errors);
                }
            }
        }
    }

    /// <summary>A odd escrita tem de bater (0,5%) com a conta das rolagens.</summary>
    private static void SameOdds(string at, double odds, ShinyMethod m, int extra, List<string> errors)
    {
        if (extra < 0)
        {
            errors.Add($"{at}: rolls negativo");
            return;
        }
        var expected = ShinyOdds.FromRolls(m.RollOdds!.Value, m.BaseRolls!.Value + extra);
        if (Math.Abs(odds - expected) / expected > 0.005)
            errors.Add($"{at}: odds {odds} não batem com as rolagens ({m.BaseRolls} + {extra} de 1/{m.RollOdds} = 1/{expected:0.##})");
    }

    private static void Map(MapGraph map, List<string> errors)
    {
        var at = $"maps/{map.Region}";
        foreach (var dup in Duplicates(map.Nodes.Select(n => n.Id)))
            errors.Add($"{at}: nó repetido {dup}");
        var ids = map.Nodes.Select(n => n.Id).ToHashSet();
        foreach (var e in map.Edges)
            foreach (var end in new[] { e.From, e.To }.Where(x => !ids.Contains(x)))
                errors.Add($"{at}: aresta {e.From}->{e.To} aponta para o nó {end}, que não existe");
        foreach (var n in map.Nodes)
        {
            if (n.X is < 0 or > 1 || n.Y is < 0 or > 1)
                errors.Add($"{at}/{n.Id}: x e y vão de 0 a 1");
            foreach (var g in (n.Games ?? []).Where(g => !IsGame(g)))
                errors.Add($"{at}/{n.Id}: jogo desconhecido {g}");
        }
    }

    private static void Game(GameDataStore store, GameVersionGroup game, Func<string, bool>? moveExists, List<string> errors)
    {
        var key = game.Key;
        var bosses = store.Bosses(key);
        var areas = store.Areas(key);
        var bossIds = bosses.Select(b => b.Id).ToHashSet();
        var areaIds = areas.Select(a => a.Id).ToHashSet();
        var map = store.Map(game.Region);

        foreach (var dup in Duplicates(bosses.Select(b => b.Id)))
            errors.Add($"bosses/{key}: id repetido {dup}");
        foreach (var b in bosses)
        {
            var at = $"bosses/{key}/{b.Id}";
            if (!BossKinds.Contains(b.Kind))
                errors.Add($"{at}: kind {b.Kind}");
            if (b.Party.Count == 0)
                errors.Add($"{at}: time vazio");
            else if (b.LevelCap != b.Party.Max(p => p.Level))
                errors.Add($"{at}: level_cap {b.LevelCap} diferente do maior nível do time ({b.Party.Max(p => p.Level)})");
            foreach (var v in (b.Versions ?? []).Where(v => !game.PokeApiVersions.Contains(v)))
                errors.Add($"{at}: versão {v} não é do {key}");
            Party(at, b.Party, moveExists, errors);
        }

        var trainers = store.Trainers(key);
        foreach (var dup in Duplicates(trainers.Select(t => t.Id)))
            errors.Add($"trainers/{key}: id repetido {dup}");
        foreach (var t in trainers)
        {
            var at = $"trainers/{key}/{t.Id}";
            if (!areaIds.Contains(t.Area))
                errors.Add($"{at}: área {t.Area} não existe em areas/{key}");
            if (t.Party.Count == 0)
                errors.Add($"{at}: time vazio");
            foreach (var v in (t.Versions ?? []).Where(v => !game.PokeApiVersions.Contains(v)))
                errors.Add($"{at}: versão {v} não é do {key}");
            Party(at, t.Party, moveExists, errors);
        }

        foreach (var dup in Duplicates(areas.Select(a => a.Id)))
            errors.Add($"areas/{key}: id repetido {dup}");
        foreach (var a in areas)
        {
            var at = $"areas/{key}/{a.Id}";
            if (!AreaKinds.Contains(a.Kind))
                errors.Add($"{at}: kind {a.Kind}");
            foreach (var id in a.BossesBefore.Where(id => !bossIds.Contains(id)))
                errors.Add($"{at}: chefe {id} não existe em bosses/{key}");
            if (a.MapNode is { } node)
            {
                var n = map?.Nodes.FirstOrDefault(x => x.Id == node);
                if (n is null)
                    errors.Add($"{at}: nó {node} não existe no mapa {game.Region}");
                else if (n.Games is { } games && !games.Contains(key))
                    errors.Add($"{at}: nó {node} não é deste jogo");
            }
        }

        var encounters = store.Encounters(key);
        for (var i = 0; i < encounters.Count; i++)
        {
            var e = encounters[i];
            var at = $"encounters/{key}[{i}]";
            if (!areaIds.Contains(e.Area))
                errors.Add($"{at}: área {e.Area} não existe em areas/{key}");
            Species(at, e.Species, e.Form, errors);
            if (!EncounterMethods.Contains(e.Method))
                errors.Add($"{at}: método {e.Method} fora da lista");
            if (e.MinLevel is < 1 or > 100 || e.MaxLevel < e.MinLevel || e.MaxLevel > 100)
                errors.Add($"{at}: níveis {e.MinLevel}-{e.MaxLevel}");
            if (e.Chance is { } c && !Chances.Contains(c))
                errors.Add($"{at}: chance {c}");
            foreach (var v in e.Versions.Where(v => !game.PokeApiVersions.Contains(v)))
                errors.Add($"{at}: versão {v} não é do {key}");
        }
    }

    private static void Party(string at, IReadOnlyList<BossPokemon> party, Func<string, bool>? moveExists, List<string> errors)
    {
        foreach (var p in party)
        {
            Species(at, p.Species, p.Form, errors);
            if (p.Level is < 1 or > 100)
                errors.Add($"{at}: nível {p.Level}");
            if (p.Moves.Count > 4)
                errors.Add($"{at}: mais de 4 golpes");
            if (moveExists is not null)
                foreach (var m in p.Moves.Where(m => !moveExists(m)))
                    errors.Add($"{at}: golpe {m} não existe no dex");
        }
    }

    private static void Species(string at, int species, int form, List<string> errors)
    {
        if (species is < 1 or > MaxSpecies)
            errors.Add($"{at}: espécie {species} fora de 1..{MaxSpecies}");
        if (form < 0)
            errors.Add($"{at}: forma {form}");
    }

    private static bool IsGame(string key) => GameVersions.All.Any(g => g.Key == key);

    private static IEnumerable<string> Duplicates(IEnumerable<string> ids) =>
        ids.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);
}

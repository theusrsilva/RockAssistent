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
            Map(store, map, errors);
        foreach (var map in store.LegacyMaps)
            LegacyMap(map, errors);
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

    public static readonly IReadOnlySet<string> PlaceKinds = new HashSet<string> { "city", "place", "building" };
    public static readonly IReadOnlySet<string> TerrainKinds = new HashSet<string> { "land", "sea", "mountain", "sand" };

    /// <summary>Jogos que usam o mapa da região (o de Johto também serve ao gs, c e hgss com a parte de Kanto).</summary>
    public static IReadOnlyList<string> RegionGames(string region) =>
        GameVersions.All.Where(g => string.Equals(g.Region.ToString(), region, StringComparison.OrdinalIgnoreCase)).Select(g => g.Key).ToList();

    /// <summary>
    /// Mapa metrô: ids únicos (lugares e rotas juntos), lugares e pontos dentro da grade, rótulo dentro da grade, sem dois
    /// lugares do mesmo jogo no mesmo ponto, rotas só com segmentos horizontais/verticais e pontas num lugar, na borda ou
    /// em outra rota, terreno com polígono, jogos da região e <c>unmapped</c> só com áreas que existem.
    /// </summary>
    private static void Map(GameDataStore store, MapGraph map, List<string> errors)
    {
        var at = $"maps/{map.Region}";
        int cols = map.Grid.Cols, rows = map.Grid.Rows;
        if (cols < 1 || rows < 1)
        {
            errors.Add($"{at}: grid {cols}x{rows} inválida");
            return;
        }
        var games = RegionGames(map.Region);
        bool Inside(double x, double y) => x >= 0 && y >= 0 && x <= cols && y <= rows;
        void Games(string where, IReadOnlyList<string>? list)
        {
            if (list is { Count: 0 })
                errors.Add($"{where}: games vazio (use null para todos)");
            foreach (var g in (list ?? []).Where(g => !games.Contains(g)))
                errors.Add($"{where}: jogo {g} não usa o mapa {map.Region}");
        }

        foreach (var dup in Duplicates(map.Places.Select(p => p.Id).Concat(map.Routes.Select(r => r.Id))))
            errors.Add($"{at}: id repetido {dup}");
        foreach (var t in map.Terrain)
        {
            if (!TerrainKinds.Contains(t.Kind))
                errors.Add($"{at}: terreno {t.Kind} (land, sea, mountain ou sand)");
            if (t.Polygon.Count < 3 || t.Polygon.Any(pt => pt.Count != 2))
                errors.Add($"{at}: terreno {t.Kind} precisa de 3 pontos [x, y] ou mais");
            else if (t.Polygon.Any(pt => !Inside(pt[0], pt[1])))
                errors.Add($"{at}: terreno {t.Kind} sai da grade");
        }
        foreach (var p in map.Places)
        {
            var pat = $"{at}/{p.Id}";
            if (!PlaceKinds.Contains(p.Kind))
                errors.Add($"{pat}: kind {p.Kind} (city, place ou building)");
            if (!Inside(p.X, p.Y))
                errors.Add($"{pat}: ({p.X}, {p.Y}) fora da grade {cols}x{rows}");
            if (!MapLabels.Positions.Contains(p.Label))
                errors.Add($"{pat}: label {p.Label} (n, s, e, w, ne, nw, se ou sw)");
            else
            {
                var b = MapLabels.Box(p);
                if (!Inside(b.X, b.Y) || !Inside(b.X + b.W, b.Y + b.H))
                    errors.Add($"{pat}: o rótulo sai da grade (mude label ou label_offset)");
            }
            if (p.LabelOffset is { Count: not 2 })
                errors.Add($"{pat}: label_offset é [dx, dy]");
            Games(pat, p.Games);
        }
        foreach (var g in games)
            foreach (var same in map.Places.Where(p => p.Games is null || p.Games.Contains(g)).GroupBy(p => (p.X, p.Y)).Where(x => x.Count() > 1))
                errors.Add($"{at}: {string.Join(" e ", same.Select(p => p.Id))} no mesmo ponto ({same.Key.X}, {same.Key.Y}) no {g}");

        var points = map.Places.Select(p => (p.X, p.Y)).ToHashSet();
        foreach (var r in map.Routes)
        {
            var rat = $"{at}/{r.Id}";
            if (string.IsNullOrWhiteSpace(r.Name))
                errors.Add($"{rat}: sem name");
            if (r.Number is { } n && string.IsNullOrWhiteSpace(n))
                errors.Add($"{rat}: number vazio (use null)");
            Games(rat, r.Games);
            if (r.Path.Count < 2 || r.Path.Any(pt => pt.Count != 2))
            {
                errors.Add($"{rat}: path precisa de 2 pontos [x, y] ou mais");
                continue;
            }
            foreach (var pt in r.Path.Where(pt => !Inside(pt[0], pt[1])))
                errors.Add($"{rat}: ponto ({pt[0]}, {pt[1]}) fora da grade");
            for (var i = 1; i < r.Path.Count; i++)
            {
                var (a, b) = (r.Path[i - 1], r.Path[i]);
                if (a[0] == b[0] && a[1] == b[1])
                    errors.Add($"{rat}: ponto repetido ({a[0]}, {a[1]})");
                else if (a[0] != b[0] && a[1] != b[1])
                    errors.Add($"{rat}: segmento ({a[0]}, {a[1]})→({b[0]}, {b[1]}) em diagonal (só horizontal ou vertical)");
            }
            foreach (var end in new[] { r.Path[0], r.Path[^1] })
            {
                var (x, y) = (end[0], end[1]);
                var onBorder = x == 0 || y == 0 || x == cols || y == rows;
                var onRoute = map.Routes.Any(o => o != r && OnPath(o.Path, x, y));
                if (!points.Contains((x, y)) && !onBorder && !onRoute)
                    errors.Add($"{rat}: a ponta ({x}, {y}) não está num lugar, na borda nem em outra rota");
            }
        }

        var ids = map.Places.Select(p => p.Id).Concat(map.Routes.Select(r => r.Id)).ToHashSet();
        var areaIds = games.SelectMany(g => store.Areas(g)).Select(a => a.Id).ToHashSet();
        foreach (var dup in Duplicates(map.Unmapped.Select(u => u.Id)))
            errors.Add($"{at}: unmapped repetido {dup}");
        foreach (var u in map.Unmapped)
        {
            if (string.IsNullOrWhiteSpace(u.Reason))
                errors.Add($"{at}: unmapped {u.Id} sem motivo");
            if (ids.Contains(u.Id))
                errors.Add($"{at}: {u.Id} está em unmapped e no mapa");
            if (!areaIds.Contains(u.Id))
                errors.Add($"{at}: unmapped {u.Id} não é área de nenhum jogo do mapa");
        }
    }

    /// <summary>O ponto está num dos segmentos do caminho.</summary>
    private static bool OnPath(IReadOnlyList<IReadOnlyList<int>> path, int x, int y)
    {
        for (var i = 1; i < path.Count; i++)
        {
            if (path[i - 1].Count != 2 || path[i].Count != 2)
                return false;
            int x1 = path[i - 1][0], y1 = path[i - 1][1], x2 = path[i][0], y2 = path[i][1];
            if (x >= Math.Min(x1, x2) && x <= Math.Max(x1, x2) && y >= Math.Min(y1, y2) && y <= Math.Max(y1, y2)
                && (x1 == x2 || y1 == y2))
                return true;
        }
        return false;
    }

    // TODO(mapas): validação do formato antigo; sai quando as 11 regiões estiverem no novo.
    private static void LegacyMap(LegacyMapGraph map, List<string> errors)
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
        var legacy = store.LegacyMap(game.Region);
        var unmapped = map?.Unmapped.Select(u => u.Id).ToHashSet() ?? [];

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
            Variants(at, key, b.Variants, moveExists, errors);
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
            if (map is not null)
            {
                // Toda área cai num lugar ou numa rota do jogo (map_node ?? id), ou está em unmapped.
                var target = a.MapNode ?? a.Id;
                var place = map.Places.FirstOrDefault(p => p.Id == target);
                var route = map.Routes.FirstOrDefault(r => r.Id == target);
                var games = place?.Games ?? route?.Games;
                if (place is null && route is null)
                {
                    if (a.MapNode is not null || !unmapped.Contains(a.Id))
                        errors.Add($"{at}: {target} não é lugar nem rota do mapa {game.Region} (ou ponha a área em unmapped)");
                }
                else if (games is not null && !games.Contains(key))
                    errors.Add($"{at}: {target} não aparece no mapa do {key} (games)");
            }
            else if (a.MapNode is { } node)
            {
                // TODO(mapas): ligação do formato antigo.
                var n = legacy?.Nodes.FirstOrDefault(x => x.Id == node);
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

    /// <summary>
    /// Variantes por inicial: o inicial é do jogo (<see cref="GameStarters"/>), sem inicial repetido, time não vazio,
    /// <c>level_cap</c> = maior nível do time e as mesmas regras de espécie e golpe do time padrão.
    /// </summary>
    private static void Variants(string at, string key, IReadOnlyList<BossVariant>? variants, Func<string, bool>? moveExists,
        List<string> errors)
    {
        if (variants is null)
            return;
        if (variants.Count == 0)
            errors.Add($"{at}: variants vazio (use null ou tire o campo)");
        foreach (var dup in Duplicates(variants.Select(v => v.Starter)))
            errors.Add($"{at}: variante repetida para {dup}");
        foreach (var v in variants)
        {
            var vat = $"{at}/variants/{v.Starter}";
            if (!GameStarters.IsStarter(key, v.Starter))
                errors.Add($"{vat}: {v.Starter} não é inicial do {key} ({string.Join(", ", GameStarters.For(key).Select(s => s.Id))})");
            if (v.Party.Count == 0)
                errors.Add($"{vat}: time vazio");
            else if (v.LevelCap != v.Party.Max(p => p.Level))
                errors.Add($"{vat}: level_cap {v.LevelCap} diferente do maior nível do time ({v.Party.Max(p => p.Level)})");
            Party(vat, v.Party, moveExists, errors);
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

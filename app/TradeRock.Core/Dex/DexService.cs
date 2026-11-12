using System.Globalization;
using System.Text;
using Microsoft.Data.Sqlite;
using TradeRock.Core.GameData;
using TradeRock.Core.Games;

namespace TradeRock.Core.Dex;

/// <summary>
/// Consultas da Pokédex offline (SQL direto no dex.sqlite). O que serve para todas as consultas (nomes, tipos, quem
/// existe em cada jogo) fica em memória depois da primeira chamada.
/// <para>Regras (o PokeAPI não tem tudo por geração):</para>
/// <list type="bullet">
/// <item>"Existe no jogo": a espécie é de uma geração até a do jogo e tem golpes no grupo de versões, ou número numa
/// Pokédex regional do jogo, ou um encontro numa versão do jogo (DLCs incluídas).</item>
/// <item>Fraquezas: a tabela de tipos da geração, só com os tipos que existem nela (Aço e Noturno a partir da Gen 2,
/// Fada a partir da Gen 6). Por par de tipos vale a linha de <c>type_efficacy_past</c> com a menor geração >= a do jogo
/// (Gen 1, Aço resistindo a Fantasma/Noturno até a Gen 5), senão a de hoje.</item>
/// <item>Tipos: os da geração (<c>pokemon_types_past</c>, a menor geração >= a do jogo; Clefairy era Normal até a
/// Gen 5), senão os de hoje. A lista sem jogo mostra os de hoje.</item>
/// <item>Golpes: tipo, força, precisão e PP da geração do jogo, pelo <c>move_changelog</c>.</item>
/// <item>Habilidades: as da geração (<c>pokemon_abilities_past</c>), nenhuma antes da Gen 3, ocultas só da Gen 5.</item>
/// </list>
/// </summary>
public sealed class DexService(DexDatabase db)
{
    private const string English = "en";
    private const int NationalDex = 1;

    /// <summary>
    /// Ids das linhas da lista que não são um <c>pokemon</c> do PokeAPI: as formas cosméticas (letras do Unown, padrões
    /// do Vivillon...) são 100000 + o id do <c>pokemon_form</c>, e a fêmea de uma espécie é 200000 + a espécie. Os
    /// sprites usam os mesmos ids (<c>/api/sprites/{espécie}?form=</c>).
    /// </summary>
    public const int CosmeticFormBase = 100_000;
    public const int FemaleFormBase = 200_000;
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

    private readonly Lazy<IReadOnlySet<string>> _moveIdentifiers = new(() =>
    {
        using var c = db.Open();
        using var cmd = Command(c, "SELECT identifier FROM moves");
        using var r = cmd.ExecuteReader();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        while (r.Read())
            ids.Add(r.GetString(0));
        return ids;
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>Identificadores de todos os golpes (como o PokeAPI escreve: <c>thunder-punch</c>).</summary>
    public IReadOnlySet<string> MoveIdentifiers() => _moveIdentifiers.Value;

    /// <summary>Nome da espécie em inglês; null se não existe.</summary>
    public string? SpeciesName(int speciesId) => Data.Species.TryGetValue(speciesId, out var s) ? s.Name : null;

    /// <summary>Cadeia de evolução da espécie (mesma cadeia = mesma linha evolutiva); null se não existe.</summary>
    public int? EvolutionChainId(int speciesId) => Data.Species.TryGetValue(speciesId, out var s) ? s.ChainId : null;

    /// <summary>
    /// Encontros de um local do PokeAPI (<c>locations.id</c>, todas as sub-áreas) nas versões do jogo, juntados por
    /// versão, espécie, forma e método (níveis de/até; chance = a maior entre as sub-áreas e, dentro de cada uma, a maior
    /// entre as condições, com os slots da mesma condição somados e o total limitado a 100). Local sem encontros = lista vazia.
    /// </summary>
    public Task<IReadOnlyList<LocationEncounter>> LocationEncountersAsync(int locationId, string gameKey, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var game = Data.Game(gameKey);
        using var c = db.Open();
        using var cmd = Command(c, $"""
            WITH forms AS (
                SELECT id, species_id, ROW_NUMBER() OVER (PARTITION BY species_id ORDER BY is_default DESC, form_order IS NULL, form_order, id) - 1 AS form
                FROM pokemon),
            -- A raridade do PokeAPI é por sub-área e por condição. Dentro de uma condição, os slots somam.
            per_condition AS (
                SELECT f.species_id, f.form, e.version_id, e.method, e.location_area_id,
                       min(e.min_level) AS min_level, max(e.max_level) AS max_level, sum(coalesce(e.rarity, 0)) AS chance
                FROM encounters e
                JOIN forms f ON f.id = e.pokemon_id
                JOIN location_areas a ON a.id = e.location_area_id
                WHERE a.location_id = $l AND e.version_id IN ({game.VersionIdList})
                GROUP BY f.species_id, f.form, e.version_id, e.method, e.location_area_id, e.condition_values),
            -- Condições diferentes (manhã/dia/noite, com/sem horda, Moltres estático 2 × 100) são alternativas: fica a maior,
            -- limitada a 100. Entre sub-áreas também fica a maior (somar entre sub-áreas passa de 100%).
            per_area AS (
                SELECT species_id, form, version_id, method, location_area_id,
                       min(min_level) AS min_level, max(max_level) AS max_level, min(max(chance), 100) AS chance
                FROM per_condition
                GROUP BY species_id, form, version_id, method, location_area_id)
            SELECT p.species_id, p.form, v.identifier, p.method, min(p.min_level), max(p.max_level), max(p.chance)
            FROM per_area p
            JOIN versions v ON v.id = p.version_id
            GROUP BY p.version_id, p.species_id, p.form, p.method
            ORDER BY p.species_id, p.form, p.version_id, p.method
            """, ("$l", locationId));
        using var r = cmd.ExecuteReader();
        var result = new List<LocationEncounter>();
        while (r.Read())
            result.Add(new LocationEncounter(r.GetInt32(0), r.GetInt32(1), r.GetString(2), Str(r, 3) ?? "unknown",
                Int(r, 4) ?? 0, Int(r, 5) ?? 0, Int(r, 6) ?? 0));
        return Task.FromResult<IReadOnlyList<LocationEncounter>>(result);
    }

    /// <summary>
    /// A linha da lista da Pokédex a que um Pokémon guardado corresponde: 0 = a espécie; senão o id da forma.
    /// <paramref name="form"/> é o índice de forma do PKHeX no contexto do arquivo; o nome dele na lista de formas do
    /// PKHeX diz qual é: a região ("Alola", "Galar", "Paldea Blaze"...) ou, nas outras formas, o mesmo nome do
    /// identificador do PokeAPI ("Wash" e <c>rotom-wash</c>, "B" e <c>unown-b</c>, "Polar" e <c>vivillon-polar</c>).
    /// Quando a forma é a da espécie e o Pokémon é fêmea (<paramref name="gender"/> 1) de uma espécie com a fêmea
    /// diferente, é a linha da fêmea. Forma que a lista não tem (Mega...) conta como a espécie. O Alcremie guarda o
    /// creme na forma e o confeito noutro campo, que o resumo do cofre não tem: conta como o do confeito de morango.
    /// </summary>
    public int ListFormId(int speciesId, int form, PKHeX.Core.EntityContext context, int gender = 0)
    {
        if (form < 0 || !Data.RegionalForms.TryGetValue(speciesId, out var forms))
            return 0;
        var listed = Listed(speciesId, form, context, forms);
        return listed == 0 && gender == 1 ? forms.FirstOrDefault(f => f.Kind == FormKind.Gender)?.PokemonId ?? 0 : listed;
    }

    private static int Listed(int speciesId, int form, PKHeX.Core.EntityContext context, List<RegionalForm> forms)
    {
        var names = PKHeX.Core.FormConverter.GetFormList((ushort)speciesId, Builder.BuilderData.Strings.types,
            Builder.BuilderData.Strings.forms, context);
        if (form >= names.Length)
            return 0;
        foreach (var region in forms.Where(f => f.Region is not null).Select(f => f.Region!).Distinct())
        {
            if (form == 0 || !names[form].StartsWith(region, StringComparison.OrdinalIgnoreCase))
                continue;
            var ordinal = Enumerable.Range(0, form).Count(i => names[i].StartsWith(region, StringComparison.OrdinalIgnoreCase));
            return forms.FirstOrDefault(f => f.Region == region && f.Ordinal == ordinal)?.PokemonId ?? 0;
        }
        // Outras formas. O PKHeX escreve "♀" (meowstic-female), "!" e "?" (unown-exclamation, unown-question), "C-Red"
        // (o Minior sem a casca: minior-red; "M-Red" é com ela) e nomes curtos ("Blue" e basculin-blue-striped, "Hoenn"
        // e pikachu-hoenn-cap, "Ruby Cream" e alcremie-ruby-cream-strawberry-sweet): igual, ou um começa com o outro.
        var name = names[form] switch
        {
            "♀" => "female",
            "♂" => "male",
            "!" => "exclamation",
            "?" => "question",
            ['C', '-', .. var core] => Letters(core),
            var other => Letters(other),
        };
        if (name.Length == 0)
            return 0;
        var others = forms.Where(f => f.Region is null && f.PokemonId < FemaleFormBase).Select(f => (Form: f, Suffix: Letters(f.Suffix))).ToList();
        // A forma 0 do PKHeX só é uma "outra forma" quando o nome bate inteiro (Maushold: a família de três é a 0 nele;
        // Vivillon: o padrão Icy Snow).
        var match = others.FirstOrDefault(o => o.Suffix == name).Form
            ?? (form == 0 ? null : others.FirstOrDefault(o => o.Suffix.StartsWith(name, StringComparison.Ordinal)
                || name.StartsWith(o.Suffix, StringComparison.Ordinal)).Form);
        return match?.PokemonId ?? 0;

        static string Letters(string text) => new([.. Normalize(text).Where(char.IsLetterOrDigit)]);
    }

    /// <summary>A linha existe na lista: a espécie (forma 0) ou uma forma dela que a lista mostra.</summary>
    public bool IsListed(int speciesId, int formId) =>
        Data.Species.ContainsKey(speciesId)
        && (formId == 0 || (Data.RegionalForms.TryGetValue(speciesId, out var forms) && forms.Any(f => f.PokemonId == formId)));

    /// <summary>
    /// Identificadores do PokeAPI das formas da espécie (<c>meowth</c>, <c>meowth-alola</c>, <c>meowth-galar</c>), na ordem
    /// em que o índice de forma dos encontros e do Nuzlocke conta (0 = a padrão). As formas sem ordem no PokeAPI (as Mega
    /// do Legends: Z-A, as Gigantamax, o Basculin de listra branca) vão para o fim: na frente, deslocavam o índice das
    /// outras (o Raichu de Alola deixava de ser a forma 1).
    /// </summary>
    public IReadOnlyList<string> PokemonForms(int speciesId)
    {
        using var c = db.Open();
        return Strings(c, "SELECT identifier FROM pokemon WHERE species_id = $s ORDER BY is_default DESC, form_order IS NULL, form_order, id", ("$s", speciesId));
    }

    /// <summary>Id do <c>pokemon</c> do PokeAPI da forma (posição em <see cref="PokemonForms"/>), para o sprite; null na forma padrão ou fora da lista.</summary>
    public int? FormPokemonId(int speciesId, int form)
    {
        if (form <= 0)
            return null;
        using var c = db.Open();
        using var cmd = Command(c, "SELECT id FROM pokemon WHERE species_id = $s ORDER BY is_default DESC, form_order IS NULL, form_order, id LIMIT 1 OFFSET $f",
            ("$s", speciesId), ("$f", form));
        return cmd.ExecuteScalar() is long id ? (int)id : null;
    }

    /// <summary>Identificador do PokeAPI da espécie (<c>meowth</c>); null se não existe.</summary>
    public string? SpeciesIdentifier(int speciesId)
    {
        using var c = db.Open();
        return Strings(c, "SELECT identifier FROM species WHERE id = $s", ("$s", speciesId)).FirstOrDefault();
    }

    /// <summary>
    /// Locais do PokeAPI (<c>locations.id</c>) onde a espécie (forma padrão) é dada de presente (método <c>gift</c>)
    /// nas versões do jogo (ou só em <paramref name="version"/>), com o menor nível. Usado para pôr o inicial na área
    /// certa do Nuzlocke.
    /// </summary>
    public IReadOnlyList<(int LocationId, int Level)> GiftLocations(int speciesId, string gameKey, string? version)
    {
        var game = Data.Game(gameKey);
        using var c = db.Open();
        using var cmd = Command(c, $"""
            SELECT a.location_id, min(e.min_level)
            FROM encounters e
            JOIN pokemon p ON p.id = e.pokemon_id
            JOIN location_areas a ON a.id = e.location_area_id
            JOIN versions v ON v.id = e.version_id
            WHERE p.species_id = $s AND p.is_default = 1 AND e.method = 'gift' AND e.version_id IN ({game.VersionIdList})
              AND ($v IS NULL OR v.identifier = $v)
            GROUP BY a.location_id
            """, ("$s", speciesId), ("$v", version));
        using var r = cmd.ExecuteReader();
        var result = new List<(int, int)>();
        while (r.Read())
            result.Add((r.GetInt32(0), Int(r, 1) ?? 0));
        return result;
    }

    /// <summary>
    /// TMs, HMs e TRs do jogo em ordem de número, com os das DLCs (<see cref="ExtraGroups"/>): número que só existe no grupo
    /// da DLC sai com <c>Dlc = true</c> (Z-A: TM108–TM160 do Mega Dimension); o mesmo número no jogo base vale mais.
    /// Jogo sem máquinas no PokeAPI (Arceus) = lista vazia; o BDSP está incompleto no PokeAPI (17 linhas).
    /// </summary>
    public Task<IReadOnlyList<MachineInfo>> MachinesAsync(string gameKey, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var data = Data;
        var game = data.Game(gameKey);
        using var c = db.Open();
        var result = new SortedDictionary<int, MachineInfo>();
        foreach (var group in game.GroupIds)
        {
            using var cmd = Command(c, """
                SELECT mc.machine_number, coalesce(i.name_en, i.identifier), m.id, m.identifier, coalesce(m.name_en, m.identifier),
                       m.type_id, m.power, m.pp, m.accuracy, m.damage_class
                FROM machines mc JOIN items i ON i.id = mc.item_id JOIN moves m ON m.id = mc.move_id
                WHERE mc.version_group_id = $vg ORDER BY mc.machine_number
                """, ("$vg", group));
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var moveId = r.GetInt32(2);
                var stats = data.MoveAt(moveId, game.Order, Int(r, 5) ?? 0, Int(r, 6), Int(r, 7), Int(r, 8));
                result.TryAdd(Int(r, 0) ?? 0, new MachineInfo(Int(r, 0) ?? 0, r.GetString(1), moveId, r.GetString(3), r.GetString(4),
                    data.TypeName(stats.Type), Str(r, 9), stats.Power, stats.Accuracy, stats.Pp, group != game.MainGroupId));
            }
        }
        return Task.FromResult<IReadOnlyList<MachineInfo>>([.. result.Values]);
    }

    public Task<IReadOnlyList<SpeciesSummary>> ListAsync(DexQuery q, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var data = Data;
        var game = q.GameKey is null ? null : data.Game(q.GameKey);
        string[] Types(SpeciesRow s) => game is null ? s.Types : data.SpeciesTypes(s, game.Generation);
        var text = string.IsNullOrWhiteSpace(q.Text) ? null : Normalize(q.Text.Trim());
        int? number = text is not null && int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var n) ? n : null;

        bool Wanted(RegionalForm f) => f.Kind switch
        {
            FormKind.Regional => q.RegionalForms,
            FormKind.Other => q.OtherForms,
            FormKind.Many => q.ManyForms,
            _ => q.GenderForms,
        };
        // Cada espécie e, se pedido, as formas dela logo em seguida (mesmo número).
        IEnumerable<(SpeciesRow Species, RegionalForm? Form)> rows = data.Species.Values.SelectMany(s =>
            (q.RegionalForms || q.OtherForms || q.ManyForms || q.GenderForms) && data.RegionalForms.TryGetValue(s.Id, out var forms)
                ? forms.Where(Wanted).Select(f => (s, (RegionalForm?)f)).Prepend((s, null))
                : [(s, null)]);
        string[] EntryTypes((SpeciesRow Species, RegionalForm? Form) e) => e.Form is not { } f
            ? Types(e.Species)
            : game is null ? f.Types : data.PokemonTypes(f.PokemonId, f.Types, game.Generation);

        if (game is not null)
            rows = rows.Where(e => game.Exists.Contains(e.Species.Id) && (e.Form is null
                || (e.Form.Kind != FormKind.Regional ? e.Form.IntroducedOrder <= game.Order : game.Forms.Contains(e.Form.PokemonId))));
        // A forma regional conta na geração em que ela apareceu (Raichu de Alola: 7), não na da espécie.
        int Generation((SpeciesRow Species, RegionalForm? Form) e) => e.Form?.Generation ?? e.Species.Generation;
        if (q.Generation is { } gen)
            rows = rows.Where(e => Generation(e) == gen);
        if (!string.IsNullOrWhiteSpace(q.Type))
            rows = rows.Where(e => EntryTypes(e).Contains(q.Type, StringComparer.OrdinalIgnoreCase));
        if (text is not null)
            rows = rows.Where(e => e.Species.Id == number
                || e.Species.Search.Any(k => k.Contains(text, StringComparison.Ordinal))
                || (e.Form is { Region: { } region } && region.Contains(text, StringComparison.Ordinal)));

        IReadOnlyList<SpeciesSummary> result = rows
            .Skip(Math.Max(0, q.Offset))
            .Take(Math.Clamp(q.Limit, 1, 2000))
            .Select(e => new SpeciesSummary(e.Species.Id, e.Species.Name, Generation(e), EntryTypes(e),
                e.Species.National, e.Form?.PokemonId, e.Form?.Region, e.Form?.Detail))
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

    /// <summary>
    /// Tipo, categoria e poder de vários golpes como valem no jogo (com as mudanças do PokeAPI), sem a lista de quem
    /// aprende. Golpe desconhecido fica fora. Para análises rápidas (times do VGC).
    /// </summary>
    public Task<IReadOnlyList<MoveBattleInfo>> MovesBattleInfoAsync(IEnumerable<int> ids, string gameKey, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var data = Data;
        var game = data.Game(gameKey);
        using var c = db.Open();
        var result = new List<MoveBattleInfo>();
        foreach (var id in ids.Distinct())
        {
            using var cmd = Command(c, "SELECT identifier, name_en, type_id, power, pp, accuracy, damage_class FROM moves WHERE id = $m",
                ("$m", id));
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                continue;
            var stats = data.MoveAt(id, game.Order, Int(r, 2) ?? 0, Int(r, 3), Int(r, 4), Int(r, 5));
            result.Add(new MoveBattleInfo(id, r.GetString(0), Str(r, 1) ?? r.GetString(0), data.TypeName(stats.Type), Str(r, 6), stats.Power));
        }
        return Task.FromResult<IReadOnlyList<MoveBattleInfo>>(result);
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
            d => d.Name, d => data.Factor(a.Id, d.Id, generation) / 100.0));
        return Task.FromResult(new TypeChart(generation, types.Select(t => t.Name).ToList(), factors));
    }

    /// <summary>Disponibilidade só com o PokeAPI. Veja a sobrecarga com os nossos encontros.</summary>
    /// <summary>
    /// Como conseguir a espécie em cada jogo: o jeito mais forte (Encounter &gt; Gift &gt; Trade &gt; Evolution &gt; Egg)
    /// entre três fontes: o PokeAPI (encontros; nos jogos com encontros próprios ele não conta), os nossos encontros
    /// (<paramref name="ownEncounters"/>: BDSP, Arceus, SV e Z-A; <c>gift</c> = Gift, <c>trade</c> = Trade, o resto =
    /// Encounter) e as tabelas do PKHeX (<see cref="PkhexAvailability"/>; o encontro fixo dele, que não diz se é
    /// presente, fica abaixo de Gift e Trade e só vira Encounter quando ninguém mais classifica). Sem nenhum deles, valem Evolution (evolui de
    /// uma espécie que existe no jogo) e Egg (melhor esforço: forma base, o jogo tem criação, e alguma evolução dela
    /// existe no jogo e pode botar ovo), senão None.
    /// </summary>
    /// <param name="ownEncounters">Encontros próprios do jogo (<c>GameDataStore.Encounters</c>); nulo = sem eles.</param>
    public Task<IReadOnlyList<GameAvailability>> AvailabilityAsync(int speciesId,
        Func<string, IReadOnlyList<OwnEncounter>>? ownEncounters, CancellationToken ct) =>
        Task.FromResult(Availability(speciesId, ownEncounters, pkhex: true, ct));

    /// <summary>
    /// Os parentes que dão a espécie no jogo quando ela não é pega direto (a disponibilidade dela é Evolution ou Egg):
    /// as pré-evoluções que têm encontro, presente ou troca lá (é só evoluir) e, se ela vem de ovo, as evoluções que
    /// têm (é só chocar). Da mais próxima para a mais distante. Vazio quando a própria espécie se pega direto.
    /// </summary>
    public IReadOnlyList<RelatedSource> RelatedSources(int speciesId, string gameKey,
        Func<string, IReadOnlyList<OwnEncounter>>? ownEncounters, CancellationToken ct)
    {
        var data = Data;
        if (!data.Species.ContainsKey(speciesId))
            return [];
        AvailabilityKind KindOf(int id) =>
            Availability(id, ownEncounters, pkhex: true, ct).FirstOrDefault(a => a.GameKey == gameKey)?.Kind ?? AvailabilityKind.None;
        static bool Direct(AvailabilityKind k) => k is AvailabilityKind.Encounter or AvailabilityKind.Gift or AvailabilityKind.Trade;

        var own = KindOf(speciesId);
        if (own is not (AvailabilityKind.Evolution or AvailabilityKind.Egg))
            return [];
        var result = new List<RelatedSource>();
        // Pré-evoluções, subindo a cadeia.
        for (var parent = data.Species[speciesId].EvolvesFrom; parent is { } id && data.Species.TryGetValue(id, out var row); parent = row.EvolvesFrom)
            if (KindOf(id) is var kind && Direct(kind))
                result.Add(new RelatedSource(id, row.Name, kind, "evolve"));
        // Evoluções, descendo (em largura): quem bota o ovo.
        if (own == AvailabilityKind.Egg)
        {
            var queue = new Queue<int>([speciesId]);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var child in data.Species.Values.Where(s => s.EvolvesFrom == current))
                {
                    queue.Enqueue(child.Id);
                    if (KindOf(child.Id) is var kind && Direct(kind))
                        result.Add(new RelatedSource(child.Id, child.Name, kind, "breed"));
                }
            }
        }
        return result;
    }

    /// <param name="pkhex">Usa as tabelas do PKHeX (desligado só na auditoria, para comparar as fontes).</param>
    internal IReadOnlyList<GameAvailability> Availability(int speciesId, Func<string, IReadOnlyList<OwnEncounter>>? ownEncounters,
        bool pkhex, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var data = Data;
        data.Species.TryGetValue(speciesId, out var row);
        return GameVersions.All.Select(g =>
        {
            var game = data.Game(g.Key);
            var all = ownEncounters?.Invoke(g.Key) ?? [];
            // Nos nossos dados, o que só sai com item de evento fica para a segunda passada (com requisito).
            var own = all.Where(e => OwnRequirement(g.Key, e) is null).ToList();
            var fromPkhex = pkhex ? PkhexAvailability.ForGame(g.Key) : new Dictionary<int, AvailabilityKind>();
            var statics = pkhex ? PkhexAvailability.Statics(g.Key) : new HashSet<int>();
            var In = Presence(game, all, pkhex);
            if (row is null)
                return new GameAvailability(g.Key, g.Name, AvailabilityKind.None);

            var kind = AvailabilityKind.None;
            if (In(speciesId))
            {
                kind = fromPkhex.GetValueOrDefault(speciesId);
                if (all.Count == 0 && game.Exists.Contains(speciesId))
                {
                    if (game.Encounters.Contains(speciesId))
                        kind = AvailabilityKind.Encounter;
                    else if (game.Gifts.Contains(speciesId))
                        kind = PkhexAvailability.Strongest(kind, AvailabilityKind.Gift);
                    else if (game.Trades.Contains(speciesId))
                        kind = PkhexAvailability.Strongest(kind, AvailabilityKind.Trade);
                }
                foreach (var e in own.Where(e => e.Species == speciesId))
                    kind = PkhexAvailability.Strongest(kind, OwnKind(e));
                if (PkhexAvailability.Strength(kind) < PkhexAvailability.Strength(AvailabilityKind.Trade) && statics.Contains(speciesId))
                    kind = AvailabilityKind.Encounter;
                if (PkhexAvailability.Strength(kind) < PkhexAvailability.Strength(AvailabilityKind.Evolution)
                    && row.EvolvesFrom is { } from && In(from))
                    kind = AvailabilityKind.Evolution;
                else if (kind == AvailabilityKind.None && row.EvolvesFrom is null && g.Generation >= 2 && !NoBreeding.Contains(g.Key)
                         && data.Descendants(speciesId).Any(d => In(d) && data.Breedable.Contains(d)))
                    kind = AvailabilityKind.Egg;
            }

            // Segunda passada: sem jeito normal, vale o que precisa do Pokéwalker ou de um item de evento (com aviso).
            string? requires = null;
            if (kind == AvailabilityKind.None)
            {
                if (Requirement(speciesId) is { } direct)
                    (kind, requires) = direct;
                // Evolui de quem só vem com requisito (Pokéwalker, evento): a espécie tem que ser da geração do jogo e a
                // forma anterior não pode ter jeito normal.
                else if (row.EvolvesFrom is { } from && row.Generation <= g.Generation && Requirement(from) is { Requires: var tag }
                         && !NormallyIn(from))
                    (kind, requires) = (AvailabilityKind.Evolution, tag);
            }

            var locked = pkhex && kind != AvailabilityKind.None && PkhexAvailability.Shiny(g.Key, speciesId) is { Locked: true }
                && !CanHatchIn(speciesId, g.Key, ownEncounters);
            return new GameAvailability(g.Key, g.Name, kind, locked, requires);

            // Tem jeito normal de conseguir (sem requisito) em alguma fonte?
            bool NormallyIn(int species) =>
                (all.Count == 0 && (game.Encounters.Contains(species) || game.Gifts.Contains(species) || game.Trades.Contains(species)))
                || own.Any(e => e.Species == species) || fromPkhex.ContainsKey(species) || statics.Contains(species);

            // Jeito com requisito de uma espécie (PKHeX, PokeAPI e nossos dados); nulo = nenhum.
            (AvailabilityKind Kind, string Requires)? Requirement(int species)
            {
                (AvailabilityKind Kind, string Requires)? best = null;
                void Offer(AvailabilityKind k, string tag)
                {
                    if (best is null || PkhexAvailability.Strength(k) > PkhexAvailability.Strength(best.Value.Kind))
                        best = (k, tag);
                }
                if (pkhex && PkhexAvailability.Required(g.Key).TryGetValue(species, out var p))
                    Offer(p.Kind, p.Requires);
                if (all.Count == 0 && game.Events.Contains(species))
                    Offer(AvailabilityKind.Encounter, Requires.EventItem);
                foreach (var e in all.Where(e => e.Species == species))
                    if (OwnRequirement(g.Key, e) is { } tag)
                        Offer(OwnKind(e), tag);
                return best;
            }
        }).ToList();
    }

    private static AvailabilityKind OwnKind(OwnEncounter e) => e.Method switch
    {
        "gift" => AvailabilityKind.Gift,
        "trade" => AvailabilityKind.Trade,
        _ => AvailabilityKind.Encounter,
    };

    /// <summary>
    /// Requisito de um encontro dos nossos dados: no BDSP, Darkrai (Newmoon Island), Shaymin (Flower Paradise) e
    /// Arceus (Hall of Origin) só com o item do evento; no SV, o Pecharunt só com o item do Mystery Gift (Mochi Mayhem).
    /// </summary>
    private static string? OwnRequirement(string gameKey, OwnEncounter e) => (gameKey, e.Area, e.Species) switch
    {
        ("bdsp", "newmoon-island" or "flower-paradise" or "hall-of-origin", _) => Requires.EventItem,
        ("sv", _, 1025) => Requires.EventItem,
        _ => null,
    };

    /// <summary>A espécie existe no jogo por alguma fonte: PokeAPI, nossos encontros ou PKHeX.</summary>
    private static Func<int, bool> Presence(GameInfo game, IReadOnlyList<OwnEncounter> own, bool pkhex)
    {
        var ownSpecies = own.Select(e => e.Species).ToHashSet();
        var fromPkhex = pkhex ? PkhexAvailability.ForGame(game.Key) : new Dictionary<int, AvailabilityKind>();
        var statics = pkhex ? PkhexAvailability.Statics(game.Key) : new HashSet<int>();
        var required = pkhex ? PkhexAvailability.Required(game.Key) : new Dictionary<int, (AvailabilityKind, string)>();
        return s => game.Exists.Contains(s) || ownSpecies.Contains(s) || fromPkhex.ContainsKey(s) || statics.Contains(s)
                    || required.ContainsKey(s);
    }

    /// <summary>
    /// A espécie sai de ovo no jogo (direto ou chocando a forma base e evoluindo)? Melhor esforço, como o Egg de
    /// <see cref="AvailabilityAsync"/>: o jogo tem criação, a espécie existe nele e alguém da família (a partir da
    /// forma base, incluindo ela) existe no jogo e não é do grupo "no-eggs". Lendários sem família ficam de fora.
    /// "Existe no jogo" é o mesmo da disponibilidade: PokeAPI, nossos encontros ou PKHeX.
    /// </summary>
    /// <param name="ownEncounters">Encontros próprios do jogo (<c>GameDataStore.Encounters</c>); nulo = sem eles.</param>
    public bool CanHatchIn(int speciesId, string gameKey, Func<string, IReadOnlyList<OwnEncounter>>? ownEncounters = null)
    {
        var data = Data;
        var group = GameVersions.ByKey(gameKey);
        var In = Presence(data.Game(gameKey), ownEncounters?.Invoke(gameKey) ?? [], pkhex: true);
        if (group.Generation < 2 || NoBreeding.Contains(gameKey) || !data.Species.TryGetValue(speciesId, out var row)
            || !In(speciesId))
            return false;
        while (row.EvolvesFrom is { } from && data.Species.TryGetValue(from, out var parent))
            row = parent;
        return data.Descendants(row.Id).Prepend(row.Id).Any(d => In(d) && data.Breedable.Contains(d));
    }

    // ---- partes da ficha ----

    private static List<SpeciesForm> Forms(SqliteConnection c, Catalog data, SpeciesRow row, GameInfo game)
    {
        var pokemon = new List<(int Id, string Identifier, bool IsDefault)>();
        using (var cmd = Command(c, "SELECT id, identifier, is_default FROM pokemon WHERE species_id = $s ORDER BY is_default DESC, form_order IS NULL, form_order, id",
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
            forms.Add(new SpeciesForm(p.Id, p.Identifier, info.Name ?? row.Name,
                data.PokemonTypes(p.Id, [.. types.GetValueOrDefault(p.Id) ?? []], game.Generation),
                Stats(stats.GetValueOrDefault(p.Id)), info.Mega, p.IsDefault,
                data.RegionalForms.TryGetValue(row.Id, out var listed) && listed.Any(f => f.PokemonId == p.Id)));
        }

        // Formas cosméticas da espécie (as formas do pokemon padrão além da dele) e a fêmea diferente: mesmos status.
        // Aqui entram também as que a lista não mostra (as de item do Arceus e do Silvally, com o tipo no nome).
        var main = pokemon.FirstOrDefault(p => p.IsDefault);
        if (main.Id != 0)
        {
            var mainTypes = data.PokemonTypes(main.Id, [.. types.GetValueOrDefault(main.Id) ?? []], game.Generation);
            var mainStats = Stats(stats.GetValueOrDefault(main.Id));
            var known = data.RegionalForms.GetValueOrDefault(row.Id) ?? [];
            using (var cmd = Command(c, """
                SELECT id, identifier, form_name_en, introduced_in_version_group, form_identifier FROM pokemon_forms
                WHERE pokemon_id = $p AND is_default = 0 AND form_name_en IS NOT NULL ORDER BY id
                """, ("$p", main.Id)))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                {
                    if (Int(r, 3) is { } vg && data.GroupOrder(vg) > game.Order)
                        continue;
                    var id = CosmeticFormBase + r.GetInt32(0);
                    var type = Str(r, 4) is { } suffix && data.TypeId(suffix) is not null ? new[] { suffix } : mainTypes;
                    forms.Add(new SpeciesForm(id, r.GetString(1), r.GetString(2), type, mainStats, false, false, known.Any(f => f.PokemonId == id)));
                }
            if (known.FirstOrDefault(f => f.PokemonId == FemaleFormBase + row.Id) is { } female && female.IntroducedOrder <= game.Order)
                forms.Add(new SpeciesForm(female.PokemonId, main.Identifier + "-female", "♀", mainTypes, mainStats, false, false, true));
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
            t => defenderIds.Aggregate(1.0, (acc, d) => acc * data.Factor(t.Id, d, generation) / 100.0));
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
        return result.Count > 0 ? result : LatestFlavor(c, speciesId, language);
    }

    /// <summary>
    /// Nenhuma versão do jogo tem texto (ex.: Gengar no SV): o texto da versão mais recente que tiver um, no idioma
    /// pedido ou em inglês. O <c>Version</c> diz de qual jogo veio.
    /// </summary>
    private static List<FlavorText> LatestFlavor(SqliteConnection c, int speciesId, string language)
    {
        using var cmd = Command(c, """
            SELECT v.identifier, f.language, f.text
            FROM flavor_text f JOIN versions v ON v.id = f.version_id
            WHERE f.species_id = $s AND f.language IN ($l, $en)
            ORDER BY f.version_id DESC, f.language = $l DESC
            LIMIT 1
            """, ("$s", speciesId), ("$l", language), ("$en", English));
        using var r = cmd.ExecuteReader();
        return r.Read() ? [new FlavorText(r.GetString(0), r.GetString(1), r.GetString(2))] : [];
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
            SELECT pm.move_id, pm.method, pm.level, coalesce(m.name_en, m.identifier), m.type_id, m.power, m.pp, m.accuracy, m.damage_class,
                   m.identifier
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
            groups.GetOrAdd(method).Add(new LearnedMove(id, r.GetString(9), r.GetString(3), data.TypeName(stats.Type), Str(r, 8), stats.Power,
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

    /// <summary>
    /// Forma que a lista da Pokédex mostra num cartão próprio. Regional: <paramref name="Region"/> preenchida,
    /// <paramref name="Ordinal"/> é a posição entre as formas da mesma região da espécie (só o Tauros de Paldea tem mais de
    /// uma: 0, 1 e 2) e <paramref name="Generation"/> é a da região (Alola 7, Galar e Hisui 8, Paldea 9). Outra forma:
    /// região nula, a geração é a da espécie e ela existe a partir do grupo de versões <paramref name="IntroducedOrder"/>.
    /// </summary>
    /// <param name="Suffix">O identificador do PokeAPI sem a espécie (<c>rotom-wash</c> → <c>wash</c>).</param>
    /// <param name="Kind">Em qual caixa da lista a forma entra.</param>
    private sealed record RegionalForm(int PokemonId, int SpeciesId, int Ordinal, string? Region, string? Detail, int? Generation,
        string[] Types, int IntroducedOrder = 0, string Suffix = "", FormKind Kind = FormKind.Other);

    /// <summary>
    /// Caixas de formas da lista: regionais (sempre); outras formas; as das espécies com muitas (Unown, Vivillon,
    /// Alcremie); e a fêmea diferente.
    /// </summary>
    private enum FormKind
    {
        Regional,
        Other,
        Many,
        Gender,
    }

    /// <param name="Forms">Ids (<c>pokemon</c> do PokeAPI) das formas regionais que existem no jogo.</param>
    private sealed record GameInfo(string Key, int Generation, int MainGroupId, int Order,
        IReadOnlyList<(int Id, string Identifier)> Versions, HashSet<int> Exists, HashSet<int> Encounters, HashSet<int> Gifts,
        HashSet<int> Trades, HashSet<int> Events, IReadOnlyList<int> GroupIds, HashSet<int> Forms)
    {
        public string VersionIdList { get; } = string.Join(",", Versions.Select(v => v.Id));
    }

    private sealed record MoveStats(int Type, int? Power, int? Pp, int? Accuracy);

    private sealed class Catalog
    {
        public required SortedDictionary<int, SpeciesRow> Species { get; init; }
        public required Dictionary<int, (string Name, int Generation)> Types { get; init; }
        public required Dictionary<(int Attack, int Defense), int> Efficacy { get; init; }
        /// <summary>Linhas antigas por par de tipos, em ordem crescente de geração.</summary>
        private Dictionary<(int Attack, int Defense), List<(int Generation, int Factor)>> EfficacyPast { get; init; } = [];
        /// <summary>Tipos antigos por Pokémon e pela forma padrão de cada espécie, em ordem crescente de geração.</summary>
        private Dictionary<int, List<(int Generation, string[] Types)>> PokemonTypesPast { get; init; } = [];
        private Dictionary<int, List<(int Generation, string[] Types)>> SpeciesTypesPast { get; init; } = [];
        public required Dictionary<string, GameInfo> Games { get; init; }
        /// <summary>Formas da lista (regionais e outras) por espécie, na ordem das formas.</summary>
        public required Dictionary<int, List<RegionalForm>> RegionalForms { get; init; }
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

        /// <summary>Dano (100 = ×1) do tipo de ataque no de defesa na geração: a menor linha antiga >= ela, senão a de hoje.</summary>
        public int Factor(int attack, int defense, int generation) =>
            EfficacyPast.TryGetValue((attack, defense), out var past) && past.FirstOrDefault(p => p.Generation >= generation) is { Generation: > 0 } p
                ? p.Factor
                : Efficacy.GetValueOrDefault((attack, defense), 100);

        public string[] PokemonTypes(int pokemonId, string[] current, int generation) =>
            Past(PokemonTypesPast, pokemonId, generation) ?? current;

        public string[] SpeciesTypes(SpeciesRow row, int generation) => Past(SpeciesTypesPast, row.Id, generation) ?? row.Types;

        private static string[]? Past(Dictionary<int, List<(int Generation, string[] Types)>> map, int key, int generation) =>
            map.TryGetValue(key, out var past) && past.FirstOrDefault(p => p.Generation >= generation) is { Generation: > 0 } p
                ? p.Types
                : null;

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

            var efficacyPast = new Dictionary<(int, int), List<(int, int)>>();
            using (var cmd = Command(c, "SELECT damage_type_id, target_type_id, generation_id, damage_factor FROM type_efficacy_past ORDER BY generation_id"))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    efficacyPast.GetOrAdd((r.GetInt32(0), r.GetInt32(1))).Add((r.GetInt32(2), r.GetInt32(3)));

            Dictionary<int, List<(int, string[])>> TypesPast(string sql)
            {
                var past = new Dictionary<int, SortedDictionary<int, List<string>>>();
                using var cmd = Command(c, sql);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!past.TryGetValue(r.GetInt32(0), out var byGeneration))
                        past[r.GetInt32(0)] = byGeneration = [];
                    if (!byGeneration.TryGetValue(r.GetInt32(1), out var list))
                        byGeneration[r.GetInt32(1)] = list = [];
                    list.Add(types.TryGetValue(r.GetInt32(2), out var t) ? t.Item1 : "unknown");
                }
                return past.ToDictionary(p => p.Key, p => p.Value.Select(g => (g.Key, g.Value.ToArray())).ToList());
            }
            var pokemonTypesPast = TypesPast("SELECT pokemon_id, generation_id, type_id FROM pokemon_types_past ORDER BY pokemon_id, generation_id, slot");
            var speciesTypesPast = TypesPast("""
                SELECT p.species_id, tp.generation_id, tp.type_id FROM pokemon_types_past tp JOIN pokemon p ON p.id = tp.pokemon_id
                WHERE p.is_default = 1 ORDER BY p.species_id, tp.generation_id, tp.slot
                """);

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

            // Formas regionais: o nome da forma no PokeAPI começa pela região ("Alolan Form", "Paldean Form (Blaze
            // Breed)"). Totem, boné do Pikachu e Zen de Galar têm outro nome e ficam de fora.
            var regional = new Dictionary<int, List<RegionalForm>>();
            using (var cmd = Command(c, """
                WITH forms AS (
                    SELECT id, species_id, ROW_NUMBER() OVER (PARTITION BY species_id ORDER BY is_default DESC, form_order IS NULL, form_order, id) - 1 AS form
                    FROM pokemon)
                SELECT f.id, f.species_id, f.form, pf.form_name_en,
                       (SELECT group_concat(type_id) FROM (SELECT type_id FROM pokemon_types WHERE pokemon_id = f.id ORDER BY slot)),
                       p.identifier, pf.is_mega, pf.is_battle_only, pf.introduced_in_version_group, s.identifier
                FROM forms f JOIN pokemon p ON p.id = f.id JOIN pokemon_forms pf ON pf.pokemon_id = f.id
                JOIN species s ON s.id = f.species_id
                WHERE f.form > 0 AND pf.form_name_en IS NOT NULL
                ORDER BY f.species_id, f.form, pf.id
                """))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                {
                    var formName = r.GetString(3);
                    var found = Regions.FirstOrDefault(x => formName.StartsWith(x.Prefix, StringComparison.Ordinal));
                    var region = found.Region;
                    var identifier = r.GetString(5);
                    // Outras formas: só as que se guardam como estão.
                    if (region is null && (r.GetInt32(6) != 0 || r.GetInt32(7) != 0 || NotKept.Any(x => identifier.Contains(x, StringComparison.Ordinal))))
                        continue;
                    var open = formName.IndexOf('(');
                    var detail = region is null ? formName : open >= 0 ? formName[(open + 1)..].TrimEnd(')').Trim() : null;
                    string[] formTypes = [.. (Str(r, 4) ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => types.TryGetValue(int.Parse(id, CultureInfo.InvariantCulture), out var t) ? t.Item1 : "unknown")];
                    var list = regional.GetOrAdd(r.GetInt32(1));
                    if (list.All(x => x.PokemonId != r.GetInt32(0)))
                        list.Add(new RegionalForm(r.GetInt32(0), r.GetInt32(1), list.Count(x => x.Region == region), region, detail,
                            region is null ? null : found.Generation, formTypes, Int(r, 8) is { } vg ? groupOrders.GetValueOrDefault(vg) : 0,
                            identifier.StartsWith(r.GetString(9) + "-", StringComparison.Ordinal) ? identifier[(r.GetString(9).Length + 1)..] : identifier,
                            region is not null ? FormKind.Regional : formName == "Female" ? FormKind.Gender : FormKind.Other));
                }

            // Formas cosméticas: as formas de um pokemon além da padrão dele (letras do Unown, padrões do Vivillon,
            // sabores do Alcremie, flores, estações...). Mesmos tipos e status da espécie. Ficam de fora as sem nome
            // (Scatterbug, Mothim: não se vê diferença), as só de batalha e as que dependem de item.
            using (var cmd = Command(c, """
                SELECT pf.id, p.species_id, pf.identifier, pf.form_name_en, pf.is_battle_only, pf.introduced_in_version_group,
                       pf.form_identifier
                FROM pokemon_forms pf JOIN pokemon p ON p.id = pf.pokemon_id
                WHERE pf.is_default = 0 AND p.is_default = 1 AND pf.form_name_en IS NOT NULL
                ORDER BY p.species_id, pf.id
                """))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                {
                    var identifier = r.GetString(2);
                    if (r.GetInt32(4) != 0 || NotKept.Concat(ItemForms).Any(x => identifier.Contains(x, StringComparison.Ordinal)))
                        continue;
                    var speciesId = r.GetInt32(1);
                    var formName = r.GetString(3);
                    regional.GetOrAdd(speciesId).Add(new RegionalForm(CosmeticFormBase + r.GetInt32(0), speciesId, 0, null,
                        formName == "Female" ? "♀" : formName, null, [.. speciesTypes.GetValueOrDefault(speciesId) ?? []],
                        Int(r, 5) is { } vg ? groupOrders.GetValueOrDefault(vg) : 0, Str(r, 6) ?? identifier,
                        formName == "Female" ? FormKind.Gender : ManySpecies.Contains(speciesId) ? FormKind.Many : FormKind.Other));
                }
            foreach (var list in regional.Values)
                for (var i = 0; i < list.Count; i++)
                    if (list[i] is { Kind: FormKind.Gender, Detail: not "♀" } female)
                        list[i] = female with { Detail = "♀" };

            // A fêmea diferente das outras espécies (Venusaur, Pikachu, Hippowdon...): existe desde a Gen 4.
            var genderFrom = groups.TryGetValue("diamond-pearl", out var dp) ? dp.Order : 0;
            using (var cmd = Command(c, "SELECT id FROM species WHERE has_gender_differences = 1 ORDER BY id"))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                {
                    var speciesId = r.GetInt32(0);
                    var list = regional.GetOrAdd(speciesId);
                    if (list.All(f => f.Kind != FormKind.Gender))
                        list.Add(new RegionalForm(FemaleFormBase + speciesId, speciesId, 0, null, "♀", null,
                            [.. speciesTypes.GetValueOrDefault(speciesId) ?? []], genderFrom, "female", FormKind.Gender));
                }

            // Quem existe em cada grupo de versões (golpes e encontros) e em cada Pokédex regional.
            var byGroup = Pairs(c, "SELECT DISTINCT p.species_id, pm.version_group_id FROM pokemon_moves pm JOIN pokemon p ON p.id = pm.pokemon_id");
            // Encontros que contam para a disponibilidade. Ficam de fora os de outros jogos e aparelhos (bônus do
            // Colosseum, Pokémon Channel, Pokémon Ranger), a Magearna do código QR e o New Mauville do Sun/Moon, que o
            // PokeAPI tem errado (é do ORAS). O PKHeX e o Serebii concordam (auditoria em DexAvailabilityAuditTests).
            const string countable = """
                SELECT DISTINCT p.species_id, v.version_group_id FROM encounters e
                JOIN pokemon p ON p.id = e.pokemon_id JOIN versions v ON v.id = e.version_id
                JOIN location_areas a ON a.id = e.location_area_id JOIN locations l ON l.id = a.location_id
                WHERE e.method NOT IN ('colosseum-bonus-disc-jpn', 'colosseum-bonus-disc-us', 'pokemon-channel-pal', 'pokemon-ranger')
                  AND NOT (l.identifier = 'new-mauville' AND v.identifier IN ('sun', 'moon'))
                  AND p.species_id <> 801
                """;
            // Lugares abertos só por item de evento (Mystery Gift): contam, mas com o requisito "event_item".
            const string eventOnly = """
                (l.identifier IN ('navel-rock', 'birth-island', 'faraway-island', 'newmoon-island', 'flower-paradise',
                                  'sinnoh-hall-of-origin-1', 'sinnoh-hall-of-origin-2')
                 OR (l.identifier = 'southern-island' AND v.identifier IN ('ruby', 'sapphire', 'emerald'))
                 OR (l.identifier = 'kanto-altering-cave' AND p.species_id <> 41))
                """;
            // Presentes (gift, gift-egg) e trocas (npc-trade) à parte: viram Gift e Trade, não Encounter.
            var normal = countable + " AND NOT " + eventOnly;
            var encounterByGroup = Pairs(c, normal + " AND e.method NOT IN ('gift', 'gift-egg', 'npc-trade')");
            var giftByGroup = Pairs(c, normal + " AND e.method IN ('gift', 'gift-egg')");
            var tradeByGroup = Pairs(c, normal + " AND e.method = 'npc-trade'");
            var eventByGroup = Pairs(c, countable + " AND " + eventOnly);
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

                HashSet<int> Union(Dictionary<int, HashSet<int>> pairs) =>
                    groupIds.SelectMany(id => pairs.GetValueOrDefault(id) ?? []).ToHashSet();
                var encounters = Union(encounterByGroup);
                var gifts = Union(giftByGroup);
                var trades = Union(tradeByGroup);
                var events = Union(eventByGroup);
                var exists = groupIds.SelectMany(id => byGroup.GetValueOrDefault(id) ?? [])
                    .Concat(encounters).Concat(gifts).Concat(trades)
                    .Concat(RegionalDexes[g.Key].SelectMany(d => byDex.TryGetValue(d, out var set)
                        ? set
                        : throw new InvalidOperationException($"Pokédex ausente no dex.sqlite: {d}")))
                    .Where(id => species.TryGetValue(id, out var s) && s.Generation <= g.Generation)
                    .ToHashSet();
                encounters.IntersectWith(exists);
                gifts.IntersectWith(exists);
                trades.IntersectWith(exists);
                events.IntersectWith(exists);
                games[g.Key] = new GameInfo(g.Key, g.Generation, groupIds[0], groups[g.PokeApiVersionGroup].Order, gameVersions,
                    exists, encounters, gifts, trades, events, groupIds, g.Generation >= 7 ? FormsInGame(g.Key, regional, exists) : []);
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
                EfficacyPast = efficacyPast,
                PokemonTypesPast = pokemonTypesPast,
                SpeciesTypesPast = speciesTypesPast,
                Games = games,
                RegionalForms = regional,
                Breedable = breedable,
                Languages = languages,
                GroupOrders = groupOrders,
                Changelog = changelog,
                TypeIds = types.ToDictionary(t => t.Value.Item1, t => t.Key),
            };
        }

        /// <summary>
        /// Formas que não entram em "outras formas" (pelo identificador do PokeAPI): Totem, Gigantamax e Eternamax, o
        /// Zygarde com Power Construct (é habilidade), o Minior com a casca (só em batalha), os modos de montaria do
        /// Koraidon e do Miraidon, as fusões (Kyurem, Necrozma, Calyrex) e as que dependem de item (Dialga, Palkia e
        /// Giratina Origem, as máscaras do Ogerpon).
        /// </summary>
        private static readonly string[] NotKept =
        [
            "-totem", "-gmax", "-eternamax", "-power-construct", "-meteor", "koraidon-", "miraidon-", "kyurem-", "necrozma-",
            "calyrex-", "-origin", "ogerpon-",
        ];

        /// <summary>Formas cosméticas que dependem do item segurado (placas do Arceus, discos do Silvally e do Genesect).</summary>
        private static readonly string[] ItemForms = ["arceus-", "silvally-", "genesect-"];

        /// <summary>Espécies com muitas formas cosméticas, numa caixa só delas: Unown, Vivillon e Alcremie.</summary>
        private static readonly HashSet<int> ManySpecies = [201, 666, 869];

        private static readonly (string Prefix, string Region, int Generation)[] Regions =
        [
            ("Alolan Form", "alola", 7), ("Galarian Form", "galar", 8), ("Hisuian Form", "hisui", 8), ("Paldean Form", "paldea", 9),
        ];

        /// <summary>
        /// Formas regionais que existem no jogo, pela tabela de espécies e formas do PKHeX (<c>IsPresentInGame</c>): o
        /// PokeAPI não tem os golpes do Legends: Z-A, então não serve para todos os jogos. O índice de forma do PKHeX
        /// vem da lista de nomes de formas dele (<c>FormConverter.GetFormList</c>: "Alola", "Galar", "Hisui",
        /// "Paldea (...)"), porque a ordem das formas no PokeAPI não é a mesma (Mega e Gigantamax entram no meio).
        /// </summary>
        private static HashSet<int> FormsInGame(string gameKey, Dictionary<int, List<RegionalForm>> regional, HashSet<int> exists)
        {
            var result = new HashSet<int>();
            if (!PkhexAvailability.Games.TryGetValue(gameKey, out var pkhex))
                return result;
            var personal = PKHeX.Core.GameData.GetPersonal(pkhex.Versions[0]);
            var strings = Builder.BuilderData.Strings;
            foreach (var (speciesId, forms) in regional)
            {
                if (!exists.Contains(speciesId) || speciesId > personal.MaxSpeciesID)
                    continue;
                var names = PKHeX.Core.FormConverter.GetFormList((ushort)speciesId, strings.types, strings.forms, pkhex.Context);
                foreach (var form in forms.Where(f => f.Kind == FormKind.Regional))
                {
                    var index = Enumerable.Range(0, names.Length)
                        .Where(i => names[i].StartsWith(form.Region!, StringComparison.OrdinalIgnoreCase))
                        .Skip(form.Ordinal).Select(i => (int?)i).FirstOrDefault();
                    if (index is { } i && i < personal[(ushort)speciesId].FormCount && personal.IsPresentInGame((ushort)speciesId, (byte)i))
                        result.Add(form.PokemonId);
                }
            }
            return result;
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

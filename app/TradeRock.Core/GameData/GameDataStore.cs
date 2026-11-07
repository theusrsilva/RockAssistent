using System.Text.Json;
using Microsoft.Extensions.Logging;
using TradeRock.Core.Dex;
using TradeRock.Core.Games;
using TradeRock.Core.Vgc;

namespace TradeRock.Core.GameData;

/// <summary>
/// Os dados dos jogos embutidos (<c>assets/data/**/*.json</c>, recurso <c>data/&lt;caminho&gt;</c>): métodos de shiny,
/// chefes, treinadores, áreas, encontros, mapas e os dados do VGC (times sugeridos e regulamentação). Carrega e valida tudo no construtor; dado inválido é erro de build
/// (os testes carregam o mesmo conteúdo). Jogo sem arquivo = lista vazia.
/// </summary>
public sealed class GameDataStore
{
    public const string ResourcePrefix = "data/";

    internal static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        // Todo campo é obrigatório (nulo escrito como null), nada fora do esquema e nada nulo onde não pode.
        RespectRequiredConstructorParameters = true,
        RespectNullableAnnotations = true,
        UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow,
    };

    private readonly Dictionary<string, IReadOnlyList<BossBattle>> _bosses = [];
    private readonly Dictionary<string, IReadOnlyList<TrainerBattle>> _trainers = [];
    private readonly Dictionary<string, IReadOnlyList<GameArea>> _areas = [];
    private readonly Dictionary<string, IReadOnlyList<OwnEncounter>> _encounters = [];
    private readonly Dictionary<Region, MapGraph> _maps = [];
    // TODO(mapas): mapas no formato antigo, até as 11 regiões passarem para o novo.
    private readonly Dictionary<Region, LegacyMapGraph> _legacyMaps = [];
    private IReadOnlyList<VgcTeam> _vgcTeams = [];

    public GameDataStore(DexService dex, ILogger<GameDataStore> logger)
        : this(EmbeddedFiles(), MoveCheck(dex, logger), logger)
    {
    }

    /// <param name="files">Caminho relativo a <c>assets/data</c> (com <c>/</c>) → conteúdo JSON.</param>
    /// <param name="moveExists">Confere os golpes; nulo pula a conferência.</param>
    internal GameDataStore(IReadOnlyDictionary<string, string> files, Func<string, bool>? moveExists, ILogger logger)
    {
        var shiny = new List<ShinyMethod>();
        var errors = new List<string>();
        foreach (var (path, json) in files.OrderBy(f => f.Key, StringComparer.Ordinal))
        {
            object parsed;
            try
            {
                parsed = Parse(path, json);
            }
            catch (Exception ex) when (ex is JsonException or InvalidDataException or ArgumentException)
            {
                errors.Add($"{path}: {ex.Message}");
                continue;
            }

            var (folder, key) = Split(path);
            switch (parsed)
            {
                case DataFile<ShinyMethod> f: shiny.AddRange(f.Items); break;
                case DataFile<BossBattle> f: _bosses[key!] = f.Items; break;
                case DataFile<TrainerBattle> f: _trainers[key!] = f.Items; break;
                case DataFile<GameArea> f: _areas[key!] = f.Items; break;
                case DataFile<OwnEncounter> f: _encounters[key!] = f.Items; break;
                case MapGraph m: _maps[ParseRegion(key!)] = m; break;
                case LegacyMapGraph m: _legacyMaps[ParseRegion(key!)] = m; break;
                case DataFile<VgcTeam> f: _vgcTeams = f.Items; break;
                case VgcRegulation r: VgcRegulation = r; break;
                default: errors.Add($"{path}: pasta desconhecida ({folder})"); break;
            }
        }
        ShinyMethods = shiny;
        GamesWithTrainers = _trainers.Where(t => t.Value.Count > 0).Select(t => t.Key).OrderBy(k => k).ToList();

        if (moveExists is null)
            logger.LogWarning("Dados dos jogos: golpes não conferidos (Pokédex offline indisponível).");
        errors.AddRange(GameDataValidator.Validate(this, moveExists));
        errors.AddRange(VgcValidator.Validate(_vgcTeams, VgcRegulation));
        if (errors.Count > 0)
            throw new InvalidDataException("Dados dos jogos inválidos:\n" + string.Join("\n", errors));
    }

    public IReadOnlyList<ShinyMethod> ShinyMethods { get; }

    public IReadOnlyList<string> GamesWithTrainers { get; }

    public IReadOnlyList<BossBattle> Bosses(string gameKey) => _bosses.GetValueOrDefault(gameKey) ?? [];

    public IReadOnlyList<TrainerBattle> Trainers(string gameKey) => _trainers.GetValueOrDefault(gameKey) ?? [];

    public IReadOnlyList<GameArea> Areas(string gameKey) => _areas.GetValueOrDefault(gameKey) ?? [];

    public IReadOnlyList<OwnEncounter> Encounters(string gameKey) => _encounters.GetValueOrDefault(gameKey) ?? [];

    /// <summary>
    /// Encontros da espécie no formato da ficha da Pokédex (aba "Onde achar"), uma linha por versão; nulo quando o jogo
    /// não tem encontros próprios (vale o PokeAPI). A área vira o lugar; a chance em texto fica de fora (0); as
    /// condições são as nossas, separadas por vírgula, mais <c>alpha</c> nos alfas.
    /// </summary>
    public IReadOnlyList<EncounterInfo>? SpeciesEncounters(string gameKey, int species)
    {
        var own = Encounters(gameKey);
        if (own.Count == 0)
            return null;
        var names = Areas(gameKey).ToDictionary(a => a.Id, a => a.Name);
        return own.Where(e => e.Species == species)
            .SelectMany(e => e.Versions.Select(v => new EncounterInfo(v, names.GetValueOrDefault(e.Area) ?? e.Area, null, e.Method,
                e.MinLevel, e.MaxLevel, 0,
                [.. (e.Conditions?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? []),
                    .. (e.Alpha ? new[] { "alpha" } : [])])))
            .ToList();
    }

    /// <summary>Mapa da região no formato metrô; nulo sem arquivo (ou com o arquivo ainda no formato antigo).</summary>
    public MapGraph? Map(Region region) => _maps.GetValueOrDefault(region);

    /// <summary>TODO(mapas): mapa no formato antigo, até as 11 regiões passarem para o novo.</summary>
    public LegacyMapGraph? LegacyMap(Region region) => _legacyMaps.GetValueOrDefault(region);

    /// <summary>Times sugeridos do VGC (<c>vgc/teams.json</c>).</summary>
    public IReadOnlyList<VgcTeam> VgcTeams => _vgcTeams;

    /// <summary>Regulamentação vigente do VGC (<c>vgc/regulation.json</c>); nula sem o arquivo.</summary>
    public VgcRegulation? VgcRegulation { get; private set; }

    internal IEnumerable<string> GameKeys =>
        _bosses.Keys.Concat(_trainers.Keys).Concat(_areas.Keys).Concat(_encounters.Keys).Distinct();

    internal IEnumerable<MapGraph> Maps => _maps.Values;

    internal IEnumerable<LegacyMapGraph> LegacyMaps => _legacyMaps.Values;

    /// <summary>Os JSON embutidos, pelo caminho relativo a <c>assets/data</c>.</summary>
    public static IReadOnlyDictionary<string, string> EmbeddedFiles()
    {
        var assembly = typeof(GameDataStore).Assembly;
        var files = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var name in assembly.GetManifestResourceNames())
        {
            if (!name.StartsWith(ResourcePrefix, StringComparison.Ordinal) || !name.EndsWith(".json", StringComparison.Ordinal))
                continue;
            using var stream = assembly.GetManifestResourceStream(name)!;
            using var reader = new StreamReader(stream);
            // No Windows o %(RecursiveDir) vem com "\".
            files[name[ResourcePrefix.Length..].Replace('\\', '/')] = reader.ReadToEnd();
        }
        return files;
    }

    /// <summary>Desserializa um arquivo pelo caminho (a pasta diz o tipo). Lança se o formato estiver errado.</summary>
    public static object Parse(string path, string json)
    {
        var (folder, key) = Split(path);
        object? result = folder switch
        {
            null when key == "shiny-methods" => Read<DataFile<ShinyMethod>>(json),
            "bosses" => Read<DataFile<BossBattle>>(json),
            "trainers" => Read<DataFile<TrainerBattle>>(json),
            "areas" => Read<DataFile<GameArea>>(json),
            "encounters" => Read<DataFile<OwnEncounter>>(json),
            "maps" => ReadMap(json),
            "vgc" when key == "teams" => Read<DataFile<VgcTeam>>(json),
            "vgc" when key == "regulation" => Read<VgcRegulation>(json),
            _ => throw new InvalidDataException("arquivo fora do formato conhecido"),
        };
        if (result is null)
            throw new InvalidDataException("arquivo vazio");
        if (folder is "bosses" or "trainers" or "areas" or "encounters" && GameVersions.All.All(g => g.Key != key))
            throw new InvalidDataException($"jogo desconhecido: {key}");
        if (folder == "maps")
        {
            var region = ParseRegion(key!);
            var fileRegion = result is MapGraph g ? g.Region : ((LegacyMapGraph)result).Region;
            if (!string.Equals(fileRegion, region.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException($"região do arquivo ({fileRegion}) diferente do nome");
        }
        return result;
    }

    private static T? Read<T>(string json) where T : class => JsonSerializer.Deserialize<T>(json, Json);

    /// <summary>Mapa no formato novo; com "nodes" no lugar de "places", o antigo (TODO(mapas): tirar o antigo).</summary>
    private static object? ReadMap(string json)
    {
        bool legacy;
        using (var doc = JsonDocument.Parse(json))
            legacy = doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("nodes", out _);
        if (legacy)
            return Read<LegacyMapFile>(json) is { } l ? new LegacyMapGraph(l.Region, l.Nodes, l.Edges) : null;
        return Read<MapFile>(json) is { } m ? new MapGraph(m.Region, m.Grid, m.Terrain, m.Places, m.Routes, m.Unmapped) : null;
    }

    private static (string? Folder, string? Key) Split(string path)
    {
        var parts = path.Split('/');
        var file = Path.GetFileNameWithoutExtension(parts[^1]);
        return parts.Length switch
        {
            1 => (null, file),
            2 => (parts[0], file),
            _ => throw new InvalidDataException("pasta funda demais"),
        };
    }

    private static Region ParseRegion(string key) =>
        Enum.TryParse<Region>(key, ignoreCase: true, out var region)
            ? region
            : throw new InvalidDataException($"região desconhecida: {key}");

    private static Func<string, bool>? MoveCheck(DexService dex, ILogger logger)
    {
        if (!DexDatabase.ResourceExists)
            return null;
        try
        {
            return dex.MoveIdentifiers().Contains;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Dados dos jogos: não deu para ler os golpes da Pokédex offline.");
            return null;
        }
    }
}

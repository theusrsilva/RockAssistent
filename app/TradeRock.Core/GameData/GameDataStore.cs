using System.Text.Json;
using Microsoft.Extensions.Logging;
using TradeRock.Core.Dex;
using TradeRock.Core.Games;

namespace TradeRock.Core.GameData;

/// <summary>
/// Os dados dos jogos embutidos (<c>assets/data/**/*.json</c>, recurso <c>data/&lt;caminho&gt;</c>): métodos de shiny,
/// chefes, treinadores, áreas, encontros e mapas. Carrega e valida tudo no construtor; dado inválido é erro de build
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
                default: errors.Add($"{path}: pasta desconhecida ({folder})"); break;
            }
        }
        ShinyMethods = shiny;
        GamesWithTrainers = _trainers.Where(t => t.Value.Count > 0).Select(t => t.Key).OrderBy(k => k).ToList();

        if (moveExists is null)
            logger.LogWarning("Dados dos jogos: golpes não conferidos (Pokédex offline indisponível).");
        errors.AddRange(GameDataValidator.Validate(this, moveExists));
        if (errors.Count > 0)
            throw new InvalidDataException("Dados dos jogos inválidos:\n" + string.Join("\n", errors));
    }

    public IReadOnlyList<ShinyMethod> ShinyMethods { get; }

    public IReadOnlyList<string> GamesWithTrainers { get; }

    public IReadOnlyList<BossBattle> Bosses(string gameKey) => _bosses.GetValueOrDefault(gameKey) ?? [];

    public IReadOnlyList<TrainerBattle> Trainers(string gameKey) => _trainers.GetValueOrDefault(gameKey) ?? [];

    public IReadOnlyList<GameArea> Areas(string gameKey) => _areas.GetValueOrDefault(gameKey) ?? [];

    public IReadOnlyList<OwnEncounter> Encounters(string gameKey) => _encounters.GetValueOrDefault(gameKey) ?? [];

    public MapGraph? Map(Region region) => _maps.GetValueOrDefault(region);

    internal IEnumerable<string> GameKeys =>
        _bosses.Keys.Concat(_trainers.Keys).Concat(_areas.Keys).Concat(_encounters.Keys).Distinct();

    internal IEnumerable<MapGraph> Maps => _maps.Values;

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
            "maps" => Read<MapFile>(json) is { } m ? new MapGraph(m.Region, m.Nodes, m.Edges) : null,
            _ => throw new InvalidDataException("arquivo fora do formato conhecido"),
        };
        if (result is null)
            throw new InvalidDataException("arquivo vazio");
        if (folder is "bosses" or "trainers" or "areas" or "encounters" && GameVersions.All.All(g => g.Key != key))
            throw new InvalidDataException($"jogo desconhecido: {key}");
        if (folder == "maps")
        {
            var region = ParseRegion(key!);
            if (!string.Equals(((MapGraph)result).Region, region.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException($"região do arquivo ({((MapGraph)result).Region}) diferente do nome");
        }
        return result;
    }

    private static T? Read<T>(string json) where T : class => JsonSerializer.Deserialize<T>(json, Json);

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

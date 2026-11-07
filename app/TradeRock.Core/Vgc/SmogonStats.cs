using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using PKHeX.Core;

namespace TradeRock.Core.Vgc;

/// <summary>Um golpe, item, habilidade ou parceiro e em quantos times daquele Pokémon ele aparece (%).</summary>
public sealed record UsageShare(string Id, string Name, double Percent);

/// <summary>
/// Um Pokémon nas estatísticas. <see cref="Name"/> é o nome do Showdown (com a forma: "Rotom-Wash", "Swampert-Mega");
/// <see cref="Species"/> é a espécie nacional para o sprite (0 quando o nome não foi reconhecido).
/// </summary>
public sealed record UsageEntry(
    int Rank,
    string Name,
    ushort Species,
    double Usage,
    IReadOnlyList<UsageShare> Moves,
    IReadOnlyList<UsageShare> Items,
    IReadOnlyList<UsageShare> Abilities,
    IReadOnlyList<UsageShare> Teammates);

/// <summary>Os mais usados de um formato num mês (<see cref="Month"/> = "aaaa-mm", vazio quando não se sabe).</summary>
public sealed record UsageReport(string Format, string Month, long Battles, IReadOnlyList<UsageEntry> Pokemon);

/// <summary>O arquivo baixado e o mês que ele cobre.</summary>
public sealed record SmogonDownload(string File, DateOnly Month);

/// <summary>
/// Estatísticas de uso mensais do Smogon (Pokémon Showdown), no formato <c>chaos</c>:
/// <c>{info:{metagame, cutoff, "number of battles"}, data:{Espécie:{usage, "Raw count", Abilities, Items, Moves, Teammates, ...}}}</c>.
/// <c>usage</c> é a fração dos times com o Pokémon; as outras tabelas são contagens ponderadas, normalizadas aqui pela soma
/// de <c>Abilities</c> (cada Pokémon tem uma habilidade, então a soma é o peso total dele). Só GET, nada é enviado.
/// </summary>
public static class SmogonStats
{
    /// <summary>VGC do Pokémon Champions, Regulation M-C, jogadores com 1760+ (conferido em smogon.com/stats/2026-10/chaos).</summary>
    public const string DefaultFormat = "gen9championsvgc2026regmc-1760";

    /// <summary>O arquivo de outubro/2026 tinha 14,3 MB.</summary>
    public const long MaxBytes = 30L * 1024 * 1024;

    public const int TopCount = 50;

    private static readonly Regex FormatPattern = new(@"^[a-z0-9]{3,60}(-[0-9]{1,4})?$", RegexOptions.CultureInvariant);
    private static readonly Regex FilePattern = new(@"^(?<format>[a-z0-9]{3,60}(-[0-9]{1,4})?)-(?<month>[0-9]{4}-[0-9]{2})\.json$",
        RegexOptions.CultureInvariant);

    /// <summary>Nome de formato seguro para a URL e para o nome do arquivo (só minúsculas, dígitos e o corte "-1760").</summary>
    public static bool IsValidFormat(string? format) => format is not null && FormatPattern.IsMatch(format);

    public static Uri Url(string format, DateOnly month) =>
        new($"https://www.smogon.com/stats/{month:yyyy-MM}/chaos/{format}.json");

    public static string FileName(string format, DateOnly month) => $"{format}-{month.ToString("yyyy-MM", CultureInfo.InvariantCulture)}.json";

    /// <summary>O mês mais recente que pode existir: o Smogon publica o mês M no começo de M+1.</summary>
    public static DateOnly LatestMonth(DateTimeOffset now) => new DateOnly(now.Year, now.Month, 1).AddMonths(-1);

    /// <summary>
    /// Baixa o arquivo de <paramref name="month"/> (ou do mês anterior, se ainda não saiu: 404) para
    /// <c>&lt;dir&gt;/&lt;formato&gt;-&lt;aaaa-mm&gt;.json</c>. Confere o tamanho e o formato antes de trocar o arquivo.
    /// </summary>
    public static async Task<SmogonDownload> DownloadAsync(HttpClient http, string format, DateOnly month, string dir, CancellationToken ct)
    {
        if (!IsValidFormat(format))
            throw new InternetException("format_invalid");
        month = new DateOnly(month.Year, month.Month, 1);
        Directory.CreateDirectory(dir);
        for (var attempt = 0; ; attempt++)
        {
            using var response = await InternetHttp.GetAsync(http, Url(format, month), ct);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound && attempt == 0)
            {
                month = month.AddMonths(-1);
                continue;
            }
            if (!response.IsSuccessStatusCode)
                throw new InternetException(InternetHttp.StatusCode(response));

            var target = Path.Combine(dir, FileName(format, month));
            var part = target + ".part";
            try
            {
                await using (var file = File.Create(part))
                    await InternetHttp.CopyLimitedAsync(response, file, MaxBytes, ct);
                // Só troca o arquivo se for mesmo um chaos válido.
                await using (var check = File.OpenRead(part))
                    Parse(check);
                File.Move(part, target, overwrite: true);
            }
            catch (Exception e) when (e is JsonException or InvalidDataException or KeyNotFoundException or InvalidOperationException or FormatException)
            {
                throw new InternetException("invalid_file");
            }
            finally
            {
                try { File.Delete(part); } catch (IOException) { }
            }
            return new SmogonDownload(target, month);
        }
    }

    /// <summary>O mais recente da pasta: do <paramref name="preferredFormat"/> se houver, senão de qualquer formato.</summary>
    public static (string File, string Format, string Month)? Latest(string dir, string? preferredFormat)
    {
        if (!Directory.Exists(dir))
            return null;
        var files = Directory.EnumerateFiles(dir, "*.json")
            .Select(f => (File: f, Match: FilePattern.Match(Path.GetFileName(f))))
            .Where(x => x.Match.Success)
            .Select(x => (x.File, Format: x.Match.Groups["format"].Value, Month: x.Match.Groups["month"].Value))
            .ToList();
        var pick = files.Where(f => f.Format == preferredFormat).OrderByDescending(f => f.Month, StringComparer.Ordinal).FirstOrDefault();
        if (pick.File is null)
            pick = files.OrderByDescending(f => f.Month, StringComparer.Ordinal).FirstOrDefault();
        return pick.File is null ? null : pick;
    }

    /// <summary>Lê um arquivo <c>chaos</c>: os <see cref="TopCount"/> mais usados, com os principais golpes, itens etc.</summary>
    public static UsageReport Parse(Stream stream, string month = "")
    {
        using var doc = JsonDocument.Parse(stream, new JsonDocumentOptions { MaxDepth = 16 });
        var root = doc.RootElement;
        if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object)
            throw new InvalidDataException("sem data");
        var format = "";
        long battles = 0;
        if (root.TryGetProperty("info", out var info) && info.ValueKind == JsonValueKind.Object)
        {
            format = info.TryGetProperty("metagame", out var m) && m.ValueKind == JsonValueKind.String ? m.GetString() ?? "" : "";
            if (info.TryGetProperty("cutoff", out var c) && c.ValueKind == JsonValueKind.Number && c.TryGetDouble(out var cutoff) && cutoff > 0)
                format += "-" + ((int)cutoff).ToString(CultureInfo.InvariantCulture);
            if (info.TryGetProperty("number of battles", out var b) && b.ValueKind == JsonValueKind.Number && b.TryGetInt64(out var n))
                battles = n;
        }

        var ranked = data.EnumerateObject()
            .Where(p => p.Value.ValueKind == JsonValueKind.Object)
            .Select(p => (p.Name, p.Value, Usage: Number(p.Value, "usage")))
            .OrderByDescending(p => p.Usage)
            .ThenBy(p => p.Name, StringComparer.Ordinal)
            .Take(TopCount)
            .ToList();

        var names = Names.Instance;
        var entries = ranked.Select((p, i) =>
        {
            var weight = Sum(p.Value, "Abilities");
            if (weight <= 0)
                weight = Sum(p.Value, "Items");
            return new UsageEntry(
                i + 1,
                p.Name,
                names.Species(p.Name),
                Round(p.Usage * 100),
                Shares(p.Value, "Moves", weight, 10, names.Moves),
                Shares(p.Value, "Items", weight, 6, names.Items),
                Shares(p.Value, "Abilities", weight, 3, names.Abilities),
                Shares(p.Value, "Teammates", weight, 8, null));
        }).ToList();
        return new UsageReport(format, month, battles, entries);
    }

    /// <summary>Lê o arquivo salvo; o mês vem do nome (<c>formato-aaaa-mm.json</c>).</summary>
    public static UsageReport ParseFile(string path)
    {
        var match = FilePattern.Match(Path.GetFileName(path));
        using var stream = File.OpenRead(path);
        return Parse(stream, match.Success ? match.Groups["month"].Value : "");
    }

    private static double Number(JsonElement e, string name) =>
        e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : 0;

    private static double Sum(JsonElement e, string table) =>
        e.TryGetProperty(table, out var t) && t.ValueKind == JsonValueKind.Object
            ? t.EnumerateObject().Where(p => p.Value.ValueKind == JsonValueKind.Number).Sum(p => p.Value.GetDouble())
            : 0;

    private static List<UsageShare> Shares(JsonElement e, string table, double weight, int top, Dictionary<string, string>? names)
    {
        if (weight <= 0 || !e.TryGetProperty(table, out var t) || t.ValueKind != JsonValueKind.Object)
            return [];
        return t.EnumerateObject()
            .Where(p => p.Name.Length > 0 && p.Value.ValueKind == JsonValueKind.Number && p.Value.GetDouble() > 0)
            .Select(p => (p.Name, Count: p.Value.GetDouble()))
            .OrderByDescending(p => p.Count)
            .ThenBy(p => p.Name, StringComparer.Ordinal)
            .Take(top)
            .Select(p => new UsageShare(p.Name, names is not null && names.TryGetValue(Key(p.Name), out var n) ? n : p.Name,
                Round(Math.Min(100, p.Count / weight * 100))))
            .ToList();
    }

    private static double Round(double v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);

    /// <summary>Chave do Showdown: só letras e dígitos, minúsculas ("Close Combat" → "closecombat").</summary>
    internal static string Key(string text) =>
        string.Concat(text.Where(char.IsAsciiLetterOrDigit)).ToLowerInvariant();

    /// <summary>Nomes em inglês do PKHeX pela chave do Showdown.</summary>
    private sealed class Names
    {
        public static readonly Names Instance = new();

        public Dictionary<string, string> Moves { get; }
        public Dictionary<string, string> Items { get; }
        public Dictionary<string, string> Abilities { get; }
        private readonly Dictionary<string, ushort> _species = [];

        private Names()
        {
            var s = GameInfo.GetStrings("en");
            Moves = Map(s.movelist);
            Items = Map(s.itemlist);
            Abilities = Map(s.abilitylist);
            for (var i = 1; i < s.specieslist.Length; i++)
                _species.TryAdd(Key(s.specieslist[i]), (ushort)i);
        }

        private static Dictionary<string, string> Map(IReadOnlyList<string> list)
        {
            var map = new Dictionary<string, string>();
            for (var i = 1; i < list.Count; i++)
                if (!string.IsNullOrWhiteSpace(list[i]))
                    map.TryAdd(Key(list[i]), list[i]);
            return map;
        }

        /// <summary>"Rotom-Wash" → Rotom; "Ho-Oh" e "Porygon-Z" inteiros. Tira a forma do fim até achar.</summary>
        public ushort Species(string showdownName)
        {
            var name = showdownName;
            while (true)
            {
                if (_species.TryGetValue(Key(name), out var id))
                    return id;
                var dash = name.LastIndexOf('-');
                if (dash <= 0)
                    return 0;
                name = name[..dash];
            }
        }
    }
}

/// <summary>Os arquivos do Smogon já baixados (<c>&lt;DataDir&gt;/vgc</c>), lidos uma vez e guardados até mudarem.</summary>
public sealed class SmogonUsageStore(AppPaths paths)
{
    private readonly Lock _lock = new();
    private (string File, DateTime Written, UsageReport Report)? _cache;

    public string Dir => Path.Combine(paths.DataDir, "vgc");

    public UsageReport? Latest(string? preferredFormat)
    {
        if (SmogonStats.Latest(Dir, preferredFormat) is not { } found)
            return null;
        var written = File.GetLastWriteTimeUtc(found.File);
        lock (_lock)
            if (_cache is { } c && c.File == found.File && c.Written == written)
                return c.Report;
        UsageReport report;
        try
        {
            report = SmogonStats.ParseFile(found.File);
        }
        catch (Exception e) when (e is JsonException or InvalidDataException or IOException or InvalidOperationException)
        {
            return null;
        }
        // O nome do arquivo manda: é o formato que foi pedido.
        report = report with { Format = found.Format };
        lock (_lock)
            _cache = (found.File, written, report);
        return report;
    }
}

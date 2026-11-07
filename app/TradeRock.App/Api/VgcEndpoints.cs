using System.Text;
using TradeRock.Core.Builder;
using TradeRock.Core.Dex;
using TradeRock.Core.Storage;
using TradeRock.Core.GameData;
using TradeRock.Core.Trading;
using TradeRock.Core.Vgc;

namespace TradeRock.App.Api;

/// <summary>
/// Seção VGC dos guias: times sugeridos, regulamentação vigente e análise de um paste colado (sem internet). Com a
/// internet opcional ligada (D32): estatísticas do Smogon e links do pokepast.es. Os downloads só fazem GET.
/// </summary>
public static class VgcEndpoints
{
    public sealed record AnalyzeDto(string? Paste, string? Game = TeamAnalysis.GameKey);

    /// <summary>Um Pokémon do paste que ficou fora da análise (espécie que não existe no SV ou além do 6º).</summary>
    public sealed record SkippedDto(string Text, IReadOnlyList<string> Problems);

    public sealed record PasteFetchDto(string? Url);

    /// <summary>Um download do Smogon por vez (o arquivo tem ~15 MB).</summary>
    private static readonly SemaphoreSlim Refreshing = new(1, 1);

    /// <summary>O formato do Smogon da regulamentação vigente (<c>smogon_format</c>), ou o padrão.</summary>
    public static string SmogonFormat(GameDataStore store) =>
        store.VgcRegulation?.SmogonFormat is { } f && SmogonStats.IsValidFormat(f) ? f : SmogonStats.DefaultFormat;

    private static IResult NetworkError(InternetException e) =>
        Results.Json(new { error = "network_error", reason = e.Code }, statusCode: StatusCodes.Status502BadGateway);

    public static void MapVgc(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("/vgc");

        group.MapGet("/teams", (GameDataStore store) => store.VgcTeams.Select(t => new
        {
            t.Id,
            t.Name,
            t.Archetype,
            t.Regulation,
            t.DateIso,
            t.Paste,
            t.IdeaPt,
            t.IdeaEn,
            t.HowToPlayPt,
            t.HowToPlayEn,
            t.DangersPt,
            t.DangersEn,
            Members = VgcPaste.Members(t.Paste),
        }));

        group.MapGet("/regulation", (GameDataStore store) =>
            store.VgcRegulation is { } r ? Results.Ok(r) : Results.NotFound());

        group.MapGet("/usage", (GameDataStore store, SmogonUsageStore usage) =>
            usage.Latest(SmogonFormat(store)) is { } report ? Results.Ok(report) : Results.NotFound(new { error = "no_usage" }));

        group.MapPost("/usage/refresh", async (GameDataStore store, SmogonUsageStore usage, SettingsStore settings,
            IHttpClientFactory factory, TimeProvider time, ILoggerFactory loggers, CancellationToken ct) =>
        {
            if (!(await settings.GetAsync(InternetSettings.Key, InternetSettings.Default, ct)).Smogon)
                return Results.BadRequest(new { error = "internet_disabled" });
            if (!await Refreshing.WaitAsync(0, ct))
                return Results.Conflict(new { error = "busy" });
            var logger = loggers.CreateLogger("TradeRock.Vgc");
            try
            {
                var format = SmogonFormat(store);
                var month = SmogonStats.LatestMonth(time.GetUtcNow());
                logger.LogInformation("Smogon: download iniciado ({Format}, {Month:yyyy-MM}).", format, month);
                var done = await SmogonStats.DownloadAsync(factory.CreateClient(InternetHttp.ClientName), format, month, usage.Dir, ct);
                logger.LogInformation("Smogon: download concluído ({Month:yyyy-MM}).", done.Month);
                return usage.Latest(format) is { } report ? Results.Ok(report) : Results.NotFound(new { error = "no_usage" });
            }
            catch (InternetException e)
            {
                logger.LogWarning("Smogon: download falhou ({Code}).", e.Code);
                return NetworkError(e);
            }
            finally
            {
                Refreshing.Release();
            }
        });

        group.MapPost("/paste/fetch", async (PasteFetchDto body, SettingsStore settings, IHttpClientFactory factory,
            ILoggerFactory loggers, CancellationToken ct) =>
        {
            if (!(await settings.GetAsync(InternetSettings.Key, InternetSettings.Default, ct)).Pokepaste)
                return Results.BadRequest(new { error = "internet_disabled" });
            if (!Uri.TryCreate(body.Url?.Trim(), UriKind.Absolute, out var url) || PokePaste.RawUri(url) is null)
                return Results.BadRequest(new { error = "paste_host_not_allowed" });
            // O endereço do paste não vai para o log (pode ser o time de alguém).
            var logger = loggers.CreateLogger("TradeRock.Vgc");
            try
            {
                logger.LogInformation("pokepast.es: download iniciado.");
                var paste = await PokePaste.FetchAsync(factory.CreateClient(InternetHttp.ClientName), url, ct);
                logger.LogInformation("pokepast.es: download concluído.");
                return Results.Ok(new { paste });
            }
            catch (InternetException e) when (e.Code is "paste_not_found")
            {
                return Results.NotFound(new { error = e.Code });
            }
            catch (InternetException e) when (e.Code is "paste_too_big")
            {
                return Results.BadRequest(new { error = e.Code });
            }
            catch (InternetException e)
            {
                logger.LogWarning("pokepast.es: download falhou ({Code}).", e.Code);
                return NetworkError(e);
            }
        });

        group.MapPost("/analyze", async (AnalyzeDto body, DexService dex, CancellationToken ct) =>
        {
            if ((body.Game ?? TeamAnalysis.GameKey) != TeamAnalysis.GameKey)
                return Results.BadRequest(new { error = "invalid", errors = new[] { "game_invalid" } });
            if (string.IsNullOrWhiteSpace(body.Paste))
                return Results.BadRequest(new { error = "invalid", errors = new[] { "paste_invalid" } });
            if (Encoding.UTF8.GetByteCount(body.Paste) > VgcPaste.MaxBytes)
                return Results.BadRequest(new { error = "invalid", errors = new[] { "paste_too_big" } });

            // O treinador não importa aqui (só espécie, forma e golpes).
            var items = ShowdownImport.Parse(body.Paste, new TrainerSpec("VGC", 0, 0), TradeGame.Scarlet);
            var valid = items.Where(i => i.Spec is not null).ToList();
            if (valid.Count == 0)
                return Results.BadRequest(new { error = "invalid", errors = new[] { "paste_invalid" } });

            var team = valid.Take(6).Select(i => new TeamMember(i.Spec!.Species, i.Spec.Form, i.Spec.Moves)).ToList();
            var skipped = items.Where(i => i.Spec is null)
                .Concat(valid.Skip(6))
                .Select(i => new SkippedDto(i.Text.Split('\n')[0].Trim(), i.Spec is null ? i.Problems : ["team_too_big"]))
                .ToList();
            var report = await TeamAnalysis.AnalyzeAsync(team, dex, ct);
            return Results.Ok(new { report, skipped });
        });
    }
}

using TradeRock.Core.Dex;
using TradeRock.Core.Games;

namespace TradeRock.App.Api;

/// <summary>Pokédex offline (dados do PokeAPI). Só leitura: no modo servidor, valem para quem estiver logado.</summary>
public static class DexEndpoints
{
    private const string DefaultLang = "pt-BR";

    public static void MapDex(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("/dex");

        group.MapGet("/games", () => Results.Ok(GameVersions.All));

        group.MapGet("/species", async (DexService dex, string? text, string? type, int? generation, string? game,
            int? offset, int? limit, CancellationToken ct) =>
        {
            if (game is not null && !KnownGame(game))
                return UnknownGame();
            return Results.Ok(await dex.ListAsync(new DexQuery(text, type, generation, game, offset ?? 0, limit ?? 100), ct));
        });

        group.MapGet("/species/{id:int}", async (int id, string? game, string? lang, DexService dex, CancellationToken ct) =>
        {
            if (game is null || !KnownGame(game))
                return UnknownGame();
            return await dex.GetAsync(id, game, lang ?? DefaultLang, ct) is { } detail ? Results.Ok(detail) : Results.NotFound();
        });

        group.MapGet("/species/{id:int}/availability", async (int id, DexService dex, CancellationToken ct) =>
            Results.Ok(await dex.AvailabilityAsync(id, ct)));

        group.MapGet("/moves/{id:int}", async (int id, string? game, DexService dex, CancellationToken ct) =>
        {
            if (game is null || !KnownGame(game))
                return UnknownGame();
            return await dex.GetMoveAsync(id, game, ct) is { } move ? Results.Ok(move) : Results.NotFound();
        });

        group.MapGet("/abilities/{id:int}", async (int id, DexService dex, CancellationToken ct) =>
            await dex.GetAbilityAsync(id, ct) is { } ability ? Results.Ok(ability) : Results.NotFound());

        group.MapGet("/items/{id:int}", async (int id, DexService dex, CancellationToken ct) =>
            await dex.GetItemAsync(id, ct) is { } item ? Results.Ok(item) : Results.NotFound());

        group.MapGet("/types", async (int? generation, DexService dex, CancellationToken ct) =>
        {
            var gen = generation ?? 9;
            if (gen is < 1 or > 9)
                return Results.BadRequest(new { error = "invalid_generation" });
            return Results.Ok(await dex.GetTypeChartAsync(gen, ct));
        });
    }

    private static bool KnownGame(string key) => GameVersions.All.Any(g => g.Key == key);

    private static IResult UnknownGame() => Results.BadRequest(new { error = "unknown_game" });
}

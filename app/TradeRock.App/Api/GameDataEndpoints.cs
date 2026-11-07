using TradeRock.Core.Dex;
using TradeRock.Core.GameData;
using TradeRock.Core.Games;

namespace TradeRock.App.Api;

/// <summary>Dados dos jogos embutidos (métodos de shiny, chefes, treinadores, áreas, encontros e mapas). Só leitura.</summary>
public static class GameDataEndpoints
{
    public static void MapGameData(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("/data");

        group.MapGet("/shiny-methods", (GameDataStore store, string? game) =>
        {
            if (game is null)
                return Results.Ok(store.ShinyMethods);
            if (!KnownGame(game))
                return UnknownGame();
            return Results.Ok(store.ShinyMethods.Where(m => m.Games.Contains(game)).ToList());
        });

        group.MapGet("/maps/{region}", (string region, GameDataStore store) =>
        {
            if (!Enum.TryParse<Region>(region, ignoreCase: true, out var r) || int.TryParse(region, out _))
                return Results.BadRequest(new { error = "unknown_region" });
            return store.Map(r) is { } map ? Results.Ok(map) : Results.NotFound();
        });

        group.MapGet("/{game}/bosses", (string game, GameDataStore store) =>
            KnownGame(game) ? Results.Ok(store.Bosses(game)) : UnknownGame());
        group.MapGet("/{game}/trainers", (string game, GameDataStore store) =>
            KnownGame(game) ? Results.Ok(store.Trainers(game)) : UnknownGame());
        group.MapGet("/{game}/areas", (string game, GameDataStore store) =>
            KnownGame(game) ? Results.Ok(store.Areas(game)) : UnknownGame());
        group.MapGet("/{game}/encounters", (string game, GameDataStore store) =>
            KnownGame(game) ? Results.Ok(store.Encounters(game)) : UnknownGame());
        // Encontros de uma área (guias): os nossos ou os do PokeAPI, uma linha por método/níveis/chance.
        group.MapGet("/{game}/areas/{area}/encounters", async (string game, string area, GameDataStore store, DexService dex,
            CancellationToken ct) =>
        {
            if (!KnownGame(game))
                return UnknownGame();
            return await AreaEncounters.ListAsync(game, area, store, dex, ct) is { } rows ? Results.Ok(rows) : Results.NotFound();
        });
    }

    private static bool KnownGame(string key) => GameVersions.All.Any(g => g.Key == key);

    private static IResult UnknownGame() => Results.BadRequest(new { error = "unknown_game" });
}

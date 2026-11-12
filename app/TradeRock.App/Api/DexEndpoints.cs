using TradeRock.Core.Dex;
using TradeRock.Core.GameData;
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
            int? offset, int? limit, bool? forms, bool? otherForms, CancellationToken ct) =>
        {
            if (game is not null && !KnownGame(game))
                return UnknownGame();
            return Results.Ok(await dex.ListAsync(new DexQuery(text, type, generation, game, offset ?? 0, limit ?? 100, forms ?? false, otherForms ?? false), ct));
        });

        // Coleção ("tenho"): as marcações da visão (geral ou de um jogo) e o que está no cofre agora.
        group.MapGet("/collection", async (string? scope, DexCollectionStore store, CancellationToken ct) =>
            DexCollectionStore.ValidScope(scope ?? "") ? Results.Ok(await store.GetAsync(scope ?? "", ct)) : UnknownGame());

        group.MapPut("/collection", async (CollectionMarkDto body, DexCollectionStore store, CancellationToken ct) =>
        {
            var scope = body.Scope ?? "";
            if (!DexCollectionStore.ValidScope(scope))
                return UnknownGame();
            return await store.SetAsync(scope, new DexOwned(body.SpeciesId, body.FormId, body.Shiny), body.Owned, ct)
                ? Results.NoContent()
                : Results.NotFound(new { error = "not_listed" });
        });

        group.MapGet("/species/{id:int}", async (int id, string? game, string? lang, DexService dex, GameDataStore data,
            CancellationToken ct) =>
        {
            if (game is null || !KnownGame(game))
                return UnknownGame();
            if (await dex.GetAsync(id, game, lang ?? DefaultLang, ct) is not { } detail)
                return Results.NotFound();
            // BDSP, Arceus, SV e Z-A: "Onde achar" mostra os nossos encontros (o PokeAPI não cobre esses jogos direito).
            if (data.SpeciesEncounters(game, id) is { } own)
                detail = detail with { Encounters = own, ExistsInGame = detail.ExistsInGame || own.Count > 0 };
            return Results.Ok(detail);
        });

        group.MapGet("/species/{id:int}/availability", async (int id, DexService dex, GameDataStore data, CancellationToken ct) =>
            Results.Ok(await dex.AvailabilityAsync(id, data.Encounters, ct)));

        group.MapGet("/moves/{id:int}", async (int id, string? game, DexService dex, CancellationToken ct) =>
        {
            if (game is null || !KnownGame(game))
                return UnknownGame();
            return await dex.GetMoveAsync(id, game, ct) is { } move ? Results.Ok(move) : Results.NotFound();
        });

        // TMs/HMs/TRs do jogo (bloco :::tms dos guias).
        group.MapGet("/machines", async (string? game, DexService dex, CancellationToken ct) =>
        {
            if (game is null || !KnownGame(game))
                return UnknownGame();
            return Results.Ok(await dex.MachinesAsync(game, ct));
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

    private sealed record CollectionMarkDto(string? Scope, int SpeciesId, int FormId, bool Shiny, bool Owned);

    private static bool KnownGame(string key) => GameVersions.All.Any(g => g.Key == key);

    private static IResult UnknownGame() => Results.BadRequest(new { error = "unknown_game" });
}

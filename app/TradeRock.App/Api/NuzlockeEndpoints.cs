using TradeRock.Core.Nuzlocke;

namespace TradeRock.App.Api;

/// <summary>
/// Runs de Nuzlocke. Por conta, como o cofre: no modo servidor cada um vê só as suas runs (e os Pokémon delas).
/// Erros: 400 <c>{error}</c> (regras; <c>invalid_starter</c> para inicial que não é do jogo), 404 <c>run_not_found</c>/<c>mon_not_found</c>, 409 <c>run_finished</c>.
/// </summary>
public static class NuzlockeEndpoints
{
    public sealed record CreateDto(string? Name, string GameKey, string? Version, NuzlockeRules? Rules, string? Starter = null,
        string? StarterNickname = null);
    public sealed record UpdateDto(string? Name, string? Status, NuzlockeRules? Rules, string? Starter = null, string? StarterNickname = null);
    public sealed record AreaDto(string AreaId);
    public sealed record EncounterDto(string AreaId, int Species, int Form, string? Nickname, int Level, string Status, bool Shiny);
    public sealed record MonDto(string? Status, int? Level, string? Nickname, string? DeathCause, IReadOnlyList<string>? Moves,
        int? Species, int? Form);
    public sealed record FinishDto(string Status);

    public static void MapNuzlocke(this IEndpointRouteBuilder api)
    {
        var runs = api.MapGroup("/nuzlocke");

        runs.MapGet("/", (NuzlockeStore store, CancellationToken ct) => Run(() => store.ListAsync(ct)));
        runs.MapPost("/", (CreateDto body, NuzlockeStore store, CancellationToken ct) =>
            Run(() => store.CreateAsync(new NewRun(body.Name, body.GameKey, body.Version, body.Rules, body.Starter, body.StarterNickname), ct)));
        runs.MapGet("/{id:guid}", (Guid id, NuzlockeStore store, CancellationToken ct) => Run(() => store.GetAsync(id, ct)));
        runs.MapPut("/{id:guid}", (Guid id, UpdateDto body, NuzlockeStore store, CancellationToken ct) =>
            Run(() => store.UpdateAsync(id, new RunChanges(body.Name, body.Status, body.Rules, body.Starter, body.StarterNickname), ct)));
        runs.MapPut("/{id:guid}/area", (Guid id, AreaDto body, NuzlockeStore store, CancellationToken ct) =>
            Run(() => store.SetAreaAsync(id, body.AreaId, ct)));
        runs.MapPost("/{id:guid}/encounter", (Guid id, EncounterDto body, NuzlockeStore store, CancellationToken ct) =>
            Run(() => store.EncounterAsync(id,
                new NewEncounter(body.AreaId, body.Species, body.Form, body.Nickname, body.Level, body.Status, body.Shiny), ct)));
        runs.MapPut("/{id:guid}/mon/{monId:guid}", (Guid id, Guid monId, MonDto body, NuzlockeStore store, CancellationToken ct) =>
            Run(() => store.UpdateMonAsync(id, monId,
                new MonChanges(body.Status, body.Level, body.Nickname, body.DeathCause, body.Moves, body.Species, body.Form), ct)));
        runs.MapGet("/{id:guid}/mon/{monId:guid}/learnset", (Guid id, Guid monId, NuzlockeStore store, CancellationToken ct) =>
            Run(() => store.LearnsetAsync(id, monId, ct)));
        runs.MapPost("/{id:guid}/boss/{bossId}/defeated", (Guid id, string bossId, NuzlockeStore store, CancellationToken ct) =>
            Run(() => store.DefeatBossAsync(id, bossId, ct)));
        runs.MapPost("/{id:guid}/finish", (Guid id, FinishDto body, NuzlockeStore store, CancellationToken ct) =>
            Run(() => store.FinishAsync(id, body.Status, ct)));
        runs.MapDelete("/{id:guid}", async (Guid id, NuzlockeStore store, CancellationToken ct) =>
        {
            try
            {
                await store.DeleteAsync(id, ct);
                return Results.NoContent();
            }
            catch (NuzlockeException ex)
            {
                return Error(ex);
            }
        });
    }

    private static async Task<IResult> Run<T>(Func<Task<T>> action)
    {
        try
        {
            return Results.Ok(await action());
        }
        catch (NuzlockeException ex)
        {
            return Error(ex);
        }
    }

    private static IResult Error(NuzlockeException ex) => Results.Json(new { error = ex.Code }, statusCode: ex.Status);
}

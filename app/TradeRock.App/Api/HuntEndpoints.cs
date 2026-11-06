using TradeRock.Core.Hunts;

namespace TradeRock.App.Api;

/// <summary>
/// Contador Hunt (hunts de shiny). Por conta, como o cofre: no modo servidor cada um vê só as suas hunts.
/// Erros: 400 <c>{error}</c> (regras), 404 <c>hunt_not_found</c>, 409 <c>hunt_finished</c>.
/// </summary>
public static class HuntEndpoints
{
    public sealed record CreateDto(int Species, int Form, string GameKey, string MethodId, IReadOnlyDictionary<string, string>? Modifiers);
    public sealed record UpdateDto(int? Count, IReadOnlyDictionary<string, string>? Modifiers, string? Notes, long? ElapsedSeconds);
    public sealed record IncrementDto(int Delta);
    public sealed record FinishDto(bool Found);

    public static void MapHunts(this IEndpointRouteBuilder api)
    {
        var hunts = api.MapGroup("/hunts");

        // Sem "all": só as em andamento. "all=1" (ou true) traz também as terminadas.
        hunts.MapGet("/", (string? all, HuntStore store, CancellationToken ct) =>
            Run(() => store.ListAsync(all is "1" || string.Equals(all, "true", StringComparison.OrdinalIgnoreCase), ct)));
        hunts.MapGet("/{id:guid}", (Guid id, HuntStore store, CancellationToken ct) => Run(() => store.GetAsync(id, ct)));
        hunts.MapPost("/", (CreateDto body, HuntStore store, CancellationToken ct) =>
            Run(() => store.CreateAsync(new NewHunt(body.Species, body.Form, body.GameKey, body.MethodId, body.Modifiers), ct)));
        hunts.MapPut("/{id:guid}", (Guid id, UpdateDto body, HuntStore store, CancellationToken ct) =>
            Run(() => store.UpdateAsync(id, new HuntChanges(body.Count, body.Modifiers, body.Notes, body.ElapsedSeconds), ct)));
        hunts.MapPost("/{id:guid}/increment", (Guid id, IncrementDto body, HuntStore store, CancellationToken ct) =>
            Run(() => store.IncrementAsync(id, body.Delta, ct)));
        hunts.MapPost("/{id:guid}/finish", (Guid id, FinishDto body, HuntStore store, CancellationToken ct) =>
            Run(() => store.FinishAsync(id, body.Found, ct)));
        hunts.MapDelete("/{id:guid}", async (Guid id, HuntStore store, CancellationToken ct) =>
        {
            try
            {
                await store.DeleteAsync(id, ct);
                return Results.NoContent();
            }
            catch (HuntException ex)
            {
                return Error(ex);
            }
        });
        hunts.MapGet("/species/{id:int}/games", (int id, HuntStore store, CancellationToken ct) =>
            Run(() => store.GamesForSpeciesAsync(id, ct)));
    }

    private static async Task<IResult> Run<T>(Func<Task<T>> action)
    {
        try
        {
            return Results.Ok(await action());
        }
        catch (HuntException ex)
        {
            return Error(ex);
        }
    }

    private static IResult Error(HuntException ex) => Results.Json(new { error = ex.Code }, statusCode: ex.Status);
}

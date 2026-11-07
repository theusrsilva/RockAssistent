using TradeRock.Core.Storage;

namespace TradeRock.App.Api;

/// <summary>
/// Histórico unificado (trocas, hunts e runs de Nuzlocke terminadas). Por conta, como as trocas.
/// <c>GET /api/history?kind&amp;limit</c>: <c>kind</c> é trade|raid|gift|circle|hunt|nuzlocke (vazio = todos);
/// <c>limit</c> padrão 50, máximo 500. Tipo desconhecido: 400 <c>invalid_kind</c>.
/// </summary>
public static class HistoryEndpoints
{
    public static void MapHistory(this IEndpointRouteBuilder api)
    {
        api.MapGet("/history", async (string? kind, int? limit, HistoryFeed feed, CancellationToken ct) =>
        {
            if (string.IsNullOrEmpty(kind))
                kind = null;
            else if (!HistoryFeed.IsKind(kind))
                return Results.BadRequest(new { error = "invalid_kind" });
            return Results.Ok(await feed.ListAsync(kind, limit ?? HistoryFeed.DefaultLimit, ct));
        });
    }
}

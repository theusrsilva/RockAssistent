using Microsoft.EntityFrameworkCore;
using TradeRock.Core.Dex;
using TradeRock.Core.Games;
using TradeRock.Core.Trading;

namespace TradeRock.Core.Storage;

/// <summary>
/// Um item do Histórico unificado. <see cref="Kind"/>: <c>trade</c>, <c>raid</c>, <c>gift</c>, <c>circle</c>,
/// <c>hunt</c> ou <c>nuzlocke</c>. Título e subtítulo são dados neutros (a interface monta o texto traduzido):
/// troca = treinador e status; hunt = espécie (inglês) e método; run = nome e status; atividade = nome do presente
/// ou do evento (senão o tipo) e o tipo da raid/círculo (senão o status).
/// </summary>
public sealed record HistoryItem(
    string Kind, Guid Id, DateTimeOffset At, string Title, string Subtitle, string? GameKey, int? Species, bool Ok);

/// <summary>
/// Junta trocas, atividades (raids, Mystery Gift, Union Circle: <see cref="ActivityRecord"/>), hunts terminadas e
/// runs de Nuzlocke terminadas numa lista só, da mais nova para a mais velha.
/// O dono vem dos filtros globais do <see cref="TradeRockDb"/> (cada conta vê só o seu).
/// </summary>
public sealed class HistoryFeed(IDbContextFactory<TradeRockDb> dbFactory, DexService dex)
{
    public const int DefaultLimit = 50;
    public const int MaxLimit = 500;

    public static readonly IReadOnlyList<string> Kinds = ["trade", "raid", "gift", "circle", "hunt", "nuzlocke"];

    public static bool IsKind(string? kind) => kind is not null && Kinds.Contains(kind);

    /// <param name="kind">Um dos <see cref="Kinds"/>; null = todos.</param>
    public async Task<IReadOnlyList<HistoryItem>> ListAsync(string? kind, int limit, CancellationToken ct)
    {
        if (kind is not null && !IsKind(kind))
            throw new ArgumentException($"Tipo desconhecido: {kind}", nameof(kind));
        limit = Math.Clamp(limit, 1, MaxLimit);
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var items = new List<HistoryItem>();

        if (kind is null or "trade")
            items.AddRange(await TradesAsync(db, limit, ct));
        if (kind is null or "hunt")
            items.AddRange(await HuntsAsync(db, limit, ct));
        if (kind is null or "nuzlocke")
            items.AddRange(await RunsAsync(db, limit, ct));
        if (kind is null or "raid" or "gift" or "circle")
            items.AddRange(await ActivitiesAsync(db, kind, limit, ct));

        return items.OrderByDescending(i => i.At).Take(limit).ToList();
    }

    private static async Task<IEnumerable<HistoryItem>> TradesAsync(TradeRockDb db, int limit, CancellationToken ct)
    {
        var trades = await db.Trades.AsNoTracking().Include(t => t.Entries)
            .OrderByDescending(t => t.EndedAt ?? t.StartedAt).Take(limit).ToListAsync(ct);

        // Sprite: o primeiro recebido; sem recebido, o primeiro enviado (os da lixeira também contam).
        var first = trades.ToDictionary(t => t.Id, t =>
        {
            var entries = t.Entries.OrderBy(e => e.TradeIndex).ToList();
            return entries.Select(e => e.ReceivedPokemonId).FirstOrDefault(id => id is not null)
                ?? entries.Select(e => (Guid?)e.SentPokemonId).FirstOrDefault();
        });
        var ids = first.Values.OfType<Guid>().Distinct().ToList();
        var species = await db.Pokemon.AsNoTracking().Where(p => ids.Contains(p.Id))
            .Select(p => new { p.Id, p.Species }).ToDictionaryAsync(p => p.Id, p => (int)p.Species, ct);

        return trades.Select(t => new HistoryItem(
            "trade", t.Id, t.EndedAt ?? t.StartedAt, t.TrainerName, Snake(t.Status.ToString()),
            GameVersions.ByFamily(GameCatalog.Of(t.Game).Family)?.Key,
            first[t.Id] is { } pid && species.TryGetValue(pid, out var s) ? s : null,
            t.Status == TradeSessionStatus.Completed));
    }

    private async Task<IEnumerable<HistoryItem>> HuntsAsync(TradeRockDb db, int limit, CancellationToken ct)
    {
        var hunts = await db.Hunts.AsNoTracking().Where(h => h.EndedAt != null)
            .OrderByDescending(h => h.EndedAt).Take(limit).ToListAsync(ct);
        return hunts.Select(h => new HistoryItem(
            "hunt", h.Id, h.EndedAt!.Value, dex.SpeciesName(h.Species) ?? $"#{h.Species}", h.MethodId,
            h.GameKey, h.Species, h.Found));
    }

    private static async Task<IEnumerable<HistoryItem>> RunsAsync(TradeRockDb db, int limit, CancellationToken ct)
    {
        var runs = await db.NuzlockeRuns.AsNoTracking().Where(r => r.Status == "champion" || r.Status == "wipe")
            .OrderByDescending(r => r.EndedAt).ThenByDescending(r => r.StartedAt).Take(limit).ToListAsync(ct);
        return runs.Select(r => new HistoryItem(
            "nuzlocke", r.Id, r.EndedAt ?? r.StartedAt, r.Name, r.Status, r.GameKey, null, r.Status == "champion"));
    }

    private static async Task<IEnumerable<HistoryItem>> ActivitiesAsync(TradeRockDb db, string? kind, int limit, CancellationToken ct)
    {
        var query = db.ActivityRecords.AsNoTracking();
        if (kind is not null)
            query = query.Where(a => a.Kind == kind);
        var records = await query.OrderByDescending(a => a.EndedAt).Take(limit).ToListAsync(ct);
        return records.Select(a => new HistoryItem(
            a.Kind, a.Id, a.EndedAt, a.Label ?? a.SubKind ?? a.Kind, a.SubKind ?? a.Status,
            GameVersions.ByFamily(GameCatalog.Of(a.Game).Family)?.Key, a.Species, a.Ok));
    }

    /// <summary>PartiallyCompleted → partially_completed (como o JSON dos enums).</summary>
    private static string Snake(string name) =>
        string.Concat(name.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
}

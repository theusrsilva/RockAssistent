using Microsoft.EntityFrameworkCore;
using TradeRock.Core.Games;
using TradeRock.Core.Storage;

namespace TradeRock.Core.Dex;

/// <summary>
/// A coleção da Pokédex ("tenho"), por conta como o cofre. Cada visão tem as suas marcações: a geral ("") e uma por jogo
/// (chave de <see cref="GameVersions"/>). Além do que a pessoa marcou, devolve o que está no cofre agora, lido na hora
/// (nada é copiado): na visão geral, o cofre inteiro; na de um jogo, só os Pokémon do formato daquele jogo.
/// </summary>
public sealed class DexCollectionStore(IDbContextFactory<TradeRockDb> dbFactory, DexService dex, TimeProvider time)
{
    public const string General = "";

    public static bool ValidScope(string scope) => scope == General || GameVersions.All.Any(g => g.Key == scope);

    public async Task<DexCollection> GetAsync(string scope, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var marks = await db.DexMarks.AsNoTracking().Where(m => m.Scope == scope)
            .OrderBy(m => m.SpeciesId).ThenBy(m => m.FormId).ThenBy(m => m.Shiny)
            .Select(m => new DexOwned(m.SpeciesId, m.FormId, m.Shiny)).ToListAsync(ct);

        var stored = await db.Pokemon.AsNoTracking().Where(p => p.DeletedAt == null && !p.IsEgg)
            .Select(p => new { p.Species, p.Form, p.IsShiny, p.Format, p.Gender }).Distinct().ToListAsync(ct);
        var vault = stored
            .Select(p => (Pokemon: p, Format: VaultFormats.Get(p.Format)))
            .Where(x => scope == General || x.Format.Game?.Key == scope)
            .Select(x => new DexOwned(x.Pokemon.Species, dex.ListFormId(x.Pokemon.Species, x.Pokemon.Form, x.Format.Context, x.Pokemon.Gender), x.Pokemon.IsShiny))
            .Distinct()
            .OrderBy(o => o.SpeciesId).ThenBy(o => o.FormId).ThenBy(o => o.Shiny)
            .ToList();
        return new DexCollection(scope, marks, vault);
    }

    /// <summary>Marca ou desmarca. Devolve false quando a linha não existe na lista da Pokédex.</summary>
    public async Task<bool> SetAsync(string scope, DexOwned entry, bool owned, CancellationToken ct)
    {
        if (!dex.IsListed(entry.SpeciesId, entry.FormId))
            return false;
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var existing = await db.DexMarks.SingleOrDefaultAsync(m =>
            m.Scope == scope && m.SpeciesId == entry.SpeciesId && m.FormId == entry.FormId && m.Shiny == entry.Shiny, ct);
        if (owned && existing is null)
            db.DexMarks.Add(new DexMark
            {
                Scope = scope, SpeciesId = entry.SpeciesId, FormId = entry.FormId, Shiny = entry.Shiny, MarkedAt = time.GetUtcNow(),
            });
        else if (!owned && existing is not null)
            db.DexMarks.Remove(existing);
        await db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Importa marcações de uma vez (o arquivo exportado de uma lista de living dex): soma às da visão, sem desmarcar
    /// nada. Linha que a lista da Pokédex não tem é ignorada; a que já estava marcada, ou veio repetida, só é contada.
    /// </summary>
    public async Task<DexImportResult> ImportAsync(string scope, IReadOnlyCollection<DexOwned> entries, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var marked = (await db.DexMarks.AsNoTracking().Where(m => m.Scope == scope)
            .Select(m => new DexOwned(m.SpeciesId, m.FormId, m.Shiny)).ToListAsync(ct)).ToHashSet();
        var now = time.GetUtcNow();
        int added = 0, already = 0, skipped = 0;
        foreach (var entry in entries)
        {
            if (!dex.IsListed(entry.SpeciesId, entry.FormId))
            {
                skipped++;
                continue;
            }
            if (!marked.Add(entry))
            {
                already++;
                continue;
            }
            db.DexMarks.Add(new DexMark
            {
                Scope = scope, SpeciesId = entry.SpeciesId, FormId = entry.FormId, Shiny = entry.Shiny, MarkedAt = now,
            });
            added++;
        }
        await db.SaveChangesAsync(ct);
        return new DexImportResult(added, already, skipped);
    }
}

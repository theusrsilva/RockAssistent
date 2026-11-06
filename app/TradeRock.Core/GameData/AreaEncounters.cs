using TradeRock.Core.Dex;
using TradeRock.Core.Games;

namespace TradeRock.Core.GameData;

/// <summary>
/// Linha da tabela de encontros de uma área (bloco <c>:::encounters</c> dos guias). <see cref="Chance"/> é texto: "20%"
/// nos do PokeAPI, o que escrevemos nos nossos ("fixed", "common"...). <see cref="Versions"/>: versões do PokeAPI.
/// </summary>
public sealed record AreaEncounterRow(int Species, int Form, string SpeciesName, string Method, int MinLevel, int MaxLevel,
    string? Chance, string? Conditions, IReadOnlyList<string> Versions, bool Alpha, bool Verified);

/// <summary>Encontros de uma área, sem juntar por espécie (o Nuzlocke junta; o guia mostra método e chance de cada um).</summary>
public static class AreaEncounters
{
    /// <summary>
    /// BDSP, Arceus, SV e Z-A: os nossos (<c>assets/data/encounters</c>); os outros: os do PokeAPI pelo
    /// <c>PokeApiLocationId</c> da área, com as linhas iguais em duas versões juntadas numa. Área desconhecida = null.
    /// </summary>
    public static async Task<IReadOnlyList<AreaEncounterRow>?> ListAsync(string gameKey, string areaId, GameDataStore data,
        DexService dex, CancellationToken ct)
    {
        var area = data.Areas(gameKey).FirstOrDefault(a => a.Id == areaId);
        if (area is null)
            return null;
        string Name(int species) => (DexDatabase.ResourceExists ? dex.SpeciesName(species) : null) ?? $"#{species}";

        var own = data.Encounters(gameKey);
        if (own.Count > 0)
            return own.Where(e => e.Area == area.Id)
                .Select(e => new AreaEncounterRow(e.Species, e.Form, Name(e.Species), e.Method, e.MinLevel, e.MaxLevel, e.Chance,
                    e.Conditions, e.Versions, e.Alpha, e.Verified))
                .ToList();

        if (area.PokeApiLocationId is not { } location || !DexDatabase.ResourceExists)
            return [];
        var order = GameVersions.ByKey(gameKey).PokeApiVersions.ToList();
        return (await dex.LocationEncountersAsync(location, gameKey, ct))
            .GroupBy(e => (e.Species, e.Form, e.Method, e.MinLevel, e.MaxLevel, e.Chance))
            .Select(g => new AreaEncounterRow(g.Key.Species, g.Key.Form, Name(g.Key.Species), g.Key.Method, g.Key.MinLevel,
                g.Key.MaxLevel, g.Key.Chance > 0 ? $"{g.Key.Chance}%" : null, null,
                g.Select(e => e.Version).Distinct().OrderBy(v => order.IndexOf(v) is var i and >= 0 ? i : int.MaxValue).ToList(),
                false, true))
            .OrderBy(r => r.Method).ThenByDescending(r => r.Chance is null ? 0 : int.Parse(r.Chance.TrimEnd('%')))
            .ThenBy(r => r.Species)
            .ToList();
    }
}

namespace TradeRock.Core.GameData;

/// <summary>
/// Como os nossos encontros (<c>assets/data/encounters</c>) e os do PokeAPI se juntam. No BDSP, no Arceus, no SV e no
/// Z-A só valem os nossos (o PokeAPI não cobre esses jogos). Nos outros jogos com arquivo próprio (ORAS, BW, B2W2, XY,
/// SM, USUM) o arquivo completa e corrige o PokeAPI: numa área (e versão) que tem linhas nossas, as linhas selvagens
/// do PokeAPI saem e ficam as nossas; encontros fixos, presentes e trocas do PokeAPI continuam.
/// </summary>
public static class EncounterSources
{
    private static readonly HashSet<string> OwnOnlyGames = ["bdsp", "pla", "sv", "za"];

    /// <summary>Métodos do PokeAPI que não são da tabela selvagem da área: continuam mesmo com linhas nossas na área.</summary>
    private static readonly HashSet<string> KeptMethods =
    [
        "static", "gift", "gift-egg", "npc-trade", "devon-scope", "roaming-grass", "roaming-water", "pokeflute",
        "squirt-bottle", "wailmer-pail", "island-scan",
    ];

    /// <summary>Métodos nossos que não são da tabela selvagem (ficam ao lado dos do PokeAPI, sem tirar nada).</summary>
    private static readonly HashSet<string> OwnKeptMethods = ["fixed", "gift", "trade", "roaming", "island_scan"];

    /// <summary>
    /// Condições do PokeAPI que são o estado normal do jogo (rádio desligado, sem Poké Radar, sem enxame, sem cartucho
    /// no slot 2): saem do texto, e as linhas que só diferiam nelas somam a chance.
    /// </summary>
    public static readonly IReadOnlyList<string> DefaultConditions = ["radio-off", "radar-off", "swarm-no", "slot2-none"];

    /// <summary>Só os nossos encontros valem neste jogo?</summary>
    public static bool OwnOnly(string gameKey) => OwnOnlyGames.Contains(gameKey);

    public static bool IsWild(string pokeApiMethod) => !KeptMethods.Contains(pokeApiMethod);

    /// <summary>
    /// Diz se uma linha do PokeAPI (local, versão, método, espécie) fica, dado o arquivo próprio do jogo: sai a linha
    /// selvagem de área coberta por nós naquela versão, e a fixa/presente/troca que nós também escrevemos lá.
    /// </summary>
    public static Func<int, string, string, int, bool> PokeApiFilter(string gameKey, GameDataStore data)
    {
        var own = data.Encounters(gameKey);
        if (own.Count == 0)
            return (_, _, _, _) => true;
        if (OwnOnly(gameKey))
            return (_, _, _, _) => false;
        var locations = data.Areas(gameKey).Where(a => a.PokeApiLocationId is not null)
            .ToDictionary(a => a.Id, a => a.PokeApiLocationId!.Value);
        var wild = new HashSet<(int Location, string Version)>();
        var kept = new HashSet<(int Location, string Version, int Species)>();
        foreach (var e in own)
        {
            if (!locations.TryGetValue(e.Area, out var location))
                continue;
            foreach (var v in e.Versions)
            {
                if (OwnKeptMethods.Contains(e.Method))
                    kept.Add((location, v, e.Species));
                else
                    wild.Add((location, v));
            }
        }
        return (location, version, method, species) =>
            IsWild(method) ? !wild.Contains((location, version)) : !kept.Contains((location, version, species));
    }

    /// <summary>"10%" → 10; texto ("common", "fixed") ou nulo → 0.</summary>
    public static int Percent(string? chance) =>
        chance is not null && chance.EndsWith('%') && int.TryParse(chance.AsSpan(0, chance.Length - 1), out var n) ? n : 0;
}

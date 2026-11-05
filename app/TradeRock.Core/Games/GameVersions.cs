namespace TradeRock.Core.Games;

/// <summary>Região onde o jogo se passa. O Legends: Z-A fica só em Lumiose.</summary>
public enum Region
{
    Kanto,
    Johto,
    Hoenn,
    Sinnoh,
    Unova,
    Kalos,
    Alola,
    Galar,
    Hisui,
    Paldea,
    Lumiose,
}

/// <summary>
/// Um grupo de versões da série principal (as versões de um par juntas, como o PokeAPI as agrupa).
/// Os identificadores do PokeAPI são os de <c>version_groups.csv</c> e <c>versions.csv</c>.
/// </summary>
public sealed record GameVersionGroup(
    string Key,
    string Name,
    int Generation,
    Region Region,
    string PokeApiVersionGroup,
    IReadOnlyList<string> PokeApiVersions,
    GameFamily? Family,
    bool HasShinyCharm,
    bool HasMasuda);

/// <summary>Todos os jogos da série principal, da Gen 1 à Gen 9, em ordem de lançamento.</summary>
public static class GameVersions
{
    // Amuleto Brilhante desde o Black 2/White 2 (inclui o Let's Go). Método Masuda desde o Diamond/Pearl,
    // menos nos jogos sem breeding (Let's Go, Arceus e Z-A).
    public static IReadOnlyList<GameVersionGroup> All { get; } =
    [
        new("rb", "Red / Blue", 1, Region.Kanto, "red-blue", ["red", "blue"], null, false, false),
        new("y", "Yellow", 1, Region.Kanto, "yellow", ["yellow"], null, false, false),
        new("gs", "Gold / Silver", 2, Region.Johto, "gold-silver", ["gold", "silver"], null, false, false),
        new("c", "Crystal", 2, Region.Johto, "crystal", ["crystal"], null, false, false),
        new("rs", "Ruby / Sapphire", 3, Region.Hoenn, "ruby-sapphire", ["ruby", "sapphire"], null, false, false),
        new("e", "Emerald", 3, Region.Hoenn, "emerald", ["emerald"], null, false, false),
        new("frlg", "FireRed / LeafGreen", 3, Region.Kanto, "firered-leafgreen", ["firered", "leafgreen"],
            GameFamily.Frlg, false, false),
        new("dp", "Diamond / Pearl", 4, Region.Sinnoh, "diamond-pearl", ["diamond", "pearl"], null, false, true),
        new("pt", "Platinum", 4, Region.Sinnoh, "platinum", ["platinum"], null, false, true),
        new("hgss", "HeartGold / SoulSilver", 4, Region.Johto, "heartgold-soulsilver", ["heartgold", "soulsilver"],
            null, false, true),
        new("bw", "Black / White", 5, Region.Unova, "black-white", ["black", "white"], null, false, true),
        new("b2w2", "Black 2 / White 2", 5, Region.Unova, "black-2-white-2", ["black-2", "white-2"], null, true, true),
        new("xy", "X / Y", 6, Region.Kalos, "x-y", ["x", "y"], null, true, true),
        new("oras", "Omega Ruby / Alpha Sapphire", 6, Region.Hoenn, "omega-ruby-alpha-sapphire",
            ["omega-ruby", "alpha-sapphire"], null, true, true),
        new("sm", "Sun / Moon", 7, Region.Alola, "sun-moon", ["sun", "moon"], null, true, true),
        new("usum", "Ultra Sun / Ultra Moon", 7, Region.Alola, "ultra-sun-ultra-moon", ["ultra-sun", "ultra-moon"],
            null, true, true),
        new("lgpe", "Let's Go, Pikachu! / Let's Go, Eevee!", 7, Region.Kanto, "lets-go-pikachu-lets-go-eevee",
            ["lets-go-pikachu", "lets-go-eevee"], GameFamily.Lgpe, true, false),
        new("swsh", "Sword / Shield", 8, Region.Galar, "sword-shield", ["sword", "shield"], GameFamily.Swsh, true, true),
        new("bdsp", "Brilliant Diamond / Shining Pearl", 8, Region.Sinnoh, "brilliant-diamond-shining-pearl",
            ["brilliant-diamond", "shining-pearl"], GameFamily.Bdsp, true, true),
        new("pla", "Legends: Arceus", 8, Region.Hisui, "legends-arceus", ["legends-arceus"], GameFamily.Pla, true, false),
        new("sv", "Scarlet / Violet", 9, Region.Paldea, "scarlet-violet", ["scarlet", "violet"], GameFamily.Sv, true, true),
        new("za", "Legends: Z-A", 9, Region.Lumiose, "legends-za", ["legends-za"], GameFamily.Za, true, false),
    ];

    public static GameVersionGroup ByKey(string key) =>
        All.FirstOrDefault(g => g.Key == key) ?? throw new KeyNotFoundException($"Jogo desconhecido: {key}");

    public static GameVersionGroup? ByFamily(GameFamily family) => All.FirstOrDefault(g => g.Family == family);
}

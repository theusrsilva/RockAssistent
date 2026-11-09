using PKHeX.Core;

namespace TradeRock.Core.Games;

/// <summary>
/// Formato de arquivo de um Pokémon no cofre (D34): um por tipo do PKHeX, de todas as gerações. É separado do
/// catálogo de troca (<see cref="GameFamily"/>): só os formatos com <see cref="VaultFormat.Family"/> trocam com o Switch.
/// Gravado como texto no banco: só acrescentar no fim.
/// </summary>
public enum StorageFormat
{
    Pk1,
    Pk2,
    Pk3,
    Pk4,
    Pk5,
    Pk6,
    Pk7,
    Pb7,
    Pk8,
    Pb8,
    Pa8,
    Pk9,
    Pa9,
}

/// <summary>Um formato do cofre.</summary>
/// <param name="Key">Chave na API e no filtro do cofre. Os formatos do catálogo de troca usam a chave do jogo ("frlg", "sv"...).</param>
/// <param name="Rank">Ordem dos caminhos de transferência (1 a 8): só se adapta para um formato de ordem maior.</param>
/// <param name="Family">Jogo do catálogo de troca com este formato, ou null (não troca com o Switch).</param>
public sealed record VaultFormat(
    StorageFormat Format,
    string Key,
    string Name,
    byte Generation,
    int Rank,
    EntityContext Context,
    Type EntityType,
    string Extension,
    GameFamily? Family)
{
    public SwitchGame? Game => Family is { } family ? GameCatalog.Get(family) : null;

    /// <summary>Tabela de espécies do último jogo do formato (o que tem mais espécies e formas).</summary>
    public IPersonalTable PersonalTable => Format switch
    {
        StorageFormat.Pk1 => PKHeX.Core.PersonalTable.Y,
        StorageFormat.Pk2 => PKHeX.Core.PersonalTable.C,
        StorageFormat.Pk3 => PKHeX.Core.PersonalTable.E,
        StorageFormat.Pk4 => PKHeX.Core.PersonalTable.HGSS,
        StorageFormat.Pk5 => PKHeX.Core.PersonalTable.B2W2,
        StorageFormat.Pk6 => PKHeX.Core.PersonalTable.AO,
        StorageFormat.Pk7 => PKHeX.Core.PersonalTable.USUM,
        _ => Game!.PersonalTable,
    };

    /// <summary>A espécie (e a forma) existe neste formato.</summary>
    public bool HasSpecies(ushort species, byte form = 0) =>
        Game is { } game ? game.HasSpecies(species, form)
            : species is > 0 && species <= PersonalTable.MaxSpeciesID && PersonalTable.IsPresentInGame(species, form);

    /// <summary>Pokémon vazio do formato (um novo a cada chamada).</summary>
    public PKM Blank() => (PKM)Activator.CreateInstance(EntityType)!;
}

public static class VaultFormats
{
    public static IReadOnlyList<VaultFormat> All { get; } =
    [
        new(StorageFormat.Pk1, "gen1", "Red/Blue/Yellow", 1, 1, EntityContext.Gen1, typeof(PK1), "pk1", null),
        new(StorageFormat.Pk2, "gen2", "Gold/Silver/Crystal", 2, 2, EntityContext.Gen2, typeof(PK2), "pk2", null),
        new(StorageFormat.Pk3, "frlg", "Gen 3 (GBA)", 3, 3, EntityContext.Gen3, typeof(PK3), "pk3", GameFamily.Frlg),
        new(StorageFormat.Pk4, "gen4", "Diamond/Pearl/Platinum/HeartGold/SoulSilver", 4, 4, EntityContext.Gen4, typeof(PK4), "pk4", null),
        new(StorageFormat.Pk5, "gen5", "Black/White/Black 2/White 2", 5, 5, EntityContext.Gen5, typeof(PK5), "pk5", null),
        new(StorageFormat.Pk6, "gen6", "X/Y/Omega Ruby/Alpha Sapphire", 6, 6, EntityContext.Gen6, typeof(PK6), "pk6", null),
        new(StorageFormat.Pk7, "gen7", "Sun/Moon/Ultra Sun/Ultra Moon", 7, 7, EntityContext.Gen7, typeof(PK7), "pk7", null),
        new(StorageFormat.Pb7, "lgpe", "Let's Go, Pikachu!/Eevee!", 7, 7, EntityContext.Gen7b, typeof(PB7), "pb7", GameFamily.Lgpe),
        new(StorageFormat.Pk8, "swsh", "Sword/Shield", 8, 8, EntityContext.Gen8, typeof(PK8), "pk8", GameFamily.Swsh),
        new(StorageFormat.Pb8, "bdsp", "Brilliant Diamond/Shining Pearl", 8, 8, EntityContext.Gen8b, typeof(PB8), "pb8", GameFamily.Bdsp),
        new(StorageFormat.Pa8, "pla", "Legends: Arceus", 8, 8, EntityContext.Gen8a, typeof(PA8), "pa8", GameFamily.Pla),
        new(StorageFormat.Pk9, "sv", "Scarlet/Violet", 9, 8, EntityContext.Gen9, typeof(PK9), "pk9", GameFamily.Sv),
        new(StorageFormat.Pa9, "za", "Legends: Z-A", 9, 8, EntityContext.Gen9a, typeof(PA9), "pa9", GameFamily.Za),
    ];

    public static VaultFormat Get(StorageFormat format) => All[(int)format];

    public static VaultFormat Of(GameFamily family) => All.First(f => f.Family == family);

    public static StorageFormat FormatOf(GameFamily family) => Of(family).Format;

    public static VaultFormat? ByKey(string key) => All.FirstOrDefault(f => f.Key == key);

    public static VaultFormat? ByExtension(string extension) =>
        All.FirstOrDefault(f => string.Equals(f.Extension, extension.TrimStart('.'), StringComparison.OrdinalIgnoreCase));

    public static VaultFormat? ByContext(EntityContext context) => All.FirstOrDefault(f => f.Context == context);
}

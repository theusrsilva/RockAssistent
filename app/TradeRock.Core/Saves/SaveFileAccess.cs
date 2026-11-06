using PKHeX.Core;
using TradeRock.Core.Games;

namespace TradeRock.Core.Saves;

/// <summary>Resumo de um save de qualquer geração, para listas.</summary>
/// <param name="GameKey">Chave do grupo em <see cref="GameVersions"/> ("frlg", "swsh", "c"...).</param>
/// <param name="TrainerId">O ID como o jogo mostra (5 dígitos até a Gen 6, 6 dígitos da Gen 7 em diante).</param>
/// <param name="Writable">O PKHeX consegue gravar o save de volta (<c>SaveFile.State.Exportable</c>).</param>
public sealed record SaveInfo(
    string Path,
    string GameKey,
    GameVersion Version,
    string TrainerName,
    uint TrainerId,
    int Generation,
    int BoxCount,
    int SlotsPerBox,
    int PartyCount,
    DateTimeOffset ModifiedAt,
    string Sha256,
    bool Writable);

/// <summary>Resumo de um Pokémon de save de qualquer geração (sem a família de troca: ver <see cref="SaveSlotEntry.Family"/>).</summary>
/// <param name="TrainerId">O ID do treinador original como o jogo mostra.</param>
public sealed record SlotSummary(
    ushort Species,
    byte Form,
    string SpeciesName,
    string Nickname,
    byte Level,
    bool IsShiny,
    bool IsEgg,
    byte Gender,
    string OtName,
    uint TrainerId,
    ushort HeldItem,
    string OriginGame,
    int Language,
    bool Fateful,
    uint Pid);

/// <summary>Um Pokémon de um save.</summary>
/// <param name="Box">Caixa (a partir de 0) ou -1 para o time.</param>
/// <param name="Data">Bytes decriptados no formato de caixa (SIZE_STORED) do tipo do save (ver <see cref="SaveFileAccess.ToEntity"/>).</param>
/// <param name="Context">Formato do Pokémon no PKHeX.</param>
/// <param name="Family">Jogo do catálogo de troca com esse formato, ou null (Gen 1, 2, 4 a 7 fora do Let's Go).</param>
public sealed record SaveSlotEntry(int Box, int Slot, byte[] Data, SlotSummary Summary, EntityContext Context, GameFamily? Family);

/// <summary>
/// Leitura e escrita de saves de todas as gerações pelo PKHeX.Core (que refaz os checksums e preserva
/// cabeçalho/rodapé de emulador). Os erros saem como <see cref="SaveInjectionException"/> com um código.
/// </summary>
public static class SaveFileAccess
{
    /// <summary>Limite de tamanho de um save (o maior, o do Scarlet/Violet, tem uns 4,4 MB).</summary>
    public const int MaxSaveBytes = 16 * 1024 * 1024;

    /// <summary>Posição do time em <see cref="SaveSlotEntry.Box"/>, <see cref="WriteSlot"/> e <see cref="ClearSlot"/>.</summary>
    public const int Party = -1;

    private const int PartySize = 6;

    // Não mexe no Pokémon; atualiza a Pokédex do save; não mexe nos registros (igual à injeção da Gen 3).
    private static readonly EntityImportSettings WriteSettings =
        new(EntityImportOption.Disable, EntityImportOption.Enable, EntityImportOption.Disable);

    private static readonly EntityImportSettings ClearSettings =
        new(EntityImportOption.Disable, EntityImportOption.Disable, EntityImportOption.Disable);

    public static SaveFile Open(byte[] bytes)
    {
        if (bytes.Length > MaxSaveBytes)
            throw new SaveInjectionException("sav_size", "Arquivo grande demais para um save.");
        return SaveUtil.GetSaveFile(bytes.ToArray(), null)
            ?? throw new SaveInjectionException("sav_unknown", "Não é um save reconhecido.");
    }

    public static SaveInfo Describe(SaveFile sav, string path, DateTimeOffset modifiedAt, string sha) =>
        new(path, GameKeyOf(sav), sav.Version, sav.OT, sav.DisplayTID, sav.Generation, sav.BoxCount, sav.BoxSlotCount,
            sav.PartyCount, modifiedAt, sha, sav.State.Exportable);

    /// <summary>Time (Box = -1) e caixas, só as posições ocupadas. Posições com checksum errado ficam de fora.</summary>
    public static IReadOnlyList<SaveSlotEntry> ReadAll(SaveFile sav)
    {
        var family = GameCatalog.ByContext(sav.Context)?.Family;
        var entries = new List<SaveSlotEntry>();
        for (var i = 0; i < sav.PartyCount; i++)
            Add(sav.GetPartySlotAtIndex(i), Party, i);
        for (var box = 0; box < sav.BoxCount; box++)
            for (var slot = 0; slot < sav.BoxSlotCount; slot++)
                Add(sav.GetBoxSlotAtIndex(box, slot), box, slot);
        return entries;

        void Add(PKM pk, int box, int slot)
        {
            if (pk.Species == 0 || pk.Species > pk.MaxSpeciesID || !pk.ChecksumValid)
                return;
            var data = new byte[pk.SIZE_STORED];
            pk.WriteDecryptedDataStored(data);
            entries.Add(new SaveSlotEntry(box, slot, data, Summarize(pk), pk.Context, family));
        }
    }

    /// <summary>Remonta o Pokémon a partir dos bytes de <see cref="SaveSlotEntry.Data"/> no formato do save.</summary>
    public static PKM ToEntity(SaveFile sav, byte[] data)
    {
        var pk = EntityFormat.GetFromBytes(data.ToArray(), sav.Context);
        if (pk is null || pk.GetType() != sav.BlankPKM.GetType())
            throw new SaveInjectionException("sav_wrong_game", "Os dados não são de um Pokémon no formato deste save.");
        return pk;
    }

    private static SlotSummary Summarize(PKM pk)
    {
        // O PKHeX não tem nomes em português; os nomes em inglês são os mesmos usados no Brasil.
        var species = GameInfo.GetStrings("en").Species;
        return new SlotSummary(
            Species: pk.Species,
            Form: pk.Form,
            SpeciesName: pk.Species < species.Count ? species[pk.Species] : $"#{pk.Species}",
            Nickname: pk.Nickname,
            Level: pk.CurrentLevel,
            IsShiny: pk.IsShiny,
            IsEgg: pk.IsEgg,
            Gender: pk.Gender,
            OtName: pk.OriginalTrainerName,
            TrainerId: pk.DisplayTID,
            HeldItem: (ushort)pk.HeldItem,
            OriginGame: pk.Version.ToString(),
            Language: pk.Language,
            Fateful: pk.FatefulEncounter,
            Pid: pk.PID);
    }

    /// <summary>
    /// Grava (sobrescrevendo) um Pokémon do mesmo formato do save. No time, e nas caixas da Gen 1 e 2 (que o jogo
    /// guarda como lista), só até a primeira posição depois das ocupadas: sem buracos.
    /// </summary>
    public static void WriteSlot(SaveFile sav, int box, int slot, PKM pk)
    {
        if (pk.Context != sav.Context || pk.GetType() != sav.BlankPKM.GetType())
            throw new SaveInjectionException("sav_wrong_game", "O Pokémon não é do formato deste save.");
        if (box == Party)
        {
            if (slot < 0 || slot >= PartySize || slot > sav.PartyCount)
                throw SlotInvalid(box, slot);
            sav.SetPartySlotAtIndex(pk, slot, WriteSettings);
            return;
        }
        CheckBoxSlot(sav, box, slot);
        if (sav is SAV1 or SAV2 && slot > OccupiedInBox(sav, box))
            throw SlotInvalid(box, slot);
        sav.SetBoxSlotAtIndex(pk, box, slot, WriteSettings);
    }

    /// <summary>
    /// Esvazia uma posição. No time, os seguintes sobem uma posição na hora (como no jogo). Nas caixas da Gen 1 e 2
    /// (SAV1/SAV2) a posição fica vazia na memória, mas os seguintes sobem quando o save é gravado (<see cref="Export"/>).
    /// </summary>
    public static void ClearSlot(SaveFile sav, int box, int slot)
    {
        if (box == Party)
        {
            if (slot < 0 || slot >= sav.PartyCount)
                throw SlotInvalid(box, slot);
            sav.DeletePartySlot(slot);
            return;
        }
        CheckBoxSlot(sav, box, slot);
        sav.SetBoxSlotAtIndex(sav.BlankPKM, box, slot, ClearSettings);
    }

    /// <summary>O save gravado no formato original (com o cabeçalho/rodapé que o arquivo tinha).</summary>
    public static byte[] Export(SaveFile sav) => sav.Write().ToArray();

    /// <summary>
    /// Chave do grupo em <see cref="GameVersions"/>. Saves que o PKHeX abre mas que não são da série principal
    /// (Colosseum/XD, Box RS, Stadium, Ranch, Bank) ou com versão indefinida lançam "sav_unsupported".
    /// </summary>
    public static string GameKeyOf(SaveFile sav) => sav.Version switch
    {
        GameVersion.RD or GameVersion.GN or GameVersion.BU or GameVersion.RB => "rb",
        GameVersion.YW => "y",
        GameVersion.GD or GameVersion.SI or GameVersion.GS => "gs",
        GameVersion.C => "c",
        GameVersion.R or GameVersion.S or GameVersion.RS => "rs",
        GameVersion.E => "e",
        GameVersion.FR or GameVersion.LG or GameVersion.FRLG => "frlg",
        GameVersion.D or GameVersion.P or GameVersion.DP => "dp",
        GameVersion.Pt => "pt",
        GameVersion.HG or GameVersion.SS or GameVersion.HGSS => "hgss",
        GameVersion.B or GameVersion.W or GameVersion.BW => "bw",
        GameVersion.B2 or GameVersion.W2 or GameVersion.B2W2 => "b2w2",
        GameVersion.X or GameVersion.Y or GameVersion.XY => "xy",
        GameVersion.OR or GameVersion.AS or GameVersion.ORAS => "oras",
        GameVersion.SN or GameVersion.MN or GameVersion.SM => "sm",
        GameVersion.US or GameVersion.UM or GameVersion.USUM => "usum",
        GameVersion.GP or GameVersion.GE or GameVersion.GG => "lgpe",
        GameVersion.SW or GameVersion.SH or GameVersion.SWSH => "swsh",
        GameVersion.BD or GameVersion.SP or GameVersion.BDSP => "bdsp",
        GameVersion.PLA => "pla",
        GameVersion.SL or GameVersion.VL or GameVersion.SV => "sv",
        GameVersion.ZA => "za",
        var other => throw new SaveInjectionException("sav_unsupported", $"Save de um jogo fora da lista ({other})."),
    };

    private static void CheckBoxSlot(SaveFile sav, int box, int slot)
    {
        if (box < 0 || box >= sav.BoxCount || slot < 0 || slot >= sav.BoxSlotCount)
            throw SlotInvalid(box, slot);
        if (sav.IsBoxSlotLocked(box, slot))
            throw new SaveInjectionException("slot_locked", $"A posição {slot + 1} da caixa {box + 1} está travada pelo jogo.");
    }

    private static int OccupiedInBox(SaveFile sav, int box) =>
        Enumerable.Range(0, sav.BoxSlotCount).Count(slot => sav.GetBoxSlotAtIndex(box, slot).Species != 0);

    private static SaveInjectionException SlotInvalid(int box, int slot) =>
        new("slot_invalid", box == Party
            ? $"Posição inválida no time: {slot + 1}."
            : $"Posição inválida: caixa {box + 1}, posição {slot + 1}.");
}

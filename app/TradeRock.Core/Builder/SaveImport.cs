using PKHeX.Core;
using TradeRock.Core.Pokemon;
using TradeRock.Core.Saves;

namespace TradeRock.Core.Builder;

public sealed record SaveEntry(int Index, string Place, int Box, int Slot, byte[] Data, Pk3Summary Summary, LegalityStatus Legality);

public sealed record SaveContents(string Game, string TrainerName, ushort Tid, ushort Sid, IReadOnlyList<SaveEntry> Entries);

public sealed class InvalidSaveException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}

/// <summary>
/// Lê um save para importar no cofre. Abre qualquer geração que o PKHeX reconheça (<see cref="SaveFileAccess"/>),
/// mas a lista para o cofre (formato .pk3) continua só da Gen 3: os outros saves dão "not_gen3".
/// </summary>
public static class SaveImport
{
    /// <summary>Limite do envio pela API de importação (saves de GBA).</summary>
    public const int MaxSaveBytes = 256 * 1024;

    public static SaveContents Read(byte[] file)
    {
        SaveFile sav;
        try
        {
            sav = SaveFileAccess.Open(file);
        }
        catch (SaveInjectionException ex)
        {
            throw new InvalidSaveException(ex.Code == "sav_size" ? "size" : "unknown", ex.Message);
        }
        return Read(sav);
    }

    /// <summary>Extrai time e caixas de um save já lido pelo PKHeX.</summary>
    public static SaveContents Read(SaveFile sav)
    {
        if (sav.Generation != 3 || sav is not SAV3)
            throw new InvalidSaveException("not_gen3", "O save não é da terceira geração (GBA).");

        var entries = new List<SaveEntry>();
        var index = 0;

        var party = sav.PartyData;
        for (var i = 0; i < party.Count; i++)
            Add(party[i], "party", 0, i);

        for (var box = 0; box < sav.BoxCount; box++)
        {
            var data = sav.GetBoxData(box);
            for (var slot = 0; slot < data.Length; slot++)
                Add(data[slot], "box", box, slot);
        }

        return new SaveContents(sav.Version.ToString(), sav.OT, sav.TID16, sav.SID16, entries);

        void Add(PKM pk, string place, int box, int slot)
        {
            if (pk.Species == 0 || pk is not PK3 pk3)
                return;
            var bytes = new byte[Pk3Reader.PartySize];
            if (!pk3.PartyStatsPresent)
                pk3.ResetPartyStats();
            pk3.WriteDecryptedDataParty(bytes);
            Pk3File parsed;
            try
            {
                parsed = Pk3Reader.Read(bytes);
            }
            catch (InvalidPokemonFileException)
            {
                return; // slot corrompido
            }
            var legality = LegalityReporter.Analyze(pk3).Status;
            entries.Add(new SaveEntry(index++, place, box, slot, parsed.Data, parsed.Summary, legality));
        }
    }
}

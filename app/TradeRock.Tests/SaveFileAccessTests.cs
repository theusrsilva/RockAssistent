using PKHeX.Core;
using TradeRock.Core.Saves;

namespace TradeRock.Tests;

public sealed class SaveFileAccessTests
{
    private const ushort Gengar = 94;

    private static SaveFile Blank(GameVersion version) => BlankSaveFile.Get(version, "ROCHA", LanguageID.English);

    private static PKM GengarFor(SaveFile sav)
    {
        var pk = sav.BlankPKM;
        pk.Species = Gengar;
        pk.CurrentLevel = 40;
        pk.RefreshChecksum();
        return pk;
    }

    [Theory]
    [InlineData(GameVersion.FR, "frlg", 3)]
    [InlineData(GameVersion.SW, "swsh", 8)]
    [InlineData(GameVersion.C, "c", 2)]
    [InlineData(GameVersion.RD, "rb", 1)]
    [InlineData(GameVersion.YW, "y", 1)]
    [InlineData(GameVersion.GD, "gs", 2)]
    [InlineData(GameVersion.B2, "b2w2", 5)]
    [InlineData(GameVersion.OR, "oras", 6)]
    [InlineData(GameVersion.GP, "lgpe", 7)]
    [InlineData(GameVersion.BD, "bdsp", 8)]
    [InlineData(GameVersion.PLA, "pla", 8)]
    [InlineData(GameVersion.SL, "sv", 9)]
    [InlineData(GameVersion.ZA, "za", 9)]
    public void Describe_da_o_jogo_certo(GameVersion version, string key, int generation)
    {
        var sav = Blank(version);
        var modified = new DateTimeOffset(2026, 11, 6, 12, 0, 0, TimeSpan.Zero);

        var info = SaveFileAccess.Describe(sav, "/saves/x.sav", modified, "abc");

        Assert.Equal(key, info.GameKey);
        Assert.Equal(key, SaveFileAccess.GameKeyOf(sav));
        Assert.Equal(version, info.Version);
        Assert.Equal(generation, info.Generation);
        Assert.Equal("/saves/x.sav", info.Path);
        Assert.Equal(modified, info.ModifiedAt);
        Assert.Equal("abc", info.Sha256);
        Assert.Equal(sav.BoxCount, info.BoxCount);
        Assert.Equal(sav.BoxSlotCount, info.SlotsPerBox);
    }

    [Fact]
    public void Describe_de_um_save_de_firered_lido_do_arquivo()
    {
        var sav = SaveFileAccess.Open(SaveFixture.BlankFireRed());

        var info = SaveFileAccess.Describe(sav, "fr.sav", DateTimeOffset.UnixEpoch, "x");

        Assert.Equal("frlg", info.GameKey);
        Assert.Equal("RED", info.TrainerName);
        Assert.Equal(24680u, info.TrainerId);
        Assert.Equal(14, info.BoxCount);
        Assert.Equal(30, info.SlotsPerBox);
        Assert.Equal(0, info.PartyCount);
        Assert.True(info.Writable);
    }

    [Theory]
    [InlineData(GameVersion.FR)]
    [InlineData(GameVersion.SW)]
    [InlineData(GameVersion.C)]
    public void Gengar_gravado_volta_na_posicao(GameVersion version)
    {
        var sav = Blank(version);

        SaveFileAccess.WriteSlot(sav, 2, 5, GengarFor(sav));
        SaveFileAccess.WriteSlot(sav, -1, 0, GengarFor(sav));

        var all = SaveFileAccess.ReadAll(sav);
        Assert.Equal(2, all.Count);
        var party = Assert.Single(all, e => e.Box == -1);
        Assert.Equal((0, Gengar), (party.Slot, party.Summary.Species));
        var boxed = Assert.Single(all, e => e.Box == 2);
        Assert.Equal((5, Gengar), (boxed.Slot, boxed.Summary.Species));
        Assert.Equal(sav.BlankPKM.SIZE_STORED, boxed.Data.Length);
        Assert.Equal(sav.Context, boxed.Context);
    }

    [Fact]
    public void Limpar_esvazia_caixa_e_time()
    {
        var sav = Blank(GameVersion.C);
        SaveFileAccess.WriteSlot(sav, 0, 0, GengarFor(sav));
        SaveFileAccess.WriteSlot(sav, -1, 0, GengarFor(sav));

        SaveFileAccess.ClearSlot(sav, 0, 0);
        SaveFileAccess.ClearSlot(sav, -1, 0);

        Assert.Empty(SaveFileAccess.ReadAll(sav));
        Assert.Equal(0, sav.PartyCount);
    }

    [Fact]
    public void Pokemon_de_outro_jogo_e_recusado()
    {
        var sav = SaveFileAccess.Open(SaveFixture.BlankFireRed());
        var pk8 = new PK8 { Species = Gengar, CurrentLevel = 40 };

        var ex = Assert.Throws<SaveInjectionException>(() => SaveFileAccess.WriteSlot(sav, 0, 0, pk8));
        Assert.Equal("sav_wrong_game", ex.Code);
    }

    [Fact]
    public void Posicao_fora_do_save_e_recusada()
    {
        var sav = Blank(GameVersion.C);
        Assert.Equal("slot_invalid", Assert.Throws<SaveInjectionException>(() =>
            SaveFileAccess.WriteSlot(sav, sav.BoxCount, 0, GengarFor(sav))).Code);
        Assert.Equal("slot_invalid", Assert.Throws<SaveInjectionException>(() =>
            SaveFileAccess.WriteSlot(sav, -1, 3, GengarFor(sav))).Code); // time vazio: só a posição 0
        Assert.Equal("slot_invalid", Assert.Throws<SaveInjectionException>(() => SaveFileAccess.ClearSlot(sav, -1, 0)).Code);
    }

    [Fact]
    public void Exportar_e_abrir_de_novo_da_o_mesmo_conteudo_gen3()
    {
        var sav = SaveFileAccess.Open(SaveFixture.BlankFireRed());
        SaveFileAccess.WriteSlot(sav, 1, 4, GengarFor(sav));
        SaveFileAccess.WriteSlot(sav, -1, 0, GengarFor(sav));
        var before = SaveFileAccess.ReadAll(sav);

        var reopened = SaveFileAccess.Open(SaveFileAccess.Export(sav));

        Assert.Equal("frlg", SaveFileAccess.GameKeyOf(reopened));
        AssertSame(before, SaveFileAccess.ReadAll(reopened));
    }

    [Theory]
    [InlineData(GameVersion.BD)]
    [InlineData(GameVersion.B2)]
    public void Exportar_e_abrir_de_novo_da_o_mesmo_conteudo(GameVersion version)
    {
        var sav = Blank(version);
        SaveFileAccess.WriteSlot(sav, 3, 7, GengarFor(sav));
        var before = SaveFileAccess.ReadAll(sav);

        var reopened = SaveFileAccess.Open(SaveFileAccess.Export(sav));

        Assert.Equal(sav.Version, reopened.Version);
        AssertSame(before, SaveFileAccess.ReadAll(reopened));
    }

    [Fact]
    public void Arquivo_pequeno_demais_nao_e_save()
    {
        var ex = Assert.Throws<SaveInjectionException>(() => SaveFileAccess.Open(new byte[10]));
        Assert.Equal("sav_unknown", ex.Code);
    }

    [Fact]
    public void Save_sem_jogo_na_lista_nao_tem_chave()
    {
        var ex = Assert.Throws<SaveInjectionException>(() => SaveFileAccess.GameKeyOf(new SAV3XD()));
        Assert.Equal("sav_unsupported", ex.Code);
    }

    private static void AssertSame(IReadOnlyList<SaveSlotEntry> expected, IReadOnlyList<SaveSlotEntry> actual)
    {
        Assert.Equal(expected.Count, actual.Count);
        foreach (var (a, b) in expected.Zip(actual))
        {
            Assert.Equal((a.Box, a.Slot), (b.Box, b.Slot));
            Assert.Equal(a.Data, b.Data);
        }
    }
}

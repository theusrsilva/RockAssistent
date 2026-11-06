using System.Text;
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
        var slot = sav is SAV2 ? 0 : 5; // a Gen 2 guarda a caixa como lista, sem buracos

        SaveFileAccess.WriteSlot(sav, 2, slot, GengarFor(sav));
        SaveFileAccess.WriteSlot(sav, -1, 0, GengarFor(sav));

        var all = SaveFileAccess.ReadAll(sav);
        Assert.Equal(2, all.Count);
        var party = Assert.Single(all, e => e.Box == -1);
        Assert.Equal((0, Gengar), (party.Slot, party.Summary.Species));
        var boxed = Assert.Single(all, e => e.Box == 2);
        Assert.Equal((slot, Gengar), (boxed.Slot, boxed.Summary.Species));
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
    [InlineData(GameVersion.BD, "bdsp")]
    [InlineData(GameVersion.B2, "b2w2")]
    public void Exportar_e_abrir_de_novo_da_o_mesmo_conteudo(GameVersion version, string key)
    {
        var sav = Blank(version);
        SaveFileAccess.WriteSlot(sav, 3, 7, GengarFor(sav));
        var before = SaveFileAccess.ReadAll(sav);

        var reopened = SaveFileAccess.Open(SaveFileAccess.Export(sav));

        Assert.Equal(sav.Version, reopened.Version);
        Assert.Equal(key, SaveFileAccess.GameKeyOf(reopened));
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

    [Fact]
    public void Rodape_do_desmume_e_preservado()
    {
        // O DeSmuME grava 0x7A bytes depois do save de 512 KB, terminando com "|-DESMUME SAVE-|".
        var raw = SaveFileAccess.Export(Blank(GameVersion.B2));
        var footer = new byte[0x7A];
        Encoding.ASCII.GetBytes("|-DESMUME SAVE-|").CopyTo(footer, footer.Length - 16);
        var original = raw.Concat(footer).ToArray();

        var sav = SaveFileAccess.Open(original);
        SaveFileAccess.WriteSlot(sav, 0, 0, GengarFor(sav));
        var exported = SaveFileAccess.Export(sav);

        Assert.Equal("b2w2", SaveFileAccess.GameKeyOf(sav));
        Assert.Equal(original.Length, exported.Length);
        Assert.Equal(footer, exported[raw.Length..]);
        Assert.Equal(Gengar, SaveFileAccess.Open(exported).GetBoxSlotAtIndex(0, 0).Species);
    }

    [Fact]
    public void Rodape_de_relogio_do_emulador_de_gba_e_preservado()
    {
        var raw = SaveFixture.BlankFireRed();
        var footer = Enumerable.Range(1, 16).Select(i => (byte)i).ToArray();
        var original = raw.Concat(footer).ToArray();

        var sav = SaveFileAccess.Open(original);
        var exported = SaveFileAccess.Export(sav);

        Assert.Equal("frlg", SaveFileAccess.GameKeyOf(sav));
        Assert.Equal(original.Length, exported.Length);
        Assert.Equal(footer, exported[raw.Length..]);
    }

    [Fact]
    public void Gen2_compacta_a_caixa_ao_gravar()
    {
        var sav = Blank(GameVersion.C);
        var second = GengarFor(sav);
        second.Species = 25;
        second.RefreshChecksum();
        SaveFileAccess.WriteSlot(sav, 0, 0, GengarFor(sav));
        SaveFileAccess.WriteSlot(sav, 0, 1, second);
        SaveFileAccess.ClearSlot(sav, 0, 0);

        var reopened = new SAV2(SaveFileAccess.Export(sav), LanguageID.English, GameVersion.C);

        var entry = Assert.Single(SaveFileAccess.ReadAll(reopened));
        Assert.Equal((0, 0, (ushort)25), (entry.Box, entry.Slot, entry.Summary.Species));
    }

    [Fact]
    public void Gen2_nao_deixa_buraco_na_caixa()
    {
        var sav = Blank(GameVersion.C);
        Assert.Equal("slot_invalid", Assert.Throws<SaveInjectionException>(() =>
            SaveFileAccess.WriteSlot(sav, 0, 2, GengarFor(sav))).Code);
        SaveFileAccess.WriteSlot(sav, 0, 0, GengarFor(sav));
        SaveFileAccess.WriteSlot(sav, 0, 1, GengarFor(sav));
        SaveFileAccess.WriteSlot(sav, 0, 0, GengarFor(sav)); // sobrescrever uma ocupada vale
    }

    [Theory]
    [InlineData(GameVersion.C)]
    [InlineData(GameVersion.FR)]
    public void Dados_lidos_voltam_a_ser_pokemon(GameVersion version)
    {
        var sav = Blank(version);
        SaveFileAccess.WriteSlot(sav, 0, 0, GengarFor(sav));
        var read = Assert.Single(SaveFileAccess.ReadAll(sav));

        var pk = SaveFileAccess.ToEntity(sav, read.Data);
        SaveFileAccess.WriteSlot(sav, 0, 1, pk);

        var all = SaveFileAccess.ReadAll(sav);
        Assert.Equal(2, all.Count);
        Assert.Equal(read.Data, all.Single(e => e.Slot == 1).Data);
        Assert.Equal(Gengar, all.Single(e => e.Slot == 1).Summary.Species);
    }

    [Fact]
    public void Dados_de_outro_formato_nao_viram_pokemon()
    {
        var sav = Blank(GameVersion.C);
        Assert.Equal("sav_wrong_game", Assert.Throws<SaveInjectionException>(() =>
            SaveFileAccess.ToEntity(sav, new byte[10])).Code);
    }

    [Fact]
    public void Fora_do_catalogo_a_familia_e_nula()
    {
        var sav = Blank(GameVersion.C);
        SaveFileAccess.WriteSlot(sav, 0, 0, GengarFor(sav));
        var entry = Assert.Single(SaveFileAccess.ReadAll(sav));
        Assert.Null(entry.Family);
        Assert.Equal("Gengar", entry.Summary.SpeciesName);
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

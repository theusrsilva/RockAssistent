using TradeRock.Core.GameData;
using TradeRock.Core.Hunts;

namespace TradeRock.Tests;

public sealed class ShinyOddsTests
{
    private static readonly GameDataStore Store = new(GameDataStore.EmbeddedFiles(), null,
        Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);

    private static ShinyMethod M(string id) => Store.ShinyMethods.Single(m => m.Id == id);

    private static Dictionary<string, string> Mods(params (string Id, string Value)[] values) =>
        values.ToDictionary(v => v.Id, v => v.Value);

    [Fact]
    public void Masuda_com_amuleto_no_SV_da_1_em_512()
    {
        Assert.Equal(512, ShinyOdds.Current(M("masuda"), Mods(("shiny_charm", "true")), 0, "sv"), 0.5);
        Assert.Equal(683.08, ShinyOdds.Current(M("masuda"), Mods(("shiny_charm", "false")), 0, "sv"), 0.01);
        Assert.Equal(683.08, ShinyOdds.Current(M("masuda"), Mods(), 0, "sv"), 0.01);
    }

    [Fact]
    public void Chance_acumulada()
    {
        Assert.Equal(0.50, ShinyOdds.Cumulative(512, 355), 0.01);
        Assert.Equal(0, ShinyOdds.Cumulative(512, 0));
        Assert.Equal(1, ShinyOdds.Cumulative(1, 3));
    }

    [Fact]
    public void Modificador_de_outro_jogo_nao_conta()
    {
        // O amuleto do masuda_gen5 só existe no Black 2/White 2.
        Assert.Equal(1365.75, ShinyOdds.Current(M("masuda_gen5"), Mods(("shiny_charm", "true")), 0, "bw"), 0.01);
        Assert.Equal(1024.44, ShinyOdds.Current(M("masuda_gen5"), Mods(("shiny_charm", "true")), 0, "b2w2"), 0.01);
    }

    [Fact]
    public void Degrau_do_contador_segue_o_count()
    {
        var sos = M("sos_chain");
        Assert.Equal(4096, ShinyOdds.Current(sos, Mods(), 5, "sm"), 0.01);
        Assert.Equal(819.6, ShinyOdds.Current(sos, Mods(), 15, "sm"), 0.01);
        Assert.Equal(273.53, ShinyOdds.Current(sos, Mods(("shiny_charm", "true")), 40, "sm"), 0.01);
    }

    [Fact]
    public void Degrau_informado_e_combinado_com_amuleto()
    {
        var sv = M("sv_wild");
        Assert.Equal(512.44, ShinyOdds.Current(sv,
            Mods(("sparkling_power", "3"), ("outbreak", "60"), ("shiny_charm", "true")), 0, "sv"), 0.01);
        // Valor que nenhum degrau cobre não muda nada.
        Assert.Equal(4096, ShinyOdds.Current(sv, Mods(("sparkling_power", "99")), 0, "sv"), 0.01);
    }

    [Fact]
    public void Metodo_sem_rolagens_substitui_as_odds()
    {
        Assert.Equal(300, ShinyOdds.Current(M("dynamax_adventure"), Mods(), 0, "swsh"));
        Assert.Equal(100, ShinyOdds.Current(M("dynamax_adventure"), Mods(("shiny_charm", "true")), 0, "swsh"));
        var radar = M("poke_radar_dppt");
        Assert.Equal(8192, ShinyOdds.Current(radar, Mods(), 39, "dp"));
        Assert.Equal(200, ShinyOdds.Current(radar, Mods(), 40, "dp"));
    }

    [Fact]
    public void Pesquisa_perfeita_e_nivel_10_sao_excludentes()
    {
        var wild = M("pla_wild");
        var perfect = ShinyOdds.Current(wild, Mods(("research_perfect", "true")), 0, "pla");
        Assert.Equal(1024.38, perfect, 0.01);
        Assert.Equal(perfect, ShinyOdds.Current(wild, Mods(("research_10", "true"), ("research_perfect", "true")), 0, "pla"), 0.001);
        Assert.Equal(2048.25, ShinyOdds.Current(wild, Mods(("research_10", "true")), 0, "pla"), 0.01);
    }
}

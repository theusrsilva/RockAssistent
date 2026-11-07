using TradeRock.Core.Games;

namespace TradeRock.Tests;

public sealed class GameVersionsTests
{
    [Fact]
    public void Catalogo_tem_os_22_grupos_com_chaves_unicas()
    {
        Assert.Equal(22, GameVersions.All.Count);
        Assert.Equal(22, GameVersions.All.Select(g => g.Key).Distinct().Count());
        Assert.Equal(22, GameVersions.All.Select(g => g.PokeApiVersionGroup).Distinct().Count());
    }

    [Fact]
    public void Dados_basicos_de_cada_grupo()
    {
        Assert.Equal(GameFamily.Frlg, GameVersions.ByKey("frlg").Family);
        Assert.Equal(1, GameVersions.ByKey("rb").Generation);
        Assert.True(GameVersions.ByKey("b2w2").HasShinyCharm);
        Assert.False(GameVersions.ByKey("bw").HasShinyCharm);
        Assert.False(GameVersions.ByKey("lgpe").HasMasuda);
    }

    [Fact]
    public void Cada_jogo_do_Switch_aparece_em_exatamente_um_grupo()
    {
        foreach (var game in GameCatalog.All)
        {
            var group = Assert.Single(GameVersions.All, g => g.Family == game.Family);
            Assert.Same(group, GameVersions.ByFamily(game.Family));
        }
    }

    [Fact]
    public void Chave_desconhecida_da_erro()
    {
        Assert.Throws<KeyNotFoundException>(() => GameVersions.ByKey("nada"));
    }
}

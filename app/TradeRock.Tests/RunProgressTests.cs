using TradeRock.Core.GameData;
using TradeRock.Core.Nuzlocke;

namespace TradeRock.Tests;

/// <summary>Regras puras da tela da run (sem banco).</summary>
public sealed class RunProgressTests
{
    private static BossBattle Boss(string id, int order, int cap, params string[] versions) =>
        new(id, "gym", id, null, null, order, null, [], cap, true, null, versions.Length == 0 ? null : versions);

    private static MonInfo Mon(int level) =>
        new(Guid.NewGuid(), "route-1", 25, 0, "Pika", level, NuzlockeStatus.Party, null, DateTimeOffset.UtcNow, null, []);

    [Fact]
    public void Proximo_chefe_e_o_de_menor_ordem_nao_vencido()
    {
        IReadOnlyList<BossBattle> bosses = [Boss("b", 2, 20), Boss("a", 1, 10), Boss("c", 3, 30)];
        Assert.Equal("a", RunProgress.NextBoss(bosses, [], null)!.Id);
        Assert.Equal("b", RunProgress.NextBoss(bosses, ["a"], null)!.Id);
        Assert.Null(RunProgress.NextBoss(bosses, ["a", "b", "c"], null));
    }

    [Fact]
    public void Chefe_de_outra_versao_fica_de_fora()
    {
        IReadOnlyList<BossBattle> bosses = [Boss("bea", 1, 30, "sword"), Boss("allister", 1, 30, "shield"), Boss("opal", 2, 40)];
        Assert.Equal("allister", RunProgress.NextBoss(bosses, [], "shield")!.Id);
        Assert.Equal("opal", RunProgress.NextBoss(bosses, ["allister"], "shield")!.Id);
    }

    [Fact]
    public void Acima_do_cap()
    {
        IReadOnlyList<MonInfo> party = [Mon(14), Mon(15), Mon(9)];
        Assert.Equal([15], RunProgress.OverCap(party, 14).Select(m => m.Level));
        Assert.Empty(RunProgress.OverCap(party, null));
    }

    [Fact]
    public void Area_atual_padrao_e_a_primeira()
    {
        IReadOnlyList<GameArea> areas = [new("b", "B", "route", 2, null, null, []), new("a", "A", "city", 1, null, null, [])];
        Assert.Equal("a", RunProgress.CurrentArea(areas, null)!.Id);
        Assert.Equal("b", RunProgress.CurrentArea(areas, "b")!.Id);
        Assert.Equal("a", RunProgress.CurrentArea(areas, "sumiu")!.Id);
        Assert.Null(RunProgress.CurrentArea([], null));
    }
}

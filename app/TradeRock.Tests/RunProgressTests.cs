using TradeRock.Core.GameData;
using TradeRock.Core.Nuzlocke;

namespace TradeRock.Tests;

/// <summary>Regras puras da tela da run (sem banco).</summary>
public sealed class RunProgressTests
{
    private static BossBattle Boss(string id, int order, int cap, params string[] versions) =>
        new(id, "gym", id, null, null, order, null, [], cap, true, null, versions.Length == 0 ? null : versions);

    private static MonInfo Mon(int level) =>
        new(Guid.NewGuid(), "route-1", 25, 0, "Pika", level, NuzlockeStatus.Party, null, DateTimeOffset.UtcNow, null, [], false);

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
    public void Proximo_chefe_usa_o_time_do_inicial_da_run()
    {
        BossPokemon P(int species, int level) => new(species, 0, level, [], null, null, null);
        var rival = new BossBattle("rival", "rival", "Silver", null, null, 1, null, [P(158, 5)], 5, true, null, null,
            [new BossVariant("chikorita", [P(155, 6)], 6, true), new BossVariant("cyndaquil", [P(158, 5)], 5, false)]);
        IReadOnlyList<BossBattle> bosses = [rival];

        var chikorita = RunProgress.NextBoss(bosses, [], null, "chikorita")!;
        Assert.Equal(155, Assert.Single(chikorita.Party).Species);
        Assert.Equal(6, chikorita.LevelCap);
        Assert.Null(chikorita.Variants);
        Assert.False(RunProgress.NextBoss(bosses, [], null, "cyndaquil")!.Verified);
        // Sem inicial, ou com um sem variante: o time padrão.
        Assert.Same(rival, RunProgress.NextBoss(bosses, [], null));
        Assert.Same(rival, RunProgress.NextBoss(bosses, [], null, "totodile"));
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

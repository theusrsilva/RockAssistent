using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using TradeRock.Core.Dex;
using TradeRock.Core.GameData;
using TradeRock.Core.Games;

namespace TradeRock.Tests;

/// <summary>
/// Auditoria da disponibilidade da Pokédex: compara, por jogo, o PokeAPI, os nossos encontros e o PKHeX. Grava
/// <c>dex-availability-audit.csv</c> (todas as divergências) e <c>dex-availability-summary.txt</c> (o que passou a ser
/// obtível e as travas de shiny) na pasta do teste.
/// </summary>
public sealed class DexAvailabilityAuditTests(DexFixture fx) : IClassFixture<DexFixture>
{
    [Fact]
    public void Auditoria_das_tres_fontes()
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido (rode scripts/build-dex.py).");
        var ct = TestContext.Current.CancellationToken;
        var dex = fx.Service;
        var store = new GameDataStore(dex, NullLogger<GameDataStore>.Instance);
        var csv = new StringBuilder("game,species,name,pokeapi_only,own,pkhex,final\n");
        var summary = new StringBuilder();
        var changed = GameVersions.All.ToDictionary(g => g.Key, _ => new List<string>());
        var lost = GameVersions.All.ToDictionary(g => g.Key, _ => new List<string>());
        var locked = GameVersions.All.ToDictionary(g => g.Key, _ => new List<string>());
        for (var species = 1; species <= 1025; species++)
        {
            var before = dex.Availability(species, null, pkhex: false, ct);
            var after = dex.Availability(species, store.Encounters, pkhex: true, ct);
            var name = dex.SpeciesName(species) ?? $"#{species}";
            foreach (var g in GameVersions.All)
            {
                var old = before.Single(a => a.GameKey == g.Key).Kind;
                var now = after.Single(a => a.GameKey == g.Key).Kind;
                var pk = PkhexAvailability.Kind(g.Key, species);
                var own = store.Encounters(g.Key).Where(e => e.Species == species).Select(e => e.Method).Distinct().ToList();
                var ownText = own.Count == 0 ? "" : string.Join("|", own);
                var pokeapiHas = old != AvailabilityKind.None;
                var pkhexHas = pk != AvailabilityKind.None;
                if (old != now || pokeapiHas != pkhexHas)
                    csv.Append($"{g.Key},{species},{name},{old},{ownText},{pk},{now}\n");
                if (old == AvailabilityKind.None && now != AvailabilityKind.None)
                    changed[g.Key].Add($"{name} ({now})");
                if (old != AvailabilityKind.None && now == AvailabilityKind.None)
                    lost[g.Key].Add(name);
                if (after.Single(a => a.GameKey == g.Key).ShinyLocked)
                    locked[g.Key].Add(name);
            }
        }
        foreach (var g in GameVersions.All)
            summary.Append($"{g.Key}: +{changed[g.Key].Count} ({string.Join(", ", changed[g.Key].Take(10))})\n");
        summary.Append("\nShiny travado (todos os encontros travados e sem ovo):\n");
        foreach (var g in GameVersions.All)
            summary.Append($"{g.Key}: {locked[g.Key].Count} ({string.Join(", ", locked[g.Key])})\n");
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "dex-availability-audit.csv"), csv.ToString());
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "dex-availability-summary.txt"), summary.ToString());
        TestContext.Current.SendDiagnosticMessage(summary.ToString());

        // A fusão nunca perde o que o PokeAPI já dava.
        Assert.All(lost, l => Assert.Empty(l.Value));
    }
}

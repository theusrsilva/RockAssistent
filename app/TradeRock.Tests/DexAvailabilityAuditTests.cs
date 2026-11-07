using System.Collections.Frozen;
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
    public void Jogo_que_falha_no_PKHeX_fica_vazio_e_os_outros_continuam()
    {
        var logger = new CountingLogger();
        var table = PkhexAvailability.Build(key => key == "frlg"
            ? throw new InvalidOperationException("tabela quebrada")
            : PkhexAvailability.GameTable.Empty with { Statics = new HashSet<int> { 25 }.ToFrozenSet() }, logger);
        Assert.Equal(PkhexAvailability.Games.Count, table.Count);
        Assert.Same(PkhexAvailability.GameTable.Empty, table["frlg"]);
        Assert.Contains(25, (IReadOnlySet<int>)table["sv"].Statics);
        Assert.Equal(1, logger.Errors);
    }

    private sealed class CountingLogger : Microsoft.Extensions.Logging.ILogger
    {
        public int Errors { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => true;

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId,
            TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel == Microsoft.Extensions.Logging.LogLevel.Error)
                Errors++;
        }
    }

    [Fact]
    public void Auditoria_das_tres_fontes()
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido (rode scripts/build-dex.py).");
        var ct = TestContext.Current.CancellationToken;
        var dex = fx.Service;
        var store = new GameDataStore(dex, NullLogger<GameDataStore>.Instance);
        var csv = new StringBuilder("game,species,name,pokeapi_only,own,pkhex,final\n");
        var full = new StringBuilder("game,species,name,final\n");
        var summary = new StringBuilder();
        var changed = GameVersions.All.ToDictionary(g => g.Key, _ => new List<string>());
        var lost = GameVersions.All.ToDictionary(g => g.Key, _ => new List<string>());
        var locked = GameVersions.All.ToDictionary(g => g.Key, _ => new List<string>());
        var giftToWild = new List<string>();
        var requires = new Dictionary<(string Game, string Tag), List<string>>();
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
                full.Append($"{g.Key},{species},{name},{now}\n");
                if (old != now || pokeapiHas != pkhexHas)
                    csv.Append($"{g.Key},{species},{name},{old},{ownText},{pk},{now}\n");
                if (old == AvailabilityKind.None && now != AvailabilityKind.None)
                    changed[g.Key].Add($"{name} ({now})");
                if (old != AvailabilityKind.None && now == AvailabilityKind.None)
                    lost[g.Key].Add(name);
                // Presente ou troca só vira "na natureza" com slot selvagem (PKHeX) ou encontro selvagem nosso.
                if (old is AvailabilityKind.Gift or AvailabilityKind.Trade && now == AvailabilityKind.Encounter
                    && pk != AvailabilityKind.Encounter && own.All(m => m is "gift" or "trade"))
                    giftToWild.Add($"{g.Key} {name}");
                if (after.Single(a => a.GameKey == g.Key).Requires is { } tag)
                    (requires.TryGetValue((g.Key, tag), out var list) ? list : requires[(g.Key, tag)] = []).Add(name);
                if (after.Single(a => a.GameKey == g.Key).ShinyLocked)
                    locked[g.Key].Add(name);
            }
        }
        foreach (var g in GameVersions.All)
            summary.Append($"{g.Key}: +{changed[g.Key].Count} ({string.Join(", ", changed[g.Key].Take(10))})\n");
        summary.Append("\nShiny travado (todos os encontros travados e sem ovo):\n");
        foreach (var g in GameVersions.All)
            summary.Append($"{g.Key}: {locked[g.Key].Count} ({string.Join(", ", locked[g.Key])})\n");
        summary.Append("\nCom requisito (Pokéwalker ou item de evento):\n");
        foreach (var ((game, tag), names) in requires.OrderBy(r => r.Key.Game).ThenBy(r => r.Key.Tag))
            summary.Append($"{game} {tag}: {names.Count} ({string.Join(", ", names)})\n");
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "dex-availability-audit.csv"), csv.ToString());
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "dex-availability-summary.txt"), summary.ToString());
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "dex-availability-full.csv"), full.ToString());
        TestContext.Current.SendDiagnosticMessage(summary.ToString());

        // A fusão nunca perde o que o PokeAPI já dava.
        Assert.All(lost, l => Assert.Empty(l.Value));
        Assert.Empty(giftToWild);
    }
}

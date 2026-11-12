using Microsoft.Extensions.Logging.Abstractions;
using TradeRock.Core.Dex;
using TradeRock.Core.GameData;

namespace TradeRock.Tests;

/// <summary>Os nossos encontros juntos com os do PokeAPI (<see cref="EncounterSources"/>).</summary>
public sealed class EncounterSourcesTests(DexFixture fx) : IClassFixture<DexFixture>
{
    private static readonly CancellationToken Ct = TestContext.Current.CancellationToken;

    /// <summary>Os dados embutidos mais um arquivo próprio do FRLG: Pidgey a 40% na Rota 1, só no FireRed.</summary>
    private static GameDataStore WithFrlgFile(string method = "walk")
    {
        var files = new Dictionary<string, string>(GameDataStore.EmbeddedFiles())
        {
            ["encounters/frlg.json"] = $$"""
                {"source": ["https://example.org"], "items": [
                  {"area": "route-1", "species": 16, "form": 0, "method": "{{method}}", "min_level": 2, "max_level": 4,
                   "chance": "40%", "conditions": null, "versions": ["firered"], "alpha": false, "verified": true}]}
                """,
        };
        return new GameDataStore(files, null, NullLogger.Instance);
    }

    [Fact]
    public async Task Area_com_linhas_nossas_troca_as_selvagens_do_PokeAPI_so_na_versao_coberta()
    {
        var rows = await AreaEncounters.ListAsync("frlg", "route-1", WithFrlgFile(), fx.Service, Ct);
        Assert.NotNull(rows);
        // FireRed: só a nossa linha. LeafGreen: continua o PokeAPI (Pidgey e Rattata).
        var fire = rows.Where(r => r.Versions.Contains("firered")).ToList();
        var only = Assert.Single(fire);
        Assert.Equal((16, "walk", "40%"), (only.Species, only.Method, only.Chance));
        Assert.Contains(rows, r => r.Species == 19 && r.Versions.SequenceEqual(["leafgreen"]));
    }

    [Fact]
    public async Task Ficha_da_especie_junta_as_duas_fontes()
    {
        var store = WithFrlgFile();
        var pidgey = (await fx.Service.GetAsync(16, "frlg", "en", Ct))!;
        var merged = store.MergedSpeciesEncounters("frlg", 16, pidgey.Encounters);
        // Rota 1 no FireRed: a nossa (40%); as outras rotas e o LeafGreen continuam do PokeAPI.
        var route1 = merged.Where(e => e.Version == "firered" && e.Location == "Route 1").ToList();
        Assert.Equal(40, Assert.Single(route1).Chance);
        Assert.Contains(merged, e => e.Version == "leafgreen" && e.LocationId == 88);
        Assert.Contains(merged, e => e.Version == "firered" && e.LocationId is not null and not 88);

        // O Rattata da Rota 1 do FireRed sai (a área é nossa nessa versão) e fica no LeafGreen.
        var rattata = (await fx.Service.GetAsync(19, "frlg", "en", Ct))!;
        var left = store.MergedSpeciesEncounters("frlg", 19, rattata.Encounters);
        Assert.DoesNotContain(left, e => e.Version == "firered" && e.LocationId == 88);
        Assert.Contains(left, e => e.Version == "leafgreen" && e.LocationId == 88);
    }

    [Fact]
    public async Task Presente_nosso_nao_tira_as_selvagens_do_PokeAPI()
    {
        var rows = await AreaEncounters.ListAsync("frlg", "route-1", WithFrlgFile("gift"), fx.Service, Ct);
        Assert.Contains(rows!, r => r.Species == 19 && r.Versions.Contains("firered"));
    }

    [Fact]
    public async Task Jogo_sem_arquivo_proprio_fica_como_o_PokeAPI()
    {
        var store = new GameDataStore(GameDataStore.EmbeddedFiles(), null, NullLogger.Instance);
        var pidgey = (await fx.Service.GetAsync(16, "frlg", "en", Ct))!;
        Assert.Same(pidgey.Encounters, store.MergedSpeciesEncounters("frlg", 16, pidgey.Encounters));
    }

    /// <summary>
    /// HeartGold, Rota 30: o PokeAPI parte a chance do Caterpie da manhã em "com rádio desligado" (30) e sem condição de
    /// rádio (20). Rádio desligado é o normal: as duas somam 50%, como no Serebii.
    /// </summary>
    [Fact]
    public async Task Radio_desligado_nao_e_condicao_e_as_chances_somam()
    {
        var caterpie = (await fx.Service.GetAsync(10, "hgss", "en", Ct))!;
        var morning = caterpie.Encounters.Where(e => e.Version == "heartgold" && e.Location == "Route 30" && e.Method == "walk"
            && e.Conditions.Contains("time-morning")).ToList();
        var row = Assert.Single(morning);
        Assert.Equal(50, row.Chance);
        Assert.DoesNotContain(caterpie.Encounters, e => e.Conditions.Any(EncounterSources.DefaultConditions.Contains));
    }

    [Theory]
    [InlineData("10%", 10)]
    [InlineData("common", 0)]
    [InlineData(null, 0)]
    public void Chance_em_porcentagem(string? chance, int expected) => Assert.Equal(expected, EncounterSources.Percent(chance));
}

/// <summary>Os arquivos próprios do ORAS, BW, B2W2, XY, SM e USUM (Serebii) junto com o PokeAPI.</summary>
public sealed class OwnEncounterFilesTests(DexFixture fx) : IClassFixture<DexFixture>
{
    private static readonly CancellationToken Ct = TestContext.Current.CancellationToken;
    private readonly GameDataStore _store = new(GameDataStore.EmbeddedFiles(), null, NullLogger.Instance);

    private async Task<IReadOnlyList<EncounterInfo>> Where(string game, int species) =>
        _store.MergedSpeciesEncounters(game, species, (await fx.Service.GetAsync(species, game, "en", Ct))!.Encounters);

    /// <summary>O caso que abriu a entrega: o PokeAPI só tinha as hordas do Oddish no ORAS.</summary>
    [Fact]
    public async Task Oddish_aparece_na_grama_das_rotas_110_e_117_no_ORAS()
    {
        var oddish = await Where("oras", 43);
        foreach (var version in new[] { "omega-ruby", "alpha-sapphire" })
        {
            Assert.Contains(oddish, e => e is { Location: "Route 110", Method: "walk", MinLevel: 13, Chance: 10 } && e.Version == version);
            Assert.Contains(oddish, e => e is { Location: "Route 117", Method: "walk", MinLevel: 14, Chance: 10 } && e.Version == version);
            Assert.Contains(oddish, e => e is { Location: "Route 117", Method: "horde" } && e.Version == version);
        }
        // A horda do PokeAPI saiu das áreas que o nosso arquivo cobre: nada repetido.
        Assert.DoesNotContain(oddish, e => e.LocationId is not null && e.Method == "horde");
    }

    [Fact]
    public async Task Correcoes_do_Serebii_valem_sobre_o_PokeAPI()
    {
        // Black, Rota 4: Super Vara Krabby 65% (PokeAPI: 55%) e o Swarm do Hippopotas (não existe no PokeAPI).
        var krabby = await Where("bw", 98);
        Assert.Contains(krabby, e => e is { Location: "Route 4", Version: "black", Method: "fish", Chance: 65 });
        Assert.Contains(await Where("bw", 449), e => e is { Location: "Route 4", Version: "black", Method: "outbreak" });
        // X, Rota 7: Swirlix 5% (PokeAPI: 10%).
        Assert.Contains(await Where("xy", 684), e => e is { Location: "Route 7", Version: "x", Method: "walk", Chance: 5 });
        // Sun/Moon: o Kadabra só vem do Poké Pelago.
        Assert.Contains(await Where("sm", 64), e => e is { Location: "Poké Pelago", Method: "poke_pelago" });
    }

    /// <summary>Encontro fixo e presente do PokeAPI continuam nas áreas que o nosso arquivo cobre.</summary>
    [Fact]
    public async Task Fixos_e_presentes_do_PokeAPI_continuam()
    {
        Assert.Contains(await Where("oras", 352), e => e.Method == "devon-scope");
        Assert.Contains(await Where("oras", 380), e => e.Method is "static" or "gift");
    }

    /// <summary>
    /// Toda espécie que o PKHeX tem como selvagem no jogo aparece no "Onde achar" (nosso arquivo ou PokeAPI). As
    /// exceções são buracos conhecidos, anotados em <c>docs/superpowers/data-doubts.md</c>.
    /// </summary>
    [Theory]
    [InlineData("oras")]
    [InlineData("bw")]
    [InlineData("b2w2")]
    [InlineData("xy")]
    [InlineData("sm")]
    [InlineData("usum")]
    public async Task Toda_especie_selvagem_do_PKHeX_tem_onde_achar(string game)
    {
        var missing = new List<int>();
        foreach (var (species, kind) in PkhexAvailability.ForGame(game))
            if (kind == AvailabilityKind.Encounter && (await Where(game, species)).Count == 0)
                missing.Add(species);
        missing.Sort();
        Assert.True((KnownHoles.GetValueOrDefault(game) ?? []).SequenceEqual(missing), string.Join(", ", missing));
    }

    /// <summary>
    /// B2W2: Pokémon das Hidden Grottos que só saem com a Funfest Mission certa (Chansey, as evoluções do Eevee, Togetic,
    /// Shelgon, Glameow, Stunky). Não estão nas páginas de área do Serebii nem no PokeAPI.
    /// </summary>
    private static readonly Dictionary<string, int[]> KnownHoles = new()
    {
        ["b2w2"] = [113, 134, 135, 136, 176, 196, 197, 372, 431, 434, 470, 471],
    };
}

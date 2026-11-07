using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;
using TradeRock.App;
using TradeRock.Core;
using TradeRock.Core.Dex;
using TradeRock.Core.GameData;
using TradeRock.Core.Games;
using TradeRock.Core.Hunts;

namespace TradeRock.Tests;

public sealed class GameDataTests(DexFixture fx) : IClassFixture<DexFixture>
{
    private const string Source = "\"source\": [\"https://example.org\"]";

    private static GameDataStore FromFiles(IReadOnlyDictionary<string, string> files, Func<string, bool>? moveExists = null) =>
        new(files, moveExists, NullLogger.Instance);

    [Fact]
    public void Dados_embutidos_carregam_e_passam_no_validador()
    {
        var store = new GameDataStore(fx.Service, NullLogger<GameDataStore>.Instance);
        Assert.NotEmpty(store.ShinyMethods);
        Assert.DoesNotContain(store.Bosses("frlg"), b => b.Party.Count == 0);
    }

    [Fact]
    public void Masuda_vale_no_SV_e_tem_amuleto()
    {
        var store = FromFiles(GameDataStore.EmbeddedFiles());
        var masuda = Assert.Single(store.ShinyMethods, m => m.Id == "masuda");
        Assert.Contains("sv", masuda.Games);
        Assert.Contains(masuda.Modifiers, m => m.Id == "shiny_charm" && m.Kind == "toggle" && m.Odds is > 0);
        Assert.All(store.ShinyMethods, m => Assert.All(m.Games, g => GameVersions.ByKey(g)));
    }

    [Fact]
    public void Metodos_e_modificadores_tem_nome_em_ingles()
    {
        var store = FromFiles(GameDataStore.EmbeddedFiles());
        Assert.All(store.ShinyMethods, m =>
        {
            Assert.False(string.IsNullOrWhiteSpace(m.NameEn), m.Id);
            Assert.All(m.Modifiers, x => Assert.False(string.IsNullOrWhiteSpace(x.NameEn), $"{m.Id}/{x.Id}"));
        });
        Assert.Equal("Masuda Method", store.ShinyMethods.Single(m => m.Id == "masuda").NameEn);
        Assert.Equal("Shiny Charm", store.ShinyMethods.Single(m => m.Id == "masuda").Modifiers.Single(x => x.Id == "shiny_charm").NameEn);
    }

    [Fact]
    public void Validador_rejeita_nome_em_ingles_vazio()
    {
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(Shiny(
            """{ "id": "a", "name": "A", "name_en": " ", "description": "d", "base_odds": 4096, "base_rolls": null, "roll_odds": null, "games": ["sv"], "notes": null, "verified": true, "modifiers": [ { "id": "c", "name": "C", "name_en": "", "kind": "toggle", "steps": null, "odds": 100, "rolls": null, "counter": false, "games": null } ] }""")));
        Assert.Contains("shiny-methods/a: name e name_en", ex.Message);
        Assert.Contains("shiny-methods/a/c: name e name_en", ex.Message);
    }

    [Fact]
    public void Cada_arquivo_de_dados_desserializa()
    {
        var files = GameDataStore.EmbeddedFiles();
        Assert.Contains("shiny-methods.json", files.Keys);
        foreach (var (path, json) in files)
            GameDataStore.Parse(path, json); // lança se não desserializar
    }

    [Fact]
    public void Times_do_VGC_tem_6_Pokemon_que_existem_no_SV()
    {
        var store = FromFiles(GameDataStore.EmbeddedFiles());
        Assert.NotNull(store.VgcRegulation);
        Assert.InRange(store.VgcTeams.Count, 6, 10);
        foreach (var team in store.VgcTeams)
        {
            Assert.Equal(store.VgcRegulation!.Name, team.Regulation);
            var items = TradeRock.Core.Builder.ShowdownImport.Parse(team.Paste,
                new TradeRock.Core.Builder.TrainerSpec("VGC", 0, 0), TradeRock.Core.Trading.TradeGame.Scarlet);
            Assert.Equal(6, items.Count);
            // Toda espécie existe no SV (para analisar e importar para o Boxes); só item pode faltar (Mega Stones).
            Assert.All(items, i => Assert.True(i.Spec is not null, $"{team.Id}: {i.Text.Split('\n')[0]} {string.Join(",", i.Problems)}"));
            Assert.All(items, i => Assert.Equal(4, i.Spec!.Moves.Length));
            Assert.All(items, i => Assert.DoesNotContain(i.Problems, p => p.StartsWith("move", StringComparison.Ordinal) || p.StartsWith("ability", StringComparison.Ordinal)));
        }
    }

    [Fact]
    public void Validador_do_VGC_rejeita_paste_com_5_Pokemon_e_item_repetido()
    {
        // O @"\n" vira o escape \n do JSON (quebra de linha no paste).
        const string mon = @"Pikachu @ Light Ball\nAbility: Static\n- Thunderbolt\n\n";
        var regulation = GameDataStore.EmbeddedFiles()["vgc/regulation.json"];
        var files = new Dictionary<string, string>
        {
            ["vgc/regulation.json"] = regulation,
            ["vgc/teams.json"] = $$"""
                { {{Source}}, "items": [ { "id": "x", "name": "X", "archetype": "rain", "regulation": "Regulation Set M-C",
                  "date_iso": "2026-11-06", "paste": "{{string.Concat(Enumerable.Repeat(mon, 5))}}",
                  "idea_pt": "a", "idea_en": "a", "how_to_play_pt": "a", "how_to_play_en": "a", "dangers_pt": "a", "dangers_en": "a" } ] }
                """,
        };
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(files));
        Assert.Contains("5 Pokémon", ex.Message);
        Assert.Contains("Species Clause", ex.Message);
        Assert.Contains("Item Clause", ex.Message);
    }

    [Fact]
    public void Validador_rejeita_especie_inexistente()
    {
        var files = new Dictionary<string, string>
        {
            ["areas/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "id": "route-1", "name": "Route 1", "kind": "route", "order": 1,
                  "map_node": null, "poke_api_location_id": null, "bosses_before": [] } ] }
                """,
            ["encounters/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "area": "route-1", "species": 2000, "form": 0, "method": "walk",
                  "min_level": 2, "max_level": 4, "chance": "common", "conditions": null, "versions": ["firered"],
                  "alpha": false, "verified": true } ] }
                """,
        };
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(files));
        Assert.Contains("2000", ex.Message);
    }

    [Fact]
    public void Validador_confere_referencias_metodos_e_golpes()
    {
        var files = new Dictionary<string, string>
        {
            ["bosses/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "id": "brock", "kind": "gym", "name": "Brock", "title": null, "location": null,
                  "order": 1, "badge": "boulder", "party": [ { "species": 74, "form": 0, "level": 12,
                  "moves": ["tackle", "not-a-move"], "ability": null, "item": null, "tera_type": null } ],
                  "level_cap": 14, "verified": true, "notes": null, "versions": ["leafgreen", "ruby"] } ] }
                """,
            ["areas/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "id": "pewter", "name": "Pewter", "kind": "city", "order": 1,
                  "map_node": "pewter", "poke_api_location_id": null, "bosses_before": ["misty"] } ] }
                """,
            ["encounters/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "area": "nowhere", "species": 16, "form": 0, "method": "teleport",
                  "min_level": 2, "max_level": 4, "chance": "common", "conditions": null, "versions": ["firered"],
                  "alpha": false, "verified": true } ] }
                """,
            ["trainers/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "id": "t1", "area": "pewter", "class": "Bug Catcher", "name": "Rick",
                  "party": [ { "species": 10, "form": 0, "level": 6, "moves": [], "ability": null, "item": null,
                  "tera_type": null } ], "verified": true, "versions": ["firered"] },
                  { "id": "t2", "area": "pewter", "class": "Lass", "name": "Ana",
                  "party": [ { "species": 16, "form": 0, "level": 6, "moves": [], "ability": null, "item": null,
                  "tera_type": null } ], "verified": true, "versions": ["sapphire"] } ] }
                """,
        };
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(files, m => m == "tackle"));
        Assert.Contains("not-a-move", ex.Message);
        Assert.Contains("level_cap", ex.Message);  // 14 em vez de 12
        Assert.Contains("misty", ex.Message);      // chefe que não existe
        Assert.Contains("pewter", ex.Message);     // nó que não existe no mapa de Kanto
        Assert.Contains("nowhere", ex.Message);    // área que não existe
        Assert.Contains("teleport", ex.Message);   // método fora da lista
        Assert.Contains("versão ruby", ex.Message); // versão de outro jogo no chefe
        Assert.DoesNotContain("versão leafgreen", ex.Message);
        Assert.DoesNotContain("t1", ex.Message);   // o treinador está certo
        Assert.Contains("t2: versão sapphire", ex.Message); // versão de outro jogo no treinador

        // Sem o dex (moveExists nulo), os golpes não são conferidos.
        var noDex = Assert.Throws<InvalidDataException>(() => FromFiles(files));
        Assert.DoesNotContain("not-a-move", noDex.Message);
    }

    private static Dictionary<string, string> RivalWithVariants(string variants) => new()
    {
        ["bosses/hgss.json"] = $$"""
            { {{Source}}, "items": [ { "id": "rival", "kind": "rival", "name": "Silver", "title": null, "location": null,
              "order": 1, "badge": null, "party": [ { "species": 158, "form": 0, "level": 5, "moves": [], "ability": null,
              "item": null, "tera_type": null } ], "level_cap": 5, "verified": true, "notes": null, "versions": null,
              "variants": {{variants}} } ] }
            """,
    };

    private static string Variant(string starter, int level, int cap, string moves = "") => $$"""
        { "starter": "{{starter}}", "party": [ { "species": 155, "form": 0, "level": {{level}}, "moves": [{{moves}}],
          "ability": null, "item": null, "tera_type": null } ], "level_cap": {{cap}}, "verified": true }
        """;

    [Fact]
    public void Variantes_por_inicial_validas_carregam()
    {
        var store = FromFiles(RivalWithVariants($"[{Variant("chikorita", 6, 6)}, {Variant("totodile", 5, 5)}]"));
        var rival = Assert.Single(store.Bosses("hgss"));
        Assert.Equal(["chikorita", "totodile"], rival.Variants!.Select(v => v.Starter));
        Assert.Equal(6, rival.ForStarter("chikorita").LevelCap);
        // Sem o campo "variants" continua valendo (um time só).
        Assert.Null(FromFiles(new Dictionary<string, string> { ["bosses/frlg.json"] = GameDataStore.EmbeddedFiles()["bosses/frlg.json"] }).Bosses("frlg")[0].Variants);
    }

    [Fact]
    public void Validador_rejeita_variante_com_inicial_de_outro_jogo_repetida_ou_cap_errado()
    {
        var badMove = Variant("totodile", 5, 5, "\"not-a-move\"");
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(RivalWithVariants(
            $"[{Variant("mudkip", 5, 5)}, {Variant("chikorita", 6, 9)}, {badMove}, {Variant("totodile", 5, 5)}]"),
            m => m != "not-a-move"));
        Assert.Contains("bosses/hgss/rival/variants/mudkip: mudkip não é inicial do hgss", ex.Message);
        Assert.Contains("variants/chikorita: level_cap 9 diferente do maior nível do time (6)", ex.Message);
        Assert.Contains("variante repetida para totodile", ex.Message);
        Assert.Contains("variants/totodile: golpe not-a-move", ex.Message);
        Assert.Contains("variants vazio", Assert.Throws<InvalidDataException>(() => FromFiles(RivalWithVariants("[]"))).Message);
    }

    [Fact]
    public void Rivais_do_HGSS_tem_os_tres_times()
    {
        var store = FromFiles(GameDataStore.EmbeddedFiles());
        var rivals = store.Bosses("hgss").Where(b => b.Id.StartsWith("rival-", StringComparison.Ordinal)).ToList();
        Assert.Equal(7, rivals.Count);
        Assert.All(rivals, r =>
        {
            Assert.Equal(["chikorita", "cyndaquil", "totodile"], r.Variants!.Select(v => v.Starter));
            // O time padrão é o de quem escolheu Cyndaquil.
            Assert.Equal(System.Text.Json.JsonSerializer.Serialize(r.Party),
                System.Text.Json.JsonSerializer.Serialize(r.Variants!.Single(v => v.Starter == "cyndaquil").Party));
            Assert.DoesNotContain("com Chikorita", r.Notes ?? "");
        });
        // O rival fica com o inicial que tem vantagem sobre o do jogador.
        var first = rivals.Single(r => r.Id == "rival-cherrygrove");
        Assert.Equal(155, first.ForStarter("chikorita").Party[0].Species);
        Assert.Equal(158, first.ForStarter("cyndaquil").Party[0].Species);
        Assert.Equal(152, first.ForStarter("totodile").Party[0].Species);
    }

    [Fact]
    public void Iniciais_de_todos_os_jogos()
    {
        Assert.All(GameVersions.All, g => Assert.NotEmpty(GameStarters.For(g.Key)));
        Assert.All(GameVersions.All.Where(g => g.Key is not ("y" or "lgpe")), g => Assert.Equal(3, GameStarters.For(g.Key).Count));
        Assert.Equal(["pikachu"], GameStarters.For("y").Select(s => s.Id));
        Assert.Equal(["chikorita", "tepig", "totodile"], GameStarters.For("za").Select(s => s.Id));
        Assert.Empty(GameStarters.For("zz"));
    }

    // Gen 1–5: os chefes da história (até o campeão) têm, em cada versão, 8 líderes com insígnias distintas, a Elite 4
    // (4 pessoas) e 1 campeão; o resto (Kanto no GSC/HGSS, revanches, Frontier Brains) vem depois, com Order maior.
    // No BW, o Alder só é enfrentado no pós-jogo, depois da revanche da Elite 4, por isso a Elite 4 aparece duas vezes
    // antes dele. Ginásio só de uma versão (Drayden no Black, Iris no White) conta só nela. "drops" lista as quedas de
    // nível entre ginásios seguidos que o jogo tem de verdade (no GSC e no HGSS, a Jasmine vem antes do Pryce e tem
    // time mais forte).
    // drops: "idA>idB" separados por vírgula, na ordem de Order; vazio = o cap nunca cai.
    [Theory]
    [InlineData("rb", "")]
    [InlineData("y", "")]
    [InlineData("gs", "jasmine>pryce")]
    [InlineData("c", "jasmine>pryce")]
    [InlineData("rs", "")]
    [InlineData("e", "")]
    [InlineData("frlg", "")]
    [InlineData("dp", "")]
    [InlineData("pt", "")]
    [InlineData("hgss", "jasmine>pryce")]
    [InlineData("bw", "")]
    [InlineData("b2w2", "")]
    public void Chefes_da_historia_das_geracoes_1_a_5(string game, string drops)
    {
        var bosses = FromFiles(GameDataStore.EmbeddedFiles()).Bosses(game).OrderBy(b => b.Order).ToList();
        Assert.Equal(bosses.Count, bosses.Select(b => b.Order).Distinct().Count());
        var champion = Assert.Single(bosses, b => b.Kind == "champion" && b.Order == bosses.Where(x => x.Kind == "champion").Min(x => x.Order));
        var story = bosses.Where(b => b.Order <= champion.Order).ToList();

        Assert.All(bosses.Where(b => b.Kind == "gym"), g => Assert.False(string.IsNullOrWhiteSpace(g.Badge)));
        foreach (var version in GameVersions.ByKey(game).PokeApiVersions)
        {
            var gyms = story.Where(b => b.Kind == "gym" && (b.Versions is null || b.Versions.Contains(version))).ToList();
            Assert.Equal(8, gyms.Count);
            Assert.Equal(8, gyms.Select(g => g.Badge).Distinct().Count());
            var found = gyms.Zip(gyms.Skip(1)).Where(p => p.Second.LevelCap < p.First.LevelCap)
                .Select(p => $"{p.First.Id}>{p.Second.Id}");
            Assert.Equal(drops, string.Join(",", found));
        }
        var elite = story.Where(b => b.Kind == "elite").ToList();
        Assert.Equal(4, elite.Select(e => e.Name).Distinct().Count());
        Assert.Equal(game == "bw" ? 8 : 4, elite.Count);
        Assert.Single(story, b => b.Kind == "champion");
        Assert.True(elite.All(e => e.Order < champion.Order));

        var rivals = bosses.Where(b => b.Kind == "rival").ToList();
        Assert.True(rivals.Count >= 3);
        Assert.All(rivals.Zip(rivals.Skip(1)), p => Assert.True(p.Second.LevelCap >= p.First.LevelCap, $"{p.First.Id} -> {p.Second.Id}"));
        // Rival no pós-jogo: GSC (Mt. Moon), Gen 4 (Survival Area, Mt. Moon) e Gen 5 (revanches).
        Assert.True(rivals.Max(r => r.Order) < champion.Order || game is "gs" or "c" or "dp" or "pt" or "hgss" or "bw" or "b2w2");
    }

    // Gen 6–9: cada jogo tem a sua estrutura, então o teste recebe o que esperar de cada um.
    // gyms: lutas kind=gym da história em cada versão, com insígnias (ou cristais Z / ranks) distintas. Alola conta os
    // trials (title "Captain"; a luta é o Totem; o do Kommo-o, sem capitão, tem title "Totem") e os grand trials (title "Kahuna"); no Z-A são as partidas de promoção
    // (Rank Z→Y ... B→A); o Legends: Arceus não tem ginásio.
    // elite: lutas da Elite 4 antes do campeão (nomes distintos). O Sword/Shield não tem Elite 4 (a final da Champion
    // Cup é "other").
    // titles: "kind:Title=n" separados por vírgula, contados na história em cada versão.
    // drops: quedas reais de nível entre ginásios seguidos ("idA>idB"), como no teste da Gen 1–5.
    // O campeão da história é o primeiro kind=champion (no Legends: Arceus, o Palkia/Dialga Origem). O Z-A não tem
    // campeão: a luta final (Floette Hyperrogue) não tem nível em nenhuma fonte e fica de fora; ali a história é o arquivo
    // inteiro (as revanches do pós-jogo são "other"). Rivais: o level cap de cada rival (por nome) não cai.
    [Theory]
    [InlineData("xy", 8, 4, "", "")]
    [InlineData("oras", 8, 4, "", "")]
    [InlineData("sm", 11, 4, "gym:Captain=6,gym:Totem=1,gym:Kahuna=4", "hapu-1>totem-kommo-o")]
    [InlineData("usum", 12, 4, "gym:Captain=7,gym:Totem=1,gym:Kahuna=4", "totem-ribombee>hapu-1")]
    [InlineData("lgpe", 8, 4, "", "")]
    [InlineData("swsh", 8, 0, "other:Champion Cup=3", "")]
    [InlineData("bdsp", 8, 4, "", "")]
    [InlineData("pla", 0, 0, "other:Noble=5", "")]
    [InlineData("sv", 8, 4, "gym:Victory Road=8,other:Path of Legends=5,evil:Starfall Street=5", "")]
    [InlineData("za", 10, 0, "", "")]
    public void Chefes_da_historia_das_geracoes_6_a_9(string game, int gyms, int elite, string titles, string drops)
    {
        var bosses = FromFiles(GameDataStore.EmbeddedFiles()).Bosses(game).OrderBy(b => b.Order).ToList();
        Assert.Equal(bosses.Count, bosses.Select(b => b.Order).Distinct().Count());
        var champion = bosses.FirstOrDefault(b => b.Kind == "champion");
        Assert.True(champion is not null || game == "za", "sem campeão");
        var story = bosses.Where(b => champion is null || b.Order <= champion.Order).ToList();

        Assert.All(bosses.Where(b => b.Kind == "gym"), g => Assert.False(string.IsNullOrWhiteSpace(g.Badge)));
        foreach (var version in GameVersions.ByKey(game).PokeApiVersions)
        {
            var inVersion = story.Where(b => b.Versions is null || b.Versions.Contains(version)).ToList();
            var g = inVersion.Where(b => b.Kind == "gym").ToList();
            Assert.Equal(gyms, g.Count);
            Assert.Equal(gyms, g.Select(x => x.Badge).Distinct().Count());
            var found = g.Zip(g.Skip(1)).Where(p => p.Second.LevelCap < p.First.LevelCap).Select(p => $"{p.First.Id}>{p.Second.Id}");
            Assert.Equal(drops, string.Join(",", found));
            foreach (var t in titles.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var (kind, rest) = (t.Split(':')[0], t.Split(':')[1]);
                var (title, count) = (rest.Split('=')[0], int.Parse(rest.Split('=')[1]));
                Assert.True(count == inVersion.Count(b => b.Kind == kind && b.Title == title), $"{version}: {t}");
            }
        }
        var e4 = story.Where(b => b.Kind == "elite").ToList();
        Assert.Equal(elite, e4.Count);
        Assert.Equal(elite, e4.Select(e => e.Name).Distinct().Count());
        Assert.Equal(champion is null ? 0 : 1, story.Count(b => b.Kind == "champion"));

        var rivals = bosses.Where(b => b.Kind == "rival").ToList();
        Assert.True(rivals.Count >= 3, "rivais");
        foreach (var r in rivals.GroupBy(b => b.Name))
            Assert.All(r.Zip(r.Skip(1)), p => Assert.True(p.Second.LevelCap >= p.First.LevelCap, $"{p.First.Id} -> {p.Second.Id}"));
    }

    // Treinadores comuns (não chefes) das Gen 1 a 5: cobertura mínima, área do arquivo de áreas, espécie e versão válidas,
    // nenhum treinador com o nome e o time de um chefe (os chefes ficam em bosses/).
    [Theory]
    [InlineData("rb")]
    [InlineData("y")]
    [InlineData("gs")]
    [InlineData("c")]
    [InlineData("rs")]
    [InlineData("e")]
    [InlineData("frlg")]
    [InlineData("dp")]
    [InlineData("pt")]
    [InlineData("hgss")]
    [InlineData("bw")]
    [InlineData("b2w2")]
    public void Treinadores_comuns_das_geracoes_1_a_5(string game)
    {
        var store = FromFiles(GameDataStore.EmbeddedFiles());
        var trainers = store.Trainers(game);
        Assert.True(trainers.Count >= 150, $"{game}: só {trainers.Count} treinadores");
        Assert.Contains(game, store.GamesWithTrainers);
        var areas = store.Areas(game).Select(a => a.Id).ToHashSet();
        var versions = GameVersions.ByKey(game).PokeApiVersions;
        static string Team(string name, IEnumerable<BossPokemon> party) =>
            name.ToLowerInvariant() + "|" + string.Join(",", party.Select(p => $"{p.Species}:{p.Level}"));
        var bossTeams = store.Bosses(game).Select(b => Team(b.Name, b.Party)).ToHashSet();
        Assert.All(trainers, t =>
        {
            Assert.Contains(t.Area, areas);
            Assert.False(string.IsNullOrWhiteSpace(t.Class), t.Id);
            Assert.False(string.IsNullOrWhiteSpace(t.Name), t.Id);
            Assert.NotEmpty(t.Party);
            Assert.All(t.Party, p => Assert.InRange(p.Species, 1, GameDataValidator.MaxSpecies));
            Assert.All(t.Versions ?? [], v => Assert.Contains(v, versions));
            Assert.DoesNotContain(Team(t.Name, t.Party), bossTeams);
        });
        // Espalhados pelo jogo, não concentrados em poucas áreas.
        Assert.True(trainers.Select(t => t.Area).Distinct().Count() >= 30, $"{game}: poucas áreas com treinador");
    }

    [Fact]
    public void Jogos_com_treinadores_sao_os_que_tem_arquivo()
    {
        var files = GameDataStore.EmbeddedFiles();
        var store = FromFiles(files);
        var expected = files.Keys.Where(k => k.StartsWith("trainers/", StringComparison.Ordinal))
            .Select(k => k["trainers/".Length..^".json".Length]).Order(StringComparer.Ordinal).ToList();
        Assert.NotEmpty(expected);
        Assert.Equal(expected, store.GamesWithTrainers.Order(StringComparer.Ordinal));
    }

    [Fact]
    public void Validador_rejeita_jogo_desconhecido_nos_metodos()
    {
        var files = new Dictionary<string, string>
        {
            ["shiny-methods.json"] = $$"""
                { {{Source}}, "items": [ { "id": "x", "name": "X", "name_en": "X", "description": "d", "base_odds": 4096,
                  "base_rolls": null, "roll_odds": null, "modifiers": [], "games": ["zz"], "notes": null, "verified": false } ] }
                """,
        };
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(files));
        Assert.Contains("zz", ex.Message);
    }

    private static double Combine(ShinyMethod m, params int[] extra) =>
        ShinyOdds.FromRolls(m.RollOdds!.Value, m.BaseRolls!.Value + extra.Sum());

    private static int Rolls(ShinyMethod m, string modifier, string? step = null)
    {
        var mod = m.Modifiers.Single(x => x.Id == modifier);
        return step is null ? mod.Rolls!.Value : mod.Steps!.Single(s => s.Label == step).Rolls!.Value;
    }

    [Fact]
    public void Regra_das_rolagens_reproduz_as_odds_das_fontes()
    {
        var store = FromFiles(GameDataStore.EmbeddedFiles());
        ShinyMethod M(string id) => store.ShinyMethods.Single(m => m.Id == id);

        var sos = M("sos_chain");
        Assert.Equal(273.53, Combine(sos, Rolls(sos, "chain", "31+"), Rolls(sos, "shiny_charm")), 0.01);
        var brilliant = M("brilliant");
        Assert.Equal(455.56, Combine(brilliant, Rolls(brilliant, "battled", "500+"), Rolls(brilliant, "shiny_charm")), 0.01);
        var outbreak = M("pla_outbreak");
        Assert.Equal(137.02, Combine(outbreak, Rolls(outbreak, "shiny_charm"), Rolls(outbreak, "research_10")), 0.01);
        var sv = M("sv_wild");
        Assert.Equal(512.44, Combine(sv, Rolls(sv, "sparkling_power", "nível 3"), Rolls(sv, "outbreak", "60+"), Rolls(sv, "shiny_charm")), 0.01);

        // Sem rolagens: não combina.
        Assert.Null(M("dynamax_adventure").BaseRolls);
        Assert.All(M("dynamax_adventure").Modifiers, x => Assert.Null(x.Rolls));
        // Amuleto só no B2W2 nos métodos da Gen 5.
        Assert.Equal(["b2w2"], M("masuda_gen5").Modifiers.Single(x => x.Id == "shiny_charm").Games);
        Assert.All(store.ShinyMethods, m => Assert.True(m.Modifiers.Count(x => x.Counter) <= 1));
        Assert.True(M("sos_chain").Modifiers.Single(x => x.Id == "chain").Counter);
    }

    private static Dictionary<string, string> Shiny(string method) => new()
    {
        ["shiny-methods.json"] = $$"""{ "source": [], "items": [ {{method}} ] }""",
    };

    public static TheoryData<string, string> MetodosInvalidos { get; } = new()
    {
        {
            """
            { "id": "a", "name": "A", "name_en": "A", "description": "d", "base_odds": 4096, "base_rolls": 1, "roll_odds": 4096, "games": ["sv"], "notes": null, "verified": true, "modifiers": [ { "id": "c", "name": "C", "name_en": "C", "kind": "toggle", "steps": null, "odds": 1000, "rolls": 2, "counter": false, "games": null } ] }
            """,
            "não batem"
        },
        {
            """
            { "id": "a", "name": "A", "name_en": "A", "description": "d", "base_odds": 4096, "base_rolls": null, "roll_odds": null, "games": ["sv"], "notes": null, "verified": true, "modifiers": [ { "id": "x", "name": "X", "name_en": "X", "kind": "steps", "steps": [ { "label": "1", "min": 0, "max": null, "odds": 10, "rolls": null } ], "odds": null, "rolls": null, "counter": true, "games": null }, { "id": "y", "name": "Y", "name_en": "Y", "kind": "steps", "steps": [ { "label": "1", "min": 0, "max": null, "odds": 10, "rolls": null } ], "odds": null, "rolls": null, "counter": true, "games": null } ] }
            """,
            "counter"
        },
        {
            """
            { "id": "a", "name": "A", "name_en": "A", "description": "d", "base_odds": 4096, "base_rolls": null, "roll_odds": null, "games": ["sv"], "notes": null, "verified": true, "modifiers": [ { "id": "c", "name": "C", "name_en": "C", "kind": "toggle", "steps": null, "odds": 100, "rolls": null, "counter": false, "games": ["swsh"] } ] }
            """,
            "swsh"
        },
        {
            """
            { "id": "a", "name": "A", "name_en": "A", "description": "d", "base_odds": 4096, "base_rolls": 1, "roll_odds": 4096, "games": ["sv"], "notes": null, "verified": true, "modifiers": [ { "id": "c", "name": "C", "name_en": "C", "kind": "toggle", "steps": null, "odds": 100, "rolls": null, "counter": false, "games": null } ] }
            """,
            "rolls"
        },
    };

    [Theory]
    [MemberData(nameof(MetodosInvalidos))]
    public void Validador_confere_rolagens_contador_e_jogos_dos_modificadores(string method, string expected)
    {
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(Shiny(method)));
        Assert.Contains(expected, ex.Message);
    }

    [Theory]
    [InlineData("""{ "source": [], "items": [ { "id": "x", "name": "X", "name_en": "X", "description": "d", "base_odds": 4096, "base_rolls": null, "roll_odds": null, "modifiers": [], "games": ["sv"], "notes": null } ] }""")]
    [InlineData("""{ "source": [], "items": [ { "id": "x", "name": "X", "name_en": "X", "description": "d", "base_odds": 4096, "base_rolls": null, "roll_odds": null, "modifiers": [], "games": ["sv"], "notes": null, "verified": true, "extra": 1 } ] }""")]
    [InlineData("""{ "source": [], "items": [ { "id": "x", "name": null, "name_en": "X", "description": "d", "base_odds": 4096, "base_rolls": null, "roll_odds": null, "modifiers": [], "games": ["sv"], "notes": null, "verified": true } ] }""")]
    [InlineData("""{ "items": [] }""")]
    public void Arquivo_fora_do_esquema_falha(string json)
    {
        Assert.ThrowsAny<Exception>(() => GameDataStore.Parse("shiny-methods.json", json));
        Assert.Throws<InvalidDataException>(() => FromFiles(new Dictionary<string, string> { ["shiny-methods.json"] = json }));
    }

    [Fact]
    public void Jogo_sem_arquivo_da_lista_vazia_e_mapa_valido_carrega()
    {
        var files = new Dictionary<string, string>
        {
            ["maps/kanto.json"] = $$"""
                { {{Source}}, "region": "kanto", "nodes": [ { "id": "pallet", "name": "Pallet Town", "kind": "city",
                  "x": 0.1, "y": 0.9, "games": null }, { "id": "route-1", "name": "Route 1", "kind": "route",
                  "x": 0.1, "y": 0.8, "games": ["frlg"] } ], "edges": [ { "from": "pallet", "to": "route-1" } ] }
                """,
            ["trainers/frlg.json"] = $$"""{ {{Source}}, "items": [] }""",
        };
        var store = FromFiles(files);
        Assert.Empty(store.Bosses("sv"));
        Assert.Empty(store.Areas("sv"));
        Assert.Empty(store.Encounters("sv"));
        Assert.Empty(store.GamesWithTrainers);
        Assert.Null(store.Map(Region.Johto));
        var kanto = store.Map(Region.Kanto);
        Assert.NotNull(kanto);
        Assert.Equal(2, kanto.Nodes.Count);
    }

    [Theory]
    [InlineData("rb", "pallet-town")]
    [InlineData("y", "pallet-town")]
    [InlineData("frlg", "pallet-town")]
    [InlineData("lgpe", "pallet-town")]
    [InlineData("gs", "new-bark-town")]
    [InlineData("c", "new-bark-town")]
    [InlineData("hgss", "new-bark-town")]
    [InlineData("rs", "littleroot-town")]
    [InlineData("e", "littleroot-town")]
    [InlineData("dp", "twinleaf-town")]
    [InlineData("pt", "twinleaf-town")]
    [InlineData("bdsp", "twinleaf-town")]
    [InlineData("bw", "nuvema-town")]
    [InlineData("b2w2", "aspertia-city")]
    [InlineData("xy", "vaniville-town")]
    [InlineData("oras", "littleroot-town")]
    [InlineData("sm", "iki-town")]
    [InlineData("usum", "iki-town")]
    [InlineData("swsh", "postwick")]
    [InlineData("pla", "jubilife-village")]
    [InlineData("sv", "cabo-poco")]
    [InlineData("za", "gare-de-lumiose")]
    public void Areas_em_ordem_com_mapa_locais_do_dex_e_chefes(string game, string start)
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido (rode scripts/build-dex.py).");
        var store = new GameDataStore(fx.Service, NullLogger<GameDataStore>.Instance);
        var areas = store.Areas(game);
        Assert.NotEmpty(areas);
        Assert.Equal(start, areas[0].Id);
        Assert.Equal(Enumerable.Range(1, areas.Count), areas.Select(a => a.Order));

        var locations = new HashSet<int>();
        using (var c = fx.Db.Open())
        using (var cmd = c.CreateCommand())
        {
            cmd.CommandText = "SELECT id FROM locations";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                locations.Add(r.GetInt32(0));
        }
        foreach (var a in areas.Where(a => a.PokeApiLocationId is not null))
            Assert.True(locations.Contains(a.PokeApiLocationId!.Value), $"{game}/{a.Id}: location {a.PokeApiLocationId} não existe no dex");

        var map = store.Map(GameVersions.ByKey(game).Region);
        Assert.NotNull(map);
        var nodes = map.Nodes.ToDictionary(n => n.Id);
        foreach (var a in areas.Where(a => a.MapNode is not null))
        {
            Assert.True(nodes.TryGetValue(a.MapNode!, out var n), $"{game}/{a.Id}: nó {a.MapNode} não existe");
            Assert.True(n.Games is null || n.Games.Contains(game), $"{game}/{a.Id}: nó {a.MapNode} não é deste jogo");
        }

        var bosses = store.Bosses(game);
        var ids = bosses.Select(b => b.Id).ToHashSet();
        var linked = areas.SelectMany(a => a.BossesBefore).ToList();
        Assert.All(linked, id => Assert.Contains(id, ids));
        Assert.Equal(linked.Count, linked.Distinct().Count());
        foreach (var gym in bosses.Where(b => b.Kind == "gym" && !b.Id.Contains("rematch")))
            Assert.True(linked.Contains(gym.Id), $"{game}: ginásio {gym.Id} não aparece em bosses_before");
    }

    [Theory]
    [InlineData("bdsp", "brilliant-diamond")]
    [InlineData("pla", "legends-arceus")]
    [InlineData("sv", "scarlet")]
    [InlineData("za", "legends-za")]
    public void Encontros_proprios_com_area_especie_metodo_e_versao(string game, string someVersion)
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido (rode scripts/build-dex.py).");
        var store = new GameDataStore(fx.Service, NullLogger<GameDataStore>.Instance);
        var encounters = store.Encounters(game);
        Assert.NotEmpty(encounters);
        var areas = store.Areas(game).Select(a => a.Id).ToHashSet();
        var versions = GameVersions.ByKey(game).PokeApiVersions;
        Assert.All(encounters, e =>
        {
            Assert.Contains(e.Area, areas);
            Assert.InRange(e.Species, 1, GameDataValidator.MaxSpecies);
            Assert.Contains(e.Method, GameDataValidator.EncounterMethods);
            Assert.NotEmpty(e.Versions);
            Assert.All(e.Versions, v => Assert.Contains(v, versions));
            Assert.InRange(e.MinLevel, 1, e.MaxLevel);
        });
        Assert.Contains(encounters, e => e.Versions.Contains(someVersion));
        // Os Legends têm Alphas; os outros não.
        Assert.Equal(game is "pla" or "za", encounters.Any(e => e.Alpha));
        // Cobertura mínima: não é um arquivo de uma área só.
        Assert.True(encounters.Select(e => e.Area).Distinct().Count() >= 10, $"{game}: poucas áreas com encontro");
    }

    [Fact]
    public void Mapas_sem_aresta_repetida()
    {
        var store = FromFiles(GameDataStore.EmbeddedFiles());
        foreach (var region in Enum.GetValues<Region>())
        {
            var map = store.Map(region);
            Assert.NotNull(map);
            var keys = map.Edges.Select(e => string.CompareOrdinal(e.From, e.To) < 0 ? (e.From, e.To) : (e.To, e.From)).ToList();
            Assert.Equal(keys.Count, keys.Distinct().Count());
        }
    }

    [Fact]
    public void Mapa_com_aresta_para_no_inexistente_falha()
    {
        var files = new Dictionary<string, string>
        {
            ["maps/kanto.json"] = $$"""
                { {{Source}}, "region": "kanto", "nodes": [ { "id": "pallet", "name": "Pallet Town", "kind": "city",
                  "x": 0, "y": 0, "games": null } ], "edges": [ { "from": "pallet", "to": "viridian" } ] }
                """,
        };
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(files));
        Assert.Contains("viridian", ex.Message);
    }
}

public sealed class GameDataApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-dataapi-").FullName;
    private WebApplication? _app;
    private HttpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        _app = TradeRockServer.Create(["--data-dir", _dir], _token, b => b.WebHost.UseTestServer());
        await _app.StartAsync();
        _client = _app.GetTestClient();
        _client.BaseAddress = new Uri("http://127.0.0.1/");
        _client.DefaultRequestHeaders.Add(SessionAuthMiddleware.HeaderName, _token.Value);
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
            await _app.DisposeAsync();
        SqliteConnection.ClearAllPools();
        try { Directory.Delete(_dir, true); } catch (IOException) { }
    }

    [Fact]
    public async Task Rotas_de_dados()
    {
        var all = await _client.GetFromJsonAsync<JsonElement>("/api/data/shiny-methods");
        var sv = await _client.GetFromJsonAsync<JsonElement>("/api/data/shiny-methods?game=sv");
        Assert.True(all.GetArrayLength() > sv.GetArrayLength());
        var masuda = sv.EnumerateArray().Single(m => m.GetProperty("id").GetString() == "masuda");
        Assert.Equal(683.08, masuda.GetProperty("baseOdds").GetDouble(), 2);
        Assert.Equal("Masuda Method", masuda.GetProperty("nameEn").GetString());
        Assert.Equal("Shiny Charm", masuda.GetProperty("modifiers")[0].GetProperty("nameEn").GetString());
        Assert.All(sv.EnumerateArray(), m =>
            Assert.Contains("sv", m.GetProperty("games").EnumerateArray().Select(g => g.GetString())));

        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/data/shiny-methods?game=zz")).StatusCode);
        foreach (var kind in new[] { "bosses", "trainers", "areas", "encounters" })
        {
            var r = await _client.GetAsync($"/api/data/frlg/{kind}");
            Assert.Equal(HttpStatusCode.OK, r.StatusCode);
            Assert.Equal(JsonValueKind.Array, (await r.Content.ReadFromJsonAsync<JsonElement>()).ValueKind);
            Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync($"/api/data/zz/{kind}")).StatusCode);
        }
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/data/maps/atlantis")).StatusCode);
        // Toda região tem mapa desde a Task 13: o 404 não tem mais como aparecer com os dados embutidos.
        var unova = await _client.GetFromJsonAsync<JsonElement>("/api/data/maps/unova");
        Assert.Equal("unova", unova.GetProperty("region").GetString());
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/data/maps/5")).StatusCode);
    }

    [Fact]
    public async Task Encontros_da_area_e_maquinas()
    {
        Assert.SkipUnless(TradeRock.Core.Dex.DexDatabase.ResourceExists, "Falta o dex.sqlite embutido (rode scripts/build-dex.py).");
        // FRLG: os do PokeAPI pela área (Pidgey e Rattata na Route 1, nas duas versões).
        var route1 = await _client.GetFromJsonAsync<JsonElement>("/api/data/frlg/areas/route-1/encounters");
        Assert.Contains(route1.EnumerateArray(), e => e.GetProperty("species").GetInt32() == 16
            && e.GetProperty("versions").GetArrayLength() == 2 && e.GetProperty("chance").GetString()!.EndsWith('%'));
        // SV: os nossos (o Sprigatito é presente em Cabo Poco).
        var poco = await _client.GetFromJsonAsync<JsonElement>("/api/data/sv/areas/cabo-poco/encounters");
        Assert.Contains(poco.EnumerateArray(), e => e.GetProperty("species").GetInt32() == 906);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/api/data/frlg/areas/atlantis/encounters")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/data/zz/areas/route-1/encounters")).StatusCode);

        var tms = await _client.GetFromJsonAsync<JsonElement>("/api/dex/machines?game=frlg");
        Assert.Contains(tms.EnumerateArray(), m => m.GetProperty("machine").GetString() == "TM30"
            && m.GetProperty("moveName").GetString() == "Shadow Ball" && m.GetProperty("type").GetString() == "ghost");
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/dex/machines?game=zz")).StatusCode);
    }
}

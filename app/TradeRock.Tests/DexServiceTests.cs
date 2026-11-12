using PKHeX.Core;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using TradeRock.App;
using TradeRock.Core;
using TradeRock.Core.Dex;

namespace TradeRock.Tests;

/// <summary>Uma cópia do dex.sqlite embutido para a classe toda (são 25 MB).</summary>
public sealed class DexFixture : IDisposable
{
    public string Dir { get; } = Directory.CreateTempSubdirectory("traderock-dex-").FullName;
    public DexDatabase Db { get; }
    public DexService Service { get; }

    public DexFixture()
    {
        Db = new DexDatabase(new AppPaths(Dir));
        Service = new DexService(Db);
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        try { Directory.Delete(Dir, true); } catch (IOException) { }
    }
}

public sealed class DexServiceTests(DexFixture fx) : IClassFixture<DexFixture>
{
    private DexService Dex
    {
        get
        {
            Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido (rode scripts/build-dex.py).");
            return fx.Service;
        }
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Maquinas_do_FireRed_do_SwSh_e_do_Arceus()
    {
        var frlg = await Dex.MachinesAsync("frlg", Ct);
        Assert.Equal(58, frlg.Count);
        var tm30 = Assert.Single(frlg, m => m.Machine == "TM30");
        Assert.Equal("Shadow Ball", tm30.MoveName);
        Assert.Equal("ghost", tm30.Type);
        Assert.Contains(frlg, m => m.Machine == "HM01" && m.MoveIdentifier == "cut");
        Assert.Equal(frlg.Select(m => m.Number).Order(), frlg.Select(m => m.Number));

        var swsh = await Dex.MachinesAsync("swsh", Ct);
        Assert.Contains(swsh, m => m.Machine == "TR99" && m.MoveName == "Body Press");
        Assert.Empty(await Dex.MachinesAsync("pla", Ct));

        // Z-A: 107 do jogo base + 53 do Mega Dimension (DLC), sem números repetidos.
        var za = await Dex.MachinesAsync("za", Ct);
        Assert.Equal(160, za.Count);
        Assert.Contains(za, m => m.Machine == "TM01" && m.MoveIdentifier == "headbutt" && !m.Dlc);
        Assert.Contains(za, m => m.Machine == "TM108" && m.Dlc);
        Assert.Equal(53, za.Count(m => m.Dlc));
        // SV: as das DLCs já estão no grupo principal.
        Assert.DoesNotContain(await Dex.MachinesAsync("sv", Ct), m => m.Dlc);
    }

    [Fact]
    public async Task Chance_do_local_nao_soma_entre_sub_areas()
    {
        // Mt. Ember (500), Pokémon Tower (160) e Safari Zone (162) têm várias sub-áreas no PokeAPI.
        foreach (var location in new[] { 500, 160, 162 })
        {
            var rows = await Dex.LocationEncountersAsync(location, "frlg", Ct);
            Assert.NotEmpty(rows);
            Assert.All(rows, e => Assert.InRange(e.Chance, 0, 100));
        }
        var ember = await Dex.LocationEncountersAsync(500, "frlg", Ct);
        // Geodude quebrando pedra: 3 slots somam 95 em cada sub-área; a maior, não a soma das sub-áreas.
        Assert.All(ember.Where(e => e.Species == 74 && e.Method == "rock-smash"), e => Assert.Equal(95, e.Chance));
        var tower = await Dex.LocationEncountersAsync(160, "frlg", Ct);
        Assert.Contains(tower, e => e.Species == 92 && e.Chance is > 0 and <= 100);
    }

    [Fact]
    public async Task Chance_do_local_fica_com_a_maior_condicao()
    {
        // Wurmple na Eterna Forest (8): no Platinum, manhã 10 e dia 10 (alternativas, não 20); no Diamond, sem enxame 20,
        // manhã 10 e dia 10 (fica 20, não 40). Na árvore de mel, grupo A 40 e grupo B 5 (fica 40).
        var pt = await Dex.LocationEncountersAsync(8, "pt", Ct);
        Assert.Equal(10, Assert.Single(pt, e => e.Species == 265 && e.Method == "walk").Chance);
        var dp = await Dex.LocationEncountersAsync(8, "dp", Ct);
        Assert.All(dp.Where(e => e.Species == 265 && e.Method == "walk"), e => Assert.Equal(20, e.Chance));
        Assert.All(dp.Where(e => e.Species == 265 && e.Method == "honey-tree"), e => Assert.Equal(40, e.Chance));
        Assert.Contains(dp, e => e.Species == 265 && e.Method == "honey-tree");
    }

    [Fact]
    public async Task Gengar_no_FireRed()
    {
        var g = await Dex.GetAsync(94, "frlg", "pt-BR", Ct);
        Assert.NotNull(g);
        Assert.True(g.ExistsInGame);
        Assert.Equal(["ghost", "poison"], g.Types);
        Assert.Equal("Gengar", g.Name);
        Assert.Equal("Gengar", g.Names["en"]);
        var ability = Assert.Single(g.Abilities);
        Assert.Equal("Levitate", ability.Name);
        Assert.False(ability.IsHidden);

        Assert.Equal([92, 93, 94], g.EvolutionChain.Select(s => s.SpeciesId));
        var haunter = g.EvolutionChain.Single(s => s.SpeciesId == 93);
        Assert.Equal(92, haunter.FromSpeciesId);
        Assert.Equal("level 25", Assert.Single(haunter.Conditions));
        Assert.Equal("trade", Assert.Single(g.EvolutionChain.Single(s => s.SpeciesId == 94).Conditions));

        Assert.Equal(2, g.DamageTaken["ground"]);
        Assert.Equal(0, g.DamageTaken["normal"]);
        Assert.False(g.DamageTaken.ContainsKey("fairy")); // a Fada só existe a partir da Gen 6
        Assert.Equal(17, g.DamageTaken.Count);

        Assert.Contains(g.Moves.LevelUp, m => m.Name == "Shadow Punch" && m.Level == 25);
        Assert.Contains(g.Moves.Machine, m => m.Name == "Shadow Ball" && m.MachineNumber == 30);
        Assert.NotEmpty(g.Moves.Tutor);
        Assert.Equal(60, g.Stats.Hp);
        Assert.Equal(500, g.Stats.Total);
        Assert.Equal(3, g.Stats.Evs["special-attack"]);
        Assert.Equal(["firered", "leafgreen"], g.FlavorTexts.Select(f => f.Version));
        Assert.DoesNotContain(g.Forms, f => f.IsMega); // a Mega só chegou no X/Y
        Assert.Equal(94, g.DexNumbers["kanto"]);
        Assert.Empty(g.Encounters); // Gengar não aparece na natureza do FRLG
    }

    [Fact]
    public async Task Golpes_com_os_valores_da_geracao()
    {
        var tackle = await Dex.GetMoveAsync(33, "frlg", Ct);
        Assert.NotNull(tackle);
        Assert.Equal(35, tackle.Power);
        Assert.Equal(95, tackle.Accuracy);
        var now = await Dex.GetMoveAsync(33, "sv", Ct);
        Assert.Equal(40, now!.Power);
        var charm = await Dex.GetMoveAsync(204, "frlg", Ct);
        Assert.Equal("normal", charm!.Type);
        var punch = await Dex.GetMoveAsync(325, "frlg", Ct);
        Assert.Contains(punch!.Learners, l => l.SpeciesId == 94 && l.Method == "level-up");
    }

    [Fact]
    public async Task Gengar_existe_no_Lets_Go()
    {
        var g = await Dex.GetAsync(94, "lgpe", "en", Ct);
        Assert.True(g!.ExistsInGame);
        Assert.Contains(g.Forms, f => f.IsMega);
        Assert.NotEmpty(g.Moves.LevelUp);
        var gastly = await Dex.GetAsync(92, "lgpe", "en", Ct);
        Assert.Contains(gastly!.Encounters, e => e.Version is "lets-go-pikachu" or "lets-go-eevee");
    }

    [Fact]
    public async Task Sem_texto_no_jogo_vem_o_mais_recente_de_outro()
    {
        var g = await Dex.GetAsync(94, "sv", "pt-BR", Ct); // o dex.sqlite não tem texto do Gengar no SV
        var flavor = Assert.Single(g!.FlavorTexts);
        Assert.NotEmpty(flavor.Text);
        Assert.DoesNotContain(flavor.Version, new[] { "scarlet", "violet" });
    }

    [Fact]
    public async Task Sprigatito_nao_existe_no_FireRed()
    {
        var s = await Dex.GetAsync(906, "frlg", "pt-BR", Ct);
        Assert.NotNull(s);
        Assert.False(s.ExistsInGame);
        Assert.Empty(s.Moves.LevelUp);
        Assert.Empty(s.Moves.Machine);
        Assert.Empty(s.Encounters);
        Assert.Empty(s.FlavorTexts);
        Assert.Empty(s.Abilities);
    }

    [Fact]
    public async Task Especie_inexistente_da_null()
    {
        Assert.Null(await Dex.GetAsync(99999, "frlg", "en", Ct));
    }

    [Fact]
    public async Task Lista_filtra_por_texto_tipo_geracao_e_jogo()
    {
        var list = await Dex.ListAsync(new("geng", null, null, null), Ct);
        var gengar = Assert.Single(list);
        Assert.Equal(94, gengar.Id);
        Assert.Equal(94, gengar.NationalNumber);
        Assert.Equal(["ghost", "poison"], gengar.Types);

        Assert.Contains(await Dex.ListAsync(new("94", null, null, null), Ct), s => s.Id == 94);
        Assert.Contains(await Dex.ListAsync(new("flabebe", null, null, null), Ct), s => s.Id == 669);

        var ghosts = await Dex.ListAsync(new(null, "ghost", 1, null), Ct);
        Assert.Equal([92, 93, 94], ghosts.Select(s => s.Id));

        var frlg = await Dex.ListAsync(new(null, null, null, "frlg", 0, 2000), Ct);
        Assert.Contains(frlg, s => s.Id == 94);
        Assert.DoesNotContain(frlg, s => s.Id == 906);

        var page = await Dex.ListAsync(new(null, null, null, null, 10, 5), Ct);
        Assert.Equal([11, 12, 13, 14, 15], page.Select(s => s.Id));
    }

    [Fact]
    public async Task Lista_com_as_formas_regionais_logo_depois_da_especie()
    {
        // Sem pedir, a lista é só de espécies (os seletores do app contam com isso).
        Assert.DoesNotContain(await Dex.ListAsync(new(null, null, null, null, 0, 2000), Ct), s => s.FormId is not null);

        var all = await Dex.ListAsync(new(null, null, null, null, 0, 2000, RegionalForms: true), Ct);
        var raichu = all.Select((s, i) => (s, i)).Where(x => x.s.Id == 26).ToList();
        Assert.Equal(2, raichu.Count);
        Assert.Null(raichu[0].s.FormId);
        Assert.Equal(raichu[0].i + 1, raichu[1].i); // logo em seguida
        Assert.Equal((10100, "alola", 26), (raichu[1].s.FormId, raichu[1].s.Region, raichu[1].s.NationalNumber));
        Assert.Equal(["electric", "psychic"], raichu[1].s.Types);
        Assert.Equal(7, raichu[1].s.Generation);

        // Só as regionais: nem Mega, nem Gigantamax, nem Totem, nem boné do Pikachu, nem o Zen de Galar.
        var forms = all.Where(s => s.FormId is not null).ToList();
        Assert.Equal(57, forms.Count);
        Assert.All(forms, f => Assert.Contains(f.Region, new[] { "alola", "galar", "hisui", "paldea" }));
        Assert.DoesNotContain(forms, f => f.Id == 25);
        Assert.Equal(2, all.Count(s => s.Id == 20)); // Raticate e Raticate de Alola, sem o Totem
        Assert.Equal(["alola", "galar"], all.Where(s => s.Id == 52 && s.FormId is not null).Select(s => s.Region));
        Assert.Equal(["Combat Breed", "Blaze Breed", "Aqua Breed"], all.Where(s => s.Id == 128 && s.FormId is not null).Select(s => s.FormDetail));
        Assert.Single(all, s => s.Id == 555 && s.FormId is not null); // Darmanitan de Galar

        // O tipo vale por forma, a geração é a da região, e a busca acha pelo nome da espécie e pela região.
        var psychic = await Dex.ListAsync(new("raichu", "psychic", null, null, RegionalForms: true), Ct);
        Assert.Equal([10100], psychic.Select(s => s.FormId));
        var gen7 = await Dex.ListAsync(new(null, null, 7, null, 0, 2000, RegionalForms: true), Ct);
        Assert.Contains(gen7, s => s.FormId == 10100);
        Assert.DoesNotContain(gen7, s => s.Id == 26 && s.FormId is null);
        var gen1 = await Dex.ListAsync(new(null, null, 1, null, 0, 2000, RegionalForms: true), Ct);
        Assert.DoesNotContain(gen1, s => s.FormId is not null);
        var hisui = await Dex.ListAsync(new("hisui", null, null, null, 0, 2000, RegionalForms: true), Ct);
        Assert.Equal(16, hisui.Count);
        Assert.All(hisui, s => Assert.Equal("hisui", s.Region));
    }

    [Fact]
    public async Task Outras_formas_so_quando_pedidas_e_so_as_que_se_guardam()
    {
        var regionalOnly = await Dex.ListAsync(new(null, null, null, null, 0, 2000, RegionalForms: true), Ct);
        Assert.Equal(57, regionalOnly.Count(s => s.FormId is not null));

        var all = await Dex.ListAsync(new(null, null, null, null, 0, 2000, RegionalForms: true, OtherForms: true), Ct);
        var others = all.Where(s => s.FormId is not null && s.Region is null).ToList();
        Assert.Equal(["Heat Rotom", "Wash Rotom", "Frost Rotom", "Fan Rotom", "Mow Rotom"], others.Where(s => s.Id == 479).Select(s => s.FormDetail));
        Assert.Equal(3, others.Count(s => s.Id == 386)); // Deoxys
        Assert.Equal(["Midnight Form", "Dusk Form"], others.Where(s => s.Id == 745).Select(s => s.FormDetail));
        Assert.All(others, s => Assert.False(string.IsNullOrWhiteSpace(s.FormDetail)));
        // Fora: Mega e Gigantamax (Charizard), Totem (Raticate só tem a de Alola), só de batalha (Darmanitan Zen,
        // Aegislash Blade), fusões (Kyurem, Necrozma, Calyrex) e as que dependem de item (Giratina, Ogerpon).
        foreach (var species in new[] { 6, 20, 555, 681, 646, 800, 898, 487, 1017, 1007, 1008, 890 })
            Assert.DoesNotContain(others, s => s.Id == species);
        Assert.Equal(7, others.Count(s => s.Id == 774)); // Minior: as sete cores, sem as formas com a casca
        // Só as outras, sem as regionais.
        var othersOnly = await Dex.ListAsync(new(null, null, null, null, 0, 2000, OtherForms: true), Ct);
        Assert.Equal(others.Count, othersOnly.Count(s => s.FormId is not null));
        Assert.DoesNotContain(othersOnly, s => s.Region is not null);
        // No jogo, a forma só aparece a partir de quando existe; a geração é a da espécie.
        var frlg = await Dex.ListAsync(new(null, null, null, "frlg", 0, 2000, OtherForms: true), Ct);
        Assert.Equal(3, frlg.Count(s => s.Id == 386 && s.FormId is not null));
        Assert.DoesNotContain(frlg, s => s.Id == 25 && s.FormId is not null);
        var gen4 = await Dex.ListAsync(new(null, null, 4, null, 0, 2000, OtherForms: true), Ct);
        Assert.Equal(5, gen4.Count(s => s.Id == 479 && s.FormId is not null));
    }

    [Fact]
    public void Pokemon_guardado_cai_na_linha_certa_da_lista()
    {
        Assert.Equal(0, Dex.ListFormId(26, 0, EntityContext.Gen8));
        Assert.Equal(10100, Dex.ListFormId(26, 1, EntityContext.Gen8)); // Raichu de Alola
        Assert.Equal(10100, Dex.ListFormId(26, 1, EntityContext.Gen9a)); // no Z-A, os Mega vêm depois da forma de Alola
        Assert.Equal(10107, Dex.ListFormId(52, 1, EntityContext.Gen8)); // Meowth de Alola
        Assert.Equal(10161, Dex.ListFormId(52, 2, EntityContext.Gen8)); // Meowth de Galar
        Assert.Equal(10165, Dex.ListFormId(80, 2, EntityContext.Gen8)); // Slowbro de Galar (1 é o Mega)
        Assert.Equal(0, Dex.ListFormId(80, 1, EntityContext.Gen8));
        Assert.Equal([10250, 10251, 10252], new[] { 1, 2, 3 }.Select(f => Dex.ListFormId(128, f, EntityContext.Gen9)));
        Assert.Equal(10177, Dex.ListFormId(555, 2, EntityContext.Gen8)); // Darmanitan de Galar
        Assert.Equal(DexService.CosmeticFormBase + 10005, Dex.ListFormId(201, 5, EntityContext.Gen3)); // Unown F
        Assert.Equal(0, Dex.ListFormId(6, 1, EntityContext.Gen6)); // Mega Charizard X também
    }

    [Fact]
    public async Task Outras_formas_guardadas_caem_na_linha_delas()
    {
        // Cada "outra forma" da lista é achada pelo nome da forma no PKHeX, em algum formato do cofre; e o índice de
        // forma do PKHeX de uma delas não cai em outra.
        var others = (await Dex.ListAsync(new(null, null, null, null, 0, 2000, OtherForms: true), Ct)).Where(s => s.FormId is not null).ToList();
        EntityContext[] contexts =
        [
            EntityContext.Gen9, EntityContext.Gen9a, EntityContext.Gen8, EntityContext.Gen8a, EntityContext.Gen7, EntityContext.Gen6,
            EntityContext.Gen5, EntityContext.Gen4,
        ];
        var notFound = new List<string>();
        foreach (var form in others)
        {
            var found = contexts.Any(c => Enumerable.Range(0, 40).Any(i => Dex.ListFormId(form.Id, i, c) == form.FormId));
            if (!found)
                notFound.Add($"{form.Name} ({form.FormDetail})");
        }
        Assert.Empty(notFound);
        // A forma 0 do PKHeX é a espécie; só no Maushold ela é uma "outra forma" (a família de três; a de quatro é a espécie).
        var zero = others.Select(o => o.Id).Distinct().Where(id => contexts.Any(c => Dex.ListFormId(id, 0, c) != 0)).ToList();
        Assert.Equal([925], zero);
        Assert.Equal(0, Dex.ListFormId(925, 1, EntityContext.Gen9));
        Assert.Equal(others.Single(s => s.Id == 774 && s.FormDetail == "Red Core").FormId, Dex.ListFormId(774, 7, EntityContext.Gen7));
        Assert.Equal(0, Dex.ListFormId(774, 0, EntityContext.Gen7)); // com a casca: conta como Minior
        Assert.Equal(10025, Dex.ListFormId(678, 1, EntityContext.Gen9)); // Meowstic fêmea (na caixa das fêmeas)

        Assert.Equal(others.Single(s => s.Id == 479 && s.FormDetail == "Wash Rotom").FormId, Dex.ListFormId(479, 2, EntityContext.Gen8));
        Assert.Equal(others.Single(s => s.Id == 386 && s.FormDetail == "Attack Forme").FormId, Dex.ListFormId(386, 1, EntityContext.Gen9));
        Assert.Equal(others.Single(s => s.Id == 745 && s.FormDetail == "Dusk Form").FormId, Dex.ListFormId(745, 2, EntityContext.Gen9));
    }

    [Fact]
    public async Task Formas_cosmeticas_e_femeas_so_quando_pedidas()
    {
        async Task<List<SpeciesSummary>> FormsAsync(DexQuery q) => [.. (await Dex.ListAsync(q, Ct)).Where(s => s.FormId is not null)];

        // Unown, Vivillon e Alcremie numa caixa só deles.
        var many = await FormsAsync(new(null, null, null, null, 0, 5000, ManyForms: true));
        Assert.Equal((27, 19, 62), (many.Count(s => s.Id == 201), many.Count(s => s.Id == 666), many.Count(s => s.Id == 869)));
        Assert.Equal(108, many.Count);
        Assert.Equal("B", many.First(s => s.Id == 201).FormDetail);
        Assert.All(many, s => Assert.InRange(s.FormId!.Value, DexService.CosmeticFormBase, DexService.FemaleFormBase - 1));

        // A fêmea diferente: as espécies com diferença de gênero e as que já têm a fêmea como forma.
        var gender = await FormsAsync(new(null, null, null, null, 0, 5000, GenderForms: true));
        Assert.All(gender, s => Assert.Equal("♀", s.FormDetail));
        Assert.Contains(gender, s => s.Id == 3 && s.FormId == DexService.FemaleFormBase + 3); // Venusaur
        Assert.Contains(gender, s => s.Id == 25);
        Assert.Single(gender, s => s.Id == 678); // Meowstic: a forma fêmea que o PokeAPI já tem
        Assert.Single(gender, s => s.Id == 668); // Pyroar
        Assert.Equal(gender.Count, gender.Select(s => s.Id).Distinct().Count());
        Assert.DoesNotContain(gender, s => s.Id == 1);
        // Só desde a Gen 4.
        Assert.Empty(await FormsAsync(new(null, null, null, "frlg", 0, 5000, GenderForms: true)));
        Assert.Contains(await FormsAsync(new(null, null, null, "dp", 0, 5000, GenderForms: true)), s => s.Id == 3);

        // As outras cosméticas entram em "outras formas"; as de item, só de batalha e sem nome ficam de fora.
        var others = await FormsAsync(new(null, null, null, null, 0, 5000, OtherForms: true));
        Assert.Equal(["Summer Form", "Autumn Form", "Winter Form"], others.Where(s => s.Id == 585).Select(s => s.FormDetail));
        Assert.Equal(9, others.Count(s => s.Id == 676)); // Furfrou
        Assert.DoesNotContain(others, s => s.FormDetail == "♀");
        foreach (var species in new[] { 201, 666, 869, 493, 773, 649, 421, 664, 665, 414 })
            Assert.DoesNotContain(others, s => s.Id == species);

        // Nenhum id repetido entre as caixas.
        var all = await FormsAsync(new(null, null, null, null, 0, 5000, true, true, true, true));
        Assert.Equal(all.Count, all.Select(s => s.FormId).Distinct().Count());
        Assert.Equal(57 + others.Count + many.Count + gender.Count, all.Count);
    }

    [Fact]
    public async Task Pagina_da_especie_mostra_as_formas_cosmeticas_e_a_femea()
    {
        var unown = (await Dex.GetAsync(201, "sv", "pt-BR", Ct))!;
        Assert.Equal(28, unown.Forms.Count);
        Assert.All(unown.Forms.Skip(1), f => Assert.True(f.Listed && f.Id > DexService.CosmeticFormBase));
        Assert.Equal(["B", "C"], unown.Forms.Skip(1).Take(2).Select(f => f.Name));

        var venusaur = (await Dex.GetAsync(3, "sv", "pt-BR", Ct))!;
        Assert.Contains(venusaur.Forms, f => f.Id == DexService.FemaleFormBase + 3 && f.Name == "♀" && f.Listed);
        Assert.Contains(venusaur.Forms, f => f.IsMega && !f.Listed); // Mega só aqui dentro, sem marcar
        Assert.DoesNotContain((await Dex.GetAsync(3, "frlg", "pt-BR", Ct))!.Forms, f => f.Name == "♀"); // antes da Gen 4 não havia

        // As de item aparecem para consulta, com o tipo, mas não entram na coleção.
        var arceus = (await Dex.GetAsync(493, "sv", "pt-BR", Ct))!;
        var fire = arceus.Forms.Single(f => f.Identifier == "arceus-fire");
        Assert.Equal(["fire"], fire.Types);
        Assert.False(fire.Listed);
    }

    [Fact]
    public void Formas_cosmeticas_e_femeas_guardadas_caem_na_linha_delas()
    {
        Assert.Equal(DexService.CosmeticFormBase + 10001, Dex.ListFormId(201, 1, EntityContext.Gen3)); // Unown B
        Assert.NotEqual(0, Dex.ListFormId(201, 26, EntityContext.Gen4)); // !
        Assert.NotEqual(Dex.ListFormId(201, 26, EntityContext.Gen4), Dex.ListFormId(201, 27, EntityContext.Gen4)); // ?
        Assert.Equal(0, Dex.ListFormId(201, 0, EntityContext.Gen3)); // Unown A é a espécie
        Assert.NotEqual(0, Dex.ListFormId(666, 0, EntityContext.Gen9)); // Vivillon Icy Snow: a forma 0 do PKHeX
        Assert.Equal(0, Dex.ListFormId(666, 6, EntityContext.Gen9)); // Meadow é a espécie na lista
        Assert.NotEqual(0, Dex.ListFormId(666, 19, EntityContext.Gen9)); // Poké Ball
        Assert.NotEqual(0, Dex.ListFormId(585, 1, EntityContext.Gen8)); // Deerling Summer
        // Alcremie: o creme vem da forma; o confeito não está no resumo do cofre (conta como o de morango).
        Assert.Equal(0, Dex.ListFormId(869, 0, EntityContext.Gen8));
        Assert.NotEqual(0, Dex.ListFormId(869, 1, EntityContext.Gen8));

        // Fêmea: só quando a forma é a da espécie e a espécie tem a fêmea diferente.
        Assert.Equal(DexService.FemaleFormBase + 3, Dex.ListFormId(3, 0, EntityContext.Gen8, gender: 1));
        Assert.Equal(0, Dex.ListFormId(3, 0, EntityContext.Gen8, gender: 0));
        Assert.Equal(0, Dex.ListFormId(1, 0, EntityContext.Gen8, gender: 1)); // Bulbasaur não tem diferença
        Assert.Equal(10100, Dex.ListFormId(26, 1, EntityContext.Gen8, gender: 1)); // Raichu de Alola fêmea é Raichu de Alola
        Assert.NotEqual(0, Dex.ListFormId(668, 0, EntityContext.Gen9, gender: 1)); // Pyroar fêmea
        Assert.True(Dex.IsListed(3, DexService.FemaleFormBase + 3));
        Assert.True(Dex.IsListed(201, DexService.CosmeticFormBase + 10001));
    }

    [Fact]
    public void Formas_sem_ordem_no_pokeapi_vao_para_o_fim()
    {
        // O índice de forma dos nossos encontros e treinadores conta nesta ordem: as Mega do Z-A, as Gigantamax e o
        // Basculin de listra branca não têm ordem no PokeAPI e entravam na frente, deslocando as outras.
        Assert.Equal(["raichu", "raichu-alola"], Dex.PokemonForms(26).Take(2));
        Assert.Equal(["meowth", "meowth-alola", "meowth-galar"], Dex.PokemonForms(52).Take(3));
        Assert.Equal(["basculin-red-striped", "basculin-blue-striped", "basculin-white-striped"], Dex.PokemonForms(550));
        Assert.Equal(["charizard", "charizard-mega-x", "charizard-mega-y"], Dex.PokemonForms(6).Take(3));
        Assert.Equal(10100, Dex.FormPokemonId(26, 1));
        Assert.Equal(10161, Dex.FormPokemonId(52, 2));
        Assert.True(Dex.IsListed(26, 10100));
        Assert.False(Dex.IsListed(26, 10107));
        Assert.False(Dex.IsListed(99999, 0));
    }

    [Fact]
    public async Task Formas_regionais_so_nos_jogos_em_que_existem()
    {
        async Task<List<SpeciesSummary>> FormsAsync(string game) =>
            [.. (await Dex.ListAsync(new(null, null, null, game, 0, 2000, RegionalForms: true), Ct)).Where(s => s.FormId is not null)];

        foreach (var old in new[] { "rb", "frlg", "hgss", "bw", "oras", "bdsp" })
            Assert.Empty(await FormsAsync(old));

        var sm = await FormsAsync("sm");
        Assert.Equal(18, sm.Count);
        Assert.All(sm, s => Assert.Equal("alola", s.Region));
        Assert.Equal(18, (await FormsAsync("lgpe")).Count);

        // As contagens batem com os golpes por forma do PokeAPI nos jogos em que ele tem os dados.
        var swsh = await FormsAsync("swsh");
        Assert.Equal((11, 19), (swsh.Count(s => s.Region == "alola"), swsh.Count(s => s.Region == "galar")));
        Assert.Equal(18, (await FormsAsync("pla")).Count);
        Assert.Equal(43, (await FormsAsync("sv")).Count);
        Assert.Contains(swsh, s => s.FormId == 10100); // Raichu de Alola
        Assert.Contains(swsh, s => s.Id == 52 && s.Region == "galar");
        Assert.DoesNotContain(swsh, s => s.Region is "hisui" or "paldea");

        var pla = await FormsAsync("pla");
        Assert.Contains(pla, s => s.Id == 157 && s.Region == "hisui");
        Assert.DoesNotContain(pla, s => s.Region is "galar" or "paldea");

        var sv = await FormsAsync("sv");
        Assert.Equal(3, sv.Count(s => s.Id == 128));
        Assert.Contains(sv, s => s.Id == 194 && s.Region == "paldea");
    }

    [Fact]
    public async Task Lista_e_rapida()
    {
        await Dex.ListAsync(new(null, null, null, "frlg"), Ct); // aquece os caches
        var watch = Stopwatch.StartNew();
        await Dex.ListAsync(new("a", "fire", null, "sv", 0, 100), Ct);
        Assert.True(watch.ElapsedMilliseconds < 100, $"{watch.ElapsedMilliseconds} ms");
    }

    [Fact]
    public async Task Disponibilidade_por_jogo()
    {
        var all = await Dex.AvailabilityAsync(94, null, Ct);
        Assert.Equal(22, all.Count);
        Assert.Equal(AvailabilityKind.Evolution, all.Single(a => a.GameKey == "frlg").Kind);
        Assert.Equal(AvailabilityKind.None, (await Dex.AvailabilityAsync(906, null, Ct)).Single(a => a.GameKey == "frlg").Kind);

        var gastly = await Dex.AvailabilityAsync(92, null, Ct);
        Assert.Equal(AvailabilityKind.Encounter, gastly.Single(a => a.GameKey == "frlg").Kind);

        var pichu = await Dex.AvailabilityAsync(172, null, Ct);
        Assert.Equal(AvailabilityKind.None, pichu.Single(a => a.GameKey == "rb").Kind);
        Assert.Equal(AvailabilityKind.Egg, pichu.Single(a => a.GameKey == "frlg").Kind);
    }

    [Fact]
    public async Task Tabela_de_tipos_por_geracao()
    {
        var gen9 = await Dex.GetTypeChartAsync(9, Ct);
        Assert.Equal(18, gen9.Types.Count);
        Assert.Equal(2, gen9.Factors["fairy"]["dragon"]);
        var gen1 = await Dex.GetTypeChartAsync(1, Ct);
        Assert.Equal(15, gen1.Types.Count);
        Assert.DoesNotContain("steel", gen1.Types);
    }

    [Fact]
    public async Task Tabela_antiga_da_Gen_1()
    {
        var gen1 = (await Dex.GetTypeChartAsync(1, Ct)).Factors;
        Assert.Equal(0, gen1["ghost"]["psychic"]); // o bug da Gen 1
        Assert.Equal(2, gen1["bug"]["poison"]);
        Assert.Equal(2, gen1["poison"]["bug"]);
        Assert.Equal(1, gen1["ice"]["fire"]);
        var gen2 = (await Dex.GetTypeChartAsync(2, Ct)).Factors;
        Assert.Equal(2, gen2["ghost"]["psychic"]);
        Assert.Equal(0.5, gen2["bug"]["poison"]);
        Assert.Equal(0.5, gen2["ice"]["fire"]);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task Aco_resiste_a_Fantasma_e_Noturno_ate_a_Gen_5(int generation)
    {
        var chart = (await Dex.GetTypeChartAsync(generation, Ct)).Factors;
        Assert.Equal(0.5, chart["ghost"]["steel"]);
        Assert.Equal(0.5, chart["dark"]["steel"]);
        var gen6 = (await Dex.GetTypeChartAsync(6, Ct)).Factors;
        Assert.Equal(1, gen6["ghost"]["steel"]);
        Assert.Equal(1, gen6["dark"]["steel"]);
    }

    [Theory]
    [InlineData("gs", 0.5)]
    [InlineData("frlg", 0.5)]
    [InlineData("bw", 0.5)]
    [InlineData("sv", 1)]
    public async Task Steelix_contra_Fantasma(string game, double factor)
    {
        var steelix = await Dex.GetAsync(208, game, "en", Ct);
        Assert.Equal(factor, steelix!.DamageTaken["ghost"]);
        Assert.Equal(factor, steelix.DamageTaken["dark"]);
    }

    [Fact]
    public async Task Tipos_antigos_dos_Pokemon()
    {
        var clefairy = await Dex.GetAsync(35, "frlg", "en", Ct);
        Assert.Equal(["normal"], clefairy!.Types);
        Assert.Equal(["normal"], clefairy.Forms.Single(f => f.IsDefault).Types);
        Assert.Equal(2, clefairy.DamageTaken["fighting"]);
        Assert.Equal(0, clefairy.DamageTaken["ghost"]);
        Assert.Equal(["fairy"], (await Dex.GetAsync(35, "sv", "en", Ct))!.Types);

        Assert.Equal(["electric"], (await Dex.GetAsync(81, "rb", "en", Ct))!.Types);
        Assert.Equal(["electric", "steel"], (await Dex.GetAsync(81, "gs", "en", Ct))!.Types);

        var frlg = await Dex.ListAsync(new("clefairy", null, null, "frlg"), Ct);
        Assert.Equal(["normal"], Assert.Single(frlg).Types);
        Assert.Contains(await Dex.ListAsync(new(null, "normal", 1, "frlg", 0, 2000), Ct), s => s.Id == 35);
        Assert.Equal(["fairy"], Assert.Single(await Dex.ListAsync(new("clefairy", null, null, null), Ct)).Types);
    }

    [Fact]
    public async Task Pagina_da_especie_e_golpe_sao_rapidos()
    {
        await Dex.GetAsync(94, "sv", "en", Ct); // aquece os caches
        var watch = Stopwatch.StartNew();
        await Dex.GetAsync(94, "sv", "en", Ct);
        Assert.True(watch.ElapsedMilliseconds < 50, $"espécie: {watch.ElapsedMilliseconds} ms");
        await Dex.GetMoveAsync(33, "sv", Ct);
        watch.Restart();
        await Dex.GetMoveAsync(33, "sv", Ct);
        Assert.True(watch.ElapsedMilliseconds < 200, $"golpe: {watch.ElapsedMilliseconds} ms");
    }

    [Fact]
    public async Task Habilidade_e_item()
    {
        var levitate = await Dex.GetAbilityAsync(26, Ct);
        Assert.Equal("Levitate", levitate!.Name);
        Assert.Contains(levitate.Pokemon, p => p.SpeciesId == 92);
        var ball = await Dex.GetItemAsync(1, Ct);
        Assert.Equal("Master Ball", ball!.Name);
        Assert.Null(await Dex.GetItemAsync(999999, Ct));
    }

    [Fact]
    public void Copia_fica_com_o_hash_e_apaga_as_antigas()
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido.");
        var dir = Directory.CreateTempSubdirectory("traderock-dexcopy-").FullName;
        try
        {
            var folder = Path.Combine(dir, "dex");
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "00000000.sqlite"), "velho");
            var db = new DexDatabase(new AppPaths(dir));
            using (var c = db.Open())
            using (var cmd = c.CreateCommand())
            {
                cmd.CommandText = "SELECT count(*) FROM species";
                Assert.Equal(1025L, cmd.ExecuteScalar());
            }
            var file = Assert.Single(Directory.GetFiles(folder, "*.sqlite"));
            Assert.Matches("^[0-9a-f]{8}\\.sqlite$", Path.GetFileName(file));
            Assert.Equal(Path.GetFullPath(file), db.FilePath);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            try { Directory.Delete(dir, true); } catch (IOException) { }
        }
    }
}

public sealed class DexApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-dexapi-").FullName;
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
    public async Task Colecao_marca_por_visao_e_le_o_cofre_na_hora()
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido.");
        async Task<(List<(int, int, bool)> Marks, List<(int, int, bool)> Vault)> GetAsync(string scope)
        {
            var c = await _client.GetFromJsonAsync<JsonElement>($"/api/dex/collection?scope={scope}");
            List<(int, int, bool)> Read(string name) => [.. c.GetProperty(name).EnumerateArray()
                .Select(e => (e.GetProperty("speciesId").GetInt32(), e.GetProperty("formId").GetInt32(), e.GetProperty("shiny").GetBoolean()))];
            return (Read("marks"), Read("vault"));
        }
        Task<HttpResponseMessage> PutAsync(string? scope, int species, int form, bool shiny, bool owned) =>
            _client.PutAsJsonAsync("/api/dex/collection", new { scope, speciesId = species, formId = form, shiny, owned });

        Assert.Empty((await GetAsync("")).Marks);
        Assert.Equal(HttpStatusCode.NoContent, (await PutAsync(null, 26, 0, false, true)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await PutAsync("", 26, 10100, true, true)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await PutAsync("", 26, 10100, true, true)).StatusCode); // repetir não duplica
        Assert.Equal(HttpStatusCode.NoContent, (await PutAsync("swsh", 25, 0, true, true)).StatusCode);
        Assert.Equal([(26, 0, false), (26, 10100, true)], (await GetAsync("")).Marks);
        Assert.Equal([(25, 0, true)], (await GetAsync("swsh")).Marks); // cada visão tem as suas
        Assert.Empty((await GetAsync("sv")).Marks);

        Assert.Equal(HttpStatusCode.NoContent, (await PutAsync("", 26, 0, false, false)).StatusCode);
        Assert.Equal([(26, 10100, true)], (await GetAsync("")).Marks);

        Assert.Equal(HttpStatusCode.NotFound, (await PutAsync("", 26, 10107, false, true)).StatusCode); // forma de outra espécie
        Assert.Equal(HttpStatusCode.NotFound, (await PutAsync("", 99999, 0, false, true)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await PutAsync("nada", 26, 0, false, true)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/dex/collection?scope=nada")).StatusCode);

        // O cofre é lido na hora: um Raichu de Alola shiny do Sword aparece na geral e na do Sword/Shield, não na do Scarlet.
        var before = await GetAsync("");
        Assert.DoesNotContain((26, 10100, true), before.Vault);
        var generated = await _client.PostAsJsonAsync("/api/builder/generate", new
        {
            spec = new { species = 26, form = 1, level = 40, shiny = true, game = "sword", trainer = new { name = "ROCHA", tid = 123456, sid = 1234 } },
        });
        var body = await generated.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.String, body.ToString());
        (await _client.PostAsJsonAsync("/api/builder/save", new { data = data.GetString(), game = "sword" })).EnsureSuccessStatusCode();
        Assert.Contains((26, 10100, true), (await GetAsync("")).Vault);
        Assert.Contains((26, 10100, true), (await GetAsync("swsh")).Vault);
        Assert.DoesNotContain((26, 10100, true), (await GetAsync("sv")).Vault);
        Assert.Equal([(26, 10100, true)], (await GetAsync("")).Marks); // o cofre não vira marcação
    }

    [Fact]
    public async Task Rotas_da_Pokedex()
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido.");
        var games = await _client.GetFromJsonAsync<JsonElement>("/api/dex/games");
        Assert.Equal("kanto", games.EnumerateArray().First(g => g.GetProperty("key").GetString() == "frlg")
            .GetProperty("region").GetString());

        var list = await _client.GetFromJsonAsync<JsonElement>("/api/dex/species?text=geng");
        Assert.Equal(94, list[0].GetProperty("id").GetInt32());

        // Sprites das formas cosméticas e das fêmeas: arquivos próprios, diferentes do da espécie.
        async Task<byte[]> SpriteAsync(string query) => await _client.GetByteArrayAsync($"/api/sprites/{query}");
        Assert.NotEqual(await SpriteAsync("201"), await SpriteAsync($"201?form={DexService.CosmeticFormBase + 10001}"));
        Assert.NotEqual(await SpriteAsync("3"), await SpriteAsync($"3?form={DexService.FemaleFormBase + 3}"));
        Assert.NotEqual(await SpriteAsync($"3?form={DexService.FemaleFormBase + 3}"), await SpriteAsync($"3?form={DexService.FemaleFormBase + 3}&shiny=1"));
        var manyForms = await _client.GetFromJsonAsync<JsonElement>("/api/dex/species?text=unown&manyForms=true");
        Assert.Equal(28, manyForms.GetArrayLength());
        Assert.Equal(2, (await _client.GetFromJsonAsync<JsonElement>("/api/dex/species?text=venusaur&genderForms=true")).GetArrayLength());

        // As formas regionais só entram com ?forms=true (a tela da Pokédex); os seletores recebem só espécies.
        Assert.Equal(1, (await _client.GetFromJsonAsync<JsonElement>("/api/dex/species?text=raichu")).GetArrayLength());
        var withForms = await _client.GetFromJsonAsync<JsonElement>("/api/dex/species?text=raichu&forms=true");
        Assert.Equal(2, withForms.GetArrayLength());
        Assert.Equal((10100, "alola"), (withForms[1].GetProperty("formId").GetInt32(), withForms[1].GetProperty("region").GetString()));

        var gengar = await _client.GetFromJsonAsync<JsonElement>("/api/dex/species/94?game=frlg&lang=pt-BR");
        Assert.True(gengar.GetProperty("existsInGame").GetBoolean());

        var availability = await _client.GetFromJsonAsync<JsonElement>("/api/dex/species/94/availability");
        Assert.Equal("evolution", availability.EnumerateArray().First(a => a.GetProperty("gameKey").GetString() == "frlg")
            .GetProperty("kind").GetString());

        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/dex/moves/33?game=frlg")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/dex/abilities/26")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/dex/items/1")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/dex/types?generation=3")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/api/dex/species/99999?game=frlg")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/dex/species/94?game=nada")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/dex/types?generation=10")).StatusCode);
    }
}

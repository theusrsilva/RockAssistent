using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging.Abstractions;
using TradeRock.App;
using TradeRock.Core;
using TradeRock.Core.Dex;
using TradeRock.Core.GameData;
using TradeRock.Core.Games;
using TradeRock.Core.Nuzlocke;
using TradeRock.Core.Storage;

namespace TradeRock.Tests;

public sealed class NuzlockeStoreTests(DexFixture fx) : IClassFixture<DexFixture>, IAsyncLifetime
{
    private readonly TestDataDir _dir = new();
    private IDbContextFactory<TradeRockDb> _db = null!;
    private GameDataStore _data = null!;
    private NuzlockeStore _store = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido (rode scripts/build-dex.py).");
        _db = await _dir.CreateDbFactoryAsync();
        _data = new GameDataStore(fx.Service, NullLogger<GameDataStore>.Instance);
        _store = new NuzlockeStore(_db, _data, fx.Service, TimeProvider.System);
    }

    public ValueTask DisposeAsync()
    {
        SqliteConnection.ClearAllPools();
        _dir.Dispose();
        return ValueTask.CompletedTask;
    }

    private Task<RunView> NewFrlg(NuzlockeRules? rules = null) => _store.CreateAsync(new NewRun("Kanto", "frlg", "firered", rules), Ct);

    private static async Task<string> Fails(Func<Task> action) =>
        (await Assert.ThrowsAsync<NuzlockeException>(action)).Code;

    [Fact]
    public async Task Run_no_frlg_comeca_na_primeira_area_com_o_primeiro_chefe()
    {
        var view = await NewFrlg();
        Assert.Equal("pallet-town", view.CurrentArea!.Id);
        var first = _data.Bosses("frlg").MinBy(b => b.Order)!;
        Assert.Equal(first.Id, view.NextBoss!.Id);
        Assert.Equal(_data.Bosses("frlg")[0].LevelCap, view.LevelCap);
        Assert.True(view.TrainersAvailable);
        Assert.Equal(NuzlockeRules.Default, view.Run.Rules);
        Assert.Equal(["pallet-town"], view.VisitedAreas);

        // Vencendo os chefes em ordem, o próximo é o seguinte (Brock vem depois dos dois primeiros do rival).
        var brock = _data.Bosses("frlg").Single(b => b.Id == "brock");
        foreach (var boss in _data.Bosses("frlg").Where(b => b.Order < brock.Order))
            view = await _store.DefeatBossAsync(view.Run.Id, boss.Id, Ct);
        Assert.Equal("brock", view.NextBoss!.Id);
        Assert.Equal(brock.LevelCap, view.LevelCap);
        Assert.Equal("unknown_boss", await Fails(() => _store.DefeatBossAsync(view.Run.Id, "giovanni-xyz", Ct)));
    }

    [Fact]
    public async Task Area_mostra_encontros_do_pokeapi_e_treinadores()
    {
        var run = await NewFrlg();
        var view = await _store.SetAreaAsync(run.Run.Id, "route-1", Ct);
        Assert.Equal("route-1", view.CurrentArea!.Id);
        var pidgey = Assert.Single(view.AreaEncounters, e => e.Species == 16);
        Assert.Equal("Pidgey", pidgey.Name);
        Assert.Contains("walk", pidgey.Methods);
        Assert.Equal(["firered"], pidgey.Versions);
        Assert.Contains("route-1", view.VisitedAreas);

        var viridian = await _store.SetAreaAsync(run.Run.Id, "viridian-city", Ct);
        Assert.Equal(_data.Trainers("frlg").Count(t => t.Area == "viridian-city"), viridian.AreaTrainers.Count);
        Assert.Equal("unknown_area", await Fails(() => _store.SetAreaAsync(run.Run.Id, "cabo-poco", Ct)));
    }

    [Fact]
    public async Task Versao_escolhida_filtra_os_encontros()
    {
        Assert.Equal("version_required", await Fails(() => _store.CreateAsync(new NewRun(null, "frlg", null, null), Ct)));
        Assert.Equal("version_required", await Fails(() => _store.CreateAsync(new NewRun(null, "swsh", " ", null), Ct)));
        // Jogo de uma versão só não precisa escolher.
        Assert.Null((await _store.CreateAsync(new NewRun(null, "pla", null, null), Ct)).Run.Version);
        var run = await _store.CreateAsync(new NewRun(null, "frlg", "leafgreen", null), Ct);
        Assert.Equal("FireRed / LeafGreen", run.Run.Name);
        var view = await _store.SetAreaAsync(run.Run.Id, "route-1", Ct);
        Assert.All(view.AreaEncounters, e => Assert.Equal(["leafgreen"], e.Versions));
        Assert.Equal("unknown_version", await Fails(() => _store.CreateAsync(new NewRun(null, "frlg", "ruby", null), Ct)));
        Assert.Equal("unknown_game", await Fails(() => _store.CreateAsync(new NewRun(null, "zz", null, null), Ct)));
    }

    [Fact]
    public async Task Segundo_encontro_na_mesma_area_e_recusado()
    {
        var run = await NewFrlg();
        var id = run.Run.Id;
        await _store.EncounterAsync(id, new NewEncounter("route-1", 16, 0, "Pidge", 3, NuzlockeStatus.Party), Ct);
        Assert.Equal("area_already_used",
            await Fails(() => _store.EncounterAsync(id, new NewEncounter("route-1", 19, 0, "Rat", 3, NuzlockeStatus.Box), Ct)));
        // Registrar o que fugiu ainda dá; e o shiny passa com a cláusula de shiny.
        await _store.EncounterAsync(id, new NewEncounter("route-1", 19, 0, null, 3, NuzlockeStatus.Missed), Ct);
        var shinyRun = await NewFrlg(NuzlockeRules.Default with { ShinyClause = true });
        await _store.EncounterAsync(shinyRun.Run.Id, new NewEncounter("route-1", 16, 0, "A", 3, NuzlockeStatus.Party), Ct);
        var view = await _store.EncounterAsync(shinyRun.Run.Id, new NewEncounter("route-1", 19, 0, "B", 3, NuzlockeStatus.Party, Shiny: true), Ct);
        Assert.Equal(2, view.Party.Count);
        Assert.True(view.Party.Single(m => m.Species == 19).Shiny);
        Assert.False(view.Party.Single(m => m.Species == 16).Shiny);

        // Encontro perdido (fugiu) também usa a área.
        await _store.EncounterAsync(id, new NewEncounter("route-22", 0, 0, null, 0, NuzlockeStatus.Failed), Ct);
        Assert.Equal("area_already_used",
            await Fails(() => _store.EncounterAsync(id, new NewEncounter("route-22", 56, 0, "Mankey", 3, NuzlockeStatus.Party), Ct)));
    }

    [Fact]
    public async Task Clausula_de_especie_e_de_linha()
    {
        var run = await NewFrlg(NuzlockeRules.Default with { SpeciesClause = true, DupesClause = true });
        var id = run.Run.Id;
        await _store.EncounterAsync(id, new NewEncounter("route-1", 16, 0, "Pidge", 3, NuzlockeStatus.Party), Ct);
        // Pidgeotto (17) é da mesma linha do Pidgey.
        Assert.Equal("dupe", await Fails(() => _store.EncounterAsync(id, new NewEncounter("route-2", 17, 0, "Otto", 9, NuzlockeStatus.Party), Ct)));
        var view = await _store.EncounterAsync(id, new NewEncounter("route-2", 17, 0, null, 9, NuzlockeStatus.Missed), Ct);
        Assert.Single(view.Lost);
        // "missed" não usa a área: dá para capturar outro ali.
        view = await _store.EncounterAsync(id, new NewEncounter("route-2", 10, 0, "Cat", 4, NuzlockeStatus.Box), Ct);
        Assert.Single(view.Box);

        // Só a cláusula de espécie: a linha não conta.
        var species = await NewFrlg(NuzlockeRules.Default with { SpeciesClause = true });
        await _store.EncounterAsync(species.Run.Id, new NewEncounter("route-1", 16, 0, "Pidge", 3, NuzlockeStatus.Party), Ct);
        await _store.EncounterAsync(species.Run.Id, new NewEncounter("route-2", 17, 0, "Otto", 9, NuzlockeStatus.Party), Ct);
        Assert.Equal("dupe", await Fails(() =>
            _store.EncounterAsync(species.Run.Id, new NewEncounter("route-22", 16, 0, "Dois", 3, NuzlockeStatus.Box), Ct)));
    }

    [Fact]
    public async Task Regras_do_encontro()
    {
        var id = (await NewFrlg()).Run.Id;
        Assert.Equal("nickname_required", await Fails(() => _store.EncounterAsync(id, new NewEncounter("route-1", 16, 0, "  ", 3, NuzlockeStatus.Party), Ct)));
        Assert.Equal("unknown_area", await Fails(() => _store.EncounterAsync(id, new NewEncounter("cabo-poco", 16, 0, "A", 3, NuzlockeStatus.Party), Ct)));
        Assert.Equal("invalid_status", await Fails(() => _store.EncounterAsync(id, new NewEncounter("route-1", 16, 0, "A", 3, NuzlockeStatus.Dead), Ct)));
        Assert.Equal("invalid_species", await Fails(() => _store.EncounterAsync(id, new NewEncounter("route-1", 0, 0, "A", 3, NuzlockeStatus.Party), Ct)));
        Assert.Equal("invalid_level", await Fails(() => _store.EncounterAsync(id, new NewEncounter("route-1", 16, 0, "A", 101, NuzlockeStatus.Party), Ct)));

        // Sem apelido obrigatório, passa sem apelido. Equipe cheia com 6.
        var free = (await NewFrlg(NuzlockeRules.Default with { NicknameRequired = false, FirstEncounterOnly = false })).Run.Id;
        for (var i = 0; i < NuzlockeStore.MaxPartySize; i++)
            await _store.EncounterAsync(free, new NewEncounter("route-1", 16 + i, 0, null, 3, NuzlockeStatus.Party), Ct);
        Assert.Equal("party_full", await Fails(() => _store.EncounterAsync(free, new NewEncounter("route-1", 30, 0, null, 3, NuzlockeStatus.Party), Ct)));
    }

    [Fact]
    public async Task Equipe_acima_do_cap_aparece_em_OverCap()
    {
        var run = await NewFrlg();
        var cap = _data.Bosses("frlg")[0].LevelCap;
        var view = await _store.EncounterAsync(run.Run.Id, new NewEncounter("route-1", 16, 0, "Pidge", 3, NuzlockeStatus.Party), Ct);
        var mon = view.Party.Single();
        Assert.Empty(view.OverCap);

        view = await _store.UpdateMonAsync(run.Run.Id, mon.Id,
            new MonChanges(null, cap + 1, null, null, ["gust", "tackle"]), Ct);
        Assert.Equal(mon.Id, Assert.Single(view.OverCap).Id);
        Assert.Equal(["gust", "tackle"], view.Party.Single().Moves);
        Assert.Equal("invalid_move", await Fails(() => _store.UpdateMonAsync(run.Run.Id, mon.Id, new MonChanges(null, null, null, null, ["nada"]), Ct)));
        Assert.Equal("too_many_moves", await Fails(() => _store.UpdateMonAsync(run.Run.Id, mon.Id,
            new MonChanges(null, null, null, null, ["gust", "tackle", "growl", "ember", "bite"]), Ct)));

        // Morte com causa vai para o cemitério; evolução troca a espécie.
        view = await _store.UpdateMonAsync(run.Run.Id, mon.Id, new MonChanges(null, null, null, null, null, Species: 17), Ct);
        Assert.Equal(17, view.Party.Single().Species);
        view = await _store.UpdateMonAsync(run.Run.Id, mon.Id, new MonChanges(NuzlockeStatus.Dead, null, null, "Onix do Brock", null), Ct);
        var dead = Assert.Single(view.Graveyard);
        Assert.Equal("Onix do Brock", dead.DeathCause);
        Assert.NotNull(dead.DiedAt);
        Assert.Empty(view.Party);
        Assert.Empty(view.OverCap);
    }

    [Fact]
    public async Task Run_no_sv_sem_treinadores_comuns_nao_falha()
    {
        var view = await _store.CreateAsync(new NewRun("Paldea", "sv", "scarlet", null), Ct);
        Assert.False(view.TrainersAvailable);
        Assert.Empty(view.AreaTrainers);
        Assert.NotNull(view.CurrentArea);
        Assert.NotNull(view.NextBoss);
        // Os encontros vêm dos nossos dados (o PokeAPI não tem o SV).
        var area = _data.Encounters("sv").First().Area;
        view = await _store.SetAreaAsync(view.Run.Id, area, Ct);
        Assert.NotEmpty(view.AreaEncounters);
        Assert.Empty(view.AreaTrainers);
        foreach (var a in _data.Areas("sv"))
            Assert.Empty((await _store.SetAreaAsync(view.Run.Id, a.Id, Ct)).AreaTrainers);
    }

    [Fact]
    public async Task Finish_com_wipe_grava_EndedAt_e_trava_a_run()
    {
        var run = await NewFrlg();
        var done = await _store.FinishAsync(run.Run.Id, "wipe", Ct);
        Assert.NotNull(done.Run.EndedAt);
        Assert.Equal("wipe", done.Run.Status);

        var ex = await Assert.ThrowsAsync<NuzlockeException>(() =>
            _store.EncounterAsync(run.Run.Id, new NewEncounter("route-1", 16, 0, "A", 3, NuzlockeStatus.Party), Ct));
        Assert.Equal(("run_finished", 409), (ex.Code, ex.Status));
        Assert.Equal("run_finished", await Fails(() => _store.FinishAsync(run.Run.Id, "champion", Ct)));
        Assert.Equal("invalid_status", await Fails(() => _store.FinishAsync(run.Run.Id, "talvez", Ct)));
        var summary = Assert.Single(await _store.ListAsync(Ct));
        Assert.Equal("wipe", summary.Status);

        await _store.DeleteAsync(run.Run.Id, Ct);
        Assert.Empty(await _store.ListAsync(Ct));
    }

    [Fact]
    public async Task Pausar_e_renomear()
    {
        var run = await NewFrlg();
        var view = await _store.UpdateAsync(run.Run.Id, new RunChanges(" Kanto 2 ", "paused", null), Ct);
        Assert.Equal(("Kanto 2", "paused"), (view.Run.Name, view.Run.Status));
        Assert.Equal("invalid_status", await Fails(() => _store.UpdateAsync(run.Run.Id, new RunChanges(null, "champion", null), Ct)));
    }

    [Fact]
    public async Task Proximos_golpes_por_nivel_e_na_evolucao()
    {
        var run = await NewFrlg();
        var view = await _store.EncounterAsync(run.Run.Id, new NewEncounter("route-1", 16, 0, "Pidge", 5, NuzlockeStatus.Party), Ct);
        var learnset = await _store.LearnsetAsync(run.Run.Id, view.Party.Single().Id, Ct);
        Assert.Contains(learnset.Known, m => m.Move == "tackle" && m.Name == "Tackle" && m.Type == "normal");
        Assert.All(learnset.Known, m => Assert.True(m.Level <= 5));
        Assert.NotEmpty(learnset.Upcoming);
        Assert.All(learnset.Upcoming, m => Assert.True(m.Level > 5));
        Assert.Contains(learnset.Upcoming, m => m.Move == "gust");

        // Pidgey 15: conhece Tackle, Sand Attack e Gust; o próximo é Whirlwind no 19.
        var mon = view.Party.Single();
        await _store.UpdateMonAsync(run.Run.Id, mon.Id, new MonChanges(null, 15, null, null, null), Ct);
        var at15 = await _store.LearnsetAsync(run.Run.Id, mon.Id, Ct);
        Assert.Superset(new HashSet<string> { "tackle", "sand-attack", "gust" }, at15.Known.Select(m => m.Move).ToHashSet());
        Assert.Equal(("whirlwind", 19), (at15.Upcoming[0].Move, at15.Upcoming[0].Level));
        var pidgeotto = Assert.Single(learnset.Evolutions);
        Assert.Equal(17, pidgeotto.Species);
        Assert.Contains("level 18", pidgeotto.Conditions);
        Assert.All(pidgeotto.Upcoming, m => Assert.True(m.Level > 5));
    }

    [Fact]
    public async Task Mon_missed_que_vira_capturado_passa_pelas_clausulas()
    {
        var id = (await NewFrlg(NuzlockeRules.Default with { SpeciesClause = true })).Run.Id;
        await _store.EncounterAsync(id, new NewEncounter("route-1", 16, 0, "Pidge", 3, NuzlockeStatus.Party), Ct);
        var view = await _store.EncounterAsync(id, new NewEncounter("route-2", 16, 0, "Dois", 3, NuzlockeStatus.Missed), Ct);
        var dupe = view.Lost.Single();
        Assert.Equal("dupe", await Fails(() => _store.UpdateMonAsync(id, dupe.Id, new MonChanges(NuzlockeStatus.Party, null, null, null, null), Ct)));

        // Área já usada: outro Pokémon capturado na rota.
        await _store.EncounterAsync(id, new NewEncounter("route-22", 56, 0, "Mankey", 3, NuzlockeStatus.Box), Ct);
        view = await _store.EncounterAsync(id, new NewEncounter("route-22", 21, 0, "Spear", 3, NuzlockeStatus.Missed), Ct);
        var second = view.Lost.Single(m => m.Species == 21);
        Assert.Equal("area_already_used", await Fails(() =>
            _store.UpdateMonAsync(id, second.Id, new MonChanges(NuzlockeStatus.Box, null, null, null, null), Ct)));

        // Sozinho na área e sem repetir espécie, pode virar capturado; e o próprio não conta contra si.
        view = await _store.EncounterAsync(id, new NewEncounter("viridian-forest", 10, 0, "Cat", 3, NuzlockeStatus.Missed), Ct);
        var caterpie = view.Lost.Single(m => m.Species == 10);
        view = await _store.UpdateMonAsync(id, caterpie.Id, new MonChanges(NuzlockeStatus.Box, null, null, null, null), Ct);
        Assert.Contains(view.Box, m => m.Id == caterpie.Id);
        // Mudar entre party e box (já capturado) não reconfere as cláusulas.
        view = await _store.UpdateMonAsync(id, caterpie.Id, new MonChanges(NuzlockeStatus.Party, null, null, null, null), Ct);
        Assert.Contains(view.Party, m => m.Id == caterpie.Id);
    }

    [Fact]
    public async Task Chefe_de_outra_versao_nao_pode_ser_vencido()
    {
        var bea = _data.Bosses("swsh").First(b => b.Versions is ["sword"]);
        var shield = await _store.CreateAsync(new NewRun(null, "swsh", "shield", null), Ct);
        Assert.Equal("unknown_boss", await Fails(() => _store.DefeatBossAsync(shield.Run.Id, bea.Id, Ct)));
        var sword = await _store.CreateAsync(new NewRun(null, "swsh", "sword", null), Ct);
        Assert.Contains(bea.Id, (await _store.DefeatBossAsync(sword.Run.Id, bea.Id, Ct)).DefeatedBosses);
    }

    [Fact]
    public async Task Run_de_outro_dono_nao_aparece()
    {
        var other = Guid.NewGuid();
        RunView theirs;
        using (OwnerScope.Use(other))
        {
            theirs = await NewFrlg();
            theirs = await _store.EncounterAsync(theirs.Run.Id, new NewEncounter("route-1", 16, 0, "Pidge", 3, NuzlockeStatus.Party), Ct);
            Assert.Single(await _store.ListAsync(Ct));
        }
        var monId = theirs.Party.Single().Id;

        Assert.Empty(await _store.ListAsync(Ct));
        Assert.Equal(404, (await Assert.ThrowsAsync<NuzlockeException>(() => _store.GetAsync(theirs.Run.Id, Ct))).Status);
        Assert.Equal(404, (await Assert.ThrowsAsync<NuzlockeException>(() =>
            _store.UpdateMonAsync(theirs.Run.Id, monId, new MonChanges(NuzlockeStatus.Dead, null, null, null, null), Ct))).Status);
        Assert.Equal(404, (await Assert.ThrowsAsync<NuzlockeException>(() => _store.LearnsetAsync(theirs.Run.Id, monId, Ct))).Status);
        Assert.Equal(404, (await Assert.ThrowsAsync<NuzlockeException>(() => _store.DeleteAsync(theirs.Run.Id, Ct))).Status);

        // O Pokémon dela também não é achado pela run de outra conta.
        var mine = await NewFrlg();
        var ex = await Assert.ThrowsAsync<NuzlockeException>(() =>
            _store.UpdateMonAsync(mine.Run.Id, monId, new MonChanges(NuzlockeStatus.Dead, null, null, null, null), Ct));
        Assert.Equal(("mon_not_found", 404), (ex.Code, ex.Status));

        await using var db = await _db.CreateDbContextAsync(Ct);
        Assert.Equal(other, (await db.NuzlockeRuns.IgnoreQueryFilters().SingleAsync(r => r.Id == theirs.Run.Id, Ct)).OwnerId);
        Assert.Equal(NuzlockeStatus.Party, (await db.NuzlockeMons.SingleAsync(m => m.Id == monId, Ct)).Status);
    }
}

public sealed class NuzlockeMigrationTests
{
    [Fact]
    public async Task Migracao_sobre_banco_antigo_mantem_as_hunts()
    {
        using var dir = new TestDataDir();
        var options = new DbContextOptionsBuilder<TradeRockDb>()
            .UseSqlite($"Data Source={dir.Paths.DatabasePath};Pooling=False").Options;
        var ct = TestContext.Current.CancellationToken;

        await using (var db = new TradeRockDb(options))
        {
            await db.GetService<IMigrator>().MigrateAsync("ShinyHunts", ct);
            db.Pokemon.Add(new StoredPokemon
            {
                Id = Guid.NewGuid(), Sha256 = "ab", Size = 100, Species = 25, SpeciesName = "Pikachu", Nickname = "PIKA",
                OtName = "RED", OriginGame = "FireRed", AddedAt = DateTimeOffset.UtcNow, Family = GameFamily.Frlg,
            });
            db.Hunts.Add(new ShinyHunt { GameKey = "sv", MethodId = "masuda", ModifiersJson = "{}", Species = 906 });
            await db.SaveChangesAsync(ct);
        }

        await using (var db = new TradeRockDb(options))
        {
            await db.Database.MigrateAsync(ct);
            Assert.Equal(1, await db.Pokemon.CountAsync(ct));
            Assert.Equal(1, await db.Hunts.CountAsync(ct));
            var run = new NuzlockeRun { Name = "Kanto", GameKey = "frlg", RulesJson = "{}", Status = "active" };
            db.NuzlockeRuns.Add(run);
            await db.SaveChangesAsync(ct);
            Assert.Equal(OwnerScope.LocalOwner, (await db.NuzlockeRuns.SingleAsync(ct)).OwnerId);
        }
    }
}

public sealed class NuzlockeApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-nuzlocke-").FullName;
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

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
        _client.Dispose();
        await _app.DisposeAsync();
        SqliteConnection.ClearAllPools();
        try { Directory.Delete(_dir, true); } catch (IOException) { }
    }

    private async Task<JsonElement> Ok(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync(Ct);
        Assert.True(response.IsSuccessStatusCode, $"{(int)response.StatusCode}: {text}");
        return text.Length == 0 ? default : JsonDocument.Parse(text).RootElement.Clone();
    }

    private async Task<string?> Error(HttpResponseMessage response, HttpStatusCode status)
    {
        Assert.Equal(status, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<JsonElement>(Ct)).GetProperty("error").GetString();
    }

    [Fact]
    public async Task Fluxo_da_run_pela_api()
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido.");
        var view = await Ok(await _client.PostAsJsonAsync("/api/nuzlocke",
            new { name = "Kanto", gameKey = "frlg", version = "firered", rules = new { firstEncounterOnly = true, nicknameRequired = true, speciesClause = true } }, Ct));
        var run = view.GetProperty("run");
        var id = run.GetProperty("id").GetGuid();
        Assert.True(run.GetProperty("rules").GetProperty("speciesClause").GetBoolean());
        Assert.False(run.GetProperty("rules").GetProperty("faintIsDeath").GetBoolean());
        Assert.Equal("pallet-town", view.GetProperty("currentArea").GetProperty("id").GetString());
        Assert.Equal(JsonValueKind.Number, view.GetProperty("levelCap").ValueKind);
        Assert.True(view.GetProperty("trainersAvailable").GetBoolean());

        view = await Ok(await _client.PutAsJsonAsync($"/api/nuzlocke/{id}/area", new { areaId = "route-1" }, Ct));
        Assert.Contains(view.GetProperty("areaEncounters").EnumerateArray(), e => e.GetProperty("species").GetInt32() == 16);
        Assert.Equal("unknown_area", await Error(await _client.PutAsJsonAsync($"/api/nuzlocke/{id}/area", new { areaId = "x" }, Ct),
            HttpStatusCode.BadRequest));

        view = await Ok(await _client.PostAsJsonAsync($"/api/nuzlocke/{id}/encounter",
            new { areaId = "route-1", species = 16, form = 0, nickname = "Pidge", level = 3, status = "party" }, Ct));
        var monId = view.GetProperty("party")[0].GetProperty("id").GetGuid();
        Assert.Equal("area_already_used", await Error(await _client.PostAsJsonAsync($"/api/nuzlocke/{id}/encounter",
            new { areaId = "route-1", species = 19, form = 0, nickname = "Rat", level = 3, status = "box" }, Ct), HttpStatusCode.BadRequest));
        Assert.Equal("nickname_required", await Error(await _client.PostAsJsonAsync($"/api/nuzlocke/{id}/encounter",
            new { areaId = "route-2", species = 19, form = 0, level = 3, status = "box" }, Ct), HttpStatusCode.BadRequest));

        view = await Ok(await _client.PutAsJsonAsync($"/api/nuzlocke/{id}/mon/{monId}", new { level = 8, moves = new[] { "gust" } }, Ct));
        Assert.Equal("gust", view.GetProperty("party")[0].GetProperty("moves")[0].GetString());
        var learnset = await Ok(await _client.GetAsync($"/api/nuzlocke/{id}/mon/{monId}/learnset", Ct));
        Assert.Equal(17, learnset.GetProperty("evolutions")[0].GetProperty("species").GetInt32());
        Assert.Contains(learnset.GetProperty("known").EnumerateArray(), m => m.GetProperty("move").GetString() == "tackle");
        Assert.False(view.GetProperty("party")[0].GetProperty("shiny").GetBoolean());

        var bossId = view.GetProperty("nextBoss").GetProperty("id").GetString();
        view = await Ok(await _client.PostAsync($"/api/nuzlocke/{id}/boss/{bossId}/defeated", null, Ct));
        Assert.Equal(bossId, view.GetProperty("defeatedBosses")[0].GetString());
        Assert.NotEqual(bossId, view.GetProperty("nextBoss").GetProperty("id").GetString());
        Assert.Equal("unknown_boss", await Error(await _client.PostAsync($"/api/nuzlocke/{id}/boss/nada/defeated", null, Ct),
            HttpStatusCode.BadRequest));

        Assert.Equal(1, (await Ok(await _client.GetAsync("/api/nuzlocke", Ct))).GetArrayLength());
        view = await Ok(await _client.PostAsJsonAsync($"/api/nuzlocke/{id}/finish", new { status = "champion" }, Ct));
        Assert.Equal(JsonValueKind.String, view.GetProperty("run").GetProperty("endedAt").ValueKind);
        Assert.Equal("run_finished", await Error(await _client.PutAsJsonAsync($"/api/nuzlocke/{id}/area", new { areaId = "route-1" }, Ct),
            HttpStatusCode.Conflict));

        Assert.Equal("unknown_game", await Error(await _client.PostAsJsonAsync("/api/nuzlocke", new { gameKey = "zz" }, Ct),
            HttpStatusCode.BadRequest));
        Assert.Equal("version_required", await Error(await _client.PostAsJsonAsync("/api/nuzlocke", new { gameKey = "swsh" }, Ct),
            HttpStatusCode.BadRequest));
        Assert.Equal(HttpStatusCode.NoContent, (await _client.DeleteAsync($"/api/nuzlocke/{id}", Ct)).StatusCode);
        Assert.Equal("run_not_found", await Error(await _client.GetAsync($"/api/nuzlocke/{id}", Ct), HttpStatusCode.NotFound));
    }
}

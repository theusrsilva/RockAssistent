using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PKHeX.Core;
using TradeRock.App;
using TradeRock.App.Server;
using TradeRock.Core;
using TradeRock.Core.Games;
using TradeRock.Core.Saves;
using TradeRock.Core.Storage;
using TradeRock.Core.Trading;

namespace TradeRock.Tests;

public sealed class BoxesEndpointsTests : IAsyncLifetime
{
    private const ushort Mewtwo = 150, Gengar = 94;

    private readonly SessionToken _token = SessionToken.Generate();
    private WebApplication _app = null!;
    private string _dataDir = null!;
    private string _saves = null!;
    private HttpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        _dataDir = Directory.CreateTempSubdirectory("traderock-boxes-").FullName;
        _saves = Directory.CreateDirectory(Path.Combine(_dataDir, "saves")).FullName;
        _app = TradeRockServer.Create(["--data-dir", _dataDir, "--Engines:Fake:SpeedFactor=0"], _token, b => b.WebHost.UseTestServer());
        await _app.StartAsync();
        _client = _app.GetTestClient();
        _client.BaseAddress = new Uri("http://127.0.0.1/");
        _client.DefaultRequestHeaders.Add(SessionAuthMiddleware.HeaderName, _token.Value);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
        try { Directory.Delete(_dataDir, recursive: true); } catch (IOException) { }
    }

    private static async Task<JsonElement> Json(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"{(int)response.StatusCode}: {text}");
        return text.Length == 0 ? default : JsonDocument.Parse(text).RootElement.Clone();
    }

    private async Task<JsonElement> Get(string url) => await Json(await _client.GetAsync(url));
    private async Task<JsonElement> Post(string url, object body) => await Json(await _client.PostAsJsonAsync(url, body));

    private async Task<string> AddFireRedAsync(string name = "fr.sav")
    {
        File.WriteAllBytes(Path.Combine(_saves, name), SaveFixture.BlankFireRed());
        await Json(await _client.PutAsJsonAsync("/api/settings/save-folders", new[] { new { path = _saves, pattern = "" } }));
        var list = await Get("/api/boxes/saves?refresh=1");
        return list.EnumerateArray().Single(e => e.GetProperty("path").GetString()!.EndsWith(name)).GetProperty("id").GetString()!;
    }

    private async Task<Guid> DemoAsync(ushort species) =>
        (await Get("/api/vault/pokemon")).EnumerateArray().Single(p => p.GetProperty("species").GetInt32() == species).GetProperty("id").GetGuid();

    private async Task<JsonElement> SlotsAsync(string saveId) => (await Get($"/api/boxes/saves/{saveId}")).GetProperty("slots");

    private static byte[] StoredBytes(PKM pk)
    {
        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);
        return data;
    }

    [Fact]
    public async Task Evoluir_Haunter_no_save_grava_e_rele()
    {
        var saveId = await AddFireRedAsync();
        var haunter = await TradeEvolutionPkhexTests.LegalAsync(TradeGame.FireRed, 93);
        var data = StoredBytes(haunter);
        // Na caixa e no time (caixa -1).
        await Post($"/api/boxes/saves/{saveId}/apply", new { changes = new object[] { new { box = 0, slot = 0, data }, new { box = -1, slot = 0, data } } });

        var options = await Get($"/api/boxes/saves/{saveId}/slot/0/0/evolve-options");
        var option = Assert.Single(options.EnumerateArray());
        Assert.Equal(Gengar, option.GetProperty("species").GetInt32());
        Assert.Equal("Gengar", option.GetProperty("speciesName").GetString());

        var sha = (await Get($"/api/boxes/saves/{saveId}")).GetProperty("info").GetProperty("sha256").GetString();
        var result = await Post($"/api/boxes/saves/{saveId}/slot/0/0/evolve", new { species = Gengar, form = 0, sha256 = sha, language = "en" });
        Assert.True(File.Exists(result.GetProperty("backupPath").GetString()));
        Assert.True(result.GetProperty("legality").GetProperty("valid").GetBoolean(), result.GetProperty("legality").ToString());
        var slots = (await SlotsAsync(saveId)).EnumerateArray().ToList();
        Assert.Equal(Gengar, slots.Single(e => e.GetProperty("box").GetInt32() == 0).GetProperty("summary").GetProperty("species").GetInt32());

        // O SHA velho não vale mais.
        var stale = await _client.PostAsJsonAsync($"/api/boxes/saves/{saveId}/slot/-1/0/evolve", new { species = Gengar, form = 0, sha256 = sha });
        Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);
        Assert.Contains("save_changed", await stale.Content.ReadAsStringAsync());

        // O time também evolui (sem SHA: vale o da leitura).
        await Post($"/api/boxes/saves/{saveId}/slot/-1/0/evolve", new { species = Gengar, form = 0 });
        var party = (await SlotsAsync(saveId)).EnumerateArray().Single(e => e.GetProperty("box").GetInt32() == -1);
        Assert.Equal(Gengar, party.GetProperty("summary").GetProperty("species").GetInt32());
        var sav = SaveFixture.Open(File.ReadAllBytes(Path.Combine(_saves, "fr.sav")));
        var partyGengar = sav.GetPartySlotAtIndex(0);
        Assert.Equal(Gengar, partyGengar.Species);
        Assert.Equal(partyGengar.GetStats(partyGengar.PersonalInfo)[0], partyGengar.Stat_HPMax); // atributos de Gengar

        // Gengar não evolui mais.
        var again = await _client.PostAsJsonAsync($"/api/boxes/saves/{saveId}/slot/0/0/evolve", new { species = Gengar, form = 0 });
        Assert.Equal(HttpStatusCode.BadRequest, again.StatusCode);
        Assert.Contains("evolve_not_available", await again.Content.ReadAsStringAsync());
        var empty = await _client.GetAsync($"/api/boxes/saves/{saveId}/slot/0/5/evolve-options");
        Assert.Equal(HttpStatusCode.NotFound, empty.StatusCode);
    }

    [Fact]
    public async Task Evoluir_no_cofre_mantem_a_entrada_e_guarda_o_original_na_lixeira()
    {
        var haunter = await TradeEvolutionPkhexTests.LegalAsync(TradeGame.FireRed, 93);
        var store = _app.Services.GetRequiredService<PokemonStore>();
        var stored = (await store.ImportAsync(StoredBytes(haunter), PokemonOrigin.Created, "haunter.pk3", default, GameFamily.Frlg)).Pokemon;

        var options = await Get($"/api/vault/pokemon/{stored.Id}/evolve-options");
        Assert.Equal(Gengar, Assert.Single(options.EnumerateArray()).GetProperty("species").GetInt32());

        var wrong = await _client.PostAsJsonAsync($"/api/vault/pokemon/{stored.Id}/evolve", new { species = Mewtwo, form = 0 });
        Assert.Equal(HttpStatusCode.BadRequest, wrong.StatusCode);
        Assert.Contains("evolve_wrong_target", await wrong.Content.ReadAsStringAsync());

        var result = await Post($"/api/vault/pokemon/{stored.Id}/evolve", new { species = Gengar, form = 0 });
        Assert.Equal(stored.Id, result.GetProperty("pokemon").GetProperty("id").GetGuid());
        Assert.Equal(Gengar, result.GetProperty("pokemon").GetProperty("species").GetInt32());
        Assert.True(result.GetProperty("legality").GetProperty("valid").GetBoolean());
        var backupId = result.GetProperty("backupId").GetGuid();
        var backup = await store.GetAsync(backupId, default);
        Assert.NotNull(backup?.DeletedAt);
        Assert.Equal((ushort)93, backup!.Species);

        var mewtwo = await DemoAsync(Mewtwo);
        Assert.Equal(0, (await Get($"/api/vault/pokemon/{mewtwo}/evolve-options")).GetArrayLength());
        var none = await _client.PostAsJsonAsync($"/api/vault/pokemon/{mewtwo}/evolve", new { species = Gengar, form = 0 });
        Assert.Contains("evolve_not_available", await none.Content.ReadAsStringAsync());
        var missing = await _client.GetAsync($"/api/vault/pokemon/{Guid.NewGuid()}/evolve-options");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task Clonar_no_cofre_cria_uma_entrada_nova_na_caixa_de_entrada()
    {
        var gengar = await TradeEvolutionPkhexTests.LegalAsync(TradeGame.FireRed, 94);
        gengar.SetShiny();
        var store = _app.Services.GetRequiredService<PokemonStore>();
        var stored = (await store.ImportAsync(StoredBytes(gengar), PokemonOrigin.Created, "gengar.pk3", default, GameFamily.Frlg)).Pokemon;

        var result = await Post($"/api/vault/pokemon/{stored.Id}/clone", new { language = "en" });
        var cloneId = result.GetProperty("pokemon").GetProperty("id").GetGuid();
        Assert.NotEqual(stored.Id, cloneId);
        Assert.True(result.GetProperty("legality").TryGetProperty("valid", out _));
        var clone = await store.GetAsync(cloneId, default);
        Assert.NotNull(clone);
        Assert.Null(clone!.BoxId);
        Assert.Null(clone.DeletedAt);
        Assert.Equal(Gengar, clone.Species);
        Assert.True(clone.IsShiny);
        Assert.NotEqual(stored.Pid, clone.Pid);
        Assert.NotEqual(stored.Sha256, clone.Sha256);
        Assert.Null((await store.GetAsync(stored.Id, default))!.DeletedAt);

        var missing = await _client.PostAsJsonAsync($"/api/vault/pokemon/{Guid.NewGuid()}/clone", new { });
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task Pastas_e_lista_de_saves_com_arquivo_ruim()
    {
        Assert.Equal(0, (await Get("/api/settings/save-folders")).GetArrayLength());
        File.WriteAllBytes(Path.Combine(_saves, "x.sav"), new byte[10]);
        var id = await AddFireRedAsync();

        var folders = await Get("/api/settings/save-folders");
        Assert.Equal(SaveFolders.DefaultPattern, folders[0].GetProperty("pattern").GetString());
        Assert.True(folders[0].GetProperty("exists").GetBoolean());

        var list = (await Get("/api/boxes/saves")).EnumerateArray().ToList();
        Assert.Equal(2, list.Count);
        Assert.Equal("sav_unknown", list.Single(e => e.GetProperty("error").ValueKind == JsonValueKind.String).GetProperty("error").GetString());
        var save = await Get($"/api/boxes/saves/{id}");
        Assert.Equal("frlg", save.GetProperty("info").GetProperty("gameKey").GetString());
        Assert.Equal(0, save.GetProperty("slots").GetArrayLength());

        var missing = await _client.GetAsync("/api/boxes/saves/deadbeef");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Contains("save_not_found", await missing.Content.ReadAsStringAsync());

        var relative = await _client.PutAsJsonAsync("/api/settings/save-folders", new[] { new { path = "relativa", pattern = "" } });
        Assert.Equal(HttpStatusCode.BadRequest, relative.StatusCode);
        Assert.Contains("folder_invalid", await relative.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Pk8_do_cofre_num_save_de_firered_falha_com_save_wrong_game()
    {
        var saveId = await AddFireRedAsync();
        var pk8 = new PK8 { Species = Gengar, CurrentLevel = 40 };
        pk8.RefreshChecksum();
        var data = new byte[pk8.SIZE_STORED];
        pk8.WriteDecryptedDataStored(data);
        var store = _app.Services.GetRequiredService<PokemonStore>();
        var stored = (await store.ImportAsync(data, PokemonOrigin.Created, "gengar.pk8", default, GameFamily.Swsh)).Pokemon;

        var response = await _client.PostAsJsonAsync("/api/boxes/move", new
        {
            from = new { kind = "vault", pokemonId = stored.Id },
            to = new { kind = "save", saveId, box = 0, slot = 0 },
            mode = "copy",
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("save_wrong_game", body);
        Assert.Contains("home_no_destination", body);
        Assert.Equal(0, (await SlotsAsync(saveId)).GetArrayLength());
    }

    [Fact]
    public async Task Copiar_baixar_achar_anexar_sincronizar_e_mover()
    {
        var saveId = await AddFireRedAsync();
        var mewtwo = await DemoAsync(Mewtwo);

        // Cofre → save (cópia).
        var copy = await Post("/api/boxes/move", new
        {
            from = new { kind = "vault", pokemonId = mewtwo },
            to = new { kind = "save", saveId, box = 0, slot = 0 },
            mode = "copy",
        });
        Assert.False(copy.GetProperty("converted").GetBoolean());
        Assert.True(File.Exists(copy.GetProperty("backupPath").GetString()));
        var slot = Assert.Single((await SlotsAsync(saveId)).EnumerateArray());
        Assert.Equal(Mewtwo, slot.GetProperty("summary").GetProperty("species").GetInt32());
        Assert.Equal("frlg", slot.GetProperty("family").GetString());

        // Posição ocupada não é sobrescrita pelo mover.
        var occupied = await _client.PostAsJsonAsync("/api/boxes/move", new
        {
            from = new { kind = "vault", pokemonId = mewtwo },
            to = new { kind = "save", saveId, box = 0, slot = 0 },
            mode = "copy",
        });
        Assert.Contains("slot_occupied", await occupied.Content.ReadAsStringAsync());

        // Baixar o .pk3 do slot.
        var file = await _client.GetAsync($"/api/boxes/saves/{saveId}/slot/0/0/file");
        Assert.Equal(HttpStatusCode.OK, file.StatusCode);
        Assert.Equal(80, (await file.Content.ReadAsByteArrayAsync()).Length);
        Assert.EndsWith(".pk3", file.Content.Headers.ContentDisposition!.FileName!.Trim('"'));

        // Onde está: no save e no cofre.
        var where = (await Get($"/api/boxes/where/{Mewtwo}")).EnumerateArray().ToList();
        Assert.Equal(["save", "vault"], where.Select(w => w.GetProperty("kind").GetString()).Order().ToList());
        Assert.Equal("fr.sav", where.Single(w => w.GetProperty("kind").GetString() == "save").GetProperty("name").GetString());

        // Anexar: cópia no slot 1 ligada ao Pokémon do cofre.
        var attach = await Post("/api/boxes/move", new
        {
            from = new { kind = "vault", pokemonId = mewtwo },
            to = new { kind = "save", saveId, box = 0, slot = 1 },
            mode = "attach",
        });
        Assert.Equal(JsonValueKind.String, attach.GetProperty("attachmentId").ValueKind);
        Assert.Single((await Get("/api/boxes/attachments")).EnumerateArray());

        // Nada mudou no save: a sincronização não mexe.
        Assert.Equal("unchanged", (await Post("/api/boxes/sync-attachments", new { }))[0].GetProperty("status").GetString());

        // O Mewtwo do slot 1 sobe de nível no "jogo": a sincronização atualiza o cofre.
        var inSave = (await SlotsAsync(saveId)).EnumerateArray().Single(s => s.GetProperty("slot").GetInt32() == 1);
        var pk = new PK3(inSave.GetProperty("data").GetBytesFromBase64()) { CurrentLevel = 75 };
        pk.RefreshChecksum();
        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);
        await Post($"/api/boxes/saves/{saveId}/apply", new { changes = new[] { new { box = 0, slot = 1, data } } });
        var sync = await Post("/api/boxes/sync-attachments", new { });
        Assert.Equal("updated", sync[0].GetProperty("status").GetString());
        Assert.Equal(75, (await Get("/api/vault/pokemon")).EnumerateArray().Single(p => p.GetProperty("id").GetGuid() == mewtwo).GetProperty("level").GetInt32());

        // Save → cofre (mover): sai do save e fica no cofre.
        var moved = await Post("/api/boxes/move", new
        {
            from = new { kind = "save", saveId, box = 0, slot = 0 },
            to = new { kind = "vault" },
            mode = "move",
        });
        Assert.Equal("vault", moved.GetProperty("to").GetProperty("kind").GetString());
        Assert.DoesNotContain((await SlotsAsync(saveId)).EnumerateArray(), s => s.GetProperty("slot").GetInt32() == 0);
    }

    [Fact]
    public async Task Sincronizacao_que_falha_tenta_de_novo_na_proxima()
    {
        var saveId = await AddFireRedAsync();
        var mewtwo = await DemoAsync(Mewtwo);
        await Post("/api/boxes/move", new
        {
            from = new { kind = "vault", pokemonId = mewtwo },
            to = new { kind = "save", saveId, box = 0, slot = 0 },
            mode = "attach",
        });

        // O Mewtwo sobe de nível no save, e o cofre já tem outro registro com esses mesmos dados:
        // substituir daria dois Pokémon iguais no cofre (pokemon_duplicate).
        var inSave = Assert.Single((await SlotsAsync(saveId)).EnumerateArray());
        var pk = new PK3(inSave.GetProperty("data").GetBytesFromBase64()) { CurrentLevel = 75 };
        pk.RefreshChecksum();
        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);
        await _app.Services.GetRequiredService<PokemonStore>().ImportAsync(data, PokemonOrigin.Imported, "m.pk3", default, GameFamily.Frlg);
        await Post($"/api/boxes/saves/{saveId}/apply", new { changes = new[] { new { box = 0, slot = 0, data } } });

        Assert.Equal("slot_changed", (await Post("/api/boxes/sync-attachments", new { }))[0].GetProperty("status").GetString());
        // O SHA do save não foi marcado como conferido: a próxima sincronização tenta de novo.
        Assert.Equal("slot_changed", (await Post("/api/boxes/sync-attachments", new { }))[0].GetProperty("status").GetString());
    }

    [Fact]
    public async Task Crystal_vai_para_o_cofre_como_pk2_e_volta_para_o_save()
    {
        var sav = BlankSaveFile.Get(GameVersion.C, "ROCHA", LanguageID.English);
        var pk = sav.BlankPKM;
        pk.Species = Gengar;
        pk.CurrentLevel = 30;
        pk.RefreshChecksum();
        SaveFileAccess.WriteSlot(sav, SaveFileAccess.Party, 0, pk);
        SaveFileAccess.WriteSlot(sav, 0, 0, pk);
        File.WriteAllBytes(Path.Combine(_saves, "crystal.sav"), SaveFileAccess.Export(sav));
        var saveId = await AddFireRedAsync();
        var crystal = (await Get("/api/boxes/saves")).EnumerateArray()
            .Single(e => e.GetProperty("path").GetString()!.EndsWith("crystal.sav")).GetProperty("id").GetString()!;

        // Save → cofre: todas as gerações cabem no cofre (D34).
        var moved = await Post("/api/boxes/move", new
        {
            from = new { kind = "save", saveId = crystal, box = 0, slot = 0 },
            to = new { kind = "vault" },
            mode = "copy",
        });
        var id = moved.GetProperty("to").GetProperty("pokemonId").GetGuid();
        var stored = await Get($"/api/pokemon/{id}");
        Assert.Equal("gen2", stored.GetProperty("family").GetString());
        Assert.Equal(2, stored.GetProperty("generation").GetInt32());
        Assert.False(stored.GetProperty("tradable").GetBoolean());
        Assert.Single((await Get("/api/vault/pokemon?game=gen2")).EnumerateArray());
        Assert.Empty((await Get("/api/vault/pokemon?game=gen4")).EnumerateArray());

        // Exporta como .pk2 e não troca com o Switch.
        var file = await _client.GetAsync($"/api/pokemon/{id}/file");
        Assert.True(file.IsSuccessStatusCode);
        Assert.EndsWith(".pk2", file.Content.Headers.ContentDisposition!.FileName!.Trim('"'));
        var builder = await _client.GetAsync($"/api/builder/from/{id}");
        Assert.Equal(HttpStatusCode.BadRequest, builder.StatusCode);
        var view = await Get($"/api/vault/pokemon/{id}/view");
        Assert.Equal(JsonValueKind.Null, view.GetProperty("nature").ValueKind);
        Assert.Equal(6, view.GetProperty("ivs").GetArrayLength());

        // Cofre → save do mesmo formato: grava direto (Gen 2 compacta a caixa: a próxima posição livre é a 1).
        var back = await Post("/api/boxes/move", new
        {
            from = new { kind = "vault", pokemonId = id },
            to = new { kind = "save", saveId = crystal, box = 0, slot = 1 },
            mode = "copy",
        });
        Assert.False(back.GetProperty("converted").GetBoolean());
        Assert.Equal(2, (await SlotsAsync(crystal)).EnumerateArray().Count(e => e.GetProperty("box").GetInt32() == 0));

        // Cofre (pk2) → save da Gen 3: não há caminho para trás.
        var cross = await _client.PostAsJsonAsync("/api/boxes/move", new
        {
            from = new { kind = "vault", pokemonId = id },
            to = new { kind = "save", saveId, box = 0, slot = 0 },
            mode = "copy",
        });
        Assert.Contains("save_wrong_game", await cross.Content.ReadAsStringAsync());

        // Entre saves de formatos diferentes sem conversão do HOME: save_wrong_game.
        cross = await _client.PostAsJsonAsync("/api/boxes/move", new
        {
            from = new { kind = "save", saveId = crystal, box = 0, slot = 0 },
            to = new { kind = "save", saveId, box = 0, slot = 0 },
            mode = "copy",
        });
        Assert.Contains("save_wrong_game", await cross.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("GET", "/api/boxes/saves")]
    [InlineData("POST", "/api/boxes/move")]
    [InlineData("GET", "/api/settings/save-folders")]
    [InlineData("PUT", "/api/settings/save-folders")]
    [InlineData("POST", "/api/saves/open-folder")]
    public void Rotas_de_saves_sao_so_da_janela_local(string method, string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = method;
        context.Request.Path = path;
        Assert.True(AccessPolicy.IsLocalOnly(context.Request));
    }

    [Fact]
    public async Task Escolher_pasta_sem_janela_da_no_window()
    {
        var response = await _client.PostAsync("/api/saves/open-folder", null);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Contains("no_window", await response.Content.ReadAsStringAsync());
    }
}

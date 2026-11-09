using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using PKHeX.Core;
using TradeRock.App;
using TradeRock.App.Server;
using TradeRock.Core;
using TradeRock.Core.Builder;
using TradeRock.Core.Games;
using TradeRock.Core.Pokemon;
using TradeRock.Core.Storage;

namespace TradeRock.Tests;

/// <summary>"Adaptar para um jogo mais novo" (D34): caminhos oficiais e as restrições de cada etapa.</summary>
public sealed class AdaptTests : IAsyncLifetime
{
    private const ushort Surf = 57, ThunderShock = 84, Growl = 45, Tackle = 33;

    private readonly TestDataDir _dir = new();
    private PokemonStore _pokemon = null!;

    private static readonly SimpleTrainerInfo Owner = new(GameVersion.SL)
    {
        OT = "ROCHA", TID16 = 1234, SID16 = 5678, Language = (int)LanguageID.English,
    };

    public async ValueTask InitializeAsync()
    {
        IDbContextFactory<TradeRockDb> db = await _dir.CreateDbFactoryAsync();
        _pokemon = new PokemonStore(db, _dir.Paths, TimeProvider.System);
    }

    public ValueTask DisposeAsync()
    {
        _dir.Dispose();
        return ValueTask.CompletedTask;
    }

    internal static PK1 Pikachu1()
    {
        var pk = new PK1 { Species = 25, CurrentLevel = 5, OriginalTrainerName = "ASH", TID16 = 12345, Nickname = "PIKACHU" };
        pk.Move1 = ThunderShock;
        pk.Move2 = Growl;
        pk.Move1_PP = 30;
        pk.Move2_PP = 40;
        pk.RefreshChecksum();
        return pk;
    }

    private static PK3 Pk3(ushort species = 25, ushort move = ThunderShock, int item = 0)
    {
        var pk = new PK3
        {
            Species = species, CurrentLevel = 20, OriginalTrainerName = "RED", TID16 = 1, SID16 = 2, Version = GameVersion.FR,
            Language = 2, PID = 0x12345678, Ball = 4, MetLevel = 5, MetLocation = 89, HeldItem = item,
        };
        pk.Move1 = move;
        pk.Move1_PP = 15;
        pk.RefreshChecksum();
        return pk;
    }

    private async Task<StoredPokemon> Store(PKM pk, StorageFormat format)
    {
        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);
        return (await _pokemon.ImportAsync(data, format, PokemonOrigin.Imported, null, default)).Pokemon;
    }

    [Fact]
    public async Task Pikachu_da_Gen_1_vai_para_a_Gen_7_e_depois_para_o_Scarlet_Violet()
    {
        var original = await Store(Pikachu1(), StorageFormat.Pk1);

        var options = await _pokemon.AdaptOptionsAsync(original, default);
        var sv = options.Single(o => o.Target == StorageFormat.Pk9);
        Assert.Null(sv.Refusal);
        Assert.Equal([TransferStep.VirtualConsole, TransferStep.Home], sv.Steps);
        Assert.Equal("no_route", options.Single(o => o.Target == StorageFormat.Pk4).Refusal);
        Assert.Equal("lgpe_no_receive", options.Single(o => o.Target == StorageFormat.Pb7).Refusal);

        var (gen7, _, pk7) = await _pokemon.AdaptAsync(original.Id, StorageFormat.Pk7, Owner, default);
        Assert.Equal(StorageFormat.Pk7, gen7.Format);
        Assert.NotEqual(original.Id, gen7.Id);
        Assert.Null(gen7.BoxId); // Caixa de entrada
        Assert.True(new LegalityAnalysis(pk7).Valid);

        // O original continua no cofre, no formato antigo.
        var still = await _pokemon.GetAsync(original.Id, default);
        Assert.NotNull(still);
        Assert.Null(still.DeletedAt);
        Assert.Equal(StorageFormat.Pk1, still.Format);

        var (gen9, _, pk9) = await _pokemon.AdaptAsync(gen7.Id, StorageFormat.Pk9, Owner, default);
        Assert.Equal(StorageFormat.Pk9, gen9.Format);
        Assert.Equal(GameFamily.Sv, gen9.Family);
        Assert.Equal(25, gen9.Species);
        // A legalidade é analisada (avisa, não bloqueia): sem o rastreador do HOME, fica "não verificável".
        var report = LegalityReporter.Analyze(pk9);
        Assert.NotEqual(LegalityStatus.Illegal, report.Status);
        Assert.Equal(report.Status.ToString().ToLowerInvariant(), gen9.Legality);
    }

    [Fact]
    public async Task Gen_3_com_golpe_de_HM_e_recusada_no_Pal_Park()
    {
        var surfer = await Store(Pk3(move: Surf), StorageFormat.Pk3);
        var options = await _pokemon.AdaptOptionsAsync(surfer, default);
        foreach (var target in new[] { StorageFormat.Pk4, StorageFormat.Pk5, StorageFormat.Pk7, StorageFormat.Pk9 })
        {
            var option = options.Single(o => o.Target == target);
            Assert.Equal("pal_park_hm", option.Refusal);
            Assert.Equal(nameof(TransferStep.PalPark), option.RefusedAt);
        }
        var ex = await Assert.ThrowsAsync<VaultException>(() => _pokemon.AdaptAsync(surfer.Id, StorageFormat.Pk4, Owner, default));
        Assert.Equal("pal_park_hm", ex.Code);
    }

    [Fact]
    public async Task Item_segue_no_Pal_Park_volta_para_a_mochila_no_Poke_Transfer_e_Mail_bloqueia()
    {
        const int Potion3 = 13, OrangeMail3 = 121;
        var holder = await Store(Pk3(item: Potion3), StorageFormat.Pk3);
        var (gen4, notes4, pk4) = await _pokemon.AdaptAsync(holder.Id, StorageFormat.Pk4, Owner, default);
        Assert.Empty(notes4);
        Assert.NotEqual(0, pk4.HeldItem); // Pal Park: o item vai junto
        Assert.NotEqual(0, gen4.HeldItem);

        var (gen5, notes5, pk5) = await _pokemon.AdaptAsync(gen4.Id, StorageFormat.Pk5, Owner, default);
        Assert.Contains("held_item_removed", notes5);
        Assert.Equal(0, pk5.HeldItem);
        Assert.Equal(0, gen5.HeldItem);

        var mail = await Store(Pk3(species: 26, item: OrangeMail3), StorageFormat.Pk3);
        var ex = await Assert.ThrowsAsync<VaultException>(() => _pokemon.AdaptAsync(mail.Id, StorageFormat.Pk4, Owner, default));
        Assert.Equal("pal_park_mail", ex.Code);
    }

    [Fact]
    public async Task Ovo_e_recusado()
    {
        var pk = new PK4 { Species = 175, CurrentLevel = 1, IsEgg = true, Version = GameVersion.Pt, Language = 2, OriginalTrainerName = "ROCHA" };
        pk.Move1 = Growl;
        pk.RefreshChecksum();
        var egg = await Store(pk, StorageFormat.Pk4);
        Assert.All((await _pokemon.AdaptOptionsAsync(egg, default)).Where(o => o.Target != StorageFormat.Pb7),
            o => Assert.Equal("egg", o.Refusal));
        var ex = await Assert.ThrowsAsync<VaultException>(() => _pokemon.AdaptAsync(egg.Id, StorageFormat.Pk5, Owner, default));
        Assert.Equal("egg", ex.Code);
    }

    [Fact]
    public async Task Especie_que_nao_existe_no_destino_e_recusada()
    {
        const ushort Pidgey = 16;
        Assert.False(VaultFormats.Get(StorageFormat.Pk9).HasSpecies(Pidgey));
        var pk = new PK7 { Species = Pidgey, CurrentLevel = 10, Version = GameVersion.US, Language = 2, OriginalTrainerName = "ROCHA" };
        pk.Move1 = Tackle;
        pk.RefreshChecksum();
        var pidgey = await Store(pk, StorageFormat.Pk7);
        var options = await _pokemon.AdaptOptionsAsync(pidgey, default);
        Assert.Equal("species_not_in_game", options.Single(o => o.Target == StorageFormat.Pk9).Refusal);
        Assert.Null(options.Single(o => o.Target == StorageFormat.Pb8).Refusal); // o BDSP tem o Pidgey
        var ex = await Assert.ThrowsAsync<VaultException>(() => _pokemon.AdaptAsync(pidgey.Id, StorageFormat.Pk9, Owner, default));
        Assert.Equal("species_not_in_game", ex.Code);
    }

    [Fact]
    public void Restricoes_do_Virtual_Console_e_do_Poke_Transfer()
    {
        var mew = Pikachu1();
        mew.Species = 151;
        mew.RefreshChecksum();
        Assert.Equal("vc_mew_event", TransferChain.Adapt(mew, StorageFormat.Pk1, StorageFormat.Pk7, Owner).Refusal);

        var holder = new PK2 { Species = 25, CurrentLevel = 5, OriginalTrainerName = "GOLD", TID16 = 1, HeldItem = 1, Nickname = "PIKACHU" };
        holder.Move1 = ThunderShock;
        holder.RefreshChecksum();
        Assert.Equal("vc_held_item", TransferChain.Adapt(holder, StorageFormat.Pk2, StorageFormat.Pk7, Owner).Refusal);

        var cutter = new PK4 { Species = 25, CurrentLevel = 30, Version = GameVersion.HG, Language = 2, OriginalTrainerName = "ROCHA" };
        cutter.Move1 = 249; // Rock Smash: HM nos dois pares da Gen 4
        cutter.RefreshChecksum();
        Assert.Equal("poke_transfer_hm", TransferChain.Adapt(cutter, StorageFormat.Pk4, StorageFormat.Pk5, Owner).Refusal);
        cutter.Move1 = 432; // Defog: HM só no DPPt, sai pelo HGSS
        cutter.RefreshChecksum();
        Assert.True(TransferChain.Adapt(cutter, StorageFormat.Pk4, StorageFormat.Pk5, Owner).Ok);

        var kyurem = new PK5 { Species = 646, Form = 1, CurrentLevel = 70, Version = GameVersion.B2, Language = 2, OriginalTrainerName = "ROCHA" };
        kyurem.Move1 = Tackle;
        kyurem.RefreshChecksum();
        Assert.Equal("kyurem_fused", TransferChain.Adapt(kyurem, StorageFormat.Pk5, StorageFormat.Pk6, Owner).Refusal);
    }
}

/// <summary>As rotas de "Adaptar" pela API.</summary>
public sealed class AdaptApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-adapt-").FullName;
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        _app = TradeRockServer.Create(["--data-dir", _dir, "--Engines:Fake:SpeedFactor=0"], _token, b => b.WebHost.UseTestServer());
        await _app.StartAsync();
        _client = _app.GetTestClient();
        _client.BaseAddress = new Uri("http://127.0.0.1/");
        _client.DefaultRequestHeaders.Add(SessionAuthMiddleware.HeaderName, _token.Value);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    private static async Task<JsonElement> Json(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"{(int)response.StatusCode}: {text}");
        return JsonDocument.Parse(text).RootElement.Clone();
    }

    [Fact]
    public async Task Opcoes_e_adaptacao_pela_api_mantem_o_original()
    {
        var pk = AdaptTests.Pikachu1();
        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);
        using var form = new MultipartFormDataContent { { new ByteArrayContent(data), "file", "pikachu.pk1" } };
        var upload = await Json(await _client.PostAsync("/api/pokemon", form));
        var id = upload[0].GetProperty("pokemon").GetProperty("id").GetGuid();
        var before = (await Json(await _client.GetAsync("/api/vault/pokemon"))).GetArrayLength();

        var options = await Json(await _client.GetAsync($"/api/vault/pokemon/{id}/adapt-options"));
        var sv = options.EnumerateArray().Single(o => o.GetProperty("target").GetString() == "sv");
        Assert.Equal(JsonValueKind.Null, sv.GetProperty("refusal").ValueKind);
        Assert.Equal(["virtual_console", "home"], sv.GetProperty("steps").EnumerateArray().Select(s => s.GetString()));
        var gen4 = options.EnumerateArray().Single(o => o.GetProperty("target").GetString() == "gen4");
        Assert.Equal("no_route", gen4.GetProperty("refusal").GetString());

        var adapted = await Json(await _client.PostAsJsonAsync($"/api/vault/pokemon/{id}/adapt", new { target = "sv", language = "en" }));
        Assert.Equal("sv", adapted.GetProperty("pokemon").GetProperty("family").GetString());
        Assert.True(adapted.GetProperty("pokemon").GetProperty("tradable").GetBoolean());
        Assert.True(adapted.GetProperty("legality").GetProperty("valid").GetBoolean());

        // O original fica, a cópia entra: um a mais no cofre.
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync($"/api/pokemon/{id}")).StatusCode);
        Assert.Equal(before + 1, (await Json(await _client.GetAsync("/api/vault/pokemon"))).GetArrayLength());

        var bad = await _client.PostAsJsonAsync($"/api/vault/pokemon/{id}/adapt", new { target = "gen4" });
        Assert.Equal(HttpStatusCode.BadRequest, bad.StatusCode);
        Assert.Contains("no_route", await bad.Content.ReadAsStringAsync());
        var invalid = await _client.PostAsJsonAsync($"/api/vault/pokemon/{id}/adapt", new { target = "nope" });
        Assert.Contains("target_invalid", await invalid.Content.ReadAsStringAsync());
    }
}

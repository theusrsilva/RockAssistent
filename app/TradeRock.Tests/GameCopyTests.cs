using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using PKHeX.Core;
using TradeRock.App;
using TradeRock.App.Server;
using TradeRock.Core;
using TradeRock.Core.Builder;
using TradeRock.Core.Games;
using TradeRock.Core.Pokemon;
using TradeRock.Core.Storage;
using TradeRock.Core.Trading;

namespace TradeRock.Tests;

/// <summary>"Copiar para ser válido no jogo" (D36): um Pokémon novo, nascido no jogo de destino, com o que dá para copiar.</summary>
public sealed class GameCopyTests : IAsyncLifetime
{
    private const int Adamant = 3;
    private static readonly int[] Ivs = [31, 30, 29, 3, 28, 27];
    private static readonly TrainerSpec SwordTrainer = new("Treinadora", 654321, 1234, 1);

    private readonly TestDataDir _dir = new();
    private PokemonStore _pokemon = null!;

    public async ValueTask InitializeAsync() =>
        _pokemon = new PokemonStore(await _dir.CreateDbFactoryAsync(), _dir.Paths, TimeProvider.System);

    public ValueTask DisposeAsync()
    {
        _dir.Dispose();
        return ValueTask.CompletedTask;
    }

    /// <summary>Um Pokémon legal nascido no jogo, feito pelo construtor.</summary>
    internal static async Task<BuildResult> BornAsync(TradeGame game, ushort species, byte level, bool shiny = false, string? nickname = null)
    {
        var spec = new PokemonSpec
        {
            Species = species, Level = level, Game = game, Shiny = shiny, Nature = Adamant, Ivs = Ivs, Nickname = nickname,
            Trainer = new TrainerSpec("ROCHA", 123456, 1234),
        };
        var result = await PokemonBuilder.GenerateAsync(spec, "en", default);
        Assert.True(result.Usable, $"{result.Generation}: {string.Join("; ", result.Legality?.Problems.Select(p => p.Message) ?? [])}");
        return result;
    }

    [Fact]
    public async Task Pikachu_do_ZA_vira_uma_copia_nascida_no_Sword_Shield()
    {
        var born = await BornAsync(TradeGame.LegendsZA, 25, 30, shiny: true, nickname: "Faisca");
        var source = PokemonReader.ToEntity(born.Data!, GameFamily.Za);
        Assert.Equal(GameVersion.ZA, source.Version);
        Assert.True(source.IsShiny);
        int[] ivs = [source.IV_HP, source.IV_ATK, source.IV_DEF, source.IV_SPA, source.IV_SPD, source.IV_SPE];
        var original = (await _pokemon.ImportAsync(born.Data!, PokemonOrigin.Imported, "pikachu.pa9", default, GameFamily.Za)).Pokemon;

        var (stored, copy) = await _pokemon.CopyForGameAsync(original.Id, GameFamily.Swsh, SwordTrainer, "en", default);

        var pk = Assert.IsType<PK8>(copy.Pokemon);
        var report = LegalityReporter.Analyze(pk, "en");
        Assert.True(report.Status is LegalityStatus.Legal or LegalityStatus.Unverifiable, string.Join(" | ", report.Problems.Select(p => p.Message)));
        // Nasceu no Sword/Shield, com o "Meu treinador" de lá.
        Assert.Contains(pk.Version, new[] { GameVersion.SW, GameVersion.SH });
        Assert.Equal("Treinadora", pk.OriginalTrainerName);
        Assert.Equal(1, pk.OriginalTrainerGender);
        Assert.Equal((654321u, 1234u), TrainerIds.Display(GameCatalog.Get(GameFamily.Swsh), pk.TID16, pk.SID16));
        // O que o original tinha.
        Assert.Equal(25, pk.Species);
        Assert.Equal(source.Form, pk.Form);
        Assert.True(pk.IsShiny);
        Assert.Equal(source.Nature, pk.Nature);
        Assert.Equal(ivs, new[] { pk.IV_HP, pk.IV_ATK, pk.IV_DEF, pk.IV_SPA, pk.IV_SPD, pk.IV_SPE });
        Assert.Equal(source.Gender, pk.Gender);
        Assert.Equal(30, pk.CurrentLevel);
        Assert.Equal("Faisca", pk.Nickname);
        Assert.DoesNotContain("nature_changed", copy.Notes);
        Assert.DoesNotContain("ivs_changed", copy.Notes);
        // Encontro real do jogo de destino, sem passar do nível atual.
        Assert.NotNull(copy.Build!.Encounter);
        Assert.False(string.IsNullOrWhiteSpace(copy.Build.Encounter!.LocationName));
        Assert.True(pk.MetLevel <= pk.CurrentLevel);
        Assert.Equal(copy.Build.Encounter.Location, pk.MetLocation);
        // Todo golpe da cópia é do Sword/Shield (o que não existe lá é trocado, com a nota).
        Assert.All(pk.Moves.Where(m => m != 0), m => Assert.True(m <= GameCatalog.Get(GameFamily.Swsh).MaxMoveId));
        if (!pk.Moves.Where(m => m != 0).Order().SequenceEqual(source.Moves.Where(m => m != 0).Order()))
            Assert.Contains("moves_replaced", copy.Notes);

        // O original fica como estava; a cópia é outro registro, na Caixa de entrada.
        var kept = await _pokemon.GetAsync(original.Id, default);
        Assert.Equal(original.Sha256, kept!.Sha256);
        Assert.Equal(GameFamily.Za, kept.Family);
        Assert.Equal(born.Data!, await _pokemon.ReadDataAsync(kept, default));
        Assert.NotEqual(original.Id, stored.Id);
        Assert.Equal(GameFamily.Swsh, stored.Family);
        Assert.Null(stored.BoxId);
        Assert.Null(stored.DeletedAt);
        Assert.Equal(PokemonOrigin.Created, stored.Origin);
        Assert.NotEqual("illegal", stored.Legality);
        Assert.Equal(2, (await _pokemon.ListAsync(default)).Count);
    }

    [Fact]
    public async Task Recusas_tem_codigo_estavel()
    {
        // Sprigatito (Gen 9) não existe no Sword/Shield.
        var sprigatito = await BornAsync(TradeGame.Scarlet, 906, 10);
        var stored = (await _pokemon.ImportAsync(sprigatito.Data!, PokemonOrigin.Imported, "sprigatito.pk9", default, GameFamily.Sv)).Pokemon;
        var missing = await Assert.ThrowsAsync<GameCopyException>(() => _pokemon.CopyForGameAsync(stored.Id, GameFamily.Swsh, SwordTrainer, "en", default));
        Assert.Equal("species_not_in_game", missing.Code);

        var same = await Assert.ThrowsAsync<GameCopyException>(() =>
            _pokemon.CopyForGameAsync(stored.Id, GameFamily.Sv, new TrainerSpec("ROCHA", 123456, 1234), "en", default));
        Assert.Equal("same_game", same.Code);

        var none = await Assert.ThrowsAsync<GameCopyException>(() => _pokemon.CopyForGameAsync(Guid.NewGuid(), GameFamily.Swsh, SwordTrainer, "en", default));
        Assert.Equal("pokemon_not_found", none.Code);
        Assert.Single(await _pokemon.ListAsync(default)); // nada foi criado
    }

    [Fact]
    public async Task Sem_encontro_no_nivel_do_original_a_copia_e_recusada()
    {
        // Um Pikachu de nível 1 não tem encontro no Sword/Shield (lá ele aparece mais forte; de ovo nasce Pichu).
        var pk = SwitchPokemon.Make(GameFamily.Za);
        pk.CurrentLevel = 1;
        pk.RefreshChecksum();
        var copy = await GameCopy.CreateAsync(pk, GameFamily.Za, GameCatalog.Get(GameFamily.Swsh), SwordTrainer, "en", default);
        Assert.False(copy.Ok);
        Assert.Equal("copy_not_possible", copy.Refusal);
        Assert.False(string.IsNullOrEmpty(copy.Reason));

        var egg = SwitchPokemon.Make(GameFamily.Za);
        egg.IsEgg = true;
        Assert.Equal("egg", (await GameCopy.CreateAsync(egg, GameFamily.Za, GameCatalog.Get(GameFamily.Swsh), SwordTrainer, "en", default)).Refusal);
    }
}

/// <summary>A rota de "Copiar para ser válido no jogo" pela API.</summary>
public sealed class GameCopyApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-copy-").FullName;
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

    private async Task<Guid> UploadAsync(byte[] data, string name)
    {
        using var form = new MultipartFormDataContent { { new ByteArrayContent(data), "file", name } };
        return (await Json(await _client.PostAsync("/api/pokemon", form)))[0].GetProperty("pokemon").GetProperty("id").GetGuid();
    }

    [Fact]
    public async Task Copia_pela_api_usa_o_meu_treinador_do_destino_e_mantem_o_original()
    {
        var born = await GameCopyTests.BornAsync(TradeGame.LegendsZA, 25, 30);
        var id = await UploadAsync(born.Data!, "pikachu.pa9");
        await _client.PutAsJsonAsync("/api/settings/my-trainer?game=swsh", new { name = "Treinadora", tid = 654321, sid = 1234 });
        var before = (await Json(await _client.GetAsync("/api/vault/pokemon"))).GetArrayLength();

        var copied = await Json(await _client.PostAsJsonAsync($"/api/vault/pokemon/{id}/copy-for-game", new { game = "swsh", language = "en" }));
        var pokemon = copied.GetProperty("pokemon");
        Assert.NotEqual(id, pokemon.GetProperty("id").GetGuid());
        Assert.Equal("swsh", pokemon.GetProperty("family").GetString());
        Assert.Equal(25, pokemon.GetProperty("species").GetInt32());
        Assert.Equal(30, pokemon.GetProperty("level").GetInt32());
        Assert.Equal("Treinadora", pokemon.GetProperty("otName").GetString());
        Assert.Equal(654321u, pokemon.GetProperty("tid").GetUInt32());
        Assert.Equal(1234u, pokemon.GetProperty("sid").GetUInt32());
        Assert.Equal(JsonValueKind.Null, pokemon.GetProperty("boxId").ValueKind); // Caixa de entrada
        Assert.NotEqual("illegal", pokemon.GetProperty("legality").GetString());
        Assert.True(copied.GetProperty("legality").GetProperty("valid").GetBoolean());
        Assert.Equal(JsonValueKind.Array, copied.GetProperty("notes").ValueKind);
        var met = copied.GetProperty("met");
        Assert.False(string.IsNullOrWhiteSpace(met.GetProperty("location").GetString()));
        Assert.InRange(met.GetProperty("level").GetInt32(), 1, 30);
        Assert.Contains(met.GetProperty("version").GetString(), new[] { "SW", "SH" });
        Assert.EndsWith("Ball", met.GetProperty("ball").GetString()); // nome inteiro, como "Poké Ball"

        // O original fica (ainda do Z-A), a cópia entra: um a mais no cofre.
        var original = await Json(await _client.GetAsync($"/api/pokemon/{id}"));
        Assert.Equal("za", original.GetProperty("family").GetString());
        Assert.Equal(before + 1, (await Json(await _client.GetAsync("/api/vault/pokemon"))).GetArrayLength());
    }

    [Fact]
    public async Task Recusas_pela_api()
    {
        var sprigatito = await GameCopyTests.BornAsync(TradeGame.Scarlet, 906, 10);
        var id = await UploadAsync(sprigatito.Data!, "sprigatito.pk9");
        var before = (await Json(await _client.GetAsync("/api/vault/pokemon"))).GetArrayLength();

        var missing = await _client.PostAsJsonAsync($"/api/vault/pokemon/{id}/copy-for-game", new { game = "swsh" });
        Assert.Equal(HttpStatusCode.BadRequest, missing.StatusCode);
        Assert.Equal("species_not_in_game", (await missing.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("error").GetString());

        var same = await _client.PostAsJsonAsync($"/api/vault/pokemon/{id}/copy-for-game", new { game = "sv" });
        Assert.Contains("same_game", await same.Content.ReadAsStringAsync());
        var invalid = await _client.PostAsJsonAsync($"/api/vault/pokemon/{id}/copy-for-game", new { game = "nope" });
        Assert.Contains("game_invalid", await invalid.Content.ReadAsStringAsync());
        var none = await _client.PostAsJsonAsync($"/api/vault/pokemon/{Guid.NewGuid()}/copy-for-game", new { game = "swsh" });
        Assert.Equal(HttpStatusCode.NotFound, none.StatusCode);

        Assert.Equal(before, (await Json(await _client.GetAsync("/api/vault/pokemon"))).GetArrayLength());
    }
}

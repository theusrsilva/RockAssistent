using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using PKHeX.Core;
using TradeRock.App;
using TradeRock.Core;
using TradeRock.Core.Builder;
using TradeRock.Core.Games;
using TradeRock.Core.Pokemon;
using TradeRock.Core.Storage;
using TradeRock.Core.Trading;

namespace TradeRock.Tests;

/// <summary>Pokémon legais de teste e a ida ao Sword/Shield, para os testes do original guardado (D37).</summary>
internal static class KeptOriginalSamples
{
    public static readonly SimpleTrainerInfo Owner = new(GameVersion.SL) { OT = "ROCHA", TID16 = 0xE240, SID16 = 1, Language = 2 };

    public static readonly SwitchGame Swsh = GameCatalog.Get(GameFamily.Swsh);

    public static async Task<PKM> LegalAsync(TradeGame game, ushort species = 25, byte level = 30)
    {
        var trainer = GameCatalog.Of(game).SixDigitTrainerId ? new TrainerSpec("ROCHA", 123456, 1234) : new TrainerSpec("ROCHA", 24680, 13579);
        var result = await PokemonBuilder.GenerateAsync(new PokemonSpec { Species = species, Level = level, Game = game, Trainer = trainer }, "en", default);
        Assert.True(result.Usable);
        return PokemonReader.ToEntity(result.Data!, GameCatalog.Of(game).Family);
    }

    /// <summary>O Pokémon como chega ao Sword/Shield (com a origem perdida).</summary>
    public static PK8 InSword(PKM pk)
    {
        var moved = HomeTransfer.Move(pk, Swsh, Owner);
        Assert.True(moved.Ok, moved.Refusal);
        Assert.True(HomeTransfer.OriginLostInSwsh(moved.Pokemon!));
        return (PK8)moved.Pokemon!;
    }

    public static string Problems(PKM pk) =>
        string.Join(" | ", LegalityReporter.Analyze(pk, "en").Problems.Select(p => $"{p.Identifier}/{p.Code}"));

    public static void AssertNotIllegal(PKM pk, string path) =>
        Assert.True(LegalityReporter.Analyze(pk, "en").Status != LegalityStatus.Illegal, $"{path}: {Problems(pk)}");
}

/// <summary>Saída do Sword/Shield a partir do original guardado (<see cref="KeptOriginal"/>, D37).</summary>
public sealed class KeptOriginalTests
{
    private static readonly SimpleTrainerInfo Owner = KeptOriginalSamples.Owner;

    [Theory]
    [InlineData(TradeGame.Scarlet)]
    [InlineData(TradeGame.BrilliantDiamond)]
    [InlineData(TradeGame.LegendsArceus)]
    public async Task Volta_ao_proprio_jogo_com_o_encontro_do_original(TradeGame origin)
    {
        var pk = await KeptOriginalSamples.LegalAsync(origin, 25, 50);
        var family = GameCatalog.Of(origin).Family;
        var inSword = KeptOriginalSamples.InSword(pk);

        var back = KeptOriginal.Move(inSword, pk, family, GameCatalog.Get(family), Owner);
        Assert.NotNull(back);
        Assert.True(back.Ok, back.Refusal);
        Assert.Equal([KeptOriginal.RestoredNote], back.Notes);
        var result = back.Pokemon!;
        Assert.Equal(pk.GetType(), result.GetType());
        Assert.Equal(pk.Version, result.Version);
        Assert.Equal(pk.MetLocation, result.MetLocation);
        Assert.Equal(pk.MetLevel, result.MetLevel);
        Assert.Equal(pk.EggLocation, result.EggLocation);
        Assert.Equal(pk.Ball, result.Ball);
        Assert.Equal(pk.Moves, result.Moves);
        KeptOriginalSamples.AssertNotIllegal(result, $"{family} > Swsh > {family}");
    }

    /// <summary>
    /// De qualquer origem que o Sword/Shield apaga, para todos os jogos: a lista de destinos e o mover concordam, a
    /// saída é a mesma de quem nunca passou pelo Sword/Shield e nada sai ilegal.
    /// </summary>
    [Theory]
    [InlineData(TradeGame.Scarlet, 25)]
    [InlineData(TradeGame.Scarlet, 133)]
    [InlineData(TradeGame.BrilliantDiamond, 25)]
    [InlineData(TradeGame.BrilliantDiamond, 448)]
    [InlineData(TradeGame.LegendsArceus, 25)]
    [InlineData(TradeGame.LegendsArceus, 133)]
    public async Task Destinos_e_mover_concordam_a_partir_do_original(TradeGame origin, ushort species)
    {
        var pk = await KeptOriginalSamples.LegalAsync(origin, species, 50);
        var family = GameCatalog.Of(origin).Family;
        var inSword = KeptOriginalSamples.InSword(pk);

        var destinations = KeptOriginal.Destinations(inSword, pk, family);
        Assert.NotNull(destinations);
        Assert.Equal("same_game", destinations[GameFamily.Swsh]);
        Assert.Equal("home_no_destination", destinations[GameFamily.Lgpe]);
        Assert.Null(destinations[family]);
        var left = 0;
        foreach (var game in GameCatalog.All.Where(g => g.Family != GameFamily.Swsh))
        {
            var moved = KeptOriginal.Move(inSword, pk, family, game, Owner);
            Assert.NotNull(moved);
            Assert.Equal(destinations[game.Family], moved.Refusal);
            if (game.Family != family)
                Assert.Equal(HomeTransfer.Move(pk, game, Owner).Refusal, moved.Refusal);
            if (!moved.Ok)
                continue;
            left++;
            Assert.Contains(KeptOriginal.RestoredNote, moved.Notes);
            Assert.DoesNotContain(moved.Notes, n => n.StartsWith(KeptOriginal.DroppedPrefix, StringComparison.Ordinal));
            Assert.False(HomeTransfer.OriginLostInSwsh(moved.Pokemon!));
            Assert.Equal(pk.Version, moved.Pokemon!.Version);
            KeptOriginalSamples.AssertNotIllegal(moved.Pokemon, $"{family} > Swsh > {game.Family}");
        }
        Assert.True(left >= 2, "o próprio jogo e pelo menos mais um destino");
    }

    [Theory]
    [InlineData(TradeGame.BrilliantDiamond, GameFamily.Sv)]
    [InlineData(TradeGame.LegendsArceus, GameFamily.Bdsp)]
    public async Task Sai_para_outro_jogo_sem_ficar_ilegal(TradeGame origin, GameFamily to)
    {
        var pk = await KeptOriginalSamples.LegalAsync(origin, 25, 50);
        var inSword = KeptOriginalSamples.InSword(pk);
        // Sem o original, a saída é recusada.
        Assert.Equal("origin_lost_in_swsh", HomeTransfer.Move(inSword, GameCatalog.Get(to), Owner).Refusal);

        var moved = KeptOriginal.Move(inSword, pk, GameCatalog.Of(origin).Family, GameCatalog.Get(to), Owner);
        Assert.NotNull(moved);
        Assert.True(moved.Ok, moved.Refusal);
        Assert.Equal(GameCatalog.Get(to).EntityType, moved.Pokemon!.GetType());
        Assert.Equal(pk.Version, moved.Pokemon.Version);
        KeptOriginalSamples.AssertNotIllegal(moved.Pokemon, $"{origin} > Swsh > {to}");
    }

    [Fact]
    public async Task Mudancas_legais_vao_junto_e_a_ilegal_e_descartada_com_nota()
    {
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Scarlet, 25, 30);
        var inSword = KeptOriginalSamples.InSword(pk);

        // No Sword/Shield: subiu de nível, ganhou EVs, apelido e item, aprendeu Thunderbolt (vale no Scarlet) e ficou
        // com um golpe que o Pikachu não aprende em jogo nenhum (Spore).
        const ushort thunderbolt = 85, spore = 147, leftovers = 234;
        inSword.CurrentLevel = 60;
        inSword.EV_HP = 100;
        inSword.EV_SPE = 252;
        inSword.Nickname = "Faisca";
        inSword.IsNicknamed = true;
        inSword.HeldItem = leftovers;
        inSword.Move1 = thunderbolt;
        inSword.Move2 = spore;
        inSword.Move3 = 0;
        inSword.Move4 = 0;
        inSword.RefreshChecksum();

        var back = KeptOriginal.Move(inSword, pk, GameFamily.Sv, GameCatalog.Get(GameFamily.Sv), Owner);
        Assert.NotNull(back);
        Assert.True(back.Ok, back.Refusal);
        var result = back.Pokemon!;
        Assert.Equal(60, result.CurrentLevel);
        Assert.Equal(100, result.EV_HP);
        Assert.Equal(252, result.EV_SPE);
        Assert.Equal("Faisca", result.Nickname);
        Assert.True(result.IsNicknamed);
        Assert.Equal(leftovers, result.HeldItem);
        Assert.Contains(thunderbolt, result.Moves);
        Assert.DoesNotContain(spore, result.Moves);
        Assert.Equal([KeptOriginal.RestoredNote, KeptOriginal.DroppedPrefix + "moves"], back.Notes);
        // O encontro continua o do original.
        Assert.Equal(pk.Version, result.Version);
        Assert.Equal(pk.MetLocation, result.MetLocation);
        Assert.Equal(pk.MetLevel, result.MetLevel);
        KeptOriginalSamples.AssertNotIllegal(result, "Sv > Swsh (mudou) > Sv");
    }

    [Fact]
    public async Task Mudanca_que_deixaria_ilegal_fica_de_fora_e_o_resto_sai_como_era()
    {
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Scarlet, 25, 30);
        var inSword = KeptOriginalSamples.InSword(pk);
        // Hyper Training no nível 30 e EVs acima do total: nenhum dos dois pode ser levado.
        inSword.HT_HP = true;
        inSword.SetEVs([252, 252, 252, 252, 252, 252]);
        inSword.RefreshChecksum();

        var back = KeptOriginal.Move(inSword, pk, GameFamily.Sv, GameCatalog.Get(GameFamily.Sv), Owner);
        Assert.NotNull(back);
        Assert.True(back.Ok, back.Refusal);
        Assert.Equal([KeptOriginal.RestoredNote, KeptOriginal.DroppedPrefix + "evs", KeptOriginal.DroppedPrefix + "hyper_training"], back.Notes);
        Assert.Equal(pk.EVTotal, back.Pokemon!.EVTotal);
        Assert.False(((IHyperTrain)back.Pokemon).HT_HP);
        // Sem nenhuma mudança levada, sai igual ao que era antes de ir (só o treinador atual é acertado).
        Assert.Equal(pk.Moves, back.Pokemon.Moves);
        Assert.Equal(pk.EXP, back.Pokemon.EXP);
        KeptOriginalSamples.AssertNotIllegal(back.Pokemon, "Sv > Swsh (mudança ilegal) > Sv");
    }

    /// <summary>
    /// Evolução feita no Sword/Shield (Pikachu → Raichu). A linha existe no Scarlet/Violet e no BDSP, então a análise
    /// aceita a espécie nova sobre o encontro original e a evolução vai junto.
    /// </summary>
    [Theory]
    [InlineData(TradeGame.Scarlet, GameFamily.Sv)]
    [InlineData(TradeGame.Scarlet, GameFamily.Bdsp)]
    [InlineData(TradeGame.BrilliantDiamond, GameFamily.Bdsp)]
    [InlineData(TradeGame.BrilliantDiamond, GameFamily.Sv)]
    public async Task Evolucao_feita_no_sword_vai_junto_quando_a_analise_aceita(TradeGame origin, GameFamily to)
    {
        var pk = await KeptOriginalSamples.LegalAsync(origin, 25, 30);
        var evolved = (PK8)TradeEvolutionLike(KeptOriginalSamples.InSword(pk), 26);
        Assert.True(KeptOriginal.SamePokemon(evolved, pk));

        var moved = KeptOriginal.Move(evolved, pk, GameCatalog.Of(origin).Family, GameCatalog.Get(to), Owner);
        Assert.NotNull(moved);
        Assert.True(moved.Ok, moved.Refusal);
        Assert.Equal(26, moved.Pokemon!.Species);
        Assert.DoesNotContain(KeptOriginal.DroppedPrefix + "species", moved.Notes);
        Assert.Equal(pk.Version, moved.Pokemon.Version);
        Assert.Equal(pk.MetLevel, moved.Pokemon.MetLevel);
        KeptOriginalSamples.AssertNotIllegal(moved.Pokemon, $"{origin} > Swsh (evoluiu) > {to}");
    }

    /// <summary>Evolução para uma espécie que o destino não tem: sai como a espécie de antes, com a nota.</summary>
    [Fact]
    public async Task Evolucao_que_o_destino_nao_tem_e_descartada_com_nota()
    {
        // Eevee do Scarlet evolui no Sword; o Arceus não recebe do Scarlet, então o destino sem a espécie é simulado
        // com a própria regra: o Legends: Arceus tem Eevee e Vaporeon, o Z-A não tem a linha. Aqui basta o BDSP, que
        // tem a linha toda menos as formas novas: confere-se o caminho pela forma que o BDSP não tem.
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Scarlet, 133, 30);
        var evolved = (PK8)TradeEvolutionLike(KeptOriginalSamples.InSword(pk), 700); // Sylveon: o BDSP não tem
        Assert.False(GameCatalog.Get(GameFamily.Bdsp).HasSpecies(700, 0));

        var moved = KeptOriginal.Move(evolved, pk, GameFamily.Sv, GameCatalog.Get(GameFamily.Bdsp), Owner);
        Assert.NotNull(moved);
        Assert.True(moved.Ok, moved.Refusal);
        Assert.Equal(133, moved.Pokemon!.Species);
        Assert.Contains(KeptOriginal.DroppedPrefix + "species", moved.Notes);
        KeptOriginalSamples.AssertNotIllegal(moved.Pokemon, "Sv > Swsh (Sylveon) > Bdsp");
    }

    [Fact]
    public async Task Outro_pokemon_no_lugar_nao_usa_o_original()
    {
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Scarlet, 25, 30);
        var inSword = KeptOriginalSamples.InSword(pk);
        Assert.True(KeptOriginal.SamePokemon(inSword, pk));

        var otherPid = (PK8)inSword.Clone();
        otherPid.PID ^= 0x10;
        Assert.False(KeptOriginal.SamePokemon(otherPid, pk));
        Assert.Null(KeptOriginal.Move(otherPid, pk, GameFamily.Sv, GameCatalog.Get(GameFamily.Sv), Owner));
        Assert.Null(KeptOriginal.Destinations(otherPid, pk, GameFamily.Sv));

        var otherConstant = (PK8)inSword.Clone();
        otherConstant.EncryptionConstant ^= 0x10;
        Assert.False(KeptOriginal.SamePokemon(otherConstant, pk));

        var otherSpecies = (PK8)inSword.Clone();
        otherSpecies.Species = 133; // fora da linha do Pikachu
        Assert.False(KeptOriginal.SamePokemon(otherSpecies, pk));

        var otherTrainer = (PK8)inSword.Clone();
        otherTrainer.TID16 ^= 1;
        Assert.False(KeptOriginal.SamePokemon(otherTrainer, pk));

        // Um original que não é de antes do Sword/Shield (o próprio PK8) também não serve.
        Assert.False(KeptOriginal.SamePokemon(inSword, inSword));
    }

    /// <summary>Evolui como o jogo faria: espécie nova, nome novo (sem apelido) e a habilidade no mesmo índice.</summary>
    internal static PKM TradeEvolutionLike(PKM pk, ushort species)
    {
        var result = pk.Clone();
        var ability = result.AbilityNumber switch { 2 => 1, 4 => 2, _ => 0 };
        result.Species = species;
        if (!result.IsNicknamed)
            result.ClearNickname();
        result.RefreshAbility(ability);
        result.ResetPartyStats();
        result.RefreshChecksum();
        return result;
    }
}

/// <summary>O cofre guarda o original de antes da ida ao Sword/Shield e o usa na saída (D37).</summary>
public sealed class OriginalKeptStoreTests : IAsyncLifetime
{
    private static readonly SimpleTrainerInfo Owner = KeptOriginalSamples.Owner;
    private readonly TestDataDir _dir = new();
    private IDbContextFactory<TradeRockDb> _db = null!;
    private PokemonStore _store = null!;
    private VaultStore _vault = null!;

    public async ValueTask InitializeAsync()
    {
        _db = await _dir.CreateDbFactoryAsync();
        _store = new PokemonStore(_db, _dir.Paths, TimeProvider.System);
        _vault = new VaultStore(_db, _store, TimeProvider.System);
    }

    public ValueTask DisposeAsync()
    {
        _dir.Dispose();
        return ValueTask.CompletedTask;
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private async Task<StoredPokemon> ImportAsync(PKM pk, GameFamily family) =>
        (await _store.ImportAsync(SwitchPokemon.Stored(pk), PokemonOrigin.Imported, null, Ct, family)).Pokemon;

    private async Task<PKM> EntityAsync(StoredPokemon stored) =>
        PokemonReader.ToEntity(await _store.ReadDataAsync(stored, Ct), stored.Format);

    private int VaultFiles() => Directory.EnumerateFiles(_dir.Paths.VaultDir, "*", SearchOption.AllDirectories).Count();

    [Fact]
    public async Task Do_scarlet_ao_sword_e_de_volta_com_o_encontro_original()
    {
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Scarlet, 25, 50);
        var stored = await ImportAsync(pk, GameFamily.Sv);
        var originalSha = stored.Sha256;
        Assert.True(await _store.KeepsOriginalInSwshAsync(stored, Ct));

        var (inSword, notes) = await _store.TransferAsync(stored.Id, GameFamily.Swsh, Owner, Ct);
        Assert.Contains(KeptOriginal.KeptNote, notes);
        Assert.Equal(GameFamily.Swsh, inSword.Family);
        Assert.Equal(originalSha, inSword.OriginalSha256);
        Assert.Equal(StorageFormat.Pk9, inSword.OriginalFormat);
        // O arquivo do original continua no disco, mesmo sem registro com esse hash como Pokémon atual.
        Assert.True(File.Exists(_store.FilePath(originalSha, StorageFormat.Pk9)));
        Assert.Equal(2, VaultFiles());
        Assert.True(HomeTransfer.OriginLostInSwsh(await EntityAsync(inSword)));
        Assert.False(await _store.KeepsOriginalInSwshAsync(inSword, Ct));

        // A lista e o mover concordam: com o original guardado, todo destino que aparece como "pode" funciona.
        var destinations = await _store.DestinationsAsync(inSword, Ct);
        Assert.Equal("same_game", destinations[GameFamily.Swsh]);
        Assert.Null(destinations[GameFamily.Sv]);
        Assert.Null(destinations[GameFamily.Bdsp]);

        var swordSha = inSword.Sha256;
        var (back, backNotes) = await _store.TransferAsync(stored.Id, GameFamily.Sv, Owner, Ct);
        Assert.Contains(KeptOriginal.RestoredNote, backNotes);
        Assert.Equal(stored.Id, back.Id);
        Assert.Equal(GameFamily.Sv, back.Family);
        Assert.Null(back.OriginalSha256);
        Assert.Null(back.OriginalFormat);
        Assert.NotEqual("illegal", back.Legality);
        var result = await EntityAsync(back);
        Assert.Equal(pk.Version, result.Version);
        Assert.Equal(pk.MetLocation, result.MetLocation);
        Assert.Equal(pk.MetLevel, result.MetLevel);
        // O PK8 saiu do disco; fica só o arquivo de agora.
        Assert.False(File.Exists(_store.FilePath(swordSha, StorageFormat.Pk8)));
        Assert.Equal(1, VaultFiles());
    }

    [Theory]
    [InlineData(TradeGame.BrilliantDiamond, GameFamily.Sv)]
    [InlineData(TradeGame.LegendsArceus, GameFamily.Bdsp)]
    public async Task Sai_do_sword_para_outro_jogo_sem_ficar_ilegal(TradeGame origin, GameFamily to)
    {
        var pk = await KeptOriginalSamples.LegalAsync(origin, 25, 50);
        var stored = await ImportAsync(pk, GameCatalog.Of(origin).Family);
        var (inSword, _) = await _store.TransferAsync(stored.Id, GameFamily.Swsh, Owner, Ct);
        Assert.NotNull(inSword.OriginalSha256);
        Assert.Null((await _store.DestinationsAsync(inSword, Ct))[to]);

        var (moved, notes) = await _store.TransferAsync(stored.Id, to, Owner, Ct);
        Assert.Contains(KeptOriginal.RestoredNote, notes);
        Assert.Equal(to, moved.Family);
        Assert.Null(moved.OriginalSha256);
        Assert.NotEqual("illegal", moved.Legality);
        var result = await EntityAsync(moved);
        Assert.Equal(pk.Version, result.Version);
        KeptOriginalSamples.AssertNotIllegal(result, $"{origin} > Swsh > {to}");
        // O original e o PK8 saíram do disco: ninguém mais os usa.
        Assert.Equal(1, VaultFiles());
    }

    [Fact]
    public async Task Editado_no_sword_volta_atualizado()
    {
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Scarlet, 25, 30);
        var stored = await ImportAsync(pk, GameFamily.Sv);
        var (inSword, _) = await _store.TransferAsync(stored.Id, GameFamily.Swsh, Owner, Ct);

        // Edição do cofre (/replace) sobre o mesmo Pokémon: nível e golpes novos.
        const ushort thunderbolt = 85, spore = 147;
        var edited = await EntityAsync(inSword);
        edited.CurrentLevel = 70;
        edited.Move1 = thunderbolt;
        edited.Move2 = spore;
        edited.RefreshChecksum();
        var replaced = await _store.ReplaceAsync(stored.Id, SwitchPokemon.Stored(edited), Ct);
        Assert.Equal(stored.Sha256, replaced.OriginalSha256); // continua o mesmo Pokémon: o original vale

        var (back, notes) = await _store.TransferAsync(stored.Id, GameFamily.Sv, Owner, Ct);
        Assert.Equal([KeptOriginal.RestoredNote, KeptOriginal.DroppedPrefix + "moves"], notes);
        Assert.Equal(70, back.Level);
        var result = await EntityAsync(back);
        Assert.Contains(thunderbolt, result.Moves);
        Assert.DoesNotContain(spore, result.Moves);
        Assert.Equal(pk.MetLocation, result.MetLocation);
        Assert.NotEqual("illegal", back.Legality);
    }

    [Fact]
    public async Task Sem_original_guardado_continua_recusado()
    {
        // Um PK8 com os códigos do HOME que veio de um arquivo: o cofre não tem o original dele.
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Scarlet, 25, 50);
        var stored = await ImportAsync(KeptOriginalSamples.InSword(pk), GameFamily.Swsh);
        Assert.Null(stored.OriginalSha256);

        var destinations = await _store.DestinationsAsync(stored, Ct);
        foreach (var family in new[] { GameFamily.Bdsp, GameFamily.Pla, GameFamily.Sv, GameFamily.Za })
        {
            Assert.Equal("origin_lost_in_swsh", destinations[family]);
            var refused = await Assert.ThrowsAsync<VaultException>(() => _store.TransferAsync(stored.Id, family, Owner, Ct));
            Assert.Equal("origin_lost_in_swsh", refused.Code);
        }
    }

    [Fact]
    public async Task Substituido_por_outro_pokemon_o_original_deixa_de_valer()
    {
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Scarlet, 25, 50);
        var stored = await ImportAsync(pk, GameFamily.Sv);
        var (inSword, _) = await _store.TransferAsync(stored.Id, GameFamily.Swsh, Owner, Ct);
        var originalPath = _store.FilePath(inSword.OriginalSha256!, StorageFormat.Pk9);
        Assert.True(File.Exists(originalPath));

        // A edição põe outro Pokémon no registro (PID e constante diferentes), também com a origem perdida.
        var other = await EntityAsync(inSword);
        other.PID ^= 0x10;
        other.EncryptionConstant ^= 0x10;
        other.RefreshChecksum();
        var replaced = await _store.ReplaceAsync(stored.Id, SwitchPokemon.Stored(other), Ct);
        Assert.Null(replaced.OriginalSha256);
        Assert.Null(replaced.OriginalFormat);
        Assert.False(File.Exists(originalPath));

        Assert.Equal("origin_lost_in_swsh", (await _store.DestinationsAsync(replaced, Ct))[GameFamily.Sv]);
        var refused = await Assert.ThrowsAsync<VaultException>(() => _store.TransferAsync(stored.Id, GameFamily.Sv, Owner, Ct));
        Assert.Equal("origin_lost_in_swsh", refused.Code);
    }

    [Fact]
    public async Task Arquivo_do_original_sumido_ou_adulterado_nao_e_usado()
    {
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Scarlet, 25, 50);
        var stored = await ImportAsync(pk, GameFamily.Sv);
        var (inSword, _) = await _store.TransferAsync(stored.Id, GameFamily.Swsh, Owner, Ct);
        var originalPath = _store.FilePath(inSword.OriginalSha256!, StorageFormat.Pk9);

        var bytes = await File.ReadAllBytesAsync(originalPath, Ct);
        bytes[0x20] ^= 0xFF;
        await File.WriteAllBytesAsync(originalPath, bytes, Ct);
        Assert.Equal("origin_lost_in_swsh", (await _store.DestinationsAsync(inSword, Ct))[GameFamily.Sv]);

        File.Delete(originalPath);
        Assert.Equal("origin_lost_in_swsh", (await _store.DestinationsAsync(inSword, Ct))[GameFamily.Sv]);
        var refused = await Assert.ThrowsAsync<VaultException>(() => _store.TransferAsync(stored.Id, GameFamily.Sv, Owner, Ct));
        Assert.Equal("origin_lost_in_swsh", refused.Code);
    }

    [Fact]
    public async Task O_original_so_sai_do_disco_quando_o_registro_e_apagado_de_vez()
    {
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Scarlet, 25, 50);
        var stored = await ImportAsync(pk, GameFamily.Sv);
        var (inSword, _) = await _store.TransferAsync(stored.Id, GameFamily.Swsh, Owner, Ct);
        var originalPath = _store.FilePath(inSword.OriginalSha256!, StorageFormat.Pk9);
        var swordPath = _store.FilePath(inSword.Sha256, StorageFormat.Pk8);

        // Outro Pokémon apagado de vez não leva o original junto.
        var other = await ImportAsync(await KeptOriginalSamples.LegalAsync(TradeGame.Sword, 133, 20), GameFamily.Swsh);
        await _vault.TrashAsync([other.Id], Ct);
        await _vault.EmptyTrashAsync(Ct);
        Assert.True(File.Exists(originalPath));

        // Na lixeira o original continua (dá para recuperar e sair do Sword/Shield depois).
        await _vault.TrashAsync([stored.Id], Ct);
        Assert.True(File.Exists(originalPath));
        Assert.True(File.Exists(swordPath));
        await _vault.RestoreAsync([stored.Id], Ct);
        var restored = await _store.GetAsync(stored.Id, Ct);
        Assert.Null((await _store.DestinationsAsync(restored!, Ct))[GameFamily.Sv]);

        // Apagado de vez: os dois arquivos somem.
        await _vault.TrashAsync([stored.Id], Ct);
        var emptied = await _vault.EmptyTrashAsync(Ct);
        Assert.Equal(1, emptied.Deleted);
        Assert.False(File.Exists(originalPath));
        Assert.False(File.Exists(swordPath));
        Assert.Equal(0, VaultFiles());
    }

    [Fact]
    public async Task O_original_que_tambem_e_outro_registro_nao_e_apagado()
    {
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Scarlet, 25, 50);
        var stored = await ImportAsync(pk, GameFamily.Sv);
        var (inSword, _) = await _store.TransferAsync(stored.Id, GameFamily.Swsh, Owner, Ct);
        var originalPath = _store.FilePath(inSword.OriginalSha256!, StorageFormat.Pk9);

        // O mesmo arquivo importado de novo vira outro registro, que usa o mesmo arquivo em disco.
        var again = await ImportAsync(pk, GameFamily.Sv);
        Assert.NotEqual(stored.Id, again.Id);

        await _vault.TrashAsync([stored.Id], Ct);
        await _vault.EmptyTrashAsync(Ct);
        Assert.True(File.Exists(originalPath));
        Assert.Equal(pk.MetLocation, (await EntityAsync(again)).MetLocation);
    }

    [Fact]
    public async Task Do_sword_para_o_sword_nada_e_guardado()
    {
        // Nascido no Sword: a ida a outro jogo e a volta não perdem a origem, então não há original guardado.
        var pk = await KeptOriginalSamples.LegalAsync(TradeGame.Sword, 25, 50);
        var stored = await ImportAsync(pk, GameFamily.Swsh);
        var (inScarlet, notes) = await _store.TransferAsync(stored.Id, GameFamily.Sv, Owner, Ct);
        Assert.DoesNotContain(KeptOriginal.KeptNote, notes);
        Assert.Null(inScarlet.OriginalSha256);
        Assert.False(await _store.KeepsOriginalInSwshAsync(inScarlet, Ct));

        var (back, backNotes) = await _store.TransferAsync(stored.Id, GameFamily.Swsh, Owner, Ct);
        Assert.DoesNotContain(KeptOriginal.KeptNote, backNotes);
        Assert.Null(back.OriginalSha256);
        Assert.Equal(1, VaultFiles());
    }
}

public sealed class KeptOriginalMigrationTests
{
    [Fact]
    public async Task Migracao_sobre_banco_antigo_mantem_o_cofre_sem_original()
    {
        using var dir = new TestDataDir();
        var options = new DbContextOptionsBuilder<TradeRockDb>()
            .UseSqlite($"Data Source={dir.Paths.DatabasePath};Pooling=False").Options;
        var ct = TestContext.Current.CancellationToken;

        await using (var db = new TradeRockDb(options))
        {
            // Banco de antes da D37: a tabela ainda não tem as colunas do original.
            await db.GetService<IMigrator>().MigrateAsync("MonIsStarter", ct);
            await db.Database.ExecuteSqlRawAsync(
                """
                INSERT INTO "Pokemon" ("Id", "OwnerId", "Sha256", "Size", "Format", "Species", "Form", "SpeciesName", "Nickname",
                    "Level", "IsShiny", "IsEgg", "Gender", "OtName", "Tid", "Sid", "OriginGame", "HeldItem", "Fateful", "Origin", "AddedAt")
                VALUES ({0}, {1}, 'ab', 344, 'Pk8', 25, 0, 'Pikachu', 'PIKA', 5, 0, 0, 0, 'RED', 1, 2, 'Sword', 0, 0, 'Imported', {2})
                """,
                [Guid.NewGuid().ToString().ToUpperInvariant(), OwnerScope.LocalOwner.ToString().ToUpperInvariant(),
                    new DateTimeOffsetToBinaryConverter().ConvertToProvider(DateTimeOffset.UtcNow)!], ct);
        }

        await using (var db = new TradeRockDb(options))
        {
            await db.Database.MigrateAsync(ct);
            var pokemon = await db.Pokemon.SingleAsync(ct);
            Assert.Equal(StorageFormat.Pk8, pokemon.Format);
            Assert.Null(pokemon.OriginalSha256);
            Assert.Null(pokemon.OriginalFormat);

            pokemon.OriginalSha256 = "cd";
            pokemon.OriginalFormat = StorageFormat.Pk9;
            await db.SaveChangesAsync(ct);
        }

        await using (var db = new TradeRockDb(options))
        {
            var pokemon = await db.Pokemon.SingleAsync(ct);
            Assert.Equal("cd", pokemon.OriginalSha256);
            Assert.Equal(StorageFormat.Pk9, pokemon.OriginalFormat);
        }
    }
}

/// <summary>O original guardado pela API do cofre (D37).</summary>
public sealed class OriginalKeptApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private WebApplication _app = null!;
    private string _dataDir = null!;
    private HttpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        _dataDir = Directory.CreateTempSubdirectory("traderock-original-").FullName;
        _app = TradeRockServer.Create(
            ["--data-dir", _dataDir, "--Engines:Fake:SpeedFactor=0"], _token, b => b.WebHost.UseTestServer());
        await _app.StartAsync();
        _client = _app.GetTestClient();
        _client.BaseAddress = new Uri("http://127.0.0.1/");
        _client.DefaultRequestHeaders.Add(SessionAuthMiddleware.HeaderName, _token.Value);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
        try
        {
            Directory.Delete(_dataDir, recursive: true);
        }
        catch (IOException)
        {
        }
    }

    private async Task<JsonElement> DestinationAsync(Guid id, string game)
    {
        var destinations = await _client.GetFromJsonAsync<JsonElement>($"/api/vault/pokemon/{id}/destinations");
        return destinations.EnumerateArray().Single(d => d.GetProperty("game").GetString() == game);
    }

    private static string[] Notes(JsonElement body) =>
        [.. body.GetProperty("notes").EnumerateArray().Select(n => n.GetString()!)];

    [Fact]
    public async Task Do_scarlet_ao_sword_e_de_volta_pela_api()
    {
        var generated = await _client.PostAsJsonAsync("/api/builder/generate", new
        {
            spec = new { species = 25, level = 30, game = "scarlet", trainer = new { name = "ROCHA", tid = 123456, sid = 1234 } },
        });
        var data = (await generated.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("data").GetString();
        var saved = await _client.PostAsJsonAsync("/api/builder/save", new { data, game = "scarlet" });
        var id = (await saved.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("pokemon").GetProperty("id").GetGuid();
        var before = PokemonReader.ToEntity(Convert.FromBase64String(data!), GameFamily.Sv);

        // A lista avisa que a ida ao Sword/Shield guarda o original; os outros destinos não.
        var swsh = await DestinationAsync(id, "swsh");
        Assert.Equal(JsonValueKind.Null, swsh.GetProperty("refusal").ValueKind);
        Assert.True(swsh.GetProperty("keepsOriginal").GetBoolean());
        Assert.False((await DestinationAsync(id, "bdsp")).GetProperty("keepsOriginal").GetBoolean());

        var toSword = await _client.PostAsJsonAsync($"/api/vault/pokemon/{id}/transfer", new { game = "swsh" });
        toSword.EnsureSuccessStatusCode();
        Assert.Contains("original_kept", Notes(await toSword.Content.ReadFromJsonAsync<JsonElement>()));

        // Com o original guardado, a saída aparece como "pode" e funciona.
        var sv = await DestinationAsync(id, "sv");
        Assert.Equal(JsonValueKind.Null, sv.GetProperty("refusal").ValueKind);
        var back = await _client.PostAsJsonAsync($"/api/vault/pokemon/{id}/transfer", new { game = "sv" });
        back.EnsureSuccessStatusCode();
        var body = await back.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains("original_restored", Notes(body));
        var pokemon = body.GetProperty("pokemon");
        Assert.Equal(id, pokemon.GetProperty("id").GetGuid());
        Assert.Equal("sv", pokemon.GetProperty("family").GetString());
        Assert.NotEqual("illegal", pokemon.GetProperty("legality").GetString());

        // O arquivo de agora (direto do cofre) tem o encontro de antes da ida.
        var store = _app.Services.GetRequiredService<PokemonStore>();
        var stored = await store.GetAsync(id, TestContext.Current.CancellationToken);
        Assert.NotNull(stored);
        Assert.Null(stored.OriginalSha256);
        var result = PokemonReader.ToEntity(await store.ReadDataAsync(stored, TestContext.Current.CancellationToken), stored.Format);
        Assert.Equal(before.MetLocation, result.MetLocation);
        Assert.Equal(before.MetLevel, result.MetLevel);
        Assert.Equal(before.Version, result.Version);
    }
}

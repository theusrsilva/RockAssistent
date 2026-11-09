using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PKHeX.Core;
using TradeRock.Core.Games;
using TradeRock.Core.Pokemon;
using TradeRock.Core.Saves;
using TradeRock.Core.Storage;

namespace TradeRock.Tests;

/// <summary>Linhas no esquema de antes da D34 (coluna Family), para os testes de migração.</summary>
internal static class LegacyVault
{
    public static Task InsertPokemonAsync(TradeRockDb db, string family, string sha, CancellationToken ct) =>
        db.Database.ExecuteSqlRawAsync(
            """
            INSERT INTO "Pokemon" ("Id", "OwnerId", "Sha256", "Size", "Family", "Species", "Form", "SpeciesName", "Nickname",
                "Level", "IsShiny", "IsEgg", "Gender", "OtName", "Tid", "Sid", "OriginGame", "HeldItem", "Fateful", "Origin", "AddedAt")
            VALUES ({0}, {1}, {2}, 100, {3}, 25, 0, 'Pikachu', 'PIKA', 5, 0, 0, 0, 'RED', 1, 2, 'FireRed', 0, 0, 'Imported', {4})
            """,
            [Guid.NewGuid().ToString().ToUpperInvariant(), OwnerScope.LocalOwner.ToString().ToUpperInvariant(), sha, family,
                new DateTimeOffsetToBinaryConverter().ConvertToProvider(DateTimeOffset.UtcNow)!], ct);
}

public sealed class VaultFormatsTests : IAsyncLifetime
{
    private readonly TestDataDir _dir = new();
    private PokemonStore _pokemon = null!;

    public async ValueTask InitializeAsync() =>
        _pokemon = new PokemonStore(await _dir.CreateDbFactoryAsync(), _dir.Paths, TimeProvider.System);

    public ValueTask DisposeAsync()
    {
        _dir.Dispose();
        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task Migracao_mantem_o_cofre_antigo_com_o_formato_de_cada_jogo()
    {
        using var dir = new TestDataDir();
        var options = new DbContextOptionsBuilder<TradeRockDb>()
            .UseSqlite($"Data Source={dir.Paths.DatabasePath};Pooling=False").Options;
        var ct = TestContext.Current.CancellationToken;

        await using (var db = new TradeRockDb(options))
        {
            await db.GetService<IMigrator>().MigrateAsync("RunStarter", ct);
            await LegacyVault.InsertPokemonAsync(db, "Frlg", "aa", ct);
            await LegacyVault.InsertPokemonAsync(db, "Sv", "bb", ct);
            await LegacyVault.InsertPokemonAsync(db, "Lgpe", "cc", ct);
        }

        await using (var db = new TradeRockDb(options))
        {
            await db.Database.MigrateAsync(ct);
            var all = await db.Pokemon.OrderBy(p => p.Sha256).ToListAsync(ct);
            Assert.Equal([StorageFormat.Pk3, StorageFormat.Pk9, StorageFormat.Pb7], all.Select(p => p.Format));
            Assert.Equal([GameFamily.Frlg, GameFamily.Sv, GameFamily.Lgpe], all.Select(p => p.Family));
            Assert.Equal("Pikachu", all[0].SpeciesName);
        }
    }

    [Fact]
    public async Task Save_do_Platinum_vai_para_o_cofre_como_pk4()
    {
        var sav = BlankSaveFile.Get(GameVersion.Pt, "ROCHA", LanguageID.English);
        var pk = sav.BlankPKM;
        pk.Species = 393; // Piplup
        pk.CurrentLevel = 12;
        pk.HeldItem = 1;
        pk.RefreshChecksum();
        SaveFileAccess.WriteSlot(sav, 0, 0, pk);
        var entry = SaveFileAccess.ReadAll(sav).Single();
        var format = VaultFormats.ByContext(entry.Context)!;
        Assert.Equal(StorageFormat.Pk4, format.Format);
        Assert.Null(format.Family);

        var imported = (await _pokemon.ImportAsync(entry.Data, format.Format, PokemonOrigin.Imported, "pt.sav", default)).Pokemon;
        Assert.Equal(StorageFormat.Pk4, imported.Format);
        Assert.Null(imported.Family);
        Assert.Equal(393, imported.Species);
        Assert.True(File.Exists(_pokemon.FilePath(imported.Sha256, StorageFormat.Pk4)));
        Assert.EndsWith(".pk4", _pokemon.FilePath(imported.Sha256, StorageFormat.Pk4));

        // Exporta como .pk4 e o arquivo exportado volta igual (pela extensão).
        var (data, extension) = PokemonExport.ToFile(await _pokemon.ReadDataAsync(imported, default), imported.Format, "pk3", 100);
        Assert.Equal("pk4", extension);
        Assert.Equal(imported.Sha256, PokemonReader.Read(data, null, "piplup.pk4").Sha256);

        // E volta para o save do mesmo formato.
        var back = PokemonReader.ToEntity(await _pokemon.ReadDataAsync(imported, default), imported.Format);
        SaveFileAccess.WriteSlot(sav, 0, 1, back);
        Assert.Equal(2, SaveFileAccess.ReadAll(sav).Count);
    }

    [Theory]
    [InlineData("pk1", 1)]
    [InlineData("pk2", 2)]
    [InlineData("pk5", 5)]
    [InlineData("pk6", 6)]
    [InlineData("pk7", 7)]
    public async Task Arquivo_de_cada_geracao_entra_no_cofre_pela_extensao(string extension, int generation)
    {
        PKM pk = extension switch
        {
            "pk1" => new PK1(),
            "pk2" => new PK2(),
            "pk5" => new PK5(),
            "pk6" => new PK6(),
            _ => new PK7(),
        };
        pk.Species = 25;
        pk.CurrentLevel = 5;
        pk.OriginalTrainerName = "ASH";
        pk.Nickname = "PIKACHU";
        pk.RefreshChecksum();
        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);

        var imported = (await _pokemon.ImportAsync(data, PokemonOrigin.Imported, $"pikachu.{extension}", default)).Pokemon;
        var format = VaultFormats.Get(imported.Format);
        Assert.Equal(extension, format.Extension);
        Assert.Equal(generation, format.Generation);
        Assert.Null(imported.Family);
        Assert.Equal("Pikachu", imported.SpeciesName);
        Assert.NotNull(imported.Legality);
    }
}

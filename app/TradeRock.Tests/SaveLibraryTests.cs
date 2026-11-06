using PKHeX.Core;
using TradeRock.Core.Saves;
using TradeRock.Core.Storage;

namespace TradeRock.Tests;

public sealed class SaveLibraryTests : IAsyncLifetime
{
    private const ushort Gastly = 92, Haunter = 93, Gengar = 94;

    private readonly TestDataDir _dir = new();
    private string _saves = null!;
    private SaveFolders _folders = null!;
    private SaveBackups _backups = null!;
    private SaveLibrary _library = null!;

    public async ValueTask InitializeAsync()
    {
        var db = await _dir.CreateDbFactoryAsync();
        _saves = Directory.CreateDirectory(Path.Combine(_dir.Paths.DataDir, "saves")).FullName;
        _folders = new SaveFolders(new SettingsStore(db));
        _backups = new SaveBackups(_dir.Paths, TimeProvider.System);
        _library = new SaveLibrary(_folders, _backups, TimeProvider.System);
        await _folders.SetAsync([new SaveFolder(_saves, "")], default);
    }

    public ValueTask DisposeAsync()
    {
        _dir.Dispose();
        return ValueTask.CompletedTask;
    }

    private static byte[] Stored(PKM pk)
    {
        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);
        return data;
    }

    private static byte[] Mon(SaveFile sav, ushort species, byte level = 30)
    {
        var pk = sav.BlankPKM;
        pk.Species = species;
        pk.CurrentLevel = level;
        pk.RefreshChecksum();
        return Stored(pk);
    }

    private async Task<SaveLibraryEntry> Single(string name) =>
        (await _library.ListAsync(true, default)).Single(e => Path.GetFileName(e.Path) == name);

    [Fact]
    public async Task Lista_saves_bons_e_ruins_sem_derrubar_a_varredura()
    {
        File.WriteAllBytes(Path.Combine(_saves, "fr.sav"), SaveFixture.BlankFireRed());
        Directory.CreateDirectory(Path.Combine(_saves, "sub"));
        File.WriteAllBytes(Path.Combine(_saves, "sub", "x.sav"), new byte[10]);
        File.WriteAllBytes(Path.Combine(_saves, "notas.txt"), new byte[10]); // fora do padrão

        var list = await _library.ListAsync(true, default);

        Assert.Equal(2, list.Count);
        var good = list.Single(e => e.Error is null);
        Assert.Equal("frlg", good.Info!.GameKey);
        Assert.Equal(SaveLibrary.IdOf(good.Path), good.Id);
        Assert.Equal(8, good.Id.Length);
        var bad = list.Single(e => e.Error is not null);
        Assert.Equal("sav_unknown", bad.Error);
        Assert.Null(bad.Info);
        Assert.EndsWith("x.sav", bad.Path);
    }

    [Fact]
    public async Task Arquivo_grande_demais_nem_e_lido()
    {
        await using (var big = File.Create(Path.Combine(_saves, "big.bin")))
            big.SetLength(SaveFileAccess.MaxSaveBytes + 1);

        var entry = await Single("big.bin");

        Assert.Equal("sav_too_big", entry.Error);
    }

    [Fact]
    public async Task Pastas_inexistentes_ou_relativas()
    {
        await _folders.SetAsync([new SaveFolder(Path.Combine(_saves, "nao-existe"), "")], default);
        Assert.Empty(await _library.ListAsync(true, default));

        var ex = await Assert.ThrowsAsync<SaveInjectionException>(() => _folders.SetAsync([new SaveFolder("relativa", "")], default));
        Assert.Equal("folder_invalid", ex.Code);
        Assert.Equal(SaveFolders.DefaultPattern, Assert.Single(await _folders.GetAsync(default)).Pattern);
    }

    [Fact]
    public async Task Apply_grava_com_backup_e_read_devolve_o_estado_do_disco()
    {
        var path = Path.Combine(_saves, "fr.sav");
        File.WriteAllBytes(path, SaveFixture.BlankFireRed());
        var entry = await Single("fr.sav");
        var gengar = Mon(SaveFileAccess.Open(File.ReadAllBytes(path)), Gengar);

        var result = await _library.ApplyAsync(entry.Id, [new SaveChange(0, 0, gengar)], default);

        Assert.True(File.Exists(result.BackupPath));
        Assert.StartsWith(_backups.Directory, result.BackupPath);
        Assert.Equal(SaveInjection.Sha256(File.ReadAllBytes(path)), result.NewSha256);
        Assert.Equal(entry.Info!.Sha256, SaveInjection.Sha256(File.ReadAllBytes(result.BackupPath)));
        var boxes = await _library.ReadAsync(entry.Id, default);
        var slot = Assert.Single(boxes.Slots);
        Assert.Equal((0, 0, Gengar), (slot.Box, slot.Slot, slot.Summary.Species));
        Assert.Equal(result.NewSha256, boxes.Info.Sha256);
        Assert.Equal(14, boxes.BoxNames.Count);
        Assert.False(File.Exists(path + ".traderock.tmp"));

        // A lista sem varredura nova já mostra o save gravado.
        var listed = Assert.Single(await _library.ListAsync(false, default));
        Assert.Equal(result.NewSha256, listed.Info!.Sha256);
        Assert.Equal(new DateTimeOffset(File.GetLastWriteTimeUtc(path), TimeSpan.Zero), listed.Info.ModifiedAt);

        // O arquivo mudou desde a leitura: não grava por cima.
        var changed = await Assert.ThrowsAsync<SaveInjectionException>(() =>
            _library.ApplyAsync(entry.Id, [new SaveChange(0, 1, gengar)], default, expectedSha256: entry.Info.Sha256));
        Assert.Equal("save_changed", changed.Code);

        // Pokémon de outro formato.
        var pk8 = Stored(new PK8 { Species = Gengar });
        var wrong = await Assert.ThrowsAsync<SaveInjectionException>(() =>
            _library.ApplyAsync(entry.Id, [new SaveChange(0, 1, pk8)], default));
        Assert.Equal("sav_wrong_game", wrong.Code);
    }

    [LinuxFact]
    [System.Runtime.Versioning.UnsupportedOSPlatform("windows")]
    public async Task Pasta_sem_permissao_de_escrita_da_save_unwritable_sem_mexer_no_original()
    {
        Assert.SkipWhen(Environment.IsPrivilegedProcess || OperatingSystem.IsWindows(), "Como root (ou no Windows) a permissão não vale.");
        var path = Path.Combine(_saves, "fr.sav");
        File.WriteAllBytes(path, SaveFixture.BlankFireRed());
        var original = File.ReadAllBytes(path);
        var entry = await Single("fr.sav");
        var gengar = Mon(SaveFileAccess.Open(original), Gengar);

        File.SetUnixFileMode(_saves, UnixFileMode.UserRead | UnixFileMode.UserExecute);
        try
        {
            var ex = await Assert.ThrowsAsync<SaveInjectionException>(() =>
                _library.ApplyAsync(entry.Id, [new SaveChange(0, 0, gengar)], default));
            Assert.Equal("save_unwritable", ex.Code);
        }
        finally
        {
            File.SetUnixFileMode(_saves, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        }
        Assert.Equal(original, File.ReadAllBytes(path));
        Assert.False(File.Exists(path + ".traderock.tmp"));
    }

    [Fact]
    public async Task Gen2_limpar_compacta_a_caixa_e_read_mostra_o_disco()
    {
        var sav = BlankSaveFile.Get(GameVersion.C, "ROCHA", LanguageID.English);
        // O PKHeX só reconhece um save da Gen 2 com as listas do time e da caixa atual montadas.
        SaveFileAccess.WriteSlot(sav, SaveFileAccess.Party, 0, SaveFileAccess.ToEntity(sav, Mon(sav, Gengar)));
        SaveFileAccess.WriteSlot(sav, 0, 0, SaveFileAccess.ToEntity(sav, Mon(sav, Gastly)));
        var path = Path.Combine(_saves, "crystal.sav");
        File.WriteAllBytes(path, SaveFileAccess.Export(sav));
        var entry = await Single("crystal.sav");
        Assert.Equal("c", entry.Info!.GameKey);

        await _library.ApplyAsync(entry.Id,
            [new SaveChange(0, 1, Mon(sav, Haunter)), new SaveChange(0, 2, Mon(sav, Gengar))], default);
        await _library.ApplyAsync(entry.Id, [new SaveChange(0, 0, null)], default);

        var slots = (await _library.ReadAsync(entry.Id, default)).Slots.Where(s => s.Box == 0).OrderBy(s => s.Slot).ToList();
        Assert.Equal([(0, Haunter), (1, Gengar)], slots.Select(s => (s.Slot, s.Summary.Species)).ToList());
    }

    [Fact]
    public async Task Cache_relê_so_quando_o_arquivo_muda()
    {
        var path = Path.Combine(_saves, "fr.sav");
        File.WriteAllBytes(path, SaveFixture.BlankFireRed());
        var first = await Single("fr.sav");
        Assert.Same(first, Assert.Single(await _library.ListAsync(false, default)));
        Assert.Same(first, Assert.Single(await _library.ListAsync(true, default)));

        File.WriteAllBytes(path, SaveFixture.BlankFireRed(ot: "BLUE"));
        File.SetLastWriteTimeUtc(path, DateTime.UtcNow.AddMinutes(1));
        var second = await Single("fr.sav");
        Assert.Equal("BLUE", second.Info!.TrainerName);
        Assert.NotEqual(first.Info!.Sha256, second.Info.Sha256);
    }

    [Fact]
    public async Task Id_desconhecido()
    {
        var ex = await Assert.ThrowsAsync<SaveInjectionException>(() => _library.ReadAsync("deadbeef", default));
        Assert.Equal("save_not_found", ex.Code);
    }

    [Theory]
    [InlineData("a/b/main", true)]
    [InlineData("main", true)]
    [InlineData("x/FR.SAV", true)]
    [InlineData("x/fr.sav.bak", false)]
    [InlineData("x/domain", false)]
    public void Glob_simples(string path, bool match) =>
        Assert.Equal(match, new SaveGlob(SaveFolders.DefaultPattern).IsMatch(path));
}

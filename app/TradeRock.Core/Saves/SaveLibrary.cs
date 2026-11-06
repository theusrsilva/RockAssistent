using System.Security.Cryptography;
using System.Text;
using PKHeX.Core;

namespace TradeRock.Core.Saves;

/// <summary>Um arquivo achado nas pastas de saves.</summary>
/// <param name="Id">8 primeiros dígitos (hex) do SHA-256 do caminho completo.</param>
/// <param name="Info">Resumo do save; null quando <paramref name="Error"/> está preenchido.</param>
/// <param name="Error">Código do erro (<c>sav_unknown</c>, <c>sav_unsupported</c>, <c>sav_too_big</c>, <c>sav_unreadable</c>...).</param>
/// <param name="Path">Caminho do arquivo (também nas entradas com erro).</param>
public sealed record SaveLibraryEntry(string Id, SaveInfo? Info, string? Error, string Path);

/// <summary>Caixas e time de um save.</summary>
/// <param name="BoxNames">Nome de cada caixa (o do jogo, ou "Box N").</param>
public sealed record SaveBoxes(SaveInfo Info, IReadOnlyList<SaveSlotEntry> Slots, IReadOnlyList<string> BoxNames);

/// <summary>Uma mudança num save: grava <paramref name="Data"/> (formato do save) ou limpa a posição (null).</summary>
public sealed record SaveChange(int Box, int Slot, byte[]? Data);

public sealed record SaveWriteResult(string BackupPath, string NewSha256);

/// <summary>
/// Varre as pastas de saves (<see cref="SaveFolders"/>), lê cada arquivo com o <see cref="SaveFileAccess"/> e grava
/// mudanças com backup antes. Guarda o resultado por caminho + data de modificação + tamanho: um arquivo só é lido
/// de novo quando muda. Arquivo truncado, de outro jogo ou em uso vira uma entrada com erro; o resto continua.
/// </summary>
public sealed class SaveLibrary(SaveFolders folders, SaveBackups backups, TimeProvider time)
{
    /// <summary>Profundidade máxima das subpastas varridas (evita descer numa pasta pessoal inteira).</summary>
    public const int MaxDepth = 6;

    /// <summary>Arquivos no máximo por pasta configurada.</summary>
    public const int MaxFilesPerFolder = 2000;

    private sealed record Cached(DateTime ModifiedUtc, long Size, SaveLibraryEntry Entry)
    {
        public SaveBoxes? Boxes { get; set; }
    }

    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly Dictionary<string, Cached> _cache = new(StringComparer.Ordinal);
    private IReadOnlyList<SaveLibraryEntry>? _last;

    /// <summary>Quando as pastas foram varridas pela última vez (null = nunca).</summary>
    public DateTimeOffset? LastScanAt { get; private set; }

    public static string IdOf(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(System.IO.Path.GetFullPath(path))))[..8];

    public async Task<IReadOnlyList<SaveLibraryEntry>> ListAsync(bool refresh, CancellationToken ct)
    {
        if (!refresh && _last is { } last)
            return last;
        var configured = await folders.GetAsync(ct);
        await _gate.WaitAsync(ct);
        try
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var result = new List<SaveLibraryEntry>();
            foreach (var folder in configured)
            {
                foreach (var path in Find(folder))
                {
                    ct.ThrowIfCancellationRequested();
                    if (!seen.Add(path))
                        continue;
                    result.Add(Scan(path).Entry);
                }
            }
            // Esquece o que sumiu das pastas.
            foreach (var gone in _cache.Keys.Where(k => !seen.Contains(k)).ToList())
                _cache.Remove(gone);
            _last = result;
            LastScanAt = time.GetUtcNow();
            return result;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>Caixas e time do save, lidos do arquivo como está agora. Lança "save_not_found" ou o erro da leitura.</summary>
    public async Task<SaveBoxes> ReadAsync(string id, CancellationToken ct)
    {
        var path = await PathOfAsync(id, ct);
        await _gate.WaitAsync(ct);
        try
        {
            var cached = Scan(path);
            if (cached.Entry.Error is { } error)
                throw new SaveInjectionException(error, $"O save {System.IO.Path.GetFileName(path)} não pôde ser lido.");
            if (cached.Boxes is { } boxes)
                return boxes;
            var bytes = File.ReadAllBytes(path);
            var sav = SaveFileAccess.Open(bytes);
            cached.Boxes = Boxes(sav, cached.Entry.Info!);
            return cached.Boxes;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>Abre o save para quem precisa do <see cref="SaveFile"/> (mover, converter). Não grava nada.</summary>
    public async Task<(SaveFile Save, SaveInfo Info)> OpenAsync(string id, CancellationToken ct)
    {
        var path = await PathOfAsync(id, ct);
        var bytes = await ReadFileAsync(path, ct);
        var sav = SaveFileAccess.Open(bytes);
        var stamp = File.GetLastWriteTimeUtc(path);
        return (sav, SaveFileAccess.Describe(sav, path, new DateTimeOffset(stamp, TimeSpan.Zero), SaveInjection.Sha256(bytes)));
    }

    /// <summary>
    /// Aplica as mudanças: gravações primeiro (posição crescente), depois as limpezas (posição decrescente, para os
    /// índices do time e das caixas da Gen 1 e 2 não andarem no meio). Confere o resultado em memória, faz o backup,
    /// grava (arquivo temporário + <see cref="File.Replace(string, string, string?)"/>) e relê o arquivo.
    /// </summary>
    /// <param name="expectedSha256">Se informado, recusa ("save_changed") quando o arquivo mudou desde a leitura.</param>
    public async Task<SaveWriteResult> ApplyAsync(string id, IReadOnlyList<SaveChange> changes, CancellationToken ct,
        string? expectedSha256 = null)
    {
        if (changes.Count == 0)
            throw new SaveInjectionException("nothing_to_apply", "Nenhuma mudança.");
        if (changes.GroupBy(c => (c.Box, c.Slot)).Any(g => g.Count() > 1))
            throw new SaveInjectionException("slot_repeated", "Duas mudanças para a mesma posição.");

        var path = await PathOfAsync(id, ct);
        await _gate.WaitAsync(ct);
        try
        {
            var original = await ReadFileAsync(path, ct);
            var originalSha = SaveInjection.Sha256(original);
            if (expectedSha256 is not null && !string.Equals(expectedSha256, originalSha, StringComparison.OrdinalIgnoreCase))
                throw new SaveInjectionException("save_changed", "O save mudou no disco desde que foi lido.");

            var sav = SaveFileAccess.Open(original);
            SaveFileAccess.GameKeyOf(sav); // jogo fora da lista: sav_unsupported
            if (!sav.State.Exportable)
                throw new SaveInjectionException("save_read_only", "Este save não pode ser gravado.");

            var expected = new List<(int Box, ushort Species, uint Pid, ushort Tid)>();
            foreach (var change in changes.Where(c => c.Data is not null).OrderBy(c => c.Box).ThenBy(c => c.Slot))
            {
                var pk = SaveFileAccess.ToEntity(sav, change.Data!);
                SaveFileAccess.WriteSlot(sav, change.Box, change.Slot, pk);
                expected.Add((change.Box, pk.Species, pk.PID, pk.TID16));
            }
            foreach (var change in changes.Where(c => c.Data is null).OrderByDescending(c => c.Box).ThenByDescending(c => c.Slot))
                SaveFileAccess.ClearSlot(sav, change.Box, change.Slot);

            var result = SaveFileAccess.Export(sav);
            Verify(SaveFileAccess.Open(result), expected);

            var backup = backups.Create(original, System.IO.Path.GetFileName(path));
            var temp = path + ".traderock.tmp";
            await File.WriteAllBytesAsync(temp, result, ct);
            File.Replace(temp, path, destinationBackupFileName: null);

            // Relê o arquivo gravado: o que vale é o disco (as caixas da Gen 1 e 2 se compactam na gravação).
            var written = await ReadFileAsync(path, ct);
            var newSha = SaveInjection.Sha256(written);
            if (newSha != SaveInjection.Sha256(result))
            {
                File.Copy(backup, path, overwrite: true);
                throw new SaveInjectionException("verify_failed", "A conferência do save gravado falhou; o original voltou.");
            }
            Verify(SaveFileAccess.Open(written), expected);
            _cache.Remove(path);
            Scan(path);
            return new SaveWriteResult(backup, newSha);
        }
        finally
        {
            _gate.Release();
        }
    }

    private static void Verify(SaveFile sav, IReadOnlyList<(int Box, ushort Species, uint Pid, ushort Tid)> expected)
    {
        var all = SaveFileAccess.ReadAll(sav);
        foreach (var (box, species, pid, tid) in expected)
        {
            // A posição exata pode andar no time e nas caixas da Gen 1 e 2 (listas sem buracos): basta estar na caixa.
            var found = all.Any(e => e.Box == box && e.Summary.Species == species && e.Summary.Pid == pid
                && SaveFileAccess.ToEntity(sav, e.Data).TID16 == tid);
            if (!found)
                throw new SaveInjectionException("verify_failed", "A conferência do save gravado falhou; nada foi alterado.");
        }
    }

    private static SaveBoxes Boxes(SaveFile sav, SaveInfo info)
    {
        var names = Enumerable.Range(0, sav.BoxCount)
            .Select(i => sav is IBoxDetailName n && !string.IsNullOrWhiteSpace(n.GetBoxName(i)) ? n.GetBoxName(i) : $"Box {i + 1}")
            .ToList();
        return new SaveBoxes(info, SaveFileAccess.ReadAll(sav), names);
    }

    private async Task<string> PathOfAsync(string id, CancellationToken ct)
    {
        var entry = (await ListAsync(false, ct)).FirstOrDefault(e => e.Id == id)
            ?? (await ListAsync(true, ct)).FirstOrDefault(e => e.Id == id)
            ?? throw new SaveInjectionException("save_not_found", "Save não encontrado nas pastas.");
        return entry.Path;
    }

    private static async Task<byte[]> ReadFileAsync(string path, CancellationToken ct)
    {
        try
        {
            if (new FileInfo(path).Length > SaveFileAccess.MaxSaveBytes)
                throw new SaveInjectionException("sav_too_big", "Arquivo grande demais para um save.");
            return await File.ReadAllBytesAsync(path, ct);
        }
        catch (FileNotFoundException)
        {
            throw new SaveInjectionException("save_not_found", "O arquivo do save sumiu.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new SaveInjectionException("sav_unreadable", "O arquivo não pôde ser lido (em uso ou sem permissão).");
        }
    }

    /// <summary>Lê (ou pega do cache) um arquivo. Chamar com o <see cref="_gate"/> fechado.</summary>
    private Cached Scan(string path)
    {
        var id = IdOf(path);
        FileInfo file;
        try
        {
            file = new FileInfo(path);
            if (!file.Exists)
                return new Cached(default, 0, new SaveLibraryEntry(id, null, "save_not_found", path));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new Cached(default, 0, new SaveLibraryEntry(id, null, "sav_unreadable", path));
        }

        if (_cache.TryGetValue(path, out var hit) && hit.ModifiedUtc == file.LastWriteTimeUtc && hit.Size == file.Length)
            return hit;

        SaveLibraryEntry entry;
        if (file.Length > SaveFileAccess.MaxSaveBytes)
            entry = new SaveLibraryEntry(id, null, "sav_too_big", path);
        else
        {
            try
            {
                var bytes = File.ReadAllBytes(path);
                var sav = SaveFileAccess.Open(bytes);
                var info = SaveFileAccess.Describe(sav, path, new DateTimeOffset(file.LastWriteTimeUtc, TimeSpan.Zero),
                    SaveInjection.Sha256(bytes));
                entry = new SaveLibraryEntry(id, info, null, path);
            }
            catch (SaveInjectionException ex)
            {
                entry = new SaveLibraryEntry(id, null, ex.Code, path);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                entry = new SaveLibraryEntry(id, null, "sav_unreadable", path);
            }
            catch (Exception)
            {
                // O PKHeX pode estourar com um arquivo estranho: o arquivo fica na lista com erro e o resto continua.
                entry = new SaveLibraryEntry(id, null, "sav_unknown", path);
            }
        }
        var cached = new Cached(file.LastWriteTimeUtc, file.Length, entry);
        _cache[path] = cached;
        return cached;
    }

    private static IEnumerable<string> Find(SaveFolder folder)
    {
        if (!Directory.Exists(folder.Path))
            return [];
        var glob = new SaveGlob(folder.Pattern);
        var options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            MaxRecursionDepth = MaxDepth,
            AttributesToSkip = FileAttributes.System,
        };
        try
        {
            return Directory.EnumerateFiles(folder.Path, "*", options)
                .Where(p => !p.EndsWith(".traderock.tmp", StringComparison.Ordinal))
                .Where(p => glob.IsMatch(System.IO.Path.GetRelativePath(folder.Path, p)))
                .Take(MaxFilesPerFolder)
                .Select(System.IO.Path.GetFullPath)
                .OrderBy(p => p, StringComparer.Ordinal)
                .ToList();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return [];
        }
    }
}

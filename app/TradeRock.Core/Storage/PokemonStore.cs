using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using PKHeX.Core;
using TradeRock.Core.Builder;
using TradeRock.Core.Games;
using TradeRock.Core.Pokemon;

namespace TradeRock.Core.Storage;

public sealed record ImportResult(StoredPokemon Pokemon, bool Created, bool Restored = false);

/// <summary>
/// Guarda Pokémon de qualquer jogo do catálogo: o arquivo decriptado vai para vault/&lt;hh&gt;/&lt;sha256&gt;.&lt;ext&gt;
/// (.pk3, .pk8, .pk9...) e o resumo para o SQLite. Arquivos idênticos (mesmo hash, inclusive o mesmo Pokémon
/// criptografado ou não) viram um só registro por cofre;
/// o arquivo em disco é um só, mesmo que esteja em vários cofres.
/// </summary>
public sealed class PokemonStore(IDbContextFactory<TradeRockDb> dbFactory, AppPaths paths, TimeProvider time)
{
    /// <param name="family">Jogo do arquivo quando o conteúdo e o nome não dizem (Pokémon recebido numa troca).</param>
    public async Task<ImportResult> ImportAsync(
        ReadOnlyMemory<byte> file, PokemonOrigin origin, string? sourceFileName, CancellationToken ct,
        GameFamily? family = null) =>
        await ImportFileAsync(PokemonReader.Read(file.Span, family, sourceFileName), origin, sourceFileName, ct);

    /// <summary>Importa no formato dado (Pokémon de um save de qualquer geração).</summary>
    public async Task<ImportResult> ImportAsync(
        ReadOnlyMemory<byte> file, StorageFormat format, PokemonOrigin origin, string? sourceFileName, CancellationToken ct) =>
        await ImportFileAsync(PokemonReader.Read(file.Span, format), origin, sourceFileName, ct);

    private async Task<ImportResult> ImportFileAsync(PokemonFile pk, PokemonOrigin origin, string? sourceFileName, CancellationToken ct)
    {

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var existing = await db.Pokemon.SingleOrDefaultAsync(p => p.Sha256 == pk.Sha256, ct);
        if (existing is not null)
        {
            // O arquivo pode ter sumido do disco; regrava a partir dos bytes recebidos.
            await WriteFileAsync(pk, ct);
            if (existing.DeletedAt is null)
                return new ImportResult(existing, Created: false);

            // Estava na lixeira: volta para a Caixa de entrada.
            existing.DeletedAt = null;
            await db.SaveChangesAsync(ct);
            return new ImportResult(existing, Created: false, Restored: true);
        }

        await WriteFileAsync(pk, ct);

        var s = pk.Summary;
        var stored = new StoredPokemon
        {
            Id = Guid.CreateVersion7(),
            Sha256 = pk.Sha256,
            Size = pk.Data.Length,
            Format = s.Format,
            Species = s.Species,
            Form = s.Form,
            SpeciesName = s.SpeciesName,
            Nickname = s.Nickname,
            Level = s.Level,
            IsShiny = s.IsShiny,
            IsEgg = s.IsEgg,
            Gender = s.Gender,
            OtName = s.OtName,
            Tid = s.Tid,
            Sid = s.Sid,
            OriginGame = s.OriginGame,
            HeldItem = s.HeldItem,
            Fateful = s.Fateful,
            Pid = s.Pid,
            Legality = AnalyzeLegality(pk.Data, s.Format),
            LegalityPkhexVersion = LegalityReporter.PkhexVersion,
            Origin = origin,
            SourceFileName = sourceFileName is null ? null : Path.GetFileName(sourceFileName),
            AddedAt = time.GetUtcNow(),
        };
        db.Pokemon.Add(stored);

        try
        {
            await db.SaveChangesAsync(ct);
            return new ImportResult(stored, Created: true);
        }
        catch (DbUpdateException)
        {
            // Duas importações simultâneas do mesmo arquivo: vale a que chegou primeiro.
            await using var retry = await dbFactory.CreateDbContextAsync(ct);
            var winner = await retry.Pokemon.SingleOrDefaultAsync(p => p.Sha256 == pk.Sha256, ct);
            if (winner is null)
                throw;
            return new ImportResult(winner, Created: false);
        }
    }

    /// <summary>
    /// Troca os dados de um Pokémon do cofre por outros (edição no painel do cofre). O registro continua o
    /// mesmo: id, caixa, posição, etiquetas, origem e as trocas do histórico que apontam para ele.
    /// </summary>
    public async Task<StoredPokemon> ReplaceAsync(Guid id, ReadOnlyMemory<byte> file, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var stored = await db.Pokemon.Include(p => p.Tags).SingleOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new VaultException("pokemon_not_found");
        if (stored.DeletedAt is not null)
            throw new VaultException("pokemon_in_trash");
        // A edição não muda o jogo do Pokémon (mover entre jogos é outra operação).
        var pk = PokemonReader.Read(file.Span, stored.Format);
        if (stored.Sha256 == pk.Sha256)
            return stored;
        // O resultado já existe como outro Pokémon deste cofre (mesmo arquivo): não dá para ter dois iguais.
        if (await db.Pokemon.AnyAsync(p => p.Sha256 == pk.Sha256 && p.Id != id, ct))
            throw new VaultException("pokemon_duplicate");

        await WriteFileAsync(pk, ct);
        var oldSha = stored.Sha256;
        var s = pk.Summary;
        stored.Sha256 = pk.Sha256;
        stored.Size = pk.Data.Length;
        stored.Species = s.Species;
        stored.Form = s.Form;
        stored.SpeciesName = s.SpeciesName;
        stored.Nickname = s.Nickname;
        stored.Level = s.Level;
        stored.IsShiny = s.IsShiny;
        stored.IsEgg = s.IsEgg;
        stored.Gender = s.Gender;
        stored.OtName = s.OtName;
        stored.Tid = s.Tid;
        stored.Sid = s.Sid;
        stored.OriginGame = s.OriginGame;
        stored.HeldItem = s.HeldItem;
        stored.Fateful = s.Fateful;
        stored.Pid = s.Pid;
        stored.Legality = AnalyzeLegality(pk.Data, s.Format);
        stored.LegalityPkhexVersion = LegalityReporter.PkhexVersion;
        // A edição pode ter posto outro Pokémon no registro: aí o original guardado (D37) deixa de valer.
        var unused = new List<(string Sha256, StorageFormat Format)> { (oldSha, stored.Format) };
        if (stored.OriginalSha256 is { } originalSha && stored.OriginalFormat is { } originalFormat
            && await ReadOriginalAsync(stored, PokemonReader.ToEntity(pk.Data, stored.Format), ct) is null)
        {
            unused.Add((originalSha, originalFormat));
            (stored.OriginalSha256, stored.OriginalFormat) = (null, null);
        }
        await db.SaveChangesAsync(ct);

        // O arquivo antigo é compartilhado entre cofres: só sai quando ninguém mais o usa.
        await DeleteUnusedFilesAsync(db, unused, ct);
        return stored;
    }

    /// <summary>
    /// Move o Pokémon para outro jogo, como o HOME (<see cref="HomeTransfer"/>). O registro continua o mesmo (id,
    /// caixa, etiquetas, histórico); mudam o jogo e os dados. Recusa com <see cref="VaultException"/> (código do HOME).
    /// </summary>
    public async Task<(StoredPokemon Pokemon, IReadOnlyList<string> Notes)> TransferAsync(
        Guid id, GameFamily to, ITrainerInfo owner, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var stored = await db.Pokemon.Include(p => p.Tags).SingleOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new VaultException("pokemon_not_found");
        if (stored.DeletedAt is not null)
            throw new VaultException("pokemon_in_trash");
        if (stored.Family == to)
            throw new VaultException("same_game");
        // Formatos fora do catálogo de troca se adaptam por outro caminho (AdaptAsync).
        if (stored.Family is null)
            throw new VaultException("home_no_destination");

        var source = PokemonReader.ToEntity(await ReadDataAsync(stored, ct), stored.Format);
        // Com o original de antes da ida ao Sword/Shield guardado (D37), a saída parte dele.
        var moved = (await ReadOriginalAsync(stored, source, ct) is { } original
                ? KeptOriginal.Move(source, original.Pokemon, original.Family, GameCatalog.Get(to), owner)
                : null)
            ?? HomeTransfer.Move(source, GameCatalog.Get(to), owner);
        if (!moved.Ok)
            throw new VaultException(moved.Refusal ?? "no_route");

        var data = new byte[moved.Pokemon!.SIZE_STORED];
        moved.Pokemon.WriteDecryptedDataStored(data);
        var pk = PokemonReader.Read(data, to);
        if (await db.Pokemon.AnyAsync(p => p.Sha256 == pk.Sha256 && p.Id != id, ct))
            throw new VaultException("pokemon_duplicate");

        await WriteFileAsync(pk, ct);
        var (oldSha, oldFormat) = (stored.Sha256, stored.Format);
        var s = pk.Summary;
        stored.Sha256 = pk.Sha256;
        stored.Size = pk.Data.Length;
        stored.Format = s.Format;
        stored.Species = s.Species;
        stored.Form = s.Form;
        stored.SpeciesName = s.SpeciesName;
        stored.Nickname = s.Nickname;
        stored.Level = s.Level;
        stored.IsShiny = s.IsShiny;
        stored.IsEgg = s.IsEgg;
        stored.Gender = s.Gender;
        stored.OtName = s.OtName;
        stored.Tid = s.Tid;
        stored.Sid = s.Sid;
        stored.OriginGame = s.OriginGame;
        stored.HeldItem = s.HeldItem;
        stored.Fateful = s.Fateful;
        stored.Pid = s.Pid;
        stored.Legality = AnalyzeLegality(pk.Data, s.Format);
        stored.LegalityPkhexVersion = LegalityReporter.PkhexVersion;

        // O original guardado só vale dentro do Sword/Shield: ao sair, o vínculo acaba (o arquivo sai se ninguém o usa).
        var notes = moved.Notes.ToList();
        var unused = new List<(string Sha256, StorageFormat Format)> { (oldSha, oldFormat) };
        if (stored.OriginalSha256 is { } previousSha && stored.OriginalFormat is { } previousFormat)
            unused.Add((previousSha, previousFormat));
        (stored.OriginalSha256, stored.OriginalFormat) = (null, null);
        // A ida ao Sword/Shield perdeu a origem: o arquivo de antes fica guardado para ele poder sair depois.
        if (to == GameFamily.Swsh && HomeTransfer.OriginLostInSwsh(moved.Pokemon))
        {
            (stored.OriginalSha256, stored.OriginalFormat) = (oldSha, oldFormat);
            notes.Add(KeptOriginal.KeptNote);
        }
        await db.SaveChangesAsync(ct);

        await DeleteUnusedFilesAsync(db, unused, ct);
        return (stored, notes);
    }

    /// <summary>
    /// "Adaptar para um jogo mais novo" (D34, <see cref="TransferChain"/>): o original continua no cofre e a cópia
    /// adaptada entra na Caixa de entrada. Recusa com <see cref="VaultException"/> (código da etapa que recusou);
    /// "adapted_exists" quando a mesma cópia já está no cofre.
    /// </summary>
    public async Task<(StoredPokemon Pokemon, IReadOnlyList<string> Notes, PKM Adapted)> AdaptAsync(
        Guid id, StorageFormat to, ITrainerInfo owner, CancellationToken ct)
    {
        if (await GetAsync(id, ct) is not { } stored)
            throw new VaultException("pokemon_not_found");
        if (stored.DeletedAt is not null)
            throw new VaultException("pokemon_in_trash");

        var source = PokemonReader.ToEntity(await ReadDataAsync(stored, ct), stored.Format);
        var adapted = TransferChain.Adapt(source, stored.Format, to, owner);
        if (!adapted.Ok)
            throw new VaultException(adapted.Refusal ?? "no_route");

        var data = new byte[adapted.Pokemon!.SIZE_STORED];
        adapted.Pokemon.WriteDecryptedDataStored(data);
        var result = await ImportFileAsync(PokemonReader.Read(data, to), stored.Origin, stored.SourceFileName, ct);
        if (!result.Created && !result.Restored)
            throw new VaultException("adapted_exists");
        return (result.Pokemon, adapted.Notes, adapted.Pokemon);
    }

    /// <summary>
    /// "Copiar para ser válido no jogo" (D36, <see cref="GameCopy"/>): cria um Pokémon novo, nascido no jogo de destino,
    /// com o que dá para copiar do original. O original continua no cofre e a cópia entra na Caixa de entrada. Recusa
    /// com <see cref="GameCopyException"/>; "copy_exists" quando a mesma cópia já está no cofre.
    /// </summary>
    /// <param name="trainer">O "Meu treinador" do jogo de destino, com os números como o jogo mostra.</param>
    public async Task<(StoredPokemon Pokemon, GameCopyResult Copy)> CopyForGameAsync(
        Guid id, GameFamily to, TrainerSpec trainer, string language, CancellationToken ct)
    {
        if (await GetAsync(id, ct) is not { } stored)
            throw new GameCopyException("pokemon_not_found");
        if (stored.DeletedAt is not null)
            throw new GameCopyException("pokemon_in_trash");
        // Gen 1, 2 e 4 a 7 vão para a frente pelo "Adaptar" (AdaptAsync).
        if (stored.Family is null)
            throw new GameCopyException("home_no_destination");

        var source = PokemonReader.ToEntity(await ReadDataAsync(stored, ct), stored.Format);
        var copy = await GameCopy.CreateAsync(source, stored.Family, GameCatalog.Get(to), trainer, language, ct);
        if (!copy.Ok)
            throw new GameCopyException(copy.Refusal ?? "copy_not_possible", copy.Reason);

        var result = await ImportAsync(copy.Build!.Data!, PokemonOrigin.Created, null, ct, to);
        if (!result.Created && !result.Restored)
            throw new GameCopyException("copy_exists");
        return (result.Pokemon, copy);
    }

    /// <summary>Destinos de "Adaptar" (formatos mais novos), com o caminho e o motivo de cada recusa.</summary>
    public async Task<IReadOnlyList<AdaptOption>> AdaptOptionsAsync(StoredPokemon pokemon, CancellationToken ct) =>
        TransferChain.Options(PokemonReader.ToEntity(await ReadDataAsync(pokemon, ct), pokemon.Format), pokemon.Format);

    /// <summary>
    /// Para quais jogos o Pokémon pode ir (null = pode; senão o código da recusa). Com o original de antes da ida ao
    /// Sword/Shield guardado (D37), a lista é a da saída a partir dele, como o <see cref="TransferAsync"/> faz.
    /// </summary>
    public async Task<IReadOnlyDictionary<GameFamily, string?>> DestinationsAsync(StoredPokemon pokemon, CancellationToken ct)
    {
        if (pokemon.Family is not { } family)
            return GameCatalog.All.ToDictionary(g => g.Family, _ => (string?)"home_no_destination");
        var source = PokemonReader.ToEntity(await ReadDataAsync(pokemon, ct), pokemon.Format);
        return (await ReadOriginalAsync(pokemon, source, ct) is { } original
                ? KeptOriginal.Destinations(source, original.Pokemon, original.Family)
                : null)
            ?? HomeTransfer.Destinations(source, family);
    }

    /// <summary>
    /// Mover este Pokémon para o Sword/Shield perde a origem, e o cofre vai guardar o original (D37): a tela avisa
    /// antes. Falso quando o mover é recusado ou quando o Sword/Shield conhece a origem.
    /// </summary>
    public async Task<bool> KeepsOriginalInSwshAsync(StoredPokemon pokemon, CancellationToken ct)
    {
        if (pokemon.Family is null or GameFamily.Swsh)
            return false;
        var source = PokemonReader.ToEntity(await ReadDataAsync(pokemon, ct), pokemon.Format);
        var game = GameCatalog.Get(GameFamily.Swsh);
        var moved = HomeTransfer.Move(source, game, new SimpleTrainerInfo(GameCatalog.ToVersion(game.Versions[0])));
        return moved.Ok && HomeTransfer.OriginLostInSwsh(moved.Pokemon!);
    }

    /// <summary>
    /// O original guardado (D37) de um Pokémon que está no Sword/Shield, ou null: sem original, arquivo sumido ou
    /// adulterado, ou o registro já não é o mesmo Pokémon (<see cref="KeptOriginal.SamePokemon"/>).
    /// </summary>
    private async Task<(PKM Pokemon, GameFamily Family)?> ReadOriginalAsync(StoredPokemon pokemon, PKM current, CancellationToken ct)
    {
        if (pokemon.Family != GameFamily.Swsh || pokemon.OriginalSha256 is not { } sha || pokemon.OriginalFormat is not { } format
            || VaultFormats.Get(format).Family is not { } family)
            return null;
        try
        {
            var data = await File.ReadAllBytesAsync(FilePath(sha, format), ct);
            if (Convert.ToHexStringLower(SHA256.HashData(data)) != sha)
                return null;
            var original = PokemonReader.ToEntity(data, format);
            return KeptOriginal.SamePokemon(current, original) ? (original, family) : null;
        }
        catch (Exception e) when (e is IOException or InvalidPokemonFileException)
        {
            return null;
        }
    }

    /// <summary>
    /// Apaga do disco os arquivos que nenhum registro usa mais, em nenhum cofre: nem como o Pokémon de agora, nem como
    /// o original guardado de antes da ida ao Sword/Shield (D37). Chame depois de salvar as mudanças no banco.
    /// </summary>
    public async Task DeleteUnusedFilesAsync(
        TradeRockDb db, IEnumerable<(string Sha256, StorageFormat Format)> files, CancellationToken ct)
    {
        var candidates = files.Distinct().ToList();
        var hashes = candidates.Select(f => f.Sha256).Distinct().ToList();
        if (hashes.Count == 0)
            return;
        var current = await db.Pokemon.IgnoreQueryFilters().Where(p => hashes.Contains(p.Sha256))
            .Select(p => p.Sha256).ToListAsync(ct);
        var originals = await db.Pokemon.IgnoreQueryFilters()
            .Where(p => p.OriginalSha256 != null && hashes.Contains(p.OriginalSha256))
            .Select(p => p.OriginalSha256!).ToListAsync(ct);
        var used = current.Concat(originals).ToHashSet();
        foreach (var (sha, format) in candidates.Where(f => !used.Contains(f.Sha256)))
        {
            var path = FilePath(sha, format);
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    public async Task<IReadOnlyList<StoredPokemon>> ListAsync(CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Pokemon.AsNoTracking().Include(p => p.Tags).Where(p => p.DeletedAt == null)
            .OrderByDescending(p => p.AddedAt).ToListAsync(ct);
    }

    public async Task<StoredPokemon?> GetAsync(Guid id, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Pokemon.AsNoTracking().Include(p => p.Tags).SingleOrDefaultAsync(p => p.Id == id, ct);
    }

    /// <summary>Lê o arquivo do disco e confere o hash (arquivo adulterado ou corrompido = erro).</summary>
    public async Task<byte[]> ReadDataAsync(StoredPokemon pokemon, CancellationToken ct)
    {
        var data = await File.ReadAllBytesAsync(FilePath(pokemon.Sha256, pokemon.Format), ct);
        var hash = Convert.ToHexStringLower(SHA256.HashData(data));
        if (hash != pokemon.Sha256)
            throw new InvalidDataException($"O arquivo do Pokémon {pokemon.Id} não confere com o hash guardado.");
        return data;
    }

    /// <summary>Relatório de legalidade completo de um Pokémon guardado.</summary>
    public async Task<LegalityReport> GetLegalityReportAsync(StoredPokemon pokemon, string language, CancellationToken ct)
    {
        var data = await ReadDataAsync(pokemon, ct);
        return LegalityReporter.Analyze(PokemonReader.ToEntity(data, pokemon.Format), language);
    }

    /// <summary>
    /// Refaz a análise dos Pokémon sem resultado ou analisados por outra versão do PKHeX.
    /// Chamado na inicialização, em segundo plano.
    /// </summary>
    public async Task<int> RefreshLegalityAsync(CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var version = LegalityReporter.PkhexVersion;
        // Todos os cofres (roda na inicialização, fora de qualquer conta).
        var stale = await db.Pokemon.IgnoreQueryFilters().Where(p => p.Legality == null || p.LegalityPkhexVersion != version || p.Pid == null).ToListAsync(ct);
        foreach (var p in stale)
        {
            try
            {
                var data = await ReadDataAsync(p, ct);
                var summary = PokemonReader.Summarize(PokemonReader.ToEntity(data, p.Format), p.Format);
                p.Legality = AnalyzeLegality(data, p.Format);
                p.Fateful = summary.Fateful;
                p.Pid = summary.Pid;
            }
            catch (Exception e) when (e is IOException or InvalidDataException)
            {
                p.Legality = null; // arquivo sumido ou adulterado: fica sem análise
                continue;
            }
            p.LegalityPkhexVersion = version;
        }
        await db.SaveChangesAsync(ct);
        return stale.Count;
    }

    public static string AnalyzeLegality(byte[] data, StorageFormat format) =>
        LegalityReporter.Analyze(PokemonReader.ToEntity(data, format)).Status.ToString().ToLowerInvariant();

    public string FilePath(string sha256, StorageFormat format) =>
        Path.Combine(paths.VaultDir, sha256[..2], sha256 + "." + VaultFormats.Get(format).Extension);

    private async Task WriteFileAsync(PokemonFile pk, CancellationToken ct)
    {
        var target = FilePath(pk.Sha256, pk.Summary.Format);
        if (File.Exists(target))
            return;

        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        var temp = target + "." + Guid.NewGuid().ToString("N") + ".tmp";
        await File.WriteAllBytesAsync(temp, pk.Data, ct);
        File.Move(temp, target, overwrite: true);
    }
}

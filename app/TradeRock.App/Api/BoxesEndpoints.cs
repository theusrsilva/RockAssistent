using Microsoft.EntityFrameworkCore;
using PKHeX.Core;
using TradeRock.Core.Builder;
using TradeRock.Core.Games;
using TradeRock.Core.Pokemon;
using TradeRock.Core.Saves;
using TradeRock.Core.Storage;

namespace TradeRock.App.Api;

/// <summary>
/// Tela Boxes: saves das pastas do PC e o cofre lado a lado. Tudo aqui é só da janela local (os saves estão no PC).
/// Pokémon de save não existem no cofre: são identificados por (saveId, caixa, posição); caixa -1 = time.
/// Erros saem como { error: código } (404 para save/Pokémon que não existe, 409 para save que mudou no disco).
/// </summary>
public static class BoxesEndpoints
{
    public sealed record ApplyDto(IReadOnlyList<SaveChange>? Changes, string? Sha256);

    /// <param name="Kind">"save" ou "vault".</param>
    /// <param name="BoxId">Caixa do cofre no destino (null = Caixa de entrada).</param>
    public sealed record LocationDto(string? Kind, string? SaveId, int? Box, int? Slot, Guid? PokemonId, Guid? BoxId);

    /// <param name="Mode">"move", "copy" ou "attach".</param>
    public sealed record MoveDto(LocationDto? From, LocationDto? To, string? Mode);

    private sealed class BoxesException(string code, int status = StatusCodes.Status400BadRequest, string? reason = null) : Exception(code)
    {
        public string Code { get; } = code;
        public int Status { get; } = status;
        public string? Reason { get; } = reason;
    }

    public static void MapBoxes(this IEndpointRouteBuilder api)
    {
        var boxes = api.MapGroup("/boxes");

        boxes.MapGet("/saves", async (HttpRequest request, SaveLibrary library, CancellationToken ct) =>
        {
            var refresh = request.Query["refresh"].ToString() is "1" or "true";
            return Results.Ok(await library.ListAsync(refresh, ct));
        });

        boxes.MapGet("/saves/{id}", (string id, SaveLibrary library, CancellationToken ct) =>
            Run(() => library.ReadAsync(id, ct)));

        boxes.MapPost("/saves/{id}/apply", (string id, ApplyDto body, SaveLibrary library, CancellationToken ct) => Run(async () =>
        {
            var result = await library.ApplyAsync(id, body.Changes ?? [], ct, body.Sha256);
            return new { result.BackupPath, result.NewSha256, save = await library.ReadAsync(id, ct) };
        }));

        boxes.MapGet("/saves/{id}/slot/{box:int}/{slot:int}/file", async (string id, int box, int slot, SaveLibrary library,
            CancellationToken ct) =>
        {
            try
            {
                var entry = await SlotAsync(library, id, box, slot, ct);
                var (sav, _) = await library.OpenAsync(id, ct);
                var extension = SaveFileAccess.ToEntity(sav, entry.Data).GetType().Name.ToLowerInvariant();
                var name = new string(entry.Summary.SpeciesName.Where(char.IsLetterOrDigit).ToArray());
                return Results.File(entry.Data, "application/octet-stream", $"{(name.Length > 0 ? name : "pokemon")}-{entry.Summary.Pid:X8}.{extension}");
            }
            catch (Exception ex) when (ex is SaveInjectionException or BoxesException)
            {
                return Error(ex);
            }
        });

        // Evoluir por troca um Pokémon do save (Tarefa 18). Vale também no time (caixa -1).
        boxes.MapGet("/saves/{id}/slot/{box:int}/{slot:int}/evolve-options", (string id, int box, int slot, SaveLibrary library,
            CancellationToken ct) => Run(async () =>
        {
            var entry = await SlotAsync(library, id, box, slot, ct);
            var (sav, _) = await library.OpenAsync(id, ct);
            return VaultEndpoints.EvolveOptions(SaveFileAccess.ToEntity(sav, entry.Data));
        }));

        boxes.MapPost("/saves/{id}/slot/{box:int}/{slot:int}/evolve", (string id, int box, int slot, VaultEndpoints.EvolveDto body,
            SaveLibrary library, CancellationToken ct) => Run(async () =>
        {
            var current = await library.ReadAsync(id, ct);
            var entry = current.Slots.FirstOrDefault(e => e.Box == box && e.Slot == slot)
                ?? throw new BoxesException("slot_empty", StatusCodes.Status404NotFound);
            var (sav, _) = await library.OpenAsync(id, ct);
            PKM evolved;
            try
            {
                evolved = TradeEvolution.Apply(SaveFileAccess.ToEntity(sav, entry.Data), body.Species, body.Form);
            }
            catch (TradeEvolutionException ex)
            {
                throw new BoxesException(ex.Code);
            }
            // O SHA da tela vale mais; sem ele, o da leitura de onde veio o Pokémon.
            var written = await library.ApplyAsync(id, [new SaveChange(box, slot, Stored(evolved))], ct, body.Sha256 ?? current.Info.Sha256);
            return new
            {
                written.BackupPath, written.NewSha256, save = await library.ReadAsync(id, ct),
                legality = VaultEndpoints.LegalityDto(evolved, body.Language),
            };
        }));

        boxes.MapPost("/move", (MoveDto body, SaveLibrary library, PokemonStore store, VaultStore vault,
            IDbContextFactory<TradeRockDb> dbFactory, TimeProvider time, CancellationToken ct) =>
            Run(() => MoveAsync(body, library, store, vault, dbFactory, time, ct)));

        boxes.MapGet("/where/{species:int}", (int species, SaveLibrary library, PokemonStore store, VaultStore vault,
            CancellationToken ct) => Run(() => WhereAsync(species, library, store, vault, ct)));

        boxes.MapGet("/attachments", async (IDbContextFactory<TradeRockDb> dbFactory, CancellationToken ct) =>
        {
            await using var db = await dbFactory.CreateDbContextAsync(ct);
            var list = await db.SaveAttachments.AsNoTracking().OrderBy(a => a.AttachedAt).ToListAsync(ct);
            return list.Select(a => new
            {
                a.Id, a.PokemonId, saveId = SaveLibrary.IdOf(a.SavePath), a.SavePath, a.Box, a.Slot, a.AttachedAt,
            });
        });

        boxes.MapPost("/sync-attachments", (SaveLibrary library, PokemonStore store, IDbContextFactory<TradeRockDb> dbFactory,
            CancellationToken ct) => Run(() => SyncAsync(library, store, dbFactory, ct)));
    }

    // ---------- Mover / copiar / anexar ----------

    private static async Task<object> MoveAsync(MoveDto body, SaveLibrary library, PokemonStore store, VaultStore vault,
        IDbContextFactory<TradeRockDb> dbFactory, TimeProvider time, CancellationToken ct)
    {
        var mode = body.Mode ?? "copy";
        if (mode is not ("move" or "copy" or "attach") || body.From is not { } from || body.To is not { } to)
            throw new BoxesException("move_invalid");

        switch (from.Kind, to.Kind)
        {
            case ("vault", "save"):
                return await VaultToSaveAsync(from, to, mode, library, store, vault, dbFactory, time, ct);
            case ("save", "vault"):
                return await SaveToVaultAsync(from, to, mode, library, store, vault, dbFactory, time, ct);
            case ("save", "save"):
                if (mode == "attach")
                    throw new BoxesException("attach_invalid");
                return await SaveToSaveAsync(from, to, mode, library, ct);
            default:
                throw new BoxesException("move_invalid");
        }
    }

    private static async Task<object> VaultToSaveAsync(LocationDto from, LocationDto to, string mode, SaveLibrary library,
        PokemonStore store, VaultStore vault, IDbContextFactory<TradeRockDb> dbFactory, TimeProvider time, CancellationToken ct)
    {
        if (from.PokemonId is not { } pokemonId)
            throw new BoxesException("move_invalid");
        var (saveId, box, slot) = Target(to);
        if (await store.GetAsync(pokemonId, ct) is not { DeletedAt: null } stored)
            throw new BoxesException("pokemon_not_found", StatusCodes.Status404NotFound);

        var (sav, info) = await library.OpenAsync(saveId, ct);
        await EnsureEmptyAsync(library, saveId, box, slot, ct);
        var source = PokemonReader.ToEntity(await store.ReadDataAsync(stored, ct), stored.Family);
        var (pk, notes, converted) = Convert(source, sav);
        if (mode == "attach" && converted)
            throw new BoxesException("attach_wrong_game");

        var data = Stored(pk);
        var written = await library.ApplyAsync(saveId, [new SaveChange(box, slot, data)], ct, info.Sha256);
        Guid? attachmentId = null;
        if (mode == "move")
            await vault.TrashAsync([pokemonId], ct);
        else if (mode == "attach")
            attachmentId = await AttachAsync(dbFactory, time, pokemonId, info.Path, written.NewSha256, box, slot, ct);

        return new
        {
            mode,
            to = new { kind = "save", saveId, box, slot },
            backupPath = written.BackupPath,
            notes,
            converted,
            legality = Legality(pk),
            attachmentId,
        };
    }

    private static async Task<object> SaveToVaultAsync(LocationDto from, LocationDto to, string mode, SaveLibrary library,
        PokemonStore store, VaultStore vault, IDbContextFactory<TradeRockDb> dbFactory, TimeProvider time, CancellationToken ct)
    {
        var (saveId, box, slot) = Source(from);
        var boxes = await library.ReadAsync(saveId, ct);
        var entry = boxes.Slots.FirstOrDefault(e => e.Box == box && e.Slot == slot) ?? throw new BoxesException("slot_empty");
        if (entry.Family is not { } family)
            throw new BoxesException("save_game_unsupported");

        if (to.BoxId is { } boxId && to.Slot is { } vaultSlot)
        {
            await using var db = await dbFactory.CreateDbContextAsync(ct);
            if (await db.Pokemon.AnyAsync(p => p.BoxId == boxId && p.Slot == vaultSlot && p.DeletedAt == null, ct))
                throw new BoxesException("slot_occupied");
        }

        StoredPokemon imported;
        try
        {
            imported = (await store.ImportAsync(entry.Data, PokemonOrigin.Imported, Path.GetFileName(boxes.Info.Path), ct, family)).Pokemon;
        }
        catch (InvalidPokemonFileException ex)
        {
            throw new BoxesException("invalid_pokemon", reason: ex.Code);
        }
        try
        {
            if (to.BoxId is not null)
                await vault.MoveAsync(imported.Id, to.BoxId, to.Slot, ct);
        }
        catch (VaultException ex)
        {
            throw new BoxesException(ex.Code);
        }

        string? backupPath = null;
        Guid? attachmentId = null;
        if (mode == "move")
            backupPath = (await library.ApplyAsync(saveId, [new SaveChange(box, slot, null)], ct, boxes.Info.Sha256)).BackupPath;
        else if (mode == "attach")
            attachmentId = await AttachAsync(dbFactory, time, imported.Id, boxes.Info.Path, boxes.Info.Sha256, box, slot, ct);

        return new
        {
            mode,
            to = new { kind = "vault", pokemonId = imported.Id, boxId = to.BoxId, slot = to.BoxId is null ? (int?)null : to.Slot },
            backupPath,
            notes = Array.Empty<string>(),
            converted = false,
            legality = imported.Legality,
            attachmentId,
        };
    }

    private static async Task<object> SaveToSaveAsync(LocationDto from, LocationDto to, string mode, SaveLibrary library, CancellationToken ct)
    {
        var (fromId, fromBox, fromSlot) = Source(from);
        var (toId, toBox, toSlot) = Target(to);
        var source = await SlotAsync(library, fromId, fromBox, fromSlot, ct);
        var (fromSav, _) = await library.OpenAsync(fromId, ct);
        var (toSav, toInfo) = await library.OpenAsync(toId, ct);
        await EnsureEmptyAsync(library, toId, toBox, toSlot, ct);
        var (pk, notes, converted) = Convert(SaveFileAccess.ToEntity(fromSav, source.Data), toSav);
        var data = Stored(pk);

        var backups = new List<string>();
        if (fromId == toId)
        {
            // No mesmo save, uma gravação só (gravar e limpar juntos).
            IReadOnlyList<SaveChange> changes = mode == "move"
                ? [new SaveChange(toBox, toSlot, data), new SaveChange(fromBox, fromSlot, null)]
                : [new SaveChange(toBox, toSlot, data)];
            backups.Add((await library.ApplyAsync(toId, changes, ct, toInfo.Sha256)).BackupPath);
        }
        else
        {
            // Grava o destino antes de limpar a origem: se algo falhar no meio, sobra uma cópia, nunca some.
            backups.Add((await library.ApplyAsync(toId, [new SaveChange(toBox, toSlot, data)], ct, toInfo.Sha256)).BackupPath);
            if (mode == "move")
                backups.Add((await library.ApplyAsync(fromId, [new SaveChange(fromBox, fromSlot, null)], ct)).BackupPath);
        }

        return new
        {
            mode,
            to = new { kind = "save", saveId = toId, box = toBox, slot = toSlot },
            backupPaths = backups,
            notes,
            converted,
            legality = Legality(pk),
        };
    }

    /// <summary>
    /// Pokémon no formato do save. Mesmo formato: direto. Formato diferente: conversão do HOME (<see cref="HomeTransfer"/>)
    /// quando o save é de um jogo do catálogo; senão (ou se o HOME recusar) "save_wrong_game" com o motivo.
    /// </summary>
    private static (PKM Pokemon, IReadOnlyList<string> Notes, bool Converted) Convert(PKM source, SaveFile sav)
    {
        if (source.Context == sav.Context && source.GetType() == sav.BlankPKM.GetType())
            return (source, [], false);
        if (GameCatalog.ByContext(sav.Context) is not { } game)
            throw new BoxesException("save_wrong_game", reason: "home_no_destination");
        var moved = HomeTransfer.Move(source, game, sav);
        if (!moved.Ok)
            throw new BoxesException("save_wrong_game", reason: moved.Refusal ?? "no_route");
        return (moved.Pokemon!, moved.Notes, true);
    }

    private static async Task EnsureEmptyAsync(SaveLibrary library, string saveId, int box, int slot, CancellationToken ct)
    {
        var boxes = await library.ReadAsync(saveId, ct);
        if (boxes.Slots.Any(e => e.Box == box && e.Slot == slot))
            throw new BoxesException("slot_occupied");
    }

    private static async Task<Guid> AttachAsync(IDbContextFactory<TradeRockDb> dbFactory, TimeProvider time, Guid pokemonId,
        string savePath, string saveSha, int box, int slot, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        // Um Pokémon do cofre fica anexado a uma posição só.
        await db.SaveAttachments.Where(a => a.PokemonId == pokemonId).ExecuteDeleteAsync(ct);
        var attachment = new SaveAttachment
        {
            Id = Guid.CreateVersion7(),
            PokemonId = pokemonId,
            SavePath = savePath,
            SaveSha256 = saveSha,
            Box = box,
            Slot = slot,
            AttachedAt = time.GetUtcNow(),
        };
        db.SaveAttachments.Add(attachment);
        await db.SaveChangesAsync(ct);
        return attachment.Id;
    }

    // ---------- Onde está ----------

    private static async Task<object> WhereAsync(int species, SaveLibrary library, PokemonStore store, VaultStore vault, CancellationToken ct)
    {
        var result = new List<object>();
        foreach (var save in (await library.ListAsync(false, ct)).Where(e => e.Info is not null))
        {
            SaveBoxes boxes;
            try
            {
                boxes = await library.ReadAsync(save.Id, ct);
            }
            catch (SaveInjectionException)
            {
                continue; // mudou ou sumiu desde a varredura
            }
            foreach (var e in boxes.Slots.Where(e => e.Summary.Species == species))
                result.Add(new
                {
                    kind = "save", saveId = save.Id, pokemonId = (Guid?)null, name = Path.GetFileName(save.Path),
                    game = boxes.Info.GameKey, box = (int?)e.Box, boxId = (Guid?)null, slot = (int?)e.Slot,
                    level = e.Summary.Level, shiny = e.Summary.IsShiny,
                });
        }

        var boxNames = (await vault.OverviewAsync(ct)).Boxes.Where(b => b.Id is not null).ToDictionary(b => b.Id!.Value, b => b.Name);
        foreach (var p in (await store.ListAsync(ct)).Where(p => p.Species == species))
            result.Add(new
            {
                kind = "vault", saveId = (string?)null, pokemonId = (Guid?)p.Id,
                name = p.BoxId is { } id && boxNames.TryGetValue(id, out var n) ? n : null,
                game = GameCatalog.Get(p.Family).Key, box = (int?)null, boxId = p.BoxId, slot = p.Slot,
                level = p.Level, shiny = p.IsShiny,
            });
        return result;
    }

    // ---------- Anexos ----------

    /// <summary>
    /// Relê os saves dos anexos. Save igual ao da última conferência: "unchanged". Mesmo Pokémon (PID, TID e nome do
    /// treinador) com dados novos: substitui no cofre ("updated"). Outro Pokémon ou posição vazia: "slot_changed" (o
    /// cofre fica como está). Save sumido ou ilegível: "missing".
    /// </summary>
    private static async Task<object> SyncAsync(SaveLibrary library, PokemonStore store, IDbContextFactory<TradeRockDb> dbFactory,
        CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var attachments = await db.SaveAttachments.ToListAsync(ct);
        var entries = await library.ListAsync(true, ct);
        var results = new List<object>();
        foreach (var a in attachments)
        {
            var status = await SyncOneAsync(a, entries, library, store, ct);
            results.Add(new { attachmentId = a.Id, a.PokemonId, saveId = SaveLibrary.IdOf(a.SavePath), status });
        }
        await db.SaveChangesAsync(ct);
        return results;
    }

    private static async Task<string> SyncOneAsync(SaveAttachment a, IReadOnlyList<SaveLibraryEntry> entries, SaveLibrary library,
        PokemonStore store, CancellationToken ct)
    {
        var entry = entries.FirstOrDefault(e => e.Path == a.SavePath);
        if (entry?.Info is null)
            return "missing";
        if (entry.Info.Sha256 == a.SaveSha256)
            return "unchanged";
        if (await store.GetAsync(a.PokemonId, ct) is not { DeletedAt: null } stored)
            return "pokemon_missing";

        SaveBoxes boxes;
        try
        {
            boxes = await library.ReadAsync(entry.Id, ct);
        }
        catch (SaveInjectionException)
        {
            return "missing";
        }
        var slot = boxes.Slots.FirstOrDefault(e => e.Box == a.Box && e.Slot == a.Slot);
        if (slot is null || slot.Family != stored.Family)
            return "slot_changed";

        var current = PokemonReader.ToEntity(await store.ReadDataAsync(stored, ct), stored.Family);
        var (sav, _) = await library.OpenAsync(entry.Id, ct);
        var inSave = SaveFileAccess.ToEntity(sav, slot.Data);
        if (inSave.PID != current.PID || inSave.TID16 != current.TID16 || inSave.OriginalTrainerName != current.OriginalTrainerName)
            return "slot_changed";

        try
        {
            var before = stored.Sha256;
            var replaced = await store.ReplaceAsync(stored.Id, slot.Data, ct);
            // Só depois de substituir: se falhar, o anexo é tentado de novo na próxima sincronização.
            a.SaveSha256 = entry.Info.Sha256;
            return replaced.Sha256 == before ? "unchanged" : "updated";
        }
        catch (Exception ex) when (ex is VaultException or InvalidPokemonFileException)
        {
            return "slot_changed";
        }
    }

    // ---------- Apoio ----------

    private static (string SaveId, int Box, int Slot) Source(LocationDto from) =>
        from is { SaveId: { } id, Box: { } box, Slot: { } slot } ? (id, box, slot) : throw new BoxesException("move_invalid");

    private static (string SaveId, int Box, int Slot) Target(LocationDto to) =>
        to is { SaveId: { } id, Box: { } box, Slot: { } slot } ? (id, box, slot) : throw new BoxesException("move_invalid");

    private static async Task<SaveSlotEntry> SlotAsync(SaveLibrary library, string id, int box, int slot, CancellationToken ct) =>
        (await library.ReadAsync(id, ct)).Slots.FirstOrDefault(e => e.Box == box && e.Slot == slot)
        ?? throw new BoxesException("slot_empty", StatusCodes.Status404NotFound);

    private static byte[] Stored(PKM pk)
    {
        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);
        return data;
    }

    private static string Legality(PKM pk) => LegalityReporter.Analyze(pk).Status.ToString().ToLowerInvariant();

    private static async Task<IResult> Run<T>(Func<Task<T>> action)
    {
        try
        {
            return Results.Ok(await action());
        }
        catch (Exception ex) when (ex is SaveInjectionException or BoxesException)
        {
            return Error(ex);
        }
    }

    private static IResult Error(Exception ex)
    {
        var (code, status, reason) = ex switch
        {
            BoxesException b => (b.Code, b.Status, b.Reason),
            SaveInjectionException s => (s.Code, s.Code switch
            {
                "save_not_found" => StatusCodes.Status404NotFound,
                "save_changed" => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest,
            }, (string?)null),
            _ => throw ex,
        };
        return Results.Json(new { error = code, reason }, statusCode: status);
    }
}

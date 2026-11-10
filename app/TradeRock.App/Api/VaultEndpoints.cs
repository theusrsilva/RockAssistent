using PKHeX.Core;
using TradeRock.App.Server;
using System.IO.Compression;
using TradeRock.Core.Builder;
using TradeRock.Core.Games;
using TradeRock.Core.Pokemon;
using TradeRock.Core.Saves;
using TradeRock.Core.Storage;

namespace TradeRock.App.Api;

public static class VaultEndpoints
{
    public sealed record NameDto(string Name);
    public sealed record OrderDto(IReadOnlyList<Guid> Ids);
    public sealed record MoveDto(Guid? BoxId, int? Slot);
    public sealed record IdsDto(IReadOnlyList<Guid> Ids);
    public sealed record TagDto(string Name, string Color);
    public sealed record AssignTagsDto(IReadOnlyList<Guid> PokemonIds, IReadOnlyList<Guid>? Add, IReadOnlyList<Guid>? Remove);
    public sealed record ReplaceDto(byte[] Data);

    /// <param name="Sha256">Só nos saves: o SHA do save aberto na tela (409 save_changed se mudou no disco).</param>
    /// <param name="Language">Idioma das mensagens de legalidade ("pt-BR" ou "en").</param>
    public sealed record EvolveDto(ushort Species, byte Form, string? Sha256 = null, string? Language = null);

    public sealed record CloneDto(string? Language = null);

    /// <summary>Jogo de destino (chave do catálogo: sv, swsh, za...).</summary>
    public sealed record TransferDto(string? Game);

    /// <summary>Jogo de destino da cópia (chave do catálogo) e o idioma das mensagens de legalidade.</summary>
    public sealed record CopyDto(string? Game, string? Language = null);

    /// <summary>Formato de destino (chave: gen4 a gen7, swsh, bdsp, pla, sv, za) e o idioma das mensagens de legalidade.</summary>
    public sealed record AdaptDto(string? Target, string? Language = null);
    public sealed record ExportDto(IReadOnlyList<Guid> Ids, string Format = "pk3", int Size = 100);

    public static void MapVault(this IEndpointRouteBuilder api)
    {
        var vault = api.MapGroup("/vault");

        vault.MapGet("/", (VaultStore store, CancellationToken ct) => Run(() => store.OverviewAsync(ct)));

        vault.MapPost("/boxes", (NameDto body, VaultStore store, CancellationToken ct) => Run(() => store.CreateBoxAsync(body.Name, ct)));
        vault.MapPut("/boxes/order", (OrderDto body, VaultStore store, CancellationToken ct) => Run(() => store.ReorderBoxesAsync(body.Ids, ct)));
        vault.MapPut("/boxes/{id:guid}", (Guid id, NameDto body, VaultStore store, CancellationToken ct) => Run(() => store.RenameBoxAsync(id, body.Name, ct)));
        vault.MapDelete("/boxes/{id:guid}", (Guid id, VaultStore store, CancellationToken ct) => Run(() => store.DeleteBoxAsync(id, ct)));

        vault.MapGet("/pokemon", async (HttpRequest request, VaultStore store, SettingsStore settings, CancellationToken ct) =>
        {
            var q = request.Query;
            var query = new VaultQuery
            {
                Scope = q["scope"].FirstOrDefault(),
                Text = q["text"].FirstOrDefault(),
                Shiny = bool.TryParse(q["shiny"], out var shiny) ? shiny : null,
                Legality = q["legality"].FirstOrDefault(),
                Origin = Enum.TryParse<PokemonOrigin>(q["origin"], true, out var origin) ? origin : null,
                TagIds = q["tags"].FirstOrDefault()?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => Guid.TryParse(t, out var g) ? g : Guid.Empty).Where(g => g != Guid.Empty).ToList(),
                MinLevel = byte.TryParse(q["minLevel"], out var min) ? min : null,
                MaxLevel = byte.TryParse(q["maxLevel"], out var max) ? max : null,
                Format = q["game"].FirstOrDefault() is { } key ? VaultFormats.ByKey(key)?.Format : null,
            };
            var me = await settings.GetAsync<MyTrainer?>(UserSettings.Key(BuilderEndpoints.MyTrainerKey), null, ct);
            return await Run(async () =>
            {
                var duplicates = await store.PossibleDuplicatesAsync(ct);
                var list = (await store.SearchAsync(query, ct))
                    .Select(p => PokemonEndpoints.PokemonDto.From(p, me, duplicates.Contains(p.Id)));
                if (q["warnings"] == "true")
                    list = list.Where(p => p.Warnings.Count > 0);
                if (q["duplicates"] == "true")
                    list = list.Where(p => p.PossibleDuplicate);
                return list.ToList();
            });
        });

        vault.MapPost("/pokemon/{id:guid}/move", (Guid id, MoveDto body, VaultStore store, CancellationToken ct) =>
            Run(() => store.MoveAsync(id, body.BoxId, body.Slot, ct)));

        // Edição no painel do cofre: os dados novos (gerados pelo construtor) substituem os do Pokémon.
        vault.MapPost("/pokemon/{id:guid}/replace", async (Guid id, ReplaceDto body, PokemonStore store, SettingsStore settings,
            CancellationToken ct) =>
        {
            if (body.Data is not { Length: > 0 })
                return Results.BadRequest(new { error = "invalid", errors = new[] { "missing_fields" } });
            try
            {
                var replaced = await store.ReplaceAsync(id, body.Data, ct);
                var me = await settings.GetAsync<MyTrainer?>(UserSettings.Key(BuilderEndpoints.MyTrainerKey), null, ct);
                return Results.Ok(PokemonEndpoints.PokemonDto.From(replaced, me));
            }
            catch (VaultException ex)
            {
                return Results.BadRequest(new { error = ex.Code });
            }
            catch (InvalidPokemonFileException ex)
            {
                return Results.BadRequest(new { error = "invalid", errors = new[] { ex.Code } });
            }
        });

        // Resumo de leitura de qualquer formato (D34): o construtor só abre os jogos do catálogo de troca, então o painel
        // do cofre usa isto nos Pokémon da Gen 1, 2 e 4 a 7. Nomes em inglês, como os golpes e itens no resto do app.
        vault.MapGet("/pokemon/{id:guid}/view", async (Guid id, PokemonStore store, CancellationToken ct) =>
        {
            if (await store.GetAsync(id, ct) is not { } p)
                return Results.NotFound();
            var pk = PokemonReader.ToEntity(await store.ReadDataAsync(p, ct), p.Format);
            return Results.Ok(ViewDto(pk));
        });

        // Evoluir por troca sem trocar (Tarefa 18). A entrada continua a mesma (id, caixa, etiquetas, histórico); o
        // Pokémon de antes vai para a lixeira, de onde dá para recuperar.
        vault.MapGet("/pokemon/{id:guid}/evolve-options", async (Guid id, PokemonStore store, CancellationToken ct) =>
        {
            if (await store.GetAsync(id, ct) is not { DeletedAt: null } p)
                return Results.NotFound(new { error = "pokemon_not_found" });
            var pk = PokemonReader.ToEntity(await store.ReadDataAsync(p, ct), p.Format);
            return Results.Ok(EvolveOptions(pk));
        });

        vault.MapPost("/pokemon/{id:guid}/evolve", async (Guid id, EvolveDto body, PokemonStore store, VaultStore vault,
            SettingsStore settings, CancellationToken ct) =>
        {
            if (await store.GetAsync(id, ct) is not { DeletedAt: null } p)
                return Results.NotFound(new { error = "pokemon_not_found" });
            var original = await store.ReadDataAsync(p, ct);
            var pk = PokemonReader.ToEntity(original, p.Format);
            try
            {
                var evolved = TradeEvolution.Apply(pk, body.Species, body.Form);
                var replaced = await store.ReplaceAsync(id, SameSize(evolved, original.Length), ct);
                // Cópia do Pokémon de antes na lixeira (o SHA antigo ficou livre com a troca dos dados).
                var backup = await store.ImportAsync(original, p.Format, p.Origin, p.SourceFileName, ct);
                if (backup.Pokemon.Id != id)
                    await vault.TrashAsync([backup.Pokemon.Id], ct);
                var me = await settings.GetAsync<MyTrainer?>(UserSettings.Key(BuilderEndpoints.MyTrainerKey), null, ct);
                return Results.Ok(new
                {
                    pokemon = PokemonEndpoints.PokemonDto.From(replaced, me),
                    legality = LegalityDto(evolved, body.Language),
                    backupId = backup.Pokemon.Id == id ? (Guid?)null : backup.Pokemon.Id,
                });
            }
            catch (TradeEvolutionException ex)
            {
                return Results.BadRequest(new { error = ex.Code });
            }
            catch (VaultException ex)
            {
                return Results.BadRequest(new { error = ex.Code });
            }
        });

        // Clonar no cofre: cópia com PID novo (EC novo na Gen 6+) na Caixa de entrada; a legalidade só avisa.
        vault.MapPost("/pokemon/{id:guid}/clone", async (Guid id, CloneDto? body, PokemonStore store, SettingsStore settings,
            CancellationToken ct) =>
        {
            if (await store.GetAsync(id, ct) is not { DeletedAt: null } p)
                return Results.NotFound(new { error = "pokemon_not_found" });
            var original = await store.ReadDataAsync(p, ct);
            try
            {
                var clone = PokemonClone.Clone(PokemonReader.ToEntity(original, p.Format));
                var result = await store.ImportAsync(SameSize(clone, original.Length), p.Format, p.Origin, p.SourceFileName, ct);
                if (!result.Created)
                    return Results.Conflict(new { error = "clone_exists" });
                var me = await settings.GetAsync<MyTrainer?>(UserSettings.Key(BuilderEndpoints.MyTrainerKey), null, ct);
                return Results.Ok(new
                {
                    pokemon = PokemonEndpoints.PokemonDto.From(result.Pokemon, me),
                    legality = LegalityDto(clone, body?.Language),
                });
            }
            catch (PokemonCloneException ex)
            {
                return Results.BadRequest(new { error = ex.Code });
            }
            catch (VaultException ex)
            {
                return Results.BadRequest(new { error = ex.Code });
            }
        });

        // Mover entre jogos, como o Pokémon HOME (Entrega 14d): destinos possíveis e a transferência.
        vault.MapGet("/pokemon/{id:guid}/destinations", async (Guid id, PokemonStore store, CancellationToken ct) =>
        {
            if (await store.GetAsync(id, ct) is not { DeletedAt: null } p)
                return Results.NotFound();
            var destinations = await store.DestinationsAsync(p, ct);
            return Results.Ok(destinations.Select(d => new { game = d.Key, refusal = d.Value }));
        });

        vault.MapPost("/pokemon/{id:guid}/transfer", async (Guid id, TransferDto body, PokemonStore store, SettingsStore settings,
            CancellationToken ct) =>
        {
            if (GameCatalog.ByKey(body.Game ?? "") is not { } to)
                return Results.BadRequest(new { error = "game_invalid" });
            if (await store.GetAsync(id, ct) is not { } current)
                return Results.NotFound();
            // O "Meu treinador" do jogo de destino recebe o Pokémon (sem ele, o treinador padrão do motor).
            var me = await settings.GetAsync<MyTrainer?>(UserSettings.Key(BuilderEndpoints.MyTrainerKeyFor(to.Family)), null, ct);
            var owner = new SimpleTrainerInfo(GameCatalog.ToVersion(to.Versions[0]))
            {
                OT = me?.Name ?? TradeRock.Core.Trading.TrainerProfile.Default.Name,
                TID16 = me?.Tid ?? TradeRock.Core.Trading.TrainerProfile.Default.Tid,
                SID16 = me?.Sid ?? TradeRock.Core.Trading.TrainerProfile.Default.Sid ?? 0,
                Gender = me?.Gender ?? 0,
                Language = (int)LanguageID.English, // o "Meu treinador" não guarda o idioma do jogo
            };
            try
            {
                var (moved, notes) = await store.TransferAsync(id, to.Family, owner, ct);
                var mine = await settings.GetAsync<MyTrainer?>(UserSettings.Key(BuilderEndpoints.MyTrainerKey), null, ct);
                return Results.Ok(new { pokemon = PokemonEndpoints.PokemonDto.From(moved, mine), notes });
            }
            catch (VaultException ex)
            {
                return Results.BadRequest(new { error = ex.Code });
            }
        });

        // Copiar para ser válido no jogo (D36): quando o destino recusa a origem ("origin_not_accepted"), nasce um
        // Pokémon novo no jogo de destino, com o "Meu treinador" de lá. O original fica; a cópia vai para a Caixa de entrada.
        vault.MapPost("/pokemon/{id:guid}/copy-for-game", async (Guid id, CopyDto body, PokemonStore store, SettingsStore settings,
            CancellationToken ct) =>
        {
            if (GameCatalog.ByKey(body.Game ?? "") is not { } to)
                return Results.BadRequest(new { error = "game_invalid" });
            if (await store.GetAsync(id, ct) is not { DeletedAt: null })
                return Results.NotFound(new { error = "pokemon_not_found" });
            var language = body.Language is "en" ? "en" : "pt-BR";
            var trainer = await BuilderEndpoints.DefaultTrainerAsync(settings, ct, to.Versions[0]);
            try
            {
                var (stored, copy) = await store.CopyForGameAsync(id, to.Family, trainer, language, ct);
                var mine = await settings.GetAsync<MyTrainer?>(UserSettings.Key(BuilderEndpoints.MyTrainerKey), null, ct);
                var pk = copy.Pokemon!;
                var encounter = copy.Build!.Encounter;
                return Results.Ok(new
                {
                    pokemon = PokemonEndpoints.PokemonDto.From(stored, mine),
                    notes = copy.Notes,
                    // Onde e em que nível a cópia "nasceu": o encontro do jogo de destino que o AutoMod escolheu.
                    met = new
                    {
                        location = encounter?.LocationName,
                        level = (int)pk.MetLevel,
                        type = encounter?.Type,
                        version = pk.Version.ToString(),
                        ball = ViewText(GameInfo.GetStrings("en").balllist, pk.Ball),
                    },
                    legality = LegalityDto(pk, language),
                });
            }
            catch (GameCopyException ex)
            {
                return Results.BadRequest(new { error = ex.Code, reason = ex.Reason });
            }
        });

        // Adaptar para um jogo mais novo (D34): caminhos oficiais, o original fica e a cópia vai para a Caixa de entrada.
        vault.MapGet("/pokemon/{id:guid}/adapt-options", async (Guid id, PokemonStore store, CancellationToken ct) =>
        {
            if (await store.GetAsync(id, ct) is not { DeletedAt: null } p)
                return Results.NotFound(new { error = "pokemon_not_found" });
            return Results.Ok((await store.AdaptOptionsAsync(p, ct)).Select(o =>
            {
                var format = VaultFormats.Get(o.Target);
                return new
                {
                    target = format.Key,
                    generation = format.Generation,
                    tradable = format.Family is not null,
                    steps = o.Steps.Select(StepKey).ToList(),
                    refusal = o.Refusal,
                    refusedAt = o.RefusedAt is { } at && Enum.TryParse<TransferStep>(at, out var step) ? StepKey(step) : null,
                };
            }));
        });

        vault.MapPost("/pokemon/{id:guid}/adapt", async (Guid id, AdaptDto body, PokemonStore store, SettingsStore settings,
            CancellationToken ct) =>
        {
            if (VaultFormats.ByKey(body.Target ?? "") is not { } to)
                return Results.BadRequest(new { error = "target_invalid" });
            if (await store.GetAsync(id, ct) is not { DeletedAt: null })
                return Results.NotFound(new { error = "pokemon_not_found" });
            // Quem recebe: o "Meu treinador" do jogo de destino; nas Gen 4 a 7 (fora do catálogo), o "Meu treinador" geral.
            var key = to.Family is { } family ? BuilderEndpoints.MyTrainerKeyFor(family) : BuilderEndpoints.MyTrainerKey;
            var me = await settings.GetAsync<MyTrainer?>(UserSettings.Key(key), null, ct);
            var owner = new SimpleTrainerInfo(to.Context.GetSingleGameVersion())
            {
                OT = me?.Name ?? TradeRock.Core.Trading.TrainerProfile.Default.Name,
                TID16 = me?.Tid ?? TradeRock.Core.Trading.TrainerProfile.Default.Tid,
                SID16 = me?.Sid ?? TradeRock.Core.Trading.TrainerProfile.Default.Sid ?? 0,
                Gender = me?.Gender ?? 0,
                Language = (int)LanguageID.English, // o "Meu treinador" não guarda o idioma do jogo
            };
            try
            {
                var (adapted, notes, pk) = await store.AdaptAsync(id, to.Format, owner, ct);
                var mine = await settings.GetAsync<MyTrainer?>(UserSettings.Key(BuilderEndpoints.MyTrainerKey), null, ct);
                return Results.Ok(new
                {
                    pokemon = PokemonEndpoints.PokemonDto.From(adapted, mine),
                    notes,
                    legality = LegalityDto(pk, body.Language),
                });
            }
            catch (VaultException ex)
            {
                return Results.BadRequest(new { error = ex.Code });
            }
        });

        vault.MapPost("/trash", (IdsDto body, VaultStore store, CancellationToken ct) => Run(() => store.TrashAsync(body.Ids, ct)));
        vault.MapPost("/restore", (IdsDto body, VaultStore store, CancellationToken ct) => Run(() => store.RestoreAsync(body.Ids, ct)));
        vault.MapPost("/trash/empty", (VaultStore store, CancellationToken ct) => Run(() => store.EmptyTrashAsync(ct)));

        vault.MapGet("/tags", (VaultStore store, CancellationToken ct) => Run(() => store.ListTagsAsync(ct)));
        vault.MapPost("/tags", (TagDto body, VaultStore store, CancellationToken ct) => Run(() => store.SaveTagAsync(null, body.Name, body.Color, ct)));
        vault.MapPut("/tags/{id:guid}", (Guid id, TagDto body, VaultStore store, CancellationToken ct) => Run(() => store.SaveTagAsync(id, body.Name, body.Color, ct)));
        vault.MapDelete("/tags/{id:guid}", (Guid id, VaultStore store, CancellationToken ct) => Run(() => store.DeleteTagAsync(id, ct)));
        vault.MapPost("/tags/assign", (AssignTagsDto body, VaultStore store, CancellationToken ct) =>
            Run(() => store.SetTagsAsync(body.PokemonIds, body.Add ?? [], body.Remove ?? [], ct)));

        // Exportação em lote: .zip (modo navegador) ou arquivos gravados numa pasta escolhida (janela nativa).
        vault.MapPost("/export", async (ExportDto body, PokemonStore store, CancellationToken ct) =>
        {
            if (!IsValidExport(body))
                return Results.BadRequest(new { error = "export_invalid" });
            var files = await ExportFilesAsync(body, store, ct);
            using var zip = new MemoryStream();
            using (var archive = new ZipArchive(zip, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var (name, data) in files)
                {
                    var entry = archive.CreateEntry(name, CompressionLevel.Fastest);
                    await using var stream = entry.Open();
                    await stream.WriteAsync(data, ct);
                }
            }
            return Results.File(zip.ToArray(), "application/zip", $"traderock-{files.Count}.zip");
        });

        vault.MapPost("/export/native", async (ExportDto body, PokemonStore store, WindowBridge window, CancellationToken ct) =>
        {
            if (!IsValidExport(body))
                return Results.BadRequest(new { error = "export_invalid" });
            if (!window.HasWindow)
                return Results.Conflict(new { error = "no_window" });

            var files = await ExportFilesAsync(body, store, ct);
            var filter = (body.Format.ToUpperInvariant(), new[] { body.Format });
            var written = new List<string>();
            if (files.Count == 1)
            {
                var path = await window.PickSaveFileAsync("Exportar Pokémon", files[0].Name, [filter]);
                if (path is null)
                    return Results.NoContent();
                await File.WriteAllBytesAsync(path, files[0].Data, ct);
                written.Add(path);
            }
            else
            {
                var folder = await window.PickFolderAsync("Escolha a pasta para exportar");
                if (folder is null)
                    return Results.NoContent();
                foreach (var (name, data) in files)
                {
                    var path = UniquePath(Path.Combine(folder, name));
                    await File.WriteAllBytesAsync(path, data, ct);
                    written.Add(path);
                }
            }
            return Results.Ok(new { written });
        });
    }

    /// <summary>Etapa em snake_case, como os enums do JSON.</summary>
    private static string StepKey(TransferStep step) => step switch
    {
        TransferStep.VirtualConsole => "virtual_console",
        TransferStep.PalPark => "pal_park",
        TransferStep.PokeTransfer => "poke_transfer",
        TransferStep.PokeTransporter => "poke_transporter",
        TransferStep.PokemonBank => "pokemon_bank",
        _ => "home",
    };

    private static string? ViewText(IReadOnlyList<string> list, int index) => index > 0 && index < list.Count ? list[index] : null;

    /// <summary>Dados para o painel (texto pronto) de um Pokémon de qualquer geração.</summary>
    internal static object ViewDto(PKM pk)
    {
        var strings = GameInfo.GetStrings("en");
        var items = strings.GetItemStrings(pk.Context, pk.Version);
        string? Text(IReadOnlyList<string> list, int index) => index > 0 && index < list.Count ? list[index] : null;
        var ivs = new int[6];
        pk.GetIVs(ivs);
        var evs = new int[6];
        pk.GetEVs(evs);
        return new
        {
            moves = pk.Moves.Where(m => m > 0).Select(m => Text(strings.movelist, m) ?? $"#{m}").ToList(),
            ivs,
            evs,
            maxEv = pk.Format <= 2 ? 65535 : 255,
            // A Gen 1 e 2 não têm natureza nem habilidade.
            nature = pk.Format >= 3 ? Text(strings.natures, (int)pk.Nature) ?? strings.natures[0] : null,
            ability = pk.Format >= 3 ? Text(strings.abilitylist, pk.Ability) : null,
            heldItem = Text(items, pk.HeldItem),
            ball = pk.Format >= 3 ? Text(strings.balllist, pk.Ball) : null,
            language = ((LanguageID)pk.Language).ToString(),
            metLevel = pk.Format >= 3 ? (int?)pk.MetLevel : null,
        };
    }

    /// <summary>Opções de evolução por troca para a tela (nomes em inglês, como os itens no resto do app).</summary>
    internal static IReadOnlyList<object> EvolveOptions(PKM pk)
    {
        var strings = GameInfo.GetStrings("en");
        return TradeEvolution.Options(pk).Select(o => (object)new
        {
            species = o.Species,
            form = o.Form,
            speciesName = strings.specieslist[o.Species],
            requiredItem = o.RequiredItem,
            requiredItemName = o.RequiredItem is { } item ? strings.itemlist[TradeEvolution.NationalItem(pk, item)] : null,
            holdsItem = o.RequiredItem is { } held && pk.HeldItem == held,
            tradeWith = o.TradeWith,
            tradeWithName = o.TradeWith is { } with ? strings.specieslist[with] : null,
        }).ToList();
    }

    /// <summary>Veredito do PKHeX depois de evoluir. Só avisa: a evolução já foi gravada.</summary>
    internal static object LegalityDto(PKM pk, string? language)
    {
        var report = LegalityReporter.Analyze(pk, language is "en" ? "en" : "pt-BR");
        return new
        {
            status = report.Status.ToString().ToLowerInvariant(),
            valid = report.Status != LegalityStatus.Illegal,
            messages = report.Problems.Select(c => c.Message).Distinct().ToList(),
        };
    }

    /// <summary>Bytes no mesmo tamanho do arquivo de antes (o .pk3 do cofre pode ter 80 ou 100 bytes).</summary>
    private static byte[] SameSize(PKM pk, int length)
    {
        var data = new byte[length == pk.SIZE_PARTY ? pk.SIZE_PARTY : pk.SIZE_STORED];
        if (data.Length == pk.SIZE_PARTY)
            pk.WriteDecryptedDataParty(data);
        else
            pk.WriteDecryptedDataStored(data);
        return data;
    }

    private static bool IsValidExport(ExportDto body) =>
        body.Ids is { Count: > 0 and <= 500 } && Pk3Export.Formats.Contains(body.Format) && body.Size is 80 or 100;

    private static async Task<List<(string Name, byte[] Data)>> ExportFilesAsync(ExportDto body, PokemonStore store, CancellationToken ct)
    {
        var files = new List<(string, byte[])>();
        var names = new HashSet<string>();
        foreach (var id in body.Ids.Distinct())
        {
            if (await store.GetAsync(id, ct) is not { } p)
                continue;
            var (data, extension) = PokemonExport.ToFile(await store.ReadDataAsync(p, ct), p.Format, body.Format, body.Size);
            var name = Pk3Export.FileName(p.Species, string.IsNullOrWhiteSpace(p.Nickname) ? p.SpeciesName : p.Nickname, extension);
            var unique = name;
            for (var i = 2; !names.Add(unique); i++)
                unique = $"{Path.GetFileNameWithoutExtension(name)}-{i}{Path.GetExtension(name)}";
            files.Add((unique, data));
        }
        return files;
    }

    private static string UniquePath(string path)
    {
        var candidate = path;
        for (var i = 2; File.Exists(candidate); i++)
            candidate = Path.Combine(Path.GetDirectoryName(path)!, $"{Path.GetFileNameWithoutExtension(path)}-{i}{Path.GetExtension(path)}");
        return candidate;
    }

    /// <summary>VaultException vira 400 com o código; o resto segue normal.</summary>
    internal static async Task<IResult> Run<T>(Func<Task<T>> action)
    {
        try
        {
            return Results.Ok(await action());
        }
        catch (VaultException ex)
        {
            return Results.BadRequest(new { error = ex.Code });
        }
    }

    internal static async Task<IResult> Run(Func<Task> action)
    {
        try
        {
            await action();
            return Results.NoContent();
        }
        catch (VaultException ex)
        {
            return Results.BadRequest(new { error = ex.Code });
        }
    }
}

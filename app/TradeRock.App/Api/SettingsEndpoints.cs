using Microsoft.EntityFrameworkCore;
using TradeRock.App.Hotkeys;
using TradeRock.App.Server;
using TradeRock.Core.Games;
using TradeRock.Core.Engines;
using TradeRock.Core.Saves;
using TradeRock.Core.Storage;
using TradeRock.Core.Trading;
using TradeRock.Core.Vgc;

namespace TradeRock.App.Api;

public static class SettingsEndpoints
{
    public const string TradeSettingsKey = "trade.settings";

    /// <summary>Atalhos do Contador Hunt (por conta no modo servidor: <see cref="UserSettings.Key"/>).</summary>
    public const string HotkeysKey = "hotkeys";

    /// <summary>
    /// Atalhos de teclado do Contador Hunt. O combo usa o <c>KeyboardEvent.code</c> do navegador com modificadores
    /// na frente: <c>Space</c>, <c>Backspace</c>, <c>Ctrl+ArrowUp</c>, <c>Shift+Equal</c>. <c>Global</c>: valem também
    /// com o app em segundo plano (só na janela local, Windows e X11).
    /// </summary>
    public sealed record HotkeySettings(string Increment, string Decrement, bool Global = false)
    {
        public static readonly HotkeySettings Default = new("Space", "Backspace");
    }

    /// <summary>O que o GET devolve: os atalhos e se o atalho global funciona aqui (<c>reason</c> quando não).</summary>
    /// <param name="Active">O sistema está com as teclas agora (só com um contador aberto).</param>
    public sealed record HotkeySettingsDto(string Increment, string Decrement, bool Global, bool Supported, string? Reason, bool Active = false)
    {
        public static HotkeySettingsDto Local(HotkeySettings s, GlobalHotkeyService global)
        {
            // Configuração antiga com tecla de texto no global: não pega as teclas (vale o atalho da janela).
            var reason = global.Reason ?? (s.Global && (IsTextKey(s.Increment) || IsTextKey(s.Decrement)) ? "text_key" : null);
            return new(s.Increment, s.Decrement, s.Global, global.Supported, reason, global.Active);
        }

        public static HotkeySettingsDto Remote(HotkeySettings s) => new(s.Increment, s.Decrement, false, false, "remote");
    }

    /// <summary>Teclas que não viram atalho: Tab e Enter navegam e clicam, Esc fecha e cancela.</summary>
    private static readonly HashSet<string> ReservedKeys = new(StringComparer.OrdinalIgnoreCase) { "Tab", "Enter", "Escape" };

    private static readonly System.Text.RegularExpressions.Regex ComboPattern =
        new(@"^((Ctrl|Alt|Shift|Meta)\+){0,4}[A-Za-z][A-Za-z0-9]{0,24}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant);

    private static readonly System.Text.RegularExpressions.Regex TextKeyPattern =
        new(@"^(Space|Backspace|Enter|Tab|Delete|Key[A-Z]|Digit[0-9]|Equal|Minus|Numpad\w+)$", System.Text.RegularExpressions.RegexOptions.CultureInvariant);

    /// <summary>
    /// Tecla de texto sem Ctrl/Alt/Meta (Espaço, Backspace, letras, dígitos, =, -, teclado numérico; Shift+letra também).
    /// No atalho global ela sumiria de todos os programas enquanto o contador estiver aberto: o PUT recusa
    /// (<c>hotkey_text_key</c>).
    /// </summary>
    public static bool IsTextKey(string? combo)
    {
        if (string.IsNullOrEmpty(combo))
            return false;
        var parts = combo.Split('+');
        if (parts[..^1].Any(m => m is "Ctrl" or "Alt" or "Meta"))
            return false;
        return TextKeyPattern.IsMatch(parts[^1]);
    }

    /// <summary>Combo válido: tecla (código do navegador) depois de no máximo quatro modificadores, sem modificador sozinho.</summary>
    public static bool IsValidCombo(string? combo)
    {
        if (string.IsNullOrEmpty(combo) || combo.Length > 60 || !ComboPattern.IsMatch(combo))
            return false;
        var key = combo[(combo.LastIndexOf('+') + 1)..];
        return key is not ("Ctrl" or "Alt" or "Shift" or "Meta" or "Control") && !ReservedKeys.Contains(key);
    }

    /// <summary>Motor e os jogos que ele troca (chaves do catálogo: frlg, sv...).</summary>
    public sealed record EngineDto(string Id, bool IsSimulation, bool Available, string? ReasonCode, bool Selected, IReadOnlyList<string> Games,
        IReadOnlyList<string> PokeldnGames, bool NeedsSetup, bool IgnoresLinkCode);

    public sealed record SelectEngineDto(string Id);

    /// <summary>Treinador salvo, com os números que o jogo mostra (guardados como TID16/SID16).</summary>
    public sealed record TrainerPresetDto(Guid? Id, string Label, string Name, uint Tid, uint? Sid, TradeGame Game)
    {
        public static TrainerPresetDto From(TrainerPreset p)
        {
            var game = GameCatalog.Of(p.Game);
            var (tid, sid) = TrainerIds.Display(game, p.Tid, p.Sid ?? 0);
            return new(p.Id, p.Label, p.Name, tid, p.Sid is null && !game.SixDigitTrainerId ? null : sid, p.Game);
        }
    }

    public static void MapSettings(this IEndpointRouteBuilder api)
    {
        api.MapGet("/settings/trade", async (SettingsStore settings, CancellationToken ct) =>
            await settings.GetAsync(TradeSettingsKey, TradeSettings.Default, ct));

        api.MapPut("/settings/trade", async (TradeSettings body, SettingsStore settings, CancellationToken ct) =>
        {
            var normalized = body.Normalized();
            await settings.SetAsync(TradeSettingsKey, normalized, ct);
            return normalized;
        });

        // Atalhos do Contador Hunt: na janela local e, no modo servidor, por conta. O atalho global só na janela local.
        api.MapGet("/settings/hotkeys", async (HttpContext context, SettingsStore settings, GlobalHotkeyService global,
            CancellationToken ct) =>
        {
            var saved = await settings.GetAsync(UserSettings.Key(HotkeysKey), HotkeySettings.Default, ct);
            return Caller.Of(context).IsLocal ? HotkeySettingsDto.Local(saved, global) : HotkeySettingsDto.Remote(saved);
        });

        api.MapPut("/settings/hotkeys", async (HotkeySettings body, HttpContext context, SettingsStore settings,
            GlobalHotkeyService global, CancellationToken ct) =>
        {
            if (!IsValidCombo(body.Increment) || !IsValidCombo(body.Decrement))
                return Results.BadRequest(new { error = "hotkey_invalid" });
            if (string.Equals(body.Increment, body.Decrement, StringComparison.OrdinalIgnoreCase))
                return Results.BadRequest(new { error = "hotkeys_same" });
            var local = Caller.Of(context).IsLocal;
            if (body.Global && !local)
                return Results.Json(new { error = "local_only" }, statusCode: StatusCodes.Status403Forbidden);
            if (body.Global && (IsTextKey(body.Increment) || IsTextKey(body.Decrement)))
                return Results.BadRequest(new { error = "hotkey_text_key" });

            var saved = new HotkeySettings(body.Increment, body.Decrement, body.Global);
            if (local)
            {
                var previous = await settings.GetAsync(UserSettings.Key(HotkeysKey), HotkeySettings.Default, ct);
                if (saved.Global && !global.Supported)
                    return Results.Conflict(new { error = GlobalHotkeyException.GlobalUnsupported, reason = global.Reason });
                if (saved.Global || previous.Global)
                {
                    try
                    {
                        global.Apply(saved);
                    }
                    catch (GlobalHotkeyException ex)
                    {
                        // Nada salvo: volta o atalho que valia.
                        try { global.Apply(previous); } catch (GlobalHotkeyException) { }
                        return Results.Json(new { error = ex.Code },
                            statusCode: ex.Code == GlobalHotkeyException.Unsupported ? StatusCodes.Status400BadRequest : StatusCodes.Status409Conflict);
                    }
                }
            }
            await settings.SetAsync(UserSettings.Key(HotkeysKey), saved, ct);
            return Results.Ok(local ? HotkeySettingsDto.Local(saved, global) : HotkeySettingsDto.Remote(saved));
        });

        // Internet opcional (D32): as duas começam desligadas. Ler vale pela rede (a tela mostra o estado); mudar, só no PC.
        api.MapGet("/settings/internet", async (SettingsStore settings, CancellationToken ct) =>
            await settings.GetAsync(InternetSettings.Key, InternetSettings.Default, ct));

        api.MapPut("/settings/internet", async (InternetSettings body, SettingsStore settings, CancellationToken ct) =>
        {
            var saved = new InternetSettings(body.Smogon, body.Pokepaste);
            await settings.SetAsync(InternetSettings.Key, saved, ct);
            return saved;
        });

        // Pastas de saves da tela Boxes (só na janela local: são caminhos do PC).
        api.MapGet("/settings/save-folders", async (SaveFolders folders, CancellationToken ct) =>
            (await folders.GetAsync(ct)).Select(f => new { f.Path, f.Pattern, exists = Directory.Exists(f.Path) }));

        api.MapPut("/settings/save-folders", async (IReadOnlyList<SaveFolder> body, SaveFolders folders, SaveLibrary library,
            CancellationToken ct) =>
        {
            try
            {
                var saved = await folders.SetAsync(body, ct);
                await library.ListAsync(refresh: true, ct);
                return Results.Ok(saved.Select(f => new { f.Path, f.Pattern, exists = Directory.Exists(f.Path) }));
            }
            catch (SaveInjectionException ex)
            {
                return Results.BadRequest(new { error = ex.Code });
            }
        });

        api.MapGet("/engines", async (EngineRegistry registry, CancellationToken ct) =>
        {
            var selected = await registry.GetSelectedAsync(ct);
            var result = new List<EngineDto>();
            foreach (var engine in registry.All)
            {
                var availability = await engine.CheckAvailabilityAsync(ct);
                result.Add(new EngineDto(engine.Id, engine.IsSimulation, availability.Available,
                    availability.ReasonCode, engine.Id == selected.Id,
                    GameCatalog.All.Where(g => engine.Supports(g.Family)).Select(g => g.Key).ToList(),
                    GameCatalog.All.Where(g => engine.Supports(g.Family) && engine.UsesPokeldn(g.Family)).Select(g => g.Key).ToList(),
                    engine.NeedsSetup, engine.IgnoresLinkCode));
            }
            return result;
        });

        api.MapPut("/engines/selected", async (SelectEngineDto body, EngineRegistry registry, TradeCoordinator coordinator,
            TradeQueue queue, ServerMode server, CancellationToken ct) =>
        {
            if (coordinator.IsBusy || queue.WaitingCount > 0)
                return Results.Conflict(new { error = "busy" });
            var engine = registry.All.FirstOrDefault(e => e.Id == body.Id);
            if (engine is null)
                return Results.NotFound();
            var availability = await engine.CheckAvailabilityAsync(ct);
            if (!availability.Available)
                return Results.Conflict(new { error = "engine_unavailable", reason = availability.ReasonCode });
            // A placa Wi-Fi sai da rede durante a troca: não pode ser a interface do modo servidor.
            if (body.Id == PythonLdnEngine.EngineId && await server.ConflictsWithWifiAsync(ct))
                return Results.Conflict(new { error = "server_mode_on_radio" });
            await registry.SelectAsync(body.Id, ct);
            return Results.NoContent();
        });

        var presets = api.MapGroup("/trainer-presets");

        presets.MapGet("/", async (IDbContextFactory<TradeRockDb> dbFactory, CancellationToken ct) =>
        {
            await using var db = await dbFactory.CreateDbContextAsync(ct);
            var list = await db.TrainerPresets.AsNoTracking().OrderBy(p => p.Label).ToListAsync(ct);
            return list.Select(TrainerPresetDto.From);
        });

        presets.MapPost("/", async (TrainerPresetDto body, IDbContextFactory<TradeRockDb> dbFactory, CancellationToken ct) =>
        {
            if (!Enum.IsDefined(body.Game))
                return Results.BadRequest(new { error = "invalid", errors = new[] { "game_invalid" } });
            var game = GameCatalog.Of(body.Game);
            var errors = TrainerName.Validate(body.Name, game).ToList();
            if (string.IsNullOrWhiteSpace(body.Label) || body.Label.Length > 40)
                errors.Add("preset_label_invalid");
            var ids = TrainerIds.FromDisplay(game, body.Tid, body.Sid ?? 0);
            if (ids is null)
                errors.Add("trainer_id_invalid");
            if (errors.Count > 0)
                return Results.BadRequest(new { error = "invalid", errors });

            await using var db = await dbFactory.CreateDbContextAsync(ct);
            var preset = new TrainerPreset
            {
                Id = Guid.CreateVersion7(),
                Label = body.Label.Trim(),
                Name = body.Name,
                Tid = ids!.Value.Tid16,
                Sid = body.Sid is null && !game.SixDigitTrainerId ? null : ids.Value.Sid16,
                Game = body.Game,
            };
            db.TrainerPresets.Add(preset);
            await db.SaveChangesAsync(ct);
            return Results.Ok(TrainerPresetDto.From(preset));
        });

        presets.MapDelete("/{id:guid}", async (Guid id, IDbContextFactory<TradeRockDb> dbFactory, CancellationToken ct) =>
        {
            await using var db = await dbFactory.CreateDbContextAsync(ct);
            var deleted = await db.TrainerPresets.Where(p => p.Id == id).ExecuteDeleteAsync(ct);
            return deleted > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}

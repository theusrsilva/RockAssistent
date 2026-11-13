using TradeRock.App.Server;

namespace TradeRock.App.Api;

public static class AppEndpoints
{
    /// <summary>Último sprite embutido (Pecharunt, fim da Gen 9).</summary>
    public const int MaxSpriteSpecies = 1025;

    /// <param name="Supported">Há janela nativa e o pedido vem do próprio PC.</param>
    /// <param name="On">A janela está encolhida.</param>
    /// <param name="OnMinimize">Minimizar com uma hunt aberta vira mini janela.</param>
    public sealed record MiniWindowDto(bool Supported, bool On, bool OnMinimize);

    /// <summary>Só o que vier muda: ligar/desligar, a preferência, e se há uma hunt em andamento na tela.</summary>
    public sealed record MiniWindowChange(bool? On = null, bool? OnMinimize = null, bool? HuntOpen = null);

    private static bool LocalWindow(HttpContext context, WindowBridge window) =>
        window.HasWindow && Caller.Find(context) is { IsLocal: true };

    public static void MapAppEndpoints(this IEndpointRouteBuilder api)
    {
        var version = AppVersion.Current;

        // Pela rede, "window" é false: os diálogos nativos abririam no PC, não no celular.
        api.MapGet("/health", (HttpContext context, IHostEnvironment env, WindowBridge window) => Results.Ok(new
        {
            app = "RockAssistent",
            version,
            os = OperatingSystem.IsWindows() ? "windows" : OperatingSystem.IsLinux() ? "linux" : OperatingSystem.IsMacOS() ? "macos" : "other",
            environment = env.EnvironmentName,
            window = window.HasWindow && Caller.Find(context) is { IsLocal: true },
        }));

        // Chamado pelo frontend depois que o usuário confirma fechar durante uma troca.
        api.MapPost("/app/close", async (WindowBridge window) =>
        {
            await window.CloseAsync();
            return Results.NoContent();
        });

        // Mini janela do Contador Hunt (D40): só na janela do PC. "supported" false pela rede ou sem janela.
        api.MapGet("/app/mini-window", async (HttpContext context, WindowBridge window, TradeRock.Core.Storage.SettingsStore settings,
            CancellationToken ct) =>
        {
            var saved = await settings.GetAsync(MiniWindowSettings.Key, new MiniWindowSettings(), ct);
            return Results.Ok(new MiniWindowDto(LocalWindow(context, window), window.IsMini, saved.OnMinimize));
        });
        api.MapPut("/app/mini-window", async (MiniWindowChange body, HttpContext context, WindowBridge window,
            TradeRock.Core.Storage.SettingsStore settings, CancellationToken ct) =>
        {
            if (!LocalWindow(context, window))
                return Results.Conflict(new { error = "no_window" });
            var saved = await settings.GetAsync(MiniWindowSettings.Key, new MiniWindowSettings(), ct);
            if (body.OnMinimize is { } onMinimize && onMinimize != saved.OnMinimize)
            {
                saved = saved with { OnMinimize = onMinimize };
                await settings.SetAsync(MiniWindowSettings.Key, saved, ct);
            }
            if (body.HuntOpen is { } huntOpen)
                window.HuntOpen = huntOpen;
            if (body.On is { } on)
                await window.SetMiniAsync(on, ct);
            return Results.Ok(new MiniWindowDto(true, window.IsMini, saved.OnMinimize));
        });

        // Sprites do PokeAPI embutidos de assets/sprites (scripts/fetch-sprites.sh). "form" é o id do pokemon no PokeAPI
        // (> 10000: Mega, regionais...), ou os ids da lista da Pokédex (DexService): 100000 + id do pokemon_form para as
        // formas cosméticas (Unown, Vivillon...) e 200000 + espécie para a fêmea. Sem o arquivo pedido, tenta: shiny da
        // forma → forma → shiny da espécie → espécie.
        var assembly = typeof(AppEndpoints).Assembly;
        api.MapGet("/sprites/{species:int}", (int species, string? shiny, int? form, HttpContext context) =>
        {
            if (species is < 1 or > MaxSpriteSpecies)
                return Results.NotFound();
            var wantShiny = shiny is "1" or "true";
            var names = new List<string>(4);
            if (form is > 0)
            {
                var (folder, id) = form switch
                {
                    >= TradeRock.Core.Dex.DexService.FemaleFormBase => ("female", form.Value - TradeRock.Core.Dex.DexService.FemaleFormBase),
                    >= TradeRock.Core.Dex.DexService.CosmeticFormBase => ("cosmetic", form.Value - TradeRock.Core.Dex.DexService.CosmeticFormBase),
                    _ => ("forms", form.Value),
                };
                if (wantShiny)
                    names.Add($"sprites/{folder}/shiny/{id}.png");
                names.Add($"sprites/{folder}/{id}.png");
            }
            if (wantShiny)
                names.Add($"sprites/shiny/{species}.png");
            names.Add($"sprites/{species}.png");
            var stream = names.Select(assembly.GetManifestResourceStream).FirstOrDefault(s => s is not null);
            if (stream is null)
                return Results.NotFound();
            context.Response.Headers.CacheControl = "private, max-age=31536000, immutable";
            return Results.Stream(stream, "image/png");
        });
    }
}

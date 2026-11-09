using TradeRock.App.Server;

namespace TradeRock.App.Api;

public static class AppEndpoints
{
    /// <summary>Último sprite embutido (Pecharunt, fim da Gen 9).</summary>
    public const int MaxSpriteSpecies = 1025;

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

        // Sprites do PokeAPI embutidos de assets/sprites (scripts/fetch-sprites.sh). "form" é o id do pokemon no PokeAPI
        // (> 10000: Mega, regionais...). Sem o arquivo pedido, tenta: shiny da forma → forma → shiny da espécie → espécie.
        var assembly = typeof(AppEndpoints).Assembly;
        api.MapGet("/sprites/{species:int}", (int species, string? shiny, int? form, HttpContext context) =>
        {
            if (species is < 1 or > MaxSpriteSpecies)
                return Results.NotFound();
            var wantShiny = shiny is "1" or "true";
            var names = new List<string>(4);
            if (form is > 0)
            {
                if (wantShiny)
                    names.Add($"sprites/forms/shiny/{form}.png");
                names.Add($"sprites/forms/{form}.png");
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

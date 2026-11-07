using System.Text;
using TradeRock.Core.Builder;
using TradeRock.Core.Dex;
using TradeRock.Core.GameData;
using TradeRock.Core.Trading;
using TradeRock.Core.Vgc;

namespace TradeRock.App.Api;

/// <summary>Seção VGC dos guias: times sugeridos, regulamentação vigente e análise de um paste colado (sem internet).</summary>
public static class VgcEndpoints
{
    public sealed record AnalyzeDto(string? Paste, string? Game = TeamAnalysis.GameKey);

    /// <summary>Um Pokémon do paste que ficou fora da análise (espécie que não existe no SV ou além do 6º).</summary>
    public sealed record SkippedDto(string Text, IReadOnlyList<string> Problems);

    public static void MapVgc(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("/vgc");

        group.MapGet("/teams", (GameDataStore store) => store.VgcTeams.Select(t => new
        {
            t.Id,
            t.Name,
            t.Archetype,
            t.Regulation,
            t.DateIso,
            t.Paste,
            t.IdeaPt,
            t.IdeaEn,
            t.HowToPlayPt,
            t.HowToPlayEn,
            t.DangersPt,
            t.DangersEn,
            Members = VgcPaste.Members(t.Paste),
        }));

        group.MapGet("/regulation", (GameDataStore store) =>
            store.VgcRegulation is { } r ? Results.Ok(r) : Results.NotFound());

        group.MapPost("/analyze", async (AnalyzeDto body, DexService dex, CancellationToken ct) =>
        {
            if ((body.Game ?? TeamAnalysis.GameKey) != TeamAnalysis.GameKey)
                return Results.BadRequest(new { error = "invalid", errors = new[] { "game_invalid" } });
            if (string.IsNullOrWhiteSpace(body.Paste))
                return Results.BadRequest(new { error = "invalid", errors = new[] { "paste_invalid" } });
            if (Encoding.UTF8.GetByteCount(body.Paste) > VgcPaste.MaxBytes)
                return Results.BadRequest(new { error = "invalid", errors = new[] { "paste_too_big" } });

            // O treinador não importa aqui (só espécie, forma e golpes).
            var items = ShowdownImport.Parse(body.Paste, new TrainerSpec("VGC", 0, 0), TradeGame.Scarlet);
            var valid = items.Where(i => i.Spec is not null).ToList();
            if (valid.Count == 0)
                return Results.BadRequest(new { error = "invalid", errors = new[] { "paste_invalid" } });

            var team = valid.Take(6).Select(i => new TeamMember(i.Spec!.Species, i.Spec.Form, i.Spec.Moves)).ToList();
            var skipped = items.Where(i => i.Spec is null)
                .Concat(valid.Skip(6))
                .Select(i => new SkippedDto(i.Text.Split('\n')[0].Trim(), i.Spec is null ? i.Problems : ["team_too_big"]))
                .ToList();
            var report = await TeamAnalysis.AnalyzeAsync(team, dex, ct);
            return Results.Ok(new { report, skipped });
        });
    }
}

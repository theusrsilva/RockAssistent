using System.Globalization;

namespace TradeRock.Core.Vgc;

/// <summary>
/// Confere os dados do VGC: regulamentação com datas e regras, times com id único, da regulamentação vigente, textos nos
/// dois idiomas e um paste com 6 Pokémon diferentes, sem item repetido e com 1 a 4 golpes cada.
/// </summary>
internal static class VgcValidator
{
    public static IReadOnlyList<string> Validate(IReadOnlyList<VgcTeam> teams, VgcRegulation? regulation)
    {
        var errors = new List<string>();
        if (regulation is not null)
        {
            if (!IsDate(regulation.ValidFrom) || regulation.ValidTo is { } to && !IsDate(to) || !IsDate(regulation.CheckedOn))
                errors.Add("vgc/regulation: data fora do formato aaaa-mm-dd");
            if (regulation.Bring < regulation.Pick || regulation.Pick < 1 || regulation.Level < 1)
                errors.Add("vgc/regulation: bring/pick/level inválidos");
            if (regulation.RulesPt.Count == 0 || regulation.RulesPt.Count != regulation.RulesEn.Count)
                errors.Add("vgc/regulation: regras faltando ou diferentes entre pt e en");
            if (regulation.Source.Count == 0)
                errors.Add("vgc/regulation: sem fontes");
            if (regulation.SmogonFormat is { } format && !SmogonStats.IsValidFormat(format))
                errors.Add("vgc/regulation: smogon_format fora do formato (ex.: gen9championsvgc2026regmc-1760)");
        }
        if (teams.Count > 0 && regulation is null)
            errors.Add("vgc/teams: falta vgc/regulation.json");

        foreach (var dup in teams.GroupBy(t => t.Id).Where(g => g.Count() > 1))
            errors.Add($"vgc/teams: id repetido {dup.Key}");
        foreach (var team in teams)
        {
            var at = $"vgc/teams/{team.Id}";
            if (regulation is not null && team.Regulation != regulation.Name)
                errors.Add($"{at}: regulamentação {team.Regulation} (a vigente é {regulation.Name})");
            if (!IsDate(team.DateIso))
                errors.Add($"{at}: date_iso fora do formato aaaa-mm-dd");
            string[] texts = [team.Name, team.Archetype, team.IdeaPt, team.IdeaEn, team.HowToPlayPt, team.HowToPlayEn, team.DangersPt, team.DangersEn];
            if (texts.Any(string.IsNullOrWhiteSpace))
                errors.Add($"{at}: texto vazio");

            var sets = VgcPaste.Sets(team.Paste);
            if (sets.Count != 6)
                errors.Add($"{at}: o paste tem {sets.Count} Pokémon (precisa de 6)");
            foreach (var set in sets)
            {
                if (set.Species == 0)
                    errors.Add($"{at}: espécie não reconhecida em \"{set.Text.Split('\n')[0]}\"");
                if (set.InvalidLines.Count > 0)
                    errors.Add($"{at}: linhas não reconhecidas em \"{set.Text.Split('\n')[0]}\": {string.Join(" | ", set.InvalidLines.Select(l => l.Value))}");
                var moves = set.Moves.Count(m => m != 0);
                if (moves is < 1 or > 4)
                    errors.Add($"{at}: {moves} golpes em \"{set.Text.Split('\n')[0]}\"");
            }
            if (sets.GroupBy(s => s.Species).Any(g => g.Count() > 1))
                errors.Add($"{at}: espécie repetida (Species Clause)");
            if (sets.Where(s => s.HeldItem > 0).GroupBy(s => s.HeldItem).Any(g => g.Count() > 1))
                errors.Add($"{at}: item repetido (Item Clause)");
        }
        return errors;
    }

    private static bool IsDate(string value) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
}

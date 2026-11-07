using System.Globalization;
using TradeRock.Core.GameData;

namespace TradeRock.Core.Hunts;

/// <summary>Contas das odds de shiny (denominador: 4096 quer dizer 1/4096).</summary>
public static class ShinyOdds
{
    /// <summary>"Pesquisa nível 10" e "pesquisa perfeita" do Legends: Arceus não somam: a perfeita já inclui a de nível 10.</summary>
    public const string Research10 = "research_10";
    public const string ResearchPerfect = "research_perfect";

    /// <summary>Denominador da chance de pelo menos uma de <paramref name="rolls"/> rolagens de 1/<paramref name="rollOdds"/>.</summary>
    public static double FromRolls(double rollOdds, int rolls) => 1 / (1 - Math.Pow(1 - 1 / rollOdds, rolls));

    /// <summary>
    /// Odd atual da hunt (o N de "1/N").
    /// </summary>
    /// <param name="modifiers">Id do modificador → <c>"true"</c>/<c>"false"</c> (toggle) ou o número em texto (steps).</param>
    /// <param name="count">Contador da hunt: é o valor do modificador marcado <c>counter</c>.</param>
    /// <param name="gameKey">Jogo da hunt: modificadores com <c>games</c> só valem nele. Nulo = todos valem.</param>
    /// <remarks>
    /// Método com rolagens (<c>base_rolls</c> e <c>roll_odds</c>): soma as <c>rolls</c> dos toggles ligados e do degrau
    /// de cada <c>steps</c> e aplica <see cref="FromRolls"/>. Método sem rolagens: as odds do toggle ligado ou do degrau
    /// substituem a base (com mais de um, vale a melhor, a menor). Degrau que não cobre o valor não muda nada.
    /// </remarks>
    public static double Current(ShinyMethod method, IReadOnlyDictionary<string, string> modifiers, int count, string? gameKey = null)
    {
        var byRolls = method.BaseRolls is not null && method.RollOdds is not null;
        var perfect = IsOn(modifiers, ResearchPerfect);
        var extra = 0;
        double? replaced = null;

        foreach (var mod in method.Modifiers)
        {
            if (gameKey is not null && mod.Games is { } games && !games.Contains(gameKey))
                continue;
            int? rolls;
            double odds;
            if (mod.Kind == "toggle")
            {
                if (!IsOn(modifiers, mod.Id) || (mod.Id == Research10 && perfect) || mod.Odds is not { } o)
                    continue;
                (rolls, odds) = (mod.Rolls, o);
            }
            else
            {
                var value = mod.Counter ? count : StepValue(modifiers, mod.Id);
                if (value is null || mod.Steps?.FirstOrDefault(s => s.Min <= value && (s.Max is null || value <= s.Max)) is not { } step)
                    continue;
                (rolls, odds) = (step.Rolls, step.Odds);
            }

            if (byRolls)
                extra += rolls ?? 0;
            else
                replaced = Math.Min(replaced ?? double.MaxValue, odds);
        }

        if (byRolls)
            return FromRolls(method.RollOdds!.Value, method.BaseRolls!.Value + extra);
        return replaced ?? method.BaseOdds;
    }

    /// <summary>Chance de já ter achado pelo menos um em <paramref name="count"/> tentativas de 1/<paramref name="oddsN"/>.</summary>
    public static double Cumulative(double oddsN, int count)
    {
        if (count <= 0)
            return 0;
        if (oddsN <= 1)
            return 1;
        return 1 - Math.Pow(1 - 1 / oddsN, count);
    }

    public static bool IsOn(IReadOnlyDictionary<string, string> modifiers, string id) =>
        modifiers.TryGetValue(id, out var v) && bool.TryParse(v, out var on) && on;

    private static int? StepValue(IReadOnlyDictionary<string, string> modifiers, string id) =>
        modifiers.TryGetValue(id, out var v) && int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : null;
}

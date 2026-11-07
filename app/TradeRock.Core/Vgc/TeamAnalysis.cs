using PKHeX.Core;
using TradeRock.Core.Dex;

namespace TradeRock.Core.Vgc;

/// <summary>Um Pokémon do time para a análise: espécie, forma (a do PKHeX) e golpes (IDs nacionais).</summary>
public sealed record TeamMember(ushort Species, byte Form, IReadOnlyList<ushort> Moves);

public sealed record MoveReport(int Id, string Name, string Type, string? Class, int? Power);

public sealed record MemberReport(ushort Species, byte Form, string Name, IReadOnlyList<string> Types, int BaseSpeed,
    IReadOnlyList<MoveReport> Moves);

/// <summary>
/// Um tipo de ataque contra o time: quantos levam mais dano (<see cref="Weak"/>), menos (<see cref="Resist"/>) ou nenhum
/// (<see cref="Immune"/>). <see cref="Factors"/>: o multiplicador de cada membro, na ordem do time.
/// </summary>
public sealed record DefenseRow(string Type, int Weak, int Resist, int Immune, IReadOnlyList<double> Factors);

/// <summary>
/// Um tipo de defesa (Pokémon de um tipo só): o melhor multiplicador dos golpes de ataque do time e os tipos de golpe
/// que acertam super efetivo.
/// </summary>
public sealed record CoverageRow(string Type, double Best, IReadOnlyList<string> SuperEffectiveBy);

public sealed record SpeedRow(ushort Species, string Name, int BaseSpeed);

/// <summary>Resultado da análise: membros, defesa por tipo, cobertura ofensiva, tipos de ataque e velocidades base.</summary>
public sealed record TeamReport(
    IReadOnlyList<MemberReport> Members,
    IReadOnlyList<DefenseRow> Defense,
    IReadOnlyList<CoverageRow> Coverage,
    IReadOnlyList<string> AttackTypes,
    IReadOnlyList<SpeedRow> Speeds);

/// <summary>
/// Análise de um time sobre os dados do Scarlet/Violet (tipos e velocidade da forma no PKHeX, golpes e tabela de tipos
/// da Gen 9 do PokeAPI). Não conta habilidades (Levitate, Water Absorb...), itens nem a Mega Evolução.
/// </summary>
public static class TeamAnalysis
{
    public const string GameKey = "sv";
    private const int Generation = 9;

    // Os tipos do PKHeX (0 = Normal ... 17 = Fairy) seguem a ordem dos IDs do PokeAPI (1 ... 18).
    private static readonly string[] TypeNames =
    [
        "normal", "fighting", "flying", "poison", "ground", "rock", "bug", "ghost", "steel",
        "fire", "water", "grass", "electric", "psychic", "ice", "dragon", "dark", "fairy",
    ];

    /// <summary>Tipos de uma espécie/forma no SV (nomes do PokeAPI); vazio se a forma não existe no jogo.</summary>
    public static IReadOnlyList<string> TypesOf(ushort species, byte form) =>
        Entry(species, form) is IPersonalType t
            ? t.Type1 == t.Type2 ? [TypeNames[t.Type1]] : [TypeNames[t.Type1], TypeNames[t.Type2]]
            : [];

    public static async Task<TeamReport> AnalyzeAsync(IReadOnlyList<TeamMember> team, DexService dex, CancellationToken ct)
    {
        var chart = await dex.GetTypeChartAsync(Generation, ct);
        var moves = (await dex.MovesBattleInfoAsync(team.SelectMany(m => m.Moves).Where(m => m != 0).Select(m => (int)m), GameKey, ct))
            .ToDictionary(m => m.Id);
        var strings = GameInfo.GetStrings("en");

        var members = team.Select(m =>
        {
            var entry = Entry(m.Species, m.Form);
            var name = m.Species < strings.specieslist.Length ? strings.specieslist[m.Species] : $"#{m.Species}";
            var memberMoves = m.Moves.Where(id => moves.ContainsKey(id))
                .Select(id => moves[id])
                .Select(i => new MoveReport(i.Id, i.Name, i.Type, i.Class, i.Power))
                .ToList();
            return new MemberReport(m.Species, m.Form, name, TypesOf(m.Species, m.Form),
                entry is IBaseStat b ? b.SPE : 0, memberMoves);
        }).ToList();

        var defense = chart.Types.Select(attack =>
        {
            var factors = members.Select(m => m.Types.Aggregate(1.0, (f, def) => f * Factor(chart, attack, def))).ToList();
            return new DefenseRow(attack, factors.Count(f => f > 1), factors.Count(f => f is > 0 and < 1), factors.Count(f => f == 0), factors);
        }).ToList();

        // Golpes de ataque: os que não são de status (Low Kick e companhia não têm poder fixo, mas contam).
        var attackTypes = members.SelectMany(m => m.Moves)
            .Where(m => m.Class is not "status")
            .Select(m => m.Type)
            .Distinct()
            .OrderBy(t => Array.IndexOf(TypeNames, t))
            .ToList();

        var coverage = chart.Types.Select(def =>
        {
            var best = attackTypes.Count == 0 ? 0 : attackTypes.Max(a => Factor(chart, a, def));
            return new CoverageRow(def, best, attackTypes.Where(a => Factor(chart, a, def) > 1).ToList());
        }).ToList();

        var speeds = members.Select(m => new SpeedRow(m.Species, m.Name, m.BaseSpeed))
            .OrderByDescending(s => s.BaseSpeed)
            .ToList();

        return new TeamReport(members, defense, coverage, attackTypes, speeds);
    }

    private static double Factor(TypeChart chart, string attack, string defense) =>
        chart.Factors.TryGetValue(attack, out var row) && row.TryGetValue(defense, out var f) ? f : 1;

    private static IPersonalInfo? Entry(ushort species, byte form)
    {
        var table = PersonalTable.SV;
        if (species == 0 || species > table.MaxSpeciesID || !table.IsPresentInGame(species, form))
            return null;
        return table.GetFormEntry(species, form);
    }
}

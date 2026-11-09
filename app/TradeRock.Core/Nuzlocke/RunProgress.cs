using System.Text.Json;
using TradeRock.Core.Dex;
using TradeRock.Core.GameData;
using TradeRock.Core.Games;
using TradeRock.Core.Storage;

namespace TradeRock.Core.Nuzlocke;

/// <summary>Dados da run como vão para a API (regras como objeto).</summary>
public sealed record RunInfo(
    Guid Id,
    string Name,
    string GameKey,
    string? Version,
    string? Starter,
    NuzlockeRules Rules,
    string Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    string? CurrentAreaId);

/// <summary>
/// Pokémon da run como vai para a API. <see cref="Moves"/>: identificadores do PokeAPI. <see cref="IsStarter"/>: o inicial
/// que a run pôs sozinha (só usa a área quando ela é uma cidade).
/// </summary>
public sealed record MonInfo(
    Guid Id,
    string AreaId,
    int Species,
    int Form,
    string? Nickname,
    int Level,
    string Status,
    string? DeathCause,
    DateTimeOffset CaughtAt,
    DateTimeOffset? DiedAt,
    IReadOnlyList<string> Moves,
    bool Shiny,
    bool IsStarter = false);

/// <summary>Uma espécie que aparece na área, com os métodos, as versões e a faixa de nível juntados.</summary>
public sealed record SpeciesEncounterOption(
    int Species,
    int Form,
    string Name,
    IReadOnlyList<string> Methods,
    int MinLevel,
    int MaxLevel,
    IReadOnlyList<string> Versions);

/// <summary>
/// A tela da run. <see cref="CurrentArea"/>: a área atual (a primeira do jogo se a run ainda não mudou; null só se o
/// jogo não tem áreas). <see cref="LevelCap"/>: o do próximo chefe. <see cref="OverCap"/>: quem da equipe passou dele.
/// <see cref="Lost"/>: encontros perdidos (<c>missed</c> e <c>failed</c>). <see cref="VisitedAreas"/> e
/// <see cref="DefeatedBosses"/> servem ao mapa e ao cartão do chefe.
/// </summary>
public sealed record RunView(
    RunInfo Run,
    GameArea? CurrentArea,
    BossBattle? NextBoss,
    int? LevelCap,
    IReadOnlyList<SpeciesEncounterOption> AreaEncounters,
    IReadOnlyList<TrainerBattle> AreaTrainers,
    bool TrainersAvailable,
    IReadOnlyList<MonInfo> Party,
    IReadOnlyList<MonInfo> Box,
    IReadOnlyList<MonInfo> Graveyard,
    IReadOnlyList<MonInfo> OverCap,
    IReadOnlyList<MonInfo> Lost,
    IReadOnlyList<string> DefeatedBosses,
    IReadOnlyList<string> VisitedAreas);

/// <summary>Linha da lista de runs.</summary>
public sealed record RunSummary(
    Guid Id,
    string Name,
    string GameKey,
    string? Version,
    string Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    string? CurrentAreaId,
    int PartyCount,
    int DeadCount,
    int BossesDefeated);

/// <summary>Calcula a tela da run a partir da run, dos Pokémon, dos eventos e dos dados dos jogos.</summary>
public static class RunProgress
{
    internal static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static async Task<RunView> ComputeAsync(NuzlockeRun run, IReadOnlyList<NuzlockeMon> mons, IReadOnlyList<NuzlockeEvent> events,
        GameDataStore data, DexService dex, CancellationToken ct)
    {
        var areas = data.Areas(run.GameKey);
        var area = CurrentArea(areas, run.CurrentAreaId);
        var defeated = DefeatedBosses(events);
        var boss = NextBoss(data.Bosses(run.GameKey), defeated, run.Version, run.Starter);
        int? cap = boss is { LevelCap: > 0 } ? boss.LevelCap : null;

        var all = mons.OrderBy(m => m.CaughtAt).Select(Info).ToList();
        var party = all.Where(m => m.Status == NuzlockeStatus.Party).ToList();
        var trainersAvailable = data.GamesWithTrainers.Contains(run.GameKey);
        var trainers = area is null || !trainersAvailable
            ? []
            : data.Trainers(run.GameKey).Where(t => t.Area == area.Id && InVersion(t.Versions, run.Version)).ToList();
        var encounters = area is null ? [] : await AreaEncountersAsync(run.GameKey, run.Version, area, data, dex, ct);

        var visited = new List<string>();
        void Visit(string? id)
        {
            if (id is not null && !visited.Contains(id))
                visited.Add(id);
        }
        Visit(areas.MinBy(a => a.Order)?.Id);
        foreach (var e in events.OrderBy(e => e.At).Where(e => e.Kind == NuzlockeEvents.AreaChanged))
            Visit(Read(e, "areaId"));
        foreach (var m in all)
            Visit(m.AreaId);
        Visit(area?.Id);

        return new RunView(
            RunInfo(run),
            area,
            boss,
            cap,
            encounters,
            trainers,
            trainersAvailable,
            party,
            all.Where(m => m.Status == NuzlockeStatus.Box).ToList(),
            all.Where(m => m.Status == NuzlockeStatus.Dead).OrderBy(m => m.DiedAt).ToList(),
            OverCap(party, cap),
            all.Where(m => m.Status is NuzlockeStatus.Missed or NuzlockeStatus.Failed).ToList(),
            defeated,
            visited);
    }

    /// <summary>A área com o id dado, senão a primeira do jogo (menor <c>Order</c>).</summary>
    public static GameArea? CurrentArea(IReadOnlyList<GameArea> areas, string? areaId) =>
        areas.FirstOrDefault(a => a.Id == areaId) ?? areas.MinBy(a => a.Order);

    /// <summary>
    /// O chefe de menor <c>Order</c> ainda não vencido. Com versão escolhida, só os chefes dela (os sem
    /// <c>Versions</c> valem para todas); sem versão, todos. Com inicial, o time (e o cap) da variante dele, quando
    /// a batalha tem uma (<see cref="BossBattle.ForStarter"/>); sem, o time padrão.
    /// </summary>
    public static BossBattle? NextBoss(IReadOnlyList<BossBattle> bosses, IReadOnlyCollection<string> defeated, string? version,
        string? starter = null) =>
        bosses.Where(b => !defeated.Contains(b.Id) && InVersion(b.Versions, version)).MinBy(b => b.Order)?.ForStarter(starter);

    /// <summary>Quem da equipe está acima do cap (sem cap, ninguém).</summary>
    public static IReadOnlyList<MonInfo> OverCap(IReadOnlyList<MonInfo> party, int? cap) =>
        cap is { } c ? party.Where(m => m.Level > c).ToList() : [];

    public static RunInfo RunInfo(NuzlockeRun run) => new(run.Id, run.Name, run.GameKey, run.Version, run.Starter, Rules(run), run.Status,
        run.StartedAt, run.EndedAt, run.CurrentAreaId);

    public static NuzlockeRules Rules(NuzlockeRun run) =>
        JsonSerializer.Deserialize<NuzlockeRules>(run.RulesJson, Json) ?? NuzlockeRules.Default;

    public static MonInfo Info(NuzlockeMon m) => new(m.Id, m.AreaId, m.Species, m.Form, m.Nickname, m.Level, m.Status,
        m.DeathCause, m.CaughtAt, m.DiedAt, JsonSerializer.Deserialize<List<string>>(m.MovesJson, Json) ?? [], m.Shiny, m.IsStarter);

    public static IReadOnlyList<string> DefeatedBosses(IReadOnlyList<NuzlockeEvent> events) =>
        events.OrderBy(e => e.At).Where(e => e.Kind == NuzlockeEvents.BossDefeated).Select(e => Read(e, "bossId"))
            .OfType<string>().Distinct().ToList();

    /// <summary>Vale na versão? (sem versão escolhida ou sem lista = vale.)</summary>
    public static bool InVersion(IReadOnlyList<string>? versions, string? version) =>
        version is null || versions is null || versions.Contains(version);

    private static string? Read(NuzlockeEvent e, string property)
    {
        using var doc = JsonDocument.Parse(e.Json);
        return doc.RootElement.TryGetProperty(property, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
    }

    /// <summary>
    /// Encontros da área, uma linha por espécie e forma. BDSP, Arceus, SV e Z-A: os nossos (<c>assets/data/encounters</c>);
    /// os outros: os do PokeAPI pelo <c>PokeApiLocationId</c> da área, nas versões do jogo (ou só na escolhida).
    /// </summary>
    private static async Task<IReadOnlyList<SpeciesEncounterOption>> AreaEncountersAsync(string gameKey, string? version, GameArea area,
        GameDataStore data, DexService dex, CancellationToken ct)
    {
        var rows = new List<(int Species, int Form, string Method, int Min, int Max, IReadOnlyList<string> Versions)>();
        var own = data.Encounters(gameKey);
        if (own.Count > 0)
        {
            foreach (var e in own.Where(e => e.Area == area.Id))
                rows.Add((e.Species, e.Form, e.Method, e.MinLevel, e.MaxLevel, e.Versions));
        }
        else if (area.PokeApiLocationId is { } location && DexDatabase.ResourceExists)
        {
            foreach (var e in await dex.LocationEncountersAsync(location, gameKey, ct))
                rows.Add((e.Species, e.Form, e.Method, e.MinLevel, e.MaxLevel, [e.Version]));
        }

        var order = GameVersions.ByKey(gameKey).PokeApiVersions.ToList();
        return rows
            .Select(r => r with { Versions = version is null ? r.Versions : r.Versions.Where(v => v == version).ToList() })
            .Where(r => r.Versions.Count > 0)
            .GroupBy(r => (r.Species, r.Form))
            .Select(g => new SpeciesEncounterOption(
                g.Key.Species,
                g.Key.Form,
                (DexDatabase.ResourceExists ? dex.SpeciesName(g.Key.Species) : null) ?? $"#{g.Key.Species}",
                g.Select(r => r.Method).Distinct().ToList(),
                g.Min(r => r.Min),
                g.Max(r => r.Max),
                g.SelectMany(r => r.Versions).Distinct().OrderBy(v => order.IndexOf(v) is var i and >= 0 ? i : int.MaxValue).ToList()))
            .OrderBy(o => o.Species)
            .ThenBy(o => o.Form)
            .ToList();
    }
}

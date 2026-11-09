using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TradeRock.Core.Dex;
using TradeRock.Core.GameData;
using TradeRock.Core.Games;
using TradeRock.Core.Storage;

namespace TradeRock.Core.Nuzlocke;

/// <summary>Erro de regra do Nuzlocke: <see cref="Code"/> vai para o JSON e <see cref="Status"/> é o HTTP.</summary>
public sealed class NuzlockeException(string code, int status = 400) : Exception(code)
{
    public string Code { get; } = code;
    public int Status { get; } = status;
}

/// <summary>Estados de um Pokémon da run.</summary>
public static class NuzlockeStatus
{
    public const string Party = "party";
    public const string Box = "box";
    public const string Dead = "dead";

    /// <summary>Encontro que não conta (pulado por cláusula, como espécie repetida): a área continua livre.</summary>
    public const string Missed = "missed";

    /// <summary>Encontro perdido (fugiu, desmaiou, sem encontro): a área conta como usada.</summary>
    public const string Failed = "failed";

    public static readonly IReadOnlyList<string> All = [Party, Box, Dead, Missed, Failed];

    /// <summary>Estados de quem foi capturado (contam para a cláusula de espécie).</summary>
    public static bool Caught(string status) => status is Party or Box or Dead;
}

/// <summary>Tipos de evento gravados.</summary>
public static class NuzlockeEvents
{
    public const string RunStarted = "run_started";
    public const string AreaChanged = "area_changed";
    public const string Encounter = "encounter";
    public const string MonUpdated = "mon_updated";
    public const string BossDefeated = "boss_defeated";
    public const string RunFinished = "run_finished";
}

/// <summary>
/// <see cref="Starter"/>: inicial do jogador (<c>GameStarters</c>), opcional. Com ele, a run já começa com o inicial na
/// equipe (apelido <see cref="StarterNickname"/>; obrigatório se a regra pede apelido).
/// </summary>
public sealed record NewRun(string? Name, string GameKey, string? Version, NuzlockeRules? Rules, string? Starter = null,
    string? StarterNickname = null);

/// <summary>
/// Campos nulos ficam como estão. <see cref="Status"/>: só <c>active</c> ou <c>paused</c> (o fim é pelo finish).
/// <see cref="Starter"/>: texto vazio apaga o inicial. <see cref="StarterNickname"/>: apelido do inicial quando ele entra
/// na equipe agora (inicial informado pela primeira vez).
/// </summary>
public sealed record RunChanges(string? Name, string? Status, NuzlockeRules? Rules, string? Starter = null,
    string? StarterNickname = null);

/// <summary>Encontro registrado. <see cref="Species"/> 0 = sem encontro (só com <c>missed</c>/<c>failed</c>).</summary>
public sealed record NewEncounter(string AreaId, int Species, int Form, string? Nickname, int Level, string Status, bool Shiny = false);

/// <summary>Campos nulos ficam como estão. Texto vazio apaga o apelido ou a causa da morte. Espécie/forma = evolução.</summary>
public sealed record MonChanges(string? Status, int? Level, string? Nickname, string? DeathCause, IReadOnlyList<string>? Moves,
    int? Species = null, int? Form = null);

/// <summary>Golpe por nível: identificador do PokeAPI, nome em inglês, tipo e nível (0 = ao evoluir).</summary>
public sealed record LearnsetMove(string Move, string Name, string Type, int Level);

/// <summary>Próxima evolução: como evolui, o que aprende ao evoluir e os golpes por nível acima do nível atual.</summary>
public sealed record EvolutionLearnset(int Species, string Name, IReadOnlyList<string> Conditions, IReadOnlyList<LearnsetMove> OnEvolve,
    IReadOnlyList<LearnsetMove> Upcoming);

/// <summary>
/// Golpes do Pokémon da run, no grupo de versões da run. <see cref="Known"/>: os por nível até o nível atual (para mostrar
/// nome e tipo dos golpes); <see cref="Upcoming"/>: os acima dele.
/// </summary>
public sealed record MonLearnset(int Species, int Level, IReadOnlyList<LearnsetMove> Known, IReadOnlyList<LearnsetMove> Upcoming,
    IReadOnlyList<EvolutionLearnset> Evolutions);

/// <summary>
/// Runs de Nuzlocke, por conta como o cofre. Os Pokémon e os eventos são da run: toda consulta passa primeiro pela run
/// (que tem o filtro de dono), então ninguém chega aos de outra conta.
/// </summary>
public sealed class NuzlockeStore(IDbContextFactory<TradeRockDb> dbFactory, GameDataStore data, DexService dex, TimeProvider time)
{
    public const int MaxSpecies = 1025;
    public const int MaxPartySize = 6;
    public const int MaxNameLength = 80;
    public const int MaxNicknameLength = 12;
    public const int MaxDeathCauseLength = 200;
    public const int MaxMoves = 4;

    public async Task<IReadOnlyList<RunSummary>> ListAsync(CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var runs = await db.NuzlockeRuns.AsNoTracking().OrderByDescending(r => r.StartedAt).ToListAsync(ct);
        var ids = runs.Select(r => r.Id).ToList();
        var mons = await db.NuzlockeMons.AsNoTracking().Where(m => ids.Contains(m.RunId))
            .Select(m => new { m.RunId, m.Status }).ToListAsync(ct);
        var events = await db.NuzlockeEvents.AsNoTracking()
            .Where(e => ids.Contains(e.RunId) && e.Kind == NuzlockeEvents.BossDefeated).ToListAsync(ct);
        return runs.Select(r => new RunSummary(r.Id, r.Name, r.GameKey, r.Version, r.Status, r.StartedAt, r.EndedAt, r.CurrentAreaId,
            mons.Count(m => m.RunId == r.Id && m.Status == NuzlockeStatus.Party),
            mons.Count(m => m.RunId == r.Id && m.Status == NuzlockeStatus.Dead),
            RunProgress.DefeatedBosses(events.Where(e => e.RunId == r.Id).ToList()).Count)).ToList();
    }

    public async Task<RunView> GetAsync(Guid id, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await ViewAsync(db, await Find(db, id, ct), ct);
    }

    public async Task<RunView> CreateAsync(NewRun body, CancellationToken ct)
    {
        if (GameVersions.All.FirstOrDefault(g => g.Key == body.GameKey) is not { } game)
            throw new NuzlockeException("unknown_game");
        var version = string.IsNullOrWhiteSpace(body.Version) ? null : body.Version.Trim();
        if (version is not null && !game.PokeApiVersions.Contains(version))
            throw new NuzlockeException("unknown_version");
        // Jogo com duas versões: chefes, treinadores e encontros mudam de uma para a outra.
        if (version is null && game.PokeApiVersions.Count > 1)
            throw new NuzlockeException("version_required");
        var name = body.Name?.Trim();
        if (string.IsNullOrEmpty(name))
            name = game.Name;
        if (name.Length > MaxNameLength)
            throw new NuzlockeException("invalid_name");
        var rules = (body.Rules ?? NuzlockeRules.Default).Normalize();
        var starter = Starter(game.Key, body.Starter);
        var starterNickname = Nickname(body.StarterNickname);
        if (starter is not null && rules.NicknameRequired && starterNickname is null)
            throw new NuzlockeException("nickname_required");

        var now = time.GetUtcNow();
        var run = new NuzlockeRun
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            GameKey = game.Key,
            Version = version,
            Starter = starter,
            RulesJson = JsonSerializer.Serialize(rules, RunProgress.Json),
            Status = "active",
            StartedAt = now,
            CurrentAreaId = data.Areas(game.Key).MinBy(a => a.Order)?.Id,
        };
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        db.NuzlockeRuns.Add(run);
        AddEvent(db, run, NuzlockeEvents.RunStarted, new { gameKey = run.GameKey, version, name, starter });
        if (starter is not null)
            AddStarterMon(db, run, starter, starterNickname);
        await db.SaveChangesAsync(ct);
        return await ViewAsync(db, run, ct);
    }

    public async Task<RunView> UpdateAsync(Guid id, RunChanges changes, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var run = await FindActive(db, id, ct);
        if (changes.Name is { } name)
        {
            name = name.Trim();
            run.Name = name.Length is > 0 and <= MaxNameLength ? name : throw new NuzlockeException("invalid_name");
        }
        if (changes.Status is { } status)
            run.Status = status is "active" or "paused" ? status : throw new NuzlockeException("invalid_status");
        if (changes.Rules is { } rules)
            run.RulesJson = JsonSerializer.Serialize(rules.Normalize(), RunProgress.Json);
        if (changes.Starter is { } starter)
        {
            var id2 = Starter(run.GameKey, starter);
            if (id2 is not null && id2 != run.Starter)
                await PlaceStarterAsync(db, run, id2, Nickname(changes.StarterNickname), ct);
            run.Starter = id2;
        }
        await db.SaveChangesAsync(ct);
        return await ViewAsync(db, run, ct);
    }

    /// <summary>
    /// Inicial informado ou trocado depois da criação. Se o Pokémon que a run pôs sozinha na equipe ainda está lá e
    /// nunca foi mexido (nenhum <c>mon_updated</c> da pessoa), ele vira o novo inicial (espécie, área e nível do
    /// presente). Se a run nunca teve esse Pokémon, ele entra agora (se a área do presente estiver livre, a espécie não
    /// tiver sido pega e houver vaga). Nos outros casos só o inicial da run muda (o time do rival), sem mexer nos Pokémon.
    /// </summary>
    private async Task PlaceStarterAsync(TradeRockDb db, NuzlockeRun run, string starter, string? nickname, CancellationToken ct)
    {
        var events = await db.NuzlockeEvents.Where(e => e.RunId == run.Id
            && (e.Kind == NuzlockeEvents.Encounter || e.Kind == NuzlockeEvents.MonUpdated)).ToListAsync(ct);
        var autoIds = events.Where(e => e.Kind == NuzlockeEvents.Encounter && IsStarterEvent(e)).Select(e => ReadGuid(e, "monId"))
            .OfType<Guid>().ToList();
        var mons = await db.NuzlockeMons.Where(m => m.RunId == run.Id).ToListAsync(ct);
        var info = GameStarters.For(run.GameKey).First(s => s.Id == starter);
        var (area, level) = StarterGift(run, info);

        if (autoIds.Count > 0)
        {
            var edited = events.Where(e => e.Kind == NuzlockeEvents.MonUpdated && !IsStarterEvent(e)).Select(e => ReadGuid(e, "monId")).ToHashSet();
            var auto = mons.FirstOrDefault(m => m.Id == autoIds[^1]);
            if (auto is null || auto.Status != NuzlockeStatus.Party || edited.Contains(auto.Id))
                return;
            (auto.Species, auto.Form, auto.Level) = (info.Species, 0, level);
            if (area is not null)
                auto.AreaId = area.Id;
            AddEvent(db, run, NuzlockeEvents.MonUpdated, new { monId = auto.Id, species = info.Species, form = 0, starter = true });
            return;
        }
        if (area is null
            || mons.Any(m => m.AreaId == area.Id && m.Status != NuzlockeStatus.Missed)
            || mons.Any(m => NuzlockeStatus.Caught(m.Status) && m.Species == info.Species)
            || mons.Count(m => m.Status == NuzlockeStatus.Party) >= MaxPartySize)
            return;
        AddStarterMon(db, run, starter, nickname);
    }

    /// <summary>Põe o inicial na equipe (status <c>party</c>, forma padrão, sem shiny) e grava o <c>encounter</c> com <c>starter: true</c>.</summary>
    private void AddStarterMon(TradeRockDb db, NuzlockeRun run, string starter, string? nickname)
    {
        var info = GameStarters.For(run.GameKey).First(s => s.Id == starter);
        var (area, level) = StarterGift(run, info);
        if (area is null)
            return;
        var mon = new NuzlockeMon
        {
            Id = Guid.CreateVersion7(),
            RunId = run.Id,
            AreaId = area.Id,
            Species = info.Species,
            Form = 0,
            Nickname = nickname,
            Level = level,
            Status = NuzlockeStatus.Party,
            Shiny = false,
            CaughtAt = time.GetUtcNow(),
            MovesJson = "[]",
        };
        db.NuzlockeMons.Add(mon);
        AddEvent(db, run, NuzlockeEvents.Encounter, new
        {
            monId = mon.Id, areaId = area.Id, species = mon.Species, form = 0, status = mon.Status, shiny = false, starter = true,
        });
    }

    /// <summary>
    /// Área e nível em que o jogo dá o inicial: o encontro <c>gift</c> da espécie (nossos encontros nos jogos que os têm,
    /// senão o PokeAPI), na área de menor ordem; sem dado, a primeira área do jogo e nível 5.
    /// </summary>
    internal (GameArea? Area, int Level) StarterGift(NuzlockeRun run, StarterInfo starter)
    {
        var areas = data.Areas(run.GameKey);
        var gifts = new List<(string AreaId, int Level)>();
        var own = data.Encounters(run.GameKey);
        if (own.Count > 0)
        {
            gifts.AddRange(own.Where(e => e.Species == starter.Species && e.Method == "gift" && RunProgress.InVersion(e.Versions, run.Version))
                .Select(e => (e.Area, e.MinLevel)));
        }
        else if (DexDatabase.ResourceExists)
        {
            foreach (var (location, lvl) in dex.GiftLocations(starter.Species, run.GameKey, run.Version))
                gifts.AddRange(areas.Where(a => a.PokeApiLocationId == location).Select(a => (a.Id, lvl)));
        }
        var best = gifts.Select(g => (Area: areas.FirstOrDefault(a => a.Id == g.AreaId), g.Level)).Where(g => g.Area is not null)
            .OrderBy(g => g.Area!.Order).FirstOrDefault();
        return best.Area is not null
            ? (best.Area, best.Level is >= 1 and <= 100 ? best.Level : 5)
            : (areas.MinBy(a => a.Order), 5);
    }

    private static bool IsStarterEvent(NuzlockeEvent e)
    {
        using var doc = JsonDocument.Parse(e.Json);
        return doc.RootElement.TryGetProperty("starter", out var v) && v.ValueKind == JsonValueKind.True;
    }

    private static Guid? ReadGuid(NuzlockeEvent e, string property)
    {
        using var doc = JsonDocument.Parse(e.Json);
        return doc.RootElement.TryGetProperty(property, out var v) && v.ValueKind == JsonValueKind.String && v.TryGetGuid(out var g) ? g : null;
    }

    /// <summary>O inicial normalizado (vazio = null); fora dos iniciais do jogo → <c>invalid_starter</c>.</summary>
    private static string? Starter(string gameKey, string? starter)
    {
        var id = string.IsNullOrWhiteSpace(starter) ? null : starter.Trim().ToLowerInvariant();
        return id is null || GameStarters.IsStarter(gameKey, id) ? id : throw new NuzlockeException("invalid_starter");
    }

    public async Task<RunView> SetAreaAsync(Guid id, string areaId, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var run = await FindActive(db, id, ct);
        var area = Area(run, areaId);
        if (RunProgress.CurrentArea(data.Areas(run.GameKey), run.CurrentAreaId)?.Id != area.Id)
        {
            run.CurrentAreaId = area.Id;
            AddEvent(db, run, NuzlockeEvents.AreaChanged, new { areaId = area.Id });
            await db.SaveChangesAsync(ct);
        }
        return await ViewAsync(db, run, ct);
    }

    /// <summary>
    /// Registra um encontro na área. Com <c>party</c>/<c>box</c>: área já usada → <c>area_already_used</c>
    /// (<see cref="NuzlockeRules.FirstEncounterOnly"/>); espécie (ou linha, com <see cref="NuzlockeRules.DupesClause"/>) já
    /// capturada → <c>dupe</c> (<see cref="NuzlockeRules.SpeciesClause"/>), e aí dá para registrar como <c>missed</c>; shiny
    /// com <see cref="NuzlockeRules.ShinyClause"/> passa pelas duas. Área usada = tem Pokémon que não seja <c>missed</c>.
    /// </summary>
    public async Task<RunView> EncounterAsync(Guid id, NewEncounter body, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var run = await FindActive(db, id, ct);
        var area = Area(run, body.AreaId);
        var status = body.Status;
        if (status is not (NuzlockeStatus.Party or NuzlockeStatus.Box or NuzlockeStatus.Missed or NuzlockeStatus.Failed))
            throw new NuzlockeException("invalid_status");
        var caught = NuzlockeStatus.Caught(status);
        CheckSpecies(body.Species, body.Form, caught);
        if (body.Level is < 0 or > 100 || (caught && body.Level < 1))
            throw new NuzlockeException("invalid_level");
        var nickname = Nickname(body.Nickname);
        var rules = RunProgress.Rules(run);
        var mons = await db.NuzlockeMons.Where(m => m.RunId == run.Id).ToListAsync(ct);

        if (caught)
        {
            if (rules.NicknameRequired && nickname is null)
                throw new NuzlockeException("nickname_required");
            CheckClauses(rules, mons, area.Id, body.Species, body.Shiny);
            if (status == NuzlockeStatus.Party && mons.Count(m => m.Status == NuzlockeStatus.Party) >= MaxPartySize)
                throw new NuzlockeException("party_full");
        }

        var mon = new NuzlockeMon
        {
            Id = Guid.CreateVersion7(),
            RunId = run.Id,
            AreaId = area.Id,
            Species = body.Species,
            Form = body.Form,
            Nickname = nickname,
            Level = body.Level,
            Status = status,
            Shiny = body.Shiny,
            CaughtAt = time.GetUtcNow(),
            MovesJson = "[]",
        };
        db.NuzlockeMons.Add(mon);
        AddEvent(db, run, NuzlockeEvents.Encounter, new
        {
            monId = mon.Id, areaId = area.Id, species = mon.Species, form = mon.Form, status, shiny = body.Shiny,
        });
        await db.SaveChangesAsync(ct);
        return await ViewAsync(db, run, ct);
    }

    public async Task<RunView> UpdateMonAsync(Guid id, Guid monId, MonChanges changes, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var run = await FindActive(db, id, ct);
        var mon = await FindMon(db, run, monId, ct);
        var rules = RunProgress.Rules(run);

        if (changes.Species is not null || changes.Form is not null)
        {
            var species = changes.Species ?? mon.Species;
            var form = changes.Form ?? mon.Form;
            CheckSpecies(species, form, NuzlockeStatus.Caught(changes.Status ?? mon.Status));
            (mon.Species, mon.Form) = (species, form);
        }
        if (changes.Level is { } level)
            mon.Level = level is >= 1 and <= 100 ? level : throw new NuzlockeException("invalid_level");
        if (changes.Nickname is { } nick)
            mon.Nickname = Nickname(nick);
        if (changes.DeathCause is { } cause)
        {
            cause = cause.Trim();
            if (cause.Length > MaxDeathCauseLength)
                throw new NuzlockeException("death_cause_too_long");
            mon.DeathCause = cause.Length == 0 ? null : cause;
        }
        if (changes.Moves is { } moves)
            mon.MovesJson = JsonSerializer.Serialize(Moves(moves), RunProgress.Json);
        var becameCaught = false;
        if (changes.Status is { } status && status != mon.Status)
        {
            becameCaught = NuzlockeStatus.Caught(status) && !NuzlockeStatus.Caught(mon.Status);
            if (!NuzlockeStatus.All.Contains(status))
                throw new NuzlockeException("invalid_status");
            if (becameCaught)
            {
                CheckSpecies(mon.Species, mon.Form, caught: true);
                var others = await db.NuzlockeMons.Where(m => m.RunId == run.Id && m.Id != mon.Id).ToListAsync(ct);
                CheckClauses(rules, others, mon.AreaId, mon.Species, mon.Shiny);
            }
            if (status == NuzlockeStatus.Party
                && await db.NuzlockeMons.CountAsync(m => m.RunId == run.Id && m.Id != mon.Id && m.Status == NuzlockeStatus.Party, ct) >= MaxPartySize)
                throw new NuzlockeException("party_full");
            mon.DiedAt = status == NuzlockeStatus.Dead ? time.GetUtcNow() : null;
            mon.Status = status;
        }
        // Só cobra o apelido de quem acabou de ser capturado ou teve o apelido apagado agora.
        if (rules.NicknameRequired && NuzlockeStatus.Caught(mon.Status) && mon.Nickname is null && (becameCaught || changes.Nickname is not null))
            throw new NuzlockeException("nickname_required");

        AddEvent(db, run, NuzlockeEvents.MonUpdated, new
        {
            monId = mon.Id, changes.Status, changes.Level, changes.Nickname, changes.DeathCause, changes.Moves, changes.Species, changes.Form,
        });
        await db.SaveChangesAsync(ct);
        return await ViewAsync(db, run, ct);
    }

    /// <summary>Marca o chefe como vencido (de novo não muda nada).</summary>
    public async Task<RunView> DefeatBossAsync(Guid id, string bossId, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var run = await FindActive(db, id, ct);
        if (!data.Bosses(run.GameKey).Any(b => b.Id == bossId && RunProgress.InVersion(b.Versions, run.Version)))
            throw new NuzlockeException("unknown_boss");
        var already = await db.NuzlockeEvents.Where(e => e.RunId == run.Id && e.Kind == NuzlockeEvents.BossDefeated).ToListAsync(ct);
        if (!RunProgress.DefeatedBosses(already).Contains(bossId))
        {
            AddEvent(db, run, NuzlockeEvents.BossDefeated, new { bossId });
            await db.SaveChangesAsync(ct);
        }
        return await ViewAsync(db, run, ct);
    }

    /// <summary>Termina a run como <c>champion</c> ou <c>wipe</c>.</summary>
    public async Task<RunView> FinishAsync(Guid id, string status, CancellationToken ct)
    {
        if (status is not ("champion" or "wipe"))
            throw new NuzlockeException("invalid_status");
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var run = await FindActive(db, id, ct);
        run.Status = status;
        run.EndedAt = time.GetUtcNow();
        AddEvent(db, run, NuzlockeEvents.RunFinished, new { status });
        await db.SaveChangesAsync(ct);
        return await ViewAsync(db, run, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var run = await Find(db, id, ct);
        await db.NuzlockeMons.Where(m => m.RunId == run.Id).ExecuteDeleteAsync(ct);
        await db.NuzlockeEvents.Where(e => e.RunId == run.Id).ExecuteDeleteAsync(ct);
        db.NuzlockeRuns.Remove(run);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Próximos golpes por nível (acima do nível atual) e, para cada próxima evolução, os que ela aprende ao evoluir
    /// (nível 0 no PokeAPI) e os próximos por nível. Forma padrão da espécie, no grupo de versões da run.
    /// </summary>
    public async Task<MonLearnset> LearnsetAsync(Guid id, Guid monId, CancellationToken ct)
    {
        MonInfo mon;
        string gameKey;
        await using (var db = await dbFactory.CreateDbContextAsync(ct))
        {
            var run = await Find(db, id, ct);
            mon = RunProgress.Info(await FindMon(db, run, monId, ct));
            gameKey = run.GameKey;
        }
        if (mon.Species < 1 || !DexDatabase.ResourceExists)
            return new MonLearnset(mon.Species, mon.Level, [], [], []);
        var detail = await dex.GetAsync(mon.Species, gameKey, "en", ct);
        if (detail is null)
            return new MonLearnset(mon.Species, mon.Level, [], [], []);

        var evolutions = new List<EvolutionLearnset>();
        foreach (var step in detail.EvolutionChain.Where(s => s.FromSpeciesId == mon.Species))
        {
            var next = await dex.GetAsync(step.SpeciesId, gameKey, "en", ct);
            if (next is null || !next.ExistsInGame)
                continue;
            evolutions.Add(new EvolutionLearnset(step.SpeciesId, next.Name, step.Conditions,
                LevelMoves(next, l => l == 0), LevelMoves(next, l => l > mon.Level)));
        }
        return new MonLearnset(mon.Species, mon.Level, LevelMoves(detail, l => l <= mon.Level), LevelMoves(detail, l => l > mon.Level),
            evolutions);
    }

    private List<LearnsetMove> LevelMoves(SpeciesDetail detail, Func<int, bool> level) =>
        detail.Moves.LevelUp.Where(m => m.Level is { } l && level(l))
            .Select(m => new LearnsetMove(m.Identifier, m.Name, m.Type, m.Level ?? 0))
            .OrderBy(m => m.Level).ThenBy(m => m.Name, StringComparer.Ordinal)
            .ToList();

    /// <summary>
    /// Cláusulas de quem vai ser capturado: área já usada por outro Pokémon (<c>area_already_used</c>) e espécie ou linha
    /// já capturada (<c>dupe</c>). Shiny com a cláusula de shiny passa pelas duas. <paramref name="others"/>: os outros
    /// Pokémon da run (sem o próprio).
    /// </summary>
    private void CheckClauses(NuzlockeRules rules, IEnumerable<NuzlockeMon> others, string areaId, int species, bool shiny)
    {
        if (rules.ShinyClause && shiny)
            return;
        var list = others.ToList();
        if (rules.FirstEncounterOnly && list.Any(m => m.AreaId == areaId && m.Status != NuzlockeStatus.Missed))
            throw new NuzlockeException("area_already_used");
        if ((rules.SpeciesClause || rules.DupesClause)
            && list.Any(m => NuzlockeStatus.Caught(m.Status) && SameLine(m.Species, species, rules.DupesClause)))
            throw new NuzlockeException("dupe");
    }

    private bool SameLine(int a, int b, bool byLine)
    {
        if (a == b)
            return true;
        return byLine && DexDatabase.ResourceExists && dex.EvolutionChainId(a) is { } chain && chain == dex.EvolutionChainId(b);
    }

    private static void CheckSpecies(int species, int form, bool caught)
    {
        if (species < 0 || species > MaxSpecies || (caught && species == 0))
            throw new NuzlockeException("invalid_species");
        if (form < 0)
            throw new NuzlockeException("invalid_form");
    }

    private static string? Nickname(string? nickname)
    {
        nickname = nickname?.Trim();
        if (nickname is { Length: > MaxNicknameLength })
            throw new NuzlockeException("nickname_too_long");
        return string.IsNullOrEmpty(nickname) ? null : nickname;
    }

    private List<string> Moves(IReadOnlyList<string> moves)
    {
        if (moves.Count > MaxMoves)
            throw new NuzlockeException("too_many_moves");
        var known = DexDatabase.ResourceExists ? dex.MoveIdentifiers() : null;
        var result = new List<string>();
        foreach (var raw in moves)
        {
            var move = raw?.Trim().ToLowerInvariant() ?? "";
            if (move.Length == 0 || (known is not null && !known.Contains(move)))
                throw new NuzlockeException("invalid_move");
            if (!result.Contains(move))
                result.Add(move);
        }
        return result;
    }

    private GameArea Area(NuzlockeRun run, string? areaId) =>
        data.Areas(run.GameKey).FirstOrDefault(a => a.Id == areaId) ?? throw new NuzlockeException("unknown_area");

    private void AddEvent(TradeRockDb db, NuzlockeRun run, string kind, object payload) =>
        db.NuzlockeEvents.Add(new NuzlockeEvent
        {
            Id = Guid.CreateVersion7(),
            RunId = run.Id,
            At = time.GetUtcNow(),
            Kind = kind,
            Json = JsonSerializer.Serialize(payload, RunProgress.Json),
        });

    private async Task<RunView> ViewAsync(TradeRockDb db, NuzlockeRun run, CancellationToken ct)
    {
        var mons = await db.NuzlockeMons.AsNoTracking().Where(m => m.RunId == run.Id).ToListAsync(ct);
        var events = await db.NuzlockeEvents.AsNoTracking().Where(e => e.RunId == run.Id).ToListAsync(ct);
        return await RunProgress.ComputeAsync(run, mons, events, data, dex, ct);
    }

    private static async Task<NuzlockeRun> Find(TradeRockDb db, Guid id, CancellationToken ct) =>
        await db.NuzlockeRuns.FirstOrDefaultAsync(r => r.Id == id, ct) ?? throw new NuzlockeException("run_not_found", 404);

    /// <summary>A run do dono, ainda não terminada (terminada → 409 <c>run_finished</c>).</summary>
    private static async Task<NuzlockeRun> FindActive(TradeRockDb db, Guid id, CancellationToken ct)
    {
        var run = await Find(db, id, ct);
        return run.EndedAt is null ? run : throw new NuzlockeException("run_finished", 409);
    }

    /// <summary>O Pokémon só é achado dentro da run já filtrada pelo dono.</summary>
    private static async Task<NuzlockeMon> FindMon(TradeRockDb db, NuzlockeRun run, Guid monId, CancellationToken ct) =>
        await db.NuzlockeMons.FirstOrDefaultAsync(m => m.RunId == run.Id && m.Id == monId, ct)
        ?? throw new NuzlockeException("mon_not_found", 404);
}

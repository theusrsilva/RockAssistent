using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TradeRock.Core.Dex;
using TradeRock.Core.GameData;
using TradeRock.Core.Games;
using TradeRock.Core.Storage;

namespace TradeRock.Core.Hunts;

/// <summary>Erro de regra das hunts: <see cref="Code"/> vai para o JSON e <see cref="Status"/> é o HTTP.</summary>
public sealed class HuntException(string code, int status = 400) : Exception(code)
{
    public string Code { get; } = code;
    public int Status { get; } = status;
}

/// <summary>Uma hunt com a odd atual e a chance acumulada já calculadas.</summary>
public sealed record HuntView(
    Guid Id,
    int Species,
    int Form,
    string GameKey,
    string MethodId,
    string MethodName,
    string MethodNameEn,
    IReadOnlyDictionary<string, string> Modifiers,
    int Count,
    long ElapsedSeconds,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    bool Found,
    string? Notes,
    double Odds,
    double Cumulative);

public sealed record NewHunt(int Species, int Form, string GameKey, string MethodId, IReadOnlyDictionary<string, string>? Modifiers);

/// <summary>Campos nulos ficam como estão.</summary>
public sealed record HuntChanges(int? Count, IReadOnlyDictionary<string, string>? Modifiers, string? Notes, long? ElapsedSeconds);

/// <summary>Método de shiny que vale para a espécie num jogo (modificadores só os do jogo).</summary>
public sealed record HuntMethodOption(string Id, string Name, string NameEn, double BaseOdds, IReadOnlyList<ShinyModifier> Modifiers);

/// <summary>Jogo em que dá para caçar a espécie: como se obtém (<c>encounter</c>, <c>gift</c>, <c>egg</c>, <c>evolution</c>) e os métodos.</summary>
public sealed record HuntGame(string GameKey, string Name, IReadOnlyList<string> Kinds, IReadOnlyList<HuntMethodOption> Methods);

/// <summary>
/// Hunts de shiny (Contador Hunt), por conta como o cofre. Guarda a escolha (espécie, jogo, método e modificadores),
/// o contador e o cronômetro; as odds são calculadas na leitura (<see cref="ShinyOdds"/>).
/// </summary>
public sealed class HuntStore(IDbContextFactory<TradeRockDb> dbFactory, GameDataStore data, DexService dex, TimeProvider time)
{
    public const int MaxDelta = 100;
    public const int MaxNotesLength = 500;
    public const int MaxSpecies = 1025;

    // Tipo de obtenção de que cada método precisa. Os que não estão aqui valem para qualquer jogo em que a
    // espécie se obtém (chance normal, amuleto, mundo aberto do Arceus/SV/Z-A: quem evolui ou choca também conta).
    private static readonly HashSet<string> EggMethods =
        ["breeding_shiny_parent_gen2", "odd_egg", "masuda_gen4", "masuda_gen5", "masuda", "masuda_bdsp", "egg_bdsp"];

    private static readonly HashSet<string> WildMethods =
    [
        "poke_radar_dppt", "poke_radar_xy", "poke_radar_bdsp", "chain_fishing", "friend_safari", "horde", "dexnav",
        "sos_chain", "ultra_wormhole_type2", "ultra_wormhole_type3", "ultra_wormhole_type4", "catch_combo", "brilliant",
        "max_raid", "dynamax_adventure", "grand_underground", "pla_outbreak", "pla_massive_outbreak", "tera_raid",
        "za_hyperspace",
    ];

    // O Ovo Estranho do Crystal só tem bebês: Pichu, Cleffa, Igglybuff, Tyrogue, Smoochum, Elekid e Magby.
    private static readonly HashSet<int> OddEggSpecies = [172, 173, 174, 236, 238, 239, 240];

    private static readonly JsonSerializerOptions Json = new();

    public async Task<IReadOnlyList<HuntView>> ListAsync(bool all, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var query = db.Hunts.AsNoTracking();
        if (!all)
            query = query.Where(h => h.EndedAt == null);
        var hunts = await query.OrderByDescending(h => h.StartedAt).ToListAsync(ct);
        return hunts.Select(View).ToList();
    }

    public async Task<HuntView> GetAsync(Guid id, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return View(await Find(db, id, ct));
    }

    public async Task<HuntView> CreateAsync(NewHunt body, CancellationToken ct)
    {
        if (body.Species is < 1 or > MaxSpecies)
            throw new HuntException("invalid_species");
        if (body.Form < 0)
            throw new HuntException("invalid_form");
        var method = Method(body.GameKey, body.MethodId);
        var modifiers = Normalize(method, body.GameKey, body.Modifiers);
        var hunt = new ShinyHunt
        {
            Id = Guid.CreateVersion7(),
            Species = body.Species,
            Form = body.Form,
            GameKey = body.GameKey,
            MethodId = method.Id,
            ModifiersJson = JsonSerializer.Serialize(modifiers, Json),
            StartedAt = time.GetUtcNow(),
        };
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        db.Hunts.Add(hunt);
        await db.SaveChangesAsync(ct);
        return View(hunt);
    }

    public async Task<HuntView> UpdateAsync(Guid id, HuntChanges changes, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var hunt = await Find(db, id, ct);
        if (changes.Count is { } count)
            hunt.Count = count >= 0 ? count : throw new HuntException("invalid_count");
        if (changes.ElapsedSeconds is { } elapsed)
            hunt.ElapsedSeconds = elapsed >= 0 ? elapsed : throw new HuntException("invalid_elapsed");
        if (changes.Notes is { } notes)
        {
            notes = notes.Trim();
            if (notes.Length > MaxNotesLength)
                throw new HuntException("notes_too_long");
            hunt.Notes = notes.Length == 0 ? null : notes;
        }
        if (changes.Modifiers is { } mods)
            hunt.ModifiersJson = JsonSerializer.Serialize(Normalize(Method(hunt.GameKey, hunt.MethodId), hunt.GameKey, mods), Json);
        await db.SaveChangesAsync(ct);
        return View(hunt);
    }

    /// <summary>
    /// Soma <paramref name="delta"/> (±1 a ±100) ao contador, sem passar de 0 para baixo. A conta é feita no banco
    /// (um UPDATE só), para dois toques seguidos (tecla segurada, atalho global) não se perderem.
    /// </summary>
    public async Task<HuntView> IncrementAsync(Guid id, int delta, CancellationToken ct)
    {
        if (delta == 0 || Math.Abs(delta) > MaxDelta)
            throw new HuntException("invalid_delta");
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var changed = await db.Hunts.Where(h => h.Id == id && h.EndedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(h => h.Count, h => h.Count + delta < 0 ? 0 : h.Count + delta), ct);
        var hunt = await Find(db, id, ct);
        if (changed == 0)
            throw new HuntException("hunt_finished", 409);
        return View(hunt);
    }

    /// <summary>Termina a hunt; <paramref name="elapsedSeconds"/>, se vier, grava o cronômetro na mesma chamada.</summary>
    public async Task<HuntView> FinishAsync(Guid id, bool found, long? elapsedSeconds, CancellationToken ct)
    {
        if (elapsedSeconds is < 0)
            throw new HuntException("invalid_elapsed");
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var hunt = await Find(db, id, ct);
        if (hunt.EndedAt is not null)
            throw new HuntException("hunt_finished", 409);
        hunt.EndedAt = time.GetUtcNow();
        hunt.Found = found;
        if (elapsedSeconds is { } elapsed)
            hunt.ElapsedSeconds = elapsed;
        await db.SaveChangesAsync(ct);
        return View(hunt);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        db.Hunts.Remove(await Find(db, id, ct));
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Jogos em que dá para caçar a espécie, com os métodos válidos. Junta a disponibilidade da Pokédex (PokeAPI) com
    /// os encontros dos nossos dados (<c>assets/data/encounters</c>: BDSP, Arceus, SV e Z-A). Jogo sem nenhum jeito
    /// de obter fica de fora.
    /// </summary>
    public async Task<IReadOnlyList<HuntGame>> GamesForSpeciesAsync(int species, CancellationToken ct)
    {
        if (species is < 1 or > MaxSpecies)
            throw new HuntException("invalid_species");
        var availability = await dex.AvailabilityAsync(species, ct);
        var result = new List<HuntGame>();
        foreach (var game in GameVersions.All)
        {
            var kinds = new HashSet<string>();
            switch (availability.FirstOrDefault(a => a.GameKey == game.Key)?.Kind)
            {
                case AvailabilityKind.Encounter: kinds.Add("encounter"); break;
                case AvailabilityKind.Evolution: kinds.Add("evolution"); break;
                case AvailabilityKind.Egg: kinds.Add("egg"); break;
                case AvailabilityKind.Gift: kinds.Add("gift"); break;
            }
            foreach (var e in data.Encounters(game.Key).Where(e => e.Species == species))
            {
                // Troca no jogo não entra: o Pokémon vem pronto (e quase sempre sem chance de shiny).
                if (e.Method == "gift")
                    kinds.Add("gift");
                else if (e.Method != "trade")
                    kinds.Add("encounter");
            }
            // A Pokédex só marca "egg" quando não há encontro nem evolução; aqui vale saber se dá para chocar.
            if (dex.CanHatchIn(species, game.Key))
                kinds.Add("egg");
            if (kinds.Count == 0)
                continue;

            var methods = data.ShinyMethods
                .Where(m => m.Games.Contains(game.Key) && Fits(m.Id, species, kinds))
                .Select(m => new HuntMethodOption(m.Id, m.Name, m.NameEn, m.BaseOdds,
                    m.Modifiers.Where(x => x.Games is null || x.Games.Contains(game.Key)).ToList()))
                .ToList();
            string[] order = ["encounter", "gift", "egg", "evolution"];
            result.Add(new HuntGame(game.Key, game.Name, order.Where(kinds.Contains).ToList(), methods));
        }
        return result;
    }

    private static bool Fits(string methodId, int species, HashSet<string> kinds)
    {
        if (methodId == "odd_egg")
            return OddEggSpecies.Contains(species);
        if (EggMethods.Contains(methodId))
            return kinds.Contains("egg");
        if (WildMethods.Contains(methodId))
            return kinds.Contains("encounter");
        return true;
    }

    private ShinyMethod Method(string gameKey, string methodId)
    {
        if (GameVersions.All.All(g => g.Key != gameKey))
            throw new HuntException("unknown_game");
        var method = data.ShinyMethods.FirstOrDefault(m => m.Id == methodId) ?? throw new HuntException("unknown_method");
        return method.Games.Contains(gameKey) ? method : throw new HuntException("method_not_in_game");
    }

    /// <summary>
    /// Confere os modificadores: só os do método (e do jogo, quando o modificador tem <c>games</c>), toggle com
    /// "true"/"false" e steps com número inteiro >= 0. O <c>steps</c> do contador não é guardado (vem do contador).
    /// </summary>
    private static Dictionary<string, string> Normalize(ShinyMethod method, string gameKey, IReadOnlyDictionary<string, string>? input)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (id, value) in input ?? new Dictionary<string, string>())
        {
            var mod = method.Modifiers.FirstOrDefault(m => m.Id == id) ?? throw new HuntException("unknown_modifier");
            if (mod.Games is { } games && !games.Contains(gameKey))
                throw new HuntException("modifier_not_in_game");
            if (mod.Kind == "toggle")
                result[id] = bool.TryParse(value, out var on) ? (on ? "true" : "false") : throw new HuntException("invalid_modifier");
            else if (mod.Counter)
                continue;
            else
                result[id] = int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) && n >= 0
                    ? n.ToString(CultureInfo.InvariantCulture)
                    : throw new HuntException("invalid_modifier");
        }
        // Arceus: o Amuleto Brilhante só vem com a Pokédex completa, então toda espécie já tem a pesquisa nível 10.
        if (method.Games.Contains("pla") && method.Modifiers.Any(m => m.Id == ShinyOdds.Research10)
            && ShinyOdds.IsOn(result, "shiny_charm") && !ShinyOdds.IsOn(result, ShinyOdds.ResearchPerfect))
            result[ShinyOdds.Research10] = "true";
        return result;
    }

    private HuntView View(ShinyHunt h)
    {
        var modifiers = JsonSerializer.Deserialize<Dictionary<string, string>>(h.ModifiersJson, Json) ?? [];
        var method = data.ShinyMethods.FirstOrDefault(m => m.Id == h.MethodId);
        // Método que sumiu dos dados (não deveria): mostra 1/4096 em vez de quebrar a lista.
        var odds = method is null ? 4096 : ShinyOdds.Current(method, modifiers, h.Count, h.GameKey);
        return new HuntView(h.Id, h.Species, h.Form, h.GameKey, h.MethodId, method?.Name ?? h.MethodId,
            method?.NameEn ?? h.MethodId, modifiers, h.Count,
            h.ElapsedSeconds, h.StartedAt, h.EndedAt, h.Found, h.Notes, odds, ShinyOdds.Cumulative(odds, h.Count));
    }

    private static async Task<ShinyHunt> Find(TradeRockDb db, Guid id, CancellationToken ct) =>
        await db.Hunts.FirstOrDefaultAsync(h => h.Id == id, ct) ?? throw new HuntException("hunt_not_found", 404);
}

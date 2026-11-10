using PKHeX.Core;
using TradeRock.Core.Games;
using TradeRock.Core.Pokemon;

namespace TradeRock.Core.Builder;

/// <summary>
/// Resultado de "Copiar para ser válido no jogo". <see cref="Refusal"/> é um código estável ("same_game", "egg",
/// "species_not_in_game" ou "copy_not_possible", este com o motivo em <see cref="Reason"/>). <see cref="Notes"/> diz o
/// que não deu para copiar igual ("moves_replaced", "item_removed", "ball_changed"...).
/// </summary>
public sealed record GameCopyResult(BuildResult? Build, PKM? Pokemon, string? Refusal, string? Reason, IReadOnlyList<string> Notes)
{
    public bool Ok => Refusal is null && Build is not null && Pokemon is not null;
}

/// <summary>Recusa da cópia, com o código estável e, no "copy_not_possible", o motivo.</summary>
public sealed class GameCopyException(string code, string? reason = null) : Exception(code)
{
    public string Code { get; } = code;
    public string? Reason { get; } = reason;
}

/// <summary>
/// "Copiar para ser válido no jogo" (D36): um Pokémon nascido num jogo mais novo não pode ser convertido para um jogo
/// mais antigo (o PKHeX não tem regra para essa origem). Aqui nasce um Pokémon NOVO no jogo de destino, pelo
/// construtor (<see cref="PokemonBuilder"/>, AutoMod), com o que o <see cref="PokemonSpec"/> deixa copiar do original:
/// espécie, forma, shiny, natureza, habilidade, gênero, IVs, EVs, nível, apelido, bola, item e golpes. O encontro
/// (local, nível e data) é um encontro real do jogo de destino, escolhido pelo AutoMod. Não é conversão: é cópia.
/// </summary>
public static class GameCopy
{
    /// <summary>Pedidos que o construtor pode afrouxar sozinho (vira nota), na ordem em que são tentados.</summary>
    private static readonly string[] SoftBlockers = ["ball", "ability", "nature", "ivs"];

    /// <summary>Diferenças que fariam a cópia ser outro Pokémon ou de outro treinador: recusa.</summary>
    private static readonly string[] HardDifferences = ["species", "form", "level", "shiny", "gender", "trainer"];

    /// <param name="from">Jogo do original (null = formato fora do catálogo de troca).</param>
    /// <param name="trainer">O "Meu treinador" do jogo de destino, com os números como o jogo mostra.</param>
    public static async Task<GameCopyResult> CreateAsync(
        PKM source, GameFamily? from, SwitchGame to, TrainerSpec trainer, string language, CancellationToken ct)
    {
        if (from == to.Family)
            return Refuse("same_game");
        if (source.IsEgg)
            return Refuse("egg");
        var gen3 = to.Family == GameFamily.Frlg;
        var form = gen3 ? (byte)0 : source.Form;
        if (gen3 ? source.Species is < 1 or > BuilderData.MaxSpecies || source.Form != 0 : !to.HasSpecies(source.Species, form))
            return Refuse("species_not_in_game");

        var wanted = Describe(source, to, form, trainer);

        // 1) O Pokémon e o encontro, com os golpes que o construtor sugere (o pedido que mais dá certo).
        var spec = wanted with { Moves = [] };
        BuildResult result;
        for (var round = 0; ; round++)
        {
            result = await PokemonBuilder.GenerateAsync(spec, language, ct);
            if (result.Usable)
                break;
            if (result.Generation == "invalid_request")
                return Refuse("copy_not_possible", result.Errors.FirstOrDefault());
            // O item entra depois da geração e pode não valer para a espécie no destino: a cópia fica sem ele.
            if (spec.HeldItem != 0)
            {
                spec = spec with { HeldItem = 0 };
                continue;
            }
            // O construtor diz o que, sozinho, destrava o pedido. O que não muda quem o Pokémon é pode ceder.
            var blockers = result.Blockers?.Select(b => b.Code).ToList() ?? [];
            var soft = SoftBlockers.FirstOrDefault(blockers.Contains);
            if (soft is null || round > SoftBlockers.Length)
                return Refuse("copy_not_possible", blockers.FirstOrDefault() ?? "generation_failed");
            spec = soft switch
            {
                "ball" => spec with { Ball = null },
                "ability" => spec with { AbilitySlot = null },
                "nature" => spec with { Nature = null },
                _ => spec with { Ivs = null },
            };
        }
        if (Hard(result) is { } hard)
            return Refuse("copy_not_possible", hard);

        // 2) Os golpes do original que valem nesse encontro; os outros lugares ficam com a sugestão do construtor.
        if (wanted.Moves.Length > 0 && result.Encounter is { } encounter)
        {
            var pinned = spec with { EncounterKey = encounter.Key };
            result = await WithMovesAsync(pinned, wanted.Moves, source.Moves.Count(m => m != 0), result, language, ct);
        }

        var pk = PokemonReader.ToEntity(result.Data!, to.Family);
        // A cópia tem que ter nascido no jogo de destino, num encontro que não passa do nível dela.
        if (!to.Versions.Select(GameCatalog.ToVersion).Contains(pk.Version))
            return Refuse("copy_not_possible", "origin");
        if (pk.MetLevel > pk.CurrentLevel)
            return Refuse("copy_not_possible", "level_below_minimum");
        return new GameCopyResult(result, pk, null, null, Notes(source, pk, result.Obtained!));
    }

    private static string? Hard(BuildResult result) =>
        result.Differences.FirstOrDefault(d => HardDifferences.Contains(d.Field))?.Field;

    /// <summary>
    /// Tenta os golpes do original no encontro já escolhido (uma tentativa só do construtor por pedido). Se o conjunto
    /// não for legal, confere um golpe por vez e fica com os que passam. Os lugares vagos recebem os golpes de
    /// <paramref name="suggested"/> (a sugestão do construtor). Se nada der certo, fica a sugestão inteira.
    /// </summary>
    private static async Task<BuildResult> WithMovesAsync(
        PokemonSpec pinned, ushort[] moves, int count, BuildResult suggested, string language, CancellationToken ct)
    {
        async Task<BuildResult?> TryAsync(IEnumerable<ushort> set)
        {
            var r = await PokemonBuilder.GenerateAsync(pinned with { Moves = [.. set] }, language, ct);
            return r.Usable && Hard(r) is null && r.Differences.All(d => d.Field != "moves") ? r : null;
        }

        var best = await TryAsync(moves);
        var legal = moves;
        if (best is null)
        {
            var kept = new List<ushort>();
            foreach (var move in moves)
            {
                if (await TryAsync([move]) is not null)
                    kept.Add(move);
            }
            if (kept.Count == 0)
                return suggested;
            legal = [.. kept];
        }

        var extra = suggested.Obtained!.Moves.Where(m => m != 0 && !legal.Contains(m));
        // Nunca mais golpes do que o original tinha.
        ushort[] filled = [.. legal.Concat(extra).Take(Math.Min(4, count))];
        if (filled.Length > legal.Length && await TryAsync(filled) is { } complete)
            return complete;
        return best ?? await TryAsync(legal) ?? suggested;
    }

    /// <summary>O que a cópia não tem igual ao original (códigos estáveis).</summary>
    private static List<string> Notes(PKM source, PKM copy, PokemonSpec obtained)
    {
        var original = PokemonSpec.FromPk(source);
        var notes = new List<string>();
        if (source.IsNicknamed && (!copy.IsNicknamed || copy.Nickname != source.Nickname.Trim()))
            notes.Add("nickname_removed");
        if (copy.Nature != source.Nature)
            notes.Add("nature_changed");
        if (copy.Ability != source.Ability)
            notes.Add("ability_changed");
        if (!obtained.Ivs!.SequenceEqual(original.Ivs!))
            notes.Add("ivs_changed");
        if (!obtained.Evs.SequenceEqual(original.Evs))
            notes.Add("evs_changed");
        if (copy.Ball != source.Ball)
            notes.Add("ball_changed");
        if (source.HeldItem > 0 && (source is PK3 || copy.HeldItem != source.HeldItem))
            notes.Add("item_removed");
        if (copy.Language != source.Language)
            notes.Add("language_changed");
        if (!copy.Moves.Where(m => m != 0).Order().SequenceEqual(source.Moves.Where(m => m != 0).Order()))
            notes.Add("moves_replaced");
        return notes;
    }

    /// <summary>O pedido ao construtor: o que o original tem e o jogo de destino aceita.</summary>
    private static PokemonSpec Describe(PKM source, SwitchGame to, byte form, TrainerSpec trainer)
    {
        var gen3 = to.Family == GameFamily.Frlg;
        var data = BuilderData.For(to);
        var original = PokemonSpec.FromPk(source, to.Versions[0]);

        string? nickname = null;
        if (source.IsNicknamed)
        {
            var name = source.Nickname.Trim();
            if (name.Length > 0 && name.Length <= (gen3 ? 10 : 12) && !name.Any(c => c is '(' or ')' or '@' or '\n' or '\r'))
                nickname = name;
        }

        // A habilidade oculta (posição 2) não existe na Gen 3.
        var ability = gen3 && original.AbilitySlot > 1 ? null : original.AbilitySlot;

        var maxEv = gen3 ? 255 : 252;
        var evs = original.Evs.Select(v => Math.Clamp(v, 0, maxEv)).ToArray();
        if (evs.Sum() > 510)
            evs = [0, 0, 0, 0, 0, 0];

        int? ball = data.Balls.Any(b => b.Value == source.Ball) ? source.Ball : null;

        // Item: a numeração da Gen 3 é outra, e o Let's Go e o Legends: Arceus não têm item segurado.
        ushort item = 0;
        if (source.HeldItem > 0)
        {
            var holds = source is not PK3 && to.Family is not (GameFamily.Frlg or GameFamily.Lgpe or GameFamily.Pla);
            if (holds && data.Items.Any(i => i.Value == source.HeldItem))
                item = (ushort)source.HeldItem;
        }

        var language = data.Languages.Any(l => l.Value == source.Language) ? source.Language : (int)LanguageID.English;

        return original with
        {
            Form = form,
            Nickname = nickname,
            AbilitySlot = ability,
            Evs = evs,
            // Só os golpes que o jogo de destino tem; se valem para a espécie, quem diz é o construtor (WithMovesAsync).
            Moves = [.. source.Moves.Where(m => m != 0).Distinct().Where(m => data.Moves.Any(o => o.Value == m))],
            HeldItem = item,
            Ball = ball,
            Language = language,
            Trainer = trainer,
        };
    }

    private static GameCopyResult Refuse(string code, string? reason = null) => new(null, null, code, reason, []);
}

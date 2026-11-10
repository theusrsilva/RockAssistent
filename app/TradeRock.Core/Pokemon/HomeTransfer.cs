using PKHeX.Core;
using TradeRock.Core.Builder;
using TradeRock.Core.Games;

namespace TradeRock.Core.Pokemon;

/// <summary>Resultado de mover um Pokémon de um jogo para outro. <see cref="Refusal"/> é um código estável.</summary>
public sealed record HomeTransferResult(PKM? Pokemon, string? Refusal, IReadOnlyList<string> Notes)
{
    public bool Ok => Refusal is null && Pokemon is not null;
}

/// <summary>
/// Move um Pokémon entre jogos como o Pokémon HOME faz, com o conversor do PKHeX (Entrega 14d, D22). Regras e
/// ajustes adaptados do ajudante PKHeX do pokeldn (services/pkhex/Program.cs, Converted; AGPL-3.0):
/// - o Let's Go só manda para o HOME, não recebe;
/// - ovos ficam no próprio jogo;
/// - do FireRed/LeafGreen, só sem item segurado e sem golpe de HM (Pal Park e Poké Transfer recusam);
/// - a espécie (e a forma) tem que existir no jogo de destino;
/// - o treinador de destino vira o treinador atual (o HOME entrega no save para onde move);
/// - golpes que o jogo novo não tem são trocados pela sugestão do PKHeX, como os jogos fazem;
/// - o jogo de destino tem que aceitar a origem: quando a espécie existe lá, mas o PKHeX não converte ou não consegue
///   analisar o resultado, a recusa é "origin_not_accepted" se o Pokémon nasceu num jogo mais novo que o destino
///   (hoje, os nascidos no Legends: Z-A) e "no_route" nos outros casos;
/// - quem chegou ao Sword/Shield vindo do BDSP, do Arceus ou do Scarlet/Violet não sai mais de lá
///   ("origin_lost_in_swsh", ver <see cref="OriginLostInSwsh"/>).
/// <see cref="Destinations"/> ensaia a mesma conversão de <see cref="Move"/>: um destino só aparece como "pode" se
/// mover der certo. Nenhuma exceção do PKHeX sai daqui: vira recusa.
/// O rastreador do HOME fica vazio: o HOME recusa um inventado, e os jogos não o conferem.
/// </summary>
public static class HomeTransfer
{
    /// <summary>HMs da Gen 3: Cut, Fly, Surf, Strength, Waterfall, Flash, Rock Smash e Dive.</summary>
    private static readonly HashSet<ushort> HiddenMoves3 = [15, 19, 57, 70, 127, 148, 249, 291];

    /// <summary>Recusa quando a espécie existe no destino, mas o jogo de destino não aceita a origem do Pokémon.</summary>
    public const string OriginNotAccepted = "origin_not_accepted";

    /// <summary>Recusa quando o formato do Sword/Shield já não guarda os dados de origem do Pokémon.</summary>
    public const string OriginLostInSwshRefusal = "origin_lost_in_swsh";

    /// <summary>
    /// Pokémon no formato do Sword/Shield que nasceu no BDSP, no Arceus ou no Scarlet/Violet. O Sword/Shield não
    /// conhece esses jogos: na ida, o conversor do PKHeX (que segue o HOME) grava a versão de origem como Sword ou
    /// Shield, troca o local de encontro por um código do HOME (59996 a 60000, <see cref="LocationsHOME"/>), o local
    /// do ovo por 65534 e a bola do Arceus pela Poké Ball. O PK8 não guarda os dados originais, e a conversão seguinte
    /// leva os códigos do HOME para o outro formato, onde nenhum encontro bate (EncInvalid, BallEncMismatch). Sem o
    /// local, o ovo e a bola de verdade não há como refazer a origem sem inventar: o Pokémon fica no Sword/Shield.
    /// </summary>
    public static bool OriginLostInSwsh(PKM pk) => pk is PK8 && LocationsHOME.IsLocationSWSH(pk.MetLocation);

    /// <summary>
    /// Jogos para onde o Pokémon pode ir (código de recusa por jogo; null = pode). Cada destino é ensaiado com a mesma
    /// conversão de <see cref="Move"/> (o treinador não muda o resultado: usa-se um neutro do jogo de destino).
    /// </summary>
    public static IReadOnlyDictionary<GameFamily, string?> Destinations(PKM source, GameFamily from)
    {
        var result = new Dictionary<GameFamily, string?>();
        foreach (var game in GameCatalog.All)
        {
            result[game.Family] = game.Family == from
                ? "same_game"
                : Move(source, game, new SimpleTrainerInfo(GameCatalog.ToVersion(game.Versions[0]))).Refusal;
        }
        return result;
    }

    public static HomeTransferResult Move(PKM source, SwitchGame to, ITrainerInfo owner)
    {
        if (Refusal(source, to) is { } refusal)
            return new HomeTransferResult(null, refusal, []);
        try
        {
            return Convert(source, to, owner);
        }
        catch (Exception e) when (e is not OutOfMemoryException)
        {
            // O PKHeX lança exceção com origens que não conhece (ex.: nascido no Z-A num formato mais antigo).
            return new HomeTransferResult(null, ConversionRefusal(source, to), [e.GetType().Name]);
        }
    }

    private static HomeTransferResult Convert(PKM source, SwitchGame to, ITrainerInfo owner)
    {
        var pk = EntityConverter.ConvertToType(source.Clone(), to.EntityType, out var result);
        if (pk is null)
            return new HomeTransferResult(null, ConversionRefusal(source, to), [result.ToString()]);

        var notes = new List<string>();
        if (pk is IHandlerUpdate handler)
            handler.UpdateHandler(owner);
        else if (pk is PB8 pb8)
            pb8.UpdateHandler(owner);
        pk.ResetPartyStats();
        pk.RefreshChecksum();

        var la = new LegalityAnalysis(pk);
        // Parsed falso: o PKHeX converteu, mas a análise quebrou por dentro (origem que o formato não conhece).
        // Seguir daqui lança exceção nos golpes e deixa no cofre um Pokémon que nenhuma tela consegue analisar.
        if (!la.Parsed)
            return new HomeTransferResult(null, ConversionRefusal(source, to), ["analysis_failed"]);
        bool Accepted(LegalityAnalysis check, PKM entity) => check.Valid || LegalityReporter.OnlyHomeTrackerMissing(entity, check);
        // O Sword/Shield quer um Pokémon de outro jogo com treinador atual, mesmo que seja o próprio dono.
        if (!Accepted(la, pk) && pk.CurrentHandler == 0)
        {
            var handled = pk.Clone();
            handled.CurrentHandler = 1;
            handled.HandlingTrainerName = owner.OT;
            handled.HandlingTrainerGender = owner.Gender;
            if (handled is IHandlerLanguage language)
                language.HandlingTrainerLanguage = (byte)owner.Language;
            handled.RefreshChecksum();
            var check = new LegalityAnalysis(handled);
            if (Accepted(check, handled))
                (pk, la) = (handled, check);
        }
        // Golpe que o jogo novo não tem: os jogos do HOME trocam os golpes; fica a sugestão do PKHeX.
        if (!Accepted(la, pk) && la.Info.Moves.Any(m => !m.Valid))
        {
            pk.SetMoveset();
            pk.SetRelearnMoves(new LegalityAnalysis(pk));
            Permit(pk);
            pk.RefreshChecksum();
            notes.Add("moves_replaced");
        }
        return new HomeTransferResult(pk, null, notes);
    }

    /// <summary>
    /// Código da recusa quando a conversão falha com a espécie presente no destino: a origem (o jogo em que o Pokémon
    /// nasceu, não o formato de agora) é de um jogo do catálogo mais novo que o destino, ou não há caminho.
    /// O catálogo está na ordem de lançamento.
    /// </summary>
    private static string ConversionRefusal(PKM source, SwitchGame to)
    {
        var origin = GameCatalog.All.FirstOrDefault(g => g.Versions.Any(v => GameCatalog.ToVersion(v) == source.Version));
        return origin is not null && origin.Family > to.Family ? OriginNotAccepted : "no_route";
    }

    private static string? Refusal(PKM source, SwitchGame to)
    {
        if (to.Family == GameFamily.Lgpe || !EntityConverter.IsConvertibleToFormat(source, to.Format))
            return "home_no_destination";
        if (source.IsEgg)
            return "egg";
        if (source is PK3 && source.HeldItem != 0)
            return "frlg_held_item";
        if (source is PK3 && source.Moves.Any(HiddenMoves3.Contains))
            return "frlg_hm_move";
        if (!to.HasSpecies(source.Species, source.Form))
            return "species_not_in_game";
        if (OriginLostInSwsh(source))
            return OriginLostInSwshRefusal;
        return null;
    }

    /// <summary>Golpe de TM/TR da sugestão só vale com a marca do registro (TRs do Sword/Shield, Arceus, Z-A).</summary>
    private static void Permit(PKM pk)
    {
        if (pk is ITechRecord record)
            record.SetRecordFlags(pk, TechnicalRecordApplicatorOption.LegalCurrent);
        if (pk is IPlusRecord plus && pk.PersonalInfo is IPermitPlus permit)
            PlusRecordApplicator.SetPlusFlags(plus, pk, permit, PlusRecordApplicatorOption.LegalCurrent);
        if (pk is IMoveShop8Mastery shop)
            shop.SetMoveShopFlags(pk);
    }
}

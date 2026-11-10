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
///   ("origin_lost_in_swsh", ver <see cref="OriginLostInSwsh"/>), a não ser que o cofre tenha guardado o original de
///   antes da ida (<see cref="KeptOriginal"/>, D37);
/// - forma que depende do item segurado (Giratina, Arceus, Silvally...) volta à forma normal quando o item fica para
///   trás na conversão, como no HOME (nota "form_reverted");
/// - os golpes de "relembrar" que o formato do meio do caminho não guarda voltam a ser os do encontro;
/// - o nível Dynamax, que só o Sword/Shield guarda, volta ao mínimo do encontro (raid, Max Lair) na volta para lá
///   (nota "dynamax_level_reset");
/// - Pokémon fundido (Kyurem, Necrozma, Calyrex) não vai: "fused_form", como no HOME;
/// - um Pokémon válido nunca sai inválido: se, depois dos ajustes, o PKHeX reprova o resultado (dados do jogo de
///   origem que o formato anterior não guarda, origem que o destino não reconhece), a recusa é
///   "illegal_after_move" (ou "origin_not_accepted", se nasceu num jogo mais novo que o destino). Quem já era
///   inválido continua sendo movido como antes: a legalidade avisa, não bloqueia (D22).
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

    /// <summary>Recusa de Pokémon fundido (Kyurem, Necrozma, Calyrex): o HOME só leva depois de separar.</summary>
    public const string FusedForm = "fused_form";

    /// <summary>
    /// Recusa quando o Pokémon era válido e sairia inválido no destino: a conversão não leva algum dado que o jogo
    /// confere (ver <see cref="Move"/>). Não há o que consertar sem inventar esse dado.
    /// </summary>
    public const string IllegalAfterMove = "illegal_after_move";

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
        static bool Accepted(LegalityAnalysis check, PKM entity) => HomeTransfer.Accepted(entity, check);
        // O Sword/Shield quer um Pokémon de outro jogo com treinador atual, mesmo que seja o próprio dono.
        void WithHandler()
        {
            if (Accepted(la, pk) || pk.CurrentHandler != 0)
                return;
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
        WithHandler();
        // Forma que depende do item segurado (Giratina, Dialga, Arceus, Silvally...): a conversão devolve o item à
        // mochila, como o HOME, e a forma volta a ser a que o item de agora dá (sem item, a normal).
        if (!Accepted(la, pk) && la.Results.Any(r => r.Result == LegalityCheckResultCode.FormItemInvalid)
            && ItemForm(pk) is var form && form != pk.Form && to.HasSpecies(pk.Species, form))
        {
            pk.Form = form;
            pk.RefreshAbility(pk.AbilityNumber >> 1);
            pk.ResetPartyStats();
            pk.RefreshChecksum();
            la = new LegalityAnalysis(pk);
            notes.Add("form_reverted");
        }
        // Golpes de "relembrar": o formato do meio do caminho pode não guardar (o HOME guarda do lado dele). São os do
        // encontro, então dá para refazer sem inventar.
        if (!Accepted(la, pk) && la.Info.Relearn.Any(m => !m.Valid))
        {
            pk.SetRelearnMoves(la);
            pk.RefreshChecksum();
            la = new LegalityAnalysis(pk);
        }
        // Nível Dynamax: só o Sword/Shield guarda. Quem saiu e volta chega com 0, e o encontro de raid ou de Max Lair
        // exige um mínimo. Com 0 o PKHeX nem reconhece o encontro, então o mínimo é o menor nível que ele aceita
        // (é o do próprio encontro, não é inventado); o que passava dele se perdeu.
        if (!Accepted(la, pk) && pk is PK8 { DynamaxLevel: 0, SWSH: true } && source is not PK8)
        {
            for (byte level = 1; level <= 10; level++)
            {
                var raised = (PK8)pk.Clone();
                raised.DynamaxLevel = level;
                raised.RefreshChecksum();
                var check = new LegalityAnalysis(raised);
                if (!Accepted(check, raised))
                    continue;
                (pk, la) = (raised, check);
                notes.Add("dynamax_level_reset");
                break;
            }
        }
        // Golpe que o jogo novo não tem: os jogos do HOME trocam os golpes; fica a sugestão do PKHeX.
        if (!Accepted(la, pk) && la.Info.Moves.Any(m => !m.Valid))
        {
            pk.SetMoveset();
            pk.SetRelearnMoves(new LegalityAnalysis(pk));
            Permit(pk);
            pk.RefreshChecksum();
            la = new LegalityAnalysis(pk);
            notes.Add("moves_replaced");
        }
        // Com a forma e os golpes acertados, o treinador atual pode ser o que faltava (quando quem move é o próprio dono).
        WithHandler();
        // Um Pokémon válido não pode sair inválido: o que falta é dado que a conversão não leva, e inventar não vale.
        if (!Accepted(la, pk) && WasAcceptable(source))
        {
            string[] problems = [.. la.Results.Where(r => !r.Valid).Select(r => r.Result.ToString()).Distinct()];
            // Sem encontro reconhecido e nascido num jogo mais novo: é a origem que o destino não conhece.
            var origin = BornInNewerGame(source, to) && la.Results.Any(r => r.Result == LegalityCheckResultCode.EncInvalid);
            return new HomeTransferResult(null, origin ? OriginNotAccepted : IllegalAfterMove, problems);
        }
        return new HomeTransferResult(pk, null, notes);
    }

    /// <summary>A forma que o item segurado dá à espécie (Arceus, Silvally, Genesect, Ogerpon); nas outras, a normal.</summary>
    private static byte ItemForm(PKM pk) =>
        pk.HeldItem != 0 && FormItem.TryGetForm(pk.Species, pk.HeldItem, pk.Format, out var form) ? form : (byte)0;

    /// <summary>O Pokémon, como está agora, é válido (ou só falta o rastreador do HOME). Nunca lança exceção.</summary>
    private static bool WasAcceptable(PKM source)
    {
        try
        {
            var la = new LegalityAnalysis(source);
            return la.Parsed && LegalityReporter.IsAcceptable(source, la);
        }
        catch (Exception e) when (e is not OutOfMemoryException)
        {
            return false;
        }
    }

    /// <summary>
    /// Código da recusa quando a conversão falha com a espécie presente no destino: a origem (o jogo em que o Pokémon
    /// nasceu, não o formato de agora) é de um jogo do catálogo mais novo que o destino, ou não há caminho.
    /// O catálogo está na ordem de lançamento.
    /// </summary>
    private static string ConversionRefusal(PKM source, SwitchGame to) =>
        BornInNewerGame(source, to) ? OriginNotAccepted : "no_route";

    private static bool BornInNewerGame(PKM source, SwitchGame to)
    {
        var origin = GameCatalog.All.FirstOrDefault(g => g.Versions.Any(v => GameCatalog.ToVersion(v) == source.Version));
        return origin is not null && origin.Family > to.Family;
    }

    private static string? Refusal(PKM source, SwitchGame to)
    {
        if (to.Family == GameFamily.Lgpe || !EntityConverter.IsConvertibleToFormat(source, to.Format))
            return "home_no_destination";
        if (source.IsEgg)
            return "egg";
        // Kyurem, Necrozma e Calyrex fundidos: o HOME não leva; é preciso separar no jogo antes (decisão do autor).
        if (FormInfo.IsFusedForm(source.Species, source.Form, source.Format))
            return FusedForm;
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

    /// <summary>O critério de aceite do mover: legal ou "não verificável" só pela falta do rastreador do HOME.</summary>
    internal static bool Accepted(PKM pk) => Accepted(pk, new LegalityAnalysis(pk));

    internal static bool Accepted(PKM pk, LegalityAnalysis la) => LegalityReporter.IsAcceptable(pk, la);

    /// <summary>Golpe de TM/TR da sugestão só vale com a marca do registro (TRs do Sword/Shield, Arceus, Z-A).</summary>
    internal static void Permit(PKM pk)
    {
        if (pk is ITechRecord record)
            record.SetRecordFlags(pk, TechnicalRecordApplicatorOption.LegalCurrent);
        if (pk is IPlusRecord plus && pk.PersonalInfo is IPermitPlus permit)
            PlusRecordApplicator.SetPlusFlags(plus, pk, permit, PlusRecordApplicatorOption.LegalCurrent);
        if (pk is IMoveShop8Mastery shop)
            shop.SetMoveShopFlags(pk);
    }
}

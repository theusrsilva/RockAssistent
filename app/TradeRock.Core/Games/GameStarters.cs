namespace TradeRock.Core.Games;

/// <summary>Um inicial: identificador do PokeAPI (<c>chikorita</c>) e número da Pokédex nacional.</summary>
public sealed record StarterInfo(string Id, int Species);

/// <summary>
/// Os iniciais que o jogador escolhe em cada jogo (<see cref="GameVersions"/>). Servem às variantes dos chefes
/// (<see cref="GameData.BossVariant"/>) e ao inicial da run de Nuzlocke.
/// </summary>
/// <remarks>
/// Yellow: só o Pikachu (o time do rival muda pela evolução do Eevee dele, não pelo inicial; sem variantes por
/// inicial). Let's Go: Pikachu ou Eevee pela versão (o que muda já está em <c>Versions</c>; sem variantes por inicial).
/// </remarks>
public static class GameStarters
{
    private static readonly StarterInfo[] Kanto = [new("bulbasaur", 1), new("charmander", 4), new("squirtle", 7)];
    private static readonly StarterInfo[] Johto = [new("chikorita", 152), new("cyndaquil", 155), new("totodile", 158)];
    private static readonly StarterInfo[] Hoenn = [new("treecko", 252), new("torchic", 255), new("mudkip", 258)];
    private static readonly StarterInfo[] Sinnoh = [new("turtwig", 387), new("chimchar", 390), new("piplup", 393)];
    private static readonly StarterInfo[] Unova = [new("snivy", 495), new("tepig", 498), new("oshawott", 501)];
    private static readonly StarterInfo[] Alola = [new("rowlet", 722), new("litten", 725), new("popplio", 728)];

    private static readonly Dictionary<string, IReadOnlyList<StarterInfo>> ByGame = new()
    {
        ["rb"] = Kanto,
        ["y"] = [new("pikachu", 25)],
        ["gs"] = Johto,
        ["c"] = Johto,
        ["rs"] = Hoenn,
        ["e"] = Hoenn,
        ["frlg"] = Kanto,
        ["dp"] = Sinnoh,
        ["pt"] = Sinnoh,
        ["hgss"] = Johto,
        ["bw"] = Unova,
        ["b2w2"] = Unova,
        ["xy"] = [new("chespin", 650), new("fennekin", 653), new("froakie", 656)],
        ["oras"] = Hoenn,
        ["sm"] = Alola,
        ["usum"] = Alola,
        ["lgpe"] = [new("pikachu", 25), new("eevee", 133)],
        ["swsh"] = [new("grookey", 810), new("scorbunny", 813), new("sobble", 816)],
        ["bdsp"] = Sinnoh,
        ["pla"] = [new("rowlet", 722), new("cyndaquil", 155), new("oshawott", 501)],
        ["sv"] = [new("sprigatito", 906), new("fuecoco", 909), new("quaxly", 912)],
        ["za"] = [new("chikorita", 152), new("tepig", 498), new("totodile", 158)],
    };

    /// <summary>Os iniciais do jogo, na ordem do jogo (planta, fogo, água); vazio para jogo desconhecido.</summary>
    public static IReadOnlyList<StarterInfo> For(string gameKey) => ByGame.GetValueOrDefault(gameKey) ?? [];

    /// <summary>O inicial é um dos do jogo?</summary>
    public static bool IsStarter(string gameKey, string starter) => For(gameKey).Any(s => s.Id == starter);
}

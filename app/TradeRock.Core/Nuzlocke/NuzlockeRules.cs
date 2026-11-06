namespace TradeRock.Core.Nuzlocke;

/// <summary>
/// Regras de uma run. As que o app confere: <see cref="FirstEncounterOnly"/> (um Pokémon por área),
/// <see cref="NicknameRequired"/> (apelido ao capturar), <see cref="SpeciesClause"/> (espécie repetida não conta),
/// <see cref="DupesClause"/> (repetida pela linha evolutiva) e <see cref="ShinyClause"/> (shiny sempre pode).
/// <see cref="FaintIsDeath"/> e <see cref="LevelCapRequired"/> são só informativas: a pessoa marca a morte, e o app só
/// avisa quem passou do cap. <see cref="Custom"/>: regras em texto livre.
/// </summary>
public sealed record NuzlockeRules(
    bool FirstEncounterOnly,
    bool NicknameRequired,
    bool FaintIsDeath,
    bool SpeciesClause,
    bool ShinyClause,
    bool DupesClause,
    bool LevelCapRequired,
    string? Custom)
{
    public const int MaxCustomLength = 2000;

    /// <summary>O padrão da spec: primeiro encontro, apelido obrigatório e desmaiou morreu.</summary>
    public static NuzlockeRules Default { get; } = new(true, true, true, false, false, false, false, null);

    /// <summary>Confere e limpa o texto livre (vazio vira null).</summary>
    public NuzlockeRules Normalize()
    {
        var custom = Custom?.Trim();
        if (custom is { Length: > MaxCustomLength })
            throw new NuzlockeException("custom_too_long");
        return this with { Custom = string.IsNullOrEmpty(custom) ? null : custom };
    }
}

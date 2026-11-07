using PKHeX.Core;

namespace TradeRock.Core.Builder;

/// <summary>Código de erro do clone no cofre (clone_not_available).</summary>
public sealed class PokemonCloneException(string code) : Exception(code)
{
    public string Code { get; } = code;
}

/// <summary>
/// Clonar no cofre: uma cópia com PID novo (e EC novo quando a geração de origem é a 6 ou mais), para o arquivo ter
/// outro SHA e caber no cofre ao lado do original. O PKHeX.Core não tem um helper que troque o PID mantendo tudo
/// (o <c>SetPIDNature</c>/<c>SetPIDGender</c> só olham uma coisa cada), então o PID é sorteado até casar com o original
/// nas partes que a geração amarra ao PID: Gen 3–5 = natureza (Gen 3–4), gênero, bit da habilidade, forma do Unown
/// (Gen 3), evolução do Wurmple e o brilho (shiny ou não, e o "quadrado" da Gen 8+). Na Gen 3–5 o EC é o próprio
/// PID. O resto dos dados (treinador, IVs, golpes...) fica igual.
/// Aviso: na Gen 3–4 o PID novo quebra a ligação PID/IV dos encontros selvagens, e o PKHeX pode acusar ilegal.
/// </summary>
public static class PokemonClone
{
    private const int MaxTries = 1_000_000;

    /// <summary>Cópia com PID novo. Gen 1–2 não têm PID: <c>clone_not_available</c>.</summary>
    public static PKM Clone(PKM original, Random? random = null)
    {
        if (original.Format <= 2 || original.Species == 0)
            throw new PokemonCloneException("clone_not_available");
        var rnd = random ?? Random.Shared;
        var pk = original.Clone();
        var gen = original.Generation is >= 3 and <= 9 ? original.Generation : original.Format;
        var oldPid = original.PID;
        var shiny = original.IsShiny;
        var square = original.ShinyXor == 0;
        var tsv = (uint)(original.TID16 ^ original.SID16);
        var genderRatio = original.PersonalInfo.Gender;
        var wurmple = WurmpleUtil.IsWurmpleEvo(original.Species) || original.Species == (ushort)PKHeX.Core.Species.Wurmple;

        for (var i = 0; i < MaxTries; i++)
        {
            var pid = RandomPid(rnd, shiny, square, gen, tsv);
            if (pid == oldPid || !Matches(original, pid, oldPid, gen, genderRatio, wurmple))
                continue;
            pk.PID = pid;
            if (pk.IsShiny != shiny)
                continue;
            if (gen <= 5)
            {
                if (pk.Format >= 6)
                    pk.EncryptionConstant = pid;
            }
            else
            {
                NewEncryptionConstant(pk, original, rnd, wurmple);
            }
            pk.RefreshChecksum();
            return pk;
        }
        throw new PokemonCloneException("clone_not_available");
    }

    private static uint RandomPid(Random rnd, bool shiny, bool square, int gen, uint tsv)
    {
        var pid = (uint)rnd.NextInt64(0, 1L << 32);
        if (!shiny)
            return pid;
        // Brilho: (alto ^ baixo ^ TID ^ SID) < 8 (Gen 3–5) ou < 16 (Gen 6+); 0 = "quadrado".
        var low = pid & 0xFFFF;
        uint xor = square ? 0u : (uint)rnd.Next(gen >= 6 ? 1 : 0, gen >= 6 ? 16 : 8);
        return ((low ^ tsv ^ xor) << 16) | low;
    }

    private static bool Matches(PKM original, uint pid, uint oldPid, int gen, byte genderRatio, bool wurmple)
    {
        if (gen > 5)
            return true;
        if (gen <= 4 && pid % 25 != oldPid % 25)
            return false;
        if (EntityGender.GetFromPIDAndRatio(pid, genderRatio) != EntityGender.GetFromPIDAndRatio(oldPid, genderRatio))
            return false;
        if (gen == 5)
        {
            // Habilidade pelo bit 16; o bit 31 e o bit 0 entram na regra dos selvagens da Gen 5.
            if (((pid >> 16) & 1) != ((oldPid >> 16) & 1) || (pid >> 31) != (oldPid >> 31) || (pid & 1) != (oldPid & 1))
                return false;
        }
        else if ((pid & 1) != (oldPid & 1))
        {
            return false;
        }
        if (gen == 3 && original.Species == (ushort)PKHeX.Core.Species.Unown
            && EntityPID.GetUnownForm3(pid) != EntityPID.GetUnownForm3(oldPid))
            return false;
        return !wurmple || WurmpleUtil.GetWurmpleEvoVal(pid) == WurmpleUtil.GetWurmpleEvoVal(oldPid);
    }

    private static void NewEncryptionConstant(PKM pk, PKM original, Random rnd, bool wurmple)
    {
        for (var i = 0; i < MaxTries; i++)
        {
            var ec = (uint)rnd.NextInt64(1, 1L << 32);
            if (ec == original.EncryptionConstant)
                continue;
            if (wurmple && WurmpleUtil.GetWurmpleEvoVal(ec) != WurmpleUtil.GetWurmpleEvoVal(original.EncryptionConstant))
                continue;
            pk.EncryptionConstant = ec;
            return;
        }
    }
}

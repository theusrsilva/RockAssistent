using PKHeX.Core;
using TradeRock.Core.Builder;
using TradeRock.Core.Trading;

namespace TradeRock.Tests;

/// <summary>Clonar no cofre: PID (e EC na Gen 6+) novos, mantendo o que a geração amarra ao PID.</summary>
public sealed class PokemonCloneTests
{
    [Fact]
    public async Task Gengar_pk3_shiny_ganha_PID_novo_e_continua_igual()
    {
        var gengar = await TradeEvolutionPkhexTests.LegalAsync(TradeGame.FireRed, 94);
        gengar.SetShiny();
        Assert.True(gengar.IsShiny);

        var clone = PokemonClone.Clone(gengar, new Random(7));

        Assert.IsType<PK3>(clone);
        Assert.NotEqual(gengar.PID, clone.PID);
        Assert.Equal(gengar.Nature, clone.Nature);
        Assert.Equal(gengar.PID % 25, clone.PID % 25);
        Assert.Equal(gengar.Gender, clone.Gender);
        Assert.Equal(gengar.AbilityNumber, clone.AbilityNumber);
        Assert.Equal(gengar.PID & 1, clone.PID & 1);
        Assert.True(clone.IsShiny);
        Assert.Equal(gengar.OriginalTrainerName, clone.OriginalTrainerName);
        Assert.Equal(((PK3)gengar).IV32, ((PK3)clone).IV32);
        Assert.True(clone.ChecksumValid);
    }

    [Fact]
    public async Task Pk8_ganha_EC_e_PID_novos()
    {
        var kadabra = await TradeEvolutionPkhexTests.LegalAsync(TradeGame.Sword, 64);
        var clone = PokemonClone.Clone(kadabra, new Random(3));

        Assert.IsType<PK8>(clone);
        Assert.NotEqual(kadabra.PID, clone.PID);
        Assert.NotEqual(kadabra.EncryptionConstant, clone.EncryptionConstant);
        Assert.Equal(kadabra.IsShiny, clone.IsShiny);
        Assert.Equal(kadabra.Nature, clone.Nature);
        Assert.True(clone.ChecksumValid);
    }

    [Fact]
    public void Gen1_nao_tem_PID()
    {
        var ex = Assert.Throws<PokemonCloneException>(() => PokemonClone.Clone(new PK1 { Species = 94, CurrentLevel = 30 }));
        Assert.Equal("clone_not_available", ex.Code);
    }
}

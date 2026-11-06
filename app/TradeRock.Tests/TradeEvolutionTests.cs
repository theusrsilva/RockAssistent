using PKHeX.Core;
using TradeRock.Core.Builder;
using TradeRock.Core.Games;
using TradeRock.Core.Pokemon;
using TradeRock.Core.Trading;
using Evolution = TradeRock.Core.Builder.TradeEvolution;

namespace TradeRock.Tests;

/// <summary>Evoluir por troca pela tabela do PKHeX (Tarefa 18). O <c>TradeEvolutionTests</c> antigo é o da Gen 3 do assistente.</summary>
public sealed class TradeEvolutionPkhexTests
{
    private const ushort Kadabra = 64, Alakazam = 65, Haunter = 93, Gengar = 94, Onix = 95, Steelix = 208, Pikachu = 25;

    internal static async Task<PKM> LegalAsync(TradeGame game, ushort species, byte level = 30)
    {
        var trainer = GameCatalog.Of(game).SixDigitTrainerId ? new TrainerSpec("ROCHA", 123456, 1234) : new TrainerSpec("ROCHA", 24680, 13579);
        var result = await PokemonBuilder.GenerateAsync(new PokemonSpec { Species = species, Level = level, Game = game, Trainer = trainer }, "en", default);
        Assert.True(result.Usable, result.Generation);
        return PokemonReader.ToEntity(result.Data!, GameCatalog.Of(game).Family);
    }

    [Fact]
    public async Task Haunter_vira_Gengar_legal()
    {
        var haunter = await LegalAsync(TradeGame.FireRed, Haunter);
        var option = Assert.Single(Evolution.Options(haunter));
        Assert.Equal(new TradeEvolutionOption(Gengar, 0, null), option);

        var gengar = Evolution.Apply(haunter, Gengar, 0);

        Assert.Equal(Haunter, haunter.Species); // o original não muda
        Assert.Equal(Gengar, gengar.Species);
        Assert.Equal(haunter.PID, gengar.PID);
        Assert.Equal(haunter.OriginalTrainerName, gengar.OriginalTrainerName);
        Assert.Equal(haunter.TID16, gengar.TID16);
        Assert.Equal("GENGAR", gengar.Nickname);
        Assert.False(gengar.IsNicknamed);
        Assert.Equal(haunter.Move1_PP, gengar.Move1_PP);
        var report = LegalityReporter.Analyze(gengar, "en");
        Assert.True(report.Status != LegalityStatus.Illegal, string.Join(" | ", report.Problems.Select(p => $"{p.Identifier}/{p.Code}: {p.Message}")));
    }

    [Fact]
    public async Task Pikachu_nao_evolui_por_troca()
    {
        var pikachu = await LegalAsync(TradeGame.FireRed, Pikachu, 10);
        Assert.Empty(Evolution.Options(pikachu));
        Assert.Equal("evolve_not_available", Assert.Throws<TradeEvolutionException>(() => Evolution.Apply(pikachu, 26, 0)).Code);
    }

    [Fact]
    public async Task Kadabra_do_sword_vira_Alakazam()
    {
        var kadabra = await LegalAsync(TradeGame.Sword, Kadabra);
        Assert.IsType<PK8>(kadabra);
        Assert.Equal(Alakazam, Assert.Single(Evolution.Options(kadabra)).Species);
        var alakazam = Evolution.Apply(kadabra, Alakazam, 0);
        Assert.Equal(Alakazam, alakazam.Species);
        Assert.Equal(kadabra.EncryptionConstant, alakazam.EncryptionConstant);
        Assert.Equal("evolve_wrong_target", Assert.Throws<TradeEvolutionException>(() => Evolution.Apply(kadabra, Gengar, 0)).Code);
    }

    [Fact]
    public void Onix_com_metal_coat_vira_Steelix_e_gasta_o_item()
    {
        var onix = new PK3 { Species = Onix, CurrentLevel = 30, Language = 2 };
        onix.ClearNickname();
        var option = Assert.Single(Evolution.Options(onix)); // sem o item, a opção aparece com o item exigido
        Assert.Equal(199, option.RequiredItem); // Metal Coat na numeração da Gen 3
        Assert.Equal(233, Evolution.NationalItem(onix, 199));

        onix.HeldItem = 199;
        var steelix = Evolution.Apply(onix, Steelix, 0);
        Assert.Equal(Steelix, steelix.Species);
        Assert.Equal(0, steelix.HeldItem);
        Assert.Equal("STEELIX", steelix.Nickname);
    }

    [Fact]
    public void Everstone_e_apelido()
    {
        var kadabra = new PK3 { Species = Kadabra, CurrentLevel = 30, Language = 2, HeldItem = 195 }; // Everstone (Gen 3)
        Assert.Empty(Evolution.Options(kadabra));

        kadabra.HeldItem = 0;
        kadabra.SetNickname("ROCHINHA");
        var alakazam = Evolution.Apply(kadabra, Alakazam, 0);
        Assert.Equal("ROCHINHA", alakazam.Nickname);
    }

    [Fact]
    public void Karrablast_pede_troca_com_Shelmet()
    {
        var karrablast = new PK8 { Species = 588, CurrentLevel = 30 };
        var option = Assert.Single(Evolution.Options(karrablast));
        Assert.Equal((ushort)589, option.Species);
        Assert.Equal((ushort)616, option.TradeWith);
        Assert.Null(option.RequiredItem);
    }

    [Fact]
    public void Gen2_pega_o_item_da_tabela_da_gen3()
    {
        var onix = new PK2 { Species = Onix, CurrentLevel = 30 };
        var option = Assert.Single(Evolution.Options(onix));
        Assert.Equal(143, option.RequiredItem); // Metal Coat na Gen 2
        Assert.Equal(233, Evolution.NationalItem(onix, 143));
        var haunter = new PK1 { Species = Haunter, CurrentLevel = 30 };
        var gengar = Evolution.Apply(haunter, Gengar, 0);
        Assert.Equal(Gengar, gengar.Species);
        Assert.Equal(8, ((PK1)gengar).Type1); // Ghost
    }
}

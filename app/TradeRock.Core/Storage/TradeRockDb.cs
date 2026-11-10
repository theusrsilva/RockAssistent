using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TradeRock.Core.Storage;

/// <summary>
/// Banco do app. Pokémon, caixas, tags, histórico, treinadores salvos, anexos, hunts e runs de Nuzlocke são de um dono
/// (<see cref="OwnerScope"/>): as consultas só enxergam os do dono atual e o que é criado vai para ele.
/// </summary>
public sealed class TradeRockDb(DbContextOptions<TradeRockDb> options) : DbContext(options)
{
    /// <summary>Dono lido na criação do contexto (o EF Core parametriza os filtros por instância).</summary>
    public Guid Owner { get; } = OwnerScope.Current;

    public DbSet<StoredPokemon> Pokemon => Set<StoredPokemon>();
    public DbSet<TradeRecord> Trades => Set<TradeRecord>();
    public DbSet<TradeRecordEntry> TradeEntries => Set<TradeRecordEntry>();
    public DbSet<TrainerPreset> TrainerPresets => Set<TrainerPreset>();
    public DbSet<SettingEntry> Settings => Set<SettingEntry>();
    public DbSet<VaultBox> Boxes => Set<VaultBox>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<PokemonTag> PokemonTags => Set<PokemonTag>();
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<SaveAttachment> SaveAttachments => Set<SaveAttachment>();
    public DbSet<ShinyHunt> Hunts => Set<ShinyHunt>();
    public DbSet<NuzlockeRun> NuzlockeRuns => Set<NuzlockeRun>();
    public DbSet<NuzlockeMon> NuzlockeMons => Set<NuzlockeMon>();
    public DbSet<NuzlockeEvent> NuzlockeEvents => Set<NuzlockeEvent>();
    public DbSet<ActivityRecord> ActivityRecords => Set<ActivityRecord>();

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampOwner();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        StampOwner();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void StampOwner()
    {
        foreach (var entry in ChangeTracker.Entries<IOwned>())
        {
            if (entry.State == EntityState.Added && entry.Entity.OwnerId == Guid.Empty)
                entry.Entity.OwnerId = Owner;
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        // O SQLite não ordena DateTimeOffset; guardamos sempre em UTC como inteiro.
        builder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetToBinaryConverter>();
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<StoredPokemon>(e =>
        {
            e.HasQueryFilter(p => p.OwnerId == Owner);
            // Cada cofre guarda o mesmo arquivo uma vez; cofres diferentes podem ter o mesmo Pokémon.
            e.HasIndex(p => new { p.OwnerId, p.Sha256 }).IsUnique();
            e.HasIndex(p => p.Species);
            e.Property(p => p.Origin).HasConversion<string>();
            e.Property(p => p.Format).HasConversion<string>();
            e.Property(p => p.OriginalFormat).HasConversion<string>();
            e.Ignore(p => p.Family);
            // Uma posição de caixa só pode ter um Pokémon ativo (os da lixeira ficam sem caixa).
            e.HasIndex(p => new { p.BoxId, p.Slot }).IsUnique().HasFilter("\"BoxId\" IS NOT NULL AND \"Slot\" IS NOT NULL");
            e.HasIndex(p => p.DeletedAt);
            e.HasOne<VaultBox>().WithMany().HasForeignKey(p => p.BoxId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(p => p.Tags).WithOne().HasForeignKey(t => t.PokemonId).OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<VaultBox>(e =>
        {
            e.HasQueryFilter(b => b.OwnerId == Owner);
            e.HasIndex(b => new { b.OwnerId, b.Order });
        });

        model.Entity<Tag>(e =>
        {
            e.HasQueryFilter(t => t.OwnerId == Owner);
            e.HasIndex(t => new { t.OwnerId, t.Name }).IsUnique();
        });

        model.Entity<PokemonTag>(e =>
        {
            e.HasKey(t => new { t.PokemonId, t.TagId });
            e.HasOne<Tag>().WithMany().HasForeignKey(t => t.TagId).OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<TradeRecord>(e =>
        {
            e.HasQueryFilter(t => t.OwnerId == Owner);
            e.HasIndex(t => new { t.OwnerId, t.StartedAt });
            e.Property(t => t.Game).HasConversion<string>();
            e.Property(t => t.Status).HasConversion<string>();
            e.HasMany(t => t.Entries).WithOne().HasForeignKey(x => x.TradeRecordId).OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<TradeRecordEntry>(e =>
        {
            e.HasOne<StoredPokemon>().WithMany().HasForeignKey(x => x.SentPokemonId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<StoredPokemon>().WithMany().HasForeignKey(x => x.ReceivedPokemonId).OnDelete(DeleteBehavior.Restrict);
        });

        model.Entity<TrainerPreset>(e =>
        {
            e.HasQueryFilter(t => t.OwnerId == Owner);
            e.HasIndex(t => t.OwnerId);
            e.Property(t => t.Game).HasConversion<string>();
        });

        model.Entity<UserAccount>(e =>
        {
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Role).HasConversion<string>();
        });

        model.Entity<SaveAttachment>(e =>
        {
            e.HasQueryFilter(a => a.OwnerId == Owner);
            e.HasIndex(a => new { a.OwnerId, a.SavePath });
            // Apagar o Pokémon do cofre de vez desfaz o anexo.
            e.HasOne<StoredPokemon>().WithMany().HasForeignKey(a => a.PokemonId).OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<ShinyHunt>(e =>
        {
            e.HasQueryFilter(h => h.OwnerId == Owner);
            e.HasIndex(h => new { h.OwnerId, h.StartedAt });
        });

        model.Entity<NuzlockeRun>(e =>
        {
            e.HasQueryFilter(r => r.OwnerId == Owner);
            e.HasIndex(r => new { r.OwnerId, r.StartedAt });
        });

        // Os Pokémon e os eventos não têm dono próprio: só se chega a eles pela run (que tem o filtro).
        model.Entity<NuzlockeMon>(e =>
        {
            e.HasIndex(m => m.RunId);
            e.HasOne<NuzlockeRun>().WithMany().HasForeignKey(m => m.RunId).OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<NuzlockeEvent>(e =>
        {
            e.HasIndex(x => new { x.RunId, x.At });
            e.HasOne<NuzlockeRun>().WithMany().HasForeignKey(x => x.RunId).OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<ActivityRecord>(e =>
        {
            e.HasQueryFilter(a => a.OwnerId == Owner);
            e.HasIndex(a => new { a.OwnerId, a.StartedAt });
            e.Property(a => a.Game).HasConversion<string>();
        });

        model.Entity<SettingEntry>(e => e.HasKey(s => s.Key));
    }
}

/// <summary>Usado só pelo "dotnet ef migrations".</summary>
internal sealed class TradeRockDbDesignFactory : IDesignTimeDbContextFactory<TradeRockDb>
{
    public TradeRockDb CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<TradeRockDb>().UseSqlite("Data Source=design.db").Options);
}

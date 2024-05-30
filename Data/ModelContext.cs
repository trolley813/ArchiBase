using ArchiBase.Models;
using ArchiBase.Users;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Data;

class ModelContext : DbContext
{
    UserResolverService service;

    public ModelContext(DbContextOptions<ModelContext> options, UserResolverService service) : base(options)
    {
        this.service = service;
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<Design> Designs { get; set; }
    public DbSet<DesignCatalogue> DesignCatalogues { get; set; }
    public DbSet<DesignCatalogueEntry> DesignCatalogueEntries { get; set; }

    public DbSet<DesignCategory> DesignCategories { get; set; }

    public DbSet<Translation> Translation { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<Street> Streets { get; set; }
    public DbSet<BuildingCard> BuildingCards { get; set; }
    public DbSet<BuildingEvent> BuildingEvents { get; set; }
    public DbSet<Building> Buildings { get; set; }

    public DbSet<AuditRecord> AuditRecords { get; set; }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Design>()
            .HasMany(e => e.CatalogueEntries)
            .WithOne(e => e.Design);

        modelBuilder.Entity<Design>()
            .HasMany(e => e.Categories)
            .WithMany(e => e.Designs);

        modelBuilder.Entity<DesignCatalogue>()
            .HasMany(e => e.Entries)
            .WithOne(e => e.Catalogue);

        modelBuilder.Entity<DesignCatalogue>()
            .Property(e => e.AbbreviatedFormat)
            .HasDefaultValue("{Code}");

        modelBuilder.Entity<Building>().HasMany(e => e.Cards).WithOne(e => e.Building);

        modelBuilder.Entity<Building>().HasMany(e => e.Events).WithOne(e => e.Building);

        modelBuilder.Entity<BuildingEvent>().OwnsOne(e => e.Date);
        modelBuilder.Entity<BuildingCard>().OwnsOne(e => e.ActualFrom);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public void OnBeforeSaving()
    {
        foreach (var e in ChangeTracker.Entries().ToList())
        {
            switch (e.State)
            {
                case EntityState.Deleted when e.Entity is IAuditable a:
                    AuditRecords.Add(new AuditRecord(a, service.GetUser(), "Deleted"));
                    break;
                case EntityState.Modified when e.Entity is IAuditable a:
                    AuditRecords.Add(new AuditRecord(a, service.GetUser(), "Modified"));
                    break;
                case EntityState.Added when e.Entity is IAuditable a:
                    AuditRecords.Add(new AuditRecord(a, service.GetUser(), "Created"));
                    break;
            }
        }
    }
}
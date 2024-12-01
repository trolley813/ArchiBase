using ArchiBase.Models;
using ArchiBase.Users;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Data;

public class ModelContext : DbContext
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
    public DbSet<Architect> Architects { get; set; }

    public DbSet<Translation> Translation { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<Street> Streets { get; set; }
    public DbSet<BuildingCard> BuildingCards { get; set; }
    public DbSet<Building> Buildings { get; set; }

    public DbSet<License> Licenses { get; set; }
    public DbSet<BuildingBind> BuildingBinds { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Style> Styles { get; set; }

    public IQueryable<Photo> ActivePhotos => Photos.Where(p => !p.IsHidden && p.Status == PhotoStatus.Published);

    public DbSet<Comment> Comments { get; set; }

    public DbSet<NewsItem> NewsItems { get; set; }

    public DbSet<AuditRecord> AuditRecords { get; set; }
    public DbSet<Gallery> Galleries { get; set; }
    public DbSet<StreetAddress> StreetAddresses { get; set; }

    public DbSet<PhotoAuthorMapping> PhotoAuthorMappings { get; set; }
    public DbSet<CommentAuthorMapping> CommentAuthorMappings { get; set; }
    public DbSet<BuildingStatusMapping> BuildingStatusMappings { get; set; }
    public DbSet<CardAddressMapping> CardAddressMappings { get; set; }
    public DbSet<PhotoLocationMapping> PhotoLocationMappings { get; set; }




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

        modelBuilder.Entity<Photo>()
            .Property(e => e.Extension)
            .HasDefaultValue("jpg");

        modelBuilder.Entity<BuildingBind>()
            .Property(b => b.Order)
            .HasDefaultValue(0);

        modelBuilder.Ignore<ArchiBaseUser>();

        modelBuilder.Entity<BuildingCard>().HasMany(e => e.Categories).WithMany(e => e.BuildingCards);

        modelBuilder.Entity<BuildingCard>().OwnsOne(e => e.ActualFrom);
        modelBuilder.Entity<Photo>().OwnsOne(e => e.ShootingDate);
        modelBuilder.Entity<Photo>().OwnsOne(e => e.Exif);
        modelBuilder.Entity<BuildingBind>().OwnsOne(e => e.Markup);

        modelBuilder.Entity<Photo>().OwnsOne(p => p.Votes, v => { v.ToJson(); v.OwnsMany(v => v.Values); });
        modelBuilder.Entity<Comment>().OwnsOne(c => c.Votes, v => { v.ToJson(); v.OwnsMany(v => v.Values); });
        modelBuilder.Entity<Building>().OwnsMany(b => b.Events, e => { e.ToJson(); e.OwnsOne(e => e.Date); });

        modelBuilder.Entity<PhotoAuthorMapping>()
            .ToView("View_PhotoAuthorMappings");
        modelBuilder.Entity<CommentAuthorMapping>()
            .ToView("View_CommentAuthorMappings");
        modelBuilder.Entity<BuildingStatusMapping>()
            .ToView("View_BuildingStatusMappings");
        modelBuilder.Entity<CardAddressMapping>()
            .ToView("View_CardAddressMappings");
        modelBuilder.Entity<PhotoLocationMapping>()
            .ToView("View_PhotoLocationMappings");

        modelBuilder.Entity<BuildingStatusMapping>().OwnsOne(m => m.ConstructionStarted, d => d.ToJson());
        modelBuilder.Entity<BuildingStatusMapping>().OwnsOne(m => m.ConstructionFinished, d => d.ToJson());
        modelBuilder.Entity<BuildingStatusMapping>().OwnsOne(m => m.RebuildingStarted, d => d.ToJson());
        modelBuilder.Entity<BuildingStatusMapping>().OwnsOne(m => m.RebuildingFinished, d => d.ToJson());
        modelBuilder.Entity<BuildingStatusMapping>().OwnsOne(m => m.Abandoned, d => d.ToJson());
        modelBuilder.Entity<BuildingStatusMapping>().OwnsOne(m => m.Demolished, d => d.ToJson());

        modelBuilder.Entity<CardAddressMapping>().OwnsMany(m => m.Addresses, a => a.ToJson());

        modelBuilder.Entity<PhotoLocationMapping>().HasKey(m => new { m.PhotoId, m.LocationId });
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
        var userId = service.GetUser();
        if (userId != Guid.Empty)
        {
            foreach (var e in ChangeTracker.Entries().ToList())
            {
                switch (e.State)
                {
                    case EntityState.Deleted when e.Entity is IAuditable a:
                        AuditRecords.Add(new AuditRecord(a, userId, "Deleted"));
                        break;
                    case EntityState.Modified when e.Entity is IAuditable a:
                        AuditRecords.Add(new AuditRecord(a, userId, "Modified"));
                        break;
                    case EntityState.Added when e.Entity is IAuditable a:
                        AuditRecords.Add(new AuditRecord(a, userId, "Created"));
                        break;
                }
            }
        }
    }
}
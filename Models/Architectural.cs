using System.ComponentModel.DataAnnotations.Schema;
using EFMaterializedPath.Entity;
using SmartFormat;

namespace ArchiBase.Models;

public class DesignCategory : IAuditable, IMaterializedPathEntity<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }
    public List<Design> Designs { get; set; }
    public List<BuildingCard> BuildingCards { get; set; }
    public string Path { get; set; }
    public int Level { get; set; }
    public Guid? ParentId { get; set; }

}

public class DesignCatalogue : IAuditable
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public string AbbreviatedFormat { get; set; } = "{Code}";

    public List<DesignCatalogueEntry> Entries { get; set; } = [];

}

public class DesignCatalogueEntry
{
    public Guid Id { get; set; }
    public Design Design { get; set; }
    public DesignCatalogue Catalogue { get; set; }
    public string Code { get; set; }

    public string? Description { get; set; }

    public string Formatted => Smart.Format(Catalogue.AbbreviatedFormat, new { Code });

}

public class Design : IAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Description { get; set; }

    public decimal? TotalArea { get; set; }

    public decimal? BuildingArea { get; set; }

    public decimal? Cubage { get; set; }

    public List<DesignCatalogueEntry> CatalogueEntries { get; set; } = [];

    public List<DesignCategory> Categories { get; set; } = [];

    public List<Architect> Architects { get; set; } = [];
}

public class Architect : IAuditable
{
    public Guid Id { get; set; }
    public string AbbreviatedName { get; set; }
    public string? FullName { get; set; }
    public ImpreciseDate? DateOfBirth { get; set; }
    public ImpreciseDate? DateOfDeath { get; set; }

    public List<Design> Designs { get; set; } = [];
    public List<BuildingCard> BuildingCards { get; set; } = [];

    [NotMapped]
    public List<Building> Buildings => BuildingCards.Select(c => c.Building).Distinct().ToList();
}

public class Style
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}
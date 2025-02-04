namespace ArchiBase.Shared.Output;

public class DesignOutputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Description { get; set; }

    public decimal? TotalArea { get; set; }

    public decimal? BuildingArea { get; set; }

    public decimal? Cubage { get; set; }

    public List<DesignCatalogueEntryOutputModel> CatalogueEntries { get; set; } = [];

    public List<DesignCategoryOutputModel> Categories { get; set; } = [];

    public List<ArchitectOutputModel> Architects { get; set; } = [];
}


public class DesignBasicOutputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
namespace ArchiBase.Shared.Input;

public class DesignInputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Description { get; set; }

    public decimal? TotalArea { get; set; }

    public decimal? BuildingArea { get; set; }

    public decimal? Cubage { get; set; }

    public List<DesignCatalogueEntryInputModel> CatalogueEntries { get; set; }

    public List<Guid> CategoryIds { get; set; }
}

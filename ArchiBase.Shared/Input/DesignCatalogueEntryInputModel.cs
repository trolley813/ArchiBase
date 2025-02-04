namespace ArchiBase.Shared.Input;

public class DesignCatalogueEntryInputModel
{
    public Guid Id { get; set; }
    public Guid? DesignId { get; set; }
    public Guid? CatalogueId { get; set; }
    public string Code { get; set; }

    public string? Description { get; set; }
}
namespace ArchiBase.Shared.Output;

public class DesignCatalogueOutputModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public string AbbreviatedFormat { get; set; } = "{Code}";

    public List<DesignCatalogueEntryOutputModel> Entries { get; set; } = [];
}

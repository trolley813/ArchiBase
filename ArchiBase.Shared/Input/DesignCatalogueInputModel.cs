namespace ArchiBase.Shared.Input;

public class DesignCatalogueInputModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public string AbbreviatedFormat { get; set; } = "{Code}";

    public List<DesignCatalogueEntryInputModel> Entries { get; set; } = [];
}

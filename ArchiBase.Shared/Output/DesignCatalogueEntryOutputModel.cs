using SmartFormat;

namespace ArchiBase.Shared.Output;

public class DesignCatalogueEntryOutputModel
{
    public Guid Id { get; set; }
    public DesignOutputModel? Design { get; set; }
    public DesignCatalogueOutputModel Catalogue { get; set; }
    public string Code { get; set; }

    public string? Description { get; set; }

    public string Formatted => Smart.Format(Catalogue.AbbreviatedFormat, new { Code });
}

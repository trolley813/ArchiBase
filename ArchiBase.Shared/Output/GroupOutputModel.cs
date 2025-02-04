namespace ArchiBase.Shared.Output;

public class GroupOutputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Description { get; set; }
    public LocationOutputModel Location { get; set; }
    public List<BuildingOutputModel> Buildings { get; set; }
}

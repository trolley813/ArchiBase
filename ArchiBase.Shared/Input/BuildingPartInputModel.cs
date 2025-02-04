namespace ArchiBase.Shared.Input;

public class BuildingPartInputModel
{
    public Guid Id { get; set; }
    public Guid BuildingCardId { get; set; }
    public Guid DesignId { get; set; }
    public string? Name { get; set; }
}
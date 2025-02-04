using ArchiBase.Models;

namespace ArchiBase.Shared.Input;

public class BuildingCardInputModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }


    public Guid BuildingId { get; set; }

    public List<BuildingPartInputModel> Parts { get; set; } = [];
    public List<int> FloorCount { get; set; } = [];
    public ImpreciseDate ActualFrom { get; set; } = new();

    public List<Guid> CategoryIds { get; set; } = [];
    public List<Guid> ArchitectIds { get; set; } = [];

    public List<StreetAddressInputModel> StreetAddresses { get; set; } = [];

    public string? Customer { get; set; }
    public string? Builder { get; set; }
    public Guid? StyleId { get; set; }
}

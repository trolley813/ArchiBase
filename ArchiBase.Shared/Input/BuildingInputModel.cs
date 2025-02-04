using ArchiBase.Models;

namespace ArchiBase.Shared.Input;

public class BuildingInputModel
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<BuildingCardInputModel> Cards { get; set; } = [];
    public List<BuildingEvent> Events { get; set; } = [];

    public Guid? ActualCardId { get; set; }

    public Guid LocationId { get; set; }

    public string? CadastreRecordNumber { get; set; }

    public List<Guid> GroupIds { get; set; }

}

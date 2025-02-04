using ArchiBase.Models;

namespace ArchiBase.Shared.Input;

public class StreetInputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Description { get; set; }

    public Guid LocationId { get; set; }

    public ImpreciseDate? From { get; set; }
    public ImpreciseDate? To { get; set; }

    public Guid? ActualStreetId { get; set; }
}

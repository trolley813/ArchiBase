using ArchiBase.Models;

namespace ArchiBase.Shared.Output;

public class StreetOutputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Description { get; set; }

    public Guid LocationId { get; set; }
    public LocationOutputModel Location { get; set; }

    public ImpreciseDate? From { get; set; }
    public ImpreciseDate? To { get; set; }

    public StreetOutputModel? ActualStreet { get; set; }

    public bool IsActual => ActualStreet is null && To is null;
}

public class StreetBasicOutputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
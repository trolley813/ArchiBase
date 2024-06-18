
using System.ComponentModel.DataAnnotations.Schema;
using EFMaterializedPath.Entity;

namespace ArchiBase.Models;

public enum BuildingEventType
{
    ConstructionStarted = 1,
    ConstructionFinished = 2,
    RebuildingStarted = 3,
    RebuildingFinished = 4,
    Abandoned = 5,
    Demolished = 6,
}
public class Location : IAuditable, IMaterializedPathEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Flag { get; set; }

    public string? Description { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public List<Street> Streets { get; set; }

    public string Path { get; set; }
    public int Level { get; set; }

    public Guid? ParentId { get; set; }

    public bool AllowStreets { get; set; }
}

public class Street : IAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Location Location { get; set; }
}

public class BuildingEvent
{
    public Guid Id { get; set; }
    public Building Building { get; set; }
    public BuildingEventType Type { get; set; }

    public ImpreciseDate Date { get; set; } = new();
}

public class StreetAddress
{
    public Guid Id { get; set; }
    public BuildingCard BuildingCard { get; set; }
    public Street Street { get; set; }
    public string HouseNumber { get; set; }
}

public class BuildingCard
{
    public Guid Id { get; set; }
    public string? Name { get; set; }


    public Building Building { get; set; }

    public List<BuildingPart> Parts { get; set; }
    public List<int> FloorCount { get; set; } = [];
    public ImpreciseDate ActualFrom { get; set; } = new();

    public List<StreetAddress> StreetAddresses { get; set; }
    public List<DesignCategory> Categories { get; set; }

    public string? Customer { get; set; }
    public string? Builder { get; set; }
    public List<Architect> Architects { get; set; }

    [NotMapped]
    public List<DesignCategory> CategoriesOfDesigns => Parts.SelectMany(p => p.Design.Categories).Distinct().ToList();
}

public class BuildingPart
{
    public Guid Id { get; set; }
    public BuildingCard BuildingCard { get; set; }
    public Design Design { get; set; }
    public string? Name { get; set; }
}

public class Building : IAuditable
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public List<BuildingEvent> Events { get; set; } = [];
    public List<BuildingCard> Cards { get; set; } = [];

    public BuildingCard? ActualCard => Cards.MaxBy(c => c.ActualFrom.Date);

    public ImpreciseDate? GetDateOfStatus(BuildingEventType type) =>
        Events.FirstOrDefault(e => e.Type == type)?.Date;

    public Location Location { get; set; }
}

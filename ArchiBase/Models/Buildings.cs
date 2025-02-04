
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EFMaterializedPath.Entity;

namespace ArchiBase.Models;

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

    public string? Description { get; set; }

    public Guid LocationId { get; set; }
    public Location Location { get; set; }

    public ImpreciseDate? From { get; set; }
    public ImpreciseDate? To { get; set; }

    public Guid? ActualStreetId { get; set; }
    public Street? ActualStreet { get; set; }

    public bool IsActual => ActualStreet is null && To is null;
}

public class StreetAddress
{
    public Guid Id { get; set; }
    public Guid BuildingCardId { get; set; }
    public BuildingCard BuildingCard { get; set; }
    public Guid StreetId { get; set; }
    public Street Street { get; set; }
    public string HouseNumber { get; set; }

    public override string ToString()
    {
        return $"{Street.Name}, {HouseNumber}";
    }
}

public class BuildingCard
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public Guid BuildingId { get; set; }
    public Building Building { get; set; }

    public List<BuildingPart> Parts { get; set; }
    public List<int> FloorCount { get; set; } = [];
    public ImpreciseDate ActualFrom { get; set; } = new();

    public List<StreetAddress> StreetAddresses { get; set; }
    public List<DesignCategory> Categories { get; set; }

    public string? Customer { get; set; }
    public string? Builder { get; set; }
    public List<Architect> Architects { get; set; }
    public Guid? StyleId { get; set; }
    public Style? Style { get; set; }
}

public class BuildingPart
{
    public Guid Id { get; set; }
    public Guid BuildingCardId { get; set; }
    public BuildingCard BuildingCard { get; set; }
    public Guid DesignId { get; set; }
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

    public Guid? ActualCardId { get; set; }
    public BuildingCard? ActualCard { get; set; }

    public BuildingCard? ActualToDate(DateTime date) => Cards.Where(c => c.ActualFrom.Date < date).MaxBy(c => c.ActualFrom.Date) ?? ActualCard;

    public string ActualAddress => string.Join(" / ", ActualCard?.StreetAddresses ?? []);
    public string ActualAddressWithLocation => $"{Location.Name}, {ActualAddress}";

    public BuildingEventType ActualStatus =>
        Events.OrderBy(e => e.Date.Date).ThenBy(e => e.Type).LastOrDefault()?.Type ?? BuildingEventType.ConstructionFinished;

    public BuildingEvent? ActualEventToDate(DateTime date) =>
        Events.Where(e => e.Date.Date < date).OrderBy(e => e.Date.Date).ThenBy(e => e.Type).LastOrDefault();

    public ImpreciseDate? GetDateOfStatus(BuildingEventType type) =>
        Events.OrderByDescending(e => e.Date.Date).FirstOrDefault(e => e.Type == type)?.Date;

    public Guid LocationId { get; set; }
    public Location Location { get; set; }

    public string? CadastreRecordNumber { get; set; }

    public List<Group> Groups { get; set; }
}


public class Group
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Description { get; set; }
    public Guid LocationId { get; set; }
    public Location Location { get; set; }
    public List<Building> Buildings { get; set; }
}

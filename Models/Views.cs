using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Models;

public class PhotoAuthorMapping
{
    [Key]
    public Guid PhotoId { get; set; }
    public string AuthorName { get; set; }
}

public class CommentAuthorMapping
{
    [Key]
    public Guid CommentId { get; set; }
    public string AuthorName { get; set; }
}

public class BuildingStatusMapping
{
    [Key]
    public Guid BuildingId { get; set; }

    public ImpreciseDate? ConstructionStarted { get; set; }
    public ImpreciseDate? ConstructionFinished { get; set; }
    public ImpreciseDate? RebuildingStarted { get; set; }
    public ImpreciseDate? RebuildingFinished { get; set; }
    public ImpreciseDate? Abandoned { get; set; }
    public ImpreciseDate? Demolished { get; set; }
}

[Owned]
public class NamedStreetAddress
{
    public string StreetName { get; set; }
    public string HouseNumber { get; set; }

    override public string ToString() => $"{StreetName}, {HouseNumber}";
}

public class CardAddressMapping
{
    [Key]
    public Guid BuildingCardId { get; set; }

    public List<NamedStreetAddress> Addresses { get; set; }
}
public class PhotoLocationMapping
{

    public Photo Photo { get; set; }
    public Guid PhotoId { get; set; }
    public Location Location { get; set; }
    public Guid LocationId { get; set; }
}
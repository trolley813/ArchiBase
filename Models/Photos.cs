using ArchiBase.Utils;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Models;

public enum PhotoStatus
{
    Published,
    Pending,
    Rejected
}

public class License : IAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Link { get; set; }
}

[Owned]
public class BindMarkup
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class BuildingBind
{
    public Guid Id { get; set; }
    public Photo Photo { get; set; }
    public Building Building { get; set; }
    public BindMarkup? Markup { get; set; }
    public bool IsMain { get; set; }
    public int Order { get; set; }
}

public class Gallery : IAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? EntityId { get; set; }
    public string? EntityType { get; set; }
    public List<Photo> Photos { get; set; }
}

[Owned]
public class ExifData
{
    public string? Cameraman { get; set; }
    public string? CameraModel { get; set; }
    public string? Editor { get; set; }
    public string? AName { get; set; }
    public string? Copy { get; set; }
    public DateTime? PhotoDate { get; set; }
    public string? TV { get; set; }
    public string? AV { get; set; }
    public int? ISO { get; set; }
    public string? EV { get; set; }
    public string? FL { get; set; }
    public string? EFL { get; set; }
    public int? Flash { get; set; }
    public int? WB { get; set; }
    public int? EMeter { get; set; }
    public int? SMode { get; set; }
    public string? DZoom { get; set; }
}

public class Photo : IAuditable
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public bool NonAuthor { get; set; }

    public string Extension { get; set; } = "jpg";

    public string? Description { get; set; }
    public ImpreciseDate ShootingDate { get; set; } = new();
    public DateTime PublicationDate { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Direction { get; set; }
    public License? License { get; set; }

    public List<BuildingBind> BuildingBinds { get; set; } = [];

    public List<Gallery> Galleries { get; set; } = [];

    public VoteData Votes { get; set; } = new();

    public int VotesCount => Votes.Votes;
    public int UpvotesCount => Votes.Upvotes;
    public int DownvotesCount => Votes.Downvotes;
    public string PhotoLink => IsHidden ? "/images/hidden.png" : $"/data/photos/{Id.ToString()[0..2]}/{Id.ToString()[2..4]}/{Id}.{Extension}";
    public string PhotoDir => $"/data/photos/{Id.ToString()[0..2]}/{Id.ToString()[2..4]}";

    public bool IsHidden { get; set; }

    public PhotoStatus Status { get; set; }

    public ExifData Exif { get; set; }
}
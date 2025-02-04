using ArchiBase.Utils;

namespace ArchiBase.Models;

public class License : IAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Link { get; set; }
}

public class BuildingBind
{
    public Guid Id { get; set; }
    public Guid PhotoId { get; set; }
    public Photo Photo { get; set; }
    public Guid BuildingId { get; set; }
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
    public Guid? LicenseId { get; set; }
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
    public bool? IsLost { get; set; }


    public PhotoStatus Status { get; set; }

    public ExifData Exif { get; set; }
    public byte[]? ThumbnailData { get; set; }
}

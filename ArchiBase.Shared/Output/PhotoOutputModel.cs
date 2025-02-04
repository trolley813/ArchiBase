using ArchiBase.Models;
using ArchiBase.Utils;

namespace ArchiBase.Shared.Output;

public class PhotoOutputModel
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
    public LicenseOutputModel? License { get; set; }

    public List<BuildingBindOutputModel> BuildingBinds { get; set; } = [];

    public List<GalleryOutputModel> Galleries { get; set; } = [];

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

using ArchiBase.Models;
using ArchiBase.Utils;

namespace ArchiBase.Shared.Input;

public class PhotoInputModel
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

    public List<Guid?> GalleryIds { get; set; }
    public List<BuildingBindInputModel> BuildingBinds { get; set; } = [];


    public VoteData Votes { get; set; } = new();

    public bool IsHidden { get; set; }
    public bool? IsLost { get; set; }


    public PhotoStatus Status { get; set; }

    public ExifData Exif { get; set; }
    public byte[]? ThumbnailData { get; set; }
}
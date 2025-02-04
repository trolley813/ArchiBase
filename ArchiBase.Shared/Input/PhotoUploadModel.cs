using ArchiBase.Models;

namespace ArchiBase.Shared.Input;

public class BuildingBindModel
{
    public Guid BuildingId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsMain { get; set; } = true;
}

public class UploadModel
{
    public string? File { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public List<BuildingBindModel> Binds { get; set; } = [];
    public ImpreciseDate ShootingDate { get; set; } = new();

    public Guid LocationId { get; set; }
    public Guid LicenseId { get; set; }
    public bool NonAuthor { get; set; } = false;
    public int Direction { get; set; } = 360;

    public ExifData ExifData { get; set; } = new();

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public string Description { get; set; } = "";

    public bool UseDirectUpload { get; set; }
}

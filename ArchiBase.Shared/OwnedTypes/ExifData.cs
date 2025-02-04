using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Models;

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

namespace ArchiBase.Shared.Input;

public class LocationInputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Flag { get; set; }

    public string? Description { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public string Path { get; set; }
    public int Level { get; set; }

    public Guid? ParentId { get; set; }

    public bool AllowStreets { get; set; }
}

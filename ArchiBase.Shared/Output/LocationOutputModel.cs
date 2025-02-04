namespace ArchiBase.Shared.Output;

public class LocationOutputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Flag { get; set; }

    public string? Description { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public List<StreetOutputModel> Streets { get; set; }

    public string Path { get; set; }
    public int Level { get; set; }

    public Guid? ParentId { get; set; }

    public bool AllowStreets { get; set; }
}

public class LocationBasicOutputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
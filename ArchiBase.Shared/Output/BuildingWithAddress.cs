using ArchiBase.Models;
namespace ArchiBase.Shared.Output;

public class BuildingWithAddress
{
    public BuildingOutputModel Building { get; set; }
    public List<NamedStreetAddress> Addresses { get; set; }
}

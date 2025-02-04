using ArchiBase.Models;
using ArchiBase.Utils;

namespace ArchiBase.Shared.Output;

public class BuildingInfoOutputModel
{
    public BuildingOutputModel Building { get; set; }
    public BuildingCardOutputModel ActualCard { get; set; }
    public BuildingCardOutputModel? LastCardWithAddress { get; set; }
    public NaturalString HouseNumber { get; set; }
    public string FloorCount { get; set; }
    public string Name { get; set; }
    public ImpreciseDate? Built { get; set; }

    public DateTime BuiltSort { get; set; }
    public ImpreciseDate? Demolished { get; set; }

    public DateTime DemolishedSort { get; set; }
    public List<StreetAddressOutputModel> Addresses { get; set; }
}

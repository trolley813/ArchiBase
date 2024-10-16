using ArchiBase.Models;

namespace ArchiBase.Components.Pages.Utils;

class BuildingInfo
{
    public Building Building { get; set; }
    public BuildingCard ActualCard { get; set; }
    public BuildingCard? LastCardWithAddress { get; set; }
    public NaturalString HouseNumber { get; set; }
    public NaturalString FloorCount { get; set; }
    public string Name { get; set; }
    public ImpreciseDate? Built { get; set; }
    public ImpreciseDate? Demolished { get; set; }

    public List<StreetAddress> Addresses { get; set; }
}
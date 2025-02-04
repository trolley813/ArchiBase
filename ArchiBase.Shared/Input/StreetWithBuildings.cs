using ArchiBase.Models;

namespace ArchiBase.Shared.Input;

public class StreetWithBuildings
{
    public StreetInputModel Street { get; set; }
    public List<BuildingCreateInputModel> Buildings { get; set; }
}

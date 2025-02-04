namespace ArchiBase.Shared.Output;
using ArchiBase.Utils;

public class ExtendedBuildingListPage
{
    public int Count { get; set; }
    public List<BuildingInfoOutputModel> Buildings { get; set; }

    public Dictionary<Guid, string> PhotoData { get; set; }
}

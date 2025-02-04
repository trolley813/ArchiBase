namespace ArchiBase.Shared.Input;

public class BuildingCreateInputModel
{
    public string Number { get; set; } = "";
    public DesignInputModel? Design { get; set; }

    public List<DesignCategoryInputModel> Categories { get; set; } = [];
    public int? ConstructionYear { get; set; }
    public int? DemolitionYear { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

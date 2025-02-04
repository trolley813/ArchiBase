using System.Text.RegularExpressions;
using ArchiBase.Models;
using ArchiBase.Shared.Output;
using Radzen;

namespace ArchiBase.Components.Utils;

public abstract class MarkerBase
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public virtual object[] Render { get; }
}

public class PrincipalMarker : MarkerBase
{
    override public object[] Render => [Latitude, Longitude];
}

public class SublocationMarker : MarkerBase
{
    public string SublocationName { get; set; }

    public SublocationMarker(LocationOutputModel location)
    {
        SublocationName = location.Name;
        Latitude = location.Latitude;
        Longitude = location.Longitude;
    }
    override public object[] Render => [Latitude, Longitude, SublocationName];
}

public class CameraMarker : MarkerBase
{
    public CameraMarker() { }
    public CameraMarker(PhotoOutputModel photo)
    {
        Latitude = photo.Latitude;
        Longitude = photo.Longitude;
        Direction = photo.Direction;
    }

    public double Direction { get; set; }

    public override object[] Render => [Latitude, Longitude, Direction];
}

public partial class BuildingMarker : MarkerBase
{
    public BuildingMarker() { }

    public BuildingMarker(BuildingOutputModel building)
    {
        BuildingId = building.Id;
        Latitude = building.Latitude;
        Longitude = building.Longitude;
        Label = string.Join("/", building.ActualCard.StreetAddresses.Select(a => GetHouseNumberAbbreviation(a.HouseNumber)));
        Tooltip = string.Join(" / ", building.ActualCard.StreetAddresses.OrderBy(a => a.HouseNumber).Select(a => $"{a.Street.Name}, {a.HouseNumber}"));
        Status = building.ActualStatus;
    }

    public Guid BuildingId { get; set; }
    public string Label { get; set; }
    public string Tooltip { get; set; }
    public bool HasPhotos { get; set; } = false;
    public BuildingEventType Status { get; set; } = BuildingEventType.ConstructionFinished;

    public override object[] Render => [Latitude, Longitude, Label, Tooltip, (HasPhotos ? "#ffffff" : "#ffa500"), GetColor(Status)];

    static string GetColor(BuildingEventType status) => status switch
    {
        BuildingEventType.ConstructionStarted => "#32cd32",
        BuildingEventType.RebuildingStarted => "ffd700",
        BuildingEventType.Abandoned => "#808080",
        BuildingEventType.Demolished => "#dc143c",
        _ => "#000000"
    };

    static string GetHouseNumberAbbreviation(string houseNumber)
    {
        var strippedParentheses = StripParenthesesRegex().Replace(houseNumber, "");
        var addressParts = WhitespaceRegex().Split(strippedParentheses).Where(s => s != "").ToArray();
        for (int i = 0; i < addressParts.Length; i++)
        {
            // reduce any non-digit element to first letter
            if (!addressParts[i].Any(char.IsDigit)) addressParts[i] = addressParts[i][0..1];
        }
        // join without any spaces
        return string.Join("", addressParts);
    }

    [GeneratedRegex(@"\(.+?\)")]
    private static partial Regex StripParenthesesRegex();
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
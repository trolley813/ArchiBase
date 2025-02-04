using ArchiBase.Models;

namespace ArchiBase.Shared.Output;

public class BuildingOutputModel
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<BuildingEvent> Events { get; set; } = [];
    public List<BuildingCardOutputModel> Cards { get; set; } = [];

    public BuildingCardOutputModel? ActualCard { get; set; }

    public BuildingCardOutputModel? ActualToDate(DateTime date) => Cards.Where(c => c.ActualFrom.Date < date).MaxBy(c => c.ActualFrom.Date) ?? ActualCard;

    public string ActualAddress => string.Join(" / ", ActualCard?.StreetAddresses ?? []);
    public string ActualAddressWithLocation => $"{Location.Name}, {ActualAddress}";

    public BuildingEventType ActualStatus =>
        Events.OrderBy(e => e.Date.Date).ThenBy(e => e.Type).LastOrDefault()?.Type ?? BuildingEventType.ConstructionFinished;

    public BuildingEvent? ActualEventToDate(DateTime date) =>
        Events.Where(e => e.Date.Date < date).OrderBy(e => e.Date.Date).ThenBy(e => e.Type).LastOrDefault();

    public ImpreciseDate? GetDateOfStatus(BuildingEventType type) =>
        Events.OrderByDescending(e => e.Date.Date).FirstOrDefault(e => e.Type == type)?.Date;

    public LocationOutputModel Location { get; set; }

    public string? CadastreRecordNumber { get; set; }

    public List<GroupOutputModel> Groups { get; set; }
}

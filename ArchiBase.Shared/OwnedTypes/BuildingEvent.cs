using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Models;

public enum BuildingEventType
{
    [Display(Name = "Construction started")]
    ConstructionStarted = 1,
    [Display(Name = "Construction finished")]
    ConstructionFinished = 2,
    [Display(Name = "Rebuilding started")]
    RebuildingStarted = 3,
    [Display(Name = "Rebuilding finished")]
    RebuildingFinished = 4,
    [Display(Name = "Abandoned")]
    Abandoned = 5,
    [Display(Name = "Demolished")]
    Demolished = 6,
}

[Owned]
public class BuildingEvent
{
    public BuildingEventType Type { get; set; } = BuildingEventType.ConstructionFinished;

    public ImpreciseDate Date { get; set; } = new();
}

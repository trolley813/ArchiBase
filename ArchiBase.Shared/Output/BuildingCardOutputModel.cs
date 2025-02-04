using System.ComponentModel.DataAnnotations.Schema;
using ArchiBase.Models;

namespace ArchiBase.Shared.Output;

public class BuildingCardOutputModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public BuildingOutputModel Building { get; set; }

    public List<BuildingPartOutputModel> Parts { get; set; }
    public List<int> FloorCount { get; set; } = [];
    public ImpreciseDate ActualFrom { get; set; } = new();

    public List<StreetAddressOutputModel> StreetAddresses { get; set; }
    public List<DesignCategoryOutputModel> Categories { get; set; }

    public string? Customer { get; set; }
    public string? Builder { get; set; }
    public List<ArchitectOutputModel> Architects { get; set; }
    public StyleOutputModel? Style { get; set; }

    public List<DesignCategoryOutputModel> CategoriesOfDesigns => Parts?.SelectMany(p => p.Design.Categories).Distinct().ToList() ?? [];
}

using ArchiBase.Models;

namespace ArchiBase.Shared.Output;

public class ArchitectOutputModel
{
    public Guid Id { get; set; }
    public string AbbreviatedName { get; set; }
    public string? FullName { get; set; }
    public ImpreciseDate? DateOfBirth { get; set; }
    public ImpreciseDate? DateOfDeath { get; set; }

    public List<DesignOutputModel> Designs { get; set; } = [];
    public List<BuildingCardOutputModel> BuildingCards { get; set; } = [];

    public List<BuildingOutputModel> Buildings => BuildingCards.Select(c => c.Building).Distinct().ToList();
}

public class ArchitectBasicOutputModel
{
    public Guid Id { get; set; }
    public string AbbreviatedName { get; set; }
}
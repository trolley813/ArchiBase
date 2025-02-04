using ArchiBase.Models;

namespace ArchiBase.Shared.Input;

public class ArchitectInputModel
{
    public Guid Id { get; set; }
    public string AbbreviatedName { get; set; }
    public string? FullName { get; set; }
    public ImpreciseDate? DateOfBirth { get; set; }
    public ImpreciseDate? DateOfDeath { get; set; }
}

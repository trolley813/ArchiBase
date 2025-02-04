namespace ArchiBase.Shared.Input;

public class GroupInputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string? Description { get; set; }
    public Guid LocationId { get; set; }
}
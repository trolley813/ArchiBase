namespace ArchiBase.Shared.Input;

public class BuildingBindInputModel
{
    public Guid Id { get; set; }
    public Guid PhotoId { get; set; }
    public Guid BuildingId { get; set; }
    public BindMarkup? Markup { get; set; }
    public bool IsMain { get; set; }
    public int Order { get; set; }
}

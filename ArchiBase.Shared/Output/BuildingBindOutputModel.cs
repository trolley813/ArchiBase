namespace ArchiBase.Shared.Output;

public class BuildingBindOutputModel
{
    public Guid Id { get; set; }
    public PhotoOutputModel Photo { get; set; }
    public BuildingOutputModel Building { get; set; }
    public BindMarkup? Markup { get; set; }
    public bool IsMain { get; set; }
    public int Order { get; set; }
}

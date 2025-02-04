namespace ArchiBase.Shared.Output;

public class DesignCategoryOutputModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public List<DesignOutputModel> Designs { get; set; }
    public List<BuildingCardOutputModel> BuildingCards { get; set; }

    public Guid? ParentId { get; set; }

}

public class DesignCategoryBasicOutputModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }
}
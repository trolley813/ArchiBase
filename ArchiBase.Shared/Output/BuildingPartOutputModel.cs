namespace ArchiBase.Shared.Output
{
    public class BuildingPartOutputModel
    {
        public Guid Id { get; set; }
        public BuildingCardOutputModel BuildingCard { get; set; }
        public Guid DesignId { get; set; }
        public DesignOutputModel Design { get; set; }
        public string? Name { get; set; }
    }
}

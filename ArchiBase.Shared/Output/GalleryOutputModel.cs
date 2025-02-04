namespace ArchiBase.Shared.Output
{
    public class GalleryOutputModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? EntityId { get; set; }
        public string? EntityType { get; set; }
        public List<PhotoOutputModel> Photos { get; set; }
    }
}

namespace ArchiBase.Shared.Input
{
    public class GalleryInputModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? EntityId { get; set; }
        public string? EntityType { get; set; }
    }
}

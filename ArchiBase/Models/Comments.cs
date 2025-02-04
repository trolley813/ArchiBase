using ArchiBase.Utils;

namespace ArchiBase.Models;

public class Comment
{
    public Guid Id { get; set; }
    public string? EntityType { get; set; }
    public Guid EntityId { get; set; }
    public Guid AuthorId { get; set; }
    public string Text { get; set; }
    public DateTime PublicationDate { get; set; }

    public VoteData Votes { get; set; } = new();

    public bool IsRecorded { get; set; }
}
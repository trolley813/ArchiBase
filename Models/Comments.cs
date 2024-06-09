namespace ArchiBase.Models;

public class Comment
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public Guid AuthorId { get; set; }
    public string Text { get; set; }
    public DateTime PublicationDate { get; set; }

    public List<CommentVote> Votes { get; set; } = [];

    public int VotesCount => Votes.Select(v => v.Vote).Sum();
    public int UpvotesCount => Votes.Where(v => v.Vote > 0).Count();
    public int DownvotesCount => Votes.Where(v => v.Vote < 0).Count();
}

public class CommentVote
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Comment Comment { get; set; }
    public int Vote { get; set; }
}
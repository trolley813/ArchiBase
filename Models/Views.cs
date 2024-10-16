using System.ComponentModel.DataAnnotations;

namespace ArchiBase.Models;

public class PhotoAuthorMapping
{
    [Key]
    public Guid PhotoId { get; set; }
    public string AuthorName { get; set; }
}

public class CommentAuthorMapping
{
    [Key]
    public Guid CommentId { get; set; }
    public string AuthorName { get; set; }
}
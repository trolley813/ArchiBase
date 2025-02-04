namespace ArchiBase.Models;

public class NewsItem
{
    public Guid Id { get; set; }
    public DateTime PublicationDate { get; set; }
    public string Text { get; set; }
}
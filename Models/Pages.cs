namespace ArchiBase.Models;

public class Page : IAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
}
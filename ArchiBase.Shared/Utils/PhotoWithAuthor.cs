namespace ArchiBase.Utils;
using ArchiBase.Shared.Output;

public class PhotoWithAuthor
{
    public PhotoOutputModel Photo { get; set; }
    public DateTime ShootingDateSort { get; set; }
    public DateTime PublicationDateSort { get; set; }
    public string AuthorName { get; set; }
}

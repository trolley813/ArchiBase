namespace ArchiBase.Shared.Output;

public class UserOutputModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public string Bio { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;

    public DateTime? LastReadCommentTime { get; set; }

    public List<Guid> Locations { get; set; } = [];

    public Guid? MyLocation { get; set; }
}
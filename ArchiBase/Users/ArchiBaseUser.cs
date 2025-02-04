using Microsoft.AspNetCore.Identity;

namespace ArchiBase.Users;

public class ArchiBaseUser : IdentityUser<Guid>
{
    public DateTime? DateOfBirth { get; set; }

    public string Bio { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;

    public List<Guid> Locations { get; set; } = [];

    public Guid? MyLocation { get; set; }

    public DateTime? LastReadCommentTime { get; set; }

    public int? UploadLimitOverride { get; set; }
    public int? DirectUploadLimitOverride { get; set; }
}
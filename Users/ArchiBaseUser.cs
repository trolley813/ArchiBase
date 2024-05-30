using ArchiBase.Models;
using Microsoft.AspNetCore.Identity;

namespace ArchiBase.Users;

public class ArchiBaseUser : IdentityUser<Guid>
{
    public DateTime? DateOfBirth { get; set; }

    public string Bio { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;
}
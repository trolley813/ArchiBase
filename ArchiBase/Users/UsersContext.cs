using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Users;

public class UsersContext : IdentityDbContext<ArchiBaseUser, ArchiBaseRole, Guid>
{
    public UsersContext(DbContextOptions<UsersContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}
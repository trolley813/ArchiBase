using ArchiBase.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Users;

class UsersContext : IdentityDbContext<ArchiBaseUser, ArchiBaseRole, Guid>
{
    public UsersContext(DbContextOptions<UsersContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}
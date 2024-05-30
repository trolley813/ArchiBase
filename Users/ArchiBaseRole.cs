using ArchiBase.Models;
using Microsoft.AspNetCore.Identity;

namespace ArchiBase.Users;

public class ArchiBaseRole(string name) : IdentityRole<Guid>(name)
{
}
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ArchiBase.Users;

public class UserResolverService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    public UserResolverService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUser()
    {
        var userIdString = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        _ = Guid.TryParse(userIdString, out Guid userId);
        return userId;
    }
}
using Microsoft.AspNetCore.Components.Authorization;

namespace ArchiBase.Users;

public class UserResolverService
{
    private readonly AuthenticationStateProvider provider;
    public UserResolverService(AuthenticationStateProvider provider)
    {
        this.provider = provider;
    }

    public Guid GetUser()
    {
        var context = provider.GetAuthenticationStateAsync().GetAwaiter().GetResult();
        Guid.TryParse(
            context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            out Guid userId
        );
        return userId;
    }
}
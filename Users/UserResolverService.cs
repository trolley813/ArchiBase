namespace ArchiBase.Users;

public class UserResolverService
{
    private readonly IHttpContextAccessor context;
    public UserResolverService(IHttpContextAccessor context)
    {
        this.context = context;
    }

    public Guid GetUser()
    {
        Guid.TryParse(
            context.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            out Guid userId
        );
        return userId;
    }
}
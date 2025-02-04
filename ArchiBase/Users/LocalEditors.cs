using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ArchiBase.Users;

public class LocalEditorRequirementHandlerServer : AuthorizationHandler<LocalEditorRequirement>
{
    private readonly LocalEditorService localEditorService;
    private readonly UserResolverService userResolverService;
    private readonly UserManager<ArchiBaseUser> userManager;

    public LocalEditorRequirementHandlerServer(
        LocalEditorService localEditorService,
        UserResolverService userResolverService,
        UserManager<ArchiBaseUser> userManager
        )
    {
        this.localEditorService = localEditorService;
        this.userResolverService = userResolverService;
        this.userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, LocalEditorRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        var userId = userResolverService.GetUser();

        var isGlobalEditor = context.User.IsInRole("Admin") || context.User.IsInRole("Editor");

        if (isGlobalEditor || localEditorService.CanEdit(await userManager.FindByIdAsync(userId.ToString()), requirement.Location))
        {
            context.Succeed(requirement);
        }
    }
}
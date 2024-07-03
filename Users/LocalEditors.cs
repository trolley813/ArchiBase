using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ArchiBase.Users;

public class LocalEditorRequirement : IAuthorizationRequirement
{
    public LocalEditorRequirement(Guid location)
    {
        Location = location;
    }

    public Guid Location { get; set; }
}

public class LocalEditorRequirementHandler : AuthorizationHandler<LocalEditorRequirement>
{
    private readonly LocalEditorService localEditorService;
    private readonly UserResolverService userResolverService;
    private readonly UserManager<ArchiBaseUser> userManager;

    public LocalEditorRequirementHandler(
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

public class LocalEditorPolicyProvider : IAuthorizationPolicyProvider
{
    const string POLICY_PREFIX = "LocalEditor-";

    // Policies are looked up by string name, so expect 'parameters' (like age)
    // to be embedded in the policy names. This is abstracted away from developers
    // by the more strongly-typed attributes derived from AuthorizeAttribute
    // (like [MinimumAgeAuthorize()] in this sample)
    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase) &&
            Guid.TryParse(policyName[POLICY_PREFIX.Length..], out var location))
        {
            var policy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
            policy.RequireRole("Admin", "Editor", "Local Editor");
            policy.AddRequirements(new LocalEditorRequirement(location));
            return Task.FromResult(policy.Build());
        }

        return Task.FromResult<AuthorizationPolicy>(null);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        Task.FromResult(new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() =>
        Task.FromResult<AuthorizationPolicy>(null);
}
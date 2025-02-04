using System.Net.Http.Json;
using ArchiBase.Shared.Output;
using ArchiBase.Utils;
using Microsoft.AspNetCore.Authorization;

namespace ArchiBase.Users;

public class LocalEditorRequirement : IAuthorizationRequirement
{
    public LocalEditorRequirement(Guid location)
    {
        Location = location;
    }

    public Guid Location { get; set; }
}

public class LocalEditorRequirementHandlerClient(IHttpClientFactory clientFactory) : AuthorizationHandler<LocalEditorRequirement>
{
    private readonly IHttpClientFactory clientFactory = clientFactory;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, LocalEditorRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);
        ArgumentNullException.ThrowIfNull(clientFactory);

        using var client = clientFactory.CreateClient();
        var currentUser = await client.GetFromJsonAsyncExtended<UserOutputModel>("/api/users/current");
        if (currentUser is null)
        {
            context.Fail(new AuthorizationFailureReason(this, "Unauthorized user"));
            return;
        }
        try
        {
            var canEdit = await client.GetFromJsonAsync<bool>($"/api/users/{currentUser.Id}/canedit?locid={requirement.Location}");
            if (canEdit)
            {
                context.Succeed(requirement);
            }
        }
        catch (Exception ex)
        {
            context.Fail(new AuthorizationFailureReason(this, $"Exception occured when verifying: {ex.Message}"));
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
            var policy = new AuthorizationPolicyBuilder("Identity.Application");
            policy.RequireRole("Admin", "Editor", "Local Editor");
            policy.AddRequirements(new LocalEditorRequirement(location));
            return Task.FromResult(policy.Build());
        }

        return Task.FromResult<AuthorizationPolicy>(null);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        Task.FromResult(new AuthorizationPolicyBuilder("Identity.Application").RequireAuthenticatedUser().Build());

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() =>
        Task.FromResult<AuthorizationPolicy>(null);
}
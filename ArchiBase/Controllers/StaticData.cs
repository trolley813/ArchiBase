using Microsoft.AspNetCore.Identity;

namespace ArchiBase.Controllers;

static class StaticData
{
    public const string AuthSchemes =
        "Identity.Application,Identity.External,Identity.TwoFactorRememberMe,Identity.TwoFactorUserId";
}
using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Users;
using Microsoft.AspNetCore.Identity;

namespace ArchiBase.Utils;

class UploadLimitService(
    ModelContext modelContext,
    UserManager<ArchiBaseUser> userManager)
{
    private readonly ModelContext modelContext = modelContext;

    private readonly UserManager<ArchiBaseUser> userManager = userManager;

    public int GetUploadLimit(Guid userId)
    {
        var user = userManager.Users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            return user.UploadLimitOverride ??
                (int)Math.Ceiling(2 * Math.Sqrt(modelContext.Photos.Where(p => p.Status == PhotoStatus.Published && p.AuthorId == userId).Count()));
        }
        else
        {
            return 0;
        }
    }

    public int GetDirectUploadLimit(Guid userId)
    {
        var user = userManager.Users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            return user.DirectUploadLimitOverride ?? 0;
        }
        else
        {
            return 0;
        }
    }
}

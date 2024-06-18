using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Users;
using Microsoft.AspNetCore.Identity;

namespace ArchiBase.Utils;

class CommentService(
    UserResolverService userResolverService,
    ModelContext modelContext,
    UserManager<ArchiBaseUser> userManager)
{
    private readonly UserResolverService userResolverService = userResolverService;
    private readonly ModelContext modelContext = modelContext;

    private readonly UserManager<ArchiBaseUser> userManager = userManager;

    public DateTime GetLastReadTimeForCurrentUser()
    {
        try
        {
            var guid = userResolverService.GetUser();
            var user = userManager.Users.FirstOrDefault(u => u.Id == guid);
            return user?.LastReadCommentTime ?? DateTime.MinValue;
        }
        catch (Exception ex)
        {
            return DateTime.MinValue;
        }
    }

    public int GetNewCommentsCount()
    {
        try
        {
            var guid = userResolverService.GetUser();
            var user = userManager.Users.FirstOrDefault(u => u.Id == guid);
            if (user is null)
            {
                return -1;
            }
            if (user.LastReadCommentTime is null)
            {
                return modelContext.Comments.Count();
            }
            return modelContext.Comments.Count(c => c.PublicationDate > user.LastReadCommentTime);
        }
        catch (Exception ex)
        {
            return -1;
        }
    }
}
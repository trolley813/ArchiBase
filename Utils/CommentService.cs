using System.Linq.Dynamic.Core;
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

    public int GetResponsesCount()
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
            return modelContext.Comments
                .Where(c => c.PublicationDate > user.LastReadCommentTime
                && c.AuthorId != guid
                && modelContext.Comments.Where(com => com.AuthorId == guid && com.EntityId == c.EntityId).DefaultIfEmpty().Max(com => com.PublicationDate) < c.PublicationDate
                ).Count();
        }
        catch (Exception ex)
        {
            return -1;
        }
    }
}
using System.Security.Claims;
using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Controllers;

[Authorize(AuthenticationSchemes = StaticData.AuthSchemes)]
[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Default")]
public class LimitsController(ModelContext modelContext, UploadLimitService uploadLimitService) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly UploadLimitService uploadLimitService = uploadLimitService;

    [HttpGet("upload")]
    public ActionResult<int> UploadLimit()
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    out Guid userId
                ))
        {
            var userPhotosCountThisWeek = modelContext.Photos.Where(p => p.AuthorId == userId &&
                    (DateTime.UtcNow.Date - p.PublicationDate.Date).TotalDays < 7).Count();
            var limit = Math.Max(uploadLimitService.GetUploadLimit(userId) - userPhotosCountThisWeek, 0);
            return Ok(limit);
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet("direct")]
    public ActionResult<int> DirectUploadLimit()
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    out Guid userId
                ))
        {
            var userPhotosCountThisWeek = modelContext.Photos.Where(p => p.AuthorId == userId &&
                    (DateTime.UtcNow.Date - p.PublicationDate.Date).TotalDays < 7).Count();
            var directLimit = Math.Max(uploadLimitService.GetDirectUploadLimit(userId) - userPhotosCountThisWeek, 0);
            return Ok(directLimit);
        }
        else
        {
            return Unauthorized();
        }
    }
}

using System.Security.Claims;
using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Shared.Input;
using ArchiBase.Shared.Output;
using ArchiBase.Users;
using ArchiBase.Utils;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Short")]
public class CommentsController(
    ModelContext modelContext,
    UserManager<ArchiBaseUser> userManager,
    IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly UserManager<ArchiBaseUser> userManager = userManager;
    private readonly IMapper mapper = mapper;

    [HttpGet("")]
    public ActionResult<CommentPage> Index(Guid? entityId, Guid? authorId, int? limit, int? offset, bool? newestFirst, DateTime? dateFrom, DateTime? dateTo, string? contains)
    {
        IQueryable<Comment> query = modelContext.Comments.Include(c => c.Votes);
        if (entityId is not null)
        {
            query = query.Where(c => c.EntityId == entityId);
        }
        if (authorId is not null)
        {
            query = query.Where(c => c.AuthorId == authorId);
        }
        if (newestFirst ?? false)
        {
            query = query.OrderByDescending(c => c.PublicationDate);
        }
        else
        {
            query = query.OrderBy(c => c.PublicationDate);
        }
        if (dateFrom is not null)
        {
            query = query.Where(c => c.PublicationDate >= dateFrom);
        }
        if (dateTo is not null)
        {
            query = query.Where(c => c.PublicationDate >= dateFrom);
        }
        if (contains is not null)
        {
            query = query.Where(c => c.Text.Contains(contains));
        }
        var count = query.Count();
        if (offset is not null)
        {
            query = query.Skip((int)offset);
        }
        if (limit is not null)
        {
            query = query.Take((int)limit);
        }
        return Ok(new CommentPage() { Comments = mapper.Map<List<CommentOutputModel>>(query.ToList()), Count = count });
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes)]
    [HttpGet("new")]
    public async Task<ActionResult<CommentPage>> NewComments(int? limit, int? offset)
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out Guid userId
        ))
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            var lastReadTime = user?.LastReadCommentTime ?? DateTime.MinValue;
            IQueryable<Comment> query = modelContext.Comments.Where(c => c.PublicationDate > lastReadTime)
                .OrderBy(c => c.PublicationDate).Include(c => c.Votes);
            var count = query.Count();
            if (offset is not null)
            {
                query = query.Skip((int)offset);
            }
            if (limit is not null)
            {
                query = query.Take((int)limit);
            }
            return Ok(new CommentPage() { Comments = mapper.Map<List<CommentOutputModel>>(query.ToList()), Count = count });
        }
        else
        {
            return Unauthorized();
        }
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes)]
    [HttpGet("responses")]
    public async Task<ActionResult<CommentPage>> Responses(int? limit, int? offset)
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out Guid userId
        ))
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            var lastReadTime = user?.LastReadCommentTime ?? DateTime.MinValue;
            IQueryable<Comment> query = modelContext.Comments
            .Where(c => c.PublicationDate > lastReadTime && c.AuthorId != user.Id
            && modelContext.Comments.Where(com => com.AuthorId == user.Id && com.EntityId == c.EntityId).DefaultIfEmpty().Max(com => com.PublicationDate) < c.PublicationDate)
            .OrderBy(c => c.PublicationDate).Include(c => c.Votes);
            var count = query.Count();
            if (offset is not null)
            {
                query = query.Skip((int)offset);
            }
            if (limit is not null)
            {
                query = query.Take((int)limit);
            }
            return Ok(new CommentPage() { Comments = mapper.Map<List<CommentOutputModel>>(query.ToList()), Count = count });
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet("count")]
    public ActionResult<int> Count(Guid? entityId)
    {
        IQueryable<Comment> query = modelContext.Comments.Include(c => c.Votes);
        if (entityId is not null)
        {
            query = query.Where(c => c.EntityId == entityId);
        }
        var count = query.Count();
        return Ok(count);
    }

    [HttpGet("new/count")]
    public async Task<ActionResult<int>> NewCount()
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out Guid userId
        ))
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            var lastReadTime = user?.LastReadCommentTime ?? DateTime.MinValue;
            IQueryable<Comment> query = modelContext.Comments.Where(c => c.PublicationDate > lastReadTime)
                .OrderBy(c => c.PublicationDate).Include(c => c.Votes);
            return Ok(query.Count());
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet("responses/count")]
    public async Task<ActionResult<int>> ResponsesCount()
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out Guid userId
        ))
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            var lastReadTime = user?.LastReadCommentTime ?? DateTime.MinValue;
            IQueryable<Comment> query = modelContext.Comments
            .Where(c => c.PublicationDate > lastReadTime && c.AuthorId != user.Id
            && modelContext.Comments.Where(com => com.AuthorId == user.Id && com.EntityId == c.EntityId).DefaultIfEmpty().Max(com => com.PublicationDate) < c.PublicationDate)
            .OrderBy(c => c.PublicationDate).Include(c => c.Votes);
            return Ok(query.Count());
        }
        else
        {
            return Unauthorized();
        }
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes)]
    [HttpPost("add")]
    public IActionResult Add(CommentInputModel commentInputModel)
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    out Guid userId
                ))
        {
            var comment = mapper.Map<Comment>(commentInputModel);
            if (comment.AuthorId != userId)
            {
                return BadRequest();
            }
            modelContext.Comments.Add(comment);
            modelContext.SaveChanges();
            return CreatedAtAction("Add", comment);
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet("last")]
    public ActionResult<CommentOutputModel> Last(Guid entityId)
    {
        var comment = modelContext.Comments.Where(c => c.EntityId == entityId).ToList().MaxBy(c => c.PublicationDate);
        if (comment is null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<CommentOutputModel>(comment));
    }

    [HttpGet("{id}")]
    public ActionResult<CommentOutputModel> Details(Guid id)
    {
        var comment = modelContext.Comments.FirstOrDefault(c => c.Id == id);
        if (comment is null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<CommentOutputModel>(comment));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes)]
    [HttpPost("{id}/vote")]
    public IActionResult Vote(Guid id, bool upvote)
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                            out Guid userId
                        ))
        {
            var comment = modelContext.Comments.FirstOrDefault(c => c.Id == id);
            if (comment is null)
            {
                return NotFound();
            }
            if (comment.AuthorId == userId || (DateTime.UtcNow - comment.PublicationDate).TotalDays > 30)
            {
                return BadRequest();
            }
            comment.Votes.Values ??= [];
            if (!comment.Votes.Values.Any(v => v.Author == userId))
            {
                comment.Votes.Values.Add(new Vote { Author = userId, VoteValue = upvote ? +1 : -1 });
            }
            else
            {
                comment.Votes.Values.RemoveAll(v => v.Author == userId);
            }
            modelContext.SaveChanges();
            return NoContent();
        }
        else
        {
            return Unauthorized();
        }
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor, Local Editor")]
    [HttpPost("{id}/record")]
    public async Task<IActionResult> Record(Guid id)
    {
        var comment = modelContext.Comments.FirstOrDefault(c => c.Id == id);
        if (comment is null)
        {
            return NotFound();
        }
        comment.IsRecorded = !comment.IsRecorded;
        modelContext.SaveChanges();
        return NoContent();
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes)]
    [HttpPut("{id}")]
    public IActionResult Update(Guid id, Comment comment)
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                            out Guid userId
                        ))
        {
            if (comment.AuthorId != userId)
            {
                return BadRequest("Cannot edit non-own comment");
            }
            var dbComment = modelContext.Comments.AsNoTracking().FirstOrDefault(c => c.Id == comment.Id);
            if (dbComment is null)
            {
                return NotFound("Comment not found in database");
            }
            if (dbComment.AuthorId != userId)
            {
                return BadRequest("Cannot edit non-own comment (from DB)");
            }
            var lastComment = modelContext.Comments.AsNoTracking()
                .Where(c => c.EntityId == comment.EntityId)
                .OrderByDescending(c => c.PublicationDate).First();
            if (lastComment.Id != dbComment.Id)
            {
                return BadRequest("Comment is not last for entity");
            }
            modelContext.Entry(comment).State = EntityState.Modified;
            modelContext.SaveChanges();
            return NoContent();
        }
        else
        {
            return Unauthorized();
        }
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes)]
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                            out Guid userId
                        ))
        {
            var comment = modelContext.Comments.FirstOrDefault(c => c.Id == id);
            if (comment is null)
            {
                return NotFound();
            }
            var lastComment = modelContext.Comments.Where(c => c.EntityId == comment.EntityId).OrderBy(c => c.PublicationDate).Last();
            if (comment.Id == lastComment.Id && comment.AuthorId == userId && (DateTime.UtcNow - comment.PublicationDate) <= 24.Hours())
            {
                modelContext.Comments.Remove(comment);
                modelContext.SaveChanges();
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
        else
        {
            return Unauthorized();
        }
    }
}

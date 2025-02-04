using System.Security.Claims;
using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Shared.Output;
using ArchiBase.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Short")]
public class UsersController(ModelContext modelContext,
                             UsersContext usersContext,
                             UserManager<ArchiBaseUser> userManager,
                             LocalEditorService localEditorService,
                             IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly UsersContext usersContext = usersContext;
    private readonly UserManager<ArchiBaseUser> userManager = userManager;
    private readonly LocalEditorService localEditorService = localEditorService;

    [HttpGet("")]
    public ActionResult<List<UserOutputModel>> Index()
    {
        var users = userManager.Users.ToList();
        return Ok(users.Select(u => mapper.Map<UserOutputModel>(u)).ToList());
    }

    [HttpGet("activecount")]
    public ActionResult<int> ActiveCount()
    {
        return Ok(usersContext.Users.Where(u => u.PasswordHash != null).Count());
    }

    [HttpGet("inrole")]
    public async Task<ActionResult<List<UserOutputModel>>> InRole(string roleName)
    {
        var users = await userManager.GetUsersInRoleAsync(roleName);
        return Ok(users.Select(u => mapper.Map<UserOutputModel>(u)));
    }

    [HttpPost("names")]
    public ActionResult<Dictionary<Guid, string>> Names(List<Guid> ids)
    {
        var names = userManager.Users.Where(u => ids.Contains(u.Id))
            .Select(u => new { u.Id, u.UserName, }).ToDictionary(x => x.Id, x => x.UserName ?? "(unnamed)");
        return Ok(names);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserOutputModel>> Details(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(mapper.Map<UserOutputModel>(user));
        }
    }

    [HttpGet("{id}/roles")]
    public async Task<ActionResult<List<string>>> Roles(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }
        else
        {
            List<string> roles = [.. (await userManager.GetRolesAsync(user))];
            return Ok(roles);
        }
    }

    [HttpGet("{id}/locations")]
    public async Task<ActionResult<List<Location>>> Locations(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }
        else
        {
            var locations = modelContext.Locations.Where(loc => user.Locations.Contains(loc.Id)).ToList();
            return Ok(locations);
        }
    }

    [HttpGet("{id}/mylocation")]
    public async Task<ActionResult<Location?>> MyLocation(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }
        else
        {
            var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == user.MyLocation);
            return Ok(location);
        }
    }

    [HttpGet("current")]
    public async Task<ActionResult<UserOutputModel>> CurrentUser()
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out Guid userId
        ))
        {
            return await Details(userId);
        }
        else
        {
            return Unauthorized();
        }
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes)]
    [HttpPost("readcomments")]
    public async Task<IActionResult> ReadComments(DateTime? toDate)
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    out Guid userId
                ))
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return NotFound();
            }
            user.LastReadCommentTime = toDate ?? DateTime.UtcNow;
            usersContext.SaveChanges();
            return NoContent();
        }
        else
        {
            return Unauthorized();
        }
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin")]
    [HttpPost("{id}/addrole")]
    public async Task<IActionResult> AddRole(Guid id, string roleName)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }
        else
        {
            var result = await userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin")]
    [HttpPost("{id}/removerole")]
    public async Task<IActionResult> RemoveRole(Guid id, string roleName)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }
        else
        {
            var result = await userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin")]
    [HttpPost("{id}/updatelocations")]
    public async Task<IActionResult> UpdateLocations(Guid id, List<Guid> locations)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }
        user.Locations = locations;
        usersContext.Entry(user).State = EntityState.Modified;
        usersContext.SaveChanges();
        return NoContent();
    }

    [HttpGet("{id}/canedit")]
    public async Task<ActionResult<bool>> CanEdit(Guid id, Guid locId)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }
        var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
        var isEditor = await userManager.IsInRoleAsync(user, "Editor");
        var isLocalEditor = await userManager.IsInRoleAsync(user, "LocalEditor") && localEditorService.CanEdit(user, locId);
        return Ok(isAdmin || isEditor || isLocalEditor);
    }
}

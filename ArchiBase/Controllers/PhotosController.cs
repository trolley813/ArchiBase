using System.Security.Claims;
using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Shared.Input;
using ArchiBase.Users;
using ArchiBase.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using ArchiBase.Shared.Output;
using AutoMapper;

namespace ArchiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Default")]
public class PhotosController(ModelContext modelContext,
    ThumbnailService thumbnailService,
    IWebHostEnvironment webHostEnvironment,
    UserResolverService userResolverService,
    StreetNameService streetNameService,
    UserManager<ArchiBaseUser> userManager,
    IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly ThumbnailService thumbnailService = thumbnailService;
    private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;
    private readonly UserResolverService userResolverService = userResolverService;
    private readonly StreetNameService streetNameService = streetNameService;
    private readonly UserManager<ArchiBaseUser> userManager = userManager;
    private readonly IMapper mapper = mapper;

    [HttpGet("count")]
    public ActionResult<int> Count(bool active)
    {
        var photos = active ? modelContext.ActivePhotos : modelContext.Photos;
        return Ok(photos.Count());
    }

    [HttpGet("recentcount")]
    public ActionResult<int> RecentCount(int days)
    {
        var count = modelContext.ActivePhotos.Where(p => (DateTime.UtcNow - p.PublicationDate).TotalDays <= days).Count();
        return Ok(count);
    }

    [HttpGet("recent")]
    public ActionResult<List<PhotoOutputModel>> Recent(int limit)
    {
        var photos = modelContext.ActivePhotos.OrderByDescending(p => p.PublicationDate).Take(limit).ToList();
        thumbnailService.GenerateThumbnails(photos.Select(p => p.Id));
        return Ok(mapper.Map<List<PhotoOutputModel>>(photos));
    }

    [HttpGet("random")]
    public ActionResult<List<Photo>> Random(int limit)
    {
        var photos = modelContext.ActivePhotos.OrderBy(p => EF.Functions.Random()).Take(limit).ToList();
        thumbnailService.GenerateThumbnails(photos.Select(p => p.Id));
        return Ok(mapper.Map<List<PhotoOutputModel>>(photos));
    }

    [HttpGet("extended")]
    public async Task<ActionResult<ExtendedPhotoListPage>> Extended(
        Guid? locId,
        Guid? authId,
        Guid? desId,
        string? filter,
        string? orderBy,
        int? limit,
        int? offset
    )
    {
        var rawQuery = modelContext.ActivePhotos
                .Include(p => p.BuildingBinds)
                .ThenInclude(b => b.Building)
                .ThenInclude(b => b.Location)
                .Include(p => p.BuildingBinds)
                .ThenInclude(b => b.Building)
                .ThenInclude(b => b.Cards)
                .ThenInclude(c => c.StreetAddresses)
                .ThenInclude(a => a.Street)
                .Include(p => p.BuildingBinds)
                .ThenInclude(b => b.Building)
                .ThenInclude(b => b.Cards)
                .ThenInclude(c => c.Parts)
                .ThenInclude(p => p.Design)
                .ThenInclude(d => d.Categories)
                .AsQueryable();

        if (locId is not null)
        {
            rawQuery = rawQuery
            .Join(modelContext.PhotoLocationMappings, p => p.Id, m => m.PhotoId, (p, m) => new { Photo = p, Mapping = m })
            .Where(x => x.Mapping.LocationId == locId)
            .Select(x => x.Photo);
        }

        if (desId is not null)
        {
            rawQuery = rawQuery.Where(p => p.BuildingBinds
            .SelectMany(b => b.Building.Cards)
            .Any(c => c.Parts.Any(p => p.DesignId == desId)));
        }

        var query = rawQuery.Join(modelContext.PhotoAuthorMappings, p => p.Id, a => a.PhotoId, (p, a) => new
        {
            Photo = p,
            ShootingDateSort = p.ShootingDate.Date,
            PublicationDateSort = p.PublicationDate,
            AuthorName = a.AuthorName
        })
        .AsQueryable();
        var selectedAuthor = userManager.Users.FirstOrDefault(u => u.Id == authId);
        if (selectedAuthor is not null)
        {
            query = query.Where(x => x.AuthorName == selectedAuthor.UserName);
        }

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(filter);
        }
        if (!string.IsNullOrEmpty(orderBy))
        {
            query = query.OrderBy(orderBy);
        }
        // Important!!! Make sure the Count property of RadzenDataGrid is set.
        var count = query.Count();
        var photos = query.Skip(offset ?? 0).Take(limit ?? 20).ToList()
        .Select(x => new PhotoWithAuthor
        {
            Photo = mapper.Map<PhotoOutputModel>(x.Photo),
            PublicationDateSort = x.PublicationDateSort,
            ShootingDateSort = x.ShootingDateSort,
            AuthorName = x.AuthorName
        }).ToList();
        thumbnailService.GenerateThumbnails(photos.Select(p => p.Photo.Id));
        var result = new ExtendedPhotoListPage { Count = count, Photos = photos };
        return Ok(result);
    }

    [HttpPost("thumbnails")]
    public ActionResult<Dictionary<Guid, string>> Thumbnails(List<Guid> ids)
    {
        var thumbnails = modelContext.Photos.Where(p => ids.Contains(p.Id))
            .ToDictionary(p => p.Id, p => $"data:image/webp;base64,{thumbnailService.GetThumbnail(p)}");
        return Ok(thumbnails);
    }

    [HttpPost("binds")]
    public ActionResult<Dictionary<Guid, List<Guid>>> Binds(List<Guid> ids)
    {
        var binds = modelContext.Photos.Where(p => ids.Contains(p.Id))
                   .Include(p => p.BuildingBinds)
                   .ThenInclude(b => b.Building)
                   .ToDictionary(p => p.Id, p => p.BuildingBinds.Select(b => b.Building.Id));
        return Ok(binds);
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Photo Moderator")]
    [HttpGet("queue")]
    public ActionResult<List<PhotoOutputModel>> Queue()
    {
        var queue = modelContext.Photos
            .Include(p => p.Votes)
            .Include(p => p.BuildingBinds)
            .ThenInclude(b => b.Building)
            .ThenInclude(b => b.Location)
            .Include(p => p.BuildingBinds)
            .ThenInclude(b => b.Building)
            .ThenInclude(b => b.Cards)
            .ThenInclude(c => c.StreetAddresses)
            .ThenInclude(a => a.Street)
            .Where(p => p.Status == PhotoStatus.Pending)
            .OrderByDescending(p => p.PublicationDate)
            .ToList();
        thumbnailService.GenerateThumbnails(queue.Select(p => p.Id));
        return Ok(mapper.Map<List<PhotoOutputModel>>(queue));
    }

    // TODO: Transfer to BuildingsController
    [HttpGet("{id}")]
    public ActionResult<PhotoOutputModel> Details(Guid id)
    {
        var photo = modelContext.Photos
            .Include(p => p.BuildingBinds).ThenInclude(b => b.Building).ThenInclude(b => b.Location)
            .Include(p => p.BuildingBinds).ThenInclude(b => b.Building).ThenInclude(b => b.Events)
            .Include(p => p.BuildingBinds).ThenInclude(b => b.Building).ThenInclude(b => b.Cards).ThenInclude(c =>
            c.StreetAddresses).ThenInclude(a => a.Street).ThenInclude(s => s.ActualStreet)
            .Include(p => p.BuildingBinds).ThenInclude(b => b.Building).ThenInclude(b => b.Cards).ThenInclude(c =>
            c.Parts).ThenInclude(p => p.Design).ThenInclude(d => d.Categories)
            .Include(p => p.BuildingBinds).ThenInclude(b => b.Building).ThenInclude(b => b.Cards).ThenInclude(c =>
            c.Categories)
            .Include(p => p.License)
            .Include(p => p.Galleries)
            .AsSplitQuery()
            .FirstOrDefault(p => p.Id == id);
        if (photo is null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<PhotoOutputModel>(photo));
    }

    [HttpGet("{id}/thumbnail")]
    public ActionResult<string> Thumbnail(Guid id)
    {
        var photo = modelContext.Photos.FirstOrDefault(p => p.Id == id);
        if (photo is null)
        {
            return NotFound();
        }
        return Ok(thumbnailService.GetThumbnail(photo));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes)]
    [HttpPost("{id}/vote")]
    public IActionResult Vote(Guid id, bool upvote)
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                            out Guid userId
                        ))
        {
            var photo = modelContext.Photos.FirstOrDefault(c => c.Id == id);
            if (photo is null)
            {
                return NotFound();
            }
            if (photo.AuthorId == userId || (DateTime.UtcNow - photo.PublicationDate).TotalDays > 30)
            {
                return BadRequest();
            }
            photo.Votes.Values ??= [];
            if (!photo.Votes.Values.Any(v => v.Author == userId))
            {
                photo.Votes.Values.Add(new Vote { Author = userId, VoteValue = upvote ? +1 : -1 });
            }
            else
            {
                photo.Votes.Values.RemoveAll(v => v.Author == userId);
            }
            modelContext.SaveChanges();
            return NoContent();
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet("{id}/buildingaddresses")]
    public ActionResult<Dictionary<Guid, string>> BuildingAddresses(Guid id)
    {
        var photo = modelContext.Photos
            .Include(p => p.BuildingBinds)
            .ThenInclude(b => b.Building)
            .ThenInclude(b => b.Cards)
            .ThenInclude(c => c.StreetAddresses)
            .ThenInclude(a => a.Street)
            .FirstOrDefault(p => p.Id == id);
        if (photo is null)
        {
            return NotFound();
        }
        var buildingAddresses = photo.BuildingBinds.ToDictionary(
            b => b.Building.Id,
            b => String.Join(" / ", b.Building.ActualToDate(photo.ShootingDate.Date)?.StreetAddresses.Select(
                a => $"{streetNameService.GetStreetNameForDate(a.Street, photo.ShootingDate)?.Name}, {a.HouseNumber}") ?? []
            )
        );
        return Ok(buildingAddresses);
    }

    [HttpGet("{id}/canbevoted")]
    public async Task<ActionResult<bool>> CanBeVoted(Guid id)
    {
        if (Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                            out Guid userId
                        ))
        {
            var photo = modelContext.Photos.FirstOrDefault(c => c.Id == id);
            if (photo is null)
            {
                return NotFound();
            }
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return NotFound();
            }
            var result = photo?.Status switch
            {
                PhotoStatus.Rejected => false,
                PhotoStatus.Published => modelContext.ActivePhotos.Where(p => p.AuthorId == user.Id).Count() >= 10,
                PhotoStatus.Pending =>
                (await userManager.GetRolesAsync(user))
                .Any(r => r == "Admin" || r == "Photo Moderator"),
                _ => false
            };
            return Ok(result);
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet("{id}/votes")]
    public ActionResult<Dictionary<string, int>> Votes(Guid id)
    {
        var photo = modelContext.Photos.FirstOrDefault(c => c.Id == id);
        if (photo is null)
        {
            return NotFound();
        }
        var authorIds = photo.Votes?.Values?.Select(v => v.Author) ?? [];
        var votes = userManager.Users
            .Where(u => authorIds.Contains(u.Id))
            .ToList()
            .ToDictionary(
                u => u.UserName ?? "unknown",
                u => photo.Votes?.Values?.FirstOrDefault(v => v.Author == u.Id)?.VoteValue ?? 0
            );
        return Ok(votes);
    }

    [HttpGet("{id}/suggestedlocation")]
    public ActionResult<LocationOutputModel> SuggestedLocation(Guid id)
    {
        var photo = modelContext.Photos.FirstOrDefault(c => c.Id == id);
        if (photo is null)
        {
            return NotFound();
        }
        var location = modelContext.Buildings
            .Include(b => b.Location)
            .OrderBy(b => (photo.Latitude - b.Latitude) * (photo.Latitude - b.Latitude) + (photo.Longitude - b.Longitude) * (photo.Longitude - b.Longitude))
            .First().Location;
        return Ok(mapper.Map<LocationOutputModel>(location));
    }

    [HttpPost("upload")]
    public IActionResult Upload(UploadModel model)
    {
        var license = modelContext.Licenses.FirstOrDefault(lic => lic.Id == model.LicenseId);
        if (license is null)
        {
            return BadRequest("License is not specified");
        }
        var p = model.File[(model.File.IndexOf(',') + 1)..];
        byte[] bytes = Convert.FromBase64String(p);
        Guid photoId = Guid.NewGuid();
        var authorId = userResolverService.GetUser();
        Photo photo = new()
        {
            Id = photoId,
            AuthorId = authorId,
            Exif = model.ExifData,
            NonAuthor = model.NonAuthor,
            License = license,
            PublicationDate = DateTime.UtcNow,
            ShootingDate = model.ShootingDate,
            Direction = model.Direction,
            Extension = Path.GetExtension(model.FileName)?.TrimStart('.') ?? "jpg",
            BuildingBinds = model.Binds.Select((b, idx) => new BuildingBind
            {
                Building = modelContext.Buildings.FirstOrDefault(bg => bg.Id == b.BuildingId),
                IsMain = b.IsMain,
                Order = idx + 1
            }).ToList(),
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            Description = model.Description,
            Status = model.UseDirectUpload ? PhotoStatus.Published : PhotoStatus.Pending
        };
        modelContext.Photos.Add(photo);
        var wwwroot = webHostEnvironment.WebRootPath;
        System.IO.Directory.CreateDirectory(wwwroot + photo.PhotoDir);
        System.IO.File.WriteAllBytes(wwwroot + photo.PhotoLink, bytes);
        modelContext.SaveChanges();
        return CreatedAtAction("Details", new { photo.Id }, photo);
    }
}

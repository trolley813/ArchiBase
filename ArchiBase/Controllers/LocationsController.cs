using ArchiBase.Components.Utils;
using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Shared.Input;
using ArchiBase.Shared.Output;
using ArchiBase.Users;
using AutoMapper;
using EFMaterializedPath;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;

namespace ArchiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Default")]
public class LocationsController(
    ModelContext modelContext,
    UsersContext usersContext,
    ITreeRepository<Location, Guid> treeRepository,
    IMapper mapper,
    IAuthorizationService authorizationService) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly UsersContext usersContext = usersContext;
    private readonly ITreeRepository<Location, Guid> treeRepository = treeRepository;
    private readonly IMapper mapper = mapper;
    private readonly IAuthorizationService authorizationService = authorizationService;

    [HttpGet("")]
    public ActionResult<List<LocationOutputModel>> Index(bool? allowStreets)
    {
        IQueryable<Location> locations = modelContext.Locations;
        if (allowStreets is not null)
        {
            locations = locations.Where(x => x.AllowStreets == allowStreets);
        }
        return Ok(mapper.Map<List<LocationOutputModel>>(locations.ToList()));
    }

    [HttpGet("basic")]
    public ActionResult<List<LocationBasicOutputModel>> Basic()
    {
        var names = modelContext.Locations;
        return Ok(mapper.Map<List<LocationBasicOutputModel>>(names));
    }

    [HttpPost("names")]
    public ActionResult<Dictionary<Guid, string>> Names(List<Guid> ids)
    {
        var names = modelContext.Locations.Where(loc => ids.Contains(loc.Id)).ToDictionary(loc => loc.Id, loc => loc.Name);
        return Ok(names);
    }

    [HttpGet("count")]
    public ActionResult<int> Count()
    {
        return Ok(modelContext.Locations.Count());
    }

    [HttpGet("roots")]
    public ActionResult<List<LocationOutputModel>> Roots()
    {
        var roots = treeRepository.QueryRoots();
        return Ok(mapper.Map<List<LocationOutputModel>>(roots));
    }

    [HttpGet("{id}")]
    public ActionResult<LocationOutputModel> Details(Guid id)
    {
        var location = modelContext.Locations
            .Include(loc => loc.Streets)
            .ThenInclude(s => s.ActualStreet)
            .FirstOrDefault(loc => loc.Id == id);
        if (location is not null)
        {
            return Ok(mapper.Map<LocationOutputModel>(location));
        }
        else
        {
            return NotFound();
        }
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor, Local Editor")]
    [HttpPost("create")]
    public async Task<ActionResult<LocationOutputModel>> Create(LocationInputModel locationInputModel, [FromQuery] Guid? parentId)
    {
        var canEditResult = await authorizationService.AuthorizeAsync(User, $"LocalEditor-{parentId}");
        if (!canEditResult.Succeeded)
        {
            return Forbid();
        }
        var location = mapper.Map<Location>(locationInputModel);
        modelContext.Locations.Add(location);
        if (parentId is not null)
        {
            var parent = modelContext.Locations.FirstOrDefault(loc => loc.Id == parentId);
            if (parent is null)
            {
                return BadRequest("Parent location not found");
            }
            await treeRepository.SetParentAsync(location, parent);
        }
        modelContext.SaveChanges();
        return CreatedAtAction("Details", new { location.Id }, mapper.Map<LocationOutputModel>(location));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor, Local Editor")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(Guid id, Guid? parentId, LocationInputModel locationInputModel)
    {
        var canEditResult = await authorizationService.AuthorizeAsync(User, $"LocalEditor-{locationInputModel.Id}");
        if (!canEditResult.Succeeded)
        {
            return Forbid();
        }
        var location = mapper.Map<Location>(locationInputModel);
        if (location.Id != id)
        {
            return BadRequest();
        }
        if (modelContext.Locations.AsNoTracking().FirstOrDefault(loc => loc.Id == id) is null)
        {
            return NotFound();
        }
        if (parentId is not null)
        {
            var parentLocation = modelContext.Locations.FirstOrDefault(loc => loc.Id == parentId);
            if (parentLocation is not null)
            {
                treeRepository.SetParentAsync(location, parentLocation);
            }
        }
        modelContext.Entry(location).State = EntityState.Modified;
        modelContext.SaveChanges();
        return NoContent();
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor")]
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        if (location is null)
        {
            return NotFound();
        }
        var childrenCount = treeRepository.QueryChildren(location).Count();
        if (childrenCount > 0)
        {
            return BadRequest("Cannot delete location which has sublocations");
        }
        else
        {
            modelContext.Locations.Remove(location);
            modelContext.SaveChanges();
            return NoContent();
        }
    }

    [HttpGet("{id}/sublocations")]
    public ActionResult<List<LocationOutputModel>> Sublocations(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        var children = treeRepository.QueryChildren(location);
        return Ok(mapper.Map<List<LocationOutputModel>>(children));
    }

    [HttpGet("{id}/sublocationtotalcount")]
    public ActionResult<int> SublocationsTotalCount(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        return Ok(treeRepository.QueryDescendants(location).Count());
    }

    [HttpGet("{id}/path")]
    public async Task<ActionResult<List<LocationOutputModel>>> Path(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        if (location is null)
        {
            return NotFound();
        }
        var path = await treeRepository.GetPathFromRootAsync(location);
        return Ok(mapper.Map<List<LocationOutputModel>>(path));
    }

    [HttpGet("{id}/editors")]
    public async Task<ActionResult<List<UserOutputModel>>> LocalEditors(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        if (location is null)
        {
            return NotFound();
        }
        var path = await treeRepository.GetPathFromRootAsync(location);
        var editors = usersContext.Users
            .Where(u => u.Locations.Any(locid => locid == id || path.Select(loc => loc.Id).Contains(locid)));
        return Ok(mapper.Map<List<UserOutputModel>>(editors));
    }

    [HttpGet("{id}/recentphotos")]
    public ActionResult<List<PhotoOutputModel>> RecentPhotos(Guid id, int limit)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        if (location is null)
        {
            return NotFound();
        }
        var photos = modelContext.ActivePhotos.Include(p => p.BuildingBinds).ThenInclude(b => b.Building)
            .Where(p => p.BuildingBinds.Any(b => b.Building.Location == location))
            .OrderByDescending(p => p.PublicationDate).Take(limit);
        return Ok(mapper.Map<PhotoOutputModel>(photos));
    }

    [HttpGet("{id}/galleries")]
    public ActionResult<List<GalleryOutputModel>> Galleries(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        if (location is null)
        {
            return NotFound();
        }
        var galleries = modelContext.Galleries
            .Where(g => g.EntityId == location.Id && g.EntityType == "Location")
            .OrderBy(g => g.Name).ToList();
        return Ok(mapper.Map<GalleryOutputModel>(galleries));
    }

    [HttpGet("{id}/designstats")]
    public ActionResult<List<(DesignOutputModel, int)>> DesignStats(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        var byDesign = modelContext.Set<BuildingPart>()
            .Include(p => p.Design)
            .ThenInclude(d => d.Categories)
            .Include(p => p.BuildingCard)
            .ThenInclude(c => c.Building)
            .ThenInclude(b => b.Location)
            .Where(p => p.BuildingCard.Building.Location == location)
            .AsEnumerable()
            .GroupBy(p => p.Design, (d, ps) => new { Design = d, Count = ps.DistinctBy(p => p.BuildingCard.Building).Count() })
            .AsEnumerable()
            .Select(x => (mapper.Map<DesignOutputModel>(x.Design), x.Count));
        return Ok(byDesign);
    }

    [HttpGet("{id}/categorystats")]
    public ActionResult<List<(DesignCategoryOutputModel, int)>> CategoryStats(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        var byCategory = modelContext.Buildings
            .Include(b => b.ActualCard)
            .ThenInclude(c => c.Categories)
            .Where(b => b.Location == location)
            .SelectMany(b => b.ActualCard.Categories.Select(c => new { BuildingId = b.Id, Category = c }))
            .GroupBy(x => x.Category, (c, bs) => new { Category = c, Count = bs.Count() })
            .AsEnumerable()
            .Select(x => (mapper.Map<DesignCategoryOutputModel>(x.Category), x.Count))
            .ToList();
        return Ok(byCategory);
    }

    [HttpGet("{id}/buildings")]
    public ActionResult<List<BuildingOutputModel>> Buildings(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        var buildings = modelContext.Buildings
            .Include(b => b.Events)
            .Include(b => b.ActualCard)
            .ThenInclude(c => c.StreetAddresses)
            .ThenInclude(a => a.Street)
            .Where(b => b.Location == location);
        return Ok(mapper.Map<List<BuildingOutputModel>>(buildings));
    }

    [HttpGet("{id}/buildingmarkers")]
    public ActionResult<List<BuildingMarker>> BuildingMarkers(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        var markers = modelContext.Buildings
            .Include(b => b.Events)
            .Include(b => b.ActualCard)
            .ThenInclude(c => c.StreetAddresses)
            .ThenInclude(a => a.Street)
            .Where(b => b.Location == location)
            .ToList()
            .Select(b => new BuildingMarker(mapper.Map<BuildingOutputModel>(b)) { })
            .ToList();
        return Ok(markers);
    }

    [HttpGet("{id}/buildingswithphotos")]
    public ActionResult<HashSet<Guid>> BuildingBinds(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        var buildingIds = modelContext.Buildings
            .AsNoTracking()
            .Where(b => b.Location == location)
            .Select(b => b.Id);
        var idsWithPhotos = modelContext.BuildingBinds
            .AsNoTracking()
            .Where(b => buildingIds.Contains(b.Building.Id))
            .Select(b => b.Building.Id)
            .ToHashSet();
        return Ok(idsWithPhotos);
    }

    [HttpGet("{id}/streets")]
    public ActionResult<List<StreetOutputModel>> Streets(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        var streets = modelContext.Streets.Include(s => s.Location).Where(s => s.Location == location).ToList();
        return Ok(mapper.Map<List<StreetOutputModel>>(streets));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor, Local Editor")]
    [HttpPost("{id}/addstreet")]
    public async Task<IActionResult> AddStreetAsync(Guid id, StreetWithBuildings streetWithBuildings)
    {
        var canEditResult = await authorizationService.AuthorizeAsync(User, $"LocalEditor-{id}");
        if (!canEditResult.Succeeded)
        {
            return Forbid();
        }
        var street = mapper.Map<Street>(streetWithBuildings.Street);
        var buildings = streetWithBuildings.Buildings;
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        if (location is null)
        {
            return NotFound();
        }
        if (street.Location.Id != location.Id)
        {
            return BadRequest();
        }
        modelContext.Streets.Add(street);
        foreach (var buildingEntry in buildings)
        {
            List<BuildingEvent> events = [];
            if (buildingEntry.ConstructionYear is int cYear) events.Add(new BuildingEvent
            {
                Date = new() { Date = new DateTime(cYear, 1, 1), Precision = DatePrecision.YearOnly },
                Type = BuildingEventType.ConstructionFinished
            });
            if (buildingEntry.DemolitionYear is int dYear) events.Add(new BuildingEvent
            {
                Date = new() { Date = new DateTime(dYear, 1, 1), Precision = DatePrecision.YearOnly },
                Type = BuildingEventType.Demolished
            });
            BuildingCard card = new()
            {
                ActualFrom = events.FirstOrDefault(e => e.Type == BuildingEventType.ConstructionFinished)?.Date.Clone() ?? new(),
                Name = street.Name,
                Parts = buildingEntry.Design is not null ? [new BuildingPart() { Design = mapper.Map<Design>(buildingEntry.Design) }] : [],
                Categories = mapper.Map<List<DesignCategory>>(buildingEntry.Categories),
                StreetAddresses = [new StreetAddress { Street = street, HouseNumber = buildingEntry.Number }],
                Builder = "",
                Customer = "",
            };
            Building building = new()
            {
                Cards = [card],
                Events = events,
                Latitude = buildingEntry.Latitude ?? 0.0,
                Longitude = buildingEntry.Longitude ?? 0.0,
                Description = String.Empty,
                Location = location
            };
            modelContext.Add(building);
        }
        modelContext.SaveChanges();
        return CreatedAtAction("AddStreet", mapper.Map<StreetOutputModel>(street));
    }

    [HttpGet("{id}/buildingswithaddresses")]
    public async Task<ActionResult<List<BuildingWithAddress>>> BuildingsWithAddresses(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        if (location is null)
        {
            return NotFound();
        }
        var buildings = modelContext.Buildings
            .Include(b => b.Location)
            .Where(b => b.Location == location)
            .AsNoTracking()
            .Include(b => b.ActualCard)
            .ThenInclude(c => c.Categories)
            .Include(b => b.ActualCard)
            .ThenInclude(c => c.StreetAddresses)
            .ThenInclude(a => a.Street)
            .Include(b => b.ActualCard)
            .ThenInclude(c => c.Parts)
            .ThenInclude(p => p.Design)
            .Join(modelContext.CardAddressMappings, b => b.ActualCard.Id, m => m.BuildingCardId,
                (b, m) => new BuildingWithAddress
                {
                    Building = mapper.Map<BuildingOutputModel>(b),
                    Addresses = m.Addresses
                }
            ).ToList();
        return Ok(buildings);
    }

    [HttpGet("{id}/flags")]
    public async Task<ActionResult<List<string>>> Flags(Guid id)
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == id);
        if (location is null)
        {
            return NotFound();
        }
        var flags = (await treeRepository.GetPathFromRootAsync(location))
                .Select(loc => loc.Flag).Where(f => f is not null && f != "" && f != "xx").ToList();
        if (location.Flag is string flag && flag != "" && flag != "xx")
        {
            flags.Add(flag);
        }
        return Ok(flags);
    }
}

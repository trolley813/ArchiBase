using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Shared.Input;
using ArchiBase.Shared.Output;
using ArchiBase.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Default")]
public class DesignsController(ModelContext modelContext, IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly IMapper mapper = mapper;

    [HttpGet("")]
    public ActionResult<List<DesignOutputModel>> Index()
    {
        var designs = modelContext.Designs;
        return Ok(mapper.Map<List<DesignOutputModel>>(designs));
    }

    [HttpGet("basic")]
    public ActionResult<List<DesignBasicOutputModel>> Basic()
    {
        var designs = modelContext.Designs;
        return Ok(mapper.Map<List<DesignBasicOutputModel>>(designs));
    }

    [HttpGet("count")]
    public ActionResult<int> Count()
    {
        return Ok(modelContext.Designs.Count());
    }

    [HttpPost("names")]
    public ActionResult<Dictionary<Guid, string>> Names(List<Guid> ids)
    {
        var names = modelContext.Designs.Where(d => ids.Contains(d.Id))
            .Select(d => new { d.Id, d.Name }).ToDictionary(p => p.Id, p => p.Name);
        return Ok(names);
    }

    [HttpGet("search")]
    public ActionResult<List<DesignOutputModel>> Search(string searchTerms)
    {
        var searchResults = modelContext.Designs
            .Include(d => d.CatalogueEntries)
            .ThenInclude(e => e.Catalogue).ToList()
            .Where(d => d.Name.Contains(searchTerms, StringComparison.OrdinalIgnoreCase) || d.CatalogueEntries.Any(e =>
            e.Code.Contains(searchTerms, StringComparison.OrdinalIgnoreCase)));
        return Ok(mapper.Map<List<DesignOutputModel>>(searchResults.ToList()));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor")]
    [HttpPost("create")]
    public ActionResult<Design> Create(DesignInputModel designInputModel)
    {
        var design = mapper.Map<Design>(designInputModel);
        modelContext.Designs.Add(design);
        modelContext.SaveChanges();
        return CreatedAtAction("Details", new { design.Id }, design);
    }

    [HttpGet("{id}")]
    public ActionResult<DesignOutputModel> Details(Guid id)
    {
        var design = modelContext.Designs
            .Include(c => c.Categories)
            .Include(c => c.CatalogueEntries)
            .ThenInclude(e => e.Catalogue)
            .FirstOrDefault(c => c.Id == id);
        if (design is null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<DesignOutputModel>(design));
    }

    [HttpGet("{id}/buildingsbylocation")]
    public ActionResult<Dictionary<Guid, int>> BuildingsByLocation(Guid id)
    {
        var design = modelContext.Designs
            .Include(c => c.Categories)
            .Include(c => c.CatalogueEntries)
            .ThenInclude(e => e.Catalogue)
            .FirstOrDefault(c => c.Id == id);
        if (design is null)
        {
            return NotFound();
        }
        var buildingsByLocation = modelContext.BuildingCards
            .Include(c => c.Parts)
            .Include(c => c.Building)
            .Where(c => c.Parts.Any(p => p.DesignId == design.Id) && c.Building.Location != null)
            .Select(c => c.Building)
            .GroupBy(b => b.LocationId)
            .ToList()
            .ToDictionary(g => g.Key, g => g.Count());
        return Ok(buildingsByLocation);
    }

    [HttpGet("{id}/buildings")]
    public ActionResult<List<BuildingInfoOutputModel>> Buildings(Guid id)
    {
        var design = modelContext.Designs
            .Include(c => c.Categories)
            .Include(c => c.CatalogueEntries)
            .ThenInclude(e => e.Catalogue)
            .FirstOrDefault(c => c.Id == id);
        if (design is null)
        {
            return NotFound();
        }
        var buildingsIds = modelContext.BuildingCards
            .Include(c => c.Parts)
            .Where(c => c.Parts.Any(p => p.DesignId == design.Id))
            .Select(c => c.BuildingId)
            .ToList();
        var buildings = modelContext.Buildings.Where(b => buildingsIds.Contains(b.Id))
            .Include(b => b.Events)
            .Include(b => b.Location)
            .Include(b => b.ActualCard)
            .ThenInclude(c => c.StreetAddresses)
            .ThenInclude(a => a.Street)
            .Include(b => b.ActualCard)
            .ThenInclude(c => c.Categories)
            .Include(b => b.ActualCard)
            .ThenInclude(c => c.Parts)
            .ThenInclude(p => p.Design)
            .ThenInclude(d => d.Categories)
            .OrderBy(b => b.Location.Name)
            .ToList()
            .Select(
            b => new BuildingInfoOutputModel
            {
                Building = mapper.Map<BuildingOutputModel>(b),
                HouseNumber = b.ActualCard.StreetAddresses.Count > 0 ? b.ActualCard?.StreetAddresses.First().HouseNumber : "000",
                FloorCount = String.Join("; ", b.ActualCard?.FloorCount ?? []),
                Name = b.ActualCard?.Name ?? "",
                Built = b.GetDateOfStatus(BuildingEventType.ConstructionFinished),
                Demolished = b.GetDateOfStatus(BuildingEventType.Demolished),
                Addresses = mapper.Map<List<StreetAddressOutputModel>>(b.ActualCard?.StreetAddresses ?? []),
                ActualCard = mapper.Map<BuildingCardOutputModel>(b.ActualCard),
                LastCardWithAddress = null
            }
            ).ToList();
        return Ok(buildings);
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor")]
    [HttpPut("{id}")]
    public ActionResult Update(Guid id, DesignInputModel designInputModel)
    {
        var design = mapper.Map<Design>(designInputModel);
        if (design.Id != id)
        {
            return BadRequest();
        }
        if (modelContext.Designs.AsNoTracking().FirstOrDefault(des => des.Id == id) is null)
        {
            return NotFound();
        }
        modelContext.Entry(design).State = EntityState.Modified;
        modelContext.SaveChanges();
        return NoContent();
    }

}

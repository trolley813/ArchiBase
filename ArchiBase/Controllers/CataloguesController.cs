using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Shared.Input;
using ArchiBase.Shared.Output;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Default")]
public class CataloguesController(ModelContext modelContext, IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly IMapper mapper = mapper;

    [HttpGet("")]
    public ActionResult<List<DesignCatalogueOutputModel>> Index()
    {
        var catalogues = modelContext.DesignCatalogues;
        return Ok(mapper.Map<List<DesignCatalogueOutputModel>>(catalogues));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor")]
    [HttpPost("create")]
    public ActionResult<DesignCategoryOutputModel> Create(DesignCatalogueInputModel catalogueInputModel)
    {
        var catalogue = mapper.Map<DesignCatalogue>(catalogueInputModel);
        modelContext.DesignCatalogues.Add(catalogue);
        modelContext.SaveChanges();
        return CreatedAtAction("Details", new { catalogue.Id }, mapper.Map<DesignCategoryOutputModel>(catalogue));
    }

    [HttpGet("{id}")]
    public ActionResult<DesignCatalogueOutputModel> Details(Guid id)
    {
        var catalogue = modelContext.DesignCatalogues
            .Include(c => c.Entries)
            .ThenInclude(e => e.Design)
            .FirstOrDefault(c => c.Id == id);
        if (catalogue is null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<DesignCatalogueOutputModel>(catalogue));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor")]
    [HttpPut("{id}")]
    public ActionResult Update(Guid id, DesignCatalogueInputModel catalogueInputModel)
    {
        var catalogue = mapper.Map<DesignCatalogue>(catalogueInputModel);
        if (catalogue.Id != id)
        {
            return BadRequest();
        }
        if (modelContext.DesignCatalogues.AsNoTracking().FirstOrDefault(cat => cat.Id == id) is null)
        {
            return NotFound();
        }
        modelContext.Entry(catalogue).State = EntityState.Modified;
        modelContext.SaveChanges();
        return NoContent();
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor")]
    [HttpDelete("{id}")]
    public ActionResult Delete(Guid id)
    {
        var catalogue = modelContext.DesignCatalogues.FirstOrDefault(cat => cat.Id == id);
        if (catalogue is null)
        {
            return NotFound();
        }
        modelContext.DesignCatalogues.Remove(catalogue);
        modelContext.SaveChanges();
        return NoContent();
    }

    [HttpGet("{id}/otherentries")]
    public ActionResult<List<DesignCatalogueEntryOutputModel>> OtherEntries(Guid id, Guid entryId)
    {
        var catalogue = modelContext.DesignCatalogues.FirstOrDefault(cat => cat.Id == id);
        if (catalogue is null)
        {
            return NotFound();
        }
        var entry = modelContext.DesignCatalogueEntries.FirstOrDefault(e => e.Id == id);
        if (entry is null)
        {
            return NotFound();
        }
        if (entry.Catalogue != catalogue)
        {
            return BadRequest();
        }
        var otherEntries = modelContext.DesignCatalogueEntries.Include(e => e.Catalogue).Where(e =>
                        e.Design == entry.Design && e.Catalogue != entry.Catalogue).ToList();
        return Ok(mapper.Map<List<DesignCatalogueEntryOutputModel>>(otherEntries));
    }

    [HttpGet("{id}/allotherentries")]
    public ActionResult<Dictionary<Guid, List<DesignCatalogueEntryOutputModel>>> AllOtherEntries(Guid id)
    {
        var catalogue = modelContext.DesignCatalogues.FirstOrDefault(cat => cat.Id == id);
        if (catalogue is null)
        {
            return NotFound();
        }
        var designIds = modelContext.DesignCatalogueEntries
            .Include(e => e.Catalogue)
            .Include(e => e.Design)
            .Where(e => e.Catalogue == catalogue)
            .Select(e => e.Design.Id)
            .Distinct()
            .ToList();

        var result = modelContext.DesignCatalogueEntries
            .Include(e => e.Catalogue)
            .Include(e => e.Design)
            .Where(e => e.Catalogue != catalogue && e.Design != null && designIds.Contains(e.Design.Id))
            .GroupBy(e => e.Design)
            .ToList()
            .ToDictionary(g => g.Key.Id, g => mapper.Map<List<DesignCatalogueEntryOutputModel>>(g.ToList())) ?? [];

        return Ok(result);
    }
}

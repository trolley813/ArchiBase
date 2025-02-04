using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Shared.Input;
using ArchiBase.Shared.Output;
using AutoMapper;
using EFMaterializedPath;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Default")]
public class CategoriesController(
    ModelContext modelContext,
    ITreeRepository<DesignCategory, Guid> treeRepository,
    IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly ITreeRepository<DesignCategory, Guid> treeRepository = treeRepository;
    private readonly IMapper mapper = mapper;

    [HttpGet("")]
    public ActionResult<List<DesignCategoryOutputModel>> Index()
    {
        var categories = modelContext.DesignCategories;
        return Ok(mapper.Map<List<DesignCategoryOutputModel>>(categories));
    }

    [HttpGet("basic")]
    public ActionResult<List<DesignCategoryBasicOutputModel>> Basic()
    {
        var categories = modelContext.DesignCategories;
        return Ok(mapper.Map<List<DesignCategoryBasicOutputModel>>(categories));
    }

    [HttpGet("roots")]
    public ActionResult<List<DesignCategoryOutputModel>> Roots()
    {
        var roots = treeRepository.QueryRoots();
        return Ok(mapper.Map<List<DesignCategoryOutputModel>>(roots));
    }

    [HttpPost("names")]
    public ActionResult<Dictionary<Guid, string>> Names(List<Guid> ids)
    {
        var names = modelContext.DesignCategories.Where(c => ids.Contains(c.Id))
            .Select(d => new { d.Id, d.Name }).ToDictionary(p => p.Id, p => p.Name); ;
        return Ok(names);
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor")]
    [HttpPost("create")]
    public async Task<ActionResult<DesignCategoryOutputModel>> Create(DesignCategoryInputModel categoryInputModel, [FromQuery] Guid? parentId)
    {
        var category = mapper.Map<DesignCategory>(categoryInputModel);
        modelContext.DesignCategories.Add(category);
        if (parentId is not null)
        {
            var parent = modelContext.DesignCategories.FirstOrDefault(loc => loc.Id == parentId);
            if (parent is null)
            {
                return BadRequest("Parent location not found");
            }
            await treeRepository.SetParentAsync(category, parent);
        }
        modelContext.SaveChanges();
        return CreatedAtAction("Details", new { category.Id }, mapper.Map<DesignCategoryOutputModel>(category));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DesignCategoryOutputModel>> Details(Guid id)
    {
        var category = await modelContext.DesignCategories
            .Include(c => c.Designs)
            .Include(c => c.BuildingCards)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<DesignCategoryOutputModel>(category));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor")]
    [HttpPut("{id}")]
    public ActionResult Update(Guid id, Guid? parentId, DesignCategoryInputModel categoryInputModel)
    {
        var category = mapper.Map<DesignCategory>(categoryInputModel);
        if (category.Id != id)
        {
            return BadRequest();
        }
        if (modelContext.DesignCategories.AsNoTracking().FirstOrDefault(loc => loc.Id == id) is null)
        {
            return NotFound();
        }
        if (parentId is not null)
        {
            var parentCategory = modelContext.DesignCategories.FirstOrDefault(loc => loc.Id == parentId);
            if (parentCategory is not null)
            {
                treeRepository.SetParentAsync(category, parentCategory);
            }
        }
        modelContext.Entry(category).State = EntityState.Modified;
        modelContext.SaveChanges();
        return NoContent();
    }

    [HttpGet("{id}/subcategories")]
    public async Task<ActionResult<List<DesignCategoryOutputModel>>> Subcategories(Guid id)
    {
        var category = await modelContext.DesignCategories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
        {
            return NotFound();
        }
        var subcategories = treeRepository.QueryChildren(category);
        return Ok(mapper.Map<List<DesignCategoryOutputModel>>(subcategories.ToList()));
    }

    [HttpGet("{id}/path")]
    public async Task<ActionResult<List<DesignCategoryOutputModel>>> Path(Guid id)
    {
        var category = modelContext.DesignCategories.FirstOrDefault(cat => cat.Id == id);
        if (category is null)
        {
            return NotFound();
        }
        var path = await treeRepository.GetPathFromRootAsync(category);
        return Ok(mapper.Map<List<DesignCategoryOutputModel>>(path));
    }

    [HttpGet("{id}/galleries")]
    public ActionResult<List<Gallery>> Galleries(Guid id)
    {
        var category = modelContext.DesignCategories.FirstOrDefault(cat => cat.Id == id);
        if (category is null)
        {
            return NotFound();
        }
        var galleries = modelContext.Galleries
            .Where(g => g.EntityId == category.Id && g.EntityType == "DesignCategory")
            .OrderBy(g => g.Name).ToList();
        return Ok(mapper.Map<List<Gallery>>(galleries));
    }

    [HttpGet("{id}/designcodes")]
    public ActionResult<Dictionary<Guid, List<string>>> DesignCodes(Guid id)
    {
        var category = modelContext.DesignCategories
            .Include(c => c.Designs)
            .FirstOrDefault(cat => cat.Id == id);
        if (category is null)
        {
            return NotFound();
        }
        var codes = modelContext.DesignCatalogueEntries
            .Include(e => e.Catalogue)
            .Where(e => e.DesignId != null && category.Designs.Select(d => d.Id).ToList().Contains(e.DesignId.Value))
            .GroupBy(e => e.DesignId.Value)
            .ToList()
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(e => e.Catalogue.Id).OrderBy(e => e.Code).Select(e => e.Formatted).ToList()
            );
        return Ok(codes);
    }
}

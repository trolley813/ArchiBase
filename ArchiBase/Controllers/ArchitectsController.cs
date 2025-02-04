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
public class ArchitectsController(ModelContext modelContext, IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;

    private readonly IMapper mapper = mapper;

    /// <summary>
    /// Gets the list of all architects.
    /// </summary>
    /// <returns>List of all architects.</returns>
    /// <response code="200">Always returns 200 OK.</response>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<List<ArchitectOutputModel>> Index()
    {
        var architects = modelContext.Architects;
        return Ok(architects.Select(a => mapper.Map<ArchitectOutputModel>(a)));
    }

    /// <summary>
    /// Gets the list of all architects (IDs and names only).
    /// </summary>
    /// <returns>List of all architects (IDs and names only).</returns>
    /// <response code="200">Always returns 200 OK.</response>
    [HttpGet("basic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<List<ArchitectBasicOutputModel>> Basic()
    {
        var architects = modelContext.Architects;
        return Ok(architects.Select(a => mapper.Map<ArchitectBasicOutputModel>(a)));
    }

    /// <summary>
    /// Creates an architect
    /// </summary>
    /// <param name="architect">Architect to create.</param>
    /// <returns>New architect</returns>
    /// <response code="201">Architect was created.</response>
    /// <response code="400">Some invalid data received.</response>
    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor, Local Editor")]
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Architect> Create(ArchitectInputModel architectInputModel)
    {
        var architect = mapper.Map<Architect>(architectInputModel);
        modelContext.Architects.Add(architect);
        modelContext.SaveChanges();
        return CreatedAtAction("Details", new { architect.Id }, architect);
    }

    [HttpGet("{id}")]
    public ActionResult<ArchitectOutputModel> Details(Guid id)
    {
        var architect = modelContext.Architects
            .Include(a => a.BuildingCards)
            .ThenInclude(c => c.StreetAddresses)
            .ThenInclude(a => a.Street)
            .Include(a => a.BuildingCards)
            .ThenInclude(c => c.Building)
            .ThenInclude(b => b.Location)
            .Include(a => a.BuildingCards)
            .ThenInclude(c => c.Building)
            .ThenInclude(b => b.Events)
            .Include(a => a.BuildingCards)
            .ThenInclude(c => c.Categories)
            .Include(a => a.BuildingCards)
            .ThenInclude(c => c.Parts)
            .ThenInclude(p => p.Design)
            .ThenInclude(d => d.Categories)
            .Include(a => a.Designs)
            .AsSplitQuery()
            .FirstOrDefault(a => a.Id == id);
        if (architect is null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<ArchitectOutputModel>(architect));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor, Local Editor")]
    [HttpPut("{id}")]
    public ActionResult Update(Guid id, ArchitectInputModel architectInputModel)
    {
        var architect = mapper.Map<Architect>(architectInputModel);
        if (architect.Id != id)
        {
            return BadRequest();
        }
        if (modelContext.Architects.AsNoTracking().FirstOrDefault(arch => arch.Id == id) is null)
        {
            return NotFound();
        }
        modelContext.Entry(architect).State = EntityState.Modified;
        modelContext.SaveChanges();
        return NoContent();
    }

}

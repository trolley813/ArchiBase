using ArchiBase.Client.Pages.Locations;
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
public class StreetsController(ModelContext modelContext, IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly IMapper mapper = mapper;

    [HttpGet("")]
    public ActionResult<List<StreetOutputModel>> Index()
    {
        var streets = modelContext.Streets;
        return Ok(mapper.Map<List<StreetOutputModel>>(streets));
    }

    [HttpGet("basic")]
    public ActionResult<List<StreetBasicOutputModel>> Basic(Guid? locId)
    {
        IQueryable<Street> streets = modelContext.Streets;
        if (locId is Guid id)
        {
            streets = streets.Where(s => s.LocationId == id);
        }
        return Ok(mapper.Map<List<StreetBasicOutputModel>>(streets));
    }

    [HttpGet("{id}")]
    public ActionResult<Street> Details(Guid id)
    {
        var street = modelContext.Streets
            .Include(s => s.ActualStreet)
            .Include(s => s.Location)
            .FirstOrDefault(s => s.Id == id);
        if (street is null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<StreetOutputModel>(street));
    }

    [HttpGet("{id}/oldnames")]
    public ActionResult<List<StreetOutputModel>> OldNames(Guid id)
    {
        var street = modelContext.Streets
                    .Include(s => s.ActualStreet)
                    .Include(s => s.Location)
                    .FirstOrDefault(s => s.Id == id);
        if (street is null)
        {
            return NotFound();
        }
        var oldStreets = modelContext.Streets.Include(s => s.ActualStreet).Where(s => s.ActualStreet == street).ToList();
        return Ok(mapper.Map<List<StreetOutputModel>>(oldStreets));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor, Local Editor")]
    [HttpPut("{id}")]
    public ActionResult Update(Guid id, StreetInputModel streetInputModel)
    {
        var street = mapper.Map<Street>(streetInputModel);
        if (street.Id != id)
        {
            return BadRequest();
        }
        if (modelContext.Streets.AsNoTracking().FirstOrDefault(loc => loc.Id == id) is null)
        {
            return NotFound();
        }
        modelContext.Entry(street).State = EntityState.Modified;
        modelContext.SaveChanges();
        return NoContent();
    }
}

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
public class GalleriesController(ModelContext modelContext, IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly IMapper mapper = mapper;

    [HttpGet("")]
    public ActionResult<List<GalleryOutputModel>> Index()
    {
        var galleries = modelContext.Galleries;
        return Ok(mapper.Map<List<GalleryOutputModel>>(galleries));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor")]
    [HttpPost("create")]
    public ActionResult<GalleryOutputModel> Create(GalleryInputModel galleryInputModel)
    {
        var gallery = mapper.Map<Gallery>(galleryInputModel);
        modelContext.Galleries.Add(gallery);
        modelContext.SaveChanges();
        return CreatedAtAction("Details", new { gallery.Id }, mapper.Map<GalleryOutputModel>(gallery));
    }

    [HttpGet("{id}")]
    public ActionResult<GalleryOutputModel> Details(Guid id)
    {
        var gallery = modelContext.Galleries
        .Include(g => g.Photos)
        .FirstOrDefault(g => g.Id == id);
        if (gallery is null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<GalleryOutputModel>(gallery));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor")]
    [HttpPut("{id}")]
    public ActionResult Update(Guid id, GalleryInputModel galleryInputModel)
    {
        var gallery = mapper.Map<Gallery>(galleryInputModel);
        if (gallery.Id != id)
        {
            return BadRequest();
        }
        if (modelContext.Galleries.AsNoTracking().FirstOrDefault(gal => gal.Id == id) is null)
        {
            return NotFound();
        }
        modelContext.Entry(gallery).State = EntityState.Modified;
        modelContext.SaveChanges();
        return NoContent();
    }

}

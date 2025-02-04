using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Shared.Output;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ArchiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Default")]
public class StylesController(ModelContext modelContext, IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly IMapper mapper = mapper;

    [HttpGet("")]
    public ActionResult<List<StyleOutputModel>> Index()
    {
        var styles = modelContext.Styles;
        return Ok(mapper.Map<List<StyleOutputModel>>(styles));
    }
}

using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Shared.Output;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Default")]
public class LicensesController(ModelContext modelContext, IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly IMapper mapper = mapper;

    [HttpGet("")]
    public ActionResult<List<LicenseOutputModel>> Index()
    {
        var licenses = modelContext.Licenses;
        return Ok(mapper.Map<List<LicenseOutputModel>>(licenses));
    }
}

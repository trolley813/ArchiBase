using Archibase.Utils;
using ArchiBase.Data;
using ArchiBase.Models;
using ArchiBase.Shared.Input;
using ArchiBase.Shared.Output;
using ArchiBase.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace ArchiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Default")]
public class BuildingsController(
    ModelContext modelContext,
    CadastreRecordService cadastreRecordService,
    ThumbnailService thumbnailService,
    IAuthorizationService authorizationService,
    IMapper mapper) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;
    private readonly CadastreRecordService cadastreRecordService = cadastreRecordService;
    private readonly ThumbnailService thumbnailService = thumbnailService;
    private readonly IAuthorizationService authorizationService = authorizationService;
    private readonly IMapper mapper = mapper;

    [HttpGet("count")]
    public ActionResult<int> Count()
    {
        return Ok(modelContext.Buildings.Count());
    }

    [HttpGet("extended")]
    public async Task<ActionResult<ExtendedBuildingListPage>> Extended(
        Guid? locId,
        Guid? strId,
        string? designs,
        string? categories,
        string? filter,
        string? orderBy,
        int? limit,
        int? offset
    )
    {
        var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == locId);
        var street = modelContext.Streets.FirstOrDefault(loc => loc.Id == strId);

        var designIds = designs?.Split(',')?.Select(s => Guid.Parse(s))?.ToArray() ?? [];
        var categoryIds = categories?.Split(',')?.Select(s => Guid.Parse(s))?.ToArray() ?? [];

        var rawQuery = modelContext.Buildings
        .AsNoTracking()
        .Include(b => b.Events)
        .Include(b => b.Cards)
        .ThenInclude(c => c.StreetAddresses)
        .Include(b => b.ActualCard)
        .ThenInclude(c => c.StreetAddresses)
        .ThenInclude(a => a.Street)
        .Include(b => b.ActualCard)
        .ThenInclude(c => c.Categories)
        .Include(b => b.ActualCard)
        .ThenInclude(c => c.Parts)
        .ThenInclude(p => p.Design)
        .ThenInclude(d => d.Categories)
        .Include(b => b.Location)
        .Join(modelContext.BuildingStatusMappings, b => b.Id, m => m.BuildingId, (b, m) => new { Building = b, Mapping = m });

        if (location is not null)
        {
            rawQuery = rawQuery.Where(b => b.Building.Location == location);
        }

        if (street is not null)
        {
            rawQuery = rawQuery.Where(b => b.Building.Cards.Any(c => c.StreetAddresses.Any(a => a.Street == street)));
        }

        if (designIds is not null && designIds.Length > 0)
        {
            rawQuery = rawQuery.Where(b => b.Building.ActualCard.Parts.Select(p => p.Design).Any(d => designIds.Contains(d.Id)));
        }

        if (categoryIds is not null && categoryIds.Length > 0)
        {
            rawQuery = rawQuery.Where(b => b.Building.ActualCard.Categories.Any(c => categoryIds.Contains(c.Id)) ||
                b.Building.ActualCard.Parts.Select(p => p.Design).Any(d => d.Categories.Any(c => categoryIds.Contains(c.Id))));
        }

        var query = rawQuery.Select(
        b => new BuildingInfoOutputModel
        {
            Building = mapper.Map<BuildingOutputModel>(b.Building),
            HouseNumber = b.Building.Cards.SelectMany(c => c.StreetAddresses).First().HouseNumber,
            FloorCount = String.Join("; ", b.Building.ActualCard.FloorCount),
            Name = b.Building.ActualCard.Name,
            Built = b.Mapping.ConstructionFinished,
            BuiltSort = (b.Mapping.ConstructionFinished != null) ? b.Mapping.ConstructionFinished.Date : DateTime.MinValue,
            Demolished = b.Mapping.Demolished,
            DemolishedSort = (b.Mapping.Demolished != null) ? b.Mapping.Demolished.Date : DateTime.MinValue,
            Addresses = mapper.Map<List<StreetAddressOutputModel>>(b.Building.ActualCard.StreetAddresses),
            ActualCard = mapper.Map<BuildingCardOutputModel>(b.Building.ActualCard),
            LastCardWithAddress = mapper.Map<BuildingCardOutputModel>(b.Building.ActualCard)
        }
        );

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(filter);
        }
        if (!string.IsNullOrEmpty(orderBy))
        {
            query = query.OrderBy(orderBy);
        }
        var count = query.Count();
        var buildings = query.Skip(offset ?? 0).Take(limit ?? 20).ToList();
        var rawBuildingIds = buildings.Select(b => b.Building.Id);
        var photoData = modelContext
        .BuildingBinds
        .Include(b => b.Photo)
        .Include(b => b.Building)
        .ThenInclude(b => b.Cards)
        .ThenInclude(c => c.StreetAddresses)
        .ThenInclude(a => a.Street)
        .Where(b => rawBuildingIds.Contains(b.Building.Id))
        .AsSplitQuery()
        .ToList()
        .GroupBy(b => b.Building)
        .Select(g => g.MaxBy(p => p.Photo.PublicationDate))
        .ToDictionary(b => b.Building.Id, b => $"data:image/webp;base64,{thumbnailService.GetThumbnail(b.Photo)}");

        var result = new ExtendedBuildingListPage { Count = count, Buildings = buildings, PhotoData = photoData };
        return Ok(result);
    }

    [HttpPost("addresses")]
    public ActionResult<Dictionary<Guid, string>> Addresses(List<Guid> ids)
    {
        var buildingAddresses = modelContext.Buildings
            .Where(b => ids.Contains(b.Id))
            .Include(b => b.Location)
            .Include(b => b.Cards)
            .ThenInclude(c => c.StreetAddresses)
            .ThenInclude(a => a.Street)
            .Select(
                b => new { b.Id, Address = b.ActualAddressWithLocation }
            ).AsSplitQuery().ToDictionary(p => p.Id, p => p.Address);
        return Ok(buildingAddresses);
    }

    [HttpPost("forphotos")]
    public ActionResult<List<Guid>> ForPhotos(List<Guid> ids)
    {
        var buildingIds = modelContext.BuildingBinds
            .Include(b => b.Building)
            .Include(b => b.Photo)
            .Where(b => ids.Contains(b.Photo.Id))
            .Select(b => b.Building.Id).Distinct()
            .ToList();
        return Ok(buildingIds);
    }

    [HttpPost("bound")]
    public ActionResult<List<BuildingOutputModel>> Bound(List<Guid> ids)
    {
        var buildings = modelContext.Buildings.Include(b => b.Location).Where(b => ids.Contains(b.Id)).ToList();
        return Ok(mapper.Map<List<BuildingOutputModel>>(buildings));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor, Local Editor")]
    [HttpPost("create")]
    public async Task<ActionResult<BuildingOutputModel>> CreateAsync(BuildingInputModel buildingInputModel)
    {
        var canEditResult = await authorizationService.AuthorizeAsync(User, $"LocalEditor-{buildingInputModel.LocationId}");
        if (!canEditResult.Succeeded)
        {
            return Forbid();
        }
        var building = mapper.Map<Building>(buildingInputModel);
        modelContext.Buildings.Add(building);
        modelContext.SaveChanges();
        return CreatedAtAction("Details", new { building.Id }, mapper.Map<BuildingOutputModel>(building));
    }

    [HttpGet("{id}")]
    public ActionResult<Building> Details(Guid id)
    {
        var building = modelContext.Buildings.Include(b => b.Events)
                .Include(b => b.Location)
                .Include(b => b.Groups)
                .Include(b => b.Cards)
                .ThenInclude(c => c.Categories)
                .Include(b => b.Cards)
                .ThenInclude(c => c.StreetAddresses)
                .ThenInclude(a => a.Street)
                .Include(b => b.Cards)
                .ThenInclude(c => c.Architects)
                .Include(b => b.Cards)
                .ThenInclude(c => c.Style)
                .Include(b => b.Cards)
                .ThenInclude(c => c.Parts)
                .ThenInclude(p => p.Design)
                .ThenInclude(d => d.Categories)
                .Include(b => b.Cards)
                .ThenInclude(c => c.Parts)
                .ThenInclude(p => p.Design)
                .ThenInclude(d => d.CatalogueEntries)
                .ThenInclude(e => e.Catalogue).AsSplitQuery().FirstOrDefault(b => b.Id == id);
        if (building is not null)
        {
            foreach (var card in building.Cards) card.Building = building;
            return Ok(building);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/photos")]
    public ActionResult<List<Photo>> PhotosForBuilding(Guid id)
    {
        var building = modelContext.Buildings.FirstOrDefault(b => b.Id == id);
        var photos = modelContext.BuildingBinds.Where(b => b.Building == building)
        .Select(b => b.Photo).Where(p => modelContext.ActivePhotos.Contains(p))
        .OrderByDescending(p => p.ShootingDate.Date).ToList();
        if (photos is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(photos);
        }
    }

    [HttpGet("{id}/cadastrelink")]
    public ActionResult<string?> CadastreLink(Guid id)
    {
        var building = modelContext.Buildings.FirstOrDefault(b => b.Id == id);
        if (building is null)
        {
            return NotFound();
        }
        return Ok(cadastreRecordService.GetLink(building));
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin, Editor, Local Editor")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(Guid id, BuildingInputModel buildingInputModel)
    {
        var canEditResult = await authorizationService.AuthorizeAsync(User, $"LocalEditor-{buildingInputModel.LocationId}");
        if (!canEditResult.Succeeded)
        {
            return Forbid();
        }
        var building = mapper.Map<Building>(buildingInputModel);
        if (building.Id != id)
        {
            return BadRequest();
        }
        if (modelContext.Buildings.AsNoTracking().FirstOrDefault(bui => bui.Id == id) is null)
        {
            return NotFound();
        }
        var collectionEntry = modelContext.Entry(building).Collection(b => b.Events);
        int i = 1;
        foreach (var ev in building.Events)
        {
            collectionEntry.FindEntry(ev)!.Property("__synthesizedOrdinal").CurrentValue = i++;
        }
        modelContext.Entry(building).State = EntityState.Modified;
        foreach (var card in building.Cards)
        {
            if (modelContext.BuildingCards.AsNoTracking().FirstOrDefault(c => c.Id == card.Id) is not null)
            {
                modelContext.Entry(card).State = EntityState.Modified;
                modelContext.Entry(card).Reference(c => c.ActualFrom).TargetEntry.State = EntityState.Modified;
            }
            else
            {
                modelContext.Entry(card).State = EntityState.Added;
                modelContext.Entry(card).Reference(c => c.ActualFrom).TargetEntry.State = EntityState.Added;
            }

            modelContext.Entry(card).Entity.Building = building;

            // Explicitly set architects and categories
            var cardInput = buildingInputModel.Cards.FirstOrDefault(c => c.Id == card.Id);
            if (cardInput is not null)
            {
                modelContext.Entry(card).Entity.Architects = [.. modelContext.Architects.Where(a => cardInput.ArchitectIds.Contains(a.Id))];
                modelContext.Entry(card).Entity.Categories = [.. modelContext.DesignCategories.Where(a => cardInput.CategoryIds.Contains(a.Id))];
            }

            foreach (var address in card.StreetAddresses)
            {
                if (modelContext.StreetAddresses.AsNoTracking().FirstOrDefault(a => a.Id == address.Id) is not null)
                {
                    modelContext.Entry(address).State = EntityState.Modified;
                }
                else
                {
                    modelContext.Entry(address).State = EntityState.Added;
                }
            }
        }
        // TODO: Better entity handling
        try
        {
            modelContext.SaveChanges();
        }
        catch (ArgumentNullException anex)
        {
            if (anex.ParamName == "entity")
            {
                return NoContent();
            }
            else
            {
                throw;
            }
        }
        return NoContent();
    }
}

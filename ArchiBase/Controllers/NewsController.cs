using ArchiBase.Data;
using ArchiBase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[ResponseCache(CacheProfileName = "Default")]
public class NewsController(ModelContext modelContext) : ControllerBase
{
    private readonly ModelContext modelContext = modelContext;

    [HttpGet("")]
    public ActionResult<List<NewsItem>> Index()
    {
        var newsItems = modelContext.NewsItems;
        return Ok(newsItems);
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin")]
    [HttpPost("create")]
    public ActionResult<NewsItem> Create(NewsItem newsItem)
    {
        modelContext.NewsItems.Add(newsItem);
        modelContext.SaveChanges();
        return CreatedAtAction("Details", new { newsItem.Id }, newsItem);
    }

    [HttpGet("{id}")]
    public ActionResult<NewsItem> Details(Guid id)
    {
        var newsItem = modelContext.NewsItems
            .FirstOrDefault(n => n.Id == id);
        if (newsItem is null)
        {
            return NotFound();
        }
        return Ok(newsItem);
    }

    [Authorize(AuthenticationSchemes = StaticData.AuthSchemes, Roles = "Admin")]
    [HttpPut("{id}")]
    public ActionResult Update(Guid id, NewsItem newsItem)
    {
        if (newsItem.Id != id)
        {
            return BadRequest();
        }
        if (modelContext.NewsItems.AsNoTracking().FirstOrDefault(news => news.Id == id) is null)
        {
            return NotFound();
        }
        modelContext.Entry(newsItem).State = EntityState.Modified;
        modelContext.SaveChanges();
        return NoContent();
    }

}

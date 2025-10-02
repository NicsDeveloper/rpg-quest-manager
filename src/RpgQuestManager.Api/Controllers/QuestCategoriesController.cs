using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class QuestCategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public QuestCategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestCategory>>> GetCategories()
    {
        var categories = await _context.QuestCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Difficulty)
            .ThenBy(c => c.MinLevel)
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestCategory>> GetCategory(int id)
    {
        var category = await _context.QuestCategories
            .Include(c => c.Quests)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpGet("{id}/quests")]
    public async Task<ActionResult<IEnumerable<Quest>>> GetQuestsByCategory(int id)
    {
        var quests = await _context.Quests
            .Where(q => q.CategoryId == id)
            .OrderBy(q => q.StoryOrder)
            .ThenBy(q => q.RequiredLevel)
            .ToListAsync();

        return Ok(quests);
    }
}

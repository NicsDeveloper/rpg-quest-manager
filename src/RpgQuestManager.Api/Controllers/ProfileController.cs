using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(ApplicationDbContext context, ILogger<ProfileController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("my-hero")]
    public async Task<ActionResult> GetMyHero()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var hero = await _context.Heroes
            .Include(h => h.HeroItems)
                .ThenInclude(hi => hi.Item)
            .Include(h => h.HeroQuests)
                .ThenInclude(hq => hq.Quest)
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (hero == null)
        {
            return NotFound(new { message = "Você ainda não possui um herói. Peça para um administrador criar um para você." });
        }

        return Ok(hero);
    }

    [HttpGet("my-quests")]
    public async Task<ActionResult> GetMyQuests()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var hero = await _context.Heroes
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (hero == null)
        {
            return NotFound(new { message = "Você ainda não possui um herói." });
        }

        var quests = await _context.HeroQuests
            .Include(hq => hq.Quest)
                .ThenInclude(q => q.Rewards)
                    .ThenInclude(r => r.Item)
            .Where(hq => hq.HeroId == hero.Id)
            .OrderByDescending(hq => hq.StartedAt)
            .ToListAsync();

        return Ok(quests);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetMyStats()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var hero = await _context.Heroes
            .Include(h => h.HeroQuests)
            .Include(h => h.HeroItems)
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (hero == null)
        {
            return NotFound(new { message = "Você ainda não possui um herói." });
        }

        var stats = new
        {
            TotalQuests = hero.HeroQuests.Count,
            CompletedQuests = hero.HeroQuests.Count(hq => hq.IsCompleted),
            TotalItems = hero.HeroItems.Sum(hi => hi.Quantity),
            UniqueItems = hero.HeroItems.Count,
            EquippedItems = hero.HeroItems.Count(hi => hi.IsEquipped),
            TotalGold = hero.Gold,
            CurrentLevel = hero.Level,
            TotalExperience = hero.Experience,
            ExperienceForNextLevel = hero.GetExperienceForNextLevel(),
            PlayDays = (DateTime.UtcNow - hero.CreatedAt).Days,
            PowerRating = hero.Strength + hero.Intelligence + hero.Dexterity
        };

        return Ok(stats);
    }
}

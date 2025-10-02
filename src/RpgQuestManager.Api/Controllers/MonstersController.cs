using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MonstersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MonstersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Monster>>> GetMonsters()
    {
        var monsters = await _context.Monsters
            .Where(m => m.IsActive)
            .OrderBy(m => m.Level)
            .ThenBy(m => m.Type)
            .ToListAsync();

        return Ok(monsters);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Monster>> GetMonster(int id)
    {
        var monster = await _context.Monsters
            .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

        if (monster == null)
        {
            return NotFound();
        }

        return Ok(monster);
    }

    [HttpGet("by-level/{level}")]
    public async Task<ActionResult<IEnumerable<Monster>>> GetMonstersByLevel(int level)
    {
        var monsters = await _context.Monsters
            .Where(m => m.IsActive && m.Level <= level + 5 && m.Level >= level - 5)
            .OrderBy(m => m.Level)
            .ToListAsync();

        return Ok(monsters);
    }

    [HttpGet("by-type/{type}")]
    public async Task<ActionResult<IEnumerable<Monster>>> GetMonstersByType(MonsterType type)
    {
        var monsters = await _context.Monsters
            .Where(m => m.IsActive && m.Type == type)
            .OrderBy(m => m.Level)
            .ToListAsync();

        return Ok(monsters);
    }

    [HttpGet("bosses")]
    public async Task<ActionResult<IEnumerable<Monster>>> GetBosses()
    {
        var bosses = await _context.Monsters
            .Where(m => m.IsActive && m.IsBoss)
            .OrderBy(m => m.Level)
            .ToListAsync();

        return Ok(bosses);
    }

    [HttpGet("elites")]
    public async Task<ActionResult<IEnumerable<Monster>>> GetElites()
    {
        var elites = await _context.Monsters
            .Where(m => m.IsActive && m.IsElite)
            .OrderBy(m => m.Level)
            .ToListAsync();

        return Ok(elites);
    }
}

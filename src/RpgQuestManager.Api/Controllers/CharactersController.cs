using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharactersController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public CharactersController(ApplicationDbContext db) { _db = db; }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _db.Characters.AsNoTracking().ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var character = await _db.Characters.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (character == null)
            {
                return NotFound(new { message = "Personagem não encontrado" });
            }

            return Ok(new
            {
                id = character.Id,
                userId = character.UserId,
                name = character.Name,
                level = character.Level,
                experience = character.Experience,
                nextLevelExperience = character.NextLevelExperience,
                health = character.Health,
                maxHealth = character.MaxHealth,
                attack = character.Attack,
                defense = character.Defense,
                morale = character.Morale,
                gold = character.Gold,
                statusEffects = character.StatusEffects.Select(se => se.ToString()).ToList(),
                createdAt = character.CreatedAt,
                lastPlayedAt = character.LastPlayedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }
}



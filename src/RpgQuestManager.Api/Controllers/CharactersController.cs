using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CharactersController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public CharactersController(ApplicationDbContext db) { _db = db; }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("Usuário não autenticado");
    }

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
            var userId = GetCurrentUserId();
            
            // Verificar se o personagem pertence ao usuário autenticado
            var character = await _db.Characters.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
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
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCharacter([FromBody] CreateCharacterRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            // Verificar se o usuário já tem um personagem
            var existingCharacter = await _db.Characters.FirstOrDefaultAsync(c => c.UserId == userId);
            if (existingCharacter != null)
            {
                return BadRequest(new { message = "Você já possui um personagem" });
            }

            var character = new Character
            {
                UserId = userId,
                Name = request.Name,
                Level = 1,
                Experience = 0,
                NextLevelExperience = 1000,
                Health = 100,
                MaxHealth = 100,
                Attack = 10,
                Defense = 5,
                Morale = 50,
                Gold = 100,
                CreatedAt = DateTime.UtcNow,
                LastPlayedAt = DateTime.UtcNow
            };

            _db.Characters.Add(character);
            await _db.SaveChangesAsync();

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
                statusEffects = new List<string>(),
                createdAt = character.CreatedAt,
                lastPlayedAt = character.LastPlayedAt
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    public record CreateCharacterRequest(string Name);
}



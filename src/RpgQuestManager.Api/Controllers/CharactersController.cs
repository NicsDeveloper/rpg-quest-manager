using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CharactersController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly InventoryService _inventoryService;
    
    public CharactersController(ApplicationDbContext db, InventoryService inventoryService) 
    { 
        _db = db;
        _inventoryService = inventoryService;
    }

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
        var data = await _db.Heroes.AsNoTracking().ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            // Verificar se o herói pertence ao usuário autenticado
            var hero = await _db.Heroes.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
            if (hero == null)
            {
                return NotFound(new { message = "Herói não encontrado" });
            }

            // Calcular stats finais com equipamentos
            var equipmentBonuses = await _inventoryService.GetEquipmentBonusesAsync(id);
            
            var finalAttack = hero.CalculateAttack() + equipmentBonuses.attack;
            var finalDefense = hero.CalculateDefense() + equipmentBonuses.defense;
            var finalHealth = hero.MaxHealth + equipmentBonuses.health;
            var finalMorale = 100 + equipmentBonuses.morale;

            return Ok(new
            {
                id = hero.Id,
                userId = hero.UserId,
                name = hero.Name,
                @class = hero.Class,
                level = hero.Level,
                experience = hero.Experience,
                strength = hero.GetTotalStrength(),
                intelligence = hero.GetTotalIntelligence(),
                dexterity = hero.GetTotalDexterity(),
                health = hero.CurrentHealth,
                maxHealth = hero.MaxHealth,
                isInActiveParty = hero.IsInActiveParty,
                partySlot = hero.PartySlot,
                createdAt = hero.CreatedAt,
                finalAttack = finalAttack,
                finalDefense = finalDefense,
                finalHealth = finalHealth,
                finalMorale = finalMorale
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
    public async Task<IActionResult> CreateHero([FromBody] CreateHeroRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            // Verificar se já tem pelo menos um herói nível 5+ (não deletado)
            var heroes = await _db.Heroes.Where(h => h.UserId == userId && !h.IsDeleted).ToListAsync();
            
            // Se já tem heróis, verifica se algum está nível 5+
            if (heroes.Count > 0)
            {
                var hasLevel5Hero = heroes.Any(h => h.Level >= 5);
                if (!hasLevel5Hero)
                {
                    return BadRequest(new { message = "Você precisa ter pelo menos um herói nível 5 ou superior para criar um novo herói." });
                }
            }

            var hero = new Hero
            {
                Name = request.Name,
                Class = request.Class,
                Level = 1,
                Experience = 0,
                UnallocatedAttributePoints = 0,
                Gold = 0,
                UserId = userId,
                IsInActiveParty = false,
                PartySlot = null,
                CreatedAt = DateTime.UtcNow
            };
            
            // Configurar atributos base baseados na classe
            hero.ConfigureInitialAttributes();

            _db.Heroes.Add(hero);
            await _db.SaveChangesAsync();

            // Se é o primeiro herói, adiciona automaticamente à party no slot 1
            var heroCount = await _db.Heroes.CountAsync(h => h.UserId == userId);
            if (heroCount == 1)
            {
                hero.IsInActiveParty = true;
                hero.PartySlot = 1;
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                id = hero.Id,
                userId = hero.UserId,
                name = hero.Name,
                @class = hero.Class,
                level = hero.Level,
                experience = hero.Experience,
                strength = hero.GetTotalStrength(),
                intelligence = hero.GetTotalIntelligence(),
                dexterity = hero.GetTotalDexterity(),
                health = hero.CurrentHealth,
                maxHealth = hero.MaxHealth,
                isInActiveParty = hero.IsInActiveParty,
                partySlot = hero.PartySlot,
                createdAt = hero.CreatedAt
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

    public record CreateHeroRequest(string Name, string Class);
}



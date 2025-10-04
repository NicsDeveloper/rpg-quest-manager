using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CombatController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICombatService _combat;
    private readonly QuestService _questService;
    public CombatController(ApplicationDbContext db, ICombatService combat, QuestService questService) { _db = db; _combat = combat; _questService = questService; }

    public record AttackRequest(int characterId, int monsterId);
    public record AbilityRequest(int characterId, int monsterId, int abilityId);
    public record ItemRequest(int characterId, int monsterId, string itemName);
    public record EscapeRequest(int characterId, int monsterId);
    public record StartCombatRequest(int characterId, int monsterId);

    [HttpPost("start")]
    public async Task<IActionResult> StartCombat([FromBody] StartCombatRequest request)
    {
        var result = await _combat.StartCombatAsync(request.characterId, request.monsterId);
        return Ok(FormatCombatResult(result));
    }

    [HttpPost("attack")]
    public async Task<IActionResult> Attack([FromBody] AttackRequest request)
    {
        var result = await _combat.AttackAsync(request.characterId, request.monsterId);
        return Ok(FormatCombatResult(result));
    }

    [HttpPost("ability")]
    public async Task<IActionResult> UseAbility([FromBody] AbilityRequest request)
    {
        try
        {
            var result = await _combat.UseAbilityAsync(request.characterId, request.monsterId, request.abilityId);
            return Ok(FormatCombatResult(result));
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao usar habilidade: {ex.Message}");
        }
    }

    [HttpPost("item")]
    public async Task<IActionResult> UseItem([FromBody] ItemRequest request)
    {
        try
        {
            var result = await _combat.UseItemAsync(request.characterId, request.monsterId, request.itemName);
            return Ok(FormatCombatResult(result));
        }
        catch (NotImplementedException)
        {
            return BadRequest("Sistema de itens ainda não implementado");
        }
    }

    [HttpPost("escape")]
    public async Task<IActionResult> TryEscape([FromBody] EscapeRequest request)
    {
        var success = await _combat.TryEscapeAsync(request.characterId, request.monsterId);
        return Ok(new { success, message = success ? "Fuga bem-sucedida!" : "Fuga falhou!" });
    }

    [HttpGet("active-quest/{characterId}")]
    public async Task<IActionResult> GetActiveQuest(int characterId)
    {
        var quest = await _questService.GetActiveQuestAsync(characterId);
        if (quest == null)
        {
            return Ok(new { hasActiveQuest = false, quest = (object?)null });
        }

        var monster = await _questService.GetQuestMonsterAsync(quest.Id);
        return Ok(new { 
            hasActiveQuest = true, 
            quest = new {
                quest.Id,
                quest.Title,
                quest.Description,
                quest.TargetMonsterName,
                quest.TargetMonsterType,
                quest.Environment,
                quest.Difficulty
            },
            monster = monster != null ? new {
                monster.Id,
                monster.Name,
                monster.Type,
                monster.Rank,
                monster.Habitat,
                monster.Health,
                monster.MaxHealth,
                monster.Attack,
                monster.Defense,
                monster.ExperienceReward
            } : null
        });
    }

    private object FormatCombatResult(CombatResult result)
    {
        return new
        {
            hero = new 
            { 
                result.Hero.Id, 
                result.Hero.Name,
                result.Hero.Level,
                result.Hero.Experience,
                result.Hero.NextLevelExperience,
                result.Hero.Health, 
                result.Hero.MaxHealth,
                result.Hero.Attack,
                result.Hero.Defense,
                result.Hero.Morale,
                moraleLevel = result.HeroMoraleLevel.ToString()
            },
            monster = new 
            { 
                result.Monster.Id, 
                result.Monster.Name,
                result.Monster.Type,
                result.Monster.Rank,
                result.Monster.Habitat,
                result.Monster.Health, 
                result.Monster.MaxHealth,
                result.Monster.Attack,
                result.Monster.Defense,
                result.Monster.ExperienceReward
            },
            combat = new
            {
                damageToMonster = result.DamageToMonster,
                damageToHero = result.DamageToHero,
                result.IsCritical,
                result.IsFumble,
                result.CombatEnded,
                result.Victory,
                result.ExperienceGained,
                result.ActionDescription,
                appliedEffects = result.AppliedEffects.Select(e => e.ToString()).ToList()
            }
        };
    }
}



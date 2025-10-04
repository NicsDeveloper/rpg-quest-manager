using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RewardsController : ControllerBase
{
    private readonly RewardService _rewardService;
    private readonly ApplicationDbContext _db;

    public RewardsController(RewardService rewardService, ApplicationDbContext db)
    {
        _rewardService = rewardService;
        _db = db;
    }

    [HttpGet("combat/{heroId}")]
    public async Task<IActionResult> GetUnclaimedCombatRewards(int heroId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Verificar se o herói pertence ao usuário autenticado
            var hero = await _db.Heroes.FirstOrDefaultAsync(h => h.Id == heroId && h.UserId == userId);
            if (hero == null)
            {
                return Unauthorized(new { message = "Herói não encontrado ou não pertence ao usuário" });
            }
            
            var rewards = await _rewardService.GetUnclaimedCombatRewardsAsync(heroId);
            
            return Ok(new
            {
                heroId,
                rewards = rewards.Select(cr => new
                {
                    id = cr.Id,
                    questId = cr.QuestId,
                    monsterId = cr.MonsterId,
                    monsterName = cr.Monster.Name,
                    questTitle = cr.Quest.Title,
                    createdAt = cr.CreatedAt,
                    rewards = cr.Rewards.Select(r => new
                    {
                        type = r.Type.ToString(),
                        name = r.Name,
                        description = r.Description,
                        quantity = r.Quantity,
                        goldAmount = r.GoldAmount,
                        experienceAmount = r.ExperienceAmount,
                        itemId = r.ItemId,
                        diceType = r.DiceType?.ToString(),
                        icon = r.Icon,
                        rarity = r.Rarity?.ToString()
                    })
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("quest/{heroId}")]
    public async Task<IActionResult> GetUnclaimedQuestRewards(int heroId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Verificar se o herói pertence ao usuário autenticado
            var hero = await _db.Heroes.FirstOrDefaultAsync(h => h.Id == heroId && h.UserId == userId);
            if (hero == null)
            {
                return Unauthorized(new { message = "Herói não encontrado ou não pertence ao usuário" });
            }
            
            var rewards = await _rewardService.GetUnclaimedQuestRewardsAsync(heroId);
            
            return Ok(new
            {
                heroId,
                rewards = rewards.Select(qr => new
                {
                    id = qr.Id,
                    questId = qr.QuestId,
                    questTitle = qr.Quest.Title,
                    createdAt = qr.CreatedAt,
                    rewards = qr.Rewards.Select(r => new
                    {
                        type = r.Type.ToString(),
                        name = r.Name,
                        description = r.Description,
                        quantity = r.Quantity,
                        goldAmount = r.GoldAmount,
                        experienceAmount = r.ExperienceAmount,
                        itemId = r.ItemId,
                        diceType = r.DiceType?.ToString(),
                        icon = r.Icon,
                        rarity = r.Rarity?.ToString()
                    })
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("combat/{combatRewardsId}/claim")]
    public async Task<IActionResult> ClaimCombatRewards(int combatRewardsId, [FromBody] ClaimRewardsRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Verificar se o herói pertence ao usuário autenticado
            var hero = await _db.Heroes.FirstOrDefaultAsync(h => h.Id == request.HeroId && h.UserId == userId);
            if (hero == null)
            {
                return Unauthorized(new { message = "Herói não encontrado ou não pertence ao usuário" });
            }
            
            var success = await _rewardService.ClaimCombatRewardsAsync(combatRewardsId, request.HeroId);
            
            if (!success)
            {
                return BadRequest(new { message = "Recompensas não encontradas ou já foram resgatadas" });
            }
            
            return Ok(new { message = "Recompensas de combate resgatadas com sucesso!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("quest/{questRewardsId}/claim")]
    public async Task<IActionResult> ClaimQuestRewards(int questRewardsId, [FromBody] ClaimRewardsRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Verificar se o herói pertence ao usuário autenticado
            var hero = await _db.Heroes.FirstOrDefaultAsync(h => h.Id == request.HeroId && h.UserId == userId);
            if (hero == null)
            {
                return Unauthorized(new { message = "Herói não encontrado ou não pertence ao usuário" });
            }
            
            var success = await _rewardService.ClaimQuestRewardsAsync(questRewardsId, request.HeroId);
            
            if (!success)
            {
                return BadRequest(new { message = "Recompensas não encontradas ou já foram resgatadas" });
            }
            
            return Ok(new { message = "Recompensas de missão resgatadas com sucesso!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    public record ClaimRewardsRequest(int HeroId);
}

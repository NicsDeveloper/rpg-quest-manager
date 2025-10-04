using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestsController : ControllerBase
{
    private readonly QuestService _questService;
    public QuestsController(QuestService questService) { _questService = questService; }

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
        var data = await _questService.GetAvailableQuestsAsync(1); // Default level 1
        return Ok(data);
    }

    [HttpGet("available/{characterId}")]
    public async Task<IActionResult> GetAvailable(int characterId)
    {
        try
        {
            var userId = GetCurrentUserId();
            // Verificar se o characterId pertence ao usuário autenticado
            if (characterId != userId)
            {
                return Forbid("Você só pode acessar suas próprias missões");
            }
            
            var data = await _questService.GetAvailableQuestsAsync(characterId);
            return Ok(data);
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

    [HttpGet("recommended/{level}")]
    public async Task<IActionResult> Recommended(int level)
    {
        var data = await _questService.GetRecommendedQuestsAsync(level);
        return Ok(data);
    }

    [HttpGet("by-category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        if (!Enum.TryParse<QuestCategory>(category, true, out var questCategory)) 
            return BadRequest("Invalid quest category");
        
        var data = await _questService.GetQuestsByCategoryAsync(questCategory);
        return Ok(data);
    }

    [HttpGet("by-difficulty/{difficulty}")]
    public async Task<IActionResult> GetByDifficulty(string difficulty)
    {
        if (!Enum.TryParse<QuestDifficulty>(difficulty, true, out var questDifficulty)) 
            return BadRequest("Invalid quest difficulty");
        
        var data = await _questService.GetQuestsByDifficultyAsync(questDifficulty);
        return Ok(data);
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var quest = await _questService.StartQuestAsync(id, userId);
            return Ok(quest);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var quest = await _questService.CompleteQuestAsync(id, userId);
            return Ok(quest);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("{id}/fail")]
    public async Task<IActionResult> Fail(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var quest = await _questService.FailQuestAsync(id, userId);
            return Ok(quest);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpGet("completed/{characterId}")]
    public async Task<IActionResult> GetCompleted(int characterId)
    {
        try
        {
            var userId = GetCurrentUserId();
            // Verificar se o characterId pertence ao usuário autenticado
            if (characterId != userId)
            {
                return Forbid("Você só pode acessar suas próprias missões");
            }
            
            var data = await _questService.GetCompletedQuestsAsync(characterId);
            return Ok(data);
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
}



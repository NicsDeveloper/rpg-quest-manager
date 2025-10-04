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

    [HttpGet("available/{heroId}")]
    public async Task<IActionResult> GetAvailable(int heroId)
    {
        try
        {
            var userId = GetCurrentUserId();
            // TODO: Verificar se o heroId pertence ao usuário autenticado
            
            var data = await _questService.GetAvailableQuestsAsync(heroId);
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
    public async Task<IActionResult> Start(int id, [FromBody] StartQuestRequest request)
    {
        try
        {
            var quest = await _questService.StartQuestAsync(id, request.HeroId);
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
    public async Task<IActionResult> Complete(int id, [FromBody] CompleteQuestRequest request)
    {
        try
        {
            var quest = await _questService.CompleteQuestAsync(id, request.HeroId);
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
    public async Task<IActionResult> Fail(int id, [FromBody] FailQuestRequest request)
    {
        try
        {
            var quest = await _questService.FailQuestAsync(id, request.HeroId);
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

    [HttpGet("completed/{heroId}")]
    public async Task<IActionResult> GetCompleted(int heroId)
    {
        try
        {
            var userId = GetCurrentUserId();
            // TODO: Verificar se o heroId pertence ao usuário autenticado
            
            var data = await _questService.GetCompletedQuestsAsync(heroId);
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

    public record StartQuestRequest(int HeroId);
    public record CompleteQuestRequest(int HeroId);
    public record FailQuestRequest(int HeroId);
}



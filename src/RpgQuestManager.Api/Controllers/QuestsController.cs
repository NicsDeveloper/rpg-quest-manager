using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestsController : ControllerBase
{
    private readonly QuestService _questService;
    public QuestsController(QuestService questService) { _questService = questService; }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _questService.GetAvailableQuestsAsync(1); // Default level 1
        return Ok(data);
    }

    [HttpGet("available/{characterLevel}")]
    public async Task<IActionResult> GetAvailable(int characterLevel)
    {
        var data = await _questService.GetAvailableQuestsAsync(characterLevel);
        return Ok(data);
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
            var quest = await _questService.StartQuestAsync(id, 1); // Default character ID 1
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
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            var quest = await _questService.CompleteQuestAsync(id, 1); // Default character ID 1
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
    }

    [HttpPost("{id}/fail")]
    public async Task<IActionResult> Fail(int id)
    {
        try
        {
            var quest = await _questService.FailQuestAsync(id, 1); // Default character ID 1
            return Ok(quest);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}



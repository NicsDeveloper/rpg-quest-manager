using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AchievementsController : ControllerBase
{
    private readonly IAchievementService _achievementService;

    public AchievementsController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAchievements()
    {
        var achievements = await _achievementService.GetAllAchievementsAsync();
        return Ok(achievements);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserAchievements(int userId)
    {
        var achievements = await _achievementService.GetUserAchievementsAsync(userId);
        return Ok(achievements);
    }

    [HttpGet("user/{userId}/available")]
    public async Task<IActionResult> GetAvailableAchievements(int userId)
    {
        var achievements = await _achievementService.GetAvailableAchievementsAsync(userId);
        return Ok(achievements);
    }

    [HttpGet("user/{userId}/completed")]
    public async Task<IActionResult> GetCompletedAchievements(int userId)
    {
        var achievements = await _achievementService.GetCompletedAchievementsAsync(userId);
        return Ok(achievements);
    }

    [HttpGet("user/{userId}/claimed")]
    public async Task<IActionResult> GetClaimedAchievements(int userId)
    {
        var achievements = await _achievementService.GetClaimedAchievementsAsync(userId);
        return Ok(achievements);
    }

    [HttpGet("user/{userId}/achievement/{achievementId}")]
    public async Task<IActionResult> GetUserAchievement(int userId, int achievementId)
    {
        var achievement = await _achievementService.GetUserAchievementAsync(userId, achievementId);
        if (achievement == null)
        {
            return NotFound(new { message = "Conquista não encontrada" });
        }
        return Ok(achievement);
    }

    public record UpdateProgressRequest(int UserId, AchievementType Type, int Value);
    [HttpPost("update-progress")]
    public async Task<IActionResult> UpdateAchievementProgress([FromBody] UpdateProgressRequest request)
    {
        var (success, message) = await _achievementService.UpdateAchievementProgressAsync(request.UserId, request.Type, request.Value);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record ClaimRewardRequest(int UserId, int AchievementId);
    [HttpPost("claim-reward")]
    public async Task<IActionResult> ClaimAchievementReward([FromBody] ClaimRewardRequest request)
    {
        var (success, message) = await _achievementService.ClaimAchievementRewardAsync(request.UserId, request.AchievementId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }
}

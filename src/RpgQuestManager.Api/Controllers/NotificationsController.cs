using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserNotifications(int userId, [FromQuery] bool unreadOnly = false)
    {
        var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly);
        return Ok(notifications);
    }

    [HttpGet("{notificationId}")]
    public async Task<IActionResult> GetNotification(int notificationId)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(notificationId);
        if (notification == null)
        {
            return NotFound(new { message = "Notificação não encontrada" });
        }
        return Ok(notification);
    }

    [HttpPost("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(int notificationId, [FromBody] MarkAsReadRequest request)
    {
        var (success, message) = await _notificationService.MarkAsReadAsync(notificationId, request.UserId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    [HttpPost("user/{userId}/read-all")]
    public async Task<IActionResult> MarkAllAsRead(int userId)
    {
        var (success, message) = await _notificationService.MarkAllAsReadAsync(userId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification(int notificationId, [FromBody] DeleteNotificationRequest request)
    {
        var (success, message) = await _notificationService.DeleteNotificationAsync(notificationId, request.UserId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
    {
        var (success, message) = await _notificationService.CreateNotificationAsync(
            request.UserId, 
            request.Title, 
            request.Message, 
            request.Type, 
            request.Priority, 
            request.ActionUrl, 
            request.IconUrl, 
            request.ExpiresAt
        );
        
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    [HttpPost("achievement")]
    public async Task<IActionResult> CreateAchievementNotification([FromBody] CreateAchievementNotificationRequest request)
    {
        var (success, message) = await _notificationService.CreateAchievementNotificationAsync(request.UserId, request.AchievementName);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    [HttpPost("level-up")]
    public async Task<IActionResult> CreateLevelUpNotification([FromBody] CreateLevelUpNotificationRequest request)
    {
        var (success, message) = await _notificationService.CreateLevelUpNotificationAsync(request.UserId, request.NewLevel);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    [HttpPost("quest")]
    public async Task<IActionResult> CreateQuestNotification([FromBody] CreateQuestNotificationRequest request)
    {
        var (success, message) = await _notificationService.CreateQuestNotificationAsync(request.UserId, request.QuestTitle, request.Message);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    [HttpPost("party")]
    public async Task<IActionResult> CreatePartyNotification([FromBody] CreatePartyNotificationRequest request)
    {
        var (success, message) = await _notificationService.CreatePartyNotificationAsync(request.UserId, request.PartyName, request.Message);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    [HttpPost("cleanup")]
    public async Task<IActionResult> CleanupExpiredNotifications()
    {
        await _notificationService.CleanupExpiredNotificationsAsync();
        return Ok(new { message = "Limpeza de notificações expiradas concluída" });
    }

    public record MarkAsReadRequest(int UserId);
    public record DeleteNotificationRequest(int UserId);
    public record CreateNotificationRequest(
        int UserId, 
        string Title, 
        string Message, 
        NotificationType Type, 
        NotificationPriority Priority = NotificationPriority.Normal, 
        string? ActionUrl = null, 
        string? IconUrl = null, 
        DateTime? ExpiresAt = null
    );
    public record CreateAchievementNotificationRequest(int UserId, string AchievementName);
    public record CreateLevelUpNotificationRequest(int UserId, int NewLevel);
    public record CreateQuestNotificationRequest(int UserId, string QuestTitle, string Message);
    public record CreatePartyNotificationRequest(int UserId, string PartyName, string Message);
}

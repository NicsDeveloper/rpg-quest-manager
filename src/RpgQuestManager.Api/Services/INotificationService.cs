using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface INotificationService
{
    Task CreateNotificationAsync(int userId, string title, string message, string type = "Info");
    Task NotifyLevelUpAsync(Hero hero, int oldLevel, int newLevel);
    Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);
}


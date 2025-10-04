using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface INotificationService
{
    Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
    Task<Notification?> GetNotificationByIdAsync(int notificationId);
    Task<(bool success, string message)> MarkAsReadAsync(int notificationId, int userId);
    Task<(bool success, string message)> MarkAllAsReadAsync(int userId);
    Task<(bool success, string message)> DeleteNotificationAsync(int notificationId, int userId);
    Task<(bool success, string message)> CreateNotificationAsync(int userId, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Normal, string? actionUrl = null, string? iconUrl = null, DateTime? expiresAt = null);
    Task<(bool success, string message)> CreateAchievementNotificationAsync(int userId, string achievementName);
    Task<(bool success, string message)> CreateLevelUpNotificationAsync(int userId, int newLevel);
    Task<(bool success, string message)> CreateQuestNotificationAsync(int userId, string questTitle, string message);
    Task<(bool success, string message)> CreatePartyNotificationAsync(int userId, string partyName, string message);
    Task CleanupExpiredNotificationsAsync();
}

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _db;

    public NotificationService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
    {
        var query = _db.Notifications
            .Where(n => n.UserId == userId && (n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow));

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification?> GetNotificationByIdAsync(int notificationId)
    {
        return await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId);
    }

    public async Task<(bool success, string message)> MarkAsReadAsync(int notificationId, int userId)
    {
        try
        {
            var notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
            {
                return (false, "Notificação não encontrada");
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return (true, "Notificação marcada como lida");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao marcar notificação como lida: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> MarkAllAsReadAsync(int userId)
    {
        try
        {
            var notifications = await _db.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return (true, $"{notifications.Count} notificações marcadas como lidas");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao marcar notificações como lidas: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> DeleteNotificationAsync(int notificationId, int userId)
    {
        try
        {
            var notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
            {
                return (false, "Notificação não encontrada");
            }

            _db.Notifications.Remove(notification);
            await _db.SaveChangesAsync();

            return (true, "Notificação removida");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao remover notificação: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> CreateNotificationAsync(int userId, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Normal, string? actionUrl = null, string? iconUrl = null, DateTime? expiresAt = null)
    {
        try
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                Priority = priority,
                ActionUrl = actionUrl,
                IconUrl = iconUrl,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            return (true, "Notificação criada com sucesso");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao criar notificação: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> CreateAchievementNotificationAsync(int userId, string achievementName)
    {
        return await CreateNotificationAsync(
            userId,
            "Conquista Desbloqueada!",
            $"Você desbloqueou a conquista: {achievementName}",
            NotificationType.Achievement,
            NotificationPriority.High,
            "/achievements",
            "trophy-icon",
            DateTime.UtcNow.AddDays(7)
        );
    }

    public async Task<(bool success, string message)> CreateLevelUpNotificationAsync(int userId, int newLevel)
    {
        return await CreateNotificationAsync(
            userId,
            "Level Up!",
            $"Parabéns! Você alcançou o nível {newLevel}!",
            NotificationType.LevelUp,
            NotificationPriority.High,
            "/character",
            "star-icon",
            DateTime.UtcNow.AddDays(3)
        );
    }

    public async Task<(bool success, string message)> CreateQuestNotificationAsync(int userId, string questTitle, string message)
    {
        return await CreateNotificationAsync(
            userId,
            $"Missão: {questTitle}",
            message,
            NotificationType.Quest,
            NotificationPriority.Normal,
            "/quests",
            "map-icon",
            DateTime.UtcNow.AddDays(1)
        );
    }

    public async Task<(bool success, string message)> CreatePartyNotificationAsync(int userId, string partyName, string message)
    {
        return await CreateNotificationAsync(
            userId,
            $"Grupo: {partyName}",
            message,
            NotificationType.Party,
            NotificationPriority.Normal,
            "/parties",
            "users-icon",
            DateTime.UtcNow.AddDays(1)
        );
    }

    public async Task CleanupExpiredNotificationsAsync()
    {
        try
        {
            var expiredNotifications = await _db.Notifications
                .Where(n => n.ExpiresAt != null && n.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            if (expiredNotifications.Any())
            {
                _db.Notifications.RemoveRange(expiredNotifications);
                await _db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw
            Console.WriteLine($"Erro ao limpar notificações expiradas: {ex.Message}");
        }
    }
}

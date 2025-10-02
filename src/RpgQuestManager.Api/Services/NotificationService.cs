using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CreateNotificationAsync(int userId, string title, string message, string type = "Info")
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Notificação criada para usuário {UserId}: {Title}", userId, title);
    }

    public async Task NotifyLevelUpAsync(Hero hero, int oldLevel, int newLevel)
    {
        if (hero.UserId == null) return;

        var statsGained = (newLevel - oldLevel) * 2;
        var goldReward = newLevel * 50;
        
        hero.Gold += goldReward;
        await _context.SaveChangesAsync();

        var message = $"🎉 Seu herói {hero.Name} subiu para o nível {newLevel}!\n\n";
        message += $"💪 Atributos: +{statsGained} em cada atributo\n";
        message += $"🪙 Recompensa: +{goldReward} ouro\n\n";

        var availableQuests = await _context.Quests
            .Where(q => q.RequiredLevel == newLevel && 
                       (q.RequiredClass == "Any" || q.RequiredClass == hero.Class))
            .ToListAsync();

        if (availableQuests.Any())
        {
            message += "🎯 Novas missões disponíveis:\n";
            foreach (var quest in availableQuests.Take(3))
            {
                message += $"• {quest.Name} ({quest.Difficulty})\n";
            }
            
            if (availableQuests.Count > 3)
            {
                message += $"... e mais {availableQuests.Count - 3} missões!";
            }
        }

        await CreateNotificationAsync(hero.UserId.Value, "🎊 Level Up!", message, "Success");
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
    {
        var query = _context.Notifications.Where(n => n.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }
}


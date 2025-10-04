using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface IAchievementService
{
    Task<List<Achievement>> GetAllAchievementsAsync();
    Task<List<UserAchievement>> GetUserAchievementsAsync(int userId);
    Task<UserAchievement?> GetUserAchievementAsync(int userId, int achievementId);
    Task<(bool success, string message)> UpdateAchievementProgressAsync(int userId, AchievementType type, int value);
    Task<(bool success, string message)> ClaimAchievementRewardAsync(int userId, int achievementId);
    Task<List<Achievement>> GetAvailableAchievementsAsync(int userId);
    Task<List<UserAchievement>> GetCompletedAchievementsAsync(int userId);
    Task<List<UserAchievement>> GetClaimedAchievementsAsync(int userId);
}

public class AchievementService : IAchievementService
{
    private readonly ApplicationDbContext _db;
    private readonly INotificationService _notificationService;

    public AchievementService(ApplicationDbContext db, INotificationService notificationService)
    {
        _db = db;
        _notificationService = notificationService;
    }

    public async Task<List<Achievement>> GetAllAchievementsAsync()
    {
        return await _db.Achievements
            .Include(a => a.ItemReward)
            .OrderBy(a => a.SortOrder)
            .ToListAsync();
    }

    public async Task<List<UserAchievement>> GetUserAchievementsAsync(int userId)
    {
        return await _db.UserAchievements
            .Include(ua => ua.Achievement)
            .ThenInclude(a => a.ItemReward)
            .Where(ua => ua.UserId == userId)
            .OrderBy(ua => ua.Achievement.SortOrder)
            .ToListAsync();
    }

    public async Task<UserAchievement?> GetUserAchievementAsync(int userId, int achievementId)
    {
        return await _db.UserAchievements
            .Include(ua => ua.Achievement)
            .ThenInclude(a => a.ItemReward)
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);
    }

    public async Task<(bool success, string message)> UpdateAchievementProgressAsync(int userId, AchievementType type, int value)
    {
        try
        {
            var achievements = await _db.Achievements
                .Where(a => a.Type == type && !a.IsHidden)
                .ToListAsync();

            var updatedAchievements = new List<string>();

            foreach (var achievement in achievements)
            {
                var userAchievement = await _db.UserAchievements
                    .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievement.Id);

                if (userAchievement == null)
                {
                    userAchievement = new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = achievement.Id,
                        Progress = 0,
                        IsCompleted = false,
                        IsClaimed = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    _db.UserAchievements.Add(userAchievement);
                }

                if (!userAchievement.IsCompleted)
                {
                    userAchievement.Progress = Math.Min(achievement.RequiredValue, userAchievement.Progress + value);
                    
                    if (userAchievement.Progress >= achievement.RequiredValue)
                    {
                        userAchievement.IsCompleted = true;
                        userAchievement.CompletedAt = DateTime.UtcNow;
                        updatedAchievements.Add(achievement.Name);
                        
                        // Criar notificação de conquista
                        await _notificationService.CreateAchievementNotificationAsync(userId, achievement.Name);
                    }
                }
            }

            await _db.SaveChangesAsync();

            if (updatedAchievements.Any())
            {
                return (true, $"Conquistas completadas: {string.Join(", ", updatedAchievements)}");
            }

            return (true, "Progresso atualizado");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao atualizar progresso: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> ClaimAchievementRewardAsync(int userId, int achievementId)
    {
        try
        {
            var userAchievement = await _db.UserAchievements
                .Include(ua => ua.Achievement)
                .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);

            if (userAchievement == null)
            {
                return (false, "Conquista não encontrada");
            }

            if (!userAchievement.IsCompleted)
            {
                return (false, "Conquista ainda não foi completada");
            }

            if (userAchievement.IsClaimed)
            {
                return (false, "Recompensa já foi reivindicada");
            }

            // Dar recompensas ao personagem
            var character = await _db.Characters
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (character != null)
            {
                if (userAchievement.Achievement.ExperienceReward > 0)
                {
                    character.Experience += userAchievement.Achievement.ExperienceReward;
                }

                if (userAchievement.Achievement.GoldReward > 0)
                {
                    character.Gold += userAchievement.Achievement.GoldReward;
                }

                // Dar item de recompensa se houver
                if (userAchievement.Achievement.ItemRewardId.HasValue)
                {
                    var inventoryItem = new InventoryItem
                    {
                        CharacterId = character.Id,
                        ItemId = userAchievement.Achievement.ItemRewardId.Value,
                        Quantity = 1,
                        IsEquipped = false
                    };
                    _db.InventoryItems.Add(inventoryItem);
                }
            }

            userAchievement.IsClaimed = true;
            userAchievement.ClaimedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return (true, $"Recompensa reivindicada: {userAchievement.Achievement.Name}");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao reivindicar recompensa: {ex.Message}");
        }
    }

    public async Task<List<Achievement>> GetAvailableAchievementsAsync(int userId)
    {
        var userAchievements = await _db.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementId)
            .ToListAsync();

        return await _db.Achievements
            .Include(a => a.ItemReward)
            .Where(a => !userAchievements.Contains(a.Id) && !a.IsHidden)
            .OrderBy(a => a.SortOrder)
            .ToListAsync();
    }

    public async Task<List<UserAchievement>> GetCompletedAchievementsAsync(int userId)
    {
        return await _db.UserAchievements
            .Include(ua => ua.Achievement)
            .ThenInclude(a => a.ItemReward)
            .Where(ua => ua.UserId == userId && ua.IsCompleted)
            .OrderBy(ua => ua.CompletedAt)
            .ToListAsync();
    }

    public async Task<List<UserAchievement>> GetClaimedAchievementsAsync(int userId)
    {
        return await _db.UserAchievements
            .Include(ua => ua.Achievement)
            .ThenInclude(a => a.ItemReward)
            .Where(ua => ua.UserId == userId && ua.IsClaimed)
            .OrderBy(ua => ua.ClaimedAt)
            .ToListAsync();
    }
}

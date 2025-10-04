using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class QuestService
{
    private readonly ApplicationDbContext _db;
    private readonly MonsterService _monsterService;

    public QuestService(ApplicationDbContext db, MonsterService monsterService)
    {
        _db = db;
        _monsterService = monsterService;
    }

    public async Task<List<Quest>> GetAvailableQuestsAsync(int characterLevel)
    {
        return await _db.Quests
            .Where(q => q.Status == QuestStatus.NotStarted && 
                       (q.RequiredLevel == null || q.RequiredLevel <= characterLevel))
            .OrderBy(q => q.RequiredLevel)
            .ThenBy(q => q.Difficulty)
            .ToListAsync();
    }

    public async Task<List<Quest>> GetCompletedQuestsAsync(int characterId)
    {
        return await _db.Quests
            .Where(q => q.Status == QuestStatus.Completed)
            .OrderBy(q => q.Id)
            .ToListAsync();
    }

    public async Task<List<Quest>> GetQuestsByCategoryAsync(QuestCategory category)
    {
        return await _db.Quests
            .Where(q => q.Category == category)
            .OrderBy(q => q.Difficulty)
            .ThenBy(q => q.RequiredLevel)
            .ToListAsync();
    }

    public async Task<List<Quest>> GetQuestsByDifficultyAsync(QuestDifficulty difficulty)
    {
        return await _db.Quests
            .Where(q => q.Difficulty == difficulty)
            .OrderBy(q => q.RequiredLevel)
            .ToListAsync();
    }

    public async Task<List<Quest>> GetRecommendedQuestsAsync(int characterLevel)
    {
        var levelRange = GetLevelRange(characterLevel);
        
        return await _db.Quests
            .Where(q => q.Status == QuestStatus.NotStarted && 
                       q.RequiredLevel >= levelRange.min && 
                       q.RequiredLevel <= levelRange.max)
            .OrderByDescending(q => q.ExperienceReward)
            .Take(10)
            .ToListAsync();
    }

    public async Task<Quest> StartQuestAsync(int questId, int characterId)
    {
        var quest = await _db.Quests.FirstOrDefaultAsync(q => q.Id == questId);
        if (quest == null) throw new ArgumentException("Quest not found");

        if (quest.Status != QuestStatus.NotStarted)
            throw new InvalidOperationException("Quest already started or completed");

        quest.Status = QuestStatus.InProgress;
        await _db.SaveChangesAsync();
        
        return quest;
    }

    public async Task<Quest> CompleteQuestAsync(int questId, int characterId)
    {
        var quest = await _db.Quests.FirstOrDefaultAsync(q => q.Id == questId);
        if (quest == null) throw new ArgumentException("Quest not found");

        if (quest.Status != QuestStatus.InProgress)
            throw new InvalidOperationException("Quest not in progress");

        quest.Status = QuestStatus.Completed;
        await _db.SaveChangesAsync();
        
        return quest;
    }

    public async Task<Quest> FailQuestAsync(int questId, int characterId)
    {
        var quest = await _db.Quests.FirstOrDefaultAsync(q => q.Id == questId);
        if (quest == null) throw new ArgumentException("Quest not found");

        quest.Status = QuestStatus.Failed;
        await _db.SaveChangesAsync();
        
        return quest;
    }

    public async Task<Quest?> GetActiveQuestAsync(int characterId)
    {
        return await _db.Quests
            .FirstOrDefaultAsync(q => q.Status == QuestStatus.InProgress);
    }

    public async Task<Monster?> GetQuestMonsterAsync(int questId)
    {
        var quest = await _db.Quests.FirstOrDefaultAsync(q => q.Id == questId);
        if (quest == null) return null;

        // Buscar monstro que corresponde ao tipo e nome da missão
        var monster = await _db.Monsters
            .FirstOrDefaultAsync(m => m.Type == quest.TargetMonsterType && 
                                m.Name.Contains(quest.TargetMonsterName));

        return monster;
    }

    public string GetCategoryDescription(QuestCategory category)
    {
        return category switch
        {
            QuestCategory.MainStory => "História Principal",
            QuestCategory.BossFight => "Batalhas Épicas",
            QuestCategory.Dungeon => "Exploração de Masmorras",
            QuestCategory.SpecialEvent => "Eventos Especiais",
            QuestCategory.Daily => "Missões Diárias",
            _ => "Desconhecido"
        };
    }

    public string GetDifficultyDescription(QuestDifficulty difficulty)
    {
        return difficulty switch
        {
            QuestDifficulty.Easy => "Fácil",
            QuestDifficulty.Medium => "Médio",
            QuestDifficulty.Hard => "Difícil",
            QuestDifficulty.Epic => "Épico",
            QuestDifficulty.Legendary => "Lendário",
            _ => "Desconhecido"
        };
    }

    public string GetDifficultyColor(QuestDifficulty difficulty)
    {
        return difficulty switch
        {
            QuestDifficulty.Easy => "#4CAF50",      // Verde
            QuestDifficulty.Medium => "#FF9800",    // Laranja
            QuestDifficulty.Hard => "#F44336",      // Vermelho
            QuestDifficulty.Epic => "#9C27B0",      // Roxo
            QuestDifficulty.Legendary => "#FFD700", // Dourado
            _ => "#666666"
        };
    }

    public int GetDifficultyLevelRange(QuestDifficulty difficulty)
    {
        return difficulty switch
        {
            QuestDifficulty.Easy => 5,      // 1-5
            QuestDifficulty.Medium => 10,   // 6-15
            QuestDifficulty.Hard => 10,     // 16-25
            QuestDifficulty.Epic => 10,     // 26-35
            QuestDifficulty.Legendary => 10, // 36+
            _ => 5
        };
    }

    private (int min, int max) GetLevelRange(int characterLevel)
    {
        return characterLevel switch
        {
            <= 5 => (1, 5),
            <= 15 => (6, 15),
            <= 25 => (16, 25),
            <= 35 => (26, 35),
            _ => (36, 50)
        };
    }
}
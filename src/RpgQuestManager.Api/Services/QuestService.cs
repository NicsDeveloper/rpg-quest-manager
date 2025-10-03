using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class QuestService : IQuestService
{
    private readonly ApplicationDbContext _db;
    public QuestService(ApplicationDbContext db) { _db = db; }

    public async Task<Quest> StartAsync(int questId)
    {
        var quest = await _db.Quests.FirstAsync(q => q.Id == questId);
        quest.Status = QuestStatus.InProgress;
        await _db.SaveChangesAsync();
        return quest;
    }

    public async Task<Quest> CompleteAsync(int questId)
    {
        var quest = await _db.Quests.FirstAsync(q => q.Id == questId);
        quest.Status = QuestStatus.Completed;
        await _db.SaveChangesAsync();
        return quest;
    }
}



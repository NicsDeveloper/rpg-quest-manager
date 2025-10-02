using RpgQuestManager.Api.DTOs.Quests;

namespace RpgQuestManager.Api.Services;

public interface IQuestService
{
    Task<QuestDto> CompleteQuestAsync(int heroId, int questId);
}


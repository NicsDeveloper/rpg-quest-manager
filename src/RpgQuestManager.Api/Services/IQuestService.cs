using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface IQuestService
{
    Task<Quest> StartAsync(int questId);
    Task<Quest> CompleteAsync(int questId);
}



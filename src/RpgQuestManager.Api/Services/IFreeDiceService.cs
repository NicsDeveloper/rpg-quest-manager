using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface IFreeDiceService
{
    Task<List<FreeDiceGrant>> GetUserGrantsAsync(int userId);
    Task<FreeDiceGrant> GetOrCreateGrantAsync(int userId, DiceType diceType);
    Task<bool> ClaimFreeDiceAsync(int userId, DiceType diceType);
}


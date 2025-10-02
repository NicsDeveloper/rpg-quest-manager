using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface ICombatService
{
    Task<CombatSession> StartCombatAsync(int heroId, int questId);
    Task<CombatSession?> GetActiveCombatAsync(int heroId);
    Task<(bool Success, int RollResult, string Message)> RollDiceAsync(int combatSessionId, int enemyId, DiceType diceType);
    Task<(CombatStatus Status, List<Item> Drops)> CompleteCombatAsync(int combatSessionId);
    Task<bool> FleeCombatAsync(int combatSessionId);
}


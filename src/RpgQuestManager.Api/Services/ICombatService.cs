using RpgQuestManager.Api.DTOs.Combat;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface ICombatService
{
    Task<CombatSessionDto> StartCombatAsync(int userId, List<int> heroIds, int questId);
    Task<CombatSessionDetailDto> GetActiveCombatSessionAsync(int userId, int combatSessionId);
    Task<CombatSessionDetailDto?> GetActiveCombatByHeroIdAsync(int userId, int heroId);
    Task<RollDiceResultDto> RollDiceAsync(int userId, int combatSessionId, DiceType diceType);
    Task<CompleteCombatResultDto> CompleteCombatAsync(int userId, int combatSessionId);
    Task FleeCombatAsync(int userId, int combatSessionId);
}

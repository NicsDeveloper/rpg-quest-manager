using RpgQuestManager.Api.DTOs.Combat;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface ICombatService
{
    Task<CombatDetailDto> StartCombatAsync(int userId, StartCombatRequest request);
    Task<CombatDetailDto?> GetActiveCombatAsync(int userId);
    Task ClearActiveCombatAsync(int userId);
    Task<RollDiceResult> RollDiceAsync(int combatSessionId, DiceType diceType);
    Task<EnemyAttackResult> EnemyAttackAsync(int combatSessionId);
    Task<CombatDetailDto> CompleteCombatAsync(int combatSessionId);
    Task<bool> CancelCombatAsync(int combatSessionId);
    Task<UseSpecialAbilityResult> UseSpecialAbilityAsync(int combatSessionId, int heroId);
}

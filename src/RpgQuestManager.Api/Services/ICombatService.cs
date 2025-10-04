using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public record CombatResult(
    Hero Hero, 
    Monster Monster, 
    int DamageToMonster, 
    int DamageToHero, 
    bool IsCritical, 
    bool IsFumble,
    bool CombatEnded,
    bool Victory,
    int ExperienceGained,
    string ActionDescription,
    List<StatusEffectType> AppliedEffects,
    MoraleLevel HeroMoraleLevel,
    int GoldReward = 0,
    List<Item>? DroppedItems = null
);

public interface ICombatService
{
    Task<CombatResult> AttackAsync(int heroId, int monsterId);
    Task<CombatResult> UseAbilityAsync(int heroId, int monsterId, int abilityId);
    Task<CombatResult> UseItemAsync(int heroId, int monsterId, string itemName);
    Task<bool> TryEscapeAsync(int heroId, int monsterId);
    Task<CombatResult> StartCombatAsync(int heroId, int monsterId);
}



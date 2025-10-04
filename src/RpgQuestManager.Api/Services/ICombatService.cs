using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public record CombatResult(
    Character Hero, 
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
    MoraleLevel HeroMoraleLevel
);

public interface ICombatService
{
    Task<CombatResult> AttackAsync(int characterId, int monsterId);
    Task<CombatResult> UseAbilityAsync(int characterId, int monsterId, string abilityName);
    Task<CombatResult> UseItemAsync(int characterId, int monsterId, string itemName);
    Task<bool> TryEscapeAsync(int characterId, int monsterId);
    Task<CombatResult> StartCombatAsync(int characterId, int monsterId);
}



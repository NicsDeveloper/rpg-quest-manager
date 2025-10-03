using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public record CombatResult(Character Hero, Monster Monster, int DamageToMonster, int DamageToHero, bool IsCritical, bool IsFumble);

public interface ICombatService
{
    Task<CombatResult> AttackAsync(int characterId, int monsterId);
}



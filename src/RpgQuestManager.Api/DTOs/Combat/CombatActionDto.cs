using System.ComponentModel.DataAnnotations;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.DTOs.Combat;

public class StartCombatRequest
{
    [Required]
    public int QuestId { get; set; }
    
    [Required]
    public List<int> HeroIds { get; set; } = new();
}

public class RollDiceRequest
{
    [Required]
    public int CombatSessionId { get; set; }
    
    [Required]
    public DiceType DiceType { get; set; }
}

public class RollDiceResult
{
    public int Roll { get; set; }
    public int RequiredRoll { get; set; }
    public bool Success { get; set; }
    public int DamageDealt { get; set; }
    public int EnemyHealthAfter { get; set; }
    public string Message { get; set; } = string.Empty;
    public CombatDetailDto UpdatedCombatSession { get; set; } = new();
}

public class EnemyAttackResult
{
    public int EnemyRoll { get; set; }
    public int EnemyPower { get; set; }
    public int TotalDamage { get; set; }
    public int HeroDefense { get; set; }
    public int FinalDamage { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool AllHeroesDead { get; set; }
    public CombatDetailDto UpdatedCombatSession { get; set; } = new();
}

public class UseSpecialAbilityRequest
{
    [Required]
    public int CombatSessionId { get; set; }
    [Required]
    public int HeroId { get; set; }
}

public class UseSpecialAbilityResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int DamageDealt { get; set; }
    public int HealingDone { get; set; }
    public int CooldownRemaining { get; set; }
    public CombatDetailDto UpdatedCombatSession { get; set; } = new();
}

public class ApplyStatusEffectRequest
{
    [Required]
    public int CombatSessionId { get; set; }
    public int? HeroId { get; set; }
    public int? EnemyId { get; set; }
    [Required]
    public StatusEffectType EffectType { get; set; }
    [Required]
    public int Duration { get; set; }
    public int Intensity { get; set; } = 1;
}

public class StatusEffectResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public StatusEffectType EffectType { get; set; }
    public int Duration { get; set; }
    public int Intensity { get; set; }
}

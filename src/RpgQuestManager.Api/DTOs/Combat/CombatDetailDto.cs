using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.DTOs.Combat;

public class CombatDetailDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<int> HeroIds { get; set; } = new();
    public List<HeroCombatInfo> Heroes { get; set; } = new();
    public int QuestId { get; set; }
    public string QuestName { get; set; } = string.Empty;
    public EnemyCombatInfo CurrentEnemy { get; set; } = new();
    public CombatStatus Status { get; set; }
    public bool IsHeroTurn { get; set; }
    public int CurrentEnemyHealth { get; set; }
    public int MaxEnemyHealth { get; set; }
    public Dictionary<int, int> HeroHealths { get; set; } = new();
    public Dictionary<int, int> MaxHeroHealths { get; set; } = new();
    public List<CombatLogDto> CombatLogs { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Sistema de Morale
    public List<MoraleStateDto> HeroMoraleStates { get; set; } = new();
    public MoraleStateDto? EnemyMoraleState { get; set; }
    
    // Sistema de Combos
    public int ConsecutiveSuccesses { get; set; }
    public int ConsecutiveFailures { get; set; }
    public int ComboMultiplier { get; set; }
    public string LastAction { get; set; } = string.Empty;
}

public class HeroCombatInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public int Dexterity { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int TotalAttack { get; set; }
    public int TotalDefense { get; set; }
    public int TotalMagic { get; set; }
}

public class EnemyCombatInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Power { get; set; }
    public int Health { get; set; }
    public DiceType RequiredDiceType { get; set; }
    public int MinimumRoll { get; set; }
    public CombatType CombatType { get; set; }
    public bool IsBoss { get; set; }
}

public class CombatLogDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EnemyName { get; set; } = string.Empty;
    public DiceType? DiceUsed { get; set; }
    public int? DiceResult { get; set; }
    public int? RequiredRoll { get; set; }
    public bool? Success { get; set; }
    public int? DamageDealt { get; set; }
    public int? EnemyHealthAfter { get; set; }
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class MoraleStateDto
{
    public int Id { get; set; }
    public int? HeroId { get; set; }
    public int? EnemyId { get; set; }
    public string Level { get; set; } = string.Empty;
    public int MoralePoints { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

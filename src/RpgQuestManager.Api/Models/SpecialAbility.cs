namespace RpgQuestManager.Api.Models;

public class SpecialAbility
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public AbilityType Type { get; set; }
    public AbilityCategory Category { get; set; }
    public int RequiredLevel { get; set; } = 1;
    public int ManaCost { get; set; } = 0;
    public int CooldownTurns { get; set; } = 0;
    public int Duration { get; set; } = 0; // Em turnos
    public int Range { get; set; } = 1; // Alcance em células
    public int AreaOfEffect { get; set; } = 1; // Área de efeito
    public bool IsPassive { get; set; } = false;
    public bool IsUltimate { get; set; } = false;
    public int ExperienceCost { get; set; } = 0;
    public int GoldCost { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Efeitos da habilidade
    public int Damage { get; set; } = 0;
    public int Healing { get; set; } = 0;
    public int AttackBonus { get; set; } = 0;
    public int DefenseBonus { get; set; } = 0;
    public int SpeedBonus { get; set; } = 0;
    public List<StatusEffectType> StatusEffects { get; set; } = new();
    public List<StatusEffectType> StatusEffectsToRemove { get; set; } = new();
    public int CriticalChanceBonus { get; set; } = 0;
    public int CriticalDamageBonus { get; set; } = 0;

    // Relacionamentos
    public List<CharacterAbility> CharacterAbilities { get; set; } = new();
    public List<ComboStep> ComboSteps { get; set; } = new();
}

public class CharacterAbility
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public int AbilityId { get; set; }
    public int Level { get; set; } = 1;
    public bool IsUnlocked { get; set; } = false;
    public bool IsEquipped { get; set; } = false;
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public Character Character { get; set; } = null!;
    public SpecialAbility Ability { get; set; } = null!;
}

public class Combo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public ComboType Type { get; set; }
    public int RequiredLevel { get; set; } = 1;
    public int ExperienceReward { get; set; } = 0;
    public int GoldReward { get; set; } = 0;
    public bool IsUnlocked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public List<ComboStep> Steps { get; set; } = new();
    public List<CharacterCombo> CharacterCombos { get; set; } = new();
}

public class ComboStep
{
    public int Id { get; set; }
    public int ComboId { get; set; }
    public int AbilityId { get; set; }
    public int StepOrder { get; set; }
    public int TimeWindow { get; set; } = 3; // Janela de tempo em turnos
    public bool IsRequired { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public Combo Combo { get; set; } = null!;
    public SpecialAbility Ability { get; set; } = null!;
}

public class CharacterCombo
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public int ComboId { get; set; }
    public int Progress { get; set; } = 0; // Progresso atual no combo
    public int LastStepCompleted { get; set; } = 0;
    public DateTime LastStepTime { get; set; } = DateTime.UtcNow;
    public bool IsUnlocked { get; set; } = false;
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public Character Character { get; set; } = null!;
    public Combo Combo { get; set; } = null!;
}

public enum AbilityType
{
    Attack,         // Habilidade de ataque
    Defense,        // Habilidade de defesa
    Support,        // Habilidade de suporte
    Healing,        // Habilidade de cura
    Buff,           // Habilidade de buff
    Debuff,         // Habilidade de debuff
    Movement,       // Habilidade de movimento
    Utility         // Habilidade utilitária
}

public enum AbilityCategory
{
    Basic,          // Habilidades básicas
    Advanced,       // Habilidades avançadas
    Master,         // Habilidades de mestre
    Legendary,      // Habilidades lendárias
    Mythic          // Habilidades míticas
}

public enum ComboType
{
    Attack,         // Combo de ataque
    Defense,        // Combo de defesa
    Support,        // Combo de suporte
    Healing,        // Combo de cura
    Mixed           // Combo misto
}

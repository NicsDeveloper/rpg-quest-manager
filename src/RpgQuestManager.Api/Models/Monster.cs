using System.ComponentModel.DataAnnotations;

namespace RpgQuestManager.Api.Models;

public enum MonsterType
{
    Goblin,     // Goblin
    Orc,        // Orc
    Troll,      // Troll
    Dragon,     // Dragão
    Undead,     // Morto-vivo
    Demon,      // Demônio
    Elemental,  // Elemental
    Beast,      // Besta
    Humanoid,   // Humanóide
    Construct,  // Constructo
    Aberration, // Aberração
    Fiend,      // Diabo
    Celestial,  // Celestial
    Fey,        // Fada
    Giant,      // Gigante
    Ooze,       // Lodo
    Plant,      // Planta
    Swarm,      // Enxame
    Boss,       // Boss
    Elite       // Elite
}

public enum MonsterSize
{
    Tiny,       // Minúsculo
    Small,      // Pequeno
    Medium,     // Médio
    Large,      // Grande
    Huge,       // Enorme
    Gargantuan  // Gigantesco
}

public class Monster
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public MonsterType Type { get; set; }
    public MonsterSize Size { get; set; } = MonsterSize.Medium;
    
    // Atributos Base
    public int Level { get; set; } = 1;
    public int Power { get; set; } = 10;
    public int Health { get; set; } = 100;
    public int Armor { get; set; } = 10;
    public int Speed { get; set; } = 30;
    
    // Atributos Avançados
    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Constitution { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Wisdom { get; set; } = 10;
    public int Charisma { get; set; } = 10;
    
    // Sistema de Combate
    public DiceType AttackDice { get; set; } = DiceType.D6;
    public int AttackBonus { get; set; } = 0;
    public int DamageBonus { get; set; } = 0;
    public int CriticalChance { get; set; } = 5; // 5% base
    
    // Sistema de Resistências
    public string Resistances { get; set; } = "[]"; // JSON string de resistências
    public string Immunities { get; set; } = "[]"; // JSON string de imunidades
    public string Vulnerabilities { get; set; } = "[]"; // JSON string de vulnerabilidades
    
    // Sistema de Status Effects
    public string StatusEffects { get; set; } = "[]"; // JSON string de status effects que pode aplicar
    public string StatusImmunities { get; set; } = "[]"; // JSON string de imunidades a status effects
    
    // Sistema de Habilidades Especiais
    public string SpecialAbilities { get; set; } = "[]"; // JSON string de habilidades especiais
    public int SpecialAbilityCooldown { get; set; } = 3; // Cooldown em turnos
    
    // Sistema de Ambiente
    public QuestEnvironment PreferredEnvironment { get; set; } = QuestEnvironment.Forest;
    public string EnvironmentalBonuses { get; set; } = "[]"; // JSON string de bônus ambientais
    
    // Sistema de Recompensas
    public int ExperienceReward { get; set; } = 100;
    public int GoldReward { get; set; } = 50;
    public string LootTable { get; set; } = "[]"; // JSON string de loot table
    
    // Sistema de Morale
    public int BaseMorale { get; set; } = 50;
    public int MoraleRange { get; set; } = 20; // Variação de ±20 pontos
    
    // Sistema de Boss
    public bool IsBoss { get; set; } = false;
    public int BossPhase { get; set; } = 1; // Fase do boss (1-3)
    public int BossHealthThreshold { get; set; } = 50; // % de vida para mudar de fase
    public string BossPhases { get; set; } = "[]"; // JSON string de fases do boss
    
    // Sistema de Grupo
    public bool IsElite { get; set; } = false;
    public int MinGroupSize { get; set; } = 1;
    public int MaxGroupSize { get; set; } = 1;
    public int SpawnChance { get; set; } = 100; // % de chance de spawn
    
    // Sistema de Aparência
    public string Icon { get; set; } = "👹";
    public string Color { get; set; } = "#ef4444";
    public string Model { get; set; } = "default";
    
    // Sistema de Lore
    public string Lore { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Weakness { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    public string GetTypeIcon()
    {
        return Type switch
        {
            MonsterType.Goblin => "👹",
            MonsterType.Orc => "👹",
            MonsterType.Troll => "🧌",
            MonsterType.Dragon => "🐉",
            MonsterType.Undead => "💀",
            MonsterType.Demon => "👹",
            MonsterType.Elemental => "⚡",
            MonsterType.Beast => "🐺",
            MonsterType.Humanoid => "🧑",
            MonsterType.Construct => "🤖",
            MonsterType.Aberration => "👽",
            MonsterType.Fiend => "😈",
            MonsterType.Celestial => "👼",
            MonsterType.Fey => "🧚",
            MonsterType.Giant => "👹",
            MonsterType.Ooze => "🟢",
            MonsterType.Plant => "🌿",
            MonsterType.Swarm => "🐝",
            MonsterType.Boss => "👑",
            MonsterType.Elite => "⭐",
            _ => "❓"
        };
    }
    
    public string GetSizeDescription()
    {
        return Size switch
        {
            MonsterSize.Tiny => "Minúsculo",
            MonsterSize.Small => "Pequeno",
            MonsterSize.Medium => "Médio",
            MonsterSize.Large => "Grande",
            MonsterSize.Huge => "Enorme",
            MonsterSize.Gargantuan => "Gigantesco",
            _ => "Desconhecido"
        };
    }
    
    public string GetDifficultyColor()
    {
        return Level switch
        {
            <= 5 => "#10b981",   // Verde - Fácil
            <= 10 => "#f59e0b",  // Amarelo - Médio
            <= 20 => "#ef4444",  // Vermelho - Difícil
            <= 30 => "#8b5cf6",  // Roxo - Épico
            _ => "#f97316"        // Laranja - Lendário
        };
    }
    
    public int GetEffectiveHealth()
    {
        var baseHealth = Health;
        var constitutionBonus = (Constitution - 10) / 2;
        return baseHealth + (constitutionBonus * Level);
    }
    
    public int GetEffectiveArmor()
    {
        var baseArmor = Armor;
        var dexterityBonus = (Dexterity - 10) / 2;
        return baseArmor + dexterityBonus;
    }
    
    public int GetEffectivePower()
    {
        var basePower = Power;
        var strengthBonus = (Strength - 10) / 2;
        return basePower + strengthBonus;
    }
}

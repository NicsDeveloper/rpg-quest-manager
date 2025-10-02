using System.ComponentModel.DataAnnotations;

namespace RpgQuestManager.Api.Models;

public enum MoraleLevel
{
    Desperate,      // Desespero: +50% dano, -50% defesa, +30% chance de crítico
    Low,           // Morale baixo: -20% dano, -15% defesa
    Normal,        // Morale normal: sem modificadores
    High,          // Morale alto: +20% chance de crítico, +10% dano
    Inspired       // Inspirado: +30% chance de crítico, +20% dano, +15% defesa
}

public class MoraleState
{
    [Key]
    public int Id { get; set; }
    
    public int CombatSessionId { get; set; }
    public CombatSession CombatSession { get; set; } = null!;
    
    public int? HeroId { get; set; }
    public Hero? Hero { get; set; }
    
    public int? EnemyId { get; set; }
    public Enemy? Enemy { get; set; }
    
    public MoraleLevel Level { get; set; } = MoraleLevel.Normal;
    public int MoralePoints { get; set; } = 50; // 0-100
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    public string GetDescription()
    {
        return Level switch
        {
            MoraleLevel.Desperate => $"Desespero - Dano +50%, Defesa -50%, Crítico +30%",
            MoraleLevel.Low => $"Morale Baixo - Dano -20%, Defesa -15%",
            MoraleLevel.Normal => $"Morale Normal - Sem modificadores",
            MoraleLevel.High => $"Morale Alto - Crítico +20%, Dano +10%",
            MoraleLevel.Inspired => $"Inspirado - Crítico +30%, Dano +20%, Defesa +15%",
            _ => "Morale Desconhecido"
        };
    }
    
    public string GetIcon()
    {
        return Level switch
        {
            MoraleLevel.Desperate => "😱",
            MoraleLevel.Low => "😔",
            MoraleLevel.Normal => "😐",
            MoraleLevel.High => "😊",
            MoraleLevel.Inspired => "🤩",
            _ => "❓"
        };
    }
    
    public Dictionary<string, float> GetModifiers()
    {
        var modifiers = new Dictionary<string, float>();
        
        switch (Level)
        {
            case MoraleLevel.Desperate:
                modifiers["damage"] = 0.50f;
                modifiers["defense"] = -0.50f;
                modifiers["critical_chance"] = 0.30f;
                break;
                
            case MoraleLevel.Low:
                modifiers["damage"] = -0.20f;
                modifiers["defense"] = -0.15f;
                break;
                
            case MoraleLevel.Normal:
                // Sem modificadores
                break;
                
            case MoraleLevel.High:
                modifiers["critical_chance"] = 0.20f;
                modifiers["damage"] = 0.10f;
                break;
                
            case MoraleLevel.Inspired:
                modifiers["critical_chance"] = 0.30f;
                modifiers["damage"] = 0.20f;
                modifiers["defense"] = 0.15f;
                break;
        }
        
        return modifiers;
    }
    
    public void UpdateMorale(int change)
    {
        MoralePoints = Math.Max(0, Math.Min(100, MoralePoints + change));
        UpdateLevel();
        LastUpdated = DateTime.UtcNow;
    }
    
    private void UpdateLevel()
    {
        Level = MoralePoints switch
        {
            <= 10 => MoraleLevel.Desperate,
            <= 30 => MoraleLevel.Low,
            <= 70 => MoraleLevel.Normal,
            <= 90 => MoraleLevel.High,
            _ => MoraleLevel.Inspired
        };
    }
    
    public void ApplyMoraleEvent(MoraleEventType eventType)
    {
        var change = eventType switch
        {
            MoraleEventType.CriticalHit => 15,
            MoraleEventType.SuccessfulAttack => 5,
            MoraleEventType.TakeDamage => -10,
            MoraleEventType.Death => -30,
            MoraleEventType.Victory => 25,
            MoraleEventType.SpecialAbility => 10,
            MoraleEventType.StatusEffect => -5,
            MoraleEventType.EnvironmentalDamage => -8,
            _ => 0
        };
        
        UpdateMorale(change);
    }
}

public enum MoraleEventType
{
    CriticalHit,
    SuccessfulAttack,
    TakeDamage,
    Death,
    Victory,
    SpecialAbility,
    StatusEffect,
    EnvironmentalDamage
}

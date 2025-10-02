using System.ComponentModel.DataAnnotations;

namespace RpgQuestManager.Api.Models;

public enum StatusEffectType
{
    Poisoned,    // Envenenado: -2 vida por turno
    Burning,     // Queimado: -3 vida por turno
    Frozen,      // Congelado: Pula 1 turno
    Bleeding,    // Sangrando: -1 vida por turno
    Berserker,   // Berserker: +50% dano, -25% defesa
    Blessed,     // Abençoado: +25% dano, +25% defesa
    Shielded,    // Protegido: +50% defesa
    Weakened,    // Enfraquecido: -25% dano
    Strengthened // Fortalecido: +25% dano
}

public class StatusEffect
{
    [Key]
    public int Id { get; set; }
    
    public int CombatSessionId { get; set; }
    public CombatSession CombatSession { get; set; } = null!;
    
    public int? HeroId { get; set; }
    public Hero? Hero { get; set; }
    
    public int? EnemyId { get; set; }
    public Enemy? Enemy { get; set; }
    
    public StatusEffectType Type { get; set; }
    public int Duration { get; set; } // Turnos restantes
    public int Intensity { get; set; } = 1; // Intensidade do efeito (1-3)
    public string Description { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    
    public bool IsActive => Duration > 0 && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);
    
    public string GetEffectDescription()
    {
        return Type switch
        {
            StatusEffectType.Poisoned => $"Envenenado: -{2 * Intensity} vida por turno ({Duration} turnos)",
            StatusEffectType.Burning => $"Queimado: -{3 * Intensity} vida por turno ({Duration} turnos)",
            StatusEffectType.Frozen => $"Congelado: Pula {Duration} turnos",
            StatusEffectType.Bleeding => $"Sangrando: -{1 * Intensity} vida por turno ({Duration} turnos)",
            StatusEffectType.Berserker => $"Berserker: +{50 * Intensity}% dano, -{25 * Intensity}% defesa ({Duration} turnos)",
            StatusEffectType.Blessed => $"Abençoado: +{25 * Intensity}% dano, +{25 * Intensity}% defesa ({Duration} turnos)",
            StatusEffectType.Shielded => $"Protegido: +{50 * Intensity}% defesa ({Duration} turnos)",
            StatusEffectType.Weakened => $"Enfraquecido: -{25 * Intensity}% dano ({Duration} turnos)",
            StatusEffectType.Strengthened => $"Fortalecido: +{25 * Intensity}% dano ({Duration} turnos)",
            _ => "Efeito desconhecido"
        };
    }
    
    public string GetEffectIcon()
    {
        return Type switch
        {
            StatusEffectType.Poisoned => "☠️",
            StatusEffectType.Burning => "🔥",
            StatusEffectType.Frozen => "❄️",
            StatusEffectType.Bleeding => "🩸",
            StatusEffectType.Berserker => "😡",
            StatusEffectType.Blessed => "✨",
            StatusEffectType.Shielded => "🛡️",
            StatusEffectType.Weakened => "😵",
            StatusEffectType.Strengthened => "💪",
            _ => "❓"
        };
    }
}

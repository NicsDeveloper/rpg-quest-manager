namespace RpgQuestManager.Api.Models;

/// <summary>
/// Define vulnerabilidades específicas de bosses a combos de party
/// </summary>
public class BossWeakness
{
    public int Id { get; set; }
    public int EnemyId { get; set; }
    public int ComboId { get; set; }
    
    // Efeitos do combo contra este boss
    public int RollReduction { get; set; } // Redução no roll necessário (ex: -3)
    public decimal DropMultiplier { get; set; } = 1.0m; // Multiplicador de drop (ex: 1.20 = +20%)
    public decimal ExpMultiplier { get; set; } = 1.0m; // Multiplicador de XP
    
    public string FlavorText { get; set; } = string.Empty; // Texto de lore
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public Enemy Enemy { get; set; } = null!;
    public PartyCombo Combo { get; set; } = null!;
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RpgQuestManager.Api.Models;

public class CombatLog
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int CombatSessionId { get; set; }
    
    public int? HeroId { get; set; }
    public int? EnemyId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty; // "HERO_ATTACK", "ENEMY_ATTACK", "DAMAGE", etc.
    
    public DiceType? DiceUsed { get; set; }
    public int? DiceResult { get; set; }
    public int? RequiredRoll { get; set; }
    public bool? Success { get; set; }
    public int? DamageDealt { get; set; }
    public int? EnemyHealthAfter { get; set; }
    
    [MaxLength(1000)]
    public string Details { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Navegação
    [ForeignKey("CombatSessionId")]
    public CombatSession CombatSession { get; set; } = null!;
    
    [ForeignKey("EnemyId")]
    public Enemy? Enemy { get; set; }
}

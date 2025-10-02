using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace RpgQuestManager.Api.Models;

public class CombatSession
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public string HeroIds { get; set; } = string.Empty; // JSON array de IDs
    
    [Required]
    public int QuestId { get; set; }
    
    [Required]
    public int CurrentEnemyId { get; set; }
    
    // Status do combate
    public CombatStatus Status { get; set; } = CombatStatus.Preparing;
    
    // Turno atual
    public bool IsHeroTurn { get; set; } = true;
    
    // Vida atual
    public int CurrentEnemyHealth { get; set; }
    public int MaxEnemyHealth { get; set; }
    
    // Vida dos heróis (JSON)
    public string HeroHealths { get; set; } = "{}"; // {heroId: health}
    public string MaxHeroHealths { get; set; } = "{}"; // {heroId: maxHealth}
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Sistema de Habilidades Especiais
    public string HeroAbilityCooldowns { get; set; } = "{}"; // JSON string de Dictionary<int, int> (HeroId, CooldownRestante)
    
    public Dictionary<int, int> GetHeroAbilityCooldowns() => JsonSerializer.Deserialize<Dictionary<int, int>>(HeroAbilityCooldowns) ?? new Dictionary<int, int>();
    
    public void SetHeroAbilityCooldowns(Dictionary<int, int> cooldowns) => HeroAbilityCooldowns = JsonSerializer.Serialize(cooldowns);
    
    // Sistema de Combos
    public int ConsecutiveSuccesses { get; set; } = 0; // Sucessos consecutivos
    public int ConsecutiveFailures { get; set; } = 0; // Falhas consecutivas
    public int ComboMultiplier { get; set; } = 1; // Multiplicador de combo atual
    public string LastAction { get; set; } = string.Empty; // Última ação realizada
    
    // Navegação
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    [ForeignKey("QuestId")]
    public Quest Quest { get; set; } = null!;
    
    [ForeignKey("CurrentEnemyId")]
    public Enemy CurrentEnemy { get; set; } = null!;
    
    // Logs de combate
    public virtual ICollection<CombatLog> CombatLogs { get; set; } = new List<CombatLog>();
    
    // Métodos auxiliares
    public List<int> GetHeroIdsList()
    {
        if (string.IsNullOrEmpty(HeroIds)) return new List<int>();
        return System.Text.Json.JsonSerializer.Deserialize<List<int>>(HeroIds) ?? new List<int>();
    }
    
    public void SetHeroIdsList(List<int> heroIds)
    {
        HeroIds = System.Text.Json.JsonSerializer.Serialize(heroIds);
    }
    
    public Dictionary<int, int> GetHeroHealths()
    {
        if (string.IsNullOrEmpty(HeroHealths)) return new Dictionary<int, int>();
        return System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(HeroHealths) ?? new Dictionary<int, int>();
    }
    
    public void SetHeroHealths(Dictionary<int, int> healths)
    {
        HeroHealths = System.Text.Json.JsonSerializer.Serialize(healths);
    }
    
    public Dictionary<int, int> GetMaxHeroHealths()
    {
        if (string.IsNullOrEmpty(MaxHeroHealths)) return new Dictionary<int, int>();
        return System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(MaxHeroHealths) ?? new Dictionary<int, int>();
    }
    
    public void SetMaxHeroHealths(Dictionary<int, int> maxHealths)
    {
        MaxHeroHealths = System.Text.Json.JsonSerializer.Serialize(maxHealths);
    }
}

public enum CombatStatus
{
    Preparing = 0,
    InProgress = 1,
    Victory = 2,
    Defeat = 3,
    Cancelled = 4
}

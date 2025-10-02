namespace RpgQuestManager.Api.Models;

/// <summary>
/// Registra quando um usuário descobre uma fraqueza de boss
/// </summary>
public class ComboDiscovery
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EnemyId { get; set; }
    public int ComboId { get; set; }
    
    public DateTime DiscoveredAt { get; set; } = DateTime.UtcNow;
    public int TimesUsed { get; set; } = 0; // Quantas vezes usou este combo contra este boss
    public int TimesWon { get; set; } = 0; // Quantas vezes venceu
    
    // Relacionamentos
    public User User { get; set; } = null!;
    public Enemy Enemy { get; set; } = null!;
    public PartyCombo Combo { get; set; } = null!;
}


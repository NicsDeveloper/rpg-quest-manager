using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.DTOs.Combat;

public class CombatSessionDto
{
    public int Id { get; set; }
    public List<int> HeroIds { get; set; } = new List<int>();
    public string HeroNames { get; set; } = string.Empty; // Nomes concatenados
    public int QuestId { get; set; }
    public string QuestName { get; set; } = string.Empty;
    public int? CurrentEnemyId { get; set; }
    public string? CurrentEnemyName { get; set; }
    public int? ComboId { get; set; }
    public string? ComboName { get; set; }
    public int GroupBonus { get; set; }
    public int ComboBonus { get; set; }
    public CombatStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

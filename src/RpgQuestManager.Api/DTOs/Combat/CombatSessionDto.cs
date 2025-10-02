using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.DTOs.Combat;

public class CombatSessionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<int> HeroIds { get; set; } = new();
    public int QuestId { get; set; }
    public string QuestName { get; set; } = string.Empty;
    public int CurrentEnemyId { get; set; }
    public string CurrentEnemyName { get; set; } = string.Empty;
    public CombatStatus Status { get; set; }
    public bool IsHeroTurn { get; set; }
    public int CurrentEnemyHealth { get; set; }
    public int MaxEnemyHealth { get; set; }
    public Dictionary<int, int> HeroHealths { get; set; } = new();
    public Dictionary<int, int> MaxHeroHealths { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

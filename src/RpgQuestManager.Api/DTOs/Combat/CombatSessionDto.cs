namespace RpgQuestManager.Api.DTOs.Combat;

public class CombatSessionDto
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public int QuestId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}


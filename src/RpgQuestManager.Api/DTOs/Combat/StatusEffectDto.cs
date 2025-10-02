namespace RpgQuestManager.Api.DTOs.Combat;

public class StatusEffectDto
{
    public int Id { get; set; }
    public int CombatSessionId { get; set; }
    public int? HeroId { get; set; }
    public int? EnemyId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int Intensity { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime ExpiresAt { get; set; }
}

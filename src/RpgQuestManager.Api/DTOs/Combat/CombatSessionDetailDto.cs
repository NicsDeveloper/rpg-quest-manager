namespace RpgQuestManager.Api.DTOs.Combat;

public class CombatSessionDetailDto
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public int QuestId { get; set; }
    public string QuestName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public List<EnemyInfoDto> Enemies { get; set; } = new List<EnemyInfoDto>();
    public List<CombatLogDto> CombatLogs { get; set; } = new List<CombatLogDto>();
}

public class EnemyInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string RequiredDiceType { get; set; } = string.Empty;
    public int MinimumRoll { get; set; }
    public bool IsBoss { get; set; }
}

public class CombatLogDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? DiceUsed { get; set; }
    public int? DiceResult { get; set; }
    public int? RequiredRoll { get; set; }
    public bool? Success { get; set; }
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}


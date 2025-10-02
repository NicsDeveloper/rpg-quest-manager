using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.DTOs.Enemies;
using RpgQuestManager.Api.DTOs.Heroes;
using RpgQuestManager.Api.DTOs.Quests;

namespace RpgQuestManager.Api.DTOs.Combat;

public class CombatLogDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public int? EnemyId { get; set; }
    public string? EnemyName { get; set; }
    public DiceType? DiceUsed { get; set; }
    public int? DiceResult { get; set; }
    public int? RequiredRoll { get; set; }
    public bool? Success { get; set; }
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class CombatSessionDetailDto
{
    public int Id { get; set; }
    public int HeroId => HeroIds.FirstOrDefault(); // Compatibilidade com frontend antigo
    public List<int> HeroIds { get; set; } = new List<int>();
    public List<HeroDto> Heroes { get; set; } = new List<HeroDto>(); // Detalhes dos heróis
    public int QuestId { get; set; }
    public string QuestName { get; set; } = string.Empty;
    public EnemyDto? CurrentEnemy { get; set; }
    public int? ComboId { get; set; }
    public string? ComboName { get; set; }
    public string? ComboDescription { get; set; }
    public int GroupBonus { get; set; }
    public int ComboBonus { get; set; }
    public CombatStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<CombatLogDto> CombatLogs { get; set; } = new List<CombatLogDto>();
    public List<EnemyDto> RemainingEnemies { get; set; } = new List<EnemyDto>();
}

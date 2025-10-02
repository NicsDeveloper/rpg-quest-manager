namespace RpgQuestManager.Api.Models;

/// <summary>
/// Sessão de combate ativa (quest em andamento)
/// </summary>
public class CombatSession
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public int QuestId { get; set; }
    public int? CurrentEnemyId { get; set; }
    
    public CombatStatus Status { get; set; } = CombatStatus.InProgress;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    
    // Log de ações realizadas
    public List<CombatLog> CombatLogs { get; set; } = new List<CombatLog>();
    
    // Relacionamentos
    public Hero Hero { get; set; } = null!;
    public Quest Quest { get; set; } = null!;
    public Enemy? CurrentEnemy { get; set; }
}

/// <summary>
/// Status da sessão de combate
/// </summary>
public enum CombatStatus
{
    InProgress,    // Em andamento
    Victory,       // Vitória (todos os bosses derrotados)
    Fled,          // Fugiu do combate
    Defeated       // Derrotado
}

/// <summary>
/// Log de ações durante o combate
/// </summary>
public class CombatLog
{
    public int Id { get; set; }
    public int CombatSessionId { get; set; }
    public int? EnemyId { get; set; }
    
    public string Action { get; set; } = string.Empty; // "ROLL", "VICTORY", "DEFEAT", "FLEE"
    public DiceType? DiceUsed { get; set; }
    public int? DiceResult { get; set; }
    public int? RequiredRoll { get; set; }
    public bool? Success { get; set; }
    
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public CombatSession CombatSession { get; set; } = null!;
    public Enemy? Enemy { get; set; }
}


namespace RpgQuestManager.Api.Models;

/// <summary>
/// Sessão de combate ativa (quest em andamento)
/// </summary>
public class CombatSession
{
    public int Id { get; set; }
    public string HeroIds { get; set; } = string.Empty; // IDs separados por vírgula (ex: "1,3,5")
    public int QuestId { get; set; }
    public int? CurrentEnemyId { get; set; }
    
    // Sistema de Combos
    public int? ComboId { get; set; } // Combo detectado
    public int GroupBonus { get; set; } = 0; // Bônus de força do grupo
    public int ComboBonus { get; set; } = 0; // Bônus do combo/fraqueza
    
    public CombatStatus Status { get; set; } = CombatStatus.InProgress;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    
    // Log de ações realizadas
    public List<CombatLog> CombatLogs { get; set; } = new List<CombatLog>();
    
    // Relacionamentos
    public Quest Quest { get; set; } = null!;
    public Enemy? CurrentEnemy { get; set; }
    public PartyCombo? Combo { get; set; }
    
    // Helper methods
    public List<int> GetHeroIdsList()
    {
        return HeroIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();
    }
    
    public void SetHeroIdsList(List<int> heroIds)
    {
        HeroIds = string.Join(",", heroIds);
    }
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


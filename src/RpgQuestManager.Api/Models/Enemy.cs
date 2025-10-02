namespace RpgQuestManager.Api.Models;

public class Enemy
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Goblin, Dragão, Orc, etc.
    public int Power { get; set; }
    public int Health { get; set; }
    
    // Sistema de Dados
    public DiceType RequiredDiceType { get; set; } = DiceType.D6; // Tipo de dado necessário
    public int MinimumRoll { get; set; } = 4; // Valor mínimo para vencer (ex: 4+ no d6)
    public CombatType CombatType { get; set; } = CombatType.Physical; // Tipo de combate (Physical, Magical, Agile)
    
    public bool IsBoss { get; set; } = false; // Se é um boss (tem drops especiais)
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public ICollection<QuestEnemy> QuestEnemies { get; set; } = new List<QuestEnemy>();
    public ICollection<BossDropTable> BossDrops { get; set; } = new List<BossDropTable>();
}

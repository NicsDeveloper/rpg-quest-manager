namespace RpgQuestManager.Api.Models;

public class QuestEnemy
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public int EnemyId { get; set; }
    public int Quantity { get; set; } = 1;
    
    // Relacionamentos
    public Quest Quest { get; set; } = null!;
    public Enemy Enemy { get; set; } = null!;
}


namespace RpgQuestManager.Api.Models;

public class Enemy
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Goblin, Dragão, Orc, etc.
    public int Power { get; set; }
    public int Health { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public ICollection<QuestEnemy> QuestEnemies { get; set; } = new List<QuestEnemy>();
}


namespace RpgQuestManager.Api.Models;

public class HeroQuest
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public int QuestId { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    
    // Relacionamentos
    public Hero Hero { get; set; } = null!;
    public Quest Quest { get; set; } = null!;
}


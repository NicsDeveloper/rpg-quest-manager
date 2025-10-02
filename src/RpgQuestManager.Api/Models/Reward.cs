namespace RpgQuestManager.Api.Models;

public class Reward
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public int Gold { get; set; }
    public int Experience { get; set; }
    public string? ItemName { get; set; }
    public string? ItemDescription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public Quest Quest { get; set; } = null!;
}


namespace RpgQuestManager.Api.Models;

public class Reward
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public int Gold { get; set; }
    public int Experience { get; set; }
    public int? ItemId { get; set; }
    public int ItemQuantity { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Quest Quest { get; set; } = null!;
    public Item? Item { get; set; }
}


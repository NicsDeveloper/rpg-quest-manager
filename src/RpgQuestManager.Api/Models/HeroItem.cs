namespace RpgQuestManager.Api.Models;

public class HeroItem
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsEquipped { get; set; } = false;
    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public Hero Hero { get; set; } = null!;
    public Item Item { get; set; } = null!;
}


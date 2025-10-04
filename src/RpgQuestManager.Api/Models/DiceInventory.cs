namespace RpgQuestManager.Api.Models;

public enum DiceType
{
    D4 = 4,
    D6 = 6,
    D8 = 8,
    D10 = 10,
    D12 = 12,
    D20 = 20
}

public class DiceInventory
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public DiceType DiceType { get; set; }
    public int Quantity { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navegação
    public Hero Hero { get; set; } = null!;
}

public class DiceShopItem
{
    public int Id { get; set; }
    public DiceType DiceType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Price { get; set; }
    public int MaxQuantityPerPurchase { get; set; } = 1;
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

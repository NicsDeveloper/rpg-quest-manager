namespace RpgQuestManager.Api.DTOs.Shop;

public class ShopItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public int Value { get; set; }
    public int BonusStrength { get; set; }
    public int BonusIntelligence { get; set; }
    public int BonusDexterity { get; set; }
    public int Quantity { get; set; } = 0; // Quantidade no inventário
    public bool IsEquipped { get; set; } = false;
    public bool IsConsumable { get; set; } = false;
    public decimal? PercentageXpBonus { get; set; } // Para poções de XP
}

namespace RpgQuestManager.Api.DTOs.Shop;

public class ShopPurchaseResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int TotalCost { get; set; }
    public int RemainingGold { get; set; }
}

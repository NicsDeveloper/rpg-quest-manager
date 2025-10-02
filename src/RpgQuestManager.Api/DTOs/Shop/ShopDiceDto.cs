namespace RpgQuestManager.Api.DTOs.Shop;

public class ShopDiceDto
{
    public string Type { get; set; } = string.Empty;
    public int Price { get; set; }
    public int Owned { get; set; }
    public string Description { get; set; } = string.Empty;
}

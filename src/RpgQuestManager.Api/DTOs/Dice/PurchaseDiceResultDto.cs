namespace RpgQuestManager.Api.DTOs.Dice;

public class PurchaseDiceResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int RemainingGold { get; set; }
    public DiceInventoryDto UpdatedInventory { get; set; } = new DiceInventoryDto();
}


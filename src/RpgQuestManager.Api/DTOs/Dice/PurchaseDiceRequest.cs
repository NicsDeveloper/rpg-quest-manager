namespace RpgQuestManager.Api.DTOs.Dice;

public class PurchaseDiceRequest
{
    public int HeroId { get; set; }
    public string DiceType { get; set; } = string.Empty; // "D6", "D8", "D12", "D20"
    public int Quantity { get; set; } = 1;
}


namespace RpgQuestManager.Api.DTOs.Dice;

public class PurchaseDiceRequest
{
    // UserId é obtido do token JWT, não precisa mais enviar HeroId
    public string DiceType { get; set; } = string.Empty; // "D6", "D10", "D12", "D20"
    public int Quantity { get; set; } = 1;
}


namespace RpgQuestManager.Api.DTOs.Dice;

public class DiceInventoryDto
{
    public int UserId { get; set; }
    public int D6Count { get; set; }
    public int D10Count { get; set; }
    public int D12Count { get; set; }
    public int D20Count { get; set; }
}


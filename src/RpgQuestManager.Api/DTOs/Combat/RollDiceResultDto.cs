namespace RpgQuestManager.Api.DTOs.Combat;

public class RollDiceResultDto
{
    public bool Success { get; set; }
    public int RollResult { get; set; }
    public string DiceType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}


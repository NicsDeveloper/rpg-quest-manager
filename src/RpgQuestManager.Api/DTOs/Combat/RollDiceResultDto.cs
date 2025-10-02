namespace RpgQuestManager.Api.DTOs.Combat;

public class RollDiceResultDto
{
    public int Roll { get; set; }
    public int RequiredRoll { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public CombatSessionDetailDto UpdatedCombatSession { get; set; } = null!;
}

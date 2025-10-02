namespace RpgQuestManager.Api.DTOs.Combat;

public class RollDiceRequest
{
    public int CombatSessionId { get; set; }
    public int EnemyId { get; set; }
    public string DiceType { get; set; } = string.Empty; // "D6", "D8", "D12", "D20"
}


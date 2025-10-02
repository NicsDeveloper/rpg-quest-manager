using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.DTOs.Combat;

public class RollDiceRequest
{
    public int CombatSessionId { get; set; }
    public DiceType DiceType { get; set; }
}

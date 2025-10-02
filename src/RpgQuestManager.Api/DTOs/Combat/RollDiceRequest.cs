using RpgQuestManager.Api.Models;
using System.Text.Json.Serialization;

namespace RpgQuestManager.Api.DTOs.Combat;

public class RollDiceRequest
{
    public int CombatSessionId { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DiceType DiceType { get; set; }
}

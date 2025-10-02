using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.DTOs.Enemies;

public class EnemyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Power { get; set; }
    public int Health { get; set; }
    public DiceType RequiredDiceType { get; set; }
    public int MinimumRoll { get; set; }
    public CombatType CombatType { get; set; }
    public bool IsBoss { get; set; }
    public DateTime CreatedAt { get; set; }
}


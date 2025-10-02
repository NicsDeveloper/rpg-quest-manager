namespace RpgQuestManager.Api.DTOs.Combat;

public class CombatItemUsageResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int XpGained { get; set; }
    public bool LeveledUp { get; set; }
    public int? NewLevel { get; set; }
    public int CombatBonus { get; set; } // Bônus para reduzir RequiredRoll
}

namespace RpgQuestManager.Api.DTOs.Items;

public class ItemUsageResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int QuantityUsed { get; set; }
    public int XpGained { get; set; }
    public bool LeveledUp { get; set; }
    public int? NewLevel { get; set; }
}

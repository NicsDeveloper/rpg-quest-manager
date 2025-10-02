namespace RpgQuestManager.Api.DTOs.Combat;

public class CompleteCombatResultDto
{
    public string Status { get; set; } = string.Empty; // "Victory", "Fled", "Defeated"
    public List<DroppedItemDto> DroppedItems { get; set; } = new List<DroppedItemDto>();
    public string Message { get; set; } = string.Empty;
}

public class DroppedItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int BonusStrength { get; set; }
    public int BonusIntelligence { get; set; }
    public int BonusDexterity { get; set; }
}


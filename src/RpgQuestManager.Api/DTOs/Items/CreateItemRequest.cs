namespace RpgQuestManager.Api.DTOs.Items;

public class CreateItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int BonusStrength { get; set; } = 0;
    public int BonusIntelligence { get; set; } = 0;
    public int BonusDexterity { get; set; } = 0;
    public int Value { get; set; } = 0;
}


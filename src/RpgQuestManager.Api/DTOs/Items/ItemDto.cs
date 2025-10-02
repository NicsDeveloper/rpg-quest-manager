namespace RpgQuestManager.Api.DTOs.Items;

public class ItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int BonusStrength { get; set; }
    public int BonusIntelligence { get; set; }
    public int BonusDexterity { get; set; }
    public int Value { get; set; }
    public DateTime CreatedAt { get; set; }
}


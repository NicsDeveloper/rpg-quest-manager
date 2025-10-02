using RpgQuestManager.Api.DTOs.Quests;

namespace RpgQuestManager.Api.DTOs.Heroes;

public class HeroItemDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsEquipped { get; set; }
    public DateTime AcquiredAt { get; set; }
    public int BonusStrength { get; set; }
    public int BonusIntelligence { get; set; }
    public int BonusDexterity { get; set; }
}

public class HeroProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public int Dexterity { get; set; }
    public int Gold { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<HeroItemDto> Items { get; set; } = new List<HeroItemDto>();
    public List<QuestDto> CompletedQuests { get; set; } = new List<QuestDto>();
}


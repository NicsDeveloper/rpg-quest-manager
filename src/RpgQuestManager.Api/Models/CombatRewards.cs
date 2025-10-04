using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Models;

public enum RewardType
{
    Gold,
    Experience,
    Item,
    Dice,
    Achievement
}

public class CombatReward
{
    public int Id { get; set; }
    public int CombatRewardsId { get; set; }
    public RewardType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public int? ItemId { get; set; }
    public DiceType? DiceType { get; set; }
    public int? GoldAmount { get; set; }
    public int? ExperienceAmount { get; set; }
    public string Icon { get; set; } = string.Empty;
    public ItemRarity? Rarity { get; set; }
}

public class CombatRewards
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public int QuestId { get; set; }
    public int MonsterId { get; set; }
    public bool IsClaimed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClaimedAt { get; set; }
    
    // Navegação
    public Hero Hero { get; set; } = null!;
    public Quest Quest { get; set; } = null!;
    public Monster Monster { get; set; } = null!;
    public List<CombatReward> Rewards { get; set; } = new();
}

public class QuestReward
{
    public int Id { get; set; }
    public int QuestRewardsId { get; set; }
    public RewardType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public int? ItemId { get; set; }
    public DiceType? DiceType { get; set; }
    public int? GoldAmount { get; set; }
    public int? ExperienceAmount { get; set; }
    public string Icon { get; set; } = string.Empty;
    public ItemRarity? Rarity { get; set; }
}

public class QuestRewards
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public int QuestId { get; set; }
    public bool IsClaimed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClaimedAt { get; set; }
    
    // Navegação
    public Hero Hero { get; set; } = null!;
    public Quest Quest { get; set; } = null!;
    public List<QuestReward> Rewards { get; set; } = new();
}

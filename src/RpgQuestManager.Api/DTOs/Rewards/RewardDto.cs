using RpgQuestManager.Api.DTOs.Items;

namespace RpgQuestManager.Api.DTOs.Rewards;

public class RewardDto
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public int Gold { get; set; }
    public int Experience { get; set; }
    public int? ItemId { get; set; }
    public int ItemQuantity { get; set; }
    public ItemDto? Item { get; set; }
    public DateTime CreatedAt { get; set; }
}


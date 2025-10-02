namespace RpgQuestManager.Api.DTOs.Rewards;

public class CreateRewardRequest
{
    public int QuestId { get; set; }
    public int Gold { get; set; }
    public int Experience { get; set; }
    public int? ItemId { get; set; }
    public int ItemQuantity { get; set; } = 1;
}


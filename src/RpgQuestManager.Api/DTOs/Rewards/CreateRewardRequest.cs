namespace RpgQuestManager.Api.DTOs.Rewards;

public class CreateRewardRequest
{
    public int QuestId { get; set; }
    public int Gold { get; set; }
    public int Experience { get; set; }
    public string? ItemName { get; set; }
    public string? ItemDescription { get; set; }
}


namespace RpgQuestManager.Api.DTOs.Quests;

public class CreateQuestRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int ExperienceReward { get; set; }
    public int GoldReward { get; set; }
}


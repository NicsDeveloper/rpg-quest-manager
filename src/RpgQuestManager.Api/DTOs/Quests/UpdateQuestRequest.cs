namespace RpgQuestManager.Api.DTOs.Quests;

public class UpdateQuestRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Difficulty { get; set; }
    public string? RequiredClass { get; set; }
    public int? RequiredLevel { get; set; }
    public int? ExperienceReward { get; set; }
    public int? GoldReward { get; set; }
}

namespace RpgQuestManager.Api.DTOs.Quests;

public class QuestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string RequiredClass { get; set; } = "Any";
    public int RequiredLevel { get; set; }
    public int ExperienceReward { get; set; }
    public int GoldReward { get; set; }
    public bool IsRepeatable { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool? IsAccepted { get; set; }
    public bool? CanAccept { get; set; }
}

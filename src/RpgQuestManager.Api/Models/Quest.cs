namespace RpgQuestManager.Api.Models;

public class Quest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EnvironmentType Environment { get; set; } = EnvironmentType.Forest;
    public string TargetMonsterName { get; set; } = string.Empty;
    public MonsterType TargetMonsterType { get; set; } = MonsterType.Goblin;
    public QuestStatus Status { get; set; } = QuestStatus.NotStarted;
    public int ExperienceReward { get; set; } = 100;
    public int? RequiredLevel { get; set; }
    public List<string> Prerequisites { get; set; } = new();
}



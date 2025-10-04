namespace RpgQuestManager.Api.Models;

public class Quest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IntroductionText { get; set; } = string.Empty;
    public string CompletionText { get; set; } = string.Empty;
    public QuestCategory Category { get; set; } = QuestCategory.MainStory;
    public QuestDifficulty Difficulty { get; set; } = QuestDifficulty.Easy;
    public EnvironmentType Environment { get; set; } = EnvironmentType.Forest;
    public string TargetMonsterName { get; set; } = string.Empty;
    public MonsterType TargetMonsterType { get; set; } = MonsterType.Goblin;
    public QuestStatus Status { get; set; } = QuestStatus.NotStarted;
    public int ExperienceReward { get; set; } = 100;
    public int GoldReward { get; set; } = 0;
    public int? RequiredLevel { get; set; }
    public List<string> Prerequisites { get; set; } = new();
    public List<string> Rewards { get; set; } = new();
    public bool IsRepeatable { get; set; } = false;
    public int EstimatedDuration { get; set; } = 30; // em minutos
}



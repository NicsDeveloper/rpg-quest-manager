namespace RpgQuestManager.Api.Models;

public class Quest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "Main";
    public string Difficulty { get; set; } = string.Empty;
    public string RequiredClass { get; set; } = "Any";
    public int RequiredLevel { get; set; } = 1;
    public int ExperienceReward { get; set; }
    public int GoldReward { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<HeroQuest> HeroQuests { get; set; } = new List<HeroQuest>();
    public ICollection<QuestEnemy> QuestEnemies { get; set; } = new List<QuestEnemy>();
    public ICollection<Reward> Rewards { get; set; } = new List<Reward>();
}

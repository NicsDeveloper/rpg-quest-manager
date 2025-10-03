namespace RpgQuestManager.Api.Models;

public class Monster
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public MonsterType Type { get; set; } = MonsterType.Goblin;
    public MonsterRank Rank { get; set; } = MonsterRank.Normal;
    public EnvironmentType Habitat { get; set; } = EnvironmentType.Forest;
    public int Health { get; set; } = 50;
    public int MaxHealth { get; set; } = 50;
    public int Attack { get; set; } = 8;
    public int Defense { get; set; } = 3;
    public int ExperienceReward { get; set; } = 25;
    public List<StatusEffectType> StatusEffects { get; set; } = new();
}



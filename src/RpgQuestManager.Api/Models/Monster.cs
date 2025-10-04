namespace RpgQuestManager.Api.Models;

public class Monster
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MonsterType Type { get; set; } = MonsterType.Goblin;
    public MonsterRank Rank { get; set; } = MonsterRank.Normal;
    public MonsterSize Size { get; set; } = MonsterSize.Medium;
    public EnvironmentType Habitat { get; set; } = EnvironmentType.Forest;
    public int Health { get; set; } = 50;
    public int MaxHealth { get; set; } = 50;
    public int Attack { get; set; } = 8;
    public int Defense { get; set; } = 3;
    public int ExperienceReward { get; set; } = 25;
    public int Level { get; set; } = 1;
    public List<StatusEffectType> StatusEffects { get; set; } = new();
    public List<StatusEffectType> SpecialAbilities { get; set; } = new();
    public int SpecialAbilityChance { get; set; } = 20;
    public string TauntMessage { get; set; } = string.Empty;
    public string VictoryMessage { get; set; } = string.Empty;
    public string DefeatMessage { get; set; } = string.Empty;
}



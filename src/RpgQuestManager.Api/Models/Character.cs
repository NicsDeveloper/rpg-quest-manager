namespace RpgQuestManager.Api.Models;

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int NextLevelExperience { get; set; } = 1000;
    public int Health { get; set; } = 100;
    public int MaxHealth { get; set; } = 100;
    public int Attack { get; set; } = 10;
    public int Defense { get; set; } = 5;
    public int Morale { get; set; } = 50;
    public List<StatusEffectType> StatusEffects { get; set; } = new();
}



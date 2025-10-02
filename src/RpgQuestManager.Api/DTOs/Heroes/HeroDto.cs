namespace RpgQuestManager.Api.DTOs.Heroes;

public class HeroDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public int Dexterity { get; set; }
    public int Gold { get; set; }
    public DateTime CreatedAt { get; set; }
}


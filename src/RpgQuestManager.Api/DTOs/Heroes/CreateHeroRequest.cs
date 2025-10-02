namespace RpgQuestManager.Api.DTOs.Heroes;

public class CreateHeroRequest
{
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int Strength { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
}


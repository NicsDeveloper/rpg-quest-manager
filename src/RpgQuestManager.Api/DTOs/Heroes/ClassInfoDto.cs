namespace RpgQuestManager.Api.DTOs.Heroes;

public class ClassInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BaseStrength { get; set; }
    public int BaseIntelligence { get; set; }
    public int BaseDexterity { get; set; }
    public string CombatFocus { get; set; } = string.Empty;
    public string RecommendedFor { get; set; } = string.Empty;
}

namespace RpgQuestManager.Api.DTOs.Heroes;

public class AttributeAllocationResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int HeroId { get; set; }
    public int NewStrength { get; set; }
    public int NewIntelligence { get; set; }
    public int NewDexterity { get; set; }
    public int RemainingPoints { get; set; }
    public int NewTotalAttack { get; set; }
    public int NewTotalDefense { get; set; }
    public int NewTotalMagic { get; set; }
}

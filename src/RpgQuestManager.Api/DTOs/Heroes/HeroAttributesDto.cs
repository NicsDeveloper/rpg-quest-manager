namespace RpgQuestManager.Api.DTOs.Heroes;

public class HeroAttributesDto
{
    public int HeroId { get; set; }
    public string HeroName { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int Level { get; set; }
    
    // Atributos totais (base + bônus)
    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public int Dexterity { get; set; }
    
    // Atributos base da classe (não editáveis)
    public int BaseStrength { get; set; }
    public int BaseIntelligence { get; set; }
    public int BaseDexterity { get; set; }
    
    // Bônus adicionais (editáveis)
    public int BonusStrength { get; set; }
    public int BonusIntelligence { get; set; }
    public int BonusDexterity { get; set; }
    
    public int UnallocatedPoints { get; set; }
    public int TotalAttack { get; set; }
    public int TotalDefense { get; set; }
    public int TotalMagic { get; set; }
}

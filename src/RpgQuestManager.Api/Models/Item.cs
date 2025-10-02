namespace RpgQuestManager.Api.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Espada, Poção, Escudo, Armadura
    public int BonusStrength { get; set; } = 0;
    public int BonusIntelligence { get; set; } = 0;
    public int BonusDexterity { get; set; } = 0;
    public int Value { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public ICollection<HeroItem> HeroItems { get; set; } = new List<HeroItem>();
}


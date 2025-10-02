namespace RpgQuestManager.Api.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Espada, Poção, Escudo, Armadura
    
    // Sistema de Raridade com Tiers
    public ItemRarity Rarity { get; set; } = ItemRarity.Common;
    public int RarityTier { get; set; } = 0; // 0 para Common/Rare, 1-3 para Epic/Legendary
    
    public int BonusStrength { get; set; } = 0;
    public int BonusIntelligence { get; set; } = 0;
    public int BonusDexterity { get; set; } = 0;
    public int Value { get; set; } = 0;
    
    public bool IsExclusiveDrop { get; set; } = false; // Se é exclusivo de um boss específico
    public bool IsConsumable { get; set; } = false; // Se é consumível (poções, etc)
    public decimal? PercentageXpBonus { get; set; } // Para poções de XP (ex: 0.05 = 5%)
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public ICollection<HeroItem> HeroItems { get; set; } = new List<HeroItem>();
    public ICollection<BossDropTable> BossDrops { get; set; } = new List<BossDropTable>();
    
    /// <summary>
    /// Retorna o nome completo da raridade (ex: "Épico III")
    /// </summary>
    public string GetFullRarityName()
    {
        if (RarityTier == 0)
            return Rarity.ToString();
        
        var tierRoman = RarityTier switch
        {
            1 => "I",
            2 => "II",
            3 => "III",
            _ => ""
        };
        
        return $"{Rarity} {tierRoman}";
    }
}

/// <summary>
/// Raridades de itens
/// </summary>
public enum ItemRarity
{
    Common,     // Comum - 55%
    Rare,       // Raro - 30%
    Epic,       // Épico (I, II, III) - 12%
    Legendary   // Lendário (I, II, III) - 3%
}

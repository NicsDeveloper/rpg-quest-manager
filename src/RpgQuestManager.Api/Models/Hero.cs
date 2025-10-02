namespace RpgQuestManager.Api.Models;

public class Hero
{
    public const int MaxLevel = 20;
    
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty; // ex: Guerreiro, Mago, Arqueiro
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int Strength { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Gold { get; set; } = 0;
    public int? UserId { get; set; }
    
    // Sistema de Party
    public bool IsInActiveParty { get; set; } = false; // Se está na party ativa (máx 3)
    public int? PartySlot { get; set; } // Slot na party: 1, 2, 3 ou null
    
    // Soft Delete (Recuperação de 7 dias)
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public User? User { get; set; }
    public ICollection<HeroQuest> HeroQuests { get; set; } = new List<HeroQuest>();
    public ICollection<HeroItem> HeroItems { get; set; } = new List<HeroItem>();
    
    /// <summary>
    /// Calcula XP necessário para o próximo nível usando progressão exponencial
    /// Fórmula: 100 * (1.5 ^ (Level - 1))
    /// </summary>
    public int GetExperienceForNextLevel()
    {
        if (Level >= MaxLevel) return int.MaxValue; // Nível máximo alcançado
        
        // Progressão exponencial: 100, 150, 225, 337, 506, 759, 1138, 1707, 2561, 3841...
        return (int)Math.Ceiling(100 * Math.Pow(1.5, Level - 1));
    }
    
    /// <summary>
    /// Verifica se o herói pode subir de nível
    /// </summary>
    public bool CanLevelUp()
    {
        return Level < MaxLevel && Experience >= GetExperienceForNextLevel();
    }
    
    /// <summary>
    /// Verifica se o herói atingiu o nível máximo
    /// </summary>
    public bool IsMaxLevel()
    {
        return Level >= MaxLevel;
    }
    
    /// <summary>
    /// Calcula recompensa de ouro ao subir de nível (escalada com o nível)
    /// </summary>
    public int GetLevelUpGoldReward()
    {
        // Recompensa escalada: Nível * 75 ouro
        return Level * 75;
    }
    
    /// <summary>
    /// Calcula bônus de atributos ao subir de nível (aumenta com o nível)
    /// </summary>
    public int GetLevelUpAttributeBonus()
    {
        // Níveis 1-5: +2, Níveis 6-10: +3, Níveis 11-15: +4, Níveis 16-20: +5
        if (Level <= 5) return 2;
        if (Level <= 10) return 3;
        if (Level <= 15) return 4;
        return 5;
    }
    
    /// <summary>
    /// Sobe de nível com recompensas escaladas
    /// </summary>
    public void LevelUp()
    {
        if (!CanLevelUp()) return;
        
        var xpNeeded = GetExperienceForNextLevel();
        Level++;
        Experience -= xpNeeded;
        
        // Recompensa de atributos escalada
        var attributeBonus = GetLevelUpAttributeBonus();
        Strength += attributeBonus;
        Intelligence += attributeBonus;
        Dexterity += attributeBonus;
        
        // Recompensa de ouro escalada
        Gold += GetLevelUpGoldReward();
        
        // Continua subindo enquanto tiver XP suficiente e não estiver no máximo
        if (CanLevelUp())
        {
            LevelUp();
        }
    }
}


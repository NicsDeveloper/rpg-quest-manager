namespace RpgQuestManager.Api.Models;

/// <summary>
/// Tipos de combate que afetam qual atributo é usado
/// </summary>
public enum CombatType
{
    Physical, // Usa Strength
    Magical,  // Usa Intelligence  
    Agile     // Usa Dexterity
}

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
    public int UnallocatedAttributePoints { get; set; } = 0; // Pontos de atributo não alocados
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
    /// Calcula o ataque total do herói (baseado em Strength + bônus de itens equipados)
    /// </summary>
    public int GetTotalAttack()
    {
        var equippedItems = HeroItems.Where(hi => hi.IsEquipped);
        var itemBonus = equippedItems.Sum(hi => hi.Item.BonusStrength);
        return Strength + itemBonus;
    }

    /// <summary>
    /// Calcula a defesa total do herói (baseado em Dexterity + bônus de itens equipados)
    /// </summary>
    public int GetTotalDefense()
    {
        var equippedItems = HeroItems.Where(hi => hi.IsEquipped);
        var itemBonus = equippedItems.Sum(hi => hi.Item.BonusDexterity);
        return Dexterity + itemBonus;
    }

    /// <summary>
    /// Calcula a magia total do herói (baseado em Intelligence + bônus de itens equipados)
    /// </summary>
    public int GetTotalMagic()
    {
        var equippedItems = HeroItems.Where(hi => hi.IsEquipped);
        var itemBonus = equippedItems.Sum(hi => hi.Item.BonusIntelligence);
        return Intelligence + itemBonus;
    }

    /// <summary>
    /// Calcula o bônus de rolagem baseado nos atributos do herói
    /// Cada 5 pontos de atributo relevante = -1 no roll necessário
    /// </summary>
    public int GetCombatBonus(CombatType combatType)
    {
        var relevantStat = combatType switch
        {
            CombatType.Physical => GetTotalAttack(),
            CombatType.Magical => GetTotalMagic(),
            CombatType.Agile => GetTotalDefense(),
            _ => GetTotalAttack()
        };

        return -(relevantStat / 5); // Cada 5 pontos = -1 no roll
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
        
        // MUDANÇA: Ao invés de aplicar atributos automaticamente, dá pontos não alocados
        var attributePoints = GetLevelUpAttributeBonus();
        UnallocatedAttributePoints += attributePoints;
        
        // Recompensa de ouro escalada
        Gold += GetLevelUpGoldReward();
        
        // Continua subindo enquanto tiver XP suficiente e não estiver no máximo
        if (CanLevelUp())
        {
            LevelUp();
        }
    }
    
    /// <summary>
    /// Obtém os atributos base para cada classe
    /// </summary>
    public static (int Strength, int Intelligence, int Dexterity) GetBaseAttributesForClass(string heroClass)
    {
        return heroClass switch
        {
            "Guerreiro" => (18, 8, 12),    // Foco em força
            "Mago" => (6, 20, 10),          // Foco em inteligência
            "Arqueiro" => (10, 10, 18),     // Foco em destreza
            "Paladino" => (15, 12, 10),     // Balanceado força/int
            "Ladino" => (10, 8, 20),        // Foco extremo em destreza
            _ => (10, 10, 10)               // Padrão balanceado
        };
    }
    
    /// <summary>
    /// Distribui pontos de atributo não alocados
    /// </summary>
    public bool AllocateAttributePoints(int strengthPoints, int intelligencePoints, int dexterityPoints)
    {
        var totalPoints = strengthPoints + intelligencePoints + dexterityPoints;
        
        if (totalPoints > UnallocatedAttributePoints)
            return false; // Não tem pontos suficientes
        
        if (strengthPoints < 0 || intelligencePoints < 0 || dexterityPoints < 0)
            return false; // Não pode alocar valores negativos
        
        Strength += strengthPoints;
        Intelligence += intelligencePoints;
        Dexterity += dexterityPoints;
        UnallocatedAttributePoints -= totalPoints;
        
        return true;
    }
}


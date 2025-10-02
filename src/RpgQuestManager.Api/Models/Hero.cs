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
    
    // Sistema de Vida
    public int MaxHealth { get; set; } = 100; // Vida máxima
    public int CurrentHealth { get; set; } = 100; // Vida atual
    
    // Atributos base da classe (não editáveis)
    public int BaseStrength { get; set; } = 10;
    public int BaseIntelligence { get; set; } = 10;
    public int BaseDexterity { get; set; } = 10;
    
    // Pontos adicionais distribuídos pelo jogador
    public int BonusStrength { get; set; } = 0;
    public int BonusIntelligence { get; set; } = 0;
    public int BonusDexterity { get; set; } = 0;
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
    /// Calcula o ataque total do herói (baseado em Strength + bônus de armas equipadas)
    /// </summary>
    public int GetTotalAttack()
    {
        var equippedItems = HeroItems.Where(hi => hi.IsEquipped);
        var weaponBonus = 0;
        
        foreach (var item in equippedItems)
        {
            var itemType = item.Item.Type.ToLower();
            
            // Armas: bônus de força para dano
            if (itemType.Contains("espada") || itemType.Contains("sword") ||
                itemType.Contains("arco") || itemType.Contains("bow") ||
                itemType.Contains("cajado") || itemType.Contains("staff") ||
                itemType.Contains("chicote") || itemType.Contains("whip") ||
                itemType.Contains("foice") || itemType.Contains("scythe") ||
                itemType.Contains("açoite") || itemType.Contains("flail") ||
                itemType.Contains("lâmina") || itemType.Contains("blade"))
            {
                weaponBonus += item.Item.BonusStrength;
            }
        }
        
        return Strength + weaponBonus;
    }
    
    /// <summary>
    /// Obtém o ataque base (sem itens)
    /// </summary>
    public int GetBaseAttack()
    {
        return BaseStrength + BonusStrength;
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
    /// Obtém a defesa base (sem itens)
    /// </summary>
    public int GetBaseDefense()
    {
        return BaseDexterity + BonusDexterity;
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
    /// Obtém a magia base (sem itens)
    /// </summary>
    public int GetBaseMagic()
    {
        return BaseIntelligence + BonusIntelligence;
    }

    /// <summary>
    /// Calcula a vida máxima baseada no nível, atributos e itens equipados
    /// </summary>
    public int CalculateMaxHealth()
    {
        // Vida base: 100 + (nível * 10) + (força * 2)
        var baseHealth = 100 + (Level * 10) + (Strength * 2);
        
        // Bônus de itens equipados
        var equippedItems = HeroItems.Where(hi => hi.IsEquipped);
        var armorBonus = 0;
        var accessoryBonus = 0;
        
        foreach (var item in equippedItems)
        {
            var itemType = item.Item.Type.ToLower();
            
            // Armaduras: 2x o bônus de força para vida
            if (itemType.Contains("armadura") || itemType.Contains("armor") || 
                itemType.Contains("cota") || itemType.Contains("mail") ||
                itemType.Contains("escama") || itemType.Contains("scale") ||
                itemType.Contains("manto") || itemType.Contains("cloak") ||
                itemType.Contains("capacete") || itemType.Contains("helmet"))
            {
                armorBonus += item.Item.BonusStrength * 2;
            }
            // Acessórios: 1x o bônus de força para vida
            else if (itemType.Contains("amuleto") || itemType.Contains("amulet") ||
                     itemType.Contains("anel") || itemType.Contains("ring") ||
                     itemType.Contains("coração") || itemType.Contains("heart") ||
                     itemType.Contains("coroa") || itemType.Contains("crown"))
            {
                accessoryBonus += item.Item.BonusStrength;
            }
        }
        
        return baseHealth + armorBonus + accessoryBonus;
    }

    /// <summary>
    /// Atualiza a vida máxima baseada nos atributos atuais
    /// </summary>
    public void UpdateMaxHealth()
    {
        var newMaxHealth = CalculateMaxHealth();
        MaxHealth = newMaxHealth;
        
        // Se a vida atual for maior que a nova vida máxima, ajusta
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }

    /// <summary>
    /// Aplica dano ao herói
    /// </summary>
    public void TakeDamage(int damage)
    {
        CurrentHealth = Math.Max(0, CurrentHealth - damage);
    }

    /// <summary>
    /// Cura o herói
    /// </summary>
    public void Heal(int healing)
    {
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + healing);
    }

    /// <summary>
    /// Verifica se o herói está vivo
    /// </summary>
    public bool IsAlive()
    {
        return CurrentHealth > 0;
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
        
        // Atualiza a vida máxima baseada no novo nível
        UpdateMaxHealth();
        
        // Recompensa de ouro escalada
        Gold += GetLevelUpGoldReward();
        
        // Continua subindo enquanto tiver XP suficiente e não estiver no máximo
        if (CanLevelUp())
        {
            LevelUp();
        }
    }
    
    /// <summary>
    /// Configura atributos iniciais baseados na classe do herói
    /// </summary>
    public void ConfigureInitialAttributes()
    {
        // Reset para valores base
        BaseStrength = 10;
        BaseIntelligence = 10;
        BaseDexterity = 10;
        
        // Reset bônus adicionais
        BonusStrength = 0;
        BonusIntelligence = 0;
        BonusDexterity = 0;
        
        // Aplica bônus baseado na classe
        switch (Class.ToLower())
        {
            case "guerreiro":
            case "warrior":
                BaseStrength += 8;    // 18 total
                BaseIntelligence += 2; // 12 total
                BaseDexterity += 4;   // 14 total
                break;
                
            case "mago":
            case "wizard":
            case "mage":
                BaseStrength += 0;    // 10 total
                BaseIntelligence += 12; // 22 total
                BaseDexterity += 6;   // 16 total
                break;
                
            case "arqueiro":
            case "archer":
            case "ranger":
                BaseStrength += 4;    // 14 total
                BaseIntelligence += 5; // 15 total
                BaseDexterity += 10;  // 20 total
                break;
                
            case "ladino":
            case "rogue":
            case "thief":
                BaseStrength += 2;    // 12 total
                BaseIntelligence += 4; // 14 total
                BaseDexterity += 8;   // 18 total
                break;
                
            case "paladino":
            case "paladin":
                BaseStrength += 6;    // 16 total
                BaseIntelligence += 8; // 18 total
                BaseDexterity += 4;   // 14 total
                break;
                
            case "clérigo":
            case "cleric":
                BaseStrength += 2;    // 12 total
                BaseIntelligence += 10; // 20 total
                BaseDexterity += 2;   // 12 total
                break;
                
            case "bárbaro":
            case "barbarian":
                BaseStrength += 10;   // 20 total
                BaseIntelligence += 0; // 10 total
                BaseDexterity += 2;   // 12 total
                break;
                
            default:
                // Classe não reconhecida - atributos balanceados
                BaseStrength += 3;    // 13 total
                BaseIntelligence += 3; // 13 total
                BaseDexterity += 3;   // 13 total
                break;
        }
        
        // Atualiza os atributos totais
        UpdateTotalAttributes();
        
        // Inicializa a vida baseada nos atributos
        UpdateMaxHealth();
        CurrentHealth = MaxHealth; // Começa com vida cheia
    }
    
    /// <summary>
    /// Atualiza os atributos totais baseado nos atributos base + bônus
    /// </summary>
    public void UpdateTotalAttributes()
    {
        Strength = BaseStrength + BonusStrength;
        Intelligence = BaseIntelligence + BonusIntelligence;
        Dexterity = BaseDexterity + BonusDexterity;
    }
    
    /// <summary>
    /// Distribui pontos de atributo não alocados (apenas bônus adicionais)
    /// </summary>
    public bool AllocateAttributePoints(int strengthPoints, int intelligencePoints, int dexterityPoints)
    {
        var totalPoints = strengthPoints + intelligencePoints + dexterityPoints;
        
        if (totalPoints > UnallocatedAttributePoints)
        {
            return false; // Não tem pontos suficientes
        }
        
        if (strengthPoints < 0 || intelligencePoints < 0 || dexterityPoints < 0)
        {
            return false; // Valores negativos não permitidos
        }
        
        // Aplica apenas aos bônus adicionais (não aos atributos base da classe)
        BonusStrength += strengthPoints;
        BonusIntelligence += intelligencePoints;
        BonusDexterity += dexterityPoints;
        UnallocatedAttributePoints -= totalPoints;
        
        // Atualiza os atributos totais
        UpdateTotalAttributes();
        
        return true;
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
    
}


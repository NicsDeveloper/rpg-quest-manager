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
    
    // Propriedades de combate
    public int Health { get => CurrentHealth; set => CurrentHealth = value; } // Alias para compatibilidade
    public int Defense { get; set; } = 10; // Defesa base
    public int Morale { get; set; } = 100; // Moral do herói
    
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
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    public HeroEquipment? Equipment { get; set; }

    /// <summary>
    /// Calcula o total de um atributo (base + bônus)
    /// </summary>
    public int GetTotalStrength() => BaseStrength + BonusStrength;
    public int GetTotalIntelligence() => BaseIntelligence + BonusIntelligence;
    public int GetTotalDexterity() => BaseDexterity + BonusDexterity;

    /// <summary>
    /// Calcula a vida máxima baseada nos atributos
    /// </summary>
    public int CalculateMaxHealth() => 80 + (GetTotalStrength() * 2);

    /// <summary>
    /// Calcula o ataque baseado nos atributos
    /// </summary>
    public int CalculateAttack() => GetTotalStrength() + (GetTotalDexterity() / 2);

    /// <summary>
    /// Calcula a defesa baseada nos atributos
    /// </summary>
    public int CalculateDefense() => (GetTotalIntelligence() / 2) + (GetTotalDexterity() / 3);

    /// <summary>
    /// Verifica se o herói atingiu o nível máximo
    /// </summary>
    public bool IsMaxLevel() => Level >= MaxLevel;

    /// <summary>
    /// Calcula a experiência necessária para o próximo nível
    /// </summary>
    public int GetExperienceForNextLevel()
    {
        if (IsMaxLevel()) return 0;
        return (Level + 1) * 1000 - Experience;
    }

    /// <summary>
    /// Calcula o nível baseado na experiência atual
    /// </summary>
    public int CalculateLevelFromExperience(int exp)
    {
        return Math.Min(MaxLevel, (int)Math.Floor(Math.Sqrt(exp / 100.0)) + 1);
    }

    /// <summary>
    /// Adiciona experiência e verifica se subiu de nível
    /// </summary>
    public bool AddExperience(int exp)
    {
        var oldLevel = Level;
        Experience += exp;
        Level = CalculateLevelFromExperience(Experience);
        
        // Se subiu de nível, adiciona pontos de atributo
        if (Level > oldLevel)
        {
            var levelsGained = Level - oldLevel;
            UnallocatedAttributePoints += levelsGained * 2; // 2 pontos por nível
            
            // Atualiza vida máxima
            MaxHealth = CalculateMaxHealth();
            CurrentHealth = MaxHealth; // Restaura vida ao máximo ao subir de nível
        }
        
        return Level > oldLevel;
    }

    /// <summary>
    /// Gasta pontos de atributo
    /// </summary>
    public bool SpendAttributePoint(string attribute)
    {
        if (UnallocatedAttributePoints <= 0) return false;

        switch (attribute.ToLower())
        {
            case "strength":
                BonusStrength++;
                break;
            case "intelligence":
                BonusIntelligence++;
                break;
            case "dexterity":
                BonusDexterity++;
                break;
            default:
                return false;
        }

        UnallocatedAttributePoints--;
        return true;
    }

    /// <summary>
    /// Configura os atributos iniciais baseados na classe
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
                BaseIntelligence += 1; // 11 total
                BaseDexterity += 3;   // 13 total
                break;
                
            case "bruxo":
            case "warlock":
                BaseStrength += 1;    // 11 total
                BaseIntelligence += 13; // 23 total
                BaseDexterity += 4;   // 14 total
                break;
                
            case "druida":
            case "druid":
                BaseStrength += 3;    // 13 total
                BaseIntelligence += 9; // 19 total
                BaseDexterity += 6;   // 16 total
                break;
                
            case "monge":
            case "monk":
                BaseStrength += 4;    // 14 total
                BaseIntelligence += 6; // 16 total
                BaseDexterity += 8;   // 18 total
                break;
                
            default:
                // Classe não reconhecida, usa valores padrão
                BaseStrength += 3;    // 13 total
                BaseIntelligence += 3; // 13 total
                BaseDexterity += 3;   // 13 total
                break;
        }
        
        // Atualiza vida máxima baseada nos novos atributos
        MaxHealth = CalculateMaxHealth();
        CurrentHealth = MaxHealth;
    }
}

public class HeroQuest
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public int QuestId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navegação
    public Hero Hero { get; set; } = null!;
    public Quest Quest { get; set; } = null!;
}

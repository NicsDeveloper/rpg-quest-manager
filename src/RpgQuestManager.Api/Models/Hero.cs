namespace RpgQuestManager.Api.Models;

public class Hero
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty; // ex: Guerreiro, Mago, Arqueiro
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int Strength { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Gold { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public ICollection<HeroQuest> HeroQuests { get; set; } = new List<HeroQuest>();
    public ICollection<HeroItem> HeroItems { get; set; } = new List<HeroItem>();
    
    // Método para calcular XP necessário para próximo nível
    public int GetExperienceForNextLevel()
    {
        return Level * 100;
    }
    
    // Método para verificar se pode subir de nível
    public bool CanLevelUp()
    {
        return Experience >= GetExperienceForNextLevel();
    }
    
    // Método para subir de nível
    public void LevelUp()
    {
        if (!CanLevelUp()) return;
        
        var xpNeeded = GetExperienceForNextLevel();
        Level++;
        Experience -= xpNeeded;
        
        // Aumenta atributos ao subir de nível
        Strength += 2;
        Intelligence += 2;
        Dexterity += 2;
        
        // Continua subindo enquanto tiver XP suficiente
        if (CanLevelUp())
        {
            LevelUp();
        }
    }
}


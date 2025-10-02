using System.ComponentModel.DataAnnotations;

namespace RpgQuestManager.Api.Models;

public enum QuestDifficulty
{
    Easy,       // Fácil - Nível 1-5
    Medium,     // Médio - Nível 6-15
    Hard,       // Difícil - Nível 16-25
    Epic,       // Épico - Nível 26-35
    Legendary   // Lendário - Nível 36+
}

public enum QuestType
{
    Main,       // História Principal
    Side,       // Missão Secundária
    Boss,       // Boss Fight
    Event,      // Evento Especial
    Dungeon,    // Dungeon
    Raid,       // Raid
    PvP,        // Player vs Player
    Daily,      // Missão Diária
    Weekly,     // Missão Semanal
    Seasonal    // Missão Sazonal
}

public enum QuestEnvironment
{
    Forest,     // Floresta
    Desert,     // Deserto
    Mountain,   // Montanha
    Cave,       // Caverna
    Ruins,      // Ruínas
    Swamp,      // Pântano
    Tundra,     // Tundra
    Volcano,    // Vulcão
    Sky,        // Céu
    Underwater, // Subaquático
    Void,       // Vazio
    Castle,     // Castelo
    Temple,     // Templo
    Crypt,      // Cripta
    Laboratory  // Laboratório
}

public class QuestCategory
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public QuestDifficulty Difficulty { get; set; }
    public QuestType Type { get; set; }
    public QuestEnvironment Environment { get; set; }
    
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = "#6366f1";
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Quest> Quests { get; set; } = new List<Quest>();
    
    public string GetDifficultyColor()
    {
        return Difficulty switch
        {
            QuestDifficulty.Easy => "#10b981",      // Verde
            QuestDifficulty.Medium => "#f59e0b",    // Amarelo
            QuestDifficulty.Hard => "#ef4444",      // Vermelho
            QuestDifficulty.Epic => "#8b5cf6",      // Roxo
            QuestDifficulty.Legendary => "#f97316", // Laranja
            _ => "#6366f1"                          // Azul padrão
        };
    }
    
    public string GetDifficultyIcon()
    {
        return Difficulty switch
        {
            QuestDifficulty.Easy => "🟢",
            QuestDifficulty.Medium => "🟡",
            QuestDifficulty.Hard => "🔴",
            QuestDifficulty.Epic => "🟣",
            QuestDifficulty.Legendary => "🟠",
            _ => "🔵"
        };
    }
    
    public string GetTypeIcon()
    {
        return Type switch
        {
            QuestType.Main => "📖",
            QuestType.Side => "📋",
            QuestType.Boss => "👹",
            QuestType.Event => "🎉",
            QuestType.Dungeon => "🏰",
            QuestType.Raid => "⚔️",
            QuestType.PvP => "⚡",
            QuestType.Daily => "📅",
            QuestType.Weekly => "📆",
            QuestType.Seasonal => "🎄",
            _ => "❓"
        };
    }
    
    public string GetEnvironmentIcon()
    {
        return Environment switch
        {
            QuestEnvironment.Forest => "🌲",
            QuestEnvironment.Desert => "🏜️",
            QuestEnvironment.Mountain => "⛰️",
            QuestEnvironment.Cave => "🕳️",
            QuestEnvironment.Ruins => "🏛️",
            QuestEnvironment.Swamp => "🐸",
            QuestEnvironment.Tundra => "🧊",
            QuestEnvironment.Volcano => "🌋",
            QuestEnvironment.Sky => "☁️",
            QuestEnvironment.Underwater => "🌊",
            QuestEnvironment.Void => "🌌",
            QuestEnvironment.Castle => "🏰",
            QuestEnvironment.Temple => "⛩️",
            QuestEnvironment.Crypt => "⚰️",
            QuestEnvironment.Laboratory => "🧪",
            _ => "🌍"
        };
    }
}

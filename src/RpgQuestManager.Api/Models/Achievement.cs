namespace RpgQuestManager.Api.Models;

public class Achievement
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public AchievementType Type { get; set; }
    public AchievementCategory Category { get; set; }
    public int RequiredValue { get; set; }
    public int ExperienceReward { get; set; } = 0;
    public int GoldReward { get; set; } = 0;
    public int? ItemRewardId { get; set; }
    public bool IsHidden { get; set; } = false;
    public bool IsRepeatable { get; set; } = false;
    public int SortOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public Item? ItemReward { get; set; }
    public List<UserAchievement> UserAchievements { get; set; } = new();
}

public class UserAchievement
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AchievementId { get; set; }
    public int Progress { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;
    public bool IsClaimed { get; set; } = false;
    public DateTime CompletedAt { get; set; }
    public DateTime ClaimedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public User User { get; set; } = null!;
    public Achievement Achievement { get; set; } = null!;
}

public enum AchievementType
{
    Combat,         // Matar X monstros, vencer X batalhas
    Quest,          // Completar X missões
    Exploration,    // Explorar X ambientes
    Collection,     // Coletar X itens
    Social,         // Interações sociais
    Progression,    // Alcançar X nível
    Equipment,      // Equipar itens
    Special         // Conquistas especiais
}

public enum AchievementCategory
{
    Bronze,         // Conquistas básicas
    Silver,         // Conquistas intermediárias
    Gold,           // Conquistas avançadas
    Platinum,       // Conquistas raras
    Legendary,      // Conquistas épicas
    Mythic          // Conquistas míticas
}

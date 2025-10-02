namespace RpgQuestManager.Api.Models;

public class Quest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "Main";
    public string Difficulty { get; set; } = string.Empty;
    public string RequiredClass { get; set; } = "Any";
    public int RequiredLevel { get; set; } = 1;
    public int ExperienceReward { get; set; }
    public int GoldReward { get; set; }
    public bool IsRepeatable { get; set; } = false; // Se pode ser repetida diariamente
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Sistema de Classificação
    public int? CategoryId { get; set; }
    public QuestCategory? Category { get; set; }
    
    // Sistema de Ambiente e Condições
    public QuestEnvironment Environment { get; set; } = QuestEnvironment.Forest;
    public EnvironmentalConditionType? EnvironmentalCondition { get; set; }
    public int EnvironmentalIntensity { get; set; } = 1;
    public string ImmuneClasses { get; set; } = "[]"; // JSON string de classes imunes
    public string ImmuneEnemyTypes { get; set; } = "[]"; // JSON string de tipos de inimigos imunes
    
    // Sistema de Recompensas Avançado
    public string SpecialRewards { get; set; } = "[]"; // JSON string de recompensas especiais
    public int BossId { get; set; } = 0; // ID do boss principal (se houver)
    public bool IsBossQuest { get; set; } = false;
    public int EstimatedDuration { get; set; } = 30; // Duração estimada em minutos
    
    // Sistema de Progressão
    public int StoryOrder { get; set; } = 0; // Ordem na história principal
    public string Prerequisites { get; set; } = "[]"; // JSON string de pré-requisitos
    public bool IsUnlocked { get; set; } = true;
    
    public ICollection<HeroQuest> HeroQuests { get; set; } = new List<HeroQuest>();
    public ICollection<QuestEnemy> QuestEnemies { get; set; } = new List<QuestEnemy>();
    public ICollection<Reward> Rewards { get; set; } = new List<Reward>();
    public ICollection<EnvironmentalCondition> EnvironmentalConditions { get; set; } = new List<EnvironmentalCondition>();
}

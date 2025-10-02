namespace RpgQuestManager.Api.Models;

public class HeroTraining
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public DateTime TrainingDate { get; set; }
    public int XpGained { get; set; }
    public string TrainingType { get; set; } = "Daily"; // Daily, Special, etc.
    
    // Relacionamentos
    public Hero Hero { get; set; } = null!;
}

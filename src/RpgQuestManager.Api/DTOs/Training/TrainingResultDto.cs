namespace RpgQuestManager.Api.DTOs.Training;

public class TrainingResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int XpGained { get; set; }
    public bool LeveledUp { get; set; }
    public int? NewLevel { get; set; }
    public int RemainingXp { get; set; }
}

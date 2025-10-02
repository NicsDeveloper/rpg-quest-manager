namespace RpgQuestManager.Api.DTOs.Training;

public class TrainingHistoryDto
{
    public int Id { get; set; }
    public DateTime TrainingDate { get; set; }
    public int XpGained { get; set; }
    public string TrainingType { get; set; } = string.Empty;
}

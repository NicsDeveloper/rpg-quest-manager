namespace RpgQuestManager.Api.DTOs.Training;

public class CanTrainDto
{
    public bool CanTrain { get; set; }
    public DateTime? LastTrainingDate { get; set; }
    public string Message { get; set; } = string.Empty;
}

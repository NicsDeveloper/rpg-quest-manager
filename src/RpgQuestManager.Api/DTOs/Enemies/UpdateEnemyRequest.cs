namespace RpgQuestManager.Api.DTOs.Enemies;

public class UpdateEnemyRequest
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public int? Power { get; set; }
    public int? Health { get; set; }
}


namespace RpgQuestManager.Api.DTOs.Enemies;

public class CreateEnemyRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Power { get; set; }
    public int Health { get; set; }
}


namespace RpgQuestManager.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Player";
    public bool HasSeenTutorial { get; set; } = false;
    public int Gold { get; set; } = 100; // Ouro do player (compartilhado entre heróis)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


namespace RpgQuestManager.Api.Models;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? ActionUrl { get; set; }
    public string? IconUrl { get; set; }

    // Relacionamentos
    public User User { get; set; } = null!;
}

public enum NotificationType
{
    Achievement,     // Conquista desbloqueada
    LevelUp,         // Level up
    Quest,           // Quest relacionada
    Combat,          // Combate
    Party,           // Grupo
    System,          // Sistema
    Reward,          // Recompensa
    Warning,         // Aviso
    Info             // Informação
}

public enum NotificationPriority
{
    Low,             // Baixa prioridade
    Normal,          // Prioridade normal
    High,            // Alta prioridade
    Critical         // Crítica
}

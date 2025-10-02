namespace RpgQuestManager.Api.Events;

public record QuestCompletedEvent
{
    public int HeroId { get; init; }
    public string HeroName { get; init; } = string.Empty;
    public int QuestId { get; init; }
    public string QuestName { get; init; } = string.Empty;
    public int ExperienceGained { get; init; }
    public int GoldGained { get; init; }
    public int NewLevel { get; init; }
    public DateTime CompletedAt { get; init; }
}


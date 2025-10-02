namespace RpgQuestManager.Api.DTOs.Combat;

public class StartCombatRequest
{
    public List<int> HeroIds { get; set; } = new List<int>(); // Múltiplos heróis
    public int QuestId { get; set; }
}

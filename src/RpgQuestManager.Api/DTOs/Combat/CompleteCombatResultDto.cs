using RpgQuestManager.Api.DTOs.Items;

namespace RpgQuestManager.Api.DTOs.Combat;

public class CompleteCombatResultDto
{
    public string Status { get; set; } = "Victory"; // Victory, Fled, Defeated
    public string Message { get; set; } = string.Empty;
    public int GoldEarned { get; set; }
    public int ExperienceEarned { get; set; }
    public int HeroNewLevel { get; set; }
    public List<ItemDto> DroppedItems { get; set; } = new List<ItemDto>();
}

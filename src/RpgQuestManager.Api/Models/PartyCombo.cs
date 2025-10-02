namespace RpgQuestManager.Api.Models;

/// <summary>
/// Define combinações de classes que geram sinergias especiais
/// </summary>
public class PartyCombo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // "Muralha Inabalável"
    public string RequiredClass1 { get; set; } = string.Empty; // "Guerreiro"
    public string RequiredClass2 { get; set; } = string.Empty; // "Paladino"
    public string? RequiredClass3 { get; set; } // Terceira classe opcional
    public string Description { get; set; } = string.Empty;
    public string Effect { get; set; } = string.Empty; // Descrição do efeito
    public string Icon { get; set; } = "⚔️"; // Emoji/ícone do combo
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public ICollection<BossWeakness> BossWeaknesses { get; set; } = new List<BossWeakness>();
    public ICollection<ComboDiscovery> ComboDiscoveries { get; set; } = new List<ComboDiscovery>();
}


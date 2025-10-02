namespace RpgQuestManager.Api.Models;

/// <summary>
/// Tabela de drops de bosses
/// </summary>
public class BossDropTable
{
    public int Id { get; set; }
    public int EnemyId { get; set; }
    public int ItemId { get; set; }
    
    /// <summary>
    /// Chance de drop (0-100%)
    /// Base modificada pela raridade do item
    /// </summary>
    public decimal DropChance { get; set; }
    
    /// <summary>
    /// Se este é um drop exclusivo deste boss
    /// </summary>
    public bool IsExclusive { get; set; } = false;
    
    // Relacionamentos
    public Enemy Enemy { get; set; } = null!;
    public Item Item { get; set; } = null!;
}


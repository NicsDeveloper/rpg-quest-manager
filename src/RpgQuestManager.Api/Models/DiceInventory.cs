namespace RpgQuestManager.Api.Models;

/// <summary>
/// Inventário de dados virtuais do player (compartilhado entre todos os heróis)
/// </summary>
public class DiceInventory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    // Quantidade de cada tipo de dado disponível
    public int D6Count { get; set; } = 3;   // Começa com 3 d6
    public int D10Count { get; set; } = 0;
    public int D12Count { get; set; } = 0;
    public int D20Count { get; set; } = 0;
    
    // Relacionamento
    public User User { get; set; } = null!;
    
    /// <summary>
    /// Verifica se tem pelo menos um dado do tipo especificado
    /// </summary>
    public bool HasDice(DiceType diceType)
    {
        return diceType switch
        {
            DiceType.D6 => D6Count > 0,
            DiceType.D10 => D10Count > 0,
            DiceType.D12 => D12Count > 0,
            DiceType.D20 => D20Count > 0,
            _ => false
        };
    }
    
    /// <summary>
    /// Usa um dado (decrementa a contagem)
    /// </summary>
    public bool UseDice(DiceType diceType)
    {
        switch (diceType)
        {
            case DiceType.D6 when D6Count > 0:
                D6Count--;
                return true;
            case DiceType.D10 when D10Count > 0:
                D10Count--;
                return true;
            case DiceType.D12 when D12Count > 0:
                D12Count--;
                return true;
            case DiceType.D20 when D20Count > 0:
                D20Count--;
                return true;
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Adiciona dados ao inventário
    /// </summary>
    public void AddDice(DiceType diceType, int quantity)
    {
        switch (diceType)
        {
            case DiceType.D6:
                D6Count += quantity;
                break;
            case DiceType.D10:
                D10Count += quantity;
                break;
            case DiceType.D12:
                D12Count += quantity;
                break;
            case DiceType.D20:
                D20Count += quantity;
                break;
        }
    }
}

/// <summary>
/// Tipos de dados disponíveis no jogo
/// </summary>
public enum DiceType
{
    D6 = 6,     // Dado de 6 lados (fácil)
    D10 = 10,   // Dado de 10 lados (médio)
    D12 = 12,   // Dado de 12 lados (difícil)
    D20 = 20    // Dado de 20 lados (muito difícil)
}


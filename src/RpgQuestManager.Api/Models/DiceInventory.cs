namespace RpgQuestManager.Api.Models;

/// <summary>
/// Inventário de dados virtuais do herói
/// </summary>
public class DiceInventory
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    
    // Quantidade de cada tipo de dado disponível
    public int D6Count { get; set; } = 3;   // Começa com 3 d6
    public int D8Count { get; set; } = 0;
    public int D12Count { get; set; } = 0;
    public int D20Count { get; set; } = 0;
    
    // Relacionamento
    public Hero Hero { get; set; } = null!;
    
    /// <summary>
    /// Verifica se tem pelo menos um dado do tipo especificado
    /// </summary>
    public bool HasDice(DiceType diceType)
    {
        return diceType switch
        {
            DiceType.D6 => D6Count > 0,
            DiceType.D8 => D8Count > 0,
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
            case DiceType.D8 when D8Count > 0:
                D8Count--;
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
            case DiceType.D8:
                D8Count += quantity;
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
    D6 = 6,    // Dado de 6 lados (fácil)
    D8 = 8,    // Dado de 8 lados (médio)
    D12 = 12,  // Dado de 12 lados (difícil)
    D20 = 20   // Dado de 20 lados (muito difícil)
}


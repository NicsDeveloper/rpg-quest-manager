namespace RpgQuestManager.Api.Models;

/// <summary>
/// Gerencia dados gratuitos periódicos por usuário
/// </summary>
public class FreeDiceGrant
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DiceType DiceType { get; set; }
    
    public DateTime LastClaimedAt { get; set; }
    public DateTime NextAvailableAt { get; set; }
    
    // Relacionamento
    public User User { get; set; } = null!;
    
    /// <summary>
    /// Verifica se o dado está disponível para resgate
    /// </summary>
    public bool IsAvailable()
    {
        return DateTime.UtcNow >= NextAvailableAt;
    }
    
    /// <summary>
    /// Retorna o tempo restante até o próximo resgate
    /// </summary>
    public TimeSpan TimeUntilAvailable()
    {
        var now = DateTime.UtcNow;
        return NextAvailableAt > now ? NextAvailableAt - now : TimeSpan.Zero;
    }
    
    /// <summary>
    /// Obtém o cooldown em horas baseado no tipo de dado
    /// </summary>
    public static int GetCooldownHours(DiceType diceType)
    {
        return diceType switch
        {
            DiceType.D6 => 24,   // 1 dia
            DiceType.D10 => 48,   // 2 dias
            DiceType.D12 => 168, // 7 dias
            DiceType.D20 => 336, // 14 dias
            _ => 24
        };
    }
}


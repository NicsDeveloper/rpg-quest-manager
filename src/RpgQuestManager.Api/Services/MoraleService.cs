using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class MoraleService
{
    public MoraleLevel GetMoraleLevel(int morale)
    {
        return morale switch
        {
            <= 10 => MoraleLevel.Despair,
            <= 30 => MoraleLevel.Low,
            <= 70 => MoraleLevel.Normal,
            <= 90 => MoraleLevel.High,
            _ => MoraleLevel.Inspired
        };
    }

    public int AdjustMorale(int currentMorale, MoraleEvent moraleEvent)
    {
        var adjustment = moraleEvent switch
        {
            MoraleEvent.CriticalHit => 15,
            MoraleEvent.SuccessfulAttack => 5,
            MoraleEvent.Victory => 25,
            MoraleEvent.TakeDamage => -10,
            MoraleEvent.AttackMiss => -5,
            MoraleEvent.Death => -30,
            MoraleEvent.LevelUp => 20,
            MoraleEvent.QuestComplete => 15,
            MoraleEvent.QuestFailed => -20,
            _ => 0
        };

        return Math.Max(0, Math.Min(100, currentMorale + adjustment));
    }

    public (double damageMultiplier, double defenseMultiplier, double criticalMultiplier) GetMoraleModifiers(MoraleLevel moraleLevel)
    {
        return moraleLevel switch
        {
            MoraleLevel.Despair => (1.5, 0.5, 1.3), // +50% dano, -50% defesa, +30% crítico
            MoraleLevel.Low => (0.8, 0.85, 1.0),    // -20% dano, -15% defesa
            MoraleLevel.Normal => (1.0, 1.0, 1.0),  // Sem modificadores
            MoraleLevel.High => (1.1, 1.0, 1.2),    // +10% dano, +20% crítico
            MoraleLevel.Inspired => (1.2, 1.15, 1.3), // +20% dano, +15% defesa, +30% crítico
            _ => (1.0, 1.0, 1.0)
        };
    }
}

public enum MoraleEvent
{
    CriticalHit,
    SuccessfulAttack,
    Victory,
    TakeDamage,
    AttackMiss,
    Death,
    LevelUp,
    QuestComplete,
    QuestFailed
}

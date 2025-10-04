using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class DiceService
{
    private readonly Random _rng = new();

    public int RollDice(DifficultyLevel difficulty)
    {
        return _rng.Next(1, (int)difficulty + 1);
    }

    public int RollDice(int sides)
    {
        return _rng.Next(1, sides + 1);
    }

    public bool RollPercentage(int percentage)
    {
        return _rng.Next(1, 101) <= percentage;
    }

    public int RollCriticalChance(MoraleLevel moraleLevel)
    {
        var baseChance = 5; // 5% base
        var moraleBonus = moraleLevel switch
        {
            MoraleLevel.Despair => 30,
            MoraleLevel.Low => 0,
            MoraleLevel.Normal => 0,
            MoraleLevel.High => 20,
            MoraleLevel.Inspired => 30,
            _ => 0
        };
        return baseChance + moraleBonus;
    }

    public int RollFumbleChance(MoraleLevel moraleLevel)
    {
        var baseChance = 5; // 5% base
        var moralePenalty = moraleLevel switch
        {
            MoraleLevel.Despair => 0,
            MoraleLevel.Low => 10,
            MoraleLevel.Normal => 0,
            MoraleLevel.High => 0,
            MoraleLevel.Inspired => 0,
            _ => 0
        };
        return baseChance + moralePenalty;
    }
}

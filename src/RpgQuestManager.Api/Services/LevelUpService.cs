using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class LevelUpService
{
    private readonly ApplicationDbContext _db;
    private readonly MoraleService _moraleService;

    public LevelUpService(ApplicationDbContext db, MoraleService moraleService)
    {
        _db = db;
        _moraleService = moraleService;
    }

    public async Task<bool> CheckAndProcessLevelUpAsync(Character character)
    {
        if (character.Experience >= character.NextLevelExperience)
        {
            await ProcessLevelUpAsync(character);
            return true;
        }
        return false;
    }

    public async Task ProcessLevelUpAsync(Character character)
    {
        // Aumenta nível
        character.Level++;
        
        // Aumenta atributos
        character.MaxHealth += 10;
        character.Health = character.MaxHealth; // Restaura HP ao máximo
        character.Attack += 2;
        character.Defense += 1;
        
        // Restaura moral ao máximo
        character.Morale = 100;
        
        // Calcula próximo nível de experiência
        character.NextLevelExperience = character.Level * 1000;
        
        await _db.SaveChangesAsync();
    }

    public int CalculateExperienceReward(Monster monster, int characterLevel)
    {
        var baseReward = monster.ExperienceReward;
        var levelDifference = monster.Rank == MonsterRank.Boss ? 2 : 1;
        var levelBonus = Math.Max(1, characterLevel - levelDifference) * 5;
        
        return baseReward + levelBonus;
    }
}

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
        return await CheckAndProcessLevelUpAsync(character);
    }

    public async Task<bool> CheckAndProcessLevelUpAsync(Hero hero)
    {
        if (hero.Experience >= GetNextLevelExperience(hero.Level))
        {
            await ProcessLevelUpAsync(hero);
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

    public async Task ProcessLevelUpAsync(Hero hero)
    {
        // Aumenta nível
        hero.Level++;
        
        // Aumenta atributos
        hero.UnallocatedAttributePoints += 3;
        hero.MaxHealth = hero.CalculateMaxHealth();
        hero.CurrentHealth = hero.MaxHealth;
        hero.Defense = hero.CalculateDefense();
        
        // Restaura moral ao máximo
        hero.Morale = 100;
        
        await _db.SaveChangesAsync();
    }

    public int GetNextLevelExperience(int currentLevel)
    {
        return currentLevel * 1000;
    }

    public int CalculateExperienceReward(Monster monster, int characterLevel)
    {
        var baseReward = monster.ExperienceReward;
        var levelDifference = monster.Rank == MonsterRank.Boss ? 2 : 1;
        var levelBonus = Math.Max(1, characterLevel - levelDifference) * 5;
        
        return baseReward + levelBonus;
    }
}

using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class CombatService : ICombatService
{
    private readonly ApplicationDbContext _db;
    private readonly Random _rng = new();
    public CombatService(ApplicationDbContext db) { _db = db; }

    public async Task<CombatResult> AttackAsync(int characterId, int monsterId)
    {
        var hero = await _db.Characters.FirstOrDefaultAsync(x => x.Id == characterId) ?? throw new InvalidOperationException("Hero not found");
        var monster = await _db.Monsters.FirstOrDefaultAsync(x => x.Id == monsterId) ?? throw new InvalidOperationException("Monster not found");

        var roll = _rng.Next(1, 101); // 1..100
        var isCritical = roll <= 10; // 10% crítico
        var isFumble = roll >= 96;   // 5% falha crítica (96..100)

        var damageToMonster = Math.Max(1, hero.Attack - monster.Defense);
        if (isCritical) damageToMonster *= 2;
        if (isFumble) damageToMonster = Math.Max(1, damageToMonster / 2);

        monster.Health = Math.Max(0, monster.Health - damageToMonster);

        var damageToHero = Math.Max(1, monster.Attack - hero.Defense);
        hero.Health = Math.Max(0, hero.Health - damageToHero);

        // Moral adjustments
        if (isCritical) hero.Morale = Math.Min(100, hero.Morale + 15);
        if (isFumble) hero.Morale = Math.Max(0, hero.Morale - 5);
        if (damageToHero > 0) hero.Morale = Math.Max(0, hero.Morale - 10);

        await _db.SaveChangesAsync();
        return new CombatResult(hero, monster, damageToMonster, damageToHero, isCritical, isFumble);
    }
}



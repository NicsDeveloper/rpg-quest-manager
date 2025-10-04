using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class MonsterService
{
    private readonly ApplicationDbContext _db;

    public MonsterService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Monster>> GetMonstersByEnvironmentAsync(EnvironmentType environment)
    {
        return await _db.Monsters
            .Where(m => m.Habitat == environment)
            .OrderBy(m => m.Level)
            .ThenBy(m => m.Rank)
            .ToListAsync();
    }

    public async Task<List<Monster>> GetBossesByEnvironmentAsync(EnvironmentType environment)
    {
        return await _db.Monsters
            .Where(m => m.Habitat == environment && m.Rank == MonsterRank.Boss)
            .OrderBy(m => m.Level)
            .ToListAsync();
    }

    public async Task<List<Monster>> GetMonstersByTypeAsync(MonsterType type)
    {
        return await _db.Monsters
            .Where(m => m.Type == type)
            .OrderBy(m => m.Level)
            .ThenBy(m => m.Rank)
            .ToListAsync();
    }

    public async Task<List<Monster>> GetMonstersByLevelRangeAsync(int minLevel, int maxLevel)
    {
        return await _db.Monsters
            .Where(m => m.Level >= minLevel && m.Level <= maxLevel)
            .OrderBy(m => m.Level)
            .ThenBy(m => m.Rank)
            .ToListAsync();
    }

    public async Task<Monster?> GetRandomMonsterAsync(EnvironmentType environment, int characterLevel)
    {
        var monsters = await _db.Monsters
            .Where(m => m.Habitat == environment && m.Level <= characterLevel + 2)
            .ToListAsync();

        if (!monsters.Any()) return null;

        var random = new Random();
        return monsters[random.Next(monsters.Count)];
    }

    public async Task<Monster?> GetRandomBossAsync(EnvironmentType environment, int characterLevel)
    {
        var bosses = await _db.Monsters
            .Where(m => m.Habitat == environment && m.Rank == MonsterRank.Boss && m.Level <= characterLevel + 3)
            .ToListAsync();

        if (!bosses.Any()) return null;

        var random = new Random();
        return bosses[random.Next(bosses.Count)];
    }

    public (double health, double attack, double defense, double experience) GetSizeModifiers(MonsterSize size)
    {
        return size switch
        {
            MonsterSize.Tiny => (0.5, 0.3, 0.2, 0.3),      // -50% HP, -70% ATK, -80% DEF, -70% XP
            MonsterSize.Small => (0.7, 0.6, 0.5, 0.6),     // -30% HP, -40% ATK, -50% DEF, -40% XP
            MonsterSize.Medium => (1.0, 1.0, 1.0, 1.0),    // Base
            MonsterSize.Large => (1.5, 1.3, 1.2, 1.3),     // +50% HP, +30% ATK, +20% DEF, +30% XP
            MonsterSize.Huge => (2.0, 1.6, 1.4, 1.6),      // +100% HP, +60% ATK, +40% DEF, +60% XP
            MonsterSize.Gargantuan => (3.0, 2.0, 1.6, 2.0), // +200% HP, +100% ATK, +60% DEF, +100% XP
            _ => (1.0, 1.0, 1.0, 1.0)
        };
    }

    public string GetSizeDescription(MonsterSize size)
    {
        return size switch
        {
            MonsterSize.Tiny => "Minúsculo",
            MonsterSize.Small => "Pequeno",
            MonsterSize.Medium => "Médio",
            MonsterSize.Large => "Grande",
            MonsterSize.Huge => "Enorme",
            MonsterSize.Gargantuan => "Gigantesco",
            _ => "Desconhecido"
        };
    }

    public string GetTypeDescription(MonsterType type)
    {
        return type switch
        {
            MonsterType.Goblin => "Goblin",
            MonsterType.Orc => "Orc",
            MonsterType.Dragon => "Dragão",
            MonsterType.Undead => "Morto-vivo",
            MonsterType.Demon => "Demônio",
            MonsterType.Troll => "Troll",
            MonsterType.Elemental => "Elemental",
            _ => "Desconhecido"
        };
    }

    public string GetEnvironmentDescription(EnvironmentType environment)
    {
        return environment switch
        {
            EnvironmentType.Forest => "Floresta",
            EnvironmentType.Desert => "Deserto",
            EnvironmentType.Dungeon => "Masmorra",
            EnvironmentType.Castle => "Castelo",
            EnvironmentType.Volcano => "Vulcão",
            EnvironmentType.Swamp => "Pântano",
            EnvironmentType.Tundra => "Tundra",
            EnvironmentType.Sky => "Céu",
            EnvironmentType.Ruins => "Ruínas",
            EnvironmentType.Temple => "Templo",
            EnvironmentType.Crypt => "Cripta",
            _ => "Desconhecido"
        };
    }
}

using RpgQuestManager.Api.Models;
using System.Security.Cryptography;

namespace RpgQuestManager.Api.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext db)
    {
        // Seed Users
        if (!db.Users.Any())
        {
            var user = new User
            {
                Username = "admin",
                Email = "admin@rpg.com",
                PasswordHash = HashPassword("admin123"),
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsActive = true
            };
            db.Users.Add(user);
            db.SaveChanges(); // Salvar usuário primeiro para ter o Id
        }

        // Seed Characters
        if (!db.Characters.Any())
        {
            var user = db.Users.FirstOrDefault();
            if (user != null)
            {
                var character = new Character
                {
                    UserId = user.Id,
                    Name = "Aragorn",
                    Level = 1,
                    Experience = 0,
                    NextLevelExperience = 1000,
                    Health = 100,
                    MaxHealth = 100,
                    Attack = 12,
                    Defense = 6,
                    Morale = 70,
                    Gold = 100,
                    CreatedAt = DateTime.UtcNow,
                    LastPlayedAt = DateTime.UtcNow
                };
                db.Characters.Add(character);
                db.SaveChanges(); // Salvar personagem
            }
        }

        // Seed Items
        var items = ItemData.GetAllItems();
        foreach (var item in items)
        {
            if (!db.Items.Any(x => x.Name == item.Name))
            {
                db.Items.Add(item);
            }
        }

        // Seed Monsters
        var monsters = MonsterData.GetAllMonsters();
        foreach (var monster in monsters)
        {
            if (!db.Monsters.Any(x => x.Name == monster.Name))
            {
                db.Monsters.Add(monster);
            }
        }

        // Seed Quests
        var quests = QuestData.GetAllQuests();
        foreach (var quest in quests)
        {
            if (!db.Quests.Any(x => x.Title == quest.Title))
            {
                db.Quests.Add(quest);
            }
        }

        // Seed Achievements
        var achievements = AchievementData.GetAllAchievements();
        foreach (var achievement in achievements)
        {
            if (!db.Achievements.Any(x => x.Name == achievement.Name))
            {
                db.Achievements.Add(achievement);
            }
        }

        // Seed Special Abilities
        var abilities = SpecialAbilityData.GetAllAbilities();
        foreach (var ability in abilities)
        {
            if (!db.SpecialAbilities.Any(x => x.Name == ability.Name))
            {
                db.SpecialAbilities.Add(ability);
            }
        }

        // Seed Combos
        var combos = SpecialAbilityData.GetAllCombos();
        foreach (var combo in combos)
        {
            if (!db.Combos.Any(x => x.Name == combo.Name))
            {
                db.Combos.Add(combo);
            }
        }

        db.SaveChanges();
    }

    private static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        var hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }
}



using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext db)
    {
        if (!db.Characters.Any())
        {
            db.Characters.Add(new Character
            {
                Name = "Aragorn",
                Level = 1,
                Experience = 0,
                NextLevelExperience = 1000,
                Health = 100,
                MaxHealth = 100,
                Attack = 12,
                Defense = 6,
                Morale = 70
            });
        }

        if (!db.Monsters.Any())
        {
            db.Monsters.AddRange(
                new Monster { Name = "Goblin", Type = MonsterType.Goblin, Rank = MonsterRank.Normal, Habitat = EnvironmentType.Forest, Health = 40, MaxHealth = 40, Attack = 6, Defense = 2, ExperienceReward = 20 },
                new Monster { Name = "Orc", Type = MonsterType.Orc, Rank = MonsterRank.Normal, Habitat = EnvironmentType.Forest, Health = 60, MaxHealth = 60, Attack = 10, Defense = 4, ExperienceReward = 35 }
            );
        }

        if (!db.Quests.Any())
        {
            db.Quests.Add(new Quest
            {
                Title = "Os Primeiros Passos",
                Description = "Derrote um Goblin nas bordas da Floresta.",
                Environment = EnvironmentType.Forest,
                TargetMonsterName = "Goblin",
                TargetMonsterType = MonsterType.Goblin,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 50,
                RequiredLevel = 1
            });
        }

        db.SaveChanges();
    }
}



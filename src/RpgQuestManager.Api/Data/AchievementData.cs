using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Data;

public static class AchievementData
{
    public static List<Achievement> GetAllAchievements()
    {
        return new List<Achievement>
        {
            // Conquistas de Combate
            new Achievement
            {
                Name = "Primeiro Sangue",
                Description = "Mate seu primeiro monstro",
                IconUrl = "sword-icon",
                Type = AchievementType.Combat,
                Category = AchievementCategory.Bronze,
                RequiredValue = 1,
                ExperienceReward = 100,
                GoldReward = 50,
                SortOrder = 1
            },
            new Achievement
            {
                Name = "Caçador de Monstros",
                Description = "Mate 10 monstros",
                IconUrl = "monster-hunter-icon",
                Type = AchievementType.Combat,
                Category = AchievementCategory.Silver,
                RequiredValue = 10,
                ExperienceReward = 500,
                GoldReward = 200,
                SortOrder = 2
            },
            new Achievement
            {
                Name = "Exterminador",
                Description = "Mate 50 monstros",
                IconUrl = "exterminator-icon",
                Type = AchievementType.Combat,
                Category = AchievementCategory.Gold,
                RequiredValue = 50,
                ExperienceReward = 1000,
                GoldReward = 500,
                SortOrder = 3
            },
            new Achievement
            {
                Name = "Lenda do Combate",
                Description = "Mate 100 monstros",
                IconUrl = "combat-legend-icon",
                Type = AchievementType.Combat,
                Category = AchievementCategory.Platinum,
                RequiredValue = 100,
                ExperienceReward = 2000,
                GoldReward = 1000,
                SortOrder = 4
            },

            // Conquistas de Missões
            new Achievement
            {
                Name = "Aventureiro Iniciante",
                Description = "Complete sua primeira missão",
                IconUrl = "adventurer-icon",
                Type = AchievementType.Quest,
                Category = AchievementCategory.Bronze,
                RequiredValue = 1,
                ExperienceReward = 150,
                GoldReward = 75,
                SortOrder = 5
            },
            new Achievement
            {
                Name = "Herói das Missões",
                Description = "Complete 10 missões",
                IconUrl = "quest-hero-icon",
                Type = AchievementType.Quest,
                Category = AchievementCategory.Silver,
                RequiredValue = 10,
                ExperienceReward = 750,
                GoldReward = 300,
                SortOrder = 6
            },
            new Achievement
            {
                Name = "Lenda das Missões",
                Description = "Complete 25 missões",
                IconUrl = "quest-legend-icon",
                Type = AchievementType.Quest,
                Category = AchievementCategory.Gold,
                RequiredValue = 25,
                ExperienceReward = 1500,
                GoldReward = 750,
                SortOrder = 7
            },

            // Conquistas de Progressão
            new Achievement
            {
                Name = "Novato",
                Description = "Alcance o nível 5",
                IconUrl = "novice-icon",
                Type = AchievementType.Progression,
                Category = AchievementCategory.Bronze,
                RequiredValue = 5,
                ExperienceReward = 200,
                GoldReward = 100,
                SortOrder = 8
            },
            new Achievement
            {
                Name = "Experiente",
                Description = "Alcance o nível 10",
                IconUrl = "experienced-icon",
                Type = AchievementType.Progression,
                Category = AchievementCategory.Silver,
                RequiredValue = 10,
                ExperienceReward = 500,
                GoldReward = 250,
                SortOrder = 9
            },
            new Achievement
            {
                Name = "Veterano",
                Description = "Alcance o nível 20",
                IconUrl = "veteran-icon",
                Type = AchievementType.Progression,
                Category = AchievementCategory.Gold,
                RequiredValue = 20,
                ExperienceReward = 1000,
                GoldReward = 500,
                SortOrder = 10
            },
            new Achievement
            {
                Name = "Mestre",
                Description = "Alcance o nível 30",
                IconUrl = "master-icon",
                Type = AchievementType.Progression,
                Category = AchievementCategory.Platinum,
                RequiredValue = 30,
                ExperienceReward = 2000,
                GoldReward = 1000,
                SortOrder = 11
            },

            // Conquistas de Coleta
            new Achievement
            {
                Name = "Colecionador",
                Description = "Colete 10 itens únicos",
                IconUrl = "collector-icon",
                Type = AchievementType.Collection,
                Category = AchievementCategory.Bronze,
                RequiredValue = 10,
                ExperienceReward = 300,
                GoldReward = 150,
                SortOrder = 12
            },
            new Achievement
            {
                Name = "Tesouro",
                Description = "Colete 25 itens únicos",
                IconUrl = "treasure-icon",
                Type = AchievementType.Collection,
                Category = AchievementCategory.Silver,
                RequiredValue = 25,
                ExperienceReward = 750,
                GoldReward = 375,
                SortOrder = 13
            },
            new Achievement
            {
                Name = "Arsenal Completo",
                Description = "Colete 50 itens únicos",
                IconUrl = "arsenal-icon",
                Type = AchievementType.Collection,
                Category = AchievementCategory.Gold,
                RequiredValue = 50,
                ExperienceReward = 1500,
                GoldReward = 750,
                SortOrder = 14
            },

            // Conquistas de Exploração
            new Achievement
            {
                Name = "Explorador",
                Description = "Explore 3 ambientes diferentes",
                IconUrl = "explorer-icon",
                Type = AchievementType.Exploration,
                Category = AchievementCategory.Bronze,
                RequiredValue = 3,
                ExperienceReward = 400,
                GoldReward = 200,
                SortOrder = 15
            },
            new Achievement
            {
                Name = "Viajante",
                Description = "Explore 5 ambientes diferentes",
                IconUrl = "traveler-icon",
                Type = AchievementType.Exploration,
                Category = AchievementCategory.Silver,
                RequiredValue = 5,
                ExperienceReward = 800,
                GoldReward = 400,
                SortOrder = 16
            },
            new Achievement
            {
                Name = "Cartógrafo",
                Description = "Explore todos os ambientes",
                IconUrl = "cartographer-icon",
                Type = AchievementType.Exploration,
                Category = AchievementCategory.Gold,
                RequiredValue = 6, // Assumindo 6 ambientes
                ExperienceReward = 1200,
                GoldReward = 600,
                SortOrder = 17
            },

            // Conquistas Especiais
            new Achievement
            {
                Name = "Primeiro Combo",
                Description = "Execute seu primeiro combo",
                IconUrl = "combo-icon",
                Type = AchievementType.Special,
                Category = AchievementCategory.Silver,
                RequiredValue = 1,
                ExperienceReward = 600,
                GoldReward = 300,
                SortOrder = 18
            },
            new Achievement
            {
                Name = "Mestre dos Combos",
                Description = "Execute 10 combos diferentes",
                IconUrl = "combo-master-icon",
                Type = AchievementType.Special,
                Category = AchievementCategory.Gold,
                RequiredValue = 10,
                ExperienceReward = 1500,
                GoldReward = 750,
                SortOrder = 19
            },
            new Achievement
            {
                Name = "Lenda Viva",
                Description = "Complete todas as conquistas",
                IconUrl = "living-legend-icon",
                Type = AchievementType.Special,
                Category = AchievementCategory.Mythic,
                RequiredValue = 1,
                ExperienceReward = 5000,
                GoldReward = 2500,
                IsHidden = true,
                SortOrder = 20
            }
        };
    }
}

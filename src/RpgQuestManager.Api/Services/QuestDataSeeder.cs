using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;
using System.Text.Json;

namespace RpgQuestManager.Api.Services;

public class QuestDataSeeder
{
    private readonly ApplicationDbContext _context;

    public QuestDataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await SeedQuestCategories();
        await SeedMonsters();
        await SeedQuests();
    }

    private async Task SeedQuestCategories()
    {
        if (await _context.QuestCategories.AnyAsync()) return;

        var categories = new List<QuestCategory>
        {
            new QuestCategory
            {
                Name = "História Principal",
                Description = "Missões que fazem parte da história principal do jogo",
                Difficulty = QuestDifficulty.Easy,
                Type = QuestType.Main,
                Environment = QuestEnvironment.Forest,
                MinLevel = 1,
                MaxLevel = 50,
                Icon = "📖",
                Color = "#3b82f6"
            },
            new QuestCategory
            {
                Name = "Boss Fights",
                Description = "Batalhas épicas contra bosses poderosos",
                Difficulty = QuestDifficulty.Epic,
                Type = QuestType.Boss,
                Environment = QuestEnvironment.Castle,
                MinLevel = 10,
                MaxLevel = 50,
                Icon = "👹",
                Color = "#ef4444"
            },
            new QuestCategory
            {
                Name = "Dungeons",
                Description = "Exploração de masmorras perigosas",
                Difficulty = QuestDifficulty.Hard,
                Type = QuestType.Dungeon,
                Environment = QuestEnvironment.Cave,
                MinLevel = 5,
                MaxLevel = 40,
                Icon = "🏰",
                Color = "#8b5cf6"
            },
            new QuestCategory
            {
                Name = "Eventos Especiais",
                Description = "Eventos limitados e sazonais",
                Difficulty = QuestDifficulty.Medium,
                Type = QuestType.Event,
                Environment = QuestEnvironment.Forest,
                MinLevel = 1,
                MaxLevel = 30,
                Icon = "🎉",
                Color = "#f59e0b"
            },
            new QuestCategory
            {
                Name = "Missões Diárias",
                Description = "Missões que podem ser repetidas diariamente",
                Difficulty = QuestDifficulty.Easy,
                Type = QuestType.Daily,
                Environment = QuestEnvironment.Forest,
                MinLevel = 1,
                MaxLevel = 20,
                Icon = "📅",
                Color = "#10b981"
            }
        };

        _context.QuestCategories.AddRange(categories);
        await _context.SaveChangesAsync();
    }

    private async Task SeedMonsters()
    {
        if (await _context.Monsters.AnyAsync()) return;

        var monsters = new List<Monster>
        {
            // Monstros Fáceis (Nível 1-5)
            new Monster
            {
                Name = "Goblin Guerreiro",
                Description = "Um goblin pequeno mas agressivo, armado com uma espada enferrujada.",
                Type = MonsterType.Goblin,
                Size = MonsterSize.Small,
                Level = 2,
                Power = 8,
                Health = 25,
                Armor = 8,
                Speed = 35,
                Strength = 12,
                Dexterity = 14,
                Constitution = 10,
                Intelligence = 8,
                Wisdom = 9,
                Charisma = 7,
                AttackDice = DiceType.D6,
                AttackBonus = 2,
                DamageBonus = 1,
                CriticalChance = 5,
                Resistances = JsonSerializer.Serialize(new[] { "Fogo" }),
                Immunities = JsonSerializer.Serialize(new[] { "Charme" }),
                StatusEffects = JsonSerializer.Serialize(new[] { "Envenenamento" }),
                SpecialAbilities = JsonSerializer.Serialize(new[] { "Ataque Furtivo" }),
                PreferredEnvironment = QuestEnvironment.Forest,
                ExperienceReward = 50,
                GoldReward = 25,
                BaseMorale = 40,
                Icon = "👹",
                Color = "#10b981",
                Lore = "Goblins são criaturas pequenas e traiçoeiras que vivem em grupos.",
                Origin = "Floresta Sombria",
                Weakness = "Luz brilhante"
            },
            new Monster
            {
                Name = "Lobo Selvagem",
                Description = "Um lobo feroz com olhos brilhantes e presas afiadas.",
                Type = MonsterType.Beast,
                Size = MonsterSize.Medium,
                Level = 3,
                Power = 12,
                Health = 35,
                Armor = 10,
                Speed = 50,
                Strength = 14,
                Dexterity = 16,
                Constitution = 12,
                Intelligence = 6,
                Wisdom = 12,
                Charisma = 8,
                AttackDice = DiceType.D6,
                AttackBonus = 3,
                DamageBonus = 2,
                CriticalChance = 8,
                Resistances = JsonSerializer.Serialize(new[] { "Frio" }),
                Immunities = JsonSerializer.Serialize(new[] { "Medo" }),
                StatusEffects = JsonSerializer.Serialize(new[] { "Sangramento" }),
                SpecialAbilities = JsonSerializer.Serialize(new[] { "Investida", "Mordida" }),
                PreferredEnvironment = QuestEnvironment.Forest,
                ExperienceReward = 75,
                GoldReward = 30,
                BaseMorale = 55,
                Icon = "🐺",
                Color = "#10b981",
                Lore = "Lobos são predadores naturais que caçam em matilhas.",
                Origin = "Floresta Profunda",
                Weakness = "Fogo"
            },
            new Monster
            {
                Name = "Esqueleto Guerreiro",
                Description = "Os restos mortais de um antigo guerreiro, reanimado por magia negra.",
                Type = MonsterType.Undead,
                Size = MonsterSize.Medium,
                Level = 4,
                Power = 15,
                Health = 40,
                Armor = 12,
                Speed = 25,
                Strength = 16,
                Dexterity = 10,
                Constitution = 14,
                Intelligence = 8,
                Wisdom = 10,
                Charisma = 6,
                AttackDice = DiceType.D10,
                AttackBonus = 4,
                DamageBonus = 3,
                CriticalChance = 5,
                Resistances = JsonSerializer.Serialize(new[] { "Necrótico", "Frio" }),
                Immunities = JsonSerializer.Serialize(new[] { "Envenenamento", "Sono", "Charme" }),
                StatusEffects = JsonSerializer.Serialize(new[] { "Congelamento" }),
                SpecialAbilities = JsonSerializer.Serialize(new[] { "Golpe Fantasma" }),
                PreferredEnvironment = QuestEnvironment.Crypt,
                ExperienceReward = 100,
                GoldReward = 40,
                BaseMorale = 30,
                Icon = "💀",
                Color = "#6b7280",
                Lore = "Esqueletos são criaturas mortas-vivas criadas por necromantes.",
                Origin = "Cripta Antiga",
                Weakness = "Luz divina"
            },

            // Monstros Médios (Nível 6-15)
            new Monster
            {
                Name = "Orc Berserker",
                Description = "Um orc furioso com músculos enormes e uma fúria incontrolável.",
                Type = MonsterType.Orc,
                Size = MonsterSize.Large,
                Level = 8,
                Power = 25,
                Health = 80,
                Armor = 15,
                Speed = 30,
                Strength = 20,
                Dexterity = 12,
                Constitution = 18,
                Intelligence = 8,
                Wisdom = 10,
                Charisma = 6,
                AttackDice = DiceType.D10,
                AttackBonus = 6,
                DamageBonus = 5,
                CriticalChance = 10,
                Resistances = JsonSerializer.Serialize(new[] { "Físico" }),
                Immunities = JsonSerializer.Serialize(new[] { "Medo" }),
                StatusEffects = JsonSerializer.Serialize(new[] { "Berserker", "Enfraquecimento" }),
                SpecialAbilities = JsonSerializer.Serialize(new[] { "Fúria Berserker", "Golpe Devastador" }),
                PreferredEnvironment = QuestEnvironment.Mountain,
                ExperienceReward = 200,
                GoldReward = 80,
                BaseMorale = 70,
                Icon = "👹",
                Color = "#f59e0b",
                Lore = "Orcs são criaturas brutais que vivem para a guerra.",
                Origin = "Montanhas Gélidas",
                Weakness = "Magia divina"
            },
            new Monster
            {
                Name = "Troll da Montanha",
                Description = "Um troll gigante com pele de pedra e força descomunal.",
                Type = MonsterType.Troll,
                Size = MonsterSize.Huge,
                Level = 12,
                Power = 35,
                Health = 150,
                Armor = 18,
                Speed = 20,
                Strength = 24,
                Dexterity = 8,
                Constitution = 22,
                Intelligence = 6,
                Wisdom = 12,
                Charisma = 4,
                AttackDice = DiceType.D12,
                AttackBonus = 8,
                DamageBonus = 8,
                CriticalChance = 5,
                Resistances = JsonSerializer.Serialize(new[] { "Físico", "Fogo" }),
                Immunities = JsonSerializer.Serialize(new[] { "Medo", "Charme" }),
                StatusEffects = JsonSerializer.Serialize(new[] { "Enfraquecimento", "Atordoamento" }),
                SpecialAbilities = JsonSerializer.Serialize(new[] { "Golpe Esmagador", "Regeneração" }),
                PreferredEnvironment = QuestEnvironment.Mountain,
                ExperienceReward = 400,
                GoldReward = 150,
                BaseMorale = 60,
                Icon = "🧌",
                Color = "#f59e0b",
                Lore = "Trolls são criaturas antigas e poderosas que vivem nas montanhas.",
                Origin = "Picos Gélidos",
                Weakness = "Fogo e ácido"
            },

            // Monstros Difíceis (Nível 16-25)
            new Monster
            {
                Name = "Dragão Jovem",
                Description = "Um dragão jovem mas já poderoso, com escamas brilhantes e fogo na boca.",
                Type = MonsterType.Dragon,
                Size = MonsterSize.Huge,
                Level = 18,
                Power = 50,
                Health = 250,
                Armor = 20,
                Speed = 40,
                Strength = 22,
                Dexterity = 14,
                Constitution = 20,
                Intelligence = 16,
                Wisdom = 14,
                Charisma = 18,
                AttackDice = DiceType.D20,
                AttackBonus = 12,
                DamageBonus = 10,
                CriticalChance = 15,
                Resistances = JsonSerializer.Serialize(new[] { "Fogo", "Físico" }),
                Immunities = JsonSerializer.Serialize(new[] { "Medo", "Charme", "Sono" }),
                StatusEffects = JsonSerializer.Serialize(new[] { "Queimadura", "Medo" }),
                SpecialAbilities = JsonSerializer.Serialize(new[] { "Sopro de Fogo", "Voo", "Garras Afiadas" }),
                PreferredEnvironment = QuestEnvironment.Volcano,
                ExperienceReward = 800,
                GoldReward = 300,
                BaseMorale = 80,
                Icon = "🐉",
                Color = "#ef4444",
                Lore = "Dragões são as criaturas mais poderosas do mundo.",
                Origin = "Vulcão Ardente",
                Weakness = "Gelo e frio"
            },

            // Bosses Épicos (Nível 26+)
            new Monster
            {
                Name = "Lich Senhor das Trevas",
                Description = "Um poderoso lich que governa sobre os mortos-vivos com magia negra.",
                Type = MonsterType.Boss,
                Size = MonsterSize.Medium,
                Level = 30,
                Power = 80,
                Health = 400,
                Armor = 25,
                Speed = 30,
                Strength = 16,
                Dexterity = 12,
                Constitution = 18,
                Intelligence = 24,
                Wisdom = 20,
                Charisma = 22,
                AttackDice = DiceType.D20,
                AttackBonus = 15,
                DamageBonus = 15,
                CriticalChance = 20,
                Resistances = JsonSerializer.Serialize(new[] { "Necrótico", "Frio", "Fogo", "Raio" }),
                Immunities = JsonSerializer.Serialize(new[] { "Envenenamento", "Sono", "Charme", "Medo" }),
                StatusEffects = JsonSerializer.Serialize(new[] { "Medo", "Enfraquecimento", "Congelamento" }),
                SpecialAbilities = JsonSerializer.Serialize(new[] { "Raio da Morte", "Névoa de Morte", "Invocar Mortos-Vivos" }),
                PreferredEnvironment = QuestEnvironment.Crypt,
                ExperienceReward = 2000,
                GoldReward = 800,
                BaseMorale = 90,
                IsBoss = true,
                BossPhase = 3,
                BossHealthThreshold = 50,
                Icon = "👑",
                Color = "#8b5cf6",
                Lore = "O Lich Senhor das Trevas é um dos seres mais poderosos do mundo.",
                Origin = "Cripta Profunda",
                Weakness = "Luz divina e magia sagrada"
            }
        };

        _context.Monsters.AddRange(monsters);
        await _context.SaveChangesAsync();
    }

    private async Task SeedQuests()
    {
        if (await _context.Quests.AnyAsync()) return;

        var mainCategory = await _context.QuestCategories.FirstAsync(c => c.Name == "História Principal");
        var bossCategory = await _context.QuestCategories.FirstAsync(c => c.Name == "Boss Fights");
        var dungeonCategory = await _context.QuestCategories.FirstAsync(c => c.Name == "Dungeons");

        var quests = new List<Quest>
        {
            // Missão 1: Tutorial
            new Quest
            {
                Name = "Os Primeiros Passos: Bem-vindo à Guilda",
                Description = "Uma missão de introdução para novos aventureiros. Derrote alguns goblins para provar seu valor.",
                Type = "Main",
                Difficulty = "Easy",
                RequiredClass = "Any",
                RequiredLevel = 1,
                ExperienceReward = 100,
                GoldReward = 50,
                IsRepeatable = false,
                CategoryId = mainCategory.Id,
                Environment = QuestEnvironment.Forest,
                EnvironmentalCondition = null,
                EnvironmentalIntensity = 1,
                ImmuneClasses = JsonSerializer.Serialize(new[] { "Clérigo" }),
                ImmuneEnemyTypes = JsonSerializer.Serialize(new[] { "goblin" }),
                SpecialRewards = JsonSerializer.Serialize(new[] { "Espada de Iniciante", "Poção de Cura" }),
                BossId = 0,
                IsBossQuest = false,
                EstimatedDuration = 15,
                StoryOrder = 1,
                Prerequisites = JsonSerializer.Serialize(new int[0]),
                IsUnlocked = true
            },

            // Missão 2: Primeira Batalha Real
            new Quest
            {
                Name = "A Floresta Sombria: Lobos Selvagens",
                Description = "Explore a Floresta Sombria e enfrente uma matilha de lobos selvagens.",
                Type = "Main",
                Difficulty = "Easy",
                RequiredClass = "Any",
                RequiredLevel = 2,
                ExperienceReward = 200,
                GoldReward = 100,
                IsRepeatable = false,
                CategoryId = mainCategory.Id,
                Environment = QuestEnvironment.Forest,
                EnvironmentalCondition = EnvironmentalConditionType.Night,
                EnvironmentalIntensity = 2,
                ImmuneClasses = JsonSerializer.Serialize(new[] { "Ranger" }),
                ImmuneEnemyTypes = JsonSerializer.Serialize(new[] { "lobo" }),
                SpecialRewards = JsonSerializer.Serialize(new[] { "Armadura de Couro", "Poção de Força" }),
                BossId = 0,
                IsBossQuest = false,
                EstimatedDuration = 25,
                StoryOrder = 2,
                Prerequisites = JsonSerializer.Serialize(new[] { 1 }),
                IsUnlocked = false
            },

            // Missão 3: Primeira Dungeon
            new Quest
            {
                Name = "A Cripta Maldita: Esqueletos Guardiões",
                Description = "Explore uma cripta antiga e enfrente os esqueletos guardiões que a protegem.",
                Type = "Main",
                Difficulty = "Medium",
                RequiredClass = "Any",
                RequiredLevel = 5,
                ExperienceReward = 500,
                GoldReward = 250,
                IsRepeatable = false,
                CategoryId = dungeonCategory.Id,
                Environment = QuestEnvironment.Crypt,
                EnvironmentalCondition = EnvironmentalConditionType.Cold,
                EnvironmentalIntensity = 1,
                ImmuneClasses = JsonSerializer.Serialize(new[] { "Clérigo", "Paladino" }),
                ImmuneEnemyTypes = JsonSerializer.Serialize(new[] { "esqueleto" }),
                SpecialRewards = JsonSerializer.Serialize(new[] { "Armadura de Aço", "Anel de Proteção" }),
                BossId = 0,
                IsBossQuest = false,
                EstimatedDuration = 45,
                StoryOrder = 3,
                Prerequisites = JsonSerializer.Serialize(new[] { 2 }),
                IsUnlocked = false
            },

            // Missão 4: Primeiro Boss
            new Quest
            {
                Name = "O Orc Berserker: Fúria das Montanhas",
                Description = "Enfrente o temível Orc Berserker que aterroriza as montanhas.",
                Type = "Main",
                Difficulty = "Hard",
                RequiredClass = "Any",
                RequiredLevel = 8,
                ExperienceReward = 1000,
                GoldReward = 500,
                IsRepeatable = false,
                CategoryId = bossCategory.Id,
                Environment = QuestEnvironment.Mountain,
                EnvironmentalCondition = EnvironmentalConditionType.Storm,
                EnvironmentalIntensity = 3,
                ImmuneClasses = JsonSerializer.Serialize(new[] { "Bárbaro" }),
                ImmuneEnemyTypes = JsonSerializer.Serialize(new[] { "orc" }),
                SpecialRewards = JsonSerializer.Serialize(new[] { "Machado Berserker", "Armadura de Ferro", "Poção de Fúria" }),
                BossId = 4, // ID do Orc Berserker
                IsBossQuest = true,
                EstimatedDuration = 60,
                StoryOrder = 4,
                Prerequisites = JsonSerializer.Serialize(new[] { 3 }),
                IsUnlocked = false
            },

            // Missão 5: Dragão Jovem
            new Quest
            {
                Name = "O Dragão Jovem: Prova de Coragem",
                Description = "Enfrente um dragão jovem em sua caverna vulcânica. Uma prova de coragem para aventureiros experientes.",
                Type = "Main",
                Difficulty = "Epic",
                RequiredClass = "Any",
                RequiredLevel = 18,
                ExperienceReward = 3000,
                GoldReward = 1500,
                IsRepeatable = false,
                CategoryId = bossCategory.Id,
                Environment = QuestEnvironment.Volcano,
                EnvironmentalCondition = EnvironmentalConditionType.Heat,
                EnvironmentalIntensity = 3,
                ImmuneClasses = JsonSerializer.Serialize(new[] { "Mago", "Elementalista" }),
                ImmuneEnemyTypes = JsonSerializer.Serialize(new[] { "dragão" }),
                SpecialRewards = JsonSerializer.Serialize(new[] { "Espada de Dragão", "Armadura de Escamas", "Anel de Fogo" }),
                BossId = 6, // ID do Dragão Jovem
                IsBossQuest = true,
                EstimatedDuration = 90,
                StoryOrder = 5,
                Prerequisites = JsonSerializer.Serialize(new[] { 4 }),
                IsUnlocked = false
            },

            // Missão 6: Boss Final
            new Quest
            {
                Name = "O Lich Senhor das Trevas: Confronto Final",
                Description = "O confronto final contra o Lich Senhor das Trevas. A batalha que decidirá o destino do mundo.",
                Type = "Main",
                Difficulty = "Legendary",
                RequiredClass = "Any",
                RequiredLevel = 30,
                ExperienceReward = 10000,
                GoldReward = 5000,
                IsRepeatable = false,
                CategoryId = bossCategory.Id,
                Environment = QuestEnvironment.Crypt,
                EnvironmentalCondition = EnvironmentalConditionType.Fog,
                EnvironmentalIntensity = 3,
                ImmuneClasses = JsonSerializer.Serialize(new[] { "Clérigo", "Paladino", "Inquisidor" }),
                ImmuneEnemyTypes = JsonSerializer.Serialize(new[] { "lich" }),
                SpecialRewards = JsonSerializer.Serialize(new[] { "Espada Lendária", "Armadura Divina", "Anel do Poder", "Título de Herói" }),
                BossId = 7, // ID do Lich Senhor das Trevas
                IsBossQuest = true,
                EstimatedDuration = 120,
                StoryOrder = 6,
                Prerequisites = JsonSerializer.Serialize(new[] { 5 }),
                IsUnlocked = false
            }
        };

        _context.Quests.AddRange(quests);
        await _context.SaveChangesAsync();
    }
}

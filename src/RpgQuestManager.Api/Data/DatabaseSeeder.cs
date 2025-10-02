using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace RpgQuestManager.Api.Data;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        // Verificar se já tem dados
        if (await _context.Heroes.AnyAsync())
        {
            _logger.LogInformation("Banco de dados já contém dados. Seed ignorado.");
            return;
        }

        _logger.LogInformation("🌱 Iniciando seed do banco de dados...");

        await SeedUsersAsync();
        await SeedHeroesAsync();
        await SeedEnemiesAsync();
        await SeedItemsAsync();
        await SeedQuestsAsync();
        await SeedRewardsAsync();
        await SeedQuestEnemiesAsync();
        await SeedHeroItemsAsync();
        await SeedHeroQuestsAsync();
        await SeedBossDropTableAsync(); // Novo
        await SeedDiceInventoriesAsync(); // Novo

        _logger.LogInformation("✅ Seed do banco de dados concluído com sucesso!");
    }

    private async Task SeedUsersAsync()
    {
        var users = new List<User>
        {
            new User
            {
                Username = "admin",
                Email = "admin@eldoria.com",
                PasswordHash = HashPassword("admin123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new User
            {
                Username = "player1",
                Email = "player1@eldoria.com",
                PasswordHash = HashPassword("senha123"),
                Role = "Player",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new User
            {
                Username = "gamer",
                Email = "gamer@eldoria.com",
                PasswordHash = HashPassword("senha123"),
                Role = "Player",
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
        _logger.LogInformation("👤 {Count} usuários criados", users.Count);
    }

    private async Task SeedHeroesAsync()
    {
        var users = await _context.Users.ToListAsync();
        var player1 = users.FirstOrDefault(u => u.Username == "player1");
        var gamer = users.FirstOrDefault(u => u.Username == "gamer");

        var heroes = new List<Hero>
        {
            // Herói do player1
            new Hero
            {
                Name = "Aragorn",
                Class = "Guerreiro",
                Level = 15,
                Experience = 450,
                Strength = 40,
                Intelligence = 22,
                Dexterity = 28,
                Gold = 5000,
                UserId = player1?.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            },
            // Herói do gamer
            new Hero
            {
                Name = "Gandalf",
                Class = "Mago",
                Level = 20,
                Experience = 800,
                Strength = 18,
                Intelligence = 50,
                Dexterity = 24,
                Gold = 8000,
                UserId = gamer?.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-28)
            },
            new Hero
            {
                Name = "Legolas",
                Class = "Arqueiro",
                Level = 18,
                Experience = 650,
                Strength = 26,
                Intelligence = 28,
                Dexterity = 48,
                Gold = 6500,
                CreatedAt = DateTime.UtcNow.AddDays(-22)
            },
            
            // Heróis Experientes (Nível Médio)
            new Hero
            {
                Name = "Gimli",
                Class = "Guerreiro",
                Level = 12,
                Experience = 300,
                Strength = 36,
                Intelligence = 18,
                Dexterity = 20,
                Gold = 3500,
                CreatedAt = DateTime.UtcNow.AddDays(-18)
            },
            new Hero
            {
                Name = "Merlin",
                Class = "Mago",
                Level = 14,
                Experience = 200,
                Strength = 16,
                Intelligence = 42,
                Dexterity = 22,
                Gold = 4200,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new Hero
            {
                Name = "Robin Hood",
                Class = "Arqueiro",
                Level = 10,
                Experience = 500,
                Strength = 22,
                Intelligence = 20,
                Dexterity = 38,
                Gold = 2800,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new Hero
            {
                Name = "Arthas",
                Class = "Paladino",
                Level = 13,
                Experience = 450,
                Strength = 32,
                Intelligence = 30,
                Dexterity = 26,
                Gold = 4000,
                CreatedAt = DateTime.UtcNow.AddDays(-17)
            },
            
            // Heróis Iniciantes (Baixo Nível)
            new Hero
            {
                Name = "Frodo",
                Class = "Ladino",
                Level = 5,
                Experience = 80,
                Strength = 18,
                Intelligence = 20,
                Dexterity = 28,
                Gold = 800,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Hero
            {
                Name = "Boromir",
                Class = "Guerreiro",
                Level = 8,
                Experience = 150,
                Strength = 28,
                Intelligence = 16,
                Dexterity = 18,
                Gold = 1500,
                CreatedAt = DateTime.UtcNow.AddDays(-12)
            },
            new Hero
            {
                Name = "Galadriel",
                Class = "Mago",
                Level = 7,
                Experience = 200,
                Strength = 14,
                Intelligence = 32,
                Dexterity = 20,
                Gold = 1200,
                CreatedAt = DateTime.UtcNow.AddDays(-8)
            },
            new Hero
            {
                Name = "Elrond",
                Class = "Paladino",
                Level = 11,
                Experience = 350,
                Strength = 26,
                Intelligence = 34,
                Dexterity = 24,
                Gold = 3000,
                CreatedAt = DateTime.UtcNow.AddDays(-16)
            },
            new Hero
            {
                Name = "Arwen",
                Class = "Arqueiro",
                Level = 9,
                Experience = 250,
                Strength = 20,
                Intelligence = 26,
                Dexterity = 36,
                Gold = 2000,
                CreatedAt = DateTime.UtcNow.AddDays(-11)
            }
        };

        _context.Heroes.AddRange(heroes);
        await _context.SaveChangesAsync();
        _logger.LogInformation("⚔️ {Count} heróis criados", heroes.Count);
    }

    private async Task SeedEnemiesAsync()
    {
        var enemies = new List<Enemy>
        {
            // Inimigos Fracos (D6 - fácil)
            new Enemy { Name = "Goblin Raider", Type = "Goblin", Power = 15, Health = 50, RequiredDiceType = DiceType.D6, MinimumRoll = 3, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new Enemy { Name = "Orc Scout", Type = "Orc", Power = 25, Health = 80, RequiredDiceType = DiceType.D6, MinimumRoll = 4, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new Enemy { Name = "Skeleton Warrior", Type = "Morto-Vivo", Power = 20, Health = 60, RequiredDiceType = DiceType.D6, MinimumRoll = 3, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new Enemy { Name = "Wolf", Type = "Besta", Power = 18, Health = 55, RequiredDiceType = DiceType.D6, MinimumRoll = 3, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new Enemy { Name = "Giant Spider", Type = "Aranha", Power = 22, Health = 70, RequiredDiceType = DiceType.D6, MinimumRoll = 4, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            
            // Inimigos Médios (D8 - médio)
            new Enemy { Name = "Orc Warlord", Type = "Orc", Power = 45, Health = 150, RequiredDiceType = DiceType.D8, MinimumRoll = 5, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            new Enemy { Name = "Dark Wizard", Type = "Humano", Power = 50, Health = 120, RequiredDiceType = DiceType.D8, MinimumRoll = 6, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            new Enemy { Name = "Troll", Type = "Troll", Power = 55, Health = 200, RequiredDiceType = DiceType.D8, MinimumRoll = 5, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            new Enemy { Name = "Vampire", Type = "Morto-Vivo", Power = 48, Health = 140, RequiredDiceType = DiceType.D8, MinimumRoll = 6, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            new Enemy { Name = "Werewolf", Type = "Besta", Power = 52, Health = 160, RequiredDiceType = DiceType.D8, MinimumRoll = 5, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            
            // BOSSES (D12 e D20 - muito difícil, com drops especiais)
            new Enemy { Name = "Demon Lord", Type = "Demônio", Power = 75, Health = 300, RequiredDiceType = DiceType.D12, MinimumRoll = 8, IsBoss = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new Enemy { Name = "Elder Dragon", Type = "Dragão", Power = 90, Health = 500, RequiredDiceType = DiceType.D20, MinimumRoll = 15, IsBoss = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new Enemy { Name = "Lich King", Type = "Morto-Vivo", Power = 80, Health = 350, RequiredDiceType = DiceType.D12, MinimumRoll = 9, IsBoss = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new Enemy { Name = "Balrog", Type = "Demônio", Power = 95, Health = 450, RequiredDiceType = DiceType.D20, MinimumRoll = 16, IsBoss = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new Enemy { Name = "Kraken", Type = "Besta", Power = 85, Health = 400, RequiredDiceType = DiceType.D12, MinimumRoll = 10, IsBoss = true, CreatedAt = DateTime.UtcNow.AddDays(-15) }
        };

        _context.Enemies.AddRange(enemies);
        await _context.SaveChangesAsync();
        _logger.LogInformation("👹 {Count} inimigos criados ({BossCount} bosses com drops especiais)", 
            enemies.Count, enemies.Count(e => e.IsBoss));
    }

    private async Task SeedItemsAsync()
    {
        var items = new List<Item>
        {
            // COMUM - Itens compartilhados (podem dropar de qualquer boss)
            new Item { Name = "Poção de Cura Menor", Description = "Restaura um pouco de vida", Type = "Poção", Rarity = ItemRarity.Common, Value = 50 },
            new Item { Name = "Poção de Mana", Description = "Restaura energia mágica", Type = "Poção", Rarity = ItemRarity.Common, BonusIntelligence = 3, Value = 50 },
            new Item { Name = "Espada de Ferro", Description = "Uma espada simples mas eficaz", Type = "Espada", Rarity = ItemRarity.Common, BonusStrength = 5, Value = 100 },
            new Item { Name = "Armadura de Couro", Description = "Proteção leve para iniciantes", Type = "Armadura", Rarity = ItemRarity.Common, BonusStrength = 3, BonusDexterity = 2, Value = 150 },
            
            // RARO - Itens menos comuns
            new Item { Name = "Espada Élfica", Description = "Forjada pelos elfos de Rivendell", Type = "Espada", Rarity = ItemRarity.Rare, BonusStrength = 12, BonusDexterity = 5, Value = 500 },
            new Item { Name = "Cota de Malha", Description = "Armadura resistente de anéis de metal", Type = "Armadura", Rarity = ItemRarity.Rare, BonusStrength = 8, Value = 400 },
            new Item { Name = "Arco Longo Élfico", Description = "Arco de madeira élfica com precisão perfeita", Type = "Arco", Rarity = ItemRarity.Rare, BonusDexterity = 15, BonusStrength = 5, Value = 800 },
            
            // ÉPICO I-III - Itens muito raros (Boss Drops)
            new Item { Name = "Espada Flamejante", Description = "Uma lâmina envolta em chamas eternas do Demon Lord", Type = "Espada", Rarity = ItemRarity.Epic, RarityTier = 1, BonusStrength = 20, BonusIntelligence = 8, Value = 1500, IsExclusiveDrop = true },
            new Item { Name = "Escama do Dragão Ancião", Description = "Escama impenetrável do Elder Dragon", Type = "Armadura", Rarity = ItemRarity.Epic, RarityTier = 2, BonusStrength = 18, BonusDexterity = 12, Value = 2000, IsExclusiveDrop = true },
            new Item { Name = "Cajado do Lich", Description = "Cajado necromântico do Lich King", Type = "Cajado", Rarity = ItemRarity.Epic, RarityTier = 2, BonusIntelligence = 25, Value = 1800, IsExclusiveDrop = true },
            new Item { Name = "Chicote Flamejante", Description = "Arma do próprio Balrog", Type = "Chicote", Rarity = ItemRarity.Epic, RarityTier = 3, BonusStrength = 22, BonusIntelligence = 15, Value = 2200, IsExclusiveDrop = true },
            
            // LENDÁRIO I-III - Itens extremamente raros (Boss Drops Exclusivos)
            new Item { Name = "Lâmina Demoníaca: Corruptor", Description = "Espada lendária do Demon Lord. Quem a empunha sente o poder das trevas", Type = "Espada", Rarity = ItemRarity.Legendary, RarityTier = 1, BonusStrength = 30, BonusIntelligence = 20, Value = 5000, IsExclusiveDrop = true },
            new Item { Name = "Coração do Dragão", Description = "O coração ainda pulsante do Elder Dragon, concede poder supremo", Type = "Amuleto", Rarity = ItemRarity.Legendary, RarityTier = 3, BonusStrength = 25, BonusIntelligence = 25, BonusDexterity = 25, Value = 10000, IsExclusiveDrop = true },
            new Item { Name = "Coroa do Lich King", Description = "Coroa amaldiçoada que controla os mortos-vivos", Type = "Capacete", Rarity = ItemRarity.Legendary, RarityTier = 2, BonusIntelligence = 35, Value = 7000, IsExclusiveDrop = true },
            new Item { Name = "Foice do Balrog", Description = "Arma ancestral forjada no coração da montanha", Type = "Foice", Rarity = ItemRarity.Legendary, RarityTier = 3, BonusStrength = 40, BonusDexterity = 15, Value = 8000, IsExclusiveDrop = true },
            new Item { Name = "Tentáculo do Kraken", Description = "Tentáculo arrancado do próprio Kraken, ainda se move!", Type = "Açoite", Rarity = ItemRarity.Legendary, RarityTier = 2, BonusStrength = 28, BonusDexterity = 22, Value = 6500, IsExclusiveDrop = true },
            
            // Itens gerais adicionais
            new Item { Name = "Escudo de Madeira", Description = "Escudo básico de proteção", Type = "Escudo", Rarity = ItemRarity.Common, BonusStrength = 4, Value = 80 },
            new Item { Name = "Manto Mágico", Description = "Manto que amplifica poderes mágicos", Type = "Armadura", Rarity = ItemRarity.Rare, BonusIntelligence = 12, Value = 900 }
        };

        _context.Items.AddRange(items);
        await _context.SaveChangesAsync();
        _logger.LogInformation("🗡️ {Count} itens criados ({Epic} épicos, {Legendary} lendários)", 
            items.Count, 
            items.Count(i => i.Rarity == ItemRarity.Epic),
            items.Count(i => i.Rarity == ItemRarity.Legendary));
    }

    private async Task SeedQuestsAsync()
    {
        var quests = new List<Quest>
        {
            // Quests Fáceis
            new Quest
            {
                Name = "Caça aos Goblins",
                Description = "Uma horda de goblins está atacando fazendas próximas. Elimine-os!",
                Difficulty = "Fácil",
                ExperienceReward = 50,
                GoldReward = 100,
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            },
            new Quest
            {
                Name = "Coleta de Ervas",
                Description = "Colete 20 ervas medicinais na floresta",
                Difficulty = "Fácil",
                ExperienceReward = 30,
                GoldReward = 75,
                CreatedAt = DateTime.UtcNow.AddDays(-24)
            },
            new Quest
            {
                Name = "Limpeza do Cemitério",
                Description = "Esqueletos emergiram do cemitério antigo. Devolva-os ao descanso eterno.",
                Difficulty = "Fácil",
                ExperienceReward = 60,
                GoldReward = 120,
                CreatedAt = DateTime.UtcNow.AddDays(-23)
            },
            
            // Quests Médias
            new Quest
            {
                Name = "A Invasão Orc",
                Description = "Um exército orc se aproxima da cidade. Derrote o líder!",
                Difficulty = "Médio",
                ExperienceReward = 150,
                GoldReward = 400,
                CreatedAt = DateTime.UtcNow.AddDays(-22)
            },
            new Quest
            {
                Name = "O Feiticeiro das Sombras",
                Description = "Um feiticeiro negro está amaldiçoando aldeões. Pare-o!",
                Difficulty = "Médio",
                ExperienceReward = 180,
                GoldReward = 500,
                CreatedAt = DateTime.UtcNow.AddDays(-21)
            },
            new Quest
            {
                Name = "Caçada ao Lobisomem",
                Description = "Um lobisomem aterroriza viajantes nas noites de lua cheia",
                Difficulty = "Médio",
                ExperienceReward = 200,
                GoldReward = 600,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new Quest
            {
                Name = "O Covil do Troll",
                Description = "Entre na caverna do troll e recupere o tesouro roubado",
                Difficulty = "Médio",
                ExperienceReward = 170,
                GoldReward = 450,
                CreatedAt = DateTime.UtcNow.AddDays(-19)
            },
            
            // Quests Difíceis
            new Quest
            {
                Name = "O Vampiro Ancestral",
                Description = "Um vampiro antigo desperta. Apenas os mais corajosos ousam enfrentá-lo.",
                Difficulty = "Difícil",
                ExperienceReward = 350,
                GoldReward = 1000,
                CreatedAt = DateTime.UtcNow.AddDays(-18)
            },
            new Quest
            {
                Name = "A Torre do Necromante",
                Description = "Suba a torre do necromante e impeça seus planos sombrios",
                Difficulty = "Difícil",
                ExperienceReward = 400,
                GoldReward = 1200,
                CreatedAt = DateTime.UtcNow.AddDays(-17)
            },
            new Quest
            {
                Name = "O Dragão das Montanhas",
                Description = "Um dragão anciã está queimando vilas. Derrote-o e salve o reino!",
                Difficulty = "Difícil",
                ExperienceReward = 450,
                GoldReward = 1500,
                CreatedAt = DateTime.UtcNow.AddDays(-16)
            },
            
            // Quests Épicas
            new Quest
            {
                Name = "O Despertar do Balrog",
                Description = "Um demônio das profundezas emerge. O destino do mundo está em suas mãos.",
                Difficulty = "Épico",
                ExperienceReward = 800,
                GoldReward = 3000,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new Quest
            {
                Name = "O Senhor dos Liches",
                Description = "O Rei Lich planeja mergulhar o mundo em escuridão eterna",
                Difficulty = "Épico",
                ExperienceReward = 1000,
                GoldReward = 5000,
                CreatedAt = DateTime.UtcNow.AddDays(-14)
            },
            new Quest
            {
                Name = "Kraken dos Mares Profundos",
                Description = "A besta marinha lendária ameaça destruir a frota",
                Difficulty = "Épico",
                ExperienceReward = 900,
                GoldReward = 4000,
                CreatedAt = DateTime.UtcNow.AddDays(-13)
            }
        };

        _context.Quests.AddRange(quests);
        await _context.SaveChangesAsync();
        _logger.LogInformation("🎯 {Count} quests criadas", quests.Count);
    }

    private async Task SeedRewardsAsync()
    {
        var quests = await _context.Quests.ToListAsync();
        var items = await _context.Items.ToListAsync();
        var rewards = new List<Reward>();

        var questRewardMap = new Dictionary<string, int?>
        {
            { "Caça aos Goblins", items.FirstOrDefault(i => i.Name == "Espada de Ferro")?.Id },
            { "Coleta de Ervas", items.FirstOrDefault(i => i.Name == "Poção de Cura Menor")?.Id },
            { "Limpeza do Cemitério", items.FirstOrDefault(i => i.Name == "Armadura de Couro")?.Id },
            { "A Invasão Orc", items.FirstOrDefault(i => i.Name == "Cota de Malha")?.Id },
            { "O Feiticeiro das Sombras", items.FirstOrDefault(i => i.Name == "Cajado Ancestral")?.Id },
            { "Caçada ao Lobisomem", items.FirstOrDefault(i => i.Name == "Poção de Força")?.Id },
            { "O Covil do Troll", items.FirstOrDefault(i => i.Name == "Escudo de Madeira")?.Id },
            { "O Vampiro Ancestral", items.FirstOrDefault(i => i.Name == "Manto Mágico")?.Id },
            { "A Maldição do Templo", items.FirstOrDefault(i => i.Name == "Poção de Mana")?.Id },
            { "O Dragão das Montanhas", items.FirstOrDefault(i => i.Name == "Espada Flamejante")?.Id },
            { "O Despertar do Balrog", items.FirstOrDefault(i => i.Name == "Armadura de Placas")?.Id },
            { "O Senhor dos Liches", items.FirstOrDefault(i => i.Name == "Escudo de Mithril")?.Id },
            { "Kraken dos Mares", items.FirstOrDefault(i => i.Name == "Espada Élfica")?.Id }
        };

        foreach (var quest in quests)
        {
            questRewardMap.TryGetValue(quest.Name, out var itemId);
            
            rewards.Add(new Reward
            {
                QuestId = quest.Id,
                Gold = quest.GoldReward,
                Experience = quest.ExperienceReward,
                ItemId = itemId,
                ItemQuantity = itemId.HasValue ? 1 : 0,
                CreatedAt = quest.CreatedAt
            });
        }

        _context.Rewards.AddRange(rewards);
        await _context.SaveChangesAsync();
        _logger.LogInformation("💰 {Count} recompensas criadas", rewards.Count);
    }

    private async Task SeedQuestEnemiesAsync()
    {
        var quests = await _context.Quests.ToListAsync();
        var enemies = await _context.Enemies.ToListAsync();
        var questEnemies = new List<QuestEnemy>();

        // Quest 1: Caça aos Goblins
        questEnemies.Add(new QuestEnemy { QuestId = quests[0].Id, EnemyId = enemies[0].Id, Quantity = 10 });
        
        // Quest 2: Limpeza do Cemitério
        questEnemies.Add(new QuestEnemy { QuestId = quests[2].Id, EnemyId = enemies[2].Id, Quantity = 15 });
        
        // Quest 3: A Invasão Orc
        questEnemies.Add(new QuestEnemy { QuestId = quests[3].Id, EnemyId = enemies[1].Id, Quantity = 5 });
        questEnemies.Add(new QuestEnemy { QuestId = quests[3].Id, EnemyId = enemies[5].Id, Quantity = 1 });
        
        // Quest 4: O Feiticeiro das Sombras
        questEnemies.Add(new QuestEnemy { QuestId = quests[4].Id, EnemyId = enemies[6].Id, Quantity = 1 });
        
        // Quest 5: Caçada ao Lobisomem
        questEnemies.Add(new QuestEnemy { QuestId = quests[5].Id, EnemyId = enemies[9].Id, Quantity = 1 });
        
        // Quest 6: O Covil do Troll
        questEnemies.Add(new QuestEnemy { QuestId = quests[6].Id, EnemyId = enemies[7].Id, Quantity = 1 });
        
        // Quest 7: O Vampiro Ancestral
        questEnemies.Add(new QuestEnemy { QuestId = quests[7].Id, EnemyId = enemies[8].Id, Quantity = 1 });
        
        // Quest 8: O Dragão das Montanhas
        questEnemies.Add(new QuestEnemy { QuestId = quests[9].Id, EnemyId = enemies[11].Id, Quantity = 1 });
        
        // Quest 9: O Despertar do Balrog
        questEnemies.Add(new QuestEnemy { QuestId = quests[10].Id, EnemyId = enemies[13].Id, Quantity = 1 });
        
        // Quest 10: O Senhor dos Liches
        questEnemies.Add(new QuestEnemy { QuestId = quests[11].Id, EnemyId = enemies[12].Id, Quantity = 1 });
        
        // Quest 11: Kraken dos Mares
        questEnemies.Add(new QuestEnemy { QuestId = quests[12].Id, EnemyId = enemies[14].Id, Quantity = 1 });

        _context.QuestEnemies.AddRange(questEnemies);
        await _context.SaveChangesAsync();
        _logger.LogInformation("⚔️ {Count} relacionamentos quest-inimigo criados", questEnemies.Count);
    }

    private async Task SeedHeroItemsAsync()
    {
        var heroes = await _context.Heroes.ToListAsync();
        var items = await _context.Items.ToListAsync();
        var heroItems = new List<HeroItem>();

        // Aragorn (Guerreiro) - Equipado para combate
        heroItems.Add(new HeroItem { HeroId = heroes[0].Id, ItemId = items[2].Id, Quantity = 1, IsEquipped = true }); // Espada Flamejante
        heroItems.Add(new HeroItem { HeroId = heroes[0].Id, ItemId = items[7].Id, Quantity = 1, IsEquipped = true }); // Armadura de Placas
        heroItems.Add(new HeroItem { HeroId = heroes[0].Id, ItemId = items[11].Id, Quantity = 3, IsEquipped = false }); // Poções de Cura

        // Gandalf (Mago) - Equipado para magia
        heroItems.Add(new HeroItem { HeroId = heroes[1].Id, ItemId = items[3].Id, Quantity = 1, IsEquipped = true }); // Cajado Ancestral
        heroItems.Add(new HeroItem { HeroId = heroes[1].Id, ItemId = items[8].Id, Quantity = 1, IsEquipped = true }); // Manto Mágico
        heroItems.Add(new HeroItem { HeroId = heroes[1].Id, ItemId = items[13].Id, Quantity = 2, IsEquipped = false }); // Poção de Mana

        // Legolas (Arqueiro) - Equipado para precisão
        heroItems.Add(new HeroItem { HeroId = heroes[2].Id, ItemId = items[4].Id, Quantity = 1, IsEquipped = true }); // Arco Élfico
        heroItems.Add(new HeroItem { HeroId = heroes[2].Id, ItemId = items[5].Id, Quantity = 1, IsEquipped = true }); // Armadura de Couro
        heroItems.Add(new HeroItem { HeroId = heroes[2].Id, ItemId = items[14].Id, Quantity = 2, IsEquipped = false }); // Elixir de Agilidade

        // Adicionar alguns itens para outros heróis também
        for (int i = 3; i < Math.Min(8, heroes.Count); i++)
        {
            heroItems.Add(new HeroItem { HeroId = heroes[i].Id, ItemId = items[0].Id, Quantity = 1, IsEquipped = true });
            heroItems.Add(new HeroItem { HeroId = heroes[i].Id, ItemId = items[11].Id, Quantity = 1, IsEquipped = false });
        }

        _context.HeroItems.AddRange(heroItems);
        await _context.SaveChangesAsync();
        _logger.LogInformation("🎒 {Count} itens adicionados aos inventários", heroItems.Count);
    }

    private async Task SeedHeroQuestsAsync()
    {
        var heroes = await _context.Heroes.ToListAsync();
        var quests = await _context.Quests.ToListAsync();
        var heroQuests = new List<HeroQuest>();

        // Aragorn completou várias quests
        heroQuests.Add(new HeroQuest { HeroId = heroes[0].Id, QuestId = quests[0].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-20), CompletedAt = DateTime.UtcNow.AddDays(-20) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[0].Id, QuestId = quests[2].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-18), CompletedAt = DateTime.UtcNow.AddDays(-18) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[0].Id, QuestId = quests[3].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-15), CompletedAt = DateTime.UtcNow.AddDays(-15) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[0].Id, QuestId = quests[7].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-10), CompletedAt = DateTime.UtcNow.AddDays(-10) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[0].Id, QuestId = quests[10].Id, IsCompleted = false, StartedAt = DateTime.UtcNow.AddDays(-2) });

        // Gandalf completou quests épicas
        heroQuests.Add(new HeroQuest { HeroId = heroes[1].Id, QuestId = quests[4].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-16), CompletedAt = DateTime.UtcNow.AddDays(-16) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[1].Id, QuestId = quests[8].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-12), CompletedAt = DateTime.UtcNow.AddDays(-12) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[1].Id, QuestId = quests[10].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-8), CompletedAt = DateTime.UtcNow.AddDays(-8) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[1].Id, QuestId = quests[11].Id, IsCompleted = false, StartedAt = DateTime.UtcNow.AddDays(-1) });

        // Legolas
        heroQuests.Add(new HeroQuest { HeroId = heroes[2].Id, QuestId = quests[0].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-19), CompletedAt = DateTime.UtcNow.AddDays(-19) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[2].Id, QuestId = quests[5].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-14), CompletedAt = DateTime.UtcNow.AddDays(-14) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[2].Id, QuestId = quests[9].Id, IsCompleted = false, StartedAt = DateTime.UtcNow.AddDays(-3) });

        // Outros heróis com quests em andamento
        for (int i = 3; i < Math.Min(7, heroes.Count); i++)
        {
            heroQuests.Add(new HeroQuest { HeroId = heroes[i].Id, QuestId = quests[0].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-10), CompletedAt = DateTime.UtcNow.AddDays(-9) });
            heroQuests.Add(new HeroQuest { HeroId = heroes[i].Id, QuestId = quests[1].Id, IsCompleted = false, StartedAt = DateTime.UtcNow.AddDays(-5) });
        }

        _context.HeroQuests.AddRange(heroQuests);
        await _context.SaveChangesAsync();
        _logger.LogInformation("📜 {Count} quests atribuídas aos heróis", heroQuests.Count);
    }

    private async Task SeedBossDropTableAsync()
    {
        var enemies = await _context.Enemies.ToListAsync();
        var items = await _context.Items.ToListAsync();

        var bosses = enemies.Where(e => e.IsBoss).ToList();
        if (!bosses.Any())
        {
            _logger.LogWarning("Nenhum boss encontrado para configurar drops");
            return;
        }

        var dropTables = new List<BossDropTable>();

        // Itens comuns que todos os bosses podem dropar (60% chance)
        var commonItems = items.Where(i => i.Rarity == ItemRarity.Common).ToList();

        foreach (var boss in bosses)
        {
            // Cada boss dropa 2-3 itens comuns
            for (int i = 0; i < Math.Min(3, commonItems.Count); i++)
            {
                dropTables.Add(new BossDropTable
                {
                    EnemyId = boss.Id,
                    ItemId = commonItems[i].Id,
                    DropChance = 0.60m, // 60%
                    IsExclusive = false
                });
            }
        }

        // Demon Lord Drops
        var demonLord = bosses.FirstOrDefault(b => b.Name == "Demon Lord");
        if (demonLord != null)
        {
            var espadaFlamejante = items.FirstOrDefault(i => i.Name == "Espada Flamejante");
            var laminaDemoniaca = items.FirstOrDefault(i => i.Name == "Lâmina Demoníaca: Corruptor");
            
            if (espadaFlamejante != null)
                dropTables.Add(new BossDropTable { EnemyId = demonLord.Id, ItemId = espadaFlamejante.Id, DropChance = 0.30m, IsExclusive = true });
            if (laminaDemoniaca != null)
                dropTables.Add(new BossDropTable { EnemyId = demonLord.Id, ItemId = laminaDemoniaca.Id, DropChance = 0.10m, IsExclusive = true });
        }

        // Elder Dragon Drops
        var elderDragon = bosses.FirstOrDefault(b => b.Name == "Elder Dragon");
        if (elderDragon != null)
        {
            var escamaDragao = items.FirstOrDefault(i => i.Name == "Escama do Dragão Ancião");
            var coracaoDragao = items.FirstOrDefault(i => i.Name == "Coração do Dragão");
            
            if (escamaDragao != null)
                dropTables.Add(new BossDropTable { EnemyId = elderDragon.Id, ItemId = escamaDragao.Id, DropChance = 0.35m, IsExclusive = true });
            if (coracaoDragao != null)
                dropTables.Add(new BossDropTable { EnemyId = elderDragon.Id, ItemId = coracaoDragao.Id, DropChance = 0.05m, IsExclusive = true }); // 5% - MUITO RARO!
        }

        // Lich King Drops
        var lichKing = bosses.FirstOrDefault(b => b.Name == "Lich King");
        if (lichKing != null)
        {
            var cajadoLich = items.FirstOrDefault(i => i.Name == "Cajado do Lich");
            var coroaLich = items.FirstOrDefault(i => i.Name == "Coroa do Lich King");
            
            if (cajadoLich != null)
                dropTables.Add(new BossDropTable { EnemyId = lichKing.Id, ItemId = cajadoLich.Id, DropChance = 0.30m, IsExclusive = true });
            if (coroaLich != null)
                dropTables.Add(new BossDropTable { EnemyId = lichKing.Id, ItemId = coroaLich.Id, DropChance = 0.08m, IsExclusive = true });
        }

        // Balrog Drops
        var balrog = bosses.FirstOrDefault(b => b.Name == "Balrog");
        if (balrog != null)
        {
            var chicoteFlamejante = items.FirstOrDefault(i => i.Name == "Chicote Flamejante");
            var foiceBalrog = items.FirstOrDefault(i => i.Name == "Foice do Balrog");
            
            if (chicoteFlamejante != null)
                dropTables.Add(new BossDropTable { EnemyId = balrog.Id, ItemId = chicoteFlamejante.Id, DropChance = 0.25m, IsExclusive = true });
            if (foiceBalrog != null)
                dropTables.Add(new BossDropTable { EnemyId = balrog.Id, ItemId = foiceBalrog.Id, DropChance = 0.06m, IsExclusive = true });
        }

        // Kraken Drops
        var kraken = bosses.FirstOrDefault(b => b.Name == "Kraken");
        if (kraken != null)
        {
            var tentaculoKraken = items.FirstOrDefault(i => i.Name == "Tentáculo do Kraken");
            
            if (tentaculoKraken != null)
                dropTables.Add(new BossDropTable { EnemyId = kraken.Id, ItemId = tentaculoKraken.Id, DropChance = 0.12m, IsExclusive = true });
        }

        _context.BossDropTables.AddRange(dropTables);
        await _context.SaveChangesAsync();
        _logger.LogInformation("🎁 {Count} drops configurados para {BossCount} bosses", dropTables.Count, bosses.Count);
    }

    private async Task SeedDiceInventoriesAsync()
    {
        var heroes = await _context.Heroes.ToListAsync();
        var inventories = new List<DiceInventory>();

        // Heróis de nível mais alto começam com mais dados
        foreach (var hero in heroes)
        {
            var inventory = new DiceInventory
            {
                HeroId = hero.Id,
                D6Count = 3 + (hero.Level / 5), // +1 D6 a cada 5 níveis
                D8Count = hero.Level >= 10 ? 2 : 0,
                D12Count = hero.Level >= 15 ? 1 : 0,
                D20Count = hero.Level >= 18 ? 1 : 0
            };

            inventories.Add(inventory);
        }

        _context.DiceInventories.AddRange(inventories);
        await _context.SaveChangesAsync();
        _logger.LogInformation("🎲 {Count} inventários de dados criados para os heróis", inventories.Count);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}


using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class RewardService
{
    private readonly ApplicationDbContext _db;
    private readonly InventoryService _inventoryService;
    private readonly DiceInventoryService _diceInventoryService;
    private readonly DiceService _diceService;

    public RewardService(
        ApplicationDbContext db, 
        InventoryService inventoryService, 
        DiceInventoryService diceInventoryService,
        DiceService diceService)
    {
        _db = db;
        _inventoryService = inventoryService;
        _diceInventoryService = diceInventoryService;
        _diceService = diceService;
    }

    public async Task<CombatRewards> GenerateCombatRewardsAsync(int heroId, int questId, int monsterId)
    {
        var hero = await _db.Heroes.FindAsync(heroId);
        var monster = await _db.Monsters.FindAsync(heroId);
        var quest = await _db.Quests.FindAsync(questId);

        if (hero == null || monster == null || quest == null)
        {
            throw new ArgumentException("Hero, Monster or Quest not found");
        }

        var combatRewards = new CombatRewards
        {
            HeroId = heroId,
            QuestId = questId,
            MonsterId = monsterId,
            CreatedAt = DateTime.UtcNow
        };

        var rewards = new List<CombatReward>();

        // Recompensas fixas de combate
        var goldReward = CalculateGoldReward(monster, quest);
        rewards.Add(new CombatReward
        {
            Type = RewardType.Gold,
            Name = "Ouro",
            Description = $"Ouro ganho derrotando {monster.Name}",
            GoldAmount = goldReward,
            Quantity = goldReward,
            Icon = "💰"
        });

        var expReward = CalculateExperienceReward(monster, quest);
        rewards.Add(new CombatReward
        {
            Type = RewardType.Experience,
            Name = "Experiência",
            Description = $"Experiência ganha derrotando {monster.Name}",
            ExperienceAmount = expReward,
            Quantity = expReward,
            Icon = "⭐"
        });

        // Drops aleatórios de itens
        var itemDrops = await GenerateItemDropsAsync(monster);
        rewards.AddRange(itemDrops);

        // Drops aleatórios de dados
        var diceDrops = GenerateDiceDrops(monster);
        rewards.AddRange(diceDrops);

        combatRewards.Rewards = rewards;
        
        _db.CombatRewards.Add(combatRewards);
        await _db.SaveChangesAsync();

        return combatRewards;
    }

    public async Task<QuestRewards> GenerateQuestRewardsAsync(int heroId, int questId)
    {
        var hero = await _db.Heroes.FindAsync(heroId);
        var quest = await _db.Quests.FindAsync(questId);

        if (hero == null || quest == null)
        {
            throw new ArgumentException("Hero or Quest not found");
        }

        var questRewards = new QuestRewards
        {
            HeroId = heroId,
            QuestId = questId,
            CreatedAt = DateTime.UtcNow
        };

        var rewards = new List<QuestReward>();

        // Recompensas fixas da missão
        rewards.Add(new QuestReward
        {
            Type = RewardType.Gold,
            Name = "Ouro da Missão",
            Description = $"Recompensa em ouro por completar: {quest.Title}",
            GoldAmount = quest.GoldReward,
            Quantity = quest.GoldReward,
            Icon = "🏆"
        });

        rewards.Add(new QuestReward
        {
            Type = RewardType.Experience,
            Name = "Experiência da Missão",
            Description = $"Experiência ganha completando: {quest.Title}",
            ExperienceAmount = quest.ExperienceReward,
            Quantity = quest.ExperienceReward,
            Icon = "🎯"
        });

        // Itens especiais da missão (se houver)
        var questItems = await GenerateQuestItemRewardsAsync(quest);
        rewards.AddRange(questItems);

        questRewards.Rewards = rewards;
        
        _db.QuestRewards.Add(questRewards);
        await _db.SaveChangesAsync();

        return questRewards;
    }

    public async Task<bool> ClaimCombatRewardsAsync(int combatRewardsId, int heroId)
    {
        var combatRewards = await _db.CombatRewards
            .Include(cr => cr.Rewards)
            .FirstOrDefaultAsync(cr => cr.Id == combatRewardsId && cr.HeroId == heroId);

        if (combatRewards == null || combatRewards.IsClaimed)
        {
            return false;
        }

        var hero = await _db.Heroes.FindAsync(heroId);
        if (hero == null) return false;

        foreach (var reward in combatRewards.Rewards)
        {
            switch (reward.Type)
            {
                case RewardType.Gold:
                    hero.Gold += reward.GoldAmount ?? 0;
                    break;
                case RewardType.Experience:
                    hero.Experience += reward.ExperienceAmount ?? 0;
                    break;
                case RewardType.Item:
                    if (reward.ItemId.HasValue)
                    {
                        await _inventoryService.AddItemAsync(heroId, reward.ItemId.Value, reward.Quantity);
                    }
                    break;
                case RewardType.Dice:
                    if (reward.DiceType.HasValue)
                    {
                        await _diceInventoryService.AddDiceAsync(heroId, reward.DiceType.Value, reward.Quantity);
                    }
                    break;
            }
        }

        combatRewards.IsClaimed = true;
        combatRewards.ClaimedAt = DateTime.UtcNow;
        
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClaimQuestRewardsAsync(int questRewardsId, int heroId)
    {
        var questRewards = await _db.QuestRewards
            .Include(qr => qr.Rewards)
            .FirstOrDefaultAsync(qr => qr.Id == questRewardsId && qr.HeroId == heroId);

        if (questRewards == null || questRewards.IsClaimed)
        {
            return false;
        }

        var hero = await _db.Heroes.FindAsync(heroId);
        if (hero == null) return false;

        foreach (var reward in questRewards.Rewards)
        {
            switch (reward.Type)
            {
                case RewardType.Gold:
                    hero.Gold += reward.GoldAmount ?? 0;
                    break;
                case RewardType.Experience:
                    hero.Experience += reward.ExperienceAmount ?? 0;
                    break;
                case RewardType.Item:
                    if (reward.ItemId.HasValue)
                    {
                        await _inventoryService.AddItemAsync(heroId, reward.ItemId.Value, reward.Quantity);
                    }
                    break;
                case RewardType.Dice:
                    if (reward.DiceType.HasValue)
                    {
                        await _diceInventoryService.AddDiceAsync(heroId, reward.DiceType.Value, reward.Quantity);
                    }
                    break;
            }
        }

        questRewards.IsClaimed = true;
        questRewards.ClaimedAt = DateTime.UtcNow;
        
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<CombatRewards>> GetUnclaimedCombatRewardsAsync(int heroId)
    {
        return await _db.CombatRewards
            .Include(cr => cr.Rewards)
            .Include(cr => cr.Monster)
            .Include(cr => cr.Quest)
            .Where(cr => cr.HeroId == heroId && !cr.IsClaimed)
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<QuestRewards>> GetUnclaimedQuestRewardsAsync(int heroId)
    {
        return await _db.QuestRewards
            .Include(qr => qr.Rewards)
            .Include(qr => qr.Quest)
            .Where(qr => qr.HeroId == heroId && !qr.IsClaimed)
            .OrderByDescending(qr => qr.CreatedAt)
            .ToListAsync();
    }

    private int CalculateGoldReward(Monster monster, Quest quest)
    {
        // Ouro base do monstro + bônus da missão
        var baseGold = monster.Level * 10 + _diceService.RollDice(20);
        var questBonus = quest.GoldReward / 4; // 25% do ouro da missão como bônus de combate
        return baseGold + questBonus;
    }

    private int CalculateExperienceReward(Monster monster, Quest quest)
    {
        // Experiência base do monstro + bônus da missão
        var baseExp = monster.Level * 15 + _diceService.RollDice(10);
        var questBonus = quest.ExperienceReward / 3; // 33% da exp da missão como bônus de combate
        return baseExp + questBonus;
    }

    private async Task<List<CombatReward>> GenerateItemDropsAsync(Monster monster)
    {
        var drops = new List<CombatReward>();
        
        // Chance base de drop de item (15%)
        if (_diceService.RollPercentage(15))
        {
            // Buscar itens que podem ser dropados por este tipo de monstro
            var availableItems = await _db.Items
                .Where(i => i.DroppedBy.Contains(monster.Type))
                .ToListAsync();

            if (availableItems.Any())
            {
                var randomItem = availableItems[_diceService.RollDice(availableItems.Count) - 1];
                var quantity = randomItem.StackSize > 1 ? _diceService.RollDice(randomItem.StackSize) : 1;
                
                drops.Add(new CombatReward
                {
                    Type = RewardType.Item,
                    Name = randomItem.Name,
                    Description = $"Item dropado por {monster.Name}",
                    ItemId = randomItem.Id,
                    Quantity = quantity,
                    Rarity = randomItem.Rarity,
                    Icon = GetItemIcon(randomItem.Type)
                });
            }
        }

        return drops;
    }

    private List<CombatReward> GenerateDiceDrops(Monster monster)
    {
        var drops = new List<CombatReward>();
        
        // Chance de drop de dados (8%)
        if (_diceService.RollPercentage(8))
        {
            // Dados mais raros têm menor chance
            var diceTypes = new[]
            {
                (DiceType.D4, 40),
                (DiceType.D8, 25),
                (DiceType.D10, 20),
                (DiceType.D12, 10),
                (DiceType.D20, 5)
            };

            var totalWeight = diceTypes.Sum(dt => dt.Item2);
            var roll = _diceService.RollDice(totalWeight);
            var currentWeight = 0;

            foreach (var (diceType, weight) in diceTypes)
            {
                currentWeight += weight;
                if (roll <= currentWeight)
                {
                    var quantity = _diceService.RollDice(3); // 1-3 dados
                    
                    drops.Add(new CombatReward
                    {
                        Type = RewardType.Dice,
                        Name = $"Dado {diceType.ToString().ToUpper()}",
                        Description = $"{quantity}x {diceType.ToString().ToUpper()} dropado por {monster.Name}",
                        DiceType = diceType,
                        Quantity = quantity,
                        Icon = "🎲"
                    });
                    break;
                }
            }
        }

        return drops;
    }

    private async Task<List<QuestReward>> GenerateQuestItemRewardsAsync(Quest quest)
    {
        var rewards = new List<QuestReward>();
        
        // Missões mais difíceis têm chance de dar itens especiais
        var itemChance = quest.Difficulty switch
        {
            QuestDifficulty.Easy => 5,
            QuestDifficulty.Medium => 15,
            QuestDifficulty.Hard => 30,
            QuestDifficulty.Epic => 50,
            QuestDifficulty.Legendary => 70,
            _ => 10
        };

        if (_diceService.RollPercentage(itemChance))
        {
            // Buscar itens especiais baseados na categoria da missão
            var specialItems = await _db.Items
                .Where(i => i.Rarity >= ItemRarity.Rare)
                .ToListAsync();

            if (specialItems.Any())
            {
                var randomItem = specialItems[_diceService.RollDice(specialItems.Count) - 1];
                
                rewards.Add(new QuestReward
                {
                    Type = RewardType.Item,
                    Name = randomItem.Name,
                    Description = $"Item especial ganho completando: {quest.Title}",
                    ItemId = randomItem.Id,
                    Quantity = 1,
                    Rarity = randomItem.Rarity,
                    Icon = GetItemIcon(randomItem.Type)
                });
            }
        }

        return rewards;
    }

    private string GetItemIcon(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Weapon => "⚔️",
            ItemType.Armor => "🛡️",
            ItemType.Potion => "🧪",
            ItemType.Material => "💎",
            ItemType.Accessory => "🔧",
            ItemType.Scroll => "📜",
            ItemType.Quest => "📋",
            ItemType.Currency => "💰",
            _ => "📦"
        };
    }
}

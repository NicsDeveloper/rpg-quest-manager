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
    private readonly DropService _dropService;

    public RewardService(
        ApplicationDbContext db, 
        InventoryService inventoryService, 
        DiceInventoryService diceInventoryService,
        DiceService diceService,
        DropService dropService)
    {
        _db = db;
        _inventoryService = inventoryService;
        _diceInventoryService = diceInventoryService;
        _diceService = diceService;
        _dropService = dropService;
    }

    public async Task<CombatRewards> GenerateCombatRewardsAsync(int heroId, int questId, int monsterId)
    {
        var hero = await _db.Heroes.FindAsync(heroId);
        var monster = await _db.Monsters.FindAsync(monsterId);
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

        // Ouro baseado no drop do monstro (usando DropService)
        var goldReward = await _dropService.CalculateGoldRewardAsync(monsterId, hero.Level);
        rewards.Add(new CombatReward
        {
            Type = RewardType.Gold,
            Name = "Ouro",
            Description = $"Ouro dropado por {monster.Name}",
            GoldAmount = goldReward,
            Quantity = goldReward,
            Icon = "💰"
        });

        // Experiência baseada no monstro (usando valor real do monstro)
        var expReward = monster.ExperienceReward;
        rewards.Add(new CombatReward
        {
            Type = RewardType.Experience,
            Name = "Experiência",
            Description = $"Experiência ganha derrotando {monster.Name}",
            ExperienceAmount = expReward,
            Quantity = expReward,
            Icon = "⭐"
        });

        // Drops reais de itens do monstro (usando DropService)
        var itemDrops = await _dropService.RollMonsterDropsAsync(monsterId, hero.Level);
        foreach (var item in itemDrops)
        {
            rewards.Add(new CombatReward
            {
                Type = RewardType.Item,
                Name = item.Name,
                Description = $"Item dropado por {monster.Name}",
                ItemId = item.Id,
                Quantity = 1,
                Rarity = item.Rarity,
                Icon = GetItemIcon(item.Type)
            });
        }

        // Drops de dados (chance pequena baseada no monstro)
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

        // Ouro da missão (usando DropService para cálculo correto)
        var questGold = await _dropService.CalculateQuestGoldRewardAsync(questId, hero.Level);
        rewards.Add(new QuestReward
        {
            Type = RewardType.Gold,
            Name = "Ouro da Missão",
            Description = $"Recompensa em ouro por completar: {quest.Title}",
            GoldAmount = questGold,
            Quantity = questGold,
            Icon = "🏆"
        });

        // Experiência da missão (valor real da missão)
        rewards.Add(new QuestReward
        {
            Type = RewardType.Experience,
            Name = "Experiência da Missão",
            Description = $"Experiência ganha completando: {quest.Title}",
            ExperienceAmount = quest.ExperienceReward,
            Quantity = quest.ExperienceReward,
            Icon = "🎯"
        });

        // Itens de recompensa da missão (usando DropService)
        var questItemRewards = await _dropService.RollQuestRewardsAsync(questId, hero.Level);
        foreach (var item in questItemRewards)
        {
            rewards.Add(new QuestReward
            {
                Type = RewardType.Item,
                Name = item.Name,
                Description = $"Recompensa de missão: {quest.Title}",
                ItemId = item.Id,
                Quantity = 1,
                Rarity = item.Rarity,
                Icon = GetItemIcon(item.Type)
            });
        }

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

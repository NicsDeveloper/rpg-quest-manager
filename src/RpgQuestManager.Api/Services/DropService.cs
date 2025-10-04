using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class DropService
{
    private readonly ApplicationDbContext _db;
    private readonly InventoryService _inventoryService;
    private readonly Random _rng = new();

    public DropService(ApplicationDbContext db, InventoryService inventoryService)
    {
        _db = db;
        _inventoryService = inventoryService;
    }

    public async Task<List<Item>> GetMonsterDropsAsync(int monsterId)
    {
        var monster = await _db.Monsters.FindAsync(monsterId);
        if (monster == null) return new List<Item>();

        var allItems = await _db.Items.ToListAsync();
        
        return allItems
            .Where(i => i.DroppedBy.Contains(monster.Type) || 
                       i.FoundIn.Contains(monster.Habitat))
            .ToList();
    }

    public async Task<List<Item>> GetEnvironmentDropsAsync(EnvironmentType environment)
    {
        var allItems = await _db.Items.ToListAsync();
        
        return allItems
            .Where(i => i.FoundIn.Contains(environment))
            .ToList();
    }

    public async Task<List<Item>> GetQuestRewardsAsync(int questId)
    {
        var quest = await _db.Quests.FindAsync(questId);
        if (quest == null) return new List<Item>();

        var allItems = await _db.Items.ToListAsync();
        
        return allItems
            .Where(i => i.Type == ItemType.Quest || 
                       (i.FoundIn.Contains(quest.Environment) && i.Rarity >= ItemRarity.Rare))
            .ToList();
    }

    public async Task<List<Item>> RollMonsterDropsAsync(int monsterId, int characterLevel)
    {
        var monster = await _db.Monsters.FindAsync(monsterId);
        if (monster == null) return new List<Item>();

        var possibleDrops = await GetMonsterDropsAsync(monsterId);
        var droppedItems = new List<Item>();

        foreach (var item in possibleDrops)
        {
            // Calcular chance de drop baseada no nível do personagem e monstro
            var dropChance = CalculateDropChance(item, monster, characterLevel);
            
            if (_rng.Next(1, 101) <= dropChance)
            {
                droppedItems.Add(item);
            }
        }

        // Garantir pelo menos um drop para bosses
        if (monster.Rank == MonsterRank.Boss && !droppedItems.Any())
        {
            var guaranteedDrop = possibleDrops
                .Where(i => i.Rarity <= ItemRarity.Rare)
                .OrderBy(i => i.Rarity)
                .FirstOrDefault();
            
            if (guaranteedDrop != null)
                droppedItems.Add(guaranteedDrop);
        }

        return droppedItems;
    }

    public async Task<List<Item>> RollQuestRewardsAsync(int questId, int characterLevel)
    {
        var quest = await _db.Quests.FindAsync(questId);
        if (quest == null) return new List<Item>();

        var possibleRewards = await GetQuestRewardsAsync(questId);
        var rewardItems = new List<Item>();

        // Recompensas garantidas baseadas na dificuldade da missão
        var guaranteedRewards = GetGuaranteedQuestRewards(quest, characterLevel);
        rewardItems.AddRange(guaranteedRewards);

        // Recompensas aleatórias
        foreach (var item in possibleRewards)
        {
            var rewardChance = CalculateQuestRewardChance(item, quest, characterLevel);
            
            if (_rng.Next(1, 101) <= rewardChance)
            {
                rewardItems.Add(item);
            }
        }

        return rewardItems;
    }

    public async Task<bool> GiveDropsToCharacterAsync(int characterId, List<Item> items)
    {
        return await GiveDropsToHeroAsync(characterId, items);
    }

    public async Task<bool> GiveDropsToHeroAsync(int heroId, List<Item> items)
    {
        foreach (var item in items)
        {
            var inventoryItem = await _inventoryService.AddItemAsync(heroId, item.Id, 1);
            if (inventoryItem == null)
                return false;
        }
        return true;
    }

    public async Task<int> CalculateGoldRewardAsync(int monsterId, int characterLevel)
    {
        var monster = await _db.Monsters.FindAsync(monsterId);
        if (monster == null) return 0;

        var baseGold = monster.ExperienceReward / 2; // Baseado na XP do monstro
        var levelBonus = characterLevel * 2;
        var rarityMultiplier = monster.Rank == MonsterRank.Boss ? 2.0 : 1.0;
        
        var totalGold = (int)((baseGold + levelBonus) * rarityMultiplier);
        
        // Adicionar variação aleatória (±20%)
        var variation = _rng.Next(80, 121);
        return (int)(totalGold * variation / 100.0);
    }

    public async Task<int> CalculateQuestGoldRewardAsync(int questId, int characterLevel)
    {
        var quest = await _db.Quests.FindAsync(questId);
        if (quest == null) return 0;

        var baseGold = quest.GoldReward;
        var levelBonus = characterLevel * 5;
        var difficultyMultiplier = quest.Difficulty switch
        {
            QuestDifficulty.Easy => 1.0,
            QuestDifficulty.Medium => 1.5,
            QuestDifficulty.Hard => 2.0,
            QuestDifficulty.Epic => 3.0,
            QuestDifficulty.Legendary => 5.0,
            _ => 1.0
        };

        return (int)((baseGold + levelBonus) * difficultyMultiplier);
    }

    private int CalculateDropChance(Item item, Monster monster, int characterLevel)
    {
        var baseChance = item.DropChance;
        
        // Modificador baseado na raridade
        var rarityModifier = item.Rarity switch
        {
            ItemRarity.Common => 1.0,
            ItemRarity.Uncommon => 0.7,
            ItemRarity.Rare => 0.4,
            ItemRarity.Epic => 0.2,
            ItemRarity.Legendary => 0.1,
            ItemRarity.Mythic => 0.05,
            _ => 1.0
        };

        // Modificador baseado no nível
        var levelModifier = characterLevel >= monster.Level ? 1.2 : 0.8;

        // Modificador baseado no rank do monstro
        var rankModifier = monster.Rank == MonsterRank.Boss ? 1.5 : 1.0;

        return (int)(baseChance * rarityModifier * levelModifier * rankModifier);
    }

    private int CalculateQuestRewardChance(Item item, Quest quest, int characterLevel)
    {
        var baseChance = 30; // 30% base para recompensas de missão
        
        // Modificador baseado na raridade
        var rarityModifier = item.Rarity switch
        {
            ItemRarity.Common => 1.0,
            ItemRarity.Uncommon => 0.6,
            ItemRarity.Rare => 0.3,
            ItemRarity.Epic => 0.15,
            ItemRarity.Legendary => 0.08,
            ItemRarity.Mythic => 0.03,
            _ => 1.0
        };

        // Modificador baseado na dificuldade da missão
        var difficultyModifier = quest.Difficulty switch
        {
            QuestDifficulty.Easy => 0.8,
            QuestDifficulty.Medium => 1.0,
            QuestDifficulty.Hard => 1.2,
            QuestDifficulty.Epic => 1.5,
            QuestDifficulty.Legendary => 2.0,
            _ => 1.0
        };

        return (int)(baseChance * rarityModifier * difficultyModifier);
    }

    private List<Item> GetGuaranteedQuestRewards(Quest quest, int characterLevel)
    {
        var rewards = new List<Item>();
        
        // Recompensas garantidas baseadas na dificuldade
        switch (quest.Difficulty)
        {
            case QuestDifficulty.Easy:
                // Nenhuma recompensa garantida
                break;
            case QuestDifficulty.Medium:
                // 1 item comum garantido
                break;
            case QuestDifficulty.Hard:
                // 1 item incomum garantido
                break;
            case QuestDifficulty.Epic:
                // 1 item raro garantido
                break;
            case QuestDifficulty.Legendary:
                // 1 item épico garantido
                break;
        }

        return rewards;
    }
}

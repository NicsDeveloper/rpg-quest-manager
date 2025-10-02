using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class DropService : IDropService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DropService> _logger;
    private readonly Random _random = new Random();

    public DropService(
        ApplicationDbContext context,
        INotificationService notificationService,
        ILogger<DropService> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<List<Item>> ProcessDropsAsync(int heroId, Enemy enemy)
    {
        var drops = new List<Item>();

        if (!enemy.IsBoss)
        {
            _logger.LogInformation("Inimigo {EnemyName} não é boss, sem drops especiais", enemy.Name);
            return drops;
        }

        // Busca a tabela de drops do boss
        var dropTable = await _context.BossDropTables
            .Include(bd => bd.Item)
            .Where(bd => bd.EnemyId == enemy.Id)
            .ToListAsync();

        if (!dropTable.Any())
        {
            _logger.LogWarning("Boss {BossName} não tem drops configurados", enemy.Name);
            return drops;
        }

        var hero = await _context.Heroes.FindAsync(heroId);
        if (hero?.UserId == null)
        {
            _logger.LogWarning("Herói {HeroId} não encontrado ou sem usuário vinculado", heroId);
            return drops;
        }

        // Processa cada entrada da tabela de drops
        foreach (var dropEntry in dropTable)
        {
            var roll = (decimal)_random.NextDouble(); // 0.0 a 1.0

            if (roll <= dropEntry.DropChance)
            {
                drops.Add(dropEntry.Item);
                
                // Adiciona ao inventário do herói
                await AddItemToInventoryAsync(heroId, dropEntry.Item.Id, 1);

                // Notifica sobre drops raros
                if (dropEntry.Item.Rarity >= ItemRarity.Epic)
                {
                    var notificationType = dropEntry.Item.Rarity switch
                    {
                        ItemRarity.Epic => "Warning", // Amarelo
                        ItemRarity.Legendary => "Success", // Verde brilhante
                        _ => "Info"
                    };

                    var emoji = dropEntry.Item.Rarity switch
                    {
                        ItemRarity.Epic => "🔥",
                        ItemRarity.Legendary => "👑",
                        _ => "✨"
                    };

                    await _notificationService.CreateNotificationAsync(
                        hero.UserId.Value,
                        $"{emoji} Drop {dropEntry.Item.GetFullRarityName()}!",
                        $"Você obteve: {dropEntry.Item.Name}\n\n{dropEntry.Item.Description}",
                        notificationType
                    );
                }

                _logger.LogInformation(
                    "DROP! Boss {Boss} dropou {Item} ({Rarity}) para herói {HeroId}. Chance: {Chance:P}",
                    enemy.Name, dropEntry.Item.Name, dropEntry.Item.GetFullRarityName(), 
                    heroId, dropEntry.DropChance);
            }
            else
            {
                _logger.LogDebug(
                    "Boss {Boss}: {Item} NÃO dropou. Roll: {Roll:P} / Chance: {Chance:P}",
                    enemy.Name, dropEntry.Item.Name, roll, dropEntry.DropChance);
            }
        }

        // Log resumo
        if (drops.Any())
        {
            _logger.LogInformation(
                "Boss {Boss} dropou {Count} item(ns) para herói {HeroId}: {Items}",
                enemy.Name, drops.Count, heroId, string.Join(", ", drops.Select(d => d.Name)));
        }
        else
        {
            _logger.LogInformation("Boss {Boss} não dropou nenhum item para herói {HeroId}", enemy.Name, heroId);
        }

        return drops;
    }

    public async Task<Item?> RollDropAsync(Enemy enemy)
    {
        if (!enemy.IsBoss) return null;

        var dropTable = await _context.BossDropTables
            .Include(bd => bd.Item)
            .Where(bd => bd.EnemyId == enemy.Id)
            .ToListAsync();

        if (!dropTable.Any()) return null;

        // Pega um drop aleatório baseado nas chances
        var roll = (decimal)_random.NextDouble();
        var cumulativeChance = 0m;

        foreach (var dropEntry in dropTable.OrderByDescending(d => d.DropChance))
        {
            cumulativeChance += dropEntry.DropChance;
            if (roll <= cumulativeChance)
            {
                return dropEntry.Item;
            }
        }

        return null;
    }

    public async Task AddItemToInventoryAsync(int heroId, int itemId, int quantity = 1)
    {
        var existingHeroItem = await _context.HeroItems
            .FirstOrDefaultAsync(hi => hi.HeroId == heroId && hi.ItemId == itemId);

        if (existingHeroItem != null)
        {
            existingHeroItem.Quantity += quantity;
            _logger.LogInformation("Item {ItemId} x{Quantity} adicionado ao inventário do herói {HeroId}", 
                itemId, quantity, heroId);
        }
        else
        {
            var heroItem = new HeroItem
            {
                HeroId = heroId,
                ItemId = itemId,
                Quantity = quantity,
                IsEquipped = false,
                AcquiredAt = DateTime.UtcNow
            };

            _context.HeroItems.Add(heroItem);
            _logger.LogInformation("Novo item {ItemId} x{Quantity} adicionado ao inventário do herói {HeroId}", 
                itemId, quantity, heroId);
        }

        await _context.SaveChangesAsync();
    }
}


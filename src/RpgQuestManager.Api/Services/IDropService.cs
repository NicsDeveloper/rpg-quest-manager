using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface IDropService
{
    Task<List<Item>> ProcessDropsAsync(int heroId, Enemy enemy);
    Task<Item?> RollDropAsync(Enemy enemy);
    Task<List<Item>> CalculateDropsAsync(int enemyId);
    Task AddItemToInventoryAsync(int heroId, int itemId, int quantity = 1);
}


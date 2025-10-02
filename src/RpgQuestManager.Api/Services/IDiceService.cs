using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface IDiceService
{
    Task<DiceInventory> GetOrCreateInventoryAsync(int heroId);
    Task<bool> PurchaseDiceAsync(int heroId, DiceType diceType, int quantity);
    Task<bool> UseDiceAsync(int heroId, DiceType diceType);
    Task<int> RollDiceAsync(DiceType diceType);
    Task<int> GetDicePriceAsync(DiceType diceType);
}


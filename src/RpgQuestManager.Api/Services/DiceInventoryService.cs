using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class DiceInventoryService
{
    private readonly ApplicationDbContext _db;

    public DiceInventoryService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Dictionary<DiceType, int>> GetHeroDiceInventoryAsync(int heroId)
    {
        var inventory = await _db.DiceInventories
            .Where(di => di.HeroId == heroId)
            .ToListAsync();

        var result = new Dictionary<DiceType, int>();
        
        // Inicializar todos os tipos de dados
        foreach (DiceType diceType in Enum.GetValues<DiceType>())
        {
            var diceInventory = inventory.FirstOrDefault(di => di.DiceType == diceType);
            
            // D6 é infinito (ataque básico)
            if (diceType == DiceType.D6)
            {
                result[diceType] = int.MaxValue; // Representa infinito
            }
            else
            {
                result[diceType] = diceInventory?.Quantity ?? 0;
            }
        }

        return result;
    }

    public async Task<bool> HasDiceAsync(int heroId, DiceType diceType)
    {
        // D6 é sempre disponível
        if (diceType == DiceType.D6)
        {
            return true;
        }

        var inventory = await _db.DiceInventories
            .FirstOrDefaultAsync(di => di.HeroId == heroId && di.DiceType == diceType);

        return inventory != null && inventory.Quantity > 0;
    }

    public async Task<bool> ConsumeDiceAsync(int heroId, DiceType diceType)
    {
        // D6 é infinito, não consome
        if (diceType == DiceType.D6)
        {
            return true;
        }

        var inventory = await _db.DiceInventories
            .FirstOrDefaultAsync(di => di.HeroId == heroId && di.DiceType == diceType);

        if (inventory == null || inventory.Quantity <= 0)
        {
            return false;
        }

        inventory.Quantity--;
        inventory.UpdatedAt = DateTime.UtcNow;

        if (inventory.Quantity == 0)
        {
            _db.DiceInventories.Remove(inventory);
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task AddDiceAsync(int heroId, DiceType diceType, int quantity)
    {
        // D6 é infinito, não precisa adicionar
        if (diceType == DiceType.D6)
        {
            return;
        }

        var inventory = await _db.DiceInventories
            .FirstOrDefaultAsync(di => di.HeroId == heroId && di.DiceType == diceType);

        if (inventory == null)
        {
            inventory = new DiceInventory
            {
                HeroId = heroId,
                DiceType = diceType,
                Quantity = quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.DiceInventories.Add(inventory);
        }
        else
        {
            inventory.Quantity += quantity;
            inventory.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    public async Task InitializeHeroDiceInventoryAsync(int heroId)
    {
        // Verificar se já existe inventário para este herói
        var existingInventory = await _db.DiceInventories
            .AnyAsync(di => di.HeroId == heroId);

        if (existingInventory)
        {
            return; // Já existe inventário
        }

        // D6 não precisa ser adicionado pois é infinito
        // Adicionar alguns dados iniciais para outros tipos
        var initialDice = new[]
        {
            new { Type = DiceType.D4, Quantity = 2 },
            new { Type = DiceType.D8, Quantity = 1 },
            new { Type = DiceType.D10, Quantity = 1 },
            new { Type = DiceType.D12, Quantity = 1 },
            new { Type = DiceType.D20, Quantity = 1 }
        };

        foreach (var dice in initialDice)
        {
            var inventory = new DiceInventory
            {
                HeroId = heroId,
                DiceType = dice.Type,
                Quantity = dice.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.DiceInventories.Add(inventory);
        }

        await _db.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class DiceService : IDiceService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DiceService> _logger;
    private readonly Random _random = new Random();

    public DiceService(ApplicationDbContext context, ILogger<DiceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DiceInventory> GetOrCreateInventoryAsync(int heroId)
    {
        var inventory = await _context.DiceInventories
            .FirstOrDefaultAsync(di => di.HeroId == heroId);

        if (inventory == null)
        {
            inventory = new DiceInventory
            {
                HeroId = heroId,
                D6Count = 3, // Começa com 3 dados D6
                D8Count = 0,
                D12Count = 0,
                D20Count = 0
            };

            _context.DiceInventories.Add(inventory);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Inventário de dados criado para herói {HeroId}", heroId);
        }

        return inventory;
    }

    public async Task<bool> PurchaseDiceAsync(int heroId, DiceType diceType, int quantity)
    {
        var hero = await _context.Heroes.FindAsync(heroId);
        if (hero == null) return false;

        var price = GetDicePriceAsync(diceType).Result * quantity;
        
        if (hero.Gold < price)
        {
            _logger.LogWarning("Herói {HeroId} não tem ouro suficiente para comprar {Quantity}x {DiceType}", 
                heroId, quantity, diceType);
            return false;
        }

        var inventory = await GetOrCreateInventoryAsync(heroId);
        
        // Deduz o ouro
        hero.Gold -= price;
        
        // Adiciona os dados
        inventory.AddDice(diceType, quantity);
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Herói {HeroId} comprou {Quantity}x {DiceType} por {Price} ouro", 
            heroId, quantity, diceType, price);
        
        return true;
    }

    public async Task<bool> UseDiceAsync(int heroId, DiceType diceType)
    {
        var inventory = await GetOrCreateInventoryAsync(heroId);
        
        if (!inventory.HasDice(diceType))
        {
            _logger.LogWarning("Herói {HeroId} não tem dados do tipo {DiceType} disponíveis", 
                heroId, diceType);
            return false;
        }

        var success = inventory.UseDice(diceType);
        
        if (success)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Herói {HeroId} usou um dado {DiceType}", heroId, diceType);
        }

        return success;
    }

    public Task<int> RollDiceAsync(DiceType diceType)
    {
        var result = _random.Next(1, (int)diceType + 1);
        _logger.LogInformation("Dado {DiceType} rolado: {Result}", diceType, result);
        return Task.FromResult(result);
    }

    public Task<int> GetDicePriceAsync(DiceType diceType)
    {
        // Preços dos dados (em ouro)
        var price = diceType switch
        {
            DiceType.D6 => 50,
            DiceType.D8 => 100,
            DiceType.D12 => 200,
            DiceType.D20 => 500,
            _ => 0
        };

        return Task.FromResult(price);
    }
}


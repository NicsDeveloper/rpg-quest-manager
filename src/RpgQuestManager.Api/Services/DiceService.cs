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

    public async Task<DiceInventory> GetOrCreateInventoryAsync(int userId)
    {
        var inventory = await _context.DiceInventories
            .FirstOrDefaultAsync(di => di.UserId == userId);

        if (inventory == null)
        {
            inventory = new DiceInventory
            {
                UserId = userId,
                D6Count = 3, // Começa com 3 dados D6
                D10Count = 1, // Começa com 1 dado D10
                D12Count = 0,
                D20Count = 0
            };

            _context.DiceInventories.Add(inventory);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Inventário de dados criado para player {UserId}", userId);
        }

        return inventory;
    }

    public async Task<bool> PurchaseDiceAsync(int userId, DiceType diceType, int quantity)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        var price = GetDicePriceAsync(diceType).Result * quantity;
        
        // Ouro está no player
        if (user.Gold < price)
        {
            _logger.LogWarning("Player {UserId} não tem ouro suficiente para comprar {Quantity}x {DiceType}", 
                userId, quantity, diceType);
            return false;
        }

        var inventory = await GetOrCreateInventoryAsync(userId);
        
        // Deduz o ouro do player
        user.Gold -= price;
        
        // Adiciona os dados
        inventory.AddDice(diceType, quantity);
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Player {UserId} comprou {Quantity}x {DiceType} por {Price} ouro", 
            userId, quantity, diceType, price);
        
        return true;
    }

    public async Task<bool> UseDiceAsync(int userId, DiceType diceType)
    {
        var inventory = await GetOrCreateInventoryAsync(userId);
        
        if (!inventory.HasDice(diceType))
        {
            _logger.LogWarning("Player {UserId} não tem dados do tipo {DiceType} disponíveis", 
                userId, diceType);
            return false;
        }

        var success = inventory.UseDice(diceType);
        
        if (success)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Player {UserId} usou um dado {DiceType}", userId, diceType);
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
            DiceType.D10 => 100,
            DiceType.D12 => 200,
            DiceType.D20 => 500,
            _ => 0
        };

        return Task.FromResult(price);
    }
}


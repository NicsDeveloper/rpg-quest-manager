using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Shop;
using RpgQuestManager.Api.Models;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

/// <summary>
/// Sistema de loja para compra de itens
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "🛒 Loja")]
public class ShopController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ShopController> _logger;
    
    public ShopController(
        ApplicationDbContext context, 
        IMapper mapper,
        ILogger<ShopController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }
    
    /// <summary>
    /// Obtém todos os itens disponíveis na loja
    /// </summary>
    [HttpGet("items")]
    public async Task<ActionResult<IEnumerable<ShopItemDto>>> GetShopItems()
    {
        var items = await _context.Items
            .Where(i => i.Value > 0) // Apenas itens com valor (compraveis)
            .OrderBy(i => i.Rarity)
            .ThenBy(i => i.Value)
            .ToListAsync();
            
        return Ok(_mapper.Map<IEnumerable<ShopItemDto>>(items));
    }
    
    /// <summary>
    /// Compra um item da loja
    /// </summary>
    [HttpPost("buy/{itemId}")]
    public async Task<ActionResult<ShopPurchaseResultDto>> BuyItem(int itemId, [FromQuery] int quantity = 1)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.UserId == userId);
        
        if (hero == null)
        {
            return BadRequest(new { message = "Você não possui um herói." });
        }
        
        var item = await _context.Items.FindAsync(itemId);
        if (item == null)
        {
            return NotFound(new { message = "Item não encontrado." });
        }
        
        if (item.Value <= 0)
        {
            return BadRequest(new { message = "Este item não está disponível para compra." });
        }
        
        var totalCost = item.Value * quantity;
        
        if (hero.Gold < totalCost)
        {
            return BadRequest(new { message = $"Ouro insuficiente. Necessário: {totalCost}, Disponível: {hero.Gold}" });
        }
        
        // Deduzir ouro
        hero.Gold -= totalCost;
        
        // Adicionar item ao inventário
        var existingHeroItem = await _context.HeroItems
            .FirstOrDefaultAsync(hi => hi.HeroId == hero.Id && hi.ItemId == itemId);
        
        if (existingHeroItem != null)
        {
            existingHeroItem.Quantity += quantity;
        }
        else
        {
            var heroItem = new HeroItem
            {
                HeroId = hero.Id,
                ItemId = itemId,
                Quantity = quantity,
                IsEquipped = false,
                AcquiredAt = DateTime.UtcNow
            };
            _context.HeroItems.Add(heroItem);
        }
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🛒 Compra realizada! Herói {HeroName} comprou {ItemName} x{Quantity} por {Cost} gold", 
            hero.Name, item.Name, quantity, totalCost);
        
        return Ok(new ShopPurchaseResultDto
        {
            Success = true,
            Message = $"Compra realizada com sucesso! {item.Name} x{quantity} adicionado ao inventário.",
            ItemName = item.Name,
            Quantity = quantity,
            TotalCost = totalCost,
            RemainingGold = hero.Gold
        });
    }
    
    /// <summary>
    /// Obtém o inventário do herói
    /// </summary>
    [HttpGet("inventory")]
    public async Task<ActionResult<IEnumerable<ShopItemDto>>> GetHeroInventory()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.UserId == userId);
        
        if (hero == null)
        {
            return BadRequest(new { message = "Você não possui um herói." });
        }
        
        var inventory = await _context.HeroItems
            .Include(hi => hi.Item)
            .Where(hi => hi.HeroId == hero.Id)
            .Select(hi => new ShopItemDto
            {
                Id = hi.Item.Id,
                Name = hi.Item.Name,
                Description = hi.Item.Description,
                Type = hi.Item.Type,
                Rarity = hi.Item.Rarity.ToString(),
                Value = hi.Item.Value,
                BonusStrength = hi.Item.BonusStrength,
                BonusIntelligence = hi.Item.BonusIntelligence,
                BonusDexterity = hi.Item.BonusDexterity,
                Quantity = hi.Quantity,
                IsEquipped = hi.IsEquipped,
                IsConsumable = hi.Item.IsConsumable,
                PercentageXpBonus = hi.Item.PercentageXpBonus
            })
            .ToListAsync();
            
        return Ok(inventory);
    }
}

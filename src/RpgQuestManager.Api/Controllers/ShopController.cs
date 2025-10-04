using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopController : ControllerBase
{
    private readonly ShopService _shopService;

    public ShopController(ShopService shopService)
    {
        _shopService = shopService;
    }

    public record BuyItemRequest(int CharacterId, int ItemId, int Quantity = 1);
    public record SellItemRequest(int CharacterId, int InventoryItemId, int Quantity = 1);

    [HttpGet]
    public async Task<IActionResult> GetShopItems([FromQuery] string shopType = "general")
    {
        try
        {
            var items = await _shopService.GetShopItemsAsync(shopType);
            return Ok(new
            {
                shopType,
                description = _shopService.GetShopTypeDescription(shopType),
                items = items.Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    description = item.Description,
                    type = item.Type.ToString(),
                    rarity = item.Rarity.ToString(),
                    rarityColor = _shopService.GetRarityColor(item.Rarity),
                    rarityDescription = _shopService.GetRarityDescription(item.Rarity),
                    level = item.Level,
                    value = item.Value,
                    shopPrice = item.ShopPrice,
                    requiredLevel = item.RequiredLevel,
                    attackBonus = item.AttackBonus,
                    defenseBonus = item.DefenseBonus,
                    healthBonus = item.HealthBonus,
                    moraleBonus = item.MoraleBonus,
                    isConsumable = item.IsConsumable,
                    stackSize = item.StackSize,
                    statusEffects = item.StatusEffects.Select(se => se.ToString()).ToList(),
                    statusEffectChance = item.StatusEffectChance,
                    statusEffectDuration = item.StatusEffectDuration
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("by-type/{itemType}")]
    public async Task<IActionResult> GetShopItemsByType(ItemType itemType, [FromQuery] string shopType = "general")
    {
        try
        {
            var items = await _shopService.GetShopItemsByTypeAsync(itemType, shopType);
            return Ok(new
            {
                itemType = itemType.ToString(),
                shopType,
                description = _shopService.GetShopTypeDescription(shopType),
                items = items.Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    description = item.Description,
                    type = item.Type.ToString(),
                    rarity = item.Rarity.ToString(),
                    rarityColor = _shopService.GetRarityColor(item.Rarity),
                    level = item.Level,
                    shopPrice = item.ShopPrice,
                    requiredLevel = item.RequiredLevel,
                    attackBonus = item.AttackBonus,
                    defenseBonus = item.DefenseBonus,
                    healthBonus = item.HealthBonus,
                    moraleBonus = item.MoraleBonus
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("by-level/{characterLevel}")]
    public async Task<IActionResult> GetShopItemsByLevel(int characterLevel, [FromQuery] string shopType = "general")
    {
        try
        {
            var items = await _shopService.GetShopItemsByLevelAsync(characterLevel, shopType);
            return Ok(new
            {
                characterLevel,
                shopType,
                description = _shopService.GetShopTypeDescription(shopType),
                items = items.Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    description = item.Description,
                    type = item.Type.ToString(),
                    rarity = item.Rarity.ToString(),
                    rarityColor = _shopService.GetRarityColor(item.Rarity),
                    level = item.Level,
                    shopPrice = item.ShopPrice,
                    requiredLevel = item.RequiredLevel,
                    attackBonus = item.AttackBonus,
                    defenseBonus = item.DefenseBonus,
                    healthBonus = item.HealthBonus,
                    moraleBonus = item.MoraleBonus
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("buy")]
    public async Task<IActionResult> BuyItem([FromBody] BuyItemRequest request)
    {
        try
        {
            var success = await _shopService.BuyItemAsync(request.CharacterId, request.ItemId, request.Quantity);
            if (!success)
            {
                return BadRequest(new { message = "Não foi possível comprar o item" });
            }

            return Ok(new { message = "Item comprado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("sell")]
    public async Task<IActionResult> SellItem([FromBody] SellItemRequest request)
    {
        try
        {
            var success = await _shopService.SellItemAsync(request.CharacterId, request.InventoryItemId, request.Quantity);
            if (!success)
            {
                return BadRequest(new { message = "Não foi possível vender o item" });
            }

            return Ok(new { message = "Item vendido com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("sell-price/{itemId}")]
    public async Task<IActionResult> GetItemSellPrice(int itemId)
    {
        try
        {
            var sellPrice = await _shopService.GetItemSellPriceAsync(itemId);
            return Ok(new
            {
                itemId,
                sellPrice
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetShopTypes()
    {
        try
        {
            var shopTypes = await _shopService.GetShopTypesAsync();
            return Ok(new
            {
                shopTypes = shopTypes.Select(st => new
                {
                    type = st,
                    description = _shopService.GetShopTypeDescription(st)
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("rarities")]
    public IActionResult GetRarities()
    {
        try
        {
            var rarities = Enum.GetValues<ItemRarity>().Select(r => new
            {
                rarity = r.ToString(),
                description = _shopService.GetRarityDescription(r),
                color = _shopService.GetRarityColor(r)
            });

            return Ok(new { rarities });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }
}

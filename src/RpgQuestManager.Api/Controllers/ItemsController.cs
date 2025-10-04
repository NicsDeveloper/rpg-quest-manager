using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ItemsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItems()
    {
        try
        {
            var items = await _db.Items
                .OrderBy(i => i.Type)
                .ThenBy(i => i.Level)
                .ThenBy(i => i.Rarity)
                .ToListAsync();

            return Ok(new
            {
                items = items.Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    description = item.Description,
                    type = item.Type.ToString(),
                    rarity = item.Rarity.ToString(),
                    level = item.Level,
                    value = item.Value,
                    stackSize = item.StackSize,
                    isConsumable = item.IsConsumable,
                    isTradeable = item.IsTradeable,
                    isSellable = item.IsSellable,
                    attackBonus = item.AttackBonus,
                    defenseBonus = item.DefenseBonus,
                    healthBonus = item.HealthBonus,
                    moraleBonus = item.MoraleBonus,
                    statusEffects = item.StatusEffects.Select(se => se.ToString()).ToList(),
                    statusEffectChance = item.StatusEffectChance,
                    statusEffectDuration = item.StatusEffectDuration,
                    requiredLevel = item.RequiredLevel,
                    droppedBy = item.DroppedBy.Select(dt => dt.ToString()).ToList(),
                    dropChance = item.DropChance,
                    foundIn = item.FoundIn.Select(ei => ei.ToString()).ToList(),
                    availableInShop = item.AvailableInShop,
                    shopPrice = item.ShopPrice,
                    shopTypes = item.ShopTypes
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(int id)
    {
        try
        {
            var item = await _db.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound(new { message = "Item não encontrado" });
            }

            return Ok(new
            {
                id = item.Id,
                name = item.Name,
                description = item.Description,
                type = item.Type.ToString(),
                rarity = item.Rarity.ToString(),
                level = item.Level,
                value = item.Value,
                stackSize = item.StackSize,
                isConsumable = item.IsConsumable,
                isTradeable = item.IsTradeable,
                isSellable = item.IsSellable,
                attackBonus = item.AttackBonus,
                defenseBonus = item.DefenseBonus,
                healthBonus = item.HealthBonus,
                moraleBonus = item.MoraleBonus,
                statusEffects = item.StatusEffects.Select(se => se.ToString()).ToList(),
                statusEffectChance = item.StatusEffectChance,
                statusEffectDuration = item.StatusEffectDuration,
                requiredLevel = item.RequiredLevel,
                droppedBy = item.DroppedBy.Select(dt => dt.ToString()).ToList(),
                dropChance = item.DropChance,
                foundIn = item.FoundIn.Select(ei => ei.ToString()).ToList(),
                availableInShop = item.AvailableInShop,
                shopPrice = item.ShopPrice,
                shopTypes = item.ShopTypes
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("by-type/{itemType}")]
    public async Task<IActionResult> GetItemsByType(ItemType itemType)
    {
        try
        {
            var items = await _db.Items
                .Where(i => i.Type == itemType)
                .OrderBy(i => i.Level)
                .ThenBy(i => i.Rarity)
                .ToListAsync();

            return Ok(new
            {
                itemType = itemType.ToString(),
                items = items.Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    description = item.Description,
                    rarity = item.Rarity.ToString(),
                    level = item.Level,
                    value = item.Value,
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

    [HttpGet("by-rarity/{rarity}")]
    public async Task<IActionResult> GetItemsByRarity(ItemRarity rarity)
    {
        try
        {
            var items = await _db.Items
                .Where(i => i.Rarity == rarity)
                .OrderBy(i => i.Type)
                .ThenBy(i => i.Level)
                .ToListAsync();

            return Ok(new
            {
                rarity = rarity.ToString(),
                items = items.Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    description = item.Description,
                    type = item.Type.ToString(),
                    level = item.Level,
                    value = item.Value,
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

    [HttpGet("by-level/{level}")]
    public async Task<IActionResult> GetItemsByLevel(int level)
    {
        try
        {
            var items = await _db.Items
                .Where(i => i.Level == level)
                .OrderBy(i => i.Type)
                .ThenBy(i => i.Rarity)
                .ToListAsync();

            return Ok(new
            {
                level,
                items = items.Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    description = item.Description,
                    type = item.Type.ToString(),
                    rarity = item.Rarity.ToString(),
                    value = item.Value,
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

    [HttpGet("search")]
    public async Task<IActionResult> SearchItems([FromQuery] string? name = null, [FromQuery] ItemType? type = null, [FromQuery] ItemRarity? rarity = null, [FromQuery] int? minLevel = null, [FromQuery] int? maxLevel = null)
    {
        try
        {
            var query = _db.Items.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(i => i.Name.Contains(name));
            }

            if (type.HasValue)
            {
                query = query.Where(i => i.Type == type.Value);
            }

            if (rarity.HasValue)
            {
                query = query.Where(i => i.Rarity == rarity.Value);
            }

            if (minLevel.HasValue)
            {
                query = query.Where(i => i.Level >= minLevel.Value);
            }

            if (maxLevel.HasValue)
            {
                query = query.Where(i => i.Level <= maxLevel.Value);
            }

            var items = await query
                .OrderBy(i => i.Type)
                .ThenBy(i => i.Level)
                .ThenBy(i => i.Rarity)
                .ToListAsync();

            return Ok(new
            {
                searchCriteria = new
                {
                    name,
                    type = type?.ToString(),
                    rarity = rarity?.ToString(),
                    minLevel,
                    maxLevel
                },
                items = items.Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    description = item.Description,
                    type = item.Type.ToString(),
                    rarity = item.Rarity.ToString(),
                    level = item.Level,
                    value = item.Value,
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
}

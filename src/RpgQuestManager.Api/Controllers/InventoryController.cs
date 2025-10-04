using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public InventoryController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public record AddItemRequest(int CharacterId, int ItemId, int Quantity = 1);
    public record RemoveItemRequest(int CharacterId, int ItemId, int Quantity = 1);
    public record EquipItemRequest(int CharacterId, int InventoryItemId, EquipmentSlot Slot);
    public record UnequipItemRequest(int CharacterId, EquipmentSlot Slot);
    public record UseItemRequest(int CharacterId, int InventoryItemId);

    [HttpGet("{characterId}")]
    public async Task<IActionResult> GetInventory(int characterId)
    {
        try
        {
            var inventory = await _inventoryService.GetCharacterInventoryAsync(characterId);
            return Ok(new
            {
                characterId,
                items = inventory.Select(ii => new
                {
                    id = ii.Id,
                    itemId = ii.ItemId,
                    name = ii.Item.Name,
                    description = ii.Item.Description,
                    type = ii.Item.Type.ToString(),
                    rarity = ii.Item.Rarity.ToString(),
                    level = ii.Item.Level,
                    quantity = ii.Quantity,
                    isEquipped = ii.IsEquipped,
                    equippedSlot = ii.EquippedSlot?.ToString(),
                    acquiredAt = ii.AcquiredAt,
                    attackBonus = ii.Item.AttackBonus,
                    defenseBonus = ii.Item.DefenseBonus,
                    healthBonus = ii.Item.HealthBonus,
                    moraleBonus = ii.Item.MoraleBonus,
                    isConsumable = ii.Item.IsConsumable,
                    stackSize = ii.Item.StackSize
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddItem([FromBody] AddItemRequest request)
    {
        try
        {
            var inventoryItem = await _inventoryService.AddItemAsync(request.CharacterId, request.ItemId, request.Quantity);
            if (inventoryItem == null)
            {
                return BadRequest(new { message = "Item não encontrado" });
            }

            return Ok(new
            {
                message = "Item adicionado ao inventário",
                inventoryItem = new
                {
                    id = inventoryItem.Id,
                    itemId = inventoryItem.ItemId,
                    name = inventoryItem.Item.Name,
                    quantity = inventoryItem.Quantity,
                    acquiredAt = inventoryItem.AcquiredAt
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveItem([FromBody] RemoveItemRequest request)
    {
        try
        {
            var success = await _inventoryService.RemoveItemAsync(request.CharacterId, request.ItemId, request.Quantity);
            if (!success)
            {
                return BadRequest(new { message = "Item não encontrado no inventário" });
            }

            return Ok(new { message = "Item removido do inventário" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("equip")]
    public async Task<IActionResult> EquipItem([FromBody] EquipItemRequest request)
    {
        try
        {
            var success = await _inventoryService.EquipItemAsync(request.CharacterId, request.InventoryItemId, request.Slot);
            if (!success)
            {
                return BadRequest(new { message = "Não foi possível equipar o item" });
            }

            return Ok(new { message = "Item equipado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("unequip")]
    public async Task<IActionResult> UnequipItem([FromBody] UnequipItemRequest request)
    {
        try
        {
            var success = await _inventoryService.UnequipItemAsync(request.CharacterId, request.Slot);
            if (!success)
            {
                return BadRequest(new { message = "Nenhum item equipado neste slot" });
            }

            return Ok(new { message = "Item desequipado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("equipment/{characterId}")]
    public async Task<IActionResult> GetEquipment(int characterId)
    {
        try
        {
            var equipment = await _inventoryService.GetCharacterEquipmentAsync(characterId);
            return Ok(new
            {
                characterId,
                equipment = new
                {
                    weaponId = equipment.WeaponId,
                    shieldId = equipment.ShieldId,
                    helmetId = equipment.HelmetId,
                    armorId = equipment.ArmorId,
                    glovesId = equipment.GlovesId,
                    bootsId = equipment.BootsId,
                    ringId = equipment.RingId,
                    amuletId = equipment.AmuletId
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("bonuses/{characterId}")]
    public async Task<IActionResult> GetEquipmentBonuses(int characterId)
    {
        try
        {
            var bonuses = await _inventoryService.GetEquipmentBonusesAsync(characterId);
            return Ok(new
            {
                characterId,
                bonuses = new
                {
                    attack = bonuses.attack,
                    defense = bonuses.defense,
                    health = bonuses.health,
                    morale = bonuses.morale
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("use")]
    public async Task<IActionResult> UseItem([FromBody] UseItemRequest request)
    {
        try
        {
            var success = await _inventoryService.UseItemAsync(request.CharacterId, request.InventoryItemId);
            if (!success)
            {
                return BadRequest(new { message = "Não foi possível usar o item" });
            }

            return Ok(new { message = "Item usado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }
}

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

    public record AddItemRequest(int HeroId, int ItemId, int Quantity = 1);
    public record RemoveItemRequest(int HeroId, int ItemId, int Quantity = 1);
    public record UseItemRequest(int HeroId, int InventoryItemId);
    
    public class EquipItemRequest
    {
        public int HeroId { get; set; }
        public int InventoryItemId { get; set; }
        public string Slot { get; set; } = string.Empty;
        
        public EquipmentSlot GetEquipmentSlot()
        {
            return Enum.TryParse<EquipmentSlot>(Slot, true, out var slot) ? slot : throw new ArgumentException($"Slot inválido: {Slot}");
        }
    }
    
    public class UnequipItemRequest
    {
        public int HeroId { get; set; }
        public string Slot { get; set; } = string.Empty;
        
        public EquipmentSlot GetEquipmentSlot()
        {
            return Enum.TryParse<EquipmentSlot>(Slot, true, out var slot) ? slot : throw new ArgumentException($"Slot inválido: {Slot}");
        }
    }

    [HttpGet("{heroId}")]
    public async Task<IActionResult> GetInventory(int heroId)
    {
        try
        {
            var inventory = await _inventoryService.GetHeroInventoryAsync(heroId);
            return Ok(new
            {
                heroId,
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
            var inventoryItem = await _inventoryService.AddItemAsync(request.HeroId, request.ItemId, request.Quantity);
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
            var success = await _inventoryService.RemoveItemAsync(request.HeroId, request.ItemId, request.Quantity);
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
            // Validação dos parâmetros
            if (request.HeroId <= 0)
            {
                return BadRequest(new { message = "HeroId inválido" });
            }
            
            if (request.InventoryItemId <= 0)
            {
                return BadRequest(new { message = "InventoryItemId inválido" });
            }

            if (string.IsNullOrWhiteSpace(request.Slot))
            {
                return BadRequest(new { message = "Slot inválido" });
            }

            // Converter string para enum
            EquipmentSlot slot;
            try
            {
                slot = request.GetEquipmentSlot();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var success = await _inventoryService.EquipItemAsync(request.HeroId, request.InventoryItemId, slot);
            if (!success)
            {
                return BadRequest(new { message = "Não foi possível equipar o item. Verifique se o item existe e é compatível com o slot." });
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
            // Converter string para enum
            EquipmentSlot slot;
            try
            {
                slot = request.GetEquipmentSlot();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var success = await _inventoryService.UnequipItemAsync(request.HeroId, slot);
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

    [HttpGet("equipment/{heroId}")]
    public async Task<IActionResult> GetEquipment(int heroId)
    {
        try
        {
            var equipment = await _inventoryService.GetHeroEquipmentAsync(heroId);
            return Ok(new
            {
                heroId,
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

    [HttpGet("bonuses/{heroId}")]
    public async Task<IActionResult> GetEquipmentBonuses(int heroId)
    {
        try
        {
            var bonuses = await _inventoryService.GetEquipmentBonusesAsync(heroId);
            return Ok(new
            {
                heroId,
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
            var success = await _inventoryService.UseItemAsync(request.HeroId, request.InventoryItemId);
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

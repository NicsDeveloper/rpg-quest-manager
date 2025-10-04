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
    public record UseItemRequest(int CharacterId, int InventoryItemId);
    
    public class EquipItemRequest
    {
        public int CharacterId { get; set; }
        public int InventoryItemId { get; set; }
        public string Slot { get; set; } = string.Empty;
        
        public EquipmentSlot GetEquipmentSlot()
        {
            return Enum.TryParse<EquipmentSlot>(Slot, true, out var slot) ? slot : throw new ArgumentException($"Slot inválido: {Slot}");
        }
    }
    
    public class UnequipItemRequest
    {
        public int CharacterId { get; set; }
        public string Slot { get; set; } = string.Empty;
        
        public EquipmentSlot GetEquipmentSlot()
        {
            return Enum.TryParse<EquipmentSlot>(Slot, true, out var slot) ? slot : throw new ArgumentException($"Slot inválido: {Slot}");
        }
    }

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
    public async Task<IActionResult> EquipItem([FromBody] dynamic request)
    {
        try
        {
            // Debug: Log dos dados recebidos
            Console.WriteLine($"EquipItem - Request: {request}");

            // Extrair dados do request dinâmico
            var characterId = (int)request.characterId;
            var inventoryItemId = (int)request.inventoryItemId;
            var slotString = (string)request.slot;

            Console.WriteLine($"CharacterId: {characterId}, InventoryItemId: {inventoryItemId}, Slot: {slotString}");

            // Validação manual dos parâmetros
            if (characterId <= 0)
            {
                return BadRequest(new { message = "CharacterId inválido" });
            }
            
            if (inventoryItemId <= 0)
            {
                return BadRequest(new { message = "InventoryItemId inválido" });
            }

            if (string.IsNullOrWhiteSpace(slotString))
            {
                return BadRequest(new { message = "Slot inválido" });
            }

            // Converter string para enum
            if (!Enum.TryParse<EquipmentSlot>(slotString, true, out var slot))
            {
                return BadRequest(new { message = $"Slot inválido: {slotString}" });
            }

            var success = await _inventoryService.EquipItemAsync(characterId, inventoryItemId, slot);
            if (!success)
            {
                return BadRequest(new { message = "Não foi possível equipar o item. Verifique se o item existe e é compatível com o slot." });
            }

            return Ok(new { message = "Item equipado com sucesso" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro em EquipItem: {ex.Message}");
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

            var success = await _inventoryService.UnequipItemAsync(request.CharacterId, slot);
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

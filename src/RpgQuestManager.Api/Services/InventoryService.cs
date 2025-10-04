using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class InventoryService
{
    private readonly ApplicationDbContext _db;

    public InventoryService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<InventoryItem>> GetCharacterInventoryAsync(int characterId)
    {
        return await _db.InventoryItems
            .Include(ii => ii.Item)
            .Where(ii => ii.CharacterId == characterId)
            .OrderBy(ii => ii.Item.Type)
            .ThenBy(ii => ii.Item.Name)
            .ToListAsync();
    }

    public async Task<InventoryItem?> AddItemAsync(int characterId, int itemId, int quantity = 1)
    {
        var item = await _db.Items.FindAsync(itemId);
        if (item == null) return null;

        var existingItem = await _db.InventoryItems
            .FirstOrDefaultAsync(ii => ii.CharacterId == characterId && ii.ItemId == itemId);

        if (existingItem != null)
        {
            // Item já existe, aumentar quantidade
            existingItem.Quantity += quantity;
            await _db.SaveChangesAsync();
            return existingItem;
        }
        else
        {
            // Novo item
            var inventoryItem = new InventoryItem
            {
                CharacterId = characterId,
                ItemId = itemId,
                Quantity = quantity,
                AcquiredAt = DateTime.UtcNow
            };

            _db.InventoryItems.Add(inventoryItem);
            await _db.SaveChangesAsync();
            
            return await _db.InventoryItems
                .Include(ii => ii.Item)
                .FirstOrDefaultAsync(ii => ii.Id == inventoryItem.Id);
        }
    }

    public async Task<bool> RemoveItemAsync(int characterId, int itemId, int quantity = 1)
    {
        var inventoryItem = await _db.InventoryItems
            .FirstOrDefaultAsync(ii => ii.CharacterId == characterId && ii.ItemId == itemId);

        if (inventoryItem == null) return false;

        if (inventoryItem.Quantity <= quantity)
        {
            _db.InventoryItems.Remove(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity -= quantity;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EquipItemAsync(int characterId, int inventoryItemId, EquipmentSlot slot)
    {
        var inventoryItem = await _db.InventoryItems
            .Include(ii => ii.Item)
            .FirstOrDefaultAsync(ii => ii.Id == inventoryItemId && ii.CharacterId == characterId);

        if (inventoryItem == null)
            return false;

        // Verificar se o item pode ser equipado
        if (inventoryItem.Item.Type != ItemType.Weapon && 
            inventoryItem.Item.Type != ItemType.Armor && 
            inventoryItem.Item.Type != ItemType.Accessory)
            return false;

        // Verificar se o slot é compatível com o tipo do item
        if (!IsSlotCompatible(inventoryItem.Item.Type, slot))
            return false;

        // Desequipar item atual no slot
        await UnequipSlotAsync(characterId, slot);

        // Equipar novo item
        inventoryItem.IsEquipped = true;
        inventoryItem.EquippedSlot = slot;

        // Atualizar equipamento do personagem
        var equipment = await GetOrCreateEquipmentAsync(characterId);
        SetEquipmentSlot(equipment, slot, inventoryItemId);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnequipItemAsync(int characterId, EquipmentSlot slot)
    {
        var inventoryItem = await _db.InventoryItems
            .FirstOrDefaultAsync(ii => ii.CharacterId == characterId && ii.EquippedSlot == slot);

        if (inventoryItem == null) return false;

        inventoryItem.IsEquipped = false;
        inventoryItem.EquippedSlot = null;

        // Atualizar equipamento do personagem
        var equipment = await GetOrCreateEquipmentAsync(characterId);
        SetEquipmentSlot(equipment, slot, null);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<CharacterEquipment> GetCharacterEquipmentAsync(int characterId)
    {
        return await GetOrCreateEquipmentAsync(characterId);
    }

    public async Task<(int attack, int defense, int health, int morale)> GetEquipmentBonusesAsync(int characterId)
    {
        var equipment = await _db.CharacterEquipment
            .Include(ce => ce.Weapon).ThenInclude(ii => ii.Item)
            .Include(ce => ce.Shield).ThenInclude(ii => ii.Item)
            .Include(ce => ce.Helmet).ThenInclude(ii => ii.Item)
            .Include(ce => ce.Armor).ThenInclude(ii => ii.Item)
            .Include(ce => ce.Gloves).ThenInclude(ii => ii.Item)
            .Include(ce => ce.Boots).ThenInclude(ii => ii.Item)
            .Include(ce => ce.Ring).ThenInclude(ii => ii.Item)
            .Include(ce => ce.Amulet).ThenInclude(ii => ii.Item)
            .FirstOrDefaultAsync(ce => ce.CharacterId == characterId);

        if (equipment == null) return (0, 0, 0, 0);

        int attack = 0, defense = 0, health = 0, morale = 0;

        var equippedItems = new[] { equipment.Weapon, equipment.Shield, equipment.Helmet, 
                                   equipment.Armor, equipment.Gloves, equipment.Boots, 
                                   equipment.Ring, equipment.Amulet };

        foreach (var item in equippedItems.Where(i => i != null))
        {
            attack += item!.Item.AttackBonus ?? 0;
            defense += item!.Item.DefenseBonus ?? 0;
            health += item!.Item.HealthBonus ?? 0;
            morale += item!.Item.MoraleBonus ?? 0;
        }

        return (attack, defense, health, morale);
    }

    public async Task<bool> UseItemAsync(int characterId, int inventoryItemId)
    {
        var inventoryItem = await _db.InventoryItems
            .Include(ii => ii.Item)
            .FirstOrDefaultAsync(ii => ii.Id == inventoryItemId && ii.CharacterId == characterId);

        if (inventoryItem == null || !inventoryItem.Item.IsConsumable)
            return false;

        // Aplicar efeitos do item
        var character = await _db.Characters.FindAsync(characterId);
        if (character == null) return false;

        // Aplicar bônus de HP
        if (inventoryItem.Item.HealthBonus.HasValue)
        {
            character.Health = Math.Min(character.MaxHealth, 
                character.Health + inventoryItem.Item.HealthBonus.Value);
        }

        // Aplicar bônus de moral
        if (inventoryItem.Item.MoraleBonus.HasValue)
        {
            character.Morale = Math.Min(100, 
                character.Morale + inventoryItem.Item.MoraleBonus.Value);
        }

        // Remover item do inventário
        if (inventoryItem.Quantity <= 1)
        {
            _db.InventoryItems.Remove(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity--;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    private async Task<CharacterEquipment> GetOrCreateEquipmentAsync(int characterId)
    {
        var equipment = await _db.CharacterEquipment
            .FirstOrDefaultAsync(ce => ce.CharacterId == characterId);

        if (equipment == null)
        {
            equipment = new CharacterEquipment
            {
                CharacterId = characterId
            };
            _db.CharacterEquipment.Add(equipment);
            await _db.SaveChangesAsync();
        }

        return equipment;
    }

    private bool IsSlotCompatible(ItemType itemType, EquipmentSlot slot)
    {
        return itemType switch
        {
            ItemType.Weapon => slot == EquipmentSlot.Weapon,
            ItemType.Armor => slot is EquipmentSlot.Helmet or EquipmentSlot.Armor or 
                             EquipmentSlot.Gloves or EquipmentSlot.Boots or EquipmentSlot.Shield,
            ItemType.Accessory => slot is EquipmentSlot.Ring or EquipmentSlot.Amulet,
            _ => false
        };
    }

    private void SetEquipmentSlot(CharacterEquipment equipment, EquipmentSlot slot, int? inventoryItemId)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon:
                equipment.WeaponId = inventoryItemId;
                break;
            case EquipmentSlot.Shield:
                equipment.ShieldId = inventoryItemId;
                break;
            case EquipmentSlot.Helmet:
                equipment.HelmetId = inventoryItemId;
                break;
            case EquipmentSlot.Armor:
                equipment.ArmorId = inventoryItemId;
                break;
            case EquipmentSlot.Gloves:
                equipment.GlovesId = inventoryItemId;
                break;
            case EquipmentSlot.Boots:
                equipment.BootsId = inventoryItemId;
                break;
            case EquipmentSlot.Ring:
                equipment.RingId = inventoryItemId;
                break;
            case EquipmentSlot.Amulet:
                equipment.AmuletId = inventoryItemId;
                break;
        }
    }

    private async Task UnequipSlotAsync(int characterId, EquipmentSlot slot)
    {
        var inventoryItem = await _db.InventoryItems
            .FirstOrDefaultAsync(ii => ii.CharacterId == characterId && ii.EquippedSlot == slot);

        if (inventoryItem != null)
        {
            inventoryItem.IsEquipped = false;
            inventoryItem.EquippedSlot = null;
        }
    }
}

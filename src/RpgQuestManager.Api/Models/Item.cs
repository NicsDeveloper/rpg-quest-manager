namespace RpgQuestManager.Api.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemType Type { get; set; } = ItemType.Material;
    public ItemRarity Rarity { get; set; } = ItemRarity.Common;
    public EquipmentSlot? EquipmentSlot { get; set; }
    public int Level { get; set; } = 1;
    public int Value { get; set; } = 0;
    public int StackSize { get; set; } = 1;
    public bool IsConsumable { get; set; } = false;
    public bool IsTradeable { get; set; } = true;
    public bool IsSellable { get; set; } = true;
    
    // Atributos de equipamento
    public int? AttackBonus { get; set; }
    public int? DefenseBonus { get; set; }
    public int? HealthBonus { get; set; }
    public int? MoraleBonus { get; set; }
    
    // Efeitos especiais
    public List<StatusEffectType> StatusEffects { get; set; } = new();
    public int? StatusEffectChance { get; set; }
    public int? StatusEffectDuration { get; set; }
    
    // Requisitos
    public int? RequiredLevel { get; set; }
    public List<string> RequiredClasses { get; set; } = new();
    
    // Dados de drop
    public List<MonsterType> DroppedBy { get; set; } = new();
    public int DropChance { get; set; } = 100; // 1-100%
    public List<EnvironmentType> FoundIn { get; set; } = new();
    
    // Dados de loja
    public bool AvailableInShop { get; set; } = false;
    public int ShopPrice { get; set; } = 0;
    public List<string> ShopTypes { get; set; } = new();
}

public class InventoryItem
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsEquipped { get; set; } = false;
    public EquipmentSlot? EquippedSlot { get; set; }
    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
    
    // Navegação
    public Hero Hero { get; set; } = null!;
    public Item Item { get; set; } = null!;
}

public class HeroEquipment
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public int? WeaponId { get; set; }
    public int? ShieldId { get; set; }
    public int? HelmetId { get; set; }
    public int? ArmorId { get; set; }
    public int? GlovesId { get; set; }
    public int? BootsId { get; set; }
    public int? RingId { get; set; }
    public int? AmuletId { get; set; }
    
    // Navegação
    public Hero Hero { get; set; } = null!;
    public InventoryItem? Weapon { get; set; }
    public InventoryItem? Shield { get; set; }
    public InventoryItem? Helmet { get; set; }
    public InventoryItem? Armor { get; set; }
    public InventoryItem? Gloves { get; set; }
    public InventoryItem? Boots { get; set; }
    public InventoryItem? Ring { get; set; }
    public InventoryItem? Amulet { get; set; }
}

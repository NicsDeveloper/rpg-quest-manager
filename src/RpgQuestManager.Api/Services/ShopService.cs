using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class ShopService
{
    private readonly ApplicationDbContext _db;
    private readonly InventoryService _inventoryService;

    public ShopService(ApplicationDbContext db, InventoryService inventoryService)
    {
        _db = db;
        _inventoryService = inventoryService;
    }

    public async Task<List<Item>> GetShopItemsAsync(string shopType = "general")
    {
        var items = await _db.Items
            .Where(i => i.AvailableInShop)
            .ToListAsync();

        return items
            .Where(i => i.ShopTypes.Contains(shopType) || i.ShopTypes.Contains("general"))
            .OrderBy(i => i.Type)
            .ThenBy(i => i.Level)
            .ThenBy(i => i.Rarity)
            .ToList();
    }

    public async Task<List<Item>> GetShopItemsByTypeAsync(ItemType itemType, string shopType = "general")
    {
        var items = await _db.Items
            .Where(i => i.AvailableInShop && i.Type == itemType)
            .ToListAsync();

        return items
            .Where(i => i.ShopTypes.Contains(shopType) || i.ShopTypes.Contains("general"))
            .OrderBy(i => i.Level)
            .ThenBy(i => i.Rarity)
            .ToList();
    }

    public async Task<List<Item>> GetShopItemsByLevelAsync(int characterLevel, string shopType = "general")
    {
        var items = await _db.Items
            .Where(i => i.AvailableInShop && 
                       (i.RequiredLevel == null || i.RequiredLevel <= characterLevel))
            .ToListAsync();

        return items
            .Where(i => i.ShopTypes.Contains(shopType) || i.ShopTypes.Contains("general"))
            .OrderBy(i => i.Type)
            .ThenBy(i => i.Level)
            .ThenBy(i => i.Rarity)
            .ToList();
    }

    public async Task<(bool success, InventoryItem? inventoryItem)> BuyItemAsync(int heroId, int itemId, int quantity = 1)
    {
        var hero = await _db.Heroes.FindAsync(heroId);
        var item = await _db.Items.FindAsync(itemId);

        if (hero == null || item == null || !item.AvailableInShop)
            return (false, null);

        var totalCost = item.ShopPrice * quantity;

        // Buscar o usuário dono do herói
        var user = await _db.Users.FindAsync(hero.UserId);
        if (user == null)
            return (false, null);

        // Verificar se o usuário tem ouro suficiente
        if (user.Gold < totalCost)
            return (false, null);

        // Verificar se o herói atende aos requisitos
        if (item.RequiredLevel.HasValue && hero.Level < item.RequiredLevel.Value)
            return (false, null);

        // Adicionar item ao inventário do herói
        var inventoryItem = await _inventoryService.AddItemAsync(heroId, itemId, quantity);
        if (inventoryItem == null)
            return (false, null);

        // Deduzir ouro do usuário
        user.Gold -= totalCost;
        await _db.SaveChangesAsync();

        // Recarregar com item incluído
        await _db.Entry(inventoryItem).Reference(ii => ii.Item).LoadAsync();

        return (true, inventoryItem);
    }

    public async Task<bool> SellItemAsync(int heroId, int inventoryItemId, int quantity = 1)
    {
        var inventoryItem = await _db.InventoryItems
            .Include(ii => ii.Item)
            .FirstOrDefaultAsync(ii => ii.Id == inventoryItemId && ii.HeroId == heroId);

        if (inventoryItem == null || !inventoryItem.Item.IsSellable)
            return false;

        var hero = await _db.Heroes.FindAsync(heroId);
        if (hero == null) return false;

        // Buscar o usuário dono do herói
        var user = await _db.Users.FindAsync(hero.UserId);
        if (user == null) return false;

        // Verificar se o item está equipado
        if (inventoryItem.IsEquipped)
            return false;

        // Calcular valor de venda (50% do preço da loja)
        var sellPrice = (inventoryItem.Item.ShopPrice / 2) * quantity;

        // Verificar quantidade disponível
        if (inventoryItem.Quantity < quantity)
            return false;

        // Remover item do inventário
        await _inventoryService.RemoveItemAsync(heroId, inventoryItem.ItemId, quantity);

        // Adicionar ouro ao usuário
        user.Gold += sellPrice;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetItemSellPriceAsync(int itemId)
    {
        var item = await _db.Items.FindAsync(itemId);
        return item?.IsSellable == true ? item.ShopPrice / 2 : 0;
    }

    public async Task<List<string>> GetShopTypesAsync()
    {
        var items = await _db.Items
            .Where(i => i.AvailableInShop)
            .ToListAsync();

        var shopTypes = items
            .SelectMany(i => i.ShopTypes)
            .Distinct()
            .ToList();

        return shopTypes;
    }

    public string GetShopTypeDescription(string shopType)
    {
        return shopType switch
        {
            "general" => "Loja Geral",
            "weapon" => "Armeiro",
            "armor" => "Armadureiro",
            "magic" => "Loja de Magia",
            "potion" => "Alquimista",
            "blacksmith" => "Ferreiro",
            "jeweler" => "Joalheiro",
            "special" => "Loja Especial",
            _ => shopType
        };
    }

    public string GetRarityColor(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => "#808080",      // Cinza
            ItemRarity.Uncommon => "#00FF00",    // Verde
            ItemRarity.Rare => "#0080FF",        // Azul
            ItemRarity.Epic => "#8000FF",        // Roxo
            ItemRarity.Legendary => "#FF8000",   // Laranja
            ItemRarity.Mythic => "#FF0080",      // Rosa
            _ => "#808080"
        };
    }

    public string GetRarityDescription(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => "Comum",
            ItemRarity.Uncommon => "Incomum",
            ItemRarity.Rare => "Raro",
            ItemRarity.Epic => "Épico",
            ItemRarity.Legendary => "Lendário",
            ItemRarity.Mythic => "Mítico",
            _ => "Desconhecido"
        };
    }
}

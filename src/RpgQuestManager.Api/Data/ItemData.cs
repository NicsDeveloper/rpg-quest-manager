using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Data;

public static class ItemData
{
    public static List<Item> GetAllItems()
    {
        return new List<Item>
        {
            // ARMAS (20 itens)
            new Item
            {
                Name = "Espada de Ferro",
                Description = "Uma espada básica feita de ferro.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Common,
                EquipmentSlot = EquipmentSlot.Weapon,
                Level = 1,
                Value = 50,
                AttackBonus = 5,
                RequiredLevel = 1,
                AvailableInShop = true,
                ShopPrice = 100,
                ShopTypes = new List<string> { "weapon", "general" }
            },
            new Item
            {
                Name = "Espada de Aço",
                Description = "Uma espada resistente feita de aço.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Uncommon,
                EquipmentSlot = EquipmentSlot.Weapon,
                Level = 3,
                Value = 150,
                AttackBonus = 8,
                RequiredLevel = 3,
                AvailableInShop = true,
                ShopPrice = 300,
                ShopTypes = new List<string> { "weapon", "general" }
            },
            new Item
            {
                Name = "Espada Mágica",
                Description = "Uma espada imbuída com magia.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Rare,
                EquipmentSlot = EquipmentSlot.Weapon,
                Level = 5,
                Value = 500,
                AttackBonus = 12,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.StrengthBoost },
                StatusEffectChance = 20,
                StatusEffectDuration = 2,
                RequiredLevel = 5,
                AvailableInShop = true,
                ShopPrice = 1000,
                ShopTypes = new List<string> { "weapon", "magic" }
            },
            new Item
            {
                Name = "Espada de Dragão",
                Description = "Uma espada forjada com escamas de dragão.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Epic,
                EquipmentSlot = EquipmentSlot.Weapon,
                Level = 10,
                Value = 2000,
                AttackBonus = 20,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Poison },
                StatusEffectChance = 30,
                StatusEffectDuration = 3,
                RequiredLevel = 10,
                AvailableInShop = true,
                ShopPrice = 4000,
                ShopTypes = new List<string> { "weapon", "special" }
            },
            new Item
            {
                Name = "Espada Lendária",
                Description = "A espada mais poderosa já forjada.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Legendary,
                EquipmentSlot = EquipmentSlot.Weapon,
                Level = 15,
                Value = 5000,
                AttackBonus = 30,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.StrengthBoost, StatusEffectType.Shielded },
                StatusEffectChance = 50,
                StatusEffectDuration = 5,
                RequiredLevel = 15,
                AvailableInShop = false,
                ShopPrice = 0,
                ShopTypes = new List<string>()
            },
            new Item
            {
                Name = "Machado de Guerra",
                Description = "Um machado pesado para batalha.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Uncommon,
                EquipmentSlot = EquipmentSlot.Weapon,
                Level = 4,
                Value = 200,
                AttackBonus = 10,
                RequiredLevel = 4,
                AvailableInShop = true,
                ShopPrice = 400,
                ShopTypes = new List<string> { "weapon", "general" }
            },
            new Item
            {
                Name = "Machado do Troll Rei",
                Description = "O machado lendário do Troll Rei.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Epic,
                EquipmentSlot = EquipmentSlot.Weapon,
                Level = 10,
                Value = 2500,
                AttackBonus = 22,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Stunned },
                StatusEffectChance = 25,
                StatusEffectDuration = 2,
                RequiredLevel = 10,
                AvailableInShop = false,
                ShopPrice = 0,
                ShopTypes = new List<string>()
            },
            new Item
            {
                Name = "Arco de Caça",
                Description = "Um arco simples para caça.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Common,
                EquipmentSlot = EquipmentSlot.Weapon,
                Level = 2,
                Value = 75,
                AttackBonus = 6,
                RequiredLevel = 2,
                AvailableInShop = true,
                ShopPrice = 150,
                ShopTypes = new List<string> { "weapon", "general" }
            },
            new Item
            {
                Name = "Arco Élfico",
                Description = "Um arco elegante feito pelos elfos.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Rare,
                EquipmentSlot = EquipmentSlot.Weapon,
                Level = 6,
                Value = 600,
                AttackBonus = 14,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Bleeding },
                StatusEffectChance = 35,
                StatusEffectDuration = 3,
                RequiredLevel = 6,
                AvailableInShop = true,
                ShopPrice = 1200,
                ShopTypes = new List<string> { "weapon", "special" }
            },
            new Item
            {
                Name = "Cajado do Mago",
                Description = "Um cajado imbuído com poder mágico.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Rare,
                EquipmentSlot = EquipmentSlot.Weapon,
                Level = 7,
                Value = 800,
                AttackBonus = 16,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Fear },
                StatusEffectChance = 40,
                StatusEffectDuration = 2,
                RequiredLevel = 7,
                AvailableInShop = true,
                ShopPrice = 1600,
                ShopTypes = new List<string> { "weapon", "magic" }
            },

            // ARMADURAS (20 itens)
            new Item
            {
                Name = "Armadura de Couro",
                Description = "Uma armadura básica feita de couro.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Common,
                EquipmentSlot = EquipmentSlot.Armor,
                Level = 1,
                Value = 40,
                DefenseBonus = 3,
                RequiredLevel = 1,
                AvailableInShop = true,
                ShopPrice = 80,
                ShopTypes = new List<string> { "armor", "general" }
            },
            new Item
            {
                Name = "Armadura de Aço",
                Description = "Uma armadura resistente feita de aço.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Uncommon,
                EquipmentSlot = EquipmentSlot.Armor,
                Level = 3,
                Value = 120,
                DefenseBonus = 6,
                RequiredLevel = 3,
                AvailableInShop = true,
                ShopPrice = 240,
                ShopTypes = new List<string> { "armor", "general" }
            },
            new Item
            {
                Name = "Armadura Mágica",
                Description = "Uma armadura imbuída com magia.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Rare,
                EquipmentSlot = EquipmentSlot.Armor,
                Level = 5,
                Value = 400,
                DefenseBonus = 10,
                HealthBonus = 20,
                RequiredLevel = 5,
                AvailableInShop = true,
                ShopPrice = 800,
                ShopTypes = new List<string> { "armor", "magic" }
            },
            new Item
            {
                Name = "Armadura de Dragão",
                Description = "Uma armadura forjada com escamas de dragão.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Epic,
                EquipmentSlot = EquipmentSlot.Armor,
                Level = 10,
                Value = 1500,
                DefenseBonus = 18,
                HealthBonus = 50,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Shielded },
                StatusEffectChance = 25,
                StatusEffectDuration = 3,
                RequiredLevel = 10,
                AvailableInShop = true,
                ShopPrice = 3000,
                ShopTypes = new List<string> { "armor", "special" }
            },
            new Item
            {
                Name = "Armadura Épica",
                Description = "A armadura mais poderosa já forjada.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Legendary,
                EquipmentSlot = EquipmentSlot.Armor,
                Level = 15,
                Value = 4000,
                DefenseBonus = 25,
                HealthBonus = 100,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Shielded, StatusEffectType.StrengthBoost },
                StatusEffectChance = 40,
                StatusEffectDuration = 5,
                RequiredLevel = 15,
                AvailableInShop = false,
                ShopPrice = 0,
                ShopTypes = new List<string>()
            },
            new Item
            {
                Name = "Capacete de Ferro",
                Description = "Um capacete básico de ferro.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Common,
                EquipmentSlot = EquipmentSlot.Helmet,
                Level = 1,
                Value = 30,
                DefenseBonus = 2,
                RequiredLevel = 1,
                AvailableInShop = true,
                ShopPrice = 60,
                ShopTypes = new List<string> { "armor", "general" }
            },
            new Item
            {
                Name = "Capacete de Aço",
                Description = "Um capacete resistente de aço.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Uncommon,
                EquipmentSlot = EquipmentSlot.Helmet,
                Level = 3,
                Value = 90,
                DefenseBonus = 4,
                RequiredLevel = 3,
                AvailableInShop = true,
                ShopPrice = 180,
                ShopTypes = new List<string> { "armor", "general" }
            },
            new Item
            {
                Name = "Escudo de Madeira",
                Description = "Um escudo básico de madeira.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Common,
                EquipmentSlot = EquipmentSlot.Shield,
                Level = 2,
                Value = 35,
                DefenseBonus = 3,
                RequiredLevel = 2,
                AvailableInShop = true,
                ShopPrice = 70,
                ShopTypes = new List<string> { "armor", "general" }
            },
            new Item
            {
                Name = "Escudo de Aço",
                Description = "Um escudo resistente de aço.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Uncommon,
                EquipmentSlot = EquipmentSlot.Shield,
                Level = 4,
                Value = 100,
                DefenseBonus = 6,
                RequiredLevel = 4,
                AvailableInShop = true,
                ShopPrice = 200,
                ShopTypes = new List<string> { "armor", "general" }
            },
            new Item
            {
                Name = "Escudo Mágico",
                Description = "Um escudo imbuído com magia.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Rare,
                EquipmentSlot = EquipmentSlot.Shield,
                Level = 6,
                Value = 300,
                DefenseBonus = 8,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Shielded },
                StatusEffectChance = 30,
                StatusEffectDuration = 2,
                RequiredLevel = 6,
                AvailableInShop = true,
                ShopPrice = 600,
                ShopTypes = new List<string> { "armor", "magic" }
            },

            // POÇÕES (15 itens)
            new Item
            {
                Name = "Poção de Cura",
                Description = "Uma poção que restaura a saúde.",
                Type = ItemType.Potion,
                Rarity = ItemRarity.Common,
                Level = 1,
                Value = 25,
                StackSize = 10,
                IsConsumable = true,
                HealthBonus = 50,
                RequiredLevel = 1,
                AvailableInShop = true,
                ShopPrice = 50,
                ShopTypes = new List<string> { "potion", "general" }
            },
            new Item
            {
                Name = "Poção de Cura Maior",
                Description = "Uma poção que restaura muita saúde.",
                Type = ItemType.Potion,
                Rarity = ItemRarity.Uncommon,
                Level = 3,
                Value = 75,
                StackSize = 10,
                IsConsumable = true,
                HealthBonus = 100,
                RequiredLevel = 3,
                AvailableInShop = true,
                ShopPrice = 150,
                ShopTypes = new List<string> { "potion", "general" }
            },
            new Item
            {
                Name = "Poção de Força",
                Description = "Uma poção que aumenta temporariamente a força.",
                Type = ItemType.Potion,
                Rarity = ItemRarity.Uncommon,
                Level = 2,
                Value = 50,
                StackSize = 5,
                IsConsumable = true,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.StrengthBoost },
                StatusEffectChance = 100,
                StatusEffectDuration = 5,
                RequiredLevel = 2,
                AvailableInShop = true,
                ShopPrice = 100,
                ShopTypes = new List<string> { "potion", "general" }
            },
            new Item
            {
                Name = "Poção de Moral",
                Description = "Uma poção que restaura a moral.",
                Type = ItemType.Potion,
                Rarity = ItemRarity.Uncommon,
                Level = 2,
                Value = 40,
                StackSize = 5,
                IsConsumable = true,
                MoraleBonus = 30,
                RequiredLevel = 2,
                AvailableInShop = true,
                ShopPrice = 80,
                ShopTypes = new List<string> { "potion", "general" }
            },
            new Item
            {
                Name = "Poção de Fogo",
                Description = "Uma poção que causa dano de fogo.",
                Type = ItemType.Potion,
                Rarity = ItemRarity.Rare,
                Level = 5,
                Value = 150,
                StackSize = 3,
                IsConsumable = true,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Poison },
                StatusEffectChance = 100,
                StatusEffectDuration = 3,
                RequiredLevel = 5,
                AvailableInShop = true,
                ShopPrice = 300,
                ShopTypes = new List<string> { "potion", "magic" }
            },
            new Item
            {
                Name = "Poção de Invisibilidade",
                Description = "Uma poção que torna o usuário invisível.",
                Type = ItemType.Potion,
                Rarity = ItemRarity.Rare,
                Level = 6,
                Value = 200,
                StackSize = 2,
                IsConsumable = true,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Shielded },
                StatusEffectChance = 100,
                StatusEffectDuration = 3,
                RequiredLevel = 6,
                AvailableInShop = true,
                ShopPrice = 400,
                ShopTypes = new List<string> { "potion", "magic" }
            },
            new Item
            {
                Name = "Poção de Resistência",
                Description = "Uma poção que aumenta a resistência.",
                Type = ItemType.Potion,
                Rarity = ItemRarity.Uncommon,
                Level = 4,
                Value = 80,
                StackSize = 5,
                IsConsumable = true,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Shielded },
                StatusEffectChance = 100,
                StatusEffectDuration = 4,
                RequiredLevel = 4,
                AvailableInShop = true,
                ShopPrice = 160,
                ShopTypes = new List<string> { "potion", "general" }
            },
            new Item
            {
                Name = "Poção de Velocidade",
                Description = "Uma poção que aumenta a velocidade.",
                Type = ItemType.Potion,
                Rarity = ItemRarity.Rare,
                Level = 5,
                Value = 120,
                StackSize = 3,
                IsConsumable = true,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.StrengthBoost },
                StatusEffectChance = 100,
                StatusEffectDuration = 3,
                RequiredLevel = 5,
                AvailableInShop = true,
                ShopPrice = 240,
                ShopTypes = new List<string> { "potion", "magic" }
            },
            new Item
            {
                Name = "Poção de Cura Épica",
                Description = "Uma poção que restaura toda a saúde.",
                Type = ItemType.Potion,
                Rarity = ItemRarity.Epic,
                Level = 8,
                Value = 300,
                StackSize = 1,
                IsConsumable = true,
                HealthBonus = 999,
                RequiredLevel = 8,
                AvailableInShop = true,
                ShopPrice = 600,
                ShopTypes = new List<string> { "potion", "special" }
            },
            new Item
            {
                Name = "Poção de Ressurreição",
                Description = "Uma poção que revive o usuário se morto.",
                Type = ItemType.Potion,
                Rarity = ItemRarity.Legendary,
                Level = 10,
                Value = 500,
                StackSize = 1,
                IsConsumable = true,
                HealthBonus = 999,
                MoraleBonus = 100,
                RequiredLevel = 10,
                AvailableInShop = false,
                ShopPrice = 0,
                ShopTypes = new List<string>()
            },

            // ACESSÓRIOS (15 itens)
            new Item
            {
                Name = "Anel de Força",
                Description = "Um anel que aumenta a força.",
                Type = ItemType.Accessory,
                Rarity = ItemRarity.Uncommon,
                EquipmentSlot = EquipmentSlot.Ring,
                Level = 3,
                Value = 100,
                AttackBonus = 3,
                RequiredLevel = 3,
                AvailableInShop = true,
                ShopPrice = 200,
                ShopTypes = new List<string> { "jeweler", "general" }
            },
            new Item
            {
                Name = "Anel de Proteção",
                Description = "Um anel que aumenta a defesa.",
                Type = ItemType.Accessory,
                Rarity = ItemRarity.Uncommon,
                EquipmentSlot = EquipmentSlot.Ring,
                Level = 3,
                Value = 100,
                DefenseBonus = 3,
                RequiredLevel = 3,
                AvailableInShop = true,
                ShopPrice = 200,
                ShopTypes = new List<string> { "jeweler", "general" }
            },
            new Item
            {
                Name = "Amuleto da Vida",
                Description = "Um amuleto que aumenta a saúde.",
                Type = ItemType.Accessory,
                Rarity = ItemRarity.Rare,
                EquipmentSlot = EquipmentSlot.Amulet,
                Level = 5,
                Value = 250,
                HealthBonus = 30,
                RequiredLevel = 5,
                AvailableInShop = true,
                ShopPrice = 500,
                ShopTypes = new List<string> { "jeweler", "magic" }
            },
            new Item
            {
                Name = "Amuleto da Coragem",
                Description = "Um amuleto que aumenta a moral.",
                Type = ItemType.Accessory,
                Rarity = ItemRarity.Rare,
                EquipmentSlot = EquipmentSlot.Amulet,
                Level = 5,
                Value = 250,
                MoraleBonus = 20,
                RequiredLevel = 5,
                AvailableInShop = true,
                ShopPrice = 500,
                ShopTypes = new List<string> { "jeweler", "magic" }
            },
            new Item
            {
                Name = "Amuleto de Proteção",
                Description = "Um amuleto que protege contra efeitos negativos.",
                Type = ItemType.Accessory,
                Rarity = ItemRarity.Epic,
                EquipmentSlot = EquipmentSlot.Amulet,
                Level = 8,
                Value = 600,
                DefenseBonus = 8,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Shielded },
                StatusEffectChance = 20,
                StatusEffectDuration = 2,
                RequiredLevel = 8,
                AvailableInShop = true,
                ShopPrice = 1200,
                ShopTypes = new List<string> { "jeweler", "special" }
            },
            new Item
            {
                Name = "Anel do Poder",
                Description = "Um anel que aumenta todos os atributos.",
                Type = ItemType.Accessory,
                Rarity = ItemRarity.Epic,
                EquipmentSlot = EquipmentSlot.Ring,
                Level = 10,
                Value = 800,
                AttackBonus = 5,
                DefenseBonus = 5,
                HealthBonus = 25,
                MoraleBonus = 15,
                RequiredLevel = 10,
                AvailableInShop = true,
                ShopPrice = 1600,
                ShopTypes = new List<string> { "jeweler", "special" }
            },
            new Item
            {
                Name = "Amuleto do Equilíbrio",
                Description = "Um amuleto que mantém o equilíbrio dos elementos.",
                Type = ItemType.Accessory,
                Rarity = ItemRarity.Legendary,
                EquipmentSlot = EquipmentSlot.Amulet,
                Level = 12,
                Value = 1500,
                AttackBonus = 8,
                DefenseBonus = 8,
                HealthBonus = 40,
                MoraleBonus = 25,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Shielded, StatusEffectType.StrengthBoost },
                StatusEffectChance = 30,
                StatusEffectDuration = 3,
                RequiredLevel = 12,
                AvailableInShop = false,
                ShopPrice = 0,
                ShopTypes = new List<string>()
            },
            new Item
            {
                Name = "Coroa do Herói",
                Description = "A coroa lendária dos heróis.",
                Type = ItemType.Accessory,
                Rarity = ItemRarity.Mythic,
                EquipmentSlot = EquipmentSlot.Amulet,
                Level = 15,
                Value = 5000,
                AttackBonus = 15,
                DefenseBonus = 15,
                HealthBonus = 100,
                MoraleBonus = 50,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.StrengthBoost, StatusEffectType.Shielded, StatusEffectType.Fear },
                StatusEffectChance = 50,
                StatusEffectDuration = 5,
                RequiredLevel = 15,
                AvailableInShop = false,
                ShopPrice = 0,
                ShopTypes = new List<string>()
            },
            new Item
            {
                Name = "Luvas de Ferro",
                Description = "Luvas básicas de ferro.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Common,
                EquipmentSlot = EquipmentSlot.Gloves,
                Level = 1,
                Value = 20,
                DefenseBonus = 1,
                RequiredLevel = 1,
                AvailableInShop = true,
                ShopPrice = 40,
                ShopTypes = new List<string> { "armor", "general" }
            },
            new Item
            {
                Name = "Botas de Couro",
                Description = "Botas básicas de couro.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Common,
                EquipmentSlot = EquipmentSlot.Boots,
                Level = 1,
                Value = 25,
                DefenseBonus = 1,
                RequiredLevel = 1,
                AvailableInShop = true,
                ShopPrice = 50,
                ShopTypes = new List<string> { "armor", "general" }
            }
        };
    }
}

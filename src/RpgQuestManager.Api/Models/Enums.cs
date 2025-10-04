namespace RpgQuestManager.Api.Models;

public enum EnvironmentType
{
    Forest,
    Desert,
    Dungeon,
    Castle,
    Volcano,
    Swamp,
    Tundra,
    Sky,
    Ruins,
    Temple,
    Crypt
}

public enum MonsterRank
{
    Normal,
    Boss
}

public enum MonsterType
{
    Goblin,
    Orc,
    Dragon,
    Undead,
    Demon,
    Troll,
    Elemental
}

public enum QuestStatus
{
    NotStarted,
    InProgress,
    Completed,
    Failed
}

public enum StatusEffectType
{
    Poison,
    Stunned,
    Weakened,
    StrengthBoost,
    Fear,
    Bleeding,
    Shielded
}

public enum DifficultyLevel
{
    Easy = 6,      // D6
    Medium = 10,   // D10
    Hard = 12,     // D12
    Boss = 20      // D20
}

public enum MoraleLevel
{
    Despair = 0,    // 0-10: +50% dano, -50% defesa, +30% crítico
    Low = 1,        // 11-30: -20% dano, -15% defesa
    Normal = 2,     // 31-70: Sem modificadores
    High = 3,       // 71-90: +20% crítico, +10% dano
    Inspired = 4    // 91-100: +30% crítico, +20% dano, +15% defesa
}

public enum MonsterSize
{
    Tiny,       // Minúsculo: Ratos, insetos
    Small,      // Pequeno: Goblins, kobolds
    Medium,     // Médio: Humanos, orcs
    Large,      // Grande: Ogros, trolls
    Huge,       // Enorme: Dragões, gigantes
    Gargantuan  // Gigantesco: Kraken, tarrasque
}

public enum QuestCategory
{
    MainStory,      // História Principal
    BossFight,      // Batalhas Épicas
    Dungeon,        // Exploração de masmorras
    SpecialEvent,   // Eventos Especiais
    Daily           // Missões Diárias
}

public enum QuestDifficulty
{
    Easy = 1,       // 1-5: Para iniciantes
    Medium = 2,     // 6-15: Para experientes
    Hard = 3,       // 16-25: Para veteranos
    Epic = 4,       // 26-35: Para lendários
    Legendary = 5   // 36+: Para os mais poderosos
}

public enum ItemType
{
    Weapon,         // Arma
    Armor,          // Armadura
    Accessory,      // Acessório
    Potion,         // Poção
    Scroll,         // Pergaminho
    Material,       // Material
    Quest,          // Item de missão
    Currency        // Moeda
}

public enum ItemRarity
{
    Common,         // Comum
    Uncommon,       // Incomum
    Rare,           // Raro
    Epic,           // Épico
    Legendary,      // Lendário
    Mythic          // Mítico
}

public enum EquipmentSlot
{
    Weapon,         // Arma principal
    Shield,         // Escudo
    Helmet,         // Capacete
    Armor,          // Armadura
    Gloves,         // Luvas
    Boots,          // Botas
    Ring,           // Anel
    Amulet          // Amuleto
}



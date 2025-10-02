using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace RpgQuestManager.Api.Data;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        // Verificar se já tem dados
        if (await _context.Heroes.AnyAsync())
        {
            _logger.LogInformation("Banco de dados já contém dados. Seed ignorado.");
            return;
        }

        _logger.LogInformation("🌱 Iniciando seed do banco de dados...");

        await SeedUsersAsync();
        await SeedHeroesAsync();
        await SeedEnemiesAsync();
        await SeedItemsAsync();
        await SeedQuestsAsync();
        await SeedRewardsAsync();
        await SeedQuestEnemiesAsync();
        await SeedHeroItemsAsync();
        await SeedHeroQuestsAsync();
        await SeedBossDropTableAsync();
        await SeedDiceInventoriesAsync();
        await SeedPartyComboWeaknessesAsync(); // Sistema de Party Combos e Boss Weaknesses

        _logger.LogInformation("✅ Seed do banco de dados concluído com sucesso!");
    }

    private async Task SeedUsersAsync()
    {
        // Verifica se já existem usuários
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("👤 Usuários já existem - pulando seed");
            return;
        }

        var users = new List<User>
        {
            new User
            {
                Username = "admin",
                Email = "admin@eldoria.com",
                PasswordHash = HashPassword("admin123"),
                Role = "Admin",
                Gold = 100,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new User
            {
                Username = "player1",
                Email = "player1@eldoria.com",
                PasswordHash = HashPassword("senha123"),
                Role = "Player",
                Gold = 100,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new User
            {
                Username = "gamer",
                Email = "gamer@eldoria.com",
                PasswordHash = HashPassword("senha123"),
                Role = "Player",
                Gold = 100,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
        _logger.LogInformation("👤 {Count} usuários criados", users.Count);
    }

    private async Task SeedHeroesAsync()
    {
        // Não cria heróis automaticamente - os players devem criar seus próprios heróis
        _logger.LogInformation("⚔️ Heróis não criados - players criarão seus próprios heróis");
        await Task.CompletedTask;
    }

    private async Task SeedHeroesAsync_OLD()
    {
        var heroes = new List<Hero>
        {
            new Hero
            {
                Name = "Gimli",
                Class = "Guerreiro",
                Level = 12,
                Experience = 300,
                Strength = 36,
                Intelligence = 18,
                Dexterity = 20,
                Gold = 3500,
                CreatedAt = DateTime.UtcNow.AddDays(-18)
            },
            new Hero
            {
                Name = "Merlin",
                Class = "Mago",
                Level = 14,
                Experience = 200,
                Strength = 16,
                Intelligence = 42,
                Dexterity = 22,
                Gold = 4200,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new Hero
            {
                Name = "Robin Hood",
                Class = "Arqueiro",
                Level = 10,
                Experience = 500,
                Strength = 22,
                Intelligence = 20,
                Dexterity = 38,
                Gold = 2800,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new Hero
            {
                Name = "Arthas",
                Class = "Paladino",
                Level = 13,
                Experience = 450,
                Strength = 32,
                Intelligence = 30,
                Dexterity = 26,
                Gold = 4000,
                CreatedAt = DateTime.UtcNow.AddDays(-17)
            },
            
            // Heróis Iniciantes (Baixo Nível)
            new Hero
            {
                Name = "Frodo",
                Class = "Ladino",
                Level = 5,
                Experience = 80,
                Strength = 18,
                Intelligence = 20,
                Dexterity = 28,
                Gold = 800,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Hero
            {
                Name = "Boromir",
                Class = "Guerreiro",
                Level = 8,
                Experience = 150,
                Strength = 28,
                Intelligence = 16,
                Dexterity = 18,
                Gold = 1500,
                CreatedAt = DateTime.UtcNow.AddDays(-12)
            },
            new Hero
            {
                Name = "Galadriel",
                Class = "Mago",
                Level = 7,
                Experience = 200,
                Strength = 14,
                Intelligence = 32,
                Dexterity = 20,
                Gold = 1200,
                CreatedAt = DateTime.UtcNow.AddDays(-8)
            },
            new Hero
            {
                Name = "Elrond",
                Class = "Paladino",
                Level = 11,
                Experience = 350,
                Strength = 26,
                Intelligence = 34,
                Dexterity = 24,
                Gold = 3000,
                CreatedAt = DateTime.UtcNow.AddDays(-16)
            },
            new Hero
            {
                Name = "Arwen",
                Class = "Arqueiro",
                Level = 9,
                Experience = 250,
                Strength = 20,
                Intelligence = 26,
                Dexterity = 36,
                Gold = 2000,
                CreatedAt = DateTime.UtcNow.AddDays(-11)
            }
        };

        _context.Heroes.AddRange(heroes);
        await _context.SaveChangesAsync();
        _logger.LogInformation("⚔️ {Count} heróis criados", heroes.Count);
    }

    private async Task SeedEnemiesAsync()
    {
        // Verifica se já existem inimigos
        if (await _context.Enemies.AnyAsync())
        {
            _logger.LogInformation("👹 Inimigos já existem - pulando seed");
            return;
        }

        var enemies = new List<Enemy>
        {
            // Inimigos Fracos (D6 - fácil)
            new Enemy { Name = "Goblin Raider", Type = "Goblin", Power = 15, Health = 50, RequiredDiceType = DiceType.D6, MinimumRoll = 3, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new Enemy { Name = "Orc Scout", Type = "Orc", Power = 25, Health = 80, RequiredDiceType = DiceType.D6, MinimumRoll = 4, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new Enemy { Name = "Skeleton Warrior", Type = "Morto-Vivo", Power = 20, Health = 60, RequiredDiceType = DiceType.D6, MinimumRoll = 3, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new Enemy { Name = "Wolf", Type = "Besta", Power = 18, Health = 55, RequiredDiceType = DiceType.D6, MinimumRoll = 3, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new Enemy { Name = "Giant Spider", Type = "Aranha", Power = 22, Health = 70, RequiredDiceType = DiceType.D6, MinimumRoll = 4, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            
            // Inimigos Médios (D10 - médio)
            new Enemy { Name = "Orc Warlord", Type = "Orc", Power = 45, Health = 150, RequiredDiceType = DiceType.D10, MinimumRoll = 5, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            new Enemy { Name = "Dark Wizard", Type = "Humano", Power = 50, Health = 120, RequiredDiceType = DiceType.D10, MinimumRoll = 6, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            new Enemy { Name = "Troll", Type = "Troll", Power = 55, Health = 200, RequiredDiceType = DiceType.D10, MinimumRoll = 5, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            new Enemy { Name = "Vampire", Type = "Morto-Vivo", Power = 48, Health = 140, RequiredDiceType = DiceType.D10, MinimumRoll = 6, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            new Enemy { Name = "Werewolf", Type = "Besta", Power = 52, Health = 160, RequiredDiceType = DiceType.D10, MinimumRoll = 5, IsBoss = false, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            
            // BOSSES (D12 e D20 - muito difícil, com drops especiais)
            new Enemy { Name = "Demon Lord", Type = "Demônio", Power = 75, Health = 300, RequiredDiceType = DiceType.D12, MinimumRoll = 8, IsBoss = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new Enemy { Name = "Elder Dragon", Type = "Dragão", Power = 90, Health = 500, RequiredDiceType = DiceType.D20, MinimumRoll = 15, IsBoss = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new Enemy { Name = "Lich King", Type = "Morto-Vivo", Power = 80, Health = 350, RequiredDiceType = DiceType.D12, MinimumRoll = 9, IsBoss = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new Enemy { Name = "Balrog", Type = "Demônio", Power = 95, Health = 450, RequiredDiceType = DiceType.D20, MinimumRoll = 16, IsBoss = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new Enemy { Name = "Kraken", Type = "Besta", Power = 85, Health = 400, RequiredDiceType = DiceType.D12, MinimumRoll = 10, IsBoss = true, CreatedAt = DateTime.UtcNow.AddDays(-15) }
        };

        _context.Enemies.AddRange(enemies);
        await _context.SaveChangesAsync();
        _logger.LogInformation("👹 {Count} inimigos criados ({BossCount} bosses com drops especiais)", 
            enemies.Count, enemies.Count(e => e.IsBoss));
    }

    private async Task SeedItemsAsync()
    {
        // Verifica se já existem itens
        if (await _context.Items.AnyAsync())
        {
            _logger.LogInformation("🗡️ Itens já existem - pulando seed");
            return;
        }

        var items = new List<Item>
        {
            // COMUM - Itens compartilhados (podem dropar de qualquer boss)
            new Item { Name = "Poção de Cura Menor", Description = "Restaura um pouco de vida", Type = "Poção", Rarity = ItemRarity.Common, Value = 50 },
            new Item { Name = "Poção de Mana", Description = "Restaura energia mágica", Type = "Poção", Rarity = ItemRarity.Common, BonusIntelligence = 3, Value = 50 },
            new Item { Name = "Espada de Ferro", Description = "Uma espada simples mas eficaz", Type = "Espada", Rarity = ItemRarity.Common, BonusStrength = 5, Value = 100 },
            new Item { Name = "Armadura de Couro", Description = "Proteção leve para iniciantes", Type = "Armadura", Rarity = ItemRarity.Common, BonusStrength = 3, BonusDexterity = 2, Value = 150 },
            
            // RARO - Itens menos comuns
            new Item { Name = "Espada Élfica", Description = "Forjada pelos elfos de Rivendell", Type = "Espada", Rarity = ItemRarity.Rare, BonusStrength = 12, BonusDexterity = 5, Value = 500 },
            new Item { Name = "Cota de Malha", Description = "Armadura resistente de anéis de metal", Type = "Armadura", Rarity = ItemRarity.Rare, BonusStrength = 8, Value = 400 },
            new Item { Name = "Arco Longo Élfico", Description = "Arco de madeira élfica com precisão perfeita", Type = "Arco", Rarity = ItemRarity.Rare, BonusDexterity = 15, BonusStrength = 5, Value = 800 },
            
            // ÉPICO I-III - Itens muito raros (Boss Drops)
            new Item { Name = "Espada Flamejante", Description = "Uma lâmina envolta em chamas eternas do Demon Lord", Type = "Espada", Rarity = ItemRarity.Epic, RarityTier = 1, BonusStrength = 20, BonusIntelligence = 8, Value = 1500, IsExclusiveDrop = true },
            new Item { Name = "Escama do Dragão Ancião", Description = "Escama impenetrável do Elder Dragon", Type = "Armadura", Rarity = ItemRarity.Epic, RarityTier = 2, BonusStrength = 18, BonusDexterity = 12, Value = 2000, IsExclusiveDrop = true },
            new Item { Name = "Cajado do Lich", Description = "Cajado necromântico do Lich King", Type = "Cajado", Rarity = ItemRarity.Epic, RarityTier = 2, BonusIntelligence = 25, Value = 1800, IsExclusiveDrop = true },
            new Item { Name = "Chicote Flamejante", Description = "Arma do próprio Balrog", Type = "Chicote", Rarity = ItemRarity.Epic, RarityTier = 3, BonusStrength = 22, BonusIntelligence = 15, Value = 2200, IsExclusiveDrop = true },
            
            // LENDÁRIO I-III - Itens extremamente raros (Boss Drops Exclusivos)
            new Item { Name = "Lâmina Demoníaca: Corruptor", Description = "Espada lendária do Demon Lord. Quem a empunha sente o poder das trevas", Type = "Espada", Rarity = ItemRarity.Legendary, RarityTier = 1, BonusStrength = 30, BonusIntelligence = 20, Value = 5000, IsExclusiveDrop = true },
            new Item { Name = "Coração do Dragão", Description = "O coração ainda pulsante do Elder Dragon, concede poder supremo", Type = "Amuleto", Rarity = ItemRarity.Legendary, RarityTier = 3, BonusStrength = 25, BonusIntelligence = 25, BonusDexterity = 25, Value = 10000, IsExclusiveDrop = true },
            new Item { Name = "Coroa do Lich King", Description = "Coroa amaldiçoada que controla os mortos-vivos", Type = "Capacete", Rarity = ItemRarity.Legendary, RarityTier = 2, BonusIntelligence = 35, Value = 7000, IsExclusiveDrop = true },
            new Item { Name = "Foice do Balrog", Description = "Arma ancestral forjada no coração da montanha", Type = "Foice", Rarity = ItemRarity.Legendary, RarityTier = 3, BonusStrength = 40, BonusDexterity = 15, Value = 8000, IsExclusiveDrop = true },
            new Item { Name = "Tentáculo do Kraken", Description = "Tentáculo arrancado do próprio Kraken, ainda se move!", Type = "Açoite", Rarity = ItemRarity.Legendary, RarityTier = 2, BonusStrength = 28, BonusDexterity = 22, Value = 6500, IsExclusiveDrop = true },
            
            // Itens gerais adicionais
            new Item { Name = "Escudo de Madeira", Description = "Escudo básico de proteção", Type = "Escudo", Rarity = ItemRarity.Common, BonusStrength = 4, Value = 80 },
            new Item { Name = "Manto Mágico", Description = "Manto que amplifica poderes mágicos", Type = "Armadura", Rarity = ItemRarity.Rare, BonusIntelligence = 12, Value = 900 }
        };

        _context.Items.AddRange(items);
        await _context.SaveChangesAsync();
        _logger.LogInformation("🗡️ {Count} itens criados ({Epic} épicos, {Legendary} lendários)", 
            items.Count, 
            items.Count(i => i.Rarity == ItemRarity.Epic),
            items.Count(i => i.Rarity == ItemRarity.Legendary));
    }

    private async Task SeedQuestsAsync()
    {
        // Verifica se já existem quests
        if (await _context.Quests.AnyAsync())
        {
            _logger.LogInformation("🎯 Quests já existem - pulando seed");
            return;
        }

        var quests = new List<Quest>
        {
            // ===== QUEST TUTORIAL (Nível 1 - SEMPRE DISPONÍVEL) =====
            new Quest
            {
                Name = "Os Primeiros Passos: Bem-vindo à Guilda",
                Description = @"🎭 **Bem-vindo à Guilda dos Heróis!**

O Mestre da Guilda, **Eldrin Coração-de-Pedra**, um veterano de mil batalhas, te recebe com um sorriso caloroso mas marcas de preocupação em seu rosto enrugado.

'Aventureiro! Finalmente chegou. Nosso reino, **Valestra**, está em tempos sombrios. Criaturas das trevas emergem das Terras Desoladas, e precisamos de cada braço forte, cada mente brilhante, cada arco certeiro que pudermos reunir.'

Ele aponta para um mapa na parede, mostrando a **Vila de Thornwood**, rodeada por pequenos símbolos vermelhos.

'Goblins. Criaturas covardes, mas perigosas em números. Eles atacaram fazendas próximas e roubaram suprimentos. Sua primeira missão: vá até a **Floresta de Thornwood** e elimine pelo menos 5 goblins. Isso mostrará que você tem o que é preciso para ser um verdadeiro herói.'

**Objetivos:**
• Derrote 5 Goblin Raiders na Floresta de Thornwood
• Retorne à Guilda com prova de sua vitória
• Aprenda as bases do combate e progressão

'Ah, e jovem herói... lembre-se: mesmo os maiores guerreiros começaram enfrentando goblins. Boa sorte!'",
                Difficulty = "Tutorial",
                RequiredLevel = 0,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 100,
                GoldReward = 50,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            
            // ===== QUESTS INICIANTES (Nível 1-3) =====
            new Quest
            {
                Name = "A Infestação do Cemitério",
                Description = @"⚰️ **O Descanso Perturbado**

O **Padre Marcus**, guardião do antigo cemitério de Thornwood, te procura desesperado. Suas mãos tremem enquanto segura um crucifixo de prata.

'Herói, algo terrível aconteceu! Na última lua nova, os mortos começaram a emergir de suas tumbas. Esqueletos guerreiros vagam entre as lápides, atacando qualquer um que se aproxime!'

Ele aponta para marcas de garras na porta da capela.

'Minha congregação não pode mais visitar seus entes queridos. As famílias choram em desespero. **10 esqueletos** atormentam o cemitério. Por favor, devolva-os ao descanso eterno!'

**Local:** Cemitério Antigo de Thornwood
**Inimigos:** Skeleton Warriors (Mortos-vivos amaldiçoados)
**Recompensa especial:** Bênção do Padre Marcus (+10% XP por 1 hora)

'Que a luz te guie nas trevas, bravo guerreiro.'",
                Difficulty = "Fácil",
                RequiredLevel = 2,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 150,
                GoldReward = 120,
                CreatedAt = DateTime.UtcNow.AddDays(-28)
            },
            
            new Quest
            {
                Name = "Os Lobos Famintos da Estrada",
                Description = @"🐺 **Perigo nas Rotas Comerciais**

**Greta**, a mercadora viajante, está presa na taverna há três dias. Ela mostra marcas de mordidas em seu braço enfaixado.

'Nunca vi nada assim em 20 anos de estrada! Uma matilha de lobos, grandes como pôneis, está bloqueando a **Estrada do Leste**. Eles atacaram minha caravana, mataram meus guardas e quase me levaram!'

Ela deixa cair uma bolsa de moedas na mesa.

'Preciso chegar a **Porto Âncora** em dois dias, ou perderei o contrato da vida. Mate pelo menos **8 lobos** e limpe a estrada. Pagarei bem - e você terá minha gratidão eterna.'

**Local:** Estrada do Leste, próximo ao Rio Cristalino
**Inimigos:** Lobos Selvagens (Matilha faminta e agressiva)
**Tempo:** Urgente - Greta precisa partir em breve

'Por favor, não me decepcione. Meu futuro depende disso.'",
                Difficulty = "Fácil",
                RequiredLevel = 3,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 180,
                GoldReward = 150,
                CreatedAt = DateTime.UtcNow.AddDays(-27)
            },

            // ===== QUESTS MÉDIAS (Nível 5-8) =====
            new Quest
            {
                Name = "A Invasão Orc: Defesa de Thornwood",
                Description = @"⚔️ **A HORDA SE APROXIMA**

**Capitão Roderick**, comandante da guarda de Thornwood, bate seu punho na mesa do quartel-general. Mapas de batalha estão espalhados, marcados com símbolos vermelhos de inimigos.

'ESCUTE BEM, HERÓI! Espiões reportam que uma horda orc, liderada pelo brutal **Warlord Grak'thor**, marcha em direção à vila. **TREZENTOS ORCS**. Queimando tudo em seu caminho!'

Ele aponta para o mapa, mostrando a rota dos orcs.

'Não temos tropas suficientes. Preciso que você e outros heróis façam uma **MISSÃO SUICIDA**: infiltrar-se no acampamento orc a leste e **MATAR O WARLORD**. Sem líder, a horda se dispersará.'

Sua expressão se suaviza por um momento.

'Sei que peço muito. Mas se Thornwood cair, **milhares morrerão**. Você é nossa única esperança.'

**Local:** Acampamento Orc, Planícies de Cinzas
**Inimigo:** Orc Warlord Grak'thor (Líder brutal e estrategista)
**Risco:** MUITO ALTO - Infiltração em território inimigo
**Impacto:** SALVA THORNWOOD DE DESTRUIÇÃO

'Volte vivo, herói. Valestra precisa de você.'",
                Difficulty = "Médio",
                RequiredLevel = 5,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 400,
                GoldReward = 500,
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            },

            new Quest
            {
                Name = "O Feiticeiro Negro de Ravenmoor",
                Description = @"🌑 **MAGIA NEGRA E MALDIÇÕES**

A **Arquimaga Lyria**, da Academia Arcana, te convoca urgentemente. Círculos mágicos brilham no chão de sua torre enquanto ela estuda pergaminhos antigos.

'Detectei uma presença sombria no pântano de Ravenmoor. Um feiticeiro negro, **Malachar, o Corrompido**, está realizando rituais proibidos!'

Ela aponta para uma bola de cristal mostrando visões de aldeões transformados em criaturas.

'Ele está transmutando aldeões em abominações! Já perdemos **20 pessoas**. Se não pararmos, ele terá poder suficiente para convocar algo... pior.'

Sua voz se torna grave.

'Malachar foi meu aluno, há anos. Eu o expulsei por praticar necromancia. Agora voltou para se vingar. Você deve confrontá-lo em sua **Torre das Sombras**.'

**Local:** Torre das Sombras, Pântano de Ravenmoor
**Inimigo:** Dark Wizard Malachar (Ex-membro da Academia, extremamente perigoso)
**Habilidades inimigas:** Magia negra, invocações demoníacas, maldições
**Recompensa especial:** Tomo de Magia Antiga

'Tenha cuidado, herói. Ele conhece todos os truques que eu ensino...'",
                Difficulty = "Médio",
                RequiredLevel = 6,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 450,
                GoldReward = 600,
                CreatedAt = DateTime.UtcNow.AddDays(-23)
            },

            new Quest
            {
                Name = "A Maldição do Lobisomem",
                Description = @"🌕 **HORROR NAS NOITES DE LUA CHEIA**

O **Lorde Edmund**, governante de Silverwood, te recebe em seu solar. Ele parece ter envelhecido 20 anos. Suas mãos tremem ao te oferecer vinho.

'A cada lua cheia, a mesma história. Corpos dilacerados. Famílias destroçadas. Um **LOBISOMEM** assombra meus domínios há três meses.'

Ele mostra retratos de família na parede - metade cobertos por pano negro.

'Perdi meu filho primogênito na primeira noite. Minha filha na segunda. Agora minha esposa está trancada na torre, rezando. Os aldeões falam em abandonar Silverwood.'

Sua voz se quebra.

'O mestre de caça identificou a toca da criatura: as **Cavernas de Prata** ao norte. Mate-o. Por favor. Antes da próxima lua cheia - em **3 dias**.'

**Local:** Cavernas de Prata, Montanhas do Norte
**Inimigo:** Werewolf (Amaldiçoado, força sobre-humana, regeneração)
**Prazo:** 3 dias até próxima lua cheia
**Equipamento recomendado:** Armas de prata (fornecidas pelo Lorde)

'Salve minha família. Salve Silverwood.'",
                Difficulty = "Médio",
                RequiredLevel = 7,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 500,
                GoldReward = 700,
                CreatedAt = DateTime.UtcNow.AddDays(-21)
            },

            // ===== QUESTS DIFÍCEIS (Nível 10-15) =====
            new Quest
            {
                Name = "O Vampiro Ancestral de Crimson Vale",
                Description = @"🦇 **TERROR IMORTAL**

A **Inquisidora Celeste**, caçadora de vampiros há 30 anos, te mostra um medalhão ensanguentado. Seu rosto cicatrizado conta histórias de batalhas perdidas.

'Este medalhão pertencia ao meu mentor. Encontrei-o drenado de sangue em **Crimson Vale**. O trabalho de **Lord Vladimor**, um vampiro de 500 anos.'

Ela abre um tomo antigo, mostrando ilustrações de horrores noturnos.

'Vladimor não é um vampiro comum. É um **ANCIÃO**. Comandou exércitos de mortos-vivos nas Guerras Sangrentas. Foi selado por meu bisavô, mas o selo se quebrou.'

Seus olhos brilham com determinação e dor.

'Já perdi 12 caçadores tentando destruí-lo. Mas você... você tem algo especial. Leve esta estaca de freixo abençoado. Perfure seu coração antes do amanhecer, ou Crimson Vale se tornará um reino de trevas eternas.'

**Local:** Castelo Crimson, Vale das Sombras Eternas
**Inimigo:** Vampire Lord Vladimor (500 anos, imortal, mestre da manipulação)
**Habilidades:** Controle mental, transformação em morcego, regeneração, exército de lacaios
**ATENÇÃO:** Extremamente perigoso - party recomendada
**Recompensa:** Capa do Caçador de Vampiros (Lendária)

'Que a luz te proteja nas trevas absolutas.'",
                Difficulty = "Difícil",
                RequiredLevel = 10,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 800,
                GoldReward = 1500,
                CreatedAt = DateTime.UtcNow.AddDays(-18)
            },

            new Quest
            {
                Name = "A Torre do Lich King",
                Description = @"💀 **O SENHOR DOS MORTOS**

O **Arcebispo Theron**, líder espiritual de Valestra, te convoca à catedral. Luz sagrada emana de seu cajado enquanto ele aponta para o norte.

'Uma presença maligna se ergue. A **Torre Negra**, selada há mil anos, voltou a brilhar com luz verde doentia. O **LICH KING** desperta!'

Ele desenrola uma profecia antiga.

'Quando o sol se alinhar com a lua negra, o Lich despertará e marchará com legião de 10.000 mortos. TRÊS DIAS até o alinhamento!'

Sua voz ecoa pela catedral.

'Você deve ESCALAR a Torre Negra, atravessar os 7 níveis de horrores mortos-vivos, e destruir o phylactery do Lich antes que o ritual se complete. Ou Valestra CAIRÁ.'

**Local:** Torre Negra, Terras Desoladas
**Inimigo Final:** Lich King (Necromante supremo, milhares de anos)
**Desafios:** 7 andares, cada um com guardião único
**Prazo:** 3 dias até ritual de despertar completo
**Risco:** EXTREMO - Muitos entraram, nenhum retornou

'Esta pode ser a última batalha de Valestra. Faça valer a pena.'",
                Difficulty = "Difícil",
                RequiredLevel = 12,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 1000,
                GoldReward = 2000,
                CreatedAt = DateTime.UtcNow.AddDays(-16)
            },

            // ===== QUESTS ÉPICAS (Nível 15-20) =====
            new Quest
            {
                Name = "O Despertar do Dragão Ancião",
                Description = @"🐉 **A FÚRIA DO CÉU E FOGO**

**Rei Aldric**, monarca de Valestra, te recebe em seu trono. Toda a corte está em pânico. Fumaça é visível através das janelas do castelo.

'**SMAUG, O DEVASTADOR** despertou! O dragão ancião que dormia há 300 anos nas Montanhas Flamejantes ACORDOU!'

Ele desce do trono, desesperado.

'Três cidades JÁ FORAM QUEIMADAS. Dez mil mortos. O dragão marcha em direção à capital. Estimo... 2 dias até nos alcançar.'

Mapas mostram um rastro de destruição.

'Convoquei todos os heróis do reino. Preciso que você lidere o assalto final. Voar até a Montanha Flamejante e **ENFRENTAR SMAUG** em seu covil. É suicídio... mas é nossa ÚNICA chance.'

Ele te entrega uma espada brilhante.

'Esta é **Luzardo**, Matadora de Dragões, forjada na primeira era. Perfurou o coração de 3 dragões. Que perfure o quarto.'

**Local:** Pico da Montanha Flamejante, Covil do Dragão
**Inimigo:** Elder Dragon Smaug (300 anos, inteligência superior, poder destrutivo absoluto)
**Habilidades:** Sopro de fogo apocalíptico, voo, escamas impenetráveis, magia dracônica
**ATENÇÃO:** BOSS RAID - PARTY DE 3 HERÓIS OBRIGATÓRIA
**Recompensa:** Tesouro de Smaug (50.000 gold + itens lendários)

'Se falhar... Valestra cai. Sem pressão.'",
                Difficulty = "Épico",
                RequiredLevel = 15,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 2000,
                GoldReward = 5000,
                CreatedAt = DateTime.UtcNow.AddDays(-14)
            },

            new Quest
            {
                Name = "O Portal Demoníaco: O Senhor Demônio Desperta",
                Description = @"😈 **O FIM DOS TEMPOS**

O **Oráculo Ancestral**, guardião do Cristal do Destino, te convoca com urgência. O cristal RACHA enquanto visões apocalípticas se manifestam.

'VI O FIM! Um portal para o **ABISMO INFERNAL** foi aberto no **Deserto de Cinzas Eternas**! **BAEL, O SENHOR DEMÔNIO**, líder das legiões do inferno, está CRUZANDO!'

Visões mostram cidades em chamas, céus vermelhos sangue.

'Se Bael cruzar completamente, traz consigo **100.000 DEMÔNIOS**. Valestra, não... TODO O MUNDO cairá em escravidão eterna!'

A voz do Oráculo se torna um sussurro profético.

'Você deve entrar no portal, lutar através das legiões demoníacas, e **DESTRUIR BAEL** antes que ele cruze completamente. Mas cuidado... o Abismo corrompe. Muitos entraram como heróis. Voltaram como monstros.'

**Local:** Portal do Abismo, Deserto de Cinzas Eternas
**Inimigo:** Demon Lord Bael (Um dos 7 Senhores do Inferno, poder incompreensível)
**Ambiente:** O próprio Abismo - calor extremo, corrupção espiritual
**PERIGO MÁXIMO:** Pode não haver retorno
**Recompensa:** Salvação de toda Valestra + Título: Matador de Demônios

'Que os deuses tenham misericórdia de sua alma... porque Bael não terá.'",
                Difficulty = "Épico",
                RequiredLevel = 18,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 3000,
                GoldReward = 10000,
                CreatedAt = DateTime.UtcNow.AddDays(-12)
            },

            new Quest
            {
                Name = "Kraken: O Terror dos Mares Profundos",
                Description = @"🦑 **LENDA DAS PROFUNDEZAS**

**Almirante Blackwater**, comandante da Armada Real, te intercepta no porto. Sua mão esquerda foi substituída por um gancho. Ele está pálido de medo.

'Vi AQUILO com meus próprios olhos! O **KRAKEN**! A lenda é REAL! Aquela... COISA... destruiu METADE DA MINHA FROTA!'

Ele bate o gancho na mesa.

'Tentáculos do tamanho de navios! Olhos grandes como escudos! Mandíbulas que engolem galeras inteiras! Perdi 300 homens em 5 minutos!'

Lágrimas escorrem por seu rosto marcado pelo sal.

'Mas não é só vingança. O Kraken bloqueou a **Rota Comercial Marítima**. Sem ela, Valestra **MORRE DE FOME** em 30 dias. Você precisa... descer. Ir ao **Abismo Oceânico**. E matar essa ABOMINAÇÃO.'

**Local:** Abismo Oceânico, 5000 metros de profundidade
**Inimigo:** Kraken (Lenda viva, 500 anos, imbatível?)
**Equipamento especial:** Poção de Respiração Aquática (fornecida)
**Ambiente:** Combate subaquático, pressão extrema, escuridão total
**ATENÇÃO:** Muitos tentaram. Todos morreram. Você será diferente?

'Traga-me um tentáculo como prova. E... boa sorte. Você precisará.'",
                Difficulty = "Épico",
                RequiredLevel = 16,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 2500,
                GoldReward = 7500,
                CreatedAt = DateTime.UtcNow.AddDays(-13)
            },

            // ===== MISSÕES DE ESCOLTA (Nível 4-6) =====
            new Quest
            {
                Name = "A Caravana Perdida",
                Description = @"🛒 **MERCADORES EM PERIGO**

**Marcus, o Mercador**, gesticula desesperadamente ao te ver. Suor escorre por seu rosto enquanto ele desdobra um mapa rasgado.

'Minha caravana! Estava indo para Porto Âncora com tecidos raros quando fomos atacados por bandidos na **Passagem do Corvo Negro**!'

Ele aponta com dedos tremulantes no mapa.

'Consegui fugir, mas meus três guardas e todo meu estoque ficaram lá! Eles vão matar todos se não conseguirem o resgate em 24 HORAS!'

Lágrimas escorrem.

'Por favor... eu pago tudo que tenho. Só... traga-os de volta vivos. São como família para mim.'

**Objetivo:** Resgatar os 3 guardas e recuperar a mercadoria
**Local:** Passagem do Corvo Negro
**Inimigos:** Bandidos (15-20 bandidos armados)
**Tempo:** 24 horas
**Bônus:** Se ninguém morrer, recompensa duplicada",
                Difficulty = "Médio",
                RequiredLevel = 4,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 300,
                GoldReward = 400,
                CreatedAt = DateTime.UtcNow.AddDays(-26)
            },

            // ===== MISSÕES DE INVESTIGAÇÃO (Nível 6-8) =====
            new Quest
            {
                Name = "O Mistério do Cult da Lua Negra",
                Description = @"🔍 **DESAPARECIMENTOS MISTERIOSOS**

**Detetive Helena Blackwood**, investigadora real, te mostra um quadro com fotos e linhas vermelhas conectando pontos.

'Nos últimos 3 meses, **17 pessoas desapareceram** em Thornwood. Todas na lua nova. Todas deixaram este símbolo.'

Ela mostra um desenho: uma lua negra com três garras.

'Segui as pistas até um galpão abandonado nos **Docas Velhas**. Vi figuras encapuzadas entrando à meia-noite. Rituais. Cânticos. Algo... sobrenatural.'

Sua mão vai instintivamente para a arma.

'Preciso de alguém com... habilidades especiais. Infiltre-se no culto. Descubra o que estão planejando. E se possível... ACABE COM ISSO.'

**Objetivo:** 
• Infiltrar o Culto da Lua Negra
• Descobrir o plano deles
• Resgatar as vítimas
• Eliminar o líder do culto

**Local:** Docas Velhas → Templo Subterrâneo
**Perigo:** Cultistas (mágicos), possível demônio invocado
**Recompensa especial:** Distintivo de Investigador Real",
                Difficulty = "Médio",
                RequiredLevel = 6,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 550,
                GoldReward = 800,
                CreatedAt = DateTime.UtcNow.AddDays(-24)
            },

            new Quest
            {
                Name = "O Ladrão Fantasma de Porto Âncora",
                Description = @"👤 **O IMPOSSÍVEL SE TORNA REAL**

**Capitão da Guarda Sullivan** te recebe em seu escritório. Três cofres abertos e vazios estão alinhados na mesa.

'Em 7 dias, **7 roubos impossíveis**. Todos os cofres trancados com magia. Todas as guardas patrulhando. Ninguém viu NADA.'

Ele esfrega as têmporas.

'As vítimas? Os homens mais ricos da cidade. O último foi o **Barão Thornhill** - roubado enquanto dormia no mesmo quarto que o cofre. Acordou e o ouro tinha SUMIDO.'

Um mapa da cidade com marcações vermelhas.

'Contratei 20 guardas extras. Instalei alarmes mágicos. E ontem? Roubaram a PRÓPRIA SALA DO TESOURO DA GUILDA!'

Sua voz tremula de raiva e medo.

'Isso não é normal. É magia. Ou algo pior. Pegue esse fantasma antes que a cidade entre em pânico total.'

**Objetivo:** Investigar padrões, encontrar e capturar o ladrão
**Pistas:** Todos os roubos foram entre 2h e 3h da manhã
**Suspeitos:** Possível mago-ladrão ou criatura etérea
**Recompensa:** 20% do ouro recuperado",
                Difficulty = "Médio",
                RequiredLevel = 7,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 480,
                GoldReward = 650,
                CreatedAt = DateTime.UtcNow.AddDays(-22)
            },

            // ===== MISSÕES DE COLETA ÉPICA (Nível 8-10) =====
            new Quest
            {
                Name = "Os Três Artefatos Perdidos do Rei Antigo",
                Description = @"👑 **A HERANÇA DO REI LOUCO**

**Arquivista Eldrin**, guardião da biblioteca real, desenrola um pergaminho com 1000 anos.

'O Rei Malachar, o Louco, antes de sua morte, dividiu seu poder em **TRÊS ARTEFATOS**. Separou-os para que ninguém pudesse reuní-los.'

Ele aponta para ilustrações antigas.

'A **Coroa da Dominação** - escondida na Catacumba Real
A **Espada da Destruição** - guardada pelo Dragão de Gelo nas Montanhas Congeladas  
O **Anel da Imortalidade** - nas profundezas do Templo Submerso'

Sua expressão se torna grave.

'Recentemente, cultistas começaram a procurar os artefatos. Se REUNIREM os três, podem ressuscitar Malachar. E isso significaria... o FIM DE VALESTRA.'

Ele te entrega um mapa antigo.

'Encontre os três artefatos ANTES deles. E quando tiver todos... destrua-os. Para sempre.'

**Objetivos:**
• Recuperar a Coroa (Catacumba Real - Nível 8)
• Recuperar a Espada (Montanha Congelada - Nível 9)  
• Recuperar o Anel (Templo Submerso - Nível 10)
• Destruir os três na Forja Sagrada

**Perigo:** MÁXIMO - 3 dungeons épicas
**Duração:** Sem limite, mas cultistas também estão procurando",
                Difficulty = "Difícil",
                RequiredLevel = 8,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 1200,
                GoldReward = 2500,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },

            // ===== MISSÕES DE DEFESA (Nível 5-7) =====
            new Quest
            {
                Name = "A Noite das Hordas: Defesa de Vila Esperança",
                Description = @"🏰 **APOCALIPSE ZUMBI**

**Prefeita Elena**, de Vila Esperança, está em prantos. Você pode ver fumaça ao longe.

'O necromante que vocês DEIXARAM ESCAPAR na Torre Negra... ele voltou. E trouxe uma HORDA DE MORTOS-VIVOS!'

Ela aponta para a vila cercada.

'300 pessoas estão aqui. Crianças. Idosos. Temos muros fracos e apenas 12 guardas. A horda tem **MIL MORTOS-VIVOS** e chegará ao amanhecer.'

Lágrimas de desespero.

'Não podemos evacuar - eles bloqueariam a estrada. Não podemos negociar - eles não são humanos mais. Só podemos... LUTAR.'

Ela te entrega uma espada.

'Organize a defesa. Fortifique os muros. Proteja meu povo. Até o amanhecer. Se sobrevivermos... tudo que temos é seu.'

**Formato:** Defesa em Ondas
**Ondas:** 5 ondas de 200 mortos-vivos cada
**Objetivo:** Sobreviver até o amanhecer (5 ondas)
**Penalidade:** Cada civil morto = -50 Gold
**Bônus:** 0 civis mortos = +1000 Gold extra",
                Difficulty = "Médio",
                RequiredLevel = 5,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 700,
                GoldReward = 1000,
                CreatedAt = DateTime.UtcNow.AddDays(-19)
            },

            // ===== MISSÕES DE ARENA (Nível 10-12) =====
            new Quest
            {
                Name = "Campeão da Arena Sangrenta",
                Description = @"⚔️ **GLÓRIA OU MORTE**

**Mestre da Arena Brutus**, um gigante com cicatrizes por todo corpo, te analisa com olhos de predador.

'Então... quer entrar na ARENA? Ver se é páreo para os VERDADEIROS guerreiros de Valestra?'

Ele cospe no chão.

'A Arena Sangrenta não é para heróizinhos de conto de fadas. É SANGUE. É DOR. É MORTE. 87 entraram este ano. 12 saíram vivos.'

Um sorriso cruel.

'Mas se VENCER... 5 lutas. 5 oponentes cada vez mais mortais. Se sobreviver ao 5º combate... será CAMPEÃO. Fama eterna. Riquezas. E este.'

Ele mostra um cinto cravejado de joias.

'O Cinto do Campeão. Só 3 pessoas vivas o conquistaram. Quer ser o 4º? Ou mais um nome na lista de MORTOS?'

**Formato:** 5 Combates Progressivos
• Combate 1: Guerreiro Veterano (D6, Roll 4)
• Combate 2: Campeã Élfica (D10, Roll 5)
• Combate 3: Troll Blindado (D10, Roll 6)
• Combate 4: Dupla de Gladiadores (D12, Roll 8)
• Combate 5: Campeão Defensor - Gorath, o Imortal (D12, Roll 9)

**Regra:** NÃO PODE FUGIR. Vence ou morre.
**Prêmio:** Cinto do Campeão + 5000 Gold + Título",
                Difficulty = "Difícil",
                RequiredLevel = 10,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 900,
                GoldReward = 5000,
                CreatedAt = DateTime.UtcNow.AddDays(-17)
            },

            // ===== MISSÕES DE RESGATE ÉPICO (Nível 12-14) =====
            new Quest
            {
                Name = "Resgate na Torre de Ferro: A Princesa Sequestrada",
                Description = @"👸 **O REI IMPLORA**

**Rei Aldric**, com os olhos vermelhos de tanto chorar, segura tua mão com desespero.

'Minha filha... minha **Princesa Elara**... LEVADA por um dragão negro! Há 3 dias!'

Ele desmorona na frente do trono.

'O dragão **Nightwing** a mantém presa na **Torre de Ferro**, nas Montanhas Amaldiçoadas. Mandei 3 grupos de resgate. Nenhum voltou.'

Guardas seguram o rei que parece à beira da loucura.

'ELA É TUDO QUE ME RESTA! Minha esposa morreu no parto. Elara é... tudo. TUDO!'

Ele se levanta, com olhos de determinação demente.

'Traga-a de volta. VIVA. E te darei... METADE DO REINO. Castelos. Terras. Ouro. Tudo. Só... TRAGA MINHA FILHA DE VOLTA!'

**Objetivo:** Resgatar Princesa Elara da Torre de Ferro
**Inimigos:** 
• Guardas Kobold (D6)
• Wyverns Guardiãs (D10)
• Nightwing, o Dragão Negro (D20, Roll 17) - BOSS

**Complicação:** Princesa está sob feitiço de sono - precisa de beijo verdadeiro OU poção de despertar
**Recompensa:** Metade do tesouro real + Título de Duque/Duquesa",
                Difficulty = "Épico",
                RequiredLevel = 12,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 1500,
                GoldReward = 8000,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },

            // ===== MISSÕES DE CAÇA (Nível 3-5) =====
            new Quest
            {
                Name = "A Grande Caçada: O Javali de Ferro",
                Description = @"🐗 **LENDA VIVA**

**Caçador-Mor Wilhelm**, coberto de cicatrizes, mostra uma presa de javali do tamanho do teu braço.

'50 anos de caçada. Matei ursos, lobos, até um wyvern jovem. Mas HÁ UMA criatura que sempre escapou.'

Olhos brilham com obsessão.

'O **Javali de Ferro**. 800 quilos de músculo e fúria. Dizem que tem 100 anos. Pele dura como aço. Chifres que perfuram carvalhos.'

Ele mostra um mapa da Floresta Negra.

'Vive nas profundezas da Floresta Negra. Já matou 23 caçadores. Meu irmão foi um deles, há 10 anos.'

Voz se quebra.

'Estou velho. Minha última chance. Me ajude a caçar a besta. Morte ou glória. Uma última caçada ÉPICA.'

**Objetivo:** Rastrear e abater o Javali de Ferro
**Desafio:** Rastreamento (INT check), depois combate
**Inimigo:** Iron Boar (D10, Roll 6, Alta Defesa)
**Troféu:** Presas de Ferro (item lendário para crafting)",
                Difficulty = "Médio",
                RequiredLevel = 3,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 250,
                GoldReward = 350,
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            },

            new Quest
            {
                Name = "Extermínio: Ninho de Aranhas Gigantes",
                Description = @"🕷️ **PESADELO DE 8 PATAS**

**Fazendeiro Thomas**, com braço enfaixado sangrando, aponta para o celeiro.

'Começou com ovelhas desaparecendo. Depois galinhas. Ontem... meu FILHO quase foi arrastado!'

Ele quebra em soluços.

'Há um NINHO no celeiro. Aranhas do tamanho de cães. Teias por toda parte. Ouço algo GRANDE lá dentro... a mãe delas.'

Tremendo de medo.

'Não posso chamar a guarda - demoram dias. Minha família está trancada em casa. PRECISO DO CELEIRO LIMPO! É nossa colheita de inverno inteira!'

**Objetivo:** Exterminar ninho de aranhas
**Inimigos:** 
• 15-20 Aranhas Gigantes (D6, Roll 3)
• 1 Aranha Rainha (D10, Roll 6) - Veneno

**Perigo:** Teias reduzem movimento
**Bônus:** Sacos de ovos destruídos = +100 Gold cada",
                Difficulty = "Fácil",
                RequiredLevel = 2,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 180,
                GoldReward = 200,
                CreatedAt = DateTime.UtcNow.AddDays(-29)
            },

            // ===== MISSÕES CÔMIC RELIEF (Nível 1-3) =====
            new Quest
            {
                Name = "O Gato da Bruxa: Missão Impossível",
                Description = @"😺 **NUNCA SUBESTIME UM GATO**

**Bruxa Morgana**, uma senhora de 80 anos, te olha com olhos lacrimejantes.

'Meu Senhor Bigodes... meu gatinho... subiu na **ÁRVORE ANCESTRAL** há 3 dias e não desce!'

Ela aponta para uma árvore de 50 metros de altura.

'Já tentei magia, mas ele é imune (experimentei nele quando era bebê). Tentei comida, não desce. Tentei rezar, não funcionou!'

Lágrimas escorrem.

'Ele é TUDO que tenho! Sem ele... eu... eu...'

Ela se recompõe, secando as lágrimas.

'Me ajude a resgatar Senhor Bigodes e te darei uma poção da juventude que estou guardando há 30 anos.'

**Objetivo:** Subir na Árvore Ancestral e resgatar o gato
**Desafio:** Escalada (DEX check) + Convencer o gato (CHA check)
**Complicação:** O gato não quer descer. Tenta arranhar você.
**Recompensa:** Poção da Juventude (-10 anos de aparência)",
                Difficulty = "Fácil",
                RequiredLevel = 0,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 50,
                GoldReward = 100,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },

            new Quest
            {
                Name = "A Taverna do Caos: Pare a Briga!",
                Description = @"🍺 **DESTRUIÇÃO ALCOÓLICA**

**Taverneiro Brutus**, desesperado, te puxa para dentro da taverna.

Caos total. 30 bêbados brigando. Mesas quebradas. Barris rolando. Alguém está PENDURADO NO LUSTRE.

'COMEÇOU COM UMA DISCUSSÃO SOBRE QUAL DRAGÃO É MAIS FORTE! AGORA ESTÃO DESTRUINDO MEU BAR!'

Ele grita por cima do barulho.

'SE NÃO PARAR ESSA BRIGA, VOU TER PREJUÍZO DE 1000 GOLD! FAÇA ALGUMA COISA!'

Um bêbado voa pela janela.

**Objetivo:** Parar a briga na taverna
**Desafio:** Opções múltiplas:
• Intimidação (STR check) - Nocautear todo mundo
• Persuasão (CHA check) - Oferecer rodada grátis  
• Magia (INT check) - Feitiço de sono em massa
• Criatividade (sua escolha!)

**Bônus:** Menos dano à taverna = mais recompensa
**Punição:** Se taverna for destruída completamente = 0 gold",
                Difficulty = "Fácil",
                RequiredLevel = 2,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 100,
                GoldReward = 150,
                CreatedAt = DateTime.UtcNow.AddDays(-28)
            },

            // ===== MISSÕES DE CRAFTING (Nível 6-9) =====
            new Quest
            {
                Name = "O Ferreiro Mestre: Forja de uma Lâmina Lendária",
                Description = @"🔨 **OBRA-PRIMA FINAL**

**Mestre Ferreiro Gareth**, com 60 anos de experiência, mostra projetos antigos.

'Meu avô forjou espadas para reis. Meu pai para heróis. Eu... quero forjar UMA LÂMINA LENDÁRIA antes de morrer.'

Ele desenrola um pergaminho com designs complexos.

'**Starfall** - assim a chamarei. Mas preciso de materiais IMPOSSÍVEIS:

• Aço de Meteoro (do meteoro nas Planícies de Fogo)
• Coração de Elemental de Fogo (das Cavernas Vulcânicas)
• Água Benta da Fonte Sagrada (no Templo dos Antigos)
• Essência de Estrela Cadente (precisa coletar à meia-noite)'

Olhos brilham.

'Traga-me esses materiais e forjarei uma espada que será LENDA por mil anos. Uma espada digna de heróis.'

**Objetivos:**
• Coletar Aço de Meteoro
• Derrotar Elemental e pegar seu coração
• Conseguir Água Benta
• Capturar Essência de Estrela (evento raro)

**Recompensa:** Espada Lendária personalizada com seu nome",
                Difficulty = "Difícil",
                RequiredLevel = 9,
                RequiredClass = "Any",
                Type = "Main",
                ExperienceReward = 850,
                GoldReward = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-18)
            },

            // ===== MISSÕES DE HORROR (Nível 13-15) =====
            new Quest
            {
                Name = "O Manicômio Abandonado: Espíritos Torturados",
                Description = @"👻 **ONDE A LOUCURA REINA**

**Médium Elizabeth**, sensitiva espiritual, está pálida de terror.

'Sinto... tantas almas. MILHARES. Todas presas. Todas... SOFRENDO.'

Ela aponta para o Manicômio Ravenwood, abandonado há 50 anos.

'O Diretor era um sádico. Experimentava em pacientes. Torturas. Lobotomias. Horrores inimagináveis. Um dia... os pacientes se rebelaram. Mataram todos os médicos. E então... TRANCARAM AS PORTAS DE DENTRO.'

Ela treme.

'Encontraram 200 corpos meses depois. Pacientes que se entre-mataram. Agora... seus espíritos vagam. Presos entre mundos. Em AGONIA ETERNA.'

Lágrimas.

'Preciso que entre lá. Encontre os restos do Diretor. QUEIME seu diário maldito. É o que os prende. Liberte essas almas.'

**Local:** Manicômio Ravenwood (3 andares de horror)
**Perigos:**
• Espíritos Enfurecidos
• Armadilhas psicológicas
• Testes de sanidade
• Boss: Fantasma do Diretor (D12, Roll 10)

**Aviso:** Pode afetar sanidade mental do herói",
                Difficulty = "Difícil",
                RequiredLevel = 13,
                RequiredClass = "Any",
                Type = "Side",
                ExperienceReward = 1100,
                GoldReward = 1800,
                CreatedAt = DateTime.UtcNow.AddDays(-14)
            }
        };

        _context.Quests.AddRange(quests);
        await _context.SaveChangesAsync();
        _logger.LogInformation("🎯 {Count} quests criadas (incluindo quest tutorial)", quests.Count);
    }

    private async Task SeedRewardsAsync()
    {
        // Verifica se já existem rewards
        if (await _context.Rewards.AnyAsync())
        {
            _logger.LogInformation("💰 Rewards já existem - pulando seed");
            return;
        }

        var quests = await _context.Quests.ToListAsync();
        var items = await _context.Items.ToListAsync();
        var rewards = new List<Reward>();

        var questRewardMap = new Dictionary<string, int?>
        {
            { "Caça aos Goblins", items.FirstOrDefault(i => i.Name == "Espada de Ferro")?.Id },
            { "Coleta de Ervas", items.FirstOrDefault(i => i.Name == "Poção de Cura Menor")?.Id },
            { "Limpeza do Cemitério", items.FirstOrDefault(i => i.Name == "Armadura de Couro")?.Id },
            { "A Invasão Orc", items.FirstOrDefault(i => i.Name == "Cota de Malha")?.Id },
            { "O Feiticeiro das Sombras", items.FirstOrDefault(i => i.Name == "Cajado Ancestral")?.Id },
            { "Caçada ao Lobisomem", items.FirstOrDefault(i => i.Name == "Poção de Força")?.Id },
            { "O Covil do Troll", items.FirstOrDefault(i => i.Name == "Escudo de Madeira")?.Id },
            { "O Vampiro Ancestral", items.FirstOrDefault(i => i.Name == "Manto Mágico")?.Id },
            { "A Maldição do Templo", items.FirstOrDefault(i => i.Name == "Poção de Mana")?.Id },
            { "O Dragão das Montanhas", items.FirstOrDefault(i => i.Name == "Espada Flamejante")?.Id },
            { "O Despertar do Balrog", items.FirstOrDefault(i => i.Name == "Armadura de Placas")?.Id },
            { "O Senhor dos Liches", items.FirstOrDefault(i => i.Name == "Escudo de Mithril")?.Id },
            { "Kraken dos Mares", items.FirstOrDefault(i => i.Name == "Espada Élfica")?.Id }
        };

        foreach (var quest in quests)
        {
            questRewardMap.TryGetValue(quest.Name, out var itemId);
            
            rewards.Add(new Reward
            {
                QuestId = quest.Id,
                Gold = quest.GoldReward,
                Experience = quest.ExperienceReward,
                ItemId = itemId,
                ItemQuantity = itemId.HasValue ? 1 : 0,
                CreatedAt = quest.CreatedAt
            });
        }

        _context.Rewards.AddRange(rewards);
        await _context.SaveChangesAsync();
        _logger.LogInformation("💰 {Count} recompensas criadas", rewards.Count);
    }

    private async Task SeedQuestEnemiesAsync()
    {
        // Verifica se já existem relações quest-enemy
        if (await _context.QuestEnemies.AnyAsync())
        {
            _logger.LogInformation("⚔️ Quest-Enemies já existem - pulando seed");
            return;
        }

        var quests = await _context.Quests.ToListAsync();
        var enemies = await _context.Enemies.ToListAsync();
        var questEnemies = new List<QuestEnemy>();

        // Quest 1: Caça aos Goblins
        questEnemies.Add(new QuestEnemy { QuestId = quests[0].Id, EnemyId = enemies[0].Id, Quantity = 10 });
        
        // Quest 2: Limpeza do Cemitério
        questEnemies.Add(new QuestEnemy { QuestId = quests[2].Id, EnemyId = enemies[2].Id, Quantity = 15 });
        
        // Quest 3: A Invasão Orc
        questEnemies.Add(new QuestEnemy { QuestId = quests[3].Id, EnemyId = enemies[1].Id, Quantity = 5 });
        questEnemies.Add(new QuestEnemy { QuestId = quests[3].Id, EnemyId = enemies[5].Id, Quantity = 1 });
        
        // Quest 4: O Feiticeiro das Sombras
        questEnemies.Add(new QuestEnemy { QuestId = quests[4].Id, EnemyId = enemies[6].Id, Quantity = 1 });
        
        // Quest 5: Caçada ao Lobisomem
        questEnemies.Add(new QuestEnemy { QuestId = quests[5].Id, EnemyId = enemies[9].Id, Quantity = 1 });
        
        // Quest 6: O Covil do Troll
        questEnemies.Add(new QuestEnemy { QuestId = quests[6].Id, EnemyId = enemies[7].Id, Quantity = 1 });
        
        // Quest 7: O Vampiro Ancestral
        questEnemies.Add(new QuestEnemy { QuestId = quests[7].Id, EnemyId = enemies[8].Id, Quantity = 1 });
        
        // Quest 8: O Dragão das Montanhas
        questEnemies.Add(new QuestEnemy { QuestId = quests[9].Id, EnemyId = enemies[11].Id, Quantity = 1 });
        
        // Quest 9: O Despertar do Balrog
        questEnemies.Add(new QuestEnemy { QuestId = quests[10].Id, EnemyId = enemies[13].Id, Quantity = 1 });
        
        // Quest 10: O Senhor dos Liches
        questEnemies.Add(new QuestEnemy { QuestId = quests[11].Id, EnemyId = enemies[12].Id, Quantity = 1 });
        
        // Quest 11: Kraken dos Mares
        questEnemies.Add(new QuestEnemy { QuestId = quests[12].Id, EnemyId = enemies[14].Id, Quantity = 1 });

        _context.QuestEnemies.AddRange(questEnemies);
        await _context.SaveChangesAsync();
        _logger.LogInformation("⚔️ {Count} relacionamentos quest-inimigo criados", questEnemies.Count);
    }

    private async Task SeedHeroItemsAsync()
    {
        // Não cria itens para heróis - os players receberão itens ao completar quests
        _logger.LogInformation("📦 HeroItems não criados - players receberão itens das quests");
        await Task.CompletedTask;
    }

    private async Task SeedHeroItemsAsync_OLD()
    {
        var heroes = await _context.Heroes.ToListAsync();
        var items = await _context.Items.ToListAsync();
        var heroItems = new List<HeroItem>();

        // Aragorn (Guerreiro) - Equipado para combate
        heroItems.Add(new HeroItem { HeroId = heroes[0].Id, ItemId = items[2].Id, Quantity = 1, IsEquipped = true }); // Espada Flamejante
        heroItems.Add(new HeroItem { HeroId = heroes[0].Id, ItemId = items[7].Id, Quantity = 1, IsEquipped = true }); // Armadura de Placas
        heroItems.Add(new HeroItem { HeroId = heroes[0].Id, ItemId = items[11].Id, Quantity = 3, IsEquipped = false }); // Poções de Cura

        // Gandalf (Mago) - Equipado para magia
        heroItems.Add(new HeroItem { HeroId = heroes[1].Id, ItemId = items[3].Id, Quantity = 1, IsEquipped = true }); // Cajado Ancestral
        heroItems.Add(new HeroItem { HeroId = heroes[1].Id, ItemId = items[8].Id, Quantity = 1, IsEquipped = true }); // Manto Mágico
        heroItems.Add(new HeroItem { HeroId = heroes[1].Id, ItemId = items[13].Id, Quantity = 2, IsEquipped = false }); // Poção de Mana

        // Legolas (Arqueiro) - Equipado para precisão
        heroItems.Add(new HeroItem { HeroId = heroes[2].Id, ItemId = items[4].Id, Quantity = 1, IsEquipped = true }); // Arco Élfico
        heroItems.Add(new HeroItem { HeroId = heroes[2].Id, ItemId = items[5].Id, Quantity = 1, IsEquipped = true }); // Armadura de Couro
        heroItems.Add(new HeroItem { HeroId = heroes[2].Id, ItemId = items[14].Id, Quantity = 2, IsEquipped = false }); // Elixir de Agilidade

        // Adicionar alguns itens para outros heróis também
        for (int i = 3; i < Math.Min(8, heroes.Count); i++)
        {
            heroItems.Add(new HeroItem { HeroId = heroes[i].Id, ItemId = items[0].Id, Quantity = 1, IsEquipped = true });
            heroItems.Add(new HeroItem { HeroId = heroes[i].Id, ItemId = items[11].Id, Quantity = 1, IsEquipped = false });
        }

        _context.HeroItems.AddRange(heroItems);
        await _context.SaveChangesAsync();
        _logger.LogInformation("🎒 {Count} itens adicionados aos inventários", heroItems.Count);
    }

    private async Task SeedHeroQuestsAsync()
    {
        // Não cria quests para heróis - os players aceitarão suas próprias quests
        _logger.LogInformation("🎯 HeroQuests não criadas - players aceitarão suas próprias quests");
        await Task.CompletedTask;
    }

    private async Task SeedHeroQuestsAsync_OLD()
    {
        var heroes = await _context.Heroes.ToListAsync();
        var quests = await _context.Quests.ToListAsync();
        var heroQuests = new List<HeroQuest>();

        // Aragorn completou várias quests
        heroQuests.Add(new HeroQuest { HeroId = heroes[0].Id, QuestId = quests[0].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-20), CompletedAt = DateTime.UtcNow.AddDays(-20) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[0].Id, QuestId = quests[2].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-18), CompletedAt = DateTime.UtcNow.AddDays(-18) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[0].Id, QuestId = quests[3].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-15), CompletedAt = DateTime.UtcNow.AddDays(-15) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[0].Id, QuestId = quests[7].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-10), CompletedAt = DateTime.UtcNow.AddDays(-10) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[0].Id, QuestId = quests[10].Id, IsCompleted = false, StartedAt = DateTime.UtcNow.AddDays(-2) });

        // Gandalf completou quests épicas
        heroQuests.Add(new HeroQuest { HeroId = heroes[1].Id, QuestId = quests[4].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-16), CompletedAt = DateTime.UtcNow.AddDays(-16) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[1].Id, QuestId = quests[8].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-12), CompletedAt = DateTime.UtcNow.AddDays(-12) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[1].Id, QuestId = quests[10].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-8), CompletedAt = DateTime.UtcNow.AddDays(-8) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[1].Id, QuestId = quests[11].Id, IsCompleted = false, StartedAt = DateTime.UtcNow.AddDays(-1) });

        // Legolas
        heroQuests.Add(new HeroQuest { HeroId = heroes[2].Id, QuestId = quests[0].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-19), CompletedAt = DateTime.UtcNow.AddDays(-19) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[2].Id, QuestId = quests[5].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-14), CompletedAt = DateTime.UtcNow.AddDays(-14) });
        heroQuests.Add(new HeroQuest { HeroId = heroes[2].Id, QuestId = quests[9].Id, IsCompleted = false, StartedAt = DateTime.UtcNow.AddDays(-3) });

        // Outros heróis com quests em andamento
        for (int i = 3; i < Math.Min(7, heroes.Count); i++)
        {
            heroQuests.Add(new HeroQuest { HeroId = heroes[i].Id, QuestId = quests[0].Id, IsCompleted = true, StartedAt = DateTime.UtcNow.AddDays(-10), CompletedAt = DateTime.UtcNow.AddDays(-9) });
            heroQuests.Add(new HeroQuest { HeroId = heroes[i].Id, QuestId = quests[1].Id, IsCompleted = false, StartedAt = DateTime.UtcNow.AddDays(-5) });
        }

        _context.HeroQuests.AddRange(heroQuests);
        await _context.SaveChangesAsync();
        _logger.LogInformation("📜 {Count} quests atribuídas aos heróis", heroQuests.Count);
    }

    private async Task SeedBossDropTableAsync()
    {
        // Verifica se já existem boss drops
        if (await _context.BossDropTables.AnyAsync())
        {
            _logger.LogInformation("🎁 Boss drops já existem - pulando seed");
            return;
        }

        var enemies = await _context.Enemies.ToListAsync();
        var items = await _context.Items.ToListAsync();

        var bosses = enemies.Where(e => e.IsBoss).ToList();
        if (!bosses.Any())
        {
            _logger.LogWarning("Nenhum boss encontrado para configurar drops");
            return;
        }

        var dropTables = new List<BossDropTable>();

        // Itens comuns que todos os bosses podem dropar (60% chance)
        var commonItems = items.Where(i => i.Rarity == ItemRarity.Common).ToList();

        foreach (var boss in bosses)
        {
            // Cada boss dropa 2-3 itens comuns
            for (int i = 0; i < Math.Min(3, commonItems.Count); i++)
            {
                dropTables.Add(new BossDropTable
                {
                    EnemyId = boss.Id,
                    ItemId = commonItems[i].Id,
                    DropChance = 0.60m, // 60%
                    IsExclusive = false
                });
            }
        }

        // Demon Lord Drops
        var demonLord = bosses.FirstOrDefault(b => b.Name == "Demon Lord");
        if (demonLord != null)
        {
            var espadaFlamejante = items.FirstOrDefault(i => i.Name == "Espada Flamejante");
            var laminaDemoniaca = items.FirstOrDefault(i => i.Name == "Lâmina Demoníaca: Corruptor");
            
            if (espadaFlamejante != null)
                dropTables.Add(new BossDropTable { EnemyId = demonLord.Id, ItemId = espadaFlamejante.Id, DropChance = 0.30m, IsExclusive = true });
            if (laminaDemoniaca != null)
                dropTables.Add(new BossDropTable { EnemyId = demonLord.Id, ItemId = laminaDemoniaca.Id, DropChance = 0.10m, IsExclusive = true });
        }

        // Elder Dragon Drops
        var elderDragon = bosses.FirstOrDefault(b => b.Name == "Elder Dragon");
        if (elderDragon != null)
        {
            var escamaDragao = items.FirstOrDefault(i => i.Name == "Escama do Dragão Ancião");
            var coracaoDragao = items.FirstOrDefault(i => i.Name == "Coração do Dragão");
            
            if (escamaDragao != null)
                dropTables.Add(new BossDropTable { EnemyId = elderDragon.Id, ItemId = escamaDragao.Id, DropChance = 0.35m, IsExclusive = true });
            if (coracaoDragao != null)
                dropTables.Add(new BossDropTable { EnemyId = elderDragon.Id, ItemId = coracaoDragao.Id, DropChance = 0.05m, IsExclusive = true }); // 5% - MUITO RARO!
        }

        // Lich King Drops
        var lichKing = bosses.FirstOrDefault(b => b.Name == "Lich King");
        if (lichKing != null)
        {
            var cajadoLich = items.FirstOrDefault(i => i.Name == "Cajado do Lich");
            var coroaLich = items.FirstOrDefault(i => i.Name == "Coroa do Lich King");
            
            if (cajadoLich != null)
                dropTables.Add(new BossDropTable { EnemyId = lichKing.Id, ItemId = cajadoLich.Id, DropChance = 0.30m, IsExclusive = true });
            if (coroaLich != null)
                dropTables.Add(new BossDropTable { EnemyId = lichKing.Id, ItemId = coroaLich.Id, DropChance = 0.08m, IsExclusive = true });
        }

        // Balrog Drops
        var balrog = bosses.FirstOrDefault(b => b.Name == "Balrog");
        if (balrog != null)
        {
            var chicoteFlamejante = items.FirstOrDefault(i => i.Name == "Chicote Flamejante");
            var foiceBalrog = items.FirstOrDefault(i => i.Name == "Foice do Balrog");
            
            if (chicoteFlamejante != null)
                dropTables.Add(new BossDropTable { EnemyId = balrog.Id, ItemId = chicoteFlamejante.Id, DropChance = 0.25m, IsExclusive = true });
            if (foiceBalrog != null)
                dropTables.Add(new BossDropTable { EnemyId = balrog.Id, ItemId = foiceBalrog.Id, DropChance = 0.06m, IsExclusive = true });
        }

        // Kraken Drops
        var kraken = bosses.FirstOrDefault(b => b.Name == "Kraken");
        if (kraken != null)
        {
            var tentaculoKraken = items.FirstOrDefault(i => i.Name == "Tentáculo do Kraken");
            
            if (tentaculoKraken != null)
                dropTables.Add(new BossDropTable { EnemyId = kraken.Id, ItemId = tentaculoKraken.Id, DropChance = 0.12m, IsExclusive = true });
        }

        _context.BossDropTables.AddRange(dropTables);
        await _context.SaveChangesAsync();
        _logger.LogInformation("🎁 {Count} drops configurados para {BossCount} bosses", dropTables.Count, bosses.Count);
    }

    private async Task SeedDiceInventoriesAsync()
    {
        // Não cria inventários de dados - serão criados ao criar heróis
        _logger.LogInformation("🎲 DiceInventories não criados - serão criados ao criar heróis");
        await Task.CompletedTask;
    }
    
    private async Task SeedPartyComboWeaknessesAsync()
    {
        // ===== PARTY COMBOS =====
        _logger.LogInformation("Seeding Party Combos...");
        if (!_context.PartyCombos.Any())
        {
            var combos = new List<PartyCombo>
            {
                new PartyCombo
                {
                    Name = "Muralha Inabalável",
                    RequiredClass1 = "Guerreiro",
                    RequiredClass2 = "Paladino",
                    Description = "A combinação de força bruta e fé divina cria uma defesa impenetrável.",
                    Effect = "Reduz o roll necessário contra bosses pesados e tanques.",
                    Icon = "🛡️"
                },
                new PartyCombo
                {
                    Name = "Trovão Arcano",
                    RequiredClass1 = "Mago",
                    RequiredClass2 = "Paladino",
                    Description = "Magia arcana amplificada por poder divino.",
                    Effect = "Super efetivo contra mortos-vivos e demônios.",
                    Icon = "⚡"
                },
                new PartyCombo
                {
                    Name = "Caçadores Silenciosos",
                    RequiredClass1 = "Arqueiro",
                    RequiredClass2 = "Assassino",
                    Description = "Precisão letal combinada com furtividade mortal.",
                    Effect = "Extremamente efetivo contra dragões e bestas voadoras.",
                    Icon = "🎯"
                },
                new PartyCombo
                {
                    Name = "Trindade Sagrada",
                    RequiredClass1 = "Guerreiro",
                    RequiredClass2 = "Mago",
                    RequiredClass3 = "Paladino",
                    Description = "A combinação perfeita: força, magia e fé.",
                    Effect = "Bônus contra todos os tipos de bosses.",
                    Icon = "✨"
                },
                new PartyCombo
                {
                    Name = "Sombra e Aço",
                    RequiredClass1 = "Guerreiro",
                    RequiredClass2 = "Assassino",
                    Description = "Força frontal distraindo enquanto o assassino ataca pelas costas.",
                    Effect = "Efetivo contra bosses humanoides e cavaleiros.",
                    Icon = "⚔️"
                },
                new PartyCombo
                {
                    Name = "Destruição Elemental",
                    RequiredClass1 = "Mago",
                    RequiredClass2 = "Arqueiro",
                    Description = "Flechas imbuídas com poder mágico devastador.",
                    Effect = "Extremamente efetivo contra golens e construtos.",
                    Icon = "🔥"
                }
            };

            await _context.PartyCombos.AddRangeAsync(combos);
            await _context.SaveChangesAsync();
            _logger.LogInformation("✅ {Count} Party Combos seeded!", combos.Count);
        }

        // ===== BOSS WEAKNESSES =====
        _logger.LogInformation("Seeding Boss Weaknesses...");
        if (!_context.BossWeaknesses.Any())
        {
            var combos = await _context.PartyCombos.ToListAsync();
            var enemies = await _context.Enemies.Where(e => e.IsBoss).ToListAsync();

            var muralhaCombo = combos.FirstOrDefault(c => c.Name == "Muralha Inabalável");
            var trovaoCombo = combos.FirstOrDefault(c => c.Name == "Trovão Arcano");
            var cacadoresCombo = combos.FirstOrDefault(c => c.Name == "Caçadores Silenciosos");
            var trindadeCombo = combos.FirstOrDefault(c => c.Name == "Trindade Sagrada");
            var sombraCombo = combos.FirstOrDefault(c => c.Name == "Sombra e Aço");

            var weaknesses = new List<BossWeakness>();

            // Dragão Ancião
            var dragao = enemies.FirstOrDefault(e => e.Name == "Dragão Ancião");
            if (dragao != null && cacadoresCombo != null && trindadeCombo != null)
            {
                weaknesses.Add(new BossWeakness
                {
                    EnemyId = dragao.Id,
                    ComboId = cacadoresCombo.Id,
                    RollReduction = -3,
                    DropMultiplier = 1.25m,
                    ExpMultiplier = 1.15m,
                    FlavorText = "Arqueiros e Assassinos exploram os pontos fracos nas escamas do dragão!"
                });
                weaknesses.Add(new BossWeakness
                {
                    EnemyId = dragao.Id,
                    ComboId = trindadeCombo.Id,
                    RollReduction = -2,
                    DropMultiplier = 1.10m,
                    ExpMultiplier = 1.10m,
                    FlavorText = "A Trindade Sagrada une forças contra o terror alado!"
                });
            }

            // Senhor Demônio
            var demonio = enemies.FirstOrDefault(e => e.Name == "Senhor Demônio");
            if (demonio != null && trovaoCombo != null && trindadeCombo != null)
            {
                weaknesses.Add(new BossWeakness
                {
                    EnemyId = demonio.Id,
                    ComboId = trovaoCombo.Id,
                    RollReduction = -4,
                    DropMultiplier = 1.30m,
                    ExpMultiplier = 1.20m,
                    FlavorText = "Magia arcana e poder divino devastam as forças demoníacas!"
                });
                weaknesses.Add(new BossWeakness
                {
                    EnemyId = demonio.Id,
                    ComboId = trindadeCombo.Id,
                    RollReduction = -2,
                    DropMultiplier = 1.15m,
                    ExpMultiplier = 1.10m,
                    FlavorText = "A luz da Trindade repele as trevas infernais!"
                });
            }

            // Cavaleiro das Trevas
            var cavaleiro = enemies.FirstOrDefault(e => e.Name == "Cavaleiro das Trevas");
            if (cavaleiro != null && sombraCombo != null && muralhaCombo != null)
            {
                weaknesses.Add(new BossWeakness
                {
                    EnemyId = cavaleiro.Id,
                    ComboId = sombraCombo.Id,
                    RollReduction = -3,
                    DropMultiplier = 1.20m,
                    ExpMultiplier = 1.15m,
                    FlavorText = "Força e furtividade quebram a guarda do cavaleiro sombrio!"
                });
                weaknesses.Add(new BossWeakness
                {
                    EnemyId = cavaleiro.Id,
                    ComboId = muralhaCombo.Id,
                    RollReduction = -2,
                    DropMultiplier = 1.10m,
                    ExpMultiplier = 1.05m,
                    FlavorText = "A Muralha Inabalável resiste aos ataques das trevas!"
                });
            }

            if (weaknesses.Any())
            {
                await _context.BossWeaknesses.AddRangeAsync(weaknesses);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ {Count} Boss Weaknesses seeded!", weaknesses.Count);
            }
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}


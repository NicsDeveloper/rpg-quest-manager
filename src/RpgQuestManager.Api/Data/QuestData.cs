using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Data;

public static class QuestData
{
    public static List<Quest> GetAllQuests()
    {
        return new List<Quest>
        {
            // MISSÕES PRINCIPAIS (6 missões)
            new Quest
            {
                Title = "Os Primeiros Passos",
                Description = "Derrote um Goblin nas bordas da Floresta para provar sua coragem.",
                IntroductionText = "Bem-vindo, jovem aventureiro! Sua jornada começa aqui, nas bordas da Floresta Sombria. Um goblin solitário está causando problemas para os aldeões. Derrote-o para provar que você tem o que é preciso para se tornar um verdadeiro herói.",
                CompletionText = "Parabéns! Você derrotou seu primeiro inimigo. Os aldeões estão impressionados com sua coragem. Sua jornada como herói apenas começou...",
                Category = QuestCategory.MainStory,
                Difficulty = QuestDifficulty.Easy,
                Environment = EnvironmentType.Forest,
                TargetMonsterName = "Goblin Guerreiro",
                TargetMonsterType = MonsterType.Goblin,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 100,
                GoldReward = 50,
                RequiredLevel = 1,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Espada de Ferro", "Poção de Cura" },
                IsRepeatable = false,
                EstimatedDuration = 15
            },

            new Quest
            {
                Title = "A Floresta Sombria",
                Description = "Explore a Floresta Sombria e derrote os goblins que a infestam.",
                IntroductionText = "A Floresta Sombria está repleta de goblins perigosos. Os aldeões não conseguem mais coletar madeira e frutas. Você deve limpar a floresta e derrotar o líder dos goblins para restaurar a paz.",
                CompletionText = "Excelente! A Floresta Sombria está segura novamente. Os aldeões podem retornar às suas atividades. Você provou ser um herói digno de confiança.",
                Category = QuestCategory.MainStory,
                Difficulty = QuestDifficulty.Easy,
                Environment = EnvironmentType.Forest,
                TargetMonsterName = "Gorak, Rei Goblin",
                TargetMonsterType = MonsterType.Goblin,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 300,
                GoldReward = 150,
                RequiredLevel = 3,
                Prerequisites = new List<string> { "Os Primeiros Passos" },
                Rewards = new List<string> { "Armadura de Couro", "Poção de Força" },
                IsRepeatable = false,
                EstimatedDuration = 30
            },

            new Quest
            {
                Title = "A Cripta Maldita",
                Description = "Explore a Cripta Maldita e enfrente os mortos-vivos que a habitam.",
                IntroductionText = "Uma antiga cripta foi descoberta nas montanhas. Os mortos-vivos que a habitam estão se espalhando pela região. Você deve entrar na cripta e derrotar o Lich que os controla.",
                CompletionText = "Incrível! Você purificou a Cripta Maldita e derrotou o Lich. Os mortos-vivos foram libertados de sua maldição. A região está segura novamente.",
                Category = QuestCategory.MainStory,
                Difficulty = QuestDifficulty.Medium,
                Environment = EnvironmentType.Crypt,
                TargetMonsterName = "Lich",
                TargetMonsterType = MonsterType.Undead,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 500,
                GoldReward = 250,
                RequiredLevel = 5,
                Prerequisites = new List<string> { "A Floresta Sombria" },
                Rewards = new List<string> { "Espada Mágica", "Amuleto de Proteção" },
                IsRepeatable = false,
                EstimatedDuration = 45
            },

            new Quest
            {
                Title = "O Orc Berserker",
                Description = "Enfrente o temível Orc Berserker que está aterrorizando a região.",
                IntroductionText = "Um poderoso Orc Berserker lidera uma horda de orcs que está devastando as aldeias. Ele é conhecido por sua fúria incontrolável e força sobre-humana. Você deve derrotá-lo para salvar a região.",
                CompletionText = "Fantástico! Você derrotou o Orc Berserker e dispersou sua horda. As aldeias estão seguras e os orcs recuaram para as montanhas. Você é um verdadeiro herói!",
                Category = QuestCategory.MainStory,
                Difficulty = QuestDifficulty.Medium,
                Environment = EnvironmentType.Forest,
                TargetMonsterName = "Grommash, Chefe Orc",
                TargetMonsterType = MonsterType.Orc,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 700,
                GoldReward = 350,
                RequiredLevel = 7,
                Prerequisites = new List<string> { "A Cripta Maldita" },
                Rewards = new List<string> { "Machado de Guerra", "Armadura de Aço" },
                IsRepeatable = false,
                EstimatedDuration = 60
            },

            new Quest
            {
                Title = "O Dragão Jovem",
                Description = "Enfrente um Dragão Jovem no Vulcão para provar sua coragem.",
                IntroductionText = "Um Dragão Jovem se estabeleceu no Vulcão Ativo. Ele está causando terremotos e erupções que ameaçam toda a região. Você deve escalar o vulcão e derrotar o dragão para salvar as cidades próximas.",
                CompletionText = "Inacreditável! Você derrotou um dragão! Sua coragem e habilidade são lendárias. O vulcão está calmo novamente e as cidades estão seguras. Você é um herói verdadeiramente épico!",
                Category = QuestCategory.MainStory,
                Difficulty = QuestDifficulty.Hard,
                Environment = EnvironmentType.Volcano,
                TargetMonsterName = "Dragão Jovem",
                TargetMonsterType = MonsterType.Dragon,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 1000,
                GoldReward = 500,
                RequiredLevel = 10,
                Prerequisites = new List<string> { "O Orc Berserker" },
                Rewards = new List<string> { "Espada de Dragão", "Escama de Dragão", "Poção de Fogo" },
                IsRepeatable = false,
                EstimatedDuration = 90
            },

            new Quest
            {
                Title = "O Lich Senhor das Trevas",
                Description = "O confronto final contra o Lich Senhor das Trevas.",
                IntroductionText = "O Lich Senhor das Trevas é o maior perigo que o mundo já enfrentou. Ele planeja transformar todos os vivos em mortos-vivos e governar o mundo das trevas. Esta é sua missão mais perigosa e importante. O destino do mundo está em suas mãos!",
                CompletionText = "LENDÁRIO! Você derrotou o Lich Senhor das Trevas e salvou o mundo! Sua coragem, habilidade e determinação são incomparáveis. Você é o maior herói que o mundo já conheceu. Sua lenda será contada por gerações!",
                Category = QuestCategory.MainStory,
                Difficulty = QuestDifficulty.Epic,
                Environment = EnvironmentType.Crypt,
                TargetMonsterName = "Lich Senhor das Sombras",
                TargetMonsterType = MonsterType.Undead,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 2000,
                GoldReward = 1000,
                RequiredLevel = 15,
                Prerequisites = new List<string> { "O Dragão Jovem" },
                Rewards = new List<string> { "Espada Lendária", "Armadura Épica", "Amuleto do Poder", "Coroa do Herói" },
                IsRepeatable = false,
                EstimatedDuration = 120
            },

            // MISSÕES DE BOSS (5 missões)
            new Quest
            {
                Title = "Batalha Épica: Dragão Ancião",
                Description = "Enfrente o Dragão Ancião, uma criatura milenar e sábia.",
                IntroductionText = "O Dragão Ancião é uma criatura milenar que guarda segredos antigos. Ele é conhecido por sua sabedoria e poder incomparáveis. Esta batalha testará todos os seus limites.",
                CompletionText = "Extraordinário! Você derrotou o Dragão Ancião! Sua sabedoria e poder agora são seus. Você é digno de ser chamado de Dragão Slayer!",
                Category = QuestCategory.BossFight,
                Difficulty = QuestDifficulty.Epic,
                Environment = EnvironmentType.Volcano,
                TargetMonsterName = "Dragão Ancião",
                TargetMonsterType = MonsterType.Dragon,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 1500,
                GoldReward = 750,
                RequiredLevel = 18,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Espada do Dragão Ancião", "Sabedoria Milenar" },
                IsRepeatable = true,
                EstimatedDuration = 100
            },

            new Quest
            {
                Title = "Batalha Épica: Demônio dos Pântanos",
                Description = "Enfrente o Demônio dos Pântanos, uma criatura corrompida.",
                IntroductionText = "O Demônio dos Pântanos corrompeu toda a região com sua presença maligna. Ele é uma criatura antiga e poderosa que deve ser derrotada para purificar a terra.",
                CompletionText = "Maravilhoso! Você purificou os pântanos e derrotou o demônio! A terra está limpa novamente e a vida pode florescer.",
                Category = QuestCategory.BossFight,
                Difficulty = QuestDifficulty.Hard,
                Environment = EnvironmentType.Swamp,
                TargetMonsterName = "Demônio dos Pântanos",
                TargetMonsterType = MonsterType.Demon,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 800,
                GoldReward = 400,
                RequiredLevel = 12,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Espada Purificadora", "Amuleto da Pureza" },
                IsRepeatable = true,
                EstimatedDuration = 75
            },

            new Quest
            {
                Title = "Batalha Épica: Troll Rei",
                Description = "Enfrente o Troll Rei, o líder supremo dos trolls.",
                IntroductionText = "O Troll Rei é o mais poderoso de todos os trolls. Ele lidera uma horda de trolls que está devastando as montanhas. Você deve derrotá-lo para restaurar a paz.",
                CompletionText = "Impressionante! Você derrotou o Troll Rei e dispersou sua horda! As montanhas estão seguras novamente.",
                Category = QuestCategory.BossFight,
                Difficulty = QuestDifficulty.Hard,
                Environment = EnvironmentType.Ruins,
                TargetMonsterName = "Troll Rei",
                TargetMonsterType = MonsterType.Troll,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 900,
                GoldReward = 450,
                RequiredLevel = 13,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Machado do Troll Rei", "Armadura de Pedra" },
                IsRepeatable = true,
                EstimatedDuration = 80
            },

            new Quest
            {
                Title = "Batalha Épica: Vampiro",
                Description = "Enfrente o Vampiro, um morto-vivo que se alimenta de sangue.",
                IntroductionText = "Um poderoso Vampiro se estabeleceu em um castelo abandonado. Ele está transformando os aldeões em vampiros. Você deve derrotá-lo para salvar a região.",
                CompletionText = "Fantástico! Você derrotou o Vampiro e libertou os aldeões de sua maldição! O castelo está purificado.",
                Category = QuestCategory.BossFight,
                Difficulty = QuestDifficulty.Hard,
                Environment = EnvironmentType.Castle,
                TargetMonsterName = "Vampiro",
                TargetMonsterType = MonsterType.Undead,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 850,
                GoldReward = 425,
                RequiredLevel = 12,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Espada Vampírica", "Amuleto da Luz" },
                IsRepeatable = true,
                EstimatedDuration = 70
            },

            new Quest
            {
                Title = "Batalha Épica: Elemental Primordial",
                Description = "Enfrente o Elemental Primordial, a força elementar mais poderosa.",
                IntroductionText = "O Elemental Primordial é a manifestação pura dos elementos. Ele é uma criatura antiga e poderosa que deve ser derrotada para restaurar o equilíbrio elemental.",
                CompletionText = "Incrível! Você derrotou o Elemental Primordial e restaurou o equilíbrio elemental! O mundo está em harmonia novamente.",
                Category = QuestCategory.BossFight,
                Difficulty = QuestDifficulty.Epic,
                Environment = EnvironmentType.Volcano,
                TargetMonsterName = "Elemental Primordial",
                TargetMonsterType = MonsterType.Elemental,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 1200,
                GoldReward = 600,
                RequiredLevel = 16,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Espada Elemental", "Amuleto do Equilíbrio" },
                IsRepeatable = true,
                EstimatedDuration = 90
            },

            // MISSÕES DE DUNGEON (5 missões)
            new Quest
            {
                Title = "Exploração: Masmorra dos Goblins",
                Description = "Explore a Masmorra dos Goblins e derrote todos os inimigos.",
                IntroductionText = "Uma antiga masmorra foi descoberta. Ela está repleta de goblins e tesouros. Explore a masmorra e derrote todos os inimigos para reclamar os tesouros.",
                CompletionText = "Excelente! Você explorou a masmorra e derrotou todos os goblins! Os tesouros são seus.",
                Category = QuestCategory.Dungeon,
                Difficulty = QuestDifficulty.Easy,
                Environment = EnvironmentType.Dungeon,
                TargetMonsterName = "Goblin Capitão",
                TargetMonsterType = MonsterType.Goblin,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 400,
                GoldReward = 200,
                RequiredLevel = 4,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Tesouro dos Goblins", "Chave da Masmorra" },
                IsRepeatable = true,
                EstimatedDuration = 45
            },

            new Quest
            {
                Title = "Exploração: Cripta dos Mortos",
                Description = "Explore a Cripta dos Mortos e enfrente os mortos-vivos.",
                IntroductionText = "Uma cripta antiga está repleta de mortos-vivos. Explore a cripta e derrote todos os inimigos para reclamar os tesouros antigos.",
                CompletionText = "Maravilhoso! Você explorou a cripta e derrotou todos os mortos-vivos! Os tesouros antigos são seus.",
                Category = QuestCategory.Dungeon,
                Difficulty = QuestDifficulty.Medium,
                Environment = EnvironmentType.Crypt,
                TargetMonsterName = "Wight",
                TargetMonsterType = MonsterType.Undead,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 600,
                GoldReward = 300,
                RequiredLevel = 6,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Tesouro Antigo", "Chave da Cripta" },
                IsRepeatable = true,
                EstimatedDuration = 60
            },

            new Quest
            {
                Title = "Exploração: Castelo Abandonado",
                Description = "Explore o Castelo Abandonado e enfrente os demônios.",
                IntroductionText = "Um castelo abandonado está repleto de demônios. Explore o castelo e derrote todos os inimigos para reclamar os tesouros do castelo.",
                CompletionText = "Fantástico! Você explorou o castelo e derrotou todos os demônios! Os tesouros do castelo são seus.",
                Category = QuestCategory.Dungeon,
                Difficulty = QuestDifficulty.Medium,
                Environment = EnvironmentType.Castle,
                TargetMonsterName = "Succubus",
                TargetMonsterType = MonsterType.Demon,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 700,
                GoldReward = 350,
                RequiredLevel = 8,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Tesouro do Castelo", "Chave do Castelo" },
                IsRepeatable = true,
                EstimatedDuration = 70
            },

            new Quest
            {
                Title = "Exploração: Ruínas Antigas",
                Description = "Explore as Ruínas Antigas e enfrente os trolls.",
                IntroductionText = "Ruínas antigas estão repleta de trolls. Explore as ruínas e derrote todos os inimigos para reclamar os tesouros das ruínas.",
                CompletionText = "Impressionante! Você explorou as ruínas e derrotou todos os trolls! Os tesouros das ruínas são seus.",
                Category = QuestCategory.Dungeon,
                Difficulty = QuestDifficulty.Hard,
                Environment = EnvironmentType.Ruins,
                TargetMonsterName = "Troll Xamã",
                TargetMonsterType = MonsterType.Troll,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 800,
                GoldReward = 400,
                RequiredLevel = 10,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Tesouro das Ruínas", "Chave das Ruínas" },
                IsRepeatable = true,
                EstimatedDuration = 80
            },

            new Quest
            {
                Title = "Exploração: Vulcão Ativo",
                Description = "Explore o Vulcão Ativo e enfrente os elementais.",
                IntroductionText = "Um vulcão ativo está repleto de elementais. Explore o vulcão e derrote todos os inimigos para reclamar os tesouros do vulcão.",
                CompletionText = "Incrível! Você explorou o vulcão e derrotou todos os elementais! Os tesouros do vulcão são seus.",
                Category = QuestCategory.Dungeon,
                Difficulty = QuestDifficulty.Hard,
                Environment = EnvironmentType.Volcano,
                TargetMonsterName = "Elemental do Fogo",
                TargetMonsterType = MonsterType.Elemental,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 900,
                GoldReward = 450,
                RequiredLevel = 11,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Tesouro do Vulcão", "Chave do Vulcão" },
                IsRepeatable = true,
                EstimatedDuration = 85
            },

            // MISSÕES ESPECIAIS (5 missões)
            new Quest
            {
                Title = "Evento Especial: Invasão Goblin",
                Description = "Defenda a vila contra uma invasão de goblins.",
                IntroductionText = "Uma horda de goblins está invadindo a vila! Você deve defender a vila e derrotar todos os goblins para salvar os aldeões.",
                CompletionText = "Heroico! Você defendeu a vila e derrotou todos os goblins! Os aldeões estão seguros.",
                Category = QuestCategory.SpecialEvent,
                Difficulty = QuestDifficulty.Easy,
                Environment = EnvironmentType.Forest,
                TargetMonsterName = "Goblin Guerreiro",
                TargetMonsterType = MonsterType.Goblin,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 300,
                GoldReward = 150,
                RequiredLevel = 2,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Medalha de Herói", "Gratidão dos Aldeões" },
                IsRepeatable = true,
                EstimatedDuration = 30
            },

            new Quest
            {
                Title = "Evento Especial: Eclipse Sombrio",
                Description = "Enfrente os mortos-vivos durante o Eclipse Sombrio.",
                IntroductionText = "Durante o Eclipse Sombrio, os mortos-vivos se tornam mais poderosos. Você deve derrotar os mortos-vivos para restaurar a luz.",
                CompletionText = "Extraordinário! Você derrotou os mortos-vivos durante o Eclipse Sombrio! A luz foi restaurada.",
                Category = QuestCategory.SpecialEvent,
                Difficulty = QuestDifficulty.Medium,
                Environment = EnvironmentType.Crypt,
                TargetMonsterName = "Fantasma",
                TargetMonsterType = MonsterType.Undead,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 500,
                GoldReward = 250,
                RequiredLevel = 5,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Amuleto da Luz", "Bênção Solar" },
                IsRepeatable = true,
                EstimatedDuration = 40
            },

            new Quest
            {
                Title = "Evento Especial: Tempestade Elemental",
                Description = "Enfrente os elementais durante a Tempestade Elemental.",
                IntroductionText = "Uma Tempestade Elemental está causando caos. Você deve derrotar os elementais para acalmar a tempestade.",
                CompletionText = "Maravilhoso! Você derrotou os elementais e acalmou a tempestade! O equilíbrio foi restaurado.",
                Category = QuestCategory.SpecialEvent,
                Difficulty = QuestDifficulty.Medium,
                Environment = EnvironmentType.Volcano,
                TargetMonsterName = "Elemental do Ar",
                TargetMonsterType = MonsterType.Elemental,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 600,
                GoldReward = 300,
                RequiredLevel = 6,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Amuleto do Equilíbrio", "Bênção Elemental" },
                IsRepeatable = true,
                EstimatedDuration = 45
            },

            new Quest
            {
                Title = "Evento Especial: Noite dos Demônios",
                Description = "Enfrente os demônios durante a Noite dos Demônios.",
                IntroductionText = "Durante a Noite dos Demônios, os demônios se tornam mais poderosos. Você deve derrotar os demônios para restaurar a paz.",
                CompletionText = "Fantástico! Você derrotou os demônios durante a Noite dos Demônios! A paz foi restaurada.",
                Category = QuestCategory.SpecialEvent,
                Difficulty = QuestDifficulty.Hard,
                Environment = EnvironmentType.Volcano,
                TargetMonsterName = "Imp",
                TargetMonsterType = MonsterType.Demon,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 700,
                GoldReward = 350,
                RequiredLevel = 8,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Amuleto da Pureza", "Bênção Divina" },
                IsRepeatable = true,
                EstimatedDuration = 50
            },

            new Quest
            {
                Title = "Evento Especial: Aurora Boreal",
                Description = "Enfrente os trolls durante a Aurora Boreal.",
                IntroductionText = "Durante a Aurora Boreal, os trolls se tornam mais poderosos. Você deve derrotar os trolls para aproveitar a magia da aurora.",
                CompletionText = "Impressionante! Você derrotou os trolls durante a Aurora Boreal! A magia da aurora é sua.",
                Category = QuestCategory.SpecialEvent,
                Difficulty = QuestDifficulty.Hard,
                Environment = EnvironmentType.Tundra,
                TargetMonsterName = "Troll de Gelo",
                TargetMonsterType = MonsterType.Troll,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 800,
                GoldReward = 400,
                RequiredLevel = 9,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Amuleto do Gelo", "Bênção da Aurora" },
                IsRepeatable = true,
                EstimatedDuration = 55
            },

            // MISSÕES DIÁRIAS (5 missões)
            new Quest
            {
                Title = "Missão Diária: Caça aos Goblins",
                Description = "Derrote 5 goblins para ganhar experiência diária.",
                IntroductionText = "Os goblins estão causando problemas novamente. Derrote 5 goblins para ganhar experiência e ouro.",
                CompletionText = "Bom trabalho! Você derrotou 5 goblins e ganhou experiência valiosa.",
                Category = QuestCategory.Daily,
                Difficulty = QuestDifficulty.Easy,
                Environment = EnvironmentType.Forest,
                TargetMonsterName = "Goblin Guerreiro",
                TargetMonsterType = MonsterType.Goblin,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 200,
                GoldReward = 100,
                RequiredLevel = 1,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Experiência Diária", "Ouro Diário" },
                IsRepeatable = true,
                EstimatedDuration = 20
            },

            new Quest
            {
                Title = "Missão Diária: Purificação",
                Description = "Derrote 3 mortos-vivos para purificar a região.",
                IntroductionText = "Os mortos-vivos estão se espalhando. Derrote 3 mortos-vivos para purificar a região.",
                CompletionText = "Excelente! Você purificou a região derrotando 3 mortos-vivos.",
                Category = QuestCategory.Daily,
                Difficulty = QuestDifficulty.Easy,
                Environment = EnvironmentType.Crypt,
                TargetMonsterName = "Esqueleto",
                TargetMonsterType = MonsterType.Undead,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 250,
                GoldReward = 125,
                RequiredLevel = 2,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Experiência Diária", "Ouro Diário" },
                IsRepeatable = true,
                EstimatedDuration = 25
            },

            new Quest
            {
                Title = "Missão Diária: Caça aos Orcs",
                Description = "Derrote 3 orcs para proteger a região.",
                IntroductionText = "Os orcs estão atacando as caravanas. Derrote 3 orcs para proteger a região.",
                CompletionText = "Ótimo! Você protegeu a região derrotando 3 orcs.",
                Category = QuestCategory.Daily,
                Difficulty = QuestDifficulty.Medium,
                Environment = EnvironmentType.Forest,
                TargetMonsterName = "Orc Guerreiro",
                TargetMonsterType = MonsterType.Orc,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 300,
                GoldReward = 150,
                RequiredLevel = 3,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Experiência Diária", "Ouro Diário" },
                IsRepeatable = true,
                EstimatedDuration = 30
            },

            new Quest
            {
                Title = "Missão Diária: Caça aos Demônios",
                Description = "Derrote 2 demônios para purificar a região.",
                IntroductionText = "Os demônios estão corrompendo a região. Derrote 2 demônios para purificar a região.",
                CompletionText = "Fantástico! Você purificou a região derrotando 2 demônios.",
                Category = QuestCategory.Daily,
                Difficulty = QuestDifficulty.Medium,
                Environment = EnvironmentType.Volcano,
                TargetMonsterName = "Imp",
                TargetMonsterType = MonsterType.Demon,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 350,
                GoldReward = 175,
                RequiredLevel = 4,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Experiência Diária", "Ouro Diário" },
                IsRepeatable = true,
                EstimatedDuration = 35
            },

            new Quest
            {
                Title = "Missão Diária: Caça aos Trolls",
                Description = "Derrote 1 troll para proteger as montanhas.",
                IntroductionText = "Os trolls estão causando problemas nas montanhas. Derrote 1 troll para proteger as montanhas.",
                CompletionText = "Incrível! Você protegeu as montanhas derrotando 1 troll.",
                Category = QuestCategory.Daily,
                Difficulty = QuestDifficulty.Hard,
                Environment = EnvironmentType.Ruins,
                TargetMonsterName = "Troll da Floresta",
                TargetMonsterType = MonsterType.Troll,
                Status = QuestStatus.NotStarted,
                ExperienceReward = 400,
                GoldReward = 200,
                RequiredLevel = 5,
                Prerequisites = new List<string>(),
                Rewards = new List<string> { "Experiência Diária", "Ouro Diário" },
                IsRepeatable = true,
                EstimatedDuration = 40
            }
        };
    }
}

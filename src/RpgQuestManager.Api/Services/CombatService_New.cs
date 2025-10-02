using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Combat;
using RpgQuestManager.Api.DTOs.Enemies;
using RpgQuestManager.Api.DTOs.Heroes;
using RpgQuestManager.Api.DTOs.Items;
using RpgQuestManager.Api.Models;
using AutoMapper;

namespace RpgQuestManager.Api.Services;

public class CombatServiceNew : ICombatService
{
    private readonly ApplicationDbContext _context;
    private readonly IDiceService _diceService;
    private readonly IDropService _dropService;
    private readonly IComboService _comboService;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<CombatServiceNew> _logger;
    private static readonly Random _random = new Random();

    public CombatServiceNew(
        ApplicationDbContext context,
        IDiceService diceService,
        IDropService dropService,
        IComboService comboService,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<CombatServiceNew> logger)
    {
        _context = context;
        _diceService = diceService;
        _dropService = dropService;
        _comboService = comboService;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CombatSessionDto> StartCombatAsync(int userId, List<int> heroIds, int questId)
    {
        _logger.LogInformation("🎯 Tentando iniciar combate: UserId={UserId}, HeroIds={HeroIds}, QuestId={QuestId}",
            userId, string.Join(",", heroIds), questId);

        if (heroIds == null || !heroIds.Any())
        {
            _logger.LogWarning("❌ Nenhum herói selecionado");
            throw new InvalidOperationException("Selecione pelo menos um herói para combate.");
        }

        if (heroIds.Count > 3)
        {
            _logger.LogWarning("❌ Mais de 3 heróis selecionados: {Count}", heroIds.Count);
            throw new InvalidOperationException("Máximo de 3 heróis por combate.");
        }

        // Valida que todos os heróis pertencem ao usuário
        var heroes = await _context.Heroes
            .Include(h => h.HeroItems)
                .ThenInclude(hi => hi.Item)
            .Where(h => heroIds.Contains(h.Id) && h.UserId == userId && !h.IsDeleted)
            .ToListAsync();

        _logger.LogInformation("🔍 Heróis encontrados: {Count}/{Expected}", heroes.Count, heroIds.Count);

        if (heroIds.Count != heroIds.Count)
        {
            _logger.LogWarning("❌ Heróis não encontrados ou não pertencem ao usuário");
            throw new InvalidOperationException("Um ou mais heróis não pertencem a você.");
        }

        var quest = await _context.Quests
            .Include(q => q.QuestEnemies)
                .ThenInclude(qe => qe.Enemy)
            .FirstOrDefaultAsync(q => q.Id == questId);

        if (quest == null)
        {
            _logger.LogWarning("❌ Quest {QuestId} não encontrada", questId);
            throw new KeyNotFoundException("Quest não encontrada.");
        }

        if (!quest.QuestEnemies.Any())
        {
            _logger.LogWarning("❌ Quest {QuestId} não tem inimigos", questId);
            throw new InvalidOperationException("Esta quest não possui inimigos configurados.");
        }

        // Verifica se já existe sessão ativa para algum dos heróis
        foreach (var heroId in heroIds)
        {
            var existingSession = await _context.CombatSessions
                .AnyAsync(cs => cs.HeroIds.Contains(heroId.ToString()) && cs.Status == CombatStatus.InProgress);
            
            if (existingSession)
            {
                var heroName = heroes.First(h => h.Id == heroId).Name;
                _logger.LogWarning("❌ Herói {HeroName} (ID: {HeroId}) já está em combate ativo", heroName, heroId);
                throw new InvalidOperationException($"O herói {heroName} já está em combate.");
            }
        }

        // Detecta combo se houver mais de um herói
        PartyCombo? combo = null;
        var groupBonus = 0;
        var comboBonus = 0;

        if (heroes.Count > 1)
        {
            var heroClasses = heroes.Select(h => h.Class).ToList();
            combo = await _comboService.DetectComboAsync(heroClasses);

            // Bônus de grupo: -1 no roll necessário por herói adicional
            groupBonus = -(heroes.Count - 1); // -1 para 2 heróis, -2 para 3 heróis
        }

        var firstEnemy = quest.QuestEnemies.OrderBy(qe => qe.Id).Select(qe => qe.Enemy).First();

        // Se tem combo, verifica fraqueza contra o primeiro boss
        if (combo != null && firstEnemy.IsBoss)
        {
            var weakness = await _comboService.GetBossWeaknessAsync(firstEnemy.Id, combo.Id);
            if (weakness != null)
            {
                comboBonus = -weakness.RollReduction; // RollReduction é negativo, então invertemos
            }
        }

        var combatSession = new CombatSession
        {
            QuestId = questId,
            CurrentEnemyId = firstEnemy.Id,
            ComboId = combo?.Id,
            GroupBonus = groupBonus,
            ComboBonus = comboBonus,
            StartedAt = DateTime.UtcNow,
            Status = CombatStatus.InProgress
        };

        combatSession.SetHeroIdsList(heroIds);

        _context.CombatSessions.Add(combatSession);
        await _context.SaveChangesAsync();

        var heroNames = string.Join(", ", heroes.Select(h => h.Name));
        _logger.LogInformation("🗡️ Combate iniciado! Party: [{HeroNames}] vs Quest '{QuestName}' (ID: {QuestId}). Combo: {ComboName}",
            heroNames, quest.Name, questId, combo?.Name ?? "Nenhum");

        return new CombatSessionDto
        {
            Id = combatSession.Id,
            HeroIds = heroIds,
            HeroNames = heroNames,
            QuestId = questId,
            QuestName = quest.Name,
            CurrentEnemyId = firstEnemy.Id,
            CurrentEnemyName = firstEnemy.Name,
            ComboId = combo?.Id,
            ComboName = combo?.Name,
            GroupBonus = groupBonus,
            ComboBonus = comboBonus,
            Status = combatSession.Status,
            StartedAt = combatSession.StartedAt
        };
    }

    public async Task<CombatSessionDetailDto> GetActiveCombatSessionAsync(int userId, int combatSessionId)
    {
        var combatSession = await _context.CombatSessions
            .Include(cs => cs.Quest)
                .ThenInclude(q => q.QuestEnemies)
                    .ThenInclude(qe => qe.Enemy)
            .Include(cs => cs.CurrentEnemy)
            .Include(cs => cs.Combo)
            .Include(cs => cs.CombatLogs)
                .ThenInclude(cl => cl.Enemy)
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId && cs.Status == CombatStatus.InProgress);

        if (combatSession == null)
        {
            throw new KeyNotFoundException("Sessão de combate não encontrada ou não está ativa.");
        }

        var heroIds = combatSession.GetHeroIdsList();

        // Valida que pelo menos um herói pertence ao usuário
        var userHeroIds = await _context.Heroes
            .Where(h => h.UserId == userId && heroIds.Contains(h.Id))
            .Select(h => h.Id)
            .ToListAsync();

        if (!userHeroIds.Any())
        {
            throw new UnauthorizedAccessException("Esta sessão de combate não pertence a você.");
        }

        // Busca os heróis com itens equipados
        var heroes = await _context.Heroes
            .Include(h => h.HeroItems)
                .ThenInclude(hi => hi.Item)
            .Where(h => heroIds.Contains(h.Id))
            .ToListAsync();

        var allQuestEnemies = combatSession.Quest.QuestEnemies.Select(qe => qe.Enemy).ToList();
        var defeatedEnemyIds = combatSession.CombatLogs
            .Where(cl => cl.Success == true && cl.EnemyId.HasValue)
            .Select(cl => cl.EnemyId!.Value)
            .Distinct()
            .ToList();

        var remainingEnemies = allQuestEnemies
            .Where(e => !defeatedEnemyIds.Contains(e.Id))
            .OrderBy(e => e.Id)
            .ToList();

        // Calcula bônus de atributos para cada herói
        var heroBonuses = new List<CombatBonusDto>();
        var currentEnemy = combatSession.CurrentEnemy;
        
        foreach (var heroForBonus in heroes)
        {
            var attackBonus = heroForBonus.GetCombatBonus(CombatType.Physical);
            var defenseBonus = heroForBonus.GetCombatBonus(CombatType.Agile);
            var magicBonus = heroForBonus.GetCombatBonus(CombatType.Magical);
            
            var combatBonus = currentEnemy != null ? heroForBonus.GetCombatBonus(currentEnemy.CombatType) : 0;
            var relevantStat = currentEnemy?.CombatType switch
            {
                CombatType.Physical => "Força",
                CombatType.Magical => "Inteligência",
                CombatType.Agile => "Destreza",
                _ => "Força"
            };

            heroBonuses.Add(new CombatBonusDto
            {
                HeroId = heroForBonus.Id,
                HeroName = heroForBonus.Name,
                AttackBonus = attackBonus,
                DefenseBonus = defenseBonus,
                MagicBonus = magicBonus,
                CombatBonus = combatBonus,
                RelevantStat = relevantStat
            });
        }

        // Calcula roll necessário considerando todos os bônus
        var baseRequiredRoll = currentEnemy?.MinimumRoll ?? 1;
        var totalAttributeBonus = heroBonuses.Sum(hb => hb.CombatBonus);
        var totalBonus = combatSession.GroupBonus + combatSession.ComboBonus + totalAttributeBonus;
        var adjustedRequiredRoll = Math.Max(1, baseRequiredRoll + totalBonus);

        var combatTypeDescription = currentEnemy?.CombatType switch
        {
            CombatType.Physical => "Combate Físico (usa Força)",
            CombatType.Magical => "Combate Mágico (usa Inteligência)",
            CombatType.Agile => "Combate Ágil (usa Destreza)",
            _ => "Combate Físico"
        };

        var dto = new CombatSessionDetailDto
        {
            Id = combatSession.Id,
            HeroIds = heroIds,
            Heroes = heroes.Select(h => new HeroDto
            {
                Id = h.Id,
                Name = h.Name,
                Class = h.Class,
                Level = h.Level,
                Experience = h.Experience,
                Strength = h.Strength,
                Intelligence = h.Intelligence,
                Dexterity = h.Dexterity,
                Gold = h.Gold,
                CreatedAt = h.CreatedAt,
                TotalAttack = h.GetTotalAttack(),
                TotalDefense = h.GetTotalDefense(),
                TotalMagic = h.GetTotalMagic()
            }).ToList(),
            QuestId = combatSession.QuestId,
            QuestName = combatSession.Quest.Name,
            CurrentEnemy = combatSession.CurrentEnemy != null ? _mapper.Map<EnemyDto>(combatSession.CurrentEnemy) : null,
            ComboId = combatSession.ComboId,
            ComboName = combatSession.Combo?.Name,
            ComboDescription = combatSession.Combo?.Description,
            GroupBonus = combatSession.GroupBonus,
            ComboBonus = combatSession.ComboBonus,
            Status = combatSession.Status,
            StartedAt = combatSession.StartedAt,
            CompletedAt = combatSession.CompletedAt,
            CombatLogs = combatSession.CombatLogs.Select(cl => new CombatLogDto
            {
                Id = cl.Id,
                Action = cl.Action,
                EnemyId = cl.EnemyId,
                EnemyName = cl.Enemy?.Name,
                DiceUsed = cl.DiceUsed,
                DiceResult = cl.DiceResult,
                RequiredRoll = cl.RequiredRoll,
                Success = cl.Success,
                Details = cl.Details,
                Timestamp = cl.Timestamp
            }).ToList(),
            RemainingEnemies = _mapper.Map<List<EnemyDto>>(remainingEnemies),
            RequiredRoll = adjustedRequiredRoll,
            CombatTypeDescription = combatTypeDescription,
            HeroBonuses = heroBonuses
        };

        return dto;
    }

    public async Task<CombatSessionDetailDto?> GetActiveCombatByHeroIdAsync(int userId, int heroId)
    {
        // Verifica se o herói pertence ao usuário
        var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.Id == heroId && h.UserId == userId);
        if (hero == null)
        {
            throw new UnauthorizedAccessException("Este herói não pertence a você.");
        }

        // Busca sessão de combate ativa que contenha este herói
        var activeSessions = await _context.CombatSessions
            .Include(cs => cs.Quest)
                .ThenInclude(q => q.QuestEnemies)
                    .ThenInclude(qe => qe.Enemy)
            .Include(cs => cs.CurrentEnemy)
            .Include(cs => cs.Combo)
            .Include(cs => cs.CombatLogs)
                .ThenInclude(cl => cl.Enemy)
            .Where(cs => cs.Status == CombatStatus.InProgress)
            .ToListAsync();

        // Filtra por sessões que contenham o heroId no campo HeroIds
        var combatSession = activeSessions.FirstOrDefault(cs => cs.GetHeroIdsList().Contains(heroId));

        if (combatSession == null)
        {
            return null;
        }

        // Retorna os detalhes da sessão
        var heroIds = combatSession.GetHeroIdsList();
        var heroes = await _context.Heroes
            .Include(h => h.HeroItems)
                .ThenInclude(hi => hi.Item)
            .Where(h => heroIds.Contains(h.Id))
            .ToListAsync();

        var allQuestEnemies = combatSession.Quest.QuestEnemies.Select(qe => qe.Enemy).ToList();
        var defeatedEnemyIds = combatSession.CombatLogs
            .Where(cl => cl.Success == true && cl.EnemyId.HasValue)
            .Select(cl => cl.EnemyId!.Value)
            .Distinct()
            .ToList();

        var remainingEnemies = allQuestEnemies
            .Where(e => !defeatedEnemyIds.Contains(e.Id))
            .OrderBy(e => e.Id)
            .ToList();

        // Calcula bônus de atributos para cada herói
        var heroBonuses = new List<CombatBonusDto>();
        var currentEnemy = combatSession.CurrentEnemy;
        
        foreach (var heroForBonus in heroes)
        {
            var attackBonus = heroForBonus.GetCombatBonus(CombatType.Physical);
            var defenseBonus = heroForBonus.GetCombatBonus(CombatType.Agile);
            var magicBonus = heroForBonus.GetCombatBonus(CombatType.Magical);
            
            var combatBonus = currentEnemy != null ? heroForBonus.GetCombatBonus(currentEnemy.CombatType) : 0;
            var relevantStat = currentEnemy?.CombatType switch
            {
                CombatType.Physical => "Força",
                CombatType.Magical => "Inteligência",
                CombatType.Agile => "Destreza",
                _ => "Força"
            };

            heroBonuses.Add(new CombatBonusDto
            {
                HeroId = heroForBonus.Id,
                HeroName = heroForBonus.Name,
                AttackBonus = attackBonus,
                DefenseBonus = defenseBonus,
                MagicBonus = magicBonus,
                CombatBonus = combatBonus,
                RelevantStat = relevantStat
            });
        }

        // Calcula roll necessário considerando todos os bônus
        var baseRequiredRoll = currentEnemy?.MinimumRoll ?? 1;
        var totalAttributeBonus = heroBonuses.Sum(hb => hb.CombatBonus);
        var totalBonus = combatSession.GroupBonus + combatSession.ComboBonus + totalAttributeBonus;
        var adjustedRequiredRoll = Math.Max(1, baseRequiredRoll + totalBonus);

        var combatTypeDescription = currentEnemy?.CombatType switch
        {
            CombatType.Physical => "Combate Físico (usa Força)",
            CombatType.Magical => "Combate Mágico (usa Inteligência)",
            CombatType.Agile => "Combate Ágil (usa Destreza)",
            _ => "Combate Físico"
        };

        var dto = new CombatSessionDetailDto
        {
            Id = combatSession.Id,
            HeroIds = heroIds,
            Heroes = heroes.Select(h => new HeroDto
            {
                Id = h.Id,
                Name = h.Name,
                Class = h.Class,
                Level = h.Level,
                Experience = h.Experience,
                Strength = h.Strength,
                Intelligence = h.Intelligence,
                Dexterity = h.Dexterity,
                Gold = h.Gold,
                CreatedAt = h.CreatedAt,
                TotalAttack = h.GetTotalAttack(),
                TotalDefense = h.GetTotalDefense(),
                TotalMagic = h.GetTotalMagic()
            }).ToList(),
            QuestId = combatSession.QuestId,
            QuestName = combatSession.Quest.Name,
            CurrentEnemy = combatSession.CurrentEnemy != null ? _mapper.Map<EnemyDto>(combatSession.CurrentEnemy) : null,
            ComboId = combatSession.ComboId,
            ComboName = combatSession.Combo?.Name,
            ComboDescription = combatSession.Combo?.Description,
            GroupBonus = combatSession.GroupBonus,
            ComboBonus = combatSession.ComboBonus,
            Status = combatSession.Status,
            StartedAt = combatSession.StartedAt,
            CompletedAt = combatSession.CompletedAt,
            CombatLogs = combatSession.CombatLogs.Select(cl => new CombatLogDto
            {
                Id = cl.Id,
                Action = cl.Action,
                EnemyId = cl.EnemyId,
                EnemyName = cl.Enemy?.Name,
                DiceUsed = cl.DiceUsed,
                DiceResult = cl.DiceResult,
                RequiredRoll = cl.RequiredRoll,
                Success = cl.Success,
                Details = cl.Details,
                Timestamp = cl.Timestamp
            }).ToList(),
            RemainingEnemies = _mapper.Map<List<EnemyDto>>(remainingEnemies),
            RequiredRoll = adjustedRequiredRoll,
            CombatTypeDescription = combatTypeDescription,
            HeroBonuses = heroBonuses
        };

        return dto;
    }

    public async Task<RollDiceResultDto> RollDiceAsync(int userId, int combatSessionId, DiceType diceType)
    {
        _logger.LogInformation("🎲 RollDice chamado: UserId={UserId}, CombatSessionId={CombatSessionId}, DiceType={DiceType}", 
            userId, combatSessionId, diceType);

        var combatSession = await _context.CombatSessions
            .Include(cs => cs.CurrentEnemy)
            .Include(cs => cs.Quest)
                .ThenInclude(q => q.QuestEnemies)
                    .ThenInclude(qe => qe.Enemy)
            .Include(cs => cs.Combo)
            .Include(cs => cs.CombatLogs)
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId && cs.Status == CombatStatus.InProgress);

        if (combatSession == null)
        {
            // Verifica se a sessão existe mas com status diferente
            var anySession = await _context.CombatSessions.FirstOrDefaultAsync(cs => cs.Id == combatSessionId);
            if (anySession != null)
            {
                _logger.LogWarning("❌ Sessão {CombatSessionId} encontrada mas com status {Status}", 
                    combatSessionId, anySession.Status);
                
                if (anySession.Status == CombatStatus.Victory)
                {
                    throw new InvalidOperationException("🎉 Combate já foi concluído com VITÓRIA! Use o endpoint /complete para receber as recompensas.");
                }
                else if (anySession.Status == CombatStatus.Fled)
                {
                    throw new InvalidOperationException("A party fugiu deste combate.");
                }
                else if (anySession.Status == CombatStatus.Defeated)
                {
                    throw new InvalidOperationException("A party foi derrotada neste combate.");
                }
                
                throw new KeyNotFoundException($"Sessão de combate não está ativa (Status: {anySession.Status}).");
            }
            
            _logger.LogWarning("❌ Sessão {CombatSessionId} não existe no banco", combatSessionId);
            throw new KeyNotFoundException("Sessão de combate não encontrada.");
        }

        if (combatSession.CurrentEnemy == null)
        {
            throw new InvalidOperationException("Não há inimigo atual para combater.");
        }

        var heroIds = combatSession.GetHeroIdsList();
        var heroes = await _context.Heroes
            .Include(h => h.HeroItems)
                .ThenInclude(hi => hi.Item)
            .Where(h => h.UserId == userId && heroIds.Contains(h.Id) && !h.IsDeleted)
            .ToListAsync();

        if (!heroes.Any())
        {
            throw new UnauthorizedAccessException("Esta sessão de combate não pertence a você.");
        }

        // Busca o inventário de dados do player (compartilhado entre todos os heróis)
        var inventory = await _context.DiceInventories
            .FirstOrDefaultAsync(di => di.UserId == userId);

        if (inventory == null || !inventory.HasDice(diceType))
        {
            throw new InvalidOperationException($"Você não possui dados do tipo {diceType}.");
        }

        if (combatSession.CurrentEnemy.RequiredDiceType != diceType)
        {
            throw new InvalidOperationException($"O inimigo {combatSession.CurrentEnemy.Name} requer um dado do tipo {combatSession.CurrentEnemy.RequiredDiceType}, mas foi usado {diceType}.");
        }

        // Usa o dado do inventário do player
        inventory.UseDice(diceType);

        // Rola o dado
        var rollResult = _random.Next(1, (int)diceType + 1);
        var baseRequiredRoll = combatSession.CurrentEnemy.MinimumRoll;
        
        // Calcula bônus de atributos dos heróis
        var heroAttributeBonus = heroes.Sum(h => h.GetCombatBonus(combatSession.CurrentEnemy.CombatType));
        
        // Aplica os bônus (reduzem o roll necessário)
        var totalBonus = combatSession.GroupBonus + combatSession.ComboBonus + heroAttributeBonus;
        var adjustedRequiredRoll = Math.Max(1, baseRequiredRoll + totalBonus);

        var success = rollResult >= adjustedRequiredRoll;

        var combatLog = new CombatLog
        {
            CombatSessionId = combatSession.Id,
            EnemyId = combatSession.CurrentEnemy.Id,
            Action = "ROLL",
            DiceUsed = diceType,
            DiceResult = rollResult,
            RequiredRoll = adjustedRequiredRoll,
            Success = success,
            Details = $"Rolou {diceType}, resultado {rollResult}. Necessário {adjustedRequiredRoll} (base: {baseRequiredRoll}, bônus: {totalBonus}).",
            Timestamp = DateTime.UtcNow
        };
        _context.CombatLogs.Add(combatLog);

        string message;
        if (success)
        {
            message = $"✅ VITÓRIA! {combatSession.CurrentEnemy.Name} derrotado!";
            _logger.LogInformation("Inimigo {EnemyName} derrotado na sessão {SessionId}", combatSession.CurrentEnemy.Name, combatSession.Id);

            // Verifica se há mais inimigos
            var allQuestEnemies = combatSession.Quest.QuestEnemies.Select(qe => qe.Enemy).ToList();
            var defeatedEnemyIds = combatSession.CombatLogs
                .Where(cl => cl.Success == true && cl.EnemyId.HasValue)
                .Select(cl => cl.EnemyId!.Value)
                .Distinct()
                .ToList();
            defeatedEnemyIds.Add(combatSession.CurrentEnemy.Id);

            var remainingEnemies = allQuestEnemies
                .Where(e => !defeatedEnemyIds.Contains(e.Id))
                .OrderBy(e => e.Id)
                .ToList();

            if (remainingEnemies.Any())
            {
                // Avança para o próximo inimigo
                combatSession.CurrentEnemyId = remainingEnemies.First().Id;
                message += $" Próximo inimigo: {remainingEnemies.First().Name}";

                // Recalcula bônus de combo para o novo boss
                if (combatSession.ComboId.HasValue && remainingEnemies.First().IsBoss)
                {
                    var weakness = await _comboService.GetBossWeaknessAsync(remainingEnemies.First().Id, combatSession.ComboId.Value);
                    combatSession.ComboBonus = weakness != null ? -weakness.RollReduction : 0;
                }
            }
            else
            {
                // Todos os inimigos derrotados!
                combatSession.Status = CombatStatus.Victory;
                message += " 🎉 VITÓRIA! Clique em 'Finalizar Combate' para suas recompensas!";
            }
        }
        else
        {
            message = $"❌ FALHOU! Rolou {rollResult}, mas precisava de {adjustedRequiredRoll}. Tente novamente!";
        }

        await _context.SaveChangesAsync();

        // Recarrega a sessão atualizada sem filtrar por status
        // (necessário porque após vitória o status muda para Victory)
        var updatedSession = await GetCombatSessionDetailAsync(combatSessionId);

        return new RollDiceResultDto
        {
            Roll = rollResult,
            RequiredRoll = adjustedRequiredRoll,
            Success = success,
            Message = message,
            UpdatedCombatSession = updatedSession
        };
    }

    // Método auxiliar para buscar sessão sem filtrar por status
    private async Task<CombatSessionDetailDto> GetCombatSessionDetailAsync(int combatSessionId)
    {
        var combatSession = await _context.CombatSessions
            .Include(cs => cs.Quest)
                .ThenInclude(q => q.QuestEnemies)
                    .ThenInclude(qe => qe.Enemy)
            .Include(cs => cs.CurrentEnemy)
            .Include(cs => cs.Combo)
            .Include(cs => cs.CombatLogs)
                .ThenInclude(cl => cl.Enemy)
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId);

        if (combatSession == null)
        {
            throw new KeyNotFoundException("Sessão de combate não encontrada.");
        }

        return await BuildCombatSessionDetailDto(combatSession);
    }

    // Constrói o DTO a partir da entidade
    private async Task<CombatSessionDetailDto> BuildCombatSessionDetailDto(CombatSession combatSession)
    {
        var sessionHeroIds = combatSession.GetHeroIdsList();
        var sessionHeroes = await _context.Heroes
            .Include(h => h.HeroItems)
                .ThenInclude(hi => hi.Item)
            .Where(h => sessionHeroIds.Contains(h.Id))
            .ToListAsync();

        var allEnemies = combatSession.Quest.QuestEnemies.Select(qe => qe.Enemy).ToList();
        var defeatedIds = combatSession.CombatLogs
            .Where(cl => cl.Success == true && cl.EnemyId.HasValue)
            .Select(cl => cl.EnemyId!.Value)
            .Distinct()
            .ToList();

        var remaining = allEnemies
            .Where(e => !defeatedIds.Contains(e.Id))
            .OrderBy(e => e.Id)
            .ToList();

        // Calcula bônus de atributos para cada herói
        var heroBonuses = new List<CombatBonusDto>();
        var currentEnemy = combatSession.CurrentEnemy;
        
        foreach (var hero in sessionHeroes)
        {
            var attackBonus = hero.GetCombatBonus(CombatType.Physical);
            var defenseBonus = hero.GetCombatBonus(CombatType.Agile);
            var magicBonus = hero.GetCombatBonus(CombatType.Magical);
            
            var combatBonus = currentEnemy != null ? hero.GetCombatBonus(currentEnemy.CombatType) : 0;
            var relevantStat = currentEnemy?.CombatType switch
            {
                CombatType.Physical => "Força",
                CombatType.Magical => "Inteligência",
                CombatType.Agile => "Destreza",
                _ => "Força"
            };

            heroBonuses.Add(new CombatBonusDto
            {
                HeroId = hero.Id,
                HeroName = hero.Name,
                AttackBonus = attackBonus,
                DefenseBonus = defenseBonus,
                MagicBonus = magicBonus,
                CombatBonus = combatBonus,
                RelevantStat = relevantStat
            });
        }

        // Calcula roll necessário considerando todos os bônus
        var baseRequiredRoll = currentEnemy?.MinimumRoll ?? 1;
        var totalAttributeBonus = heroBonuses.Sum(hb => hb.CombatBonus);
        var totalBonus = combatSession.GroupBonus + combatSession.ComboBonus + totalAttributeBonus;
        var adjustedRequiredRoll = Math.Max(1, baseRequiredRoll + totalBonus);

        var combatTypeDescription = currentEnemy?.CombatType switch
        {
            CombatType.Physical => "Combate Físico (usa Força)",
            CombatType.Magical => "Combate Mágico (usa Inteligência)",
            CombatType.Agile => "Combate Ágil (usa Destreza)",
            _ => "Combate Físico"
        };

        var dto = new CombatSessionDetailDto
        {
            Id = combatSession.Id,
            HeroIds = sessionHeroIds,
            Heroes = sessionHeroes.Select(h => new HeroDto
            {
                Id = h.Id,
                Name = h.Name,
                Class = h.Class,
                Level = h.Level,
                Experience = h.Experience,
                Strength = h.Strength,
                Intelligence = h.Intelligence,
                Dexterity = h.Dexterity,
                Gold = h.Gold,
                CreatedAt = h.CreatedAt,
                TotalAttack = h.GetTotalAttack(),
                TotalDefense = h.GetTotalDefense(),
                TotalMagic = h.GetTotalMagic()
            }).ToList(),
            QuestId = combatSession.QuestId,
            QuestName = combatSession.Quest.Name,
            CurrentEnemy = combatSession.CurrentEnemy != null ? new EnemyDto
            {
                Id = combatSession.CurrentEnemy.Id,
                Name = combatSession.CurrentEnemy.Name,
                Type = combatSession.CurrentEnemy.Type,
                Power = combatSession.CurrentEnemy.Power,
                Health = combatSession.CurrentEnemy.Health,
                RequiredDiceType = combatSession.CurrentEnemy.RequiredDiceType,
                MinimumRoll = combatSession.CurrentEnemy.MinimumRoll,
                CombatType = combatSession.CurrentEnemy.CombatType,
                IsBoss = combatSession.CurrentEnemy.IsBoss,
                CreatedAt = combatSession.CurrentEnemy.CreatedAt
            } : null,
            ComboId = combatSession.ComboId,
            ComboName = combatSession.Combo?.Name,
            ComboDescription = combatSession.Combo?.Description,
            GroupBonus = combatSession.GroupBonus,
            ComboBonus = combatSession.ComboBonus,
            Status = combatSession.Status,
            StartedAt = combatSession.StartedAt,
            CompletedAt = combatSession.CompletedAt,
            CombatLogs = combatSession.CombatLogs.OrderByDescending(cl => cl.Timestamp).Select(cl => new CombatLogDto
            {
                Id = cl.Id,
                Action = cl.Action,
                EnemyId = cl.EnemyId,
                EnemyName = cl.Enemy?.Name,
                DiceUsed = cl.DiceUsed,
                DiceResult = cl.DiceResult,
                RequiredRoll = cl.RequiredRoll,
                Success = cl.Success,
                Details = cl.Details,
                Timestamp = cl.Timestamp
            }).ToList(),
            RemainingEnemies = remaining.Select(e => new EnemyDto
            {
                Id = e.Id,
                Name = e.Name,
                Type = e.Type,
                Power = e.Power,
                Health = e.Health,
                RequiredDiceType = e.RequiredDiceType,
                MinimumRoll = e.MinimumRoll,
                CombatType = e.CombatType,
                IsBoss = e.IsBoss,
                CreatedAt = e.CreatedAt
            }).ToList(),
            RequiredRoll = adjustedRequiredRoll,
            CombatTypeDescription = combatTypeDescription,
            HeroBonuses = heroBonuses
        };

        return dto;
    }

    public async Task<CompleteCombatResultDto> CompleteCombatAsync(int userId, int combatSessionId)
    {
        var combatSession = await _context.CombatSessions
            .Include(cs => cs.Quest)
                .ThenInclude(q => q.Rewards)
                    .ThenInclude(r => r.Item)
            .Include(cs => cs.Quest)
                .ThenInclude(q => q.QuestEnemies)
                    .ThenInclude(qe => qe.Enemy)
            .Include(cs => cs.Combo)
            .Include(cs => cs.CombatLogs)
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId);

        if (combatSession == null)
        {
            throw new KeyNotFoundException("Sessão de combate não encontrada.");
        }

        if (combatSession.Status != CombatStatus.Victory)
        {
            throw new InvalidOperationException("O combate ainda não foi vencido ou já foi completado.");
        }

        var heroIds = combatSession.GetHeroIdsList();
        var heroes = await _context.Heroes
            .Where(h => h.UserId == userId && heroIds.Contains(h.Id))
            .ToListAsync();

        if (!heroes.Any())
        {
            throw new UnauthorizedAccessException("Esta sessão de combate não pertence a você.");
        }

        // Calcula penalidade de recompensas baseado no número de heróis
        decimal rewardMultiplier = heroes.Count switch
        {
            1 => 1.0m,     // 100% das recompensas
            2 => 0.75m,    // 75% das recompensas
            3 => 0.60m,    // 60% das recompensas
            _ => 0.60m
        };

        // Distribui XP e ouro
        var baseGold = combatSession.Quest.GoldReward;
        var baseExp = combatSession.Quest.ExperienceReward;

        var totalGold = (int)(baseGold * rewardMultiplier);
        var expPerHero = (int)(baseExp * rewardMultiplier / heroes.Count);

        // Ouro vai para o usuário, não para os heróis individuais
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user != null)
        {
            user.Gold += totalGold;
            _logger.LogInformation("💰 Player {UserId} recebeu {Gold} ouro da quest {QuestName}", 
                userId, totalGold, combatSession.Quest.Name);
        }

        // XP vai para cada herói
        foreach (var heroToLevel in heroes)
        {
            heroToLevel.Experience += expPerHero;

            var oldLevel = heroToLevel.Level;
            if (heroToLevel.CanLevelUp())
            {
                heroToLevel.LevelUp();
                if (heroToLevel.Level > oldLevel)
                {
                    await _notificationService.NotifyLevelUpAsync(heroToLevel, oldLevel, heroToLevel.Level);
                }
            }
            
            // Sistema de drops de itens especiais
            await TryDropSpecialItems(heroToLevel, combatSession.Quest);
        }

        // Drops de TODOS os inimigos derrotados (não apenas bosses)
        var allDroppedItems = new List<Item>();
        var defeatedEnemies = combatSession.Quest.QuestEnemies
            .Where(qe => combatSession.CombatLogs.Any(cl => cl.Success == true && cl.EnemyId == qe.EnemyId))
            .Select(qe => qe.Enemy)
            .ToList(); // REMOVIDO: .Where(e => e.IsBoss) - agora TODOS os inimigos podem dropar!

        foreach (var enemy in defeatedEnemies)
        {
            var drops = await _dropService.CalculateDropsAsync(enemy.Id);
            allDroppedItems.AddRange(drops);
            
            _logger.LogInformation("🎁 Inimigo {EnemyName} dropou {Count} itens", 
                enemy.Name, drops.Count);
        }

        // Adiciona itens dropados ao primeiro herói
        if (allDroppedItems.Any() && heroes.Any())
        {
            var mainHero = heroes.First();
            foreach (var item in allDroppedItems)
            {
                var heroItem = await _context.HeroItems
                    .FirstOrDefaultAsync(hi => hi.HeroId == mainHero.Id && hi.ItemId == item.Id);

                if (heroItem != null)
                {
                    heroItem.Quantity++;
                }
                else
                {
                    _context.HeroItems.Add(new HeroItem
                    {
                        HeroId = mainHero.Id,
                        ItemId = item.Id,
                        Quantity = 1,
                        IsEquipped = false,
                        AcquiredAt = DateTime.UtcNow
                    });
                }
            }
        }

        // Registra descoberta de combos (APENAS para bosses)
        var defeatedBosses = defeatedEnemies.Where(e => e.IsBoss).ToList();
        if (combatSession.ComboId.HasValue && defeatedBosses.Any())
        {
            foreach (var boss in defeatedBosses)
            {
                var isNewDiscovery = await _comboService.RegisterDiscoveryAsync(userId, boss.Id, combatSession.ComboId.Value);
                if (isNewDiscovery)
                {
                    await _notificationService.CreateNotificationAsync(
                        userId,
                        "🔍 Fraqueza Descoberta!",
                        $"Você descobriu que o boss {boss.Name} é fraco contra o combo {combatSession.Combo!.Name}!",
                        "Success"
                    );
                }
            }
        }

        // Marca a quest como completada para todos os heróis que participaram
        foreach (var hero in heroes)
        {
            var heroQuest = await _context.HeroQuests
                .FirstOrDefaultAsync(hq => hq.HeroId == hero.Id && hq.QuestId == combatSession.QuestId);

            if (heroQuest != null && !heroQuest.IsCompleted)
            {
                heroQuest.IsCompleted = true;
                heroQuest.CompletedAt = DateTime.UtcNow;
                
                _logger.LogInformation("✅ Quest {QuestName} marcada como completada para herói {HeroName}", 
                    combatSession.Quest.Name, hero.Name);
            }
        }

        combatSession.Status = CombatStatus.Victory;
        combatSession.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("💰 Combate completado! Party recebeu {Gold} ouro e {Exp} XP. Drops: {ItemCount} itens",
            totalGold, expPerHero * heroes.Count, allDroppedItems.Count);

        // Monta mensagem detalhada dos drops
        string dropMessage = "";
        if (allDroppedItems.Any())
        {
            var dropsByBoss = allDroppedItems
                .GroupBy(item => defeatedEnemies.FirstOrDefault(e => 
                    combatSession.CombatLogs.Any(cl => cl.Success == true && cl.EnemyId == e.Id))?.Name ?? "Boss")
                .ToList();

            dropMessage = "\n\n🎁 **DROPS RECEBIDOS:**\n";
            foreach (var group in dropsByBoss)
            {
                dropMessage += $"\n**{group.Key}** dropou:\n";
                foreach (var item in group)
                {
                    var rarityIcon = item.Rarity switch
                    {
                        ItemRarity.Common => "⚪",
                        ItemRarity.Rare => "🔵",
                        ItemRarity.Epic => "🟣",
                        ItemRarity.Legendary => "🟠",
                        _ => "⚪"
                    };
                    dropMessage += $"  {rarityIcon} **{item.Name}** ({item.GetFullRarityName()})\n";
                }
            }
        }
        else
        {
            dropMessage = "\n\n💔 Nenhum item foi dropado desta vez...";
        }

        return new CompleteCombatResultDto
        {
            Status = "Victory",
            Message = $"🎉 **VITÓRIA!**\n\n💰 Você recebeu **{totalGold} ouro**\n⭐ Cada herói ganhou **{expPerHero} XP**\n\n⚔️ Recompensa ajustada para {heroes.Count} herói(s) ({rewardMultiplier:P0}){dropMessage}",
            GoldEarned = totalGold,
            ExperienceEarned = expPerHero * heroes.Count,
            HeroNewLevel = heroes.Max(h => h.Level),
            DroppedItems = _mapper.Map<List<ItemDto>>(allDroppedItems)
        };
    }

    public async Task FleeCombatAsync(int userId, int combatSessionId)
    {
        var combatSession = await _context.CombatSessions
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId && cs.Status == CombatStatus.InProgress);

        if (combatSession == null)
        {
            throw new KeyNotFoundException("Sessão de combate não encontrada ou não está ativa.");
        }

        var heroIds = combatSession.GetHeroIdsList();
        var userHeroIds = await _context.Heroes
            .Where(h => h.UserId == userId && heroIds.Contains(h.Id))
            .Select(h => h.Id)
            .ToListAsync();

        if (!userHeroIds.Any())
        {
            throw new UnauthorizedAccessException("Esta sessão de combate não pertence a você.");
        }

        combatSession.Status = CombatStatus.Fled;
        combatSession.CompletedAt = DateTime.UtcNow;

        var combatLog = new CombatLog
        {
            CombatSessionId = combatSession.Id,
            Action = "FLEE",
            Details = "A party fugiu do combate.",
            Timestamp = DateTime.UtcNow
        };
        _context.CombatLogs.Add(combatLog);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Party fugiu do combate na sessão {SessionId}", combatSessionId);
    }

    /// <summary>
    /// Tenta dropar itens especiais para o herói após completar uma quest
    /// </summary>
    private async Task TryDropSpecialItems(Hero hero, Quest quest)
    {
        var random = new Random();
        
        // Chance base de drop (5% por quest)
        var baseDropChance = 0.05;
        
        // Aumenta chance baseado na dificuldade da quest
        var difficultyMultiplier = quest.Difficulty switch
        {
            "Fácil" => 1.0,
            "Médio" => 1.5,
            "Difícil" => 2.0,
            "Muito Difícil" => 2.5,
            "Épico" => 3.0,
            "Lendário" => 4.0,
            _ => 1.0
        };
        
        var finalDropChance = baseDropChance * difficultyMultiplier;
        
        if (random.NextDouble() < finalDropChance)
        {
            // Seleciona um item especial para dropar
            var specialItems = await _context.Items
                .Where(i => i.IsConsumable && (i.BonusStrength > 0 || i.BonusIntelligence > 0 || i.BonusDexterity > 0))
                .OrderBy(i => i.Rarity)
                .ToListAsync();
            
            if (specialItems.Any())
            {
                // Peso baseado na raridade (mais raro = menos chance)
                var weightedItems = new List<Item>();
                foreach (var item in specialItems)
                {
                    var weight = item.Rarity switch
                    {
                        ItemRarity.Common => 50,
                        ItemRarity.Rare => 30,
                        ItemRarity.Epic => 15,
                        ItemRarity.Legendary => 5,
                        _ => 10
                    };
                    
                    for (int i = 0; i < weight; i++)
                    {
                        weightedItems.Add(item);
                    }
                }
                
                var selectedItem = weightedItems[random.Next(weightedItems.Count)];
                
                // Adicionar ao inventário do herói
                var existingHeroItem = await _context.HeroItems
                    .FirstOrDefaultAsync(hi => hi.HeroId == hero.Id && hi.ItemId == selectedItem.Id);
                
                if (existingHeroItem != null)
                {
                    existingHeroItem.Quantity += 1;
                }
                else
                {
                    _context.HeroItems.Add(new HeroItem
                    {
                        HeroId = hero.Id,
                        ItemId = selectedItem.Id,
                        Quantity = 1
                    });
                }
                
                _logger.LogInformation("🎁 Item especial dropado! {HeroName} recebeu {ItemName} ({Rarity})", 
                    hero.Name, selectedItem.Name, selectedItem.Rarity);
            }
        }
    }

}

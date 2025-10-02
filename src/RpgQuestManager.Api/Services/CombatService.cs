using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class CombatService : ICombatService
{
    private readonly ApplicationDbContext _context;
    private readonly IDiceService _diceService;
    private readonly IDropService _dropService;
    private readonly ILogger<CombatService> _logger;

    public CombatService(
        ApplicationDbContext context,
        IDiceService diceService,
        IDropService dropService,
        ILogger<CombatService> logger)
    {
        _context = context;
        _diceService = diceService;
        _dropService = dropService;
        _logger = logger;
    }

    public async Task<CombatSession> StartCombatAsync(int heroId, int questId)
    {
        // Verifica se já existe uma sessão ativa
        var existingSession = await GetActiveCombatAsync(heroId);
        if (existingSession != null)
        {
            throw new InvalidOperationException("Já existe uma sessão de combate ativa para este herói.");
        }

        var quest = await _context.Quests
            .Include(q => q.QuestEnemies)
                .ThenInclude(qe => qe.Enemy)
            .FirstOrDefaultAsync(q => q.Id == questId);

        if (quest == null)
        {
            throw new InvalidOperationException("Quest não encontrada.");
        }

        var session = new CombatSession
        {
            HeroId = heroId,
            QuestId = questId,
            Status = CombatStatus.InProgress,
            StartedAt = DateTime.UtcNow
        };

        _context.CombatSessions.Add(session);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Combate iniciado: Herói {HeroId} vs Quest {QuestId}", heroId, questId);

        return session;
    }

    public async Task<CombatSession?> GetActiveCombatAsync(int heroId)
    {
        return await _context.CombatSessions
            .Include(cs => cs.Quest)
                .ThenInclude(q => q.QuestEnemies)
                    .ThenInclude(qe => qe.Enemy)
            .Include(cs => cs.CombatLogs)
            .Include(cs => cs.CurrentEnemy)
            .FirstOrDefaultAsync(cs => cs.HeroId == heroId && cs.Status == CombatStatus.InProgress);
    }

    public async Task<(bool Success, int RollResult, string Message)> RollDiceAsync(
        int combatSessionId, 
        int enemyId, 
        DiceType diceType)
    {
        var session = await _context.CombatSessions
            .Include(cs => cs.Hero)
            .Include(cs => cs.Quest)
                .ThenInclude(q => q.QuestEnemies)
                    .ThenInclude(qe => qe.Enemy)
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId);

        if (session == null)
        {
            throw new InvalidOperationException("Sessão de combate não encontrada.");
        }

        if (session.Status != CombatStatus.InProgress)
        {
            throw new InvalidOperationException("Esta sessão de combate já foi finalizada.");
        }

        var enemy = await _context.Enemies.FindAsync(enemyId);
        if (enemy == null)
        {
            throw new InvalidOperationException("Inimigo não encontrado.");
        }

        // Verifica se o herói tem o dado necessário
        var hasUsed = await _diceService.UseDiceAsync(session.HeroId, diceType);
        if (!hasUsed)
        {
            throw new InvalidOperationException($"Você não possui dados do tipo {diceType} disponíveis.");
        }

        // Rola o dado
        var rollResult = await _diceService.RollDiceAsync(diceType);
        var success = rollResult >= enemy.MinimumRoll;

        // Registra no log
        var log = new CombatLog
        {
            CombatSessionId = combatSessionId,
            EnemyId = enemyId,
            Action = "ROLL",
            DiceUsed = diceType,
            DiceResult = rollResult,
            RequiredRoll = enemy.MinimumRoll,
            Success = success,
            Details = $"Rolou {diceType}: {rollResult} (necessário: {enemy.MinimumRoll}+)",
            Timestamp = DateTime.UtcNow
        };

        _context.CombatLogs.Add(log);
        session.CurrentEnemyId = enemyId;
        
        await _context.SaveChangesAsync();

        var message = success
            ? $"🎲 Sucesso! Você rolou {rollResult} no {diceType} e derrotou {enemy.Name}!"
            : $"💥 Falhou! Você rolou {rollResult} no {diceType}, mas precisava de {enemy.MinimumRoll}+";

        _logger.LogInformation(
            "Combate {SessionId}: Herói {HeroId} rolou {DiceType} = {Result} vs {Enemy} (necessário {MinRoll}): {Success}",
            combatSessionId, session.HeroId, diceType, rollResult, enemy.Name, enemy.MinimumRoll, success);

        return (success, rollResult, message);
    }

    public async Task<(CombatStatus Status, List<Item> Drops)> CompleteCombatAsync(int combatSessionId)
    {
        var session = await _context.CombatSessions
            .Include(cs => cs.Hero)
            .Include(cs => cs.Quest)
                .ThenInclude(q => q.QuestEnemies)
                    .ThenInclude(qe => qe.Enemy)
                        .ThenInclude(e => e.BossDrops)
                            .ThenInclude(bd => bd.Item)
            .Include(cs => cs.CombatLogs)
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId);

        if (session == null)
        {
            throw new InvalidOperationException("Sessão de combate não encontrada.");
        }

        // Verifica quais inimigos foram derrotados
        var defeatedEnemyIds = session.CombatLogs
            .Where(log => log.Success == true && log.EnemyId.HasValue)
            .Select(log => log.EnemyId!.Value)
            .Distinct()
            .ToList();

        var totalEnemies = session.Quest.QuestEnemies.Count;
        var defeatedCount = defeatedEnemyIds.Count;

        // Determina o status
        CombatStatus status;
        if (defeatedCount >= totalEnemies)
        {
            status = CombatStatus.Victory;
        }
        else if (defeatedCount == 0)
        {
            status = CombatStatus.Defeated;
        }
        else
        {
            status = CombatStatus.Fled; // Parcialmente completado = fugiu
        }

        session.Status = status;
        session.CompletedAt = DateTime.UtcNow;

        // Processa drops apenas se vitória
        var drops = new List<Item>();
        if (status == CombatStatus.Victory)
        {
            var defeatedEnemies = session.Quest.QuestEnemies
                .Where(qe => defeatedEnemyIds.Contains(qe.EnemyId))
                .Select(qe => qe.Enemy)
                .ToList();

            foreach (var enemy in defeatedEnemies)
            {
                var enemyDrops = await _dropService.ProcessDropsAsync(session.HeroId, enemy);
                drops.AddRange(enemyDrops);
            }

            _logger.LogInformation(
                "Combate {SessionId} completado com VITÓRIA! {DropsCount} itens dropados",
                combatSessionId, drops.Count);
        }

        await _context.SaveChangesAsync();

        return (status, drops);
    }

    public async Task<bool> FleeCombatAsync(int combatSessionId)
    {
        var session = await _context.CombatSessions.FindAsync(combatSessionId);
        if (session == null) return false;

        session.Status = CombatStatus.Fled;
        session.CompletedAt = DateTime.UtcNow;

        var log = new CombatLog
        {
            CombatSessionId = combatSessionId,
            Action = "FLEE",
            Details = "O herói fugiu do combate",
            Timestamp = DateTime.UtcNow
        };

        _context.CombatLogs.Add(log);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Combate {SessionId}: Herói fugiu", combatSessionId);

        return true;
    }
}


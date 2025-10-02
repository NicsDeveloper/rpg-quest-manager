using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Combat;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class CombatService : ICombatService
{
    private readonly ApplicationDbContext _context;
    private readonly Random _random = new();

    public CombatService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CombatDetailDto> StartCombatAsync(int userId, StartCombatRequest request)
    {
        // Verificar se já existe combate ativo
        var activeCombat = await GetActiveCombatAsync(userId);
        if (activeCombat != null)
        {
            throw new InvalidOperationException("Já existe um combate ativo para este usuário.");
        }

        // Buscar quest e inimigos
        var quest = await _context.Quests
            .Include(q => q.QuestEnemies)
                .ThenInclude(qe => qe.Enemy)
            .FirstOrDefaultAsync(q => q.Id == request.QuestId);

        if (quest == null)
        {
            throw new KeyNotFoundException("Quest não encontrada.");
        }

        if (!quest.QuestEnemies.Any())
        {
            throw new InvalidOperationException("Esta quest não possui inimigos.");
        }

        // Buscar heróis
        var heroes = await _context.Heroes
            .Where(h => request.HeroIds.Contains(h.Id) && h.UserId == userId && !h.IsDeleted)
            .ToListAsync();

        if (heroes.Count != request.HeroIds.Count)
        {
            throw new InvalidOperationException("Alguns heróis não foram encontrados ou não pertencem ao usuário.");
        }

        // Selecionar primeiro inimigo
        var firstEnemy = quest.QuestEnemies.First().Enemy;

        // Calcular vida máxima dos heróis
        var heroHealths = new Dictionary<int, int>();
        var maxHeroHealths = new Dictionary<int, int>();

        foreach (var hero in heroes)
        {
            var maxHealth = CalculateMaxHealth(hero);
            maxHeroHealths[hero.Id] = maxHealth;
            heroHealths[hero.Id] = maxHealth;
        }

        // Criar sessão de combate
        var combatSession = new CombatSession
        {
            UserId = userId,
            QuestId = request.QuestId,
            CurrentEnemyId = firstEnemy.Id,
            Status = CombatStatus.Preparing,
            IsHeroTurn = true,
            CurrentEnemyHealth = firstEnemy.Health,
            MaxEnemyHealth = firstEnemy.Health,
            HeroHealths = System.Text.Json.JsonSerializer.Serialize(heroHealths),
            MaxHeroHealths = System.Text.Json.JsonSerializer.Serialize(maxHeroHealths),
            StartedAt = DateTime.UtcNow
        };

        combatSession.SetHeroIdsList(request.HeroIds);

        _context.CombatSessions.Add(combatSession);
        await _context.SaveChangesAsync();

        // Log de início
        var startLog = new CombatLog
        {
            CombatSessionId = combatSession.Id,
            Action = "COMBAT_START",
            Details = $"Combate iniciado contra {firstEnemy.Name}",
            Timestamp = DateTime.UtcNow
        };
        _context.CombatLogs.Add(startLog);

        // Mudar status para em progresso
        combatSession.Status = CombatStatus.InProgress;
        await _context.SaveChangesAsync();

        return await GetCombatDetailAsync(combatSession.Id);
    }

    public async Task<CombatDetailDto?> GetActiveCombatAsync(int userId)
    {
        var combatSession = await _context.CombatSessions
            .Include(cs => cs.Quest)
            .Include(cs => cs.CurrentEnemy)
            .Include(cs => cs.CombatLogs)
            .FirstOrDefaultAsync(cs => cs.UserId == userId && cs.Status == CombatStatus.InProgress);

        if (combatSession == null) return null;

        return await GetCombatDetailAsync(combatSession.Id);
    }

    public async Task<RollDiceResult> RollDiceAsync(int combatSessionId, DiceType diceType)
    {
        var combatSession = await _context.CombatSessions
            .Include(cs => cs.Quest)
            .Include(cs => cs.CurrentEnemy)
            .Include(cs => cs.CombatLogs)
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId);

        if (combatSession == null)
        {
            throw new KeyNotFoundException("Sessão de combate não encontrada.");
        }

        if (combatSession.Status != CombatStatus.InProgress)
        {
            throw new InvalidOperationException("O combate não está em andamento.");
        }

        if (!combatSession.IsHeroTurn)
        {
            throw new InvalidOperationException("Não é a vez do herói atacar.");
        }

        // Buscar heróis
        var heroIds = combatSession.GetHeroIdsList();
        var heroes = await _context.Heroes
            .Where(h => heroIds.Contains(h.Id))
            .ToListAsync();

        // Rolar dados
        var roll = _random.Next(1, GetDiceMaxValue(diceType) + 1);
        var requiredRoll = combatSession.CurrentEnemy.MinimumRoll;
        var success = roll >= requiredRoll;

        string message;
        int damageDealt = 0;
        int enemyHealthAfter = combatSession.CurrentEnemyHealth;

        if (success)
        {
            // Calcular dano total dos heróis
            var totalDamage = heroes.Sum(h => CalculateAttackPower(h));
            damageDealt = totalDamage;
            
            // Aplicar dano ao inimigo
            combatSession.CurrentEnemyHealth = Math.Max(0, combatSession.CurrentEnemyHealth - totalDamage);
            enemyHealthAfter = combatSession.CurrentEnemyHealth;

            message = $"✅ SUCESSO! Rolou {roll}, precisava de {requiredRoll}. Causou {totalDamage} de dano!";

            // Verificar se inimigo morreu
            if (combatSession.CurrentEnemyHealth <= 0)
            {
                combatSession.Status = CombatStatus.Victory;
                combatSession.CompletedAt = DateTime.UtcNow;
                message += " Inimigo derrotado!";
            }
            else
            {
                // Passar vez para o inimigo
                combatSession.IsHeroTurn = false;
            }
        }
        else
        {
            message = $"❌ FALHOU! Rolou {roll}, mas precisava de {requiredRoll}. Vez do inimigo!";
            combatSession.IsHeroTurn = false;
        }

        // Criar log
        var log = new CombatLog
        {
            CombatSessionId = combatSession.Id,
            Action = "HERO_ATTACK",
            DiceUsed = diceType,
            DiceResult = roll,
            RequiredRoll = requiredRoll,
            Success = success,
            DamageDealt = damageDealt,
            EnemyHealthAfter = enemyHealthAfter,
            Details = message,
            Timestamp = DateTime.UtcNow
        };
        _context.CombatLogs.Add(log);

        await _context.SaveChangesAsync();

        return new RollDiceResult
        {
            Roll = roll,
            RequiredRoll = requiredRoll,
            Success = success,
            DamageDealt = damageDealt,
            EnemyHealthAfter = enemyHealthAfter,
            Message = message,
            UpdatedCombatSession = await GetCombatDetailAsync(combatSession.Id)
        };
    }

    public async Task<EnemyAttackResult> EnemyAttackAsync(int combatSessionId)
    {
        var combatSession = await _context.CombatSessions
            .Include(cs => cs.Quest)
            .Include(cs => cs.CurrentEnemy)
            .Include(cs => cs.CombatLogs)
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId);

        if (combatSession == null)
        {
            throw new KeyNotFoundException("Sessão de combate não encontrada.");
        }

        if (combatSession.Status != CombatStatus.InProgress)
        {
            throw new InvalidOperationException("O combate não está em andamento.");
        }

        if (combatSession.IsHeroTurn)
        {
            throw new InvalidOperationException("Não é a vez do inimigo atacar.");
        }

        // Buscar heróis
        var heroIds = combatSession.GetHeroIdsList();
        var heroes = await _context.Heroes
            .Where(h => heroIds.Contains(h.Id))
            .ToListAsync();

        // Inimigo ataca
        var enemyRoll = _random.Next(1, 7); // D6 para inimigo
        var enemyPower = combatSession.CurrentEnemy.Power;
        var totalDamage = enemyPower + enemyRoll;

        // Calcular defesa total dos heróis
        var totalDefense = heroes.Sum(h => CalculateDefensePower(h));
        var finalDamage = Math.Max(1, totalDamage - totalDefense);

        // Aplicar dano aos heróis
        var heroHealths = combatSession.GetHeroHealths();
        var allHeroesDead = true;

        foreach (var hero in heroes)
        {
            if (heroHealths.ContainsKey(hero.Id))
            {
                heroHealths[hero.Id] = Math.Max(0, heroHealths[hero.Id] - finalDamage);
                if (heroHealths[hero.Id] > 0)
                {
                    allHeroesDead = false;
                }
            }
        }

        combatSession.SetHeroHealths(heroHealths);

        string message;
        if (allHeroesDead)
        {
            combatSession.Status = CombatStatus.Defeat;
            combatSession.CompletedAt = DateTime.UtcNow;
            message = $"💀 DERROTA! {combatSession.CurrentEnemy.Name} causou {finalDamage} de dano e derrotou todos os heróis!";
        }
        else
        {
            combatSession.IsHeroTurn = true;
            message = $"⚔️ {combatSession.CurrentEnemy.Name} atacou! Causou {finalDamage} de dano. Vez dos heróis!";
        }

        // Criar log
        var log = new CombatLog
        {
            CombatSessionId = combatSession.Id,
            EnemyId = combatSession.CurrentEnemyId,
            Action = "ENEMY_ATTACK",
            DiceUsed = DiceType.D6,
            DiceResult = enemyRoll,
            RequiredRoll = 0,
            Success = true,
            DamageDealt = finalDamage,
            Details = message,
            Timestamp = DateTime.UtcNow
        };
        _context.CombatLogs.Add(log);

        await _context.SaveChangesAsync();

        return new EnemyAttackResult
        {
            EnemyRoll = enemyRoll,
            EnemyPower = enemyPower,
            TotalDamage = totalDamage,
            HeroDefense = totalDefense,
            FinalDamage = finalDamage,
            Message = message,
            AllHeroesDead = allHeroesDead,
            UpdatedCombatSession = await GetCombatDetailAsync(combatSession.Id)
        };
    }

    public async Task<CombatDetailDto> CompleteCombatAsync(int combatSessionId)
    {
        var combatSession = await _context.CombatSessions
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId);

        if (combatSession == null)
        {
            throw new KeyNotFoundException("Sessão de combate não encontrada.");
        }

        combatSession.Status = CombatStatus.Victory;
        combatSession.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetCombatDetailAsync(combatSession.Id);
    }

    public async Task<bool> CancelCombatAsync(int combatSessionId)
    {
        var combatSession = await _context.CombatSessions
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId);

        if (combatSession == null) return false;

        combatSession.Status = CombatStatus.Cancelled;
        combatSession.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<CombatDetailDto> GetCombatDetailAsync(int combatSessionId)
    {
        var combatSession = await _context.CombatSessions
            .Include(cs => cs.Quest)
            .Include(cs => cs.CurrentEnemy)
            .Include(cs => cs.CombatLogs)
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId);

        if (combatSession == null)
        {
            throw new KeyNotFoundException("Sessão de combate não encontrada.");
        }

        var heroIds = combatSession.GetHeroIdsList();
        var heroes = await _context.Heroes
            .Where(h => heroIds.Contains(h.Id))
            .ToListAsync();

        var heroCombatInfos = heroes.Select(h => new HeroCombatInfo
        {
            Id = h.Id,
            Name = h.Name,
            Class = h.Class,
            Level = h.Level,
            Experience = h.Experience,
            Strength = h.Strength,
            Intelligence = h.Intelligence,
            Dexterity = h.Dexterity,
            Health = combatSession.GetHeroHealths().GetValueOrDefault(h.Id, 0),
            MaxHealth = combatSession.GetMaxHeroHealths().GetValueOrDefault(h.Id, 0),
            TotalAttack = CalculateAttackPower(h),
            TotalDefense = CalculateDefensePower(h),
            TotalMagic = h.Intelligence
        }).ToList();

        var enemyCombatInfo = new EnemyCombatInfo
        {
            Id = combatSession.CurrentEnemy.Id,
            Name = combatSession.CurrentEnemy.Name,
            Type = combatSession.CurrentEnemy.Type,
            Power = combatSession.CurrentEnemy.Power,
            Health = combatSession.CurrentEnemy.Health,
            RequiredDiceType = combatSession.CurrentEnemy.RequiredDiceType,
            MinimumRoll = combatSession.CurrentEnemy.MinimumRoll,
            CombatType = combatSession.CurrentEnemy.CombatType,
            IsBoss = combatSession.CurrentEnemy.IsBoss
        };

        var combatLogs = combatSession.CombatLogs
            .OrderByDescending(cl => cl.Timestamp)
            .Select(cl => new CombatLogDto
            {
                Id = cl.Id,
                Action = cl.Action,
                EnemyName = cl.Enemy?.Name ?? "",
                DiceUsed = cl.DiceUsed,
                DiceResult = cl.DiceResult,
                RequiredRoll = cl.RequiredRoll,
                Success = cl.Success,
                DamageDealt = cl.DamageDealt,
                EnemyHealthAfter = cl.EnemyHealthAfter,
                Details = cl.Details,
                Timestamp = cl.Timestamp
            }).ToList();

        return new CombatDetailDto
        {
            Id = combatSession.Id,
            UserId = combatSession.UserId,
            HeroIds = heroIds,
            Heroes = heroCombatInfos,
            QuestId = combatSession.QuestId,
            QuestName = combatSession.Quest.Name,
            CurrentEnemy = enemyCombatInfo,
            Status = combatSession.Status,
            IsHeroTurn = combatSession.IsHeroTurn,
            CurrentEnemyHealth = combatSession.CurrentEnemyHealth,
            MaxEnemyHealth = combatSession.MaxEnemyHealth,
            HeroHealths = combatSession.GetHeroHealths(),
            MaxHeroHealths = combatSession.GetMaxHeroHealths(),
            CombatLogs = combatLogs,
            CreatedAt = combatSession.CreatedAt,
            StartedAt = combatSession.StartedAt,
            CompletedAt = combatSession.CompletedAt
        };
    }

    private int CalculateMaxHealth(Hero hero)
    {
        var baseHealth = 100; // Vida base
        var armorBonus = 0;
        var accessoryBonus = 0;

        // Calcular bônus de equipamentos (se implementado)
        // Por enquanto, retorna vida base
        return baseHealth + armorBonus + accessoryBonus;
    }

    private int CalculateAttackPower(Hero hero)
    {
        var baseAttack = hero.Strength;
        var weaponBonus = 0; // Calcular bônus de armas (se implementado)
        return baseAttack + weaponBonus;
    }

    private int CalculateDefensePower(Hero hero)
    {
        var baseDefense = 0;
        var armorBonus = 0; // Calcular bônus de armaduras (se implementado)
        var accessoryBonus = 0; // Calcular bônus de acessórios (se implementado)
        return baseDefense + armorBonus + accessoryBonus;
    }

    private int GetDiceMaxValue(DiceType diceType)
    {
        return diceType switch
        {
            DiceType.D6 => 6,
            DiceType.D10 => 10,
            DiceType.D12 => 12,
            DiceType.D20 => 20,
            _ => 6
        };
    }
}

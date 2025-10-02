using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Combat;
using RpgQuestManager.Api.Models;
using System.Text.Json;

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

        // Verificar se quest tem monstro principal
        if (quest.MainMonsterId == 0)
        {
            throw new InvalidOperationException("Esta quest não possui um monstro principal definido.");
        }

        // Buscar heróis
        var heroes = await _context.Heroes
            .Where(h => request.HeroIds.Contains(h.Id) && h.UserId == userId && !h.IsDeleted)
            .ToListAsync();

        if (heroes.Count != request.HeroIds.Count)
        {
            throw new InvalidOperationException("Alguns heróis não foram encontrados ou não pertencem ao usuário.");
        }

        // Buscar monstro principal
        var mainMonster = await _context.Monsters
            .FirstOrDefaultAsync(m => m.Id == quest.MainMonsterId);

        if (mainMonster == null)
        {
            throw new InvalidOperationException("Monstro principal da quest não encontrado.");
        }

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
            CurrentEnemyId = mainMonster.Id,
            Status = CombatStatus.Preparing,
            IsHeroTurn = true,
            CurrentEnemyHealth = mainMonster.Health,
            MaxEnemyHealth = mainMonster.Health,
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
            Details = $"Combate iniciado contra {mainMonster.Name}",
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

    public async Task ClearActiveCombatAsync(int userId)
    {
        var activeCombat = await _context.CombatSessions
            .FirstOrDefaultAsync(cs => cs.UserId == userId && cs.Status == CombatStatus.InProgress);

        if (activeCombat != null)
        {
            activeCombat.Status = CombatStatus.Cancelled;
            activeCombat.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
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
            
            // Sistema de Combos
            UpdateComboSystem(combatSession, true);
            
            // Aplicar multiplicador de combo
            totalDamage = (int)(totalDamage * combatSession.ComboMultiplier);
            
            // Sistema de Críticos e Falhas
            var diceMaxValue = GetDiceMaxValue(diceType);
            var isCritical = roll == diceMaxValue;
            var isCriticalFailure = roll == 1;
            
            if (isCritical)
            {
                totalDamage = (int)(totalDamage * 2.0); // Crítico: dano dobrado
                message = $"🔥 CRÍTICO! Rolou {roll} (máximo)! Dano dobrado: {totalDamage}!";
                
                // Atualizar morale para crítico
                await UpdateMoraleForCritical(combatSession, true);
            }
            else if (isCriticalFailure)
            {
                totalDamage = (int)(totalDamage * 0.5); // Falha crítica: dano pela metade
                message = $"💥 FALHA CRÍTICA! Rolou {roll} (mínimo)! Dano reduzido: {totalDamage}!";
            }
            else
            {
                var comboText = combatSession.ComboMultiplier > 1 ? $" (Combo x{combatSession.ComboMultiplier})" : "";
                message = $"✅ SUCESSO! Rolou {roll}, precisava de {requiredRoll}. Causou {totalDamage} de dano!{comboText}";
            }
            
            // Atualizar morale para sucesso
            await UpdateMoraleForAction(combatSession, true, true);
            
            damageDealt = totalDamage;
            
            // Aplicar dano ao inimigo
            combatSession.CurrentEnemyHealth = Math.Max(0, combatSession.CurrentEnemyHealth - totalDamage);
            enemyHealthAfter = combatSession.CurrentEnemyHealth;

            // Verificar se inimigo morreu
            if (combatSession.CurrentEnemyHealth <= 0)
            {
                combatSession.Status = CombatStatus.Victory;
                combatSession.CompletedAt = DateTime.UtcNow;
                message += " Inimigo derrotado!";
                
                // Processar recompensas automaticamente quando inimigo morre
                await ProcessQuestRewardsAsync(combatSession);
            }
            else
            {
                // Passar vez para o inimigo
                combatSession.IsHeroTurn = false;
            }
        }
        else
        {
            // Sistema de Combos - falha reseta o combo
            UpdateComboSystem(combatSession, false);
            
            message = $"❌ FALHOU! Rolou {roll}, mas precisava de {requiredRoll}. Vez do inimigo!";
            combatSession.IsHeroTurn = false;
            
            // Atualizar morale para falha
            await UpdateMoraleForAction(combatSession, true, false);
        }

        // Processar status effects antes de finalizar o turno
        await ProcessStatusEffectsAsync(combatSession.Id);

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

        // Inimigo ataca - dados baseados na dificuldade
        var enemyDice = GetEnemyDiceByDifficulty(combatSession.CurrentEnemy);
        var enemyRoll = _random.Next(1, enemyDice + 1);
        var enemyPower = combatSession.CurrentEnemy.Power;
        var totalDamage = enemyPower + enemyRoll;
        
        // Aplicar status effects do inimigo
        await ApplyEnemyStatusEffects(combatSession);
        
        // Aplicar condições ambientais
        await ApplyEnvironmentalConditions(combatSession);
        
        // Aplicar sistema de morale
        await ApplyMoraleSystem(combatSession);
        
        // Atualizar morale baseado nas ações
        await UpdateMoraleForAction(combatSession, false, false); // Falha do herói
        
        // Sistema de Críticos para inimigo
        var isEnemyCritical = enemyRoll == enemyDice;
        var isEnemyCriticalFailure = enemyRoll == 1;
        
        if (isEnemyCritical)
        {
            totalDamage = (int)(totalDamage * 1.5); // Crítico do inimigo: +50% dano
            // Atualizar morale para crítico do inimigo
            await UpdateMoraleForCritical(combatSession, false);
        }
        else if (isEnemyCriticalFailure)
        {
            totalDamage = (int)(totalDamage * 0.7); // Falha crítica do inimigo: -30% dano
        }
        
        // Atualizar morale para ação do inimigo
        await UpdateMoraleForAction(combatSession, false, true);

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
            if (isEnemyCritical)
            {
                message = $"💀 DERROTA! {combatSession.CurrentEnemy.Name} desferiu um GOLPE CRÍTICO! Causou {finalDamage} de dano e derrotou todos os heróis!";
            }
            else if (isEnemyCriticalFailure)
            {
                message = $"💀 DERROTA! {combatSession.CurrentEnemy.Name} atacou com dificuldade, mas ainda causou {finalDamage} de dano e derrotou todos os heróis!";
            }
            else
            {
                message = $"💀 DERROTA! {combatSession.CurrentEnemy.Name} causou {finalDamage} de dano e derrotou todos os heróis!";
            }
        }
        else
        {
            combatSession.IsHeroTurn = true;
            if (isEnemyCritical)
            {
                message = $"🔥 {combatSession.CurrentEnemy.Name} desferiu um GOLPE CRÍTICO! Causou {finalDamage} de dano. Vez dos heróis!";
            }
            else if (isEnemyCriticalFailure)
            {
                message = $"💥 {combatSession.CurrentEnemy.Name} atacou com dificuldade, causou {finalDamage} de dano. Vez dos heróis!";
        }
        else
        {
                message = $"⚔️ {combatSession.CurrentEnemy.Name} atacou! Causou {finalDamage} de dano. Vez dos heróis!";
            }
        }

        // Processar status effects antes de finalizar o turno
        await ProcessStatusEffectsAsync(combatSession.Id);

        // Criar log
        var log = new CombatLog
        {
            CombatSessionId = combatSession.Id,
            EnemyId = combatSession.CurrentEnemyId,
            Action = "ENEMY_ATTACK",
            DiceUsed = (DiceType)enemyDice,
            DiceResult = enemyRoll,
            RequiredRoll = 0,
            Success = true,
            DamageDealt = finalDamage,
            Details = $"{combatSession.CurrentEnemy.Name} usou {GetEnemyDiceName(enemyDice)} e rolou {enemyRoll}! {(isEnemyCritical ? "CRÍTICO!" : isEnemyCriticalFailure ? "FALHA CRÍTICA!" : "")} {message}",
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
            .Include(cs => cs.Quest)
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId);

        if (combatSession == null)
        {
            throw new KeyNotFoundException("Sessão de combate não encontrada.");
        }

        combatSession.Status = CombatStatus.Victory;
        combatSession.CompletedAt = DateTime.UtcNow;

        // Processar recompensas da missão
        await ProcessQuestRewardsAsync(combatSession);

        await _context.SaveChangesAsync();

        return await GetCombatDetailAsync(combatSession.Id);
    }

    private async Task ProcessQuestRewardsAsync(CombatSession combatSession)
    {
        if (combatSession.Quest == null) 
        {
            Console.WriteLine("❌ ProcessQuestRewardsAsync: Quest é null!");
            return;
        }

        var heroIds = combatSession.GetHeroIdsList();
        Console.WriteLine($"🎯 ProcessQuestRewardsAsync: HeroIds = [{string.Join(", ", heroIds)}]");
        
        var heroes = await _context.Heroes
            .Where(h => heroIds.Contains(h.Id))
            .ToListAsync();
            
        Console.WriteLine($"🎯 ProcessQuestRewardsAsync: Encontrados {heroes.Count} heróis");

        // Calcular recompensas baseadas na missão
        var experienceReward = combatSession.Quest.ExperienceReward;
        var goldReward = combatSession.Quest.GoldReward;
        
        Console.WriteLine($"💰 ProcessQuestRewardsAsync: XP Reward = {experienceReward}, Gold Reward = {goldReward}");

        // Aplicar recompensas aos heróis
        var levelUpLogs = new List<string>();
        
        foreach (var hero in heroes)
        {
            var oldLevel = hero.Level;
            var oldExp = hero.Experience;
            var oldGold = hero.Gold;
            
            Console.WriteLine($"🦸 Processando herói {hero.Name}: Nível {oldLevel}, XP {oldExp}, Ouro {oldGold}");
            
            hero.Experience += experienceReward;
            hero.Gold += goldReward;
            
            Console.WriteLine($"🦸 Após recompensas: Nível {hero.Level}, XP {hero.Experience}, Ouro {hero.Gold}");

            // Verificar se o herói subiu de nível
            var newLevel = CalculateHeroLevel(hero.Experience);
            Console.WriteLine($"📊 Calculado novo nível: {newLevel} (atual: {hero.Level})");
            
            if (newLevel > hero.Level)
            {
                hero.Level = newLevel;
                
                // Subir atributos ao subir de nível (2 pontos por nível)
                var levelsGained = newLevel - oldLevel;
                var attributePoints = levelsGained * 2;
                
                var oldStrength = hero.Strength;
                var oldIntelligence = hero.Intelligence;
                var oldDexterity = hero.Dexterity;
                
                // Distribuir pontos aleatoriamente entre os atributos
                var attributes = new[] { "Strength", "Intelligence", "Dexterity" };
                for (int i = 0; i < attributePoints; i++)
                {
                    var randomAttribute = attributes[new Random().Next(attributes.Length)];
                    switch (randomAttribute)
                    {
                        case "Strength":
                            hero.Strength++;
                            break;
                        case "Intelligence":
                            hero.Intelligence++;
                            break;
                        case "Dexterity":
                            hero.Dexterity++;
                            break;
                    }
                }
                
                // Criar log detalhado de subida de nível
                var levelUpLog = new CombatLog
                {
                    CombatSessionId = combatSession.Id,
                    Action = "LEVEL_UP",
                    HeroId = hero.Id,
                    Details = $"{hero.Name} subiu do nível {oldLevel} para {newLevel}! " +
                             $"XP: {oldExp} → {hero.Experience} (+{experienceReward}). " +
                             $"Atributos: STR {oldStrength}→{hero.Strength}, " +
                             $"INT {oldIntelligence}→{hero.Intelligence}, " +
                             $"DEX {oldDexterity}→{hero.Dexterity}",
                    Timestamp = DateTime.UtcNow
                };
                
                _context.CombatLogs.Add(levelUpLog);
                levelUpLogs.Add($"{hero.Name} subiu para nível {newLevel}!");
            }
        }

        // Salvar mudanças dos heróis
        Console.WriteLine($"💾 Salvando mudanças de {heroes.Count} heróis no banco de dados...");
        _context.Heroes.UpdateRange(heroes);
        Console.WriteLine($"✅ Heróis atualizados com sucesso!");

        // Criar log de recompensas
        var rewardDetails = $"Recompensas da missão: {experienceReward} XP, {goldReward} Ouro";
        if (levelUpLogs.Any())
        {
            rewardDetails += $". Subidas de nível: {string.Join(", ", levelUpLogs)}";
        }
        
        var rewardLog = new CombatLog
        {
            CombatSessionId = combatSession.Id,
            Action = "QUEST_REWARDS",
            Details = rewardDetails,
            Timestamp = DateTime.UtcNow
        };

        _context.CombatLogs.Add(rewardLog);
    }

    private int CalculateHeroLevel(int experience)
    {
        // Fórmula progressiva: 100 XP para nível 2, 200 para nível 3, 300 para nível 4, etc.
        if (experience < 100) return 1;
        if (experience < 300) return 2;
        if (experience < 600) return 3;
        if (experience < 1000) return 4;
        if (experience < 1500) return 5;
        if (experience < 2100) return 6;
        if (experience < 2800) return 7;
        if (experience < 3600) return 8;
        if (experience < 4500) return 9;
        if (experience < 5500) return 10;
        
        // Para níveis acima de 10, usar fórmula: nível = sqrt(experiência / 50)
        return Math.Max(10, (int)Math.Sqrt(experience / 50.0));
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

        // Buscar estados de morale
        var heroMoraleStates = await _context.MoraleStates
            .Where(m => m.CombatSessionId == combatSession.Id && m.HeroId.HasValue)
            .ToListAsync();
            
        var enemyMoraleState = await _context.MoraleStates
            .FirstOrDefaultAsync(m => m.CombatSessionId == combatSession.Id && m.EnemyId.HasValue);

        // Buscar status effects
        var allStatusEffects = await _context.StatusEffects
            .Where(se => se.CombatSessionId == combatSession.Id)
            .ToListAsync();
            
        var heroStatusEffects = allStatusEffects.Where(se => se.HeroId.HasValue).ToList();
        var enemyStatusEffects = allStatusEffects.Where(se => se.EnemyId.HasValue).ToList();

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
            CompletedAt = combatSession.CompletedAt,
            
            // Sistema de Morale
            HeroMoraleStates = heroMoraleStates.Select(m => new MoraleStateDto
            {
                Id = m.Id,
                HeroId = m.HeroId,
                EnemyId = m.EnemyId,
                Level = m.Level.ToString(),
                MoralePoints = m.MoralePoints,
                Icon = m.GetIcon(),
                Description = m.GetDescription()
            }).ToList(),
            
            EnemyMoraleState = enemyMoraleState != null ? new MoraleStateDto
            {
                Id = enemyMoraleState.Id,
                HeroId = enemyMoraleState.HeroId,
                EnemyId = enemyMoraleState.EnemyId,
                Level = enemyMoraleState.Level.ToString(),
                MoralePoints = enemyMoraleState.MoralePoints,
                Icon = enemyMoraleState.GetIcon(),
                Description = enemyMoraleState.GetDescription()
            } : null,
            
            // Sistema de Combos
            ConsecutiveSuccesses = combatSession.ConsecutiveSuccesses,
            ConsecutiveFailures = combatSession.ConsecutiveFailures,
            ComboMultiplier = combatSession.ComboMultiplier,
            LastAction = combatSession.LastAction,
            
            // Sistema de Status Effects
            HeroStatusEffects = heroStatusEffects
                .Where(se => se.IsActive)
                .Select(se => new StatusEffectDto
                {
                    Id = se.Id,
                    CombatSessionId = se.CombatSessionId,
                    HeroId = se.HeroId,
                    EnemyId = se.EnemyId,
                    Type = se.Type.ToString(),
                    Duration = se.Duration,
                    Intensity = se.Intensity,
                    Description = se.Description,
                    IsActive = se.IsActive,
                    ExpiresAt = se.ExpiresAt ?? DateTime.UtcNow
                }).ToList(),
            
            EnemyStatusEffects = enemyStatusEffects
                .Where(se => se.IsActive)
                .Select(se => new StatusEffectDto
                {
                    Id = se.Id,
                    CombatSessionId = se.CombatSessionId,
                    HeroId = se.HeroId,
                    EnemyId = se.EnemyId,
                    Type = se.Type.ToString(),
                    Duration = se.Duration,
                    Intensity = se.Intensity,
                    Description = se.Description,
                    IsActive = se.IsActive,
                    ExpiresAt = se.ExpiresAt ?? DateTime.UtcNow
                }).ToList()
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
        var baseAttack = Math.Max(1, hero.Strength); // Mínimo de 1 de dano
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

    private int GetEnemyDiceByDifficulty(Enemy enemy)
    {
        // Sistema de dificuldade baseado no poder do inimigo
        return enemy.Power switch
        {
            <= 10 => 6,  // Inimigos fáceis: D6 (1-6)
            <= 20 => 8,  // Inimigos médios: D10 (1-8) - usando D8 para simular D10(1-8)
            <= 35 => 10, // Inimigos difíceis: D12 (1-10) - usando D10 para simular D12(1-10)
            <= 50 => 20, // Bosses: D12 (1-20) - usando D20 para simular D12(1-20)
            _ => 20      // Bosses extremos: D12 (1-20)
        };
    }

    private string GetEnemyDiceName(int diceValue)
    {
        return diceValue switch
        {
            6 => "D6 (1-6)",
            8 => "D10 (1-8)", 
            10 => "D12 (1-10)",
            20 => "D12 (1-20)",
            _ => "D6 (1-6)"
        };
    }

    public async Task<UseSpecialAbilityResult> UseSpecialAbilityAsync(int combatSessionId, int heroId)
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
            throw new InvalidOperationException("Não é a vez do herói usar habilidades.");
        }

        // Buscar herói
        var hero = await _context.Heroes.FindAsync(heroId);
        if (hero == null)
        {
            throw new KeyNotFoundException("Herói não encontrado.");
        }

        // Verificar cooldown
        var cooldowns = combatSession.GetHeroAbilityCooldowns();
        if (cooldowns.ContainsKey(heroId) && cooldowns[heroId] > 0)
        {
            throw new InvalidOperationException($"Habilidade em cooldown! Restam {cooldowns[heroId]} turnos.");
        }

        // Executar habilidade especial
        var abilityName = hero.GetSpecialAbility();
        var abilityDescription = hero.GetSpecialAbilityDescription();
        var cooldown = hero.GetSpecialAbilityCooldown();
        
        string message;
        int damageDealt = 0;
        int enemyHealthAfter = combatSession.CurrentEnemyHealth;
        int healingDone = 0;

        // Aplicar efeito da habilidade baseado na classe
        switch (hero.Class.ToLower())
        {
            case "guerreiro":
                // Investida Devastadora - 200% de dano
                var warriorDamage = (int)(CalculateAttackPower(hero) * 2.0);
                damageDealt = warriorDamage;
                combatSession.CurrentEnemyHealth = Math.Max(0, combatSession.CurrentEnemyHealth - warriorDamage);
                enemyHealthAfter = combatSession.CurrentEnemyHealth;
                message = $"⚔️ {hero.Name} usou {abilityName}! Causou {warriorDamage} de dano devastador!";
                break;

            case "mago":
                // Bola de Fogo - dano mágico em área + queimadura
                var mageDamage = (int)(CalculateAttackPower(hero) * 1.5);
                damageDealt = mageDamage;
                combatSession.CurrentEnemyHealth = Math.Max(0, combatSession.CurrentEnemyHealth - mageDamage);
                enemyHealthAfter = combatSession.CurrentEnemyHealth;
                
                // Aplicar queimadura ao inimigo
                await ApplyStatusEffectAsync(combatSession.Id, null, combatSession.CurrentEnemyId, StatusEffectType.Burning, 3, 2);
                
                message = $"🔥 {hero.Name} lançou {abilityName}! A bola de fogo causou {mageDamage} de dano mágico e queimou o inimigo!";
                break;

            case "arqueiro":
                // Chuva de Flechas - múltiplos ataques
                var archerDamage = (int)(CalculateAttackPower(hero) * 1.8);
                damageDealt = archerDamage;
                combatSession.CurrentEnemyHealth = Math.Max(0, combatSession.CurrentEnemyHealth - archerDamage);
                enemyHealthAfter = combatSession.CurrentEnemyHealth;
                message = $"🏹 {hero.Name} usou {abilityName}! Múltiplas flechas causaram {archerDamage} de dano!";
                break;

            case "clérigo":
                // Cura Divina - cura todos os heróis
                var heroHealths = combatSession.GetHeroHealths();
                var maxHeroHealths = combatSession.GetMaxHeroHealths();
                var totalHealing = 0;
                
                foreach (var heroIdInList in combatSession.GetHeroIdsList())
                {
                    if (heroHealths.ContainsKey(heroIdInList) && maxHeroHealths.ContainsKey(heroIdInList))
                    {
                        var maxHealth = maxHeroHealths[heroIdInList];
                        var currentHealth = heroHealths[heroIdInList];
                        var healAmount = (int)(maxHealth * 0.5); // 50% da vida máxima
                        var newHealth = Math.Min(maxHealth, currentHealth + healAmount);
                        var actualHealing = newHealth - currentHealth;
                        heroHealths[heroIdInList] = newHealth;
                        totalHealing += actualHealing;
                    }
                }
                
                combatSession.SetHeroHealths(heroHealths);
                healingDone = totalHealing;
                message = $"✨ {hero.Name} usou {abilityName}! Curou {totalHealing} pontos de vida de todos os heróis!";
                break;

            case "ladrão":
                // Ataque Furtivo - ignora defesa, dano crítico + sangramento
                var rogueDamage = (int)(CalculateAttackPower(hero) * 2.5);
                damageDealt = rogueDamage;
                combatSession.CurrentEnemyHealth = Math.Max(0, combatSession.CurrentEnemyHealth - rogueDamage);
                enemyHealthAfter = combatSession.CurrentEnemyHealth;
                
                // Aplicar sangramento ao inimigo
                await ApplyStatusEffectAsync(combatSession.Id, null, combatSession.CurrentEnemyId, StatusEffectType.Bleeding, 4, 3);
                
                message = $"🗡️ {hero.Name} usou {abilityName}! Ataque furtivo causou {rogueDamage} de dano crítico e causou sangramento!";
                break;

            case "paladino":
                // Golpe Sagrado - dano extra + bênção para heróis
                var paladinDamage = (int)(CalculateAttackPower(hero) * 1.8);
                damageDealt = paladinDamage;
                combatSession.CurrentEnemyHealth = Math.Max(0, combatSession.CurrentEnemyHealth - paladinDamage);
                enemyHealthAfter = combatSession.CurrentEnemyHealth;
                
                // Aplicar bênção a todos os heróis
                foreach (var heroIdInList in combatSession.GetHeroIdsList())
                {
                    await ApplyStatusEffectAsync(combatSession.Id, heroIdInList, null, StatusEffectType.Blessed, 5, 2);
                }
                
                message = $"✨ {hero.Name} usou {abilityName}! Golpe sagrado causou {paladinDamage} de dano e abençoou todos os heróis!";
                break;

            case "bárbaro":
                // Fúria Berserker - dano + efeito berserker
                var barbarianDamage = (int)(CalculateAttackPower(hero) * 2.2);
                damageDealt = barbarianDamage;
                combatSession.CurrentEnemyHealth = Math.Max(0, combatSession.CurrentEnemyHealth - barbarianDamage);
                enemyHealthAfter = combatSession.CurrentEnemyHealth;
                
                // Aplicar fúria berserker ao herói
                await ApplyStatusEffectAsync(combatSession.Id, heroId, null, StatusEffectType.Berserker, 6, 3);
                
                message = $"😡 {hero.Name} entrou em {abilityName}! Causou {barbarianDamage} de dano e entrou em fúria!";
                break;

            default:
                // Habilidade padrão - 150% de dano
                var defaultDamage = (int)(CalculateAttackPower(hero) * 1.5);
                damageDealt = defaultDamage;
                combatSession.CurrentEnemyHealth = Math.Max(0, combatSession.CurrentEnemyHealth - defaultDamage);
                enemyHealthAfter = combatSession.CurrentEnemyHealth;
                message = $"💫 {hero.Name} usou {abilityName}! Causou {defaultDamage} de dano especial!";
                break;
        }

        // Verificar se inimigo morreu
        if (combatSession.CurrentEnemyHealth <= 0)
        {
            combatSession.Status = CombatStatus.Victory;
            combatSession.CompletedAt = DateTime.UtcNow;
            message += " Inimigo derrotado!";
            
            // Processar recompensas automaticamente quando inimigo morre
            await ProcessQuestRewardsAsync(combatSession);
        }
        else
        {
            // Passar vez para o inimigo
            combatSession.IsHeroTurn = false;
        }

        // Definir cooldown
        cooldowns[heroId] = cooldown;
        combatSession.SetHeroAbilityCooldowns(cooldowns);
        
        // Atualizar morale para habilidade especial
        await UpdateMoraleForSpecialAbility(combatSession, heroId);

        // Criar log
        var log = new CombatLog
        {
            CombatSessionId = combatSession.Id,
            Action = "SPECIAL_ABILITY",
            DiceUsed = null,
            DiceResult = null,
            RequiredRoll = null,
            Success = true,
            DamageDealt = damageDealt,
            EnemyHealthAfter = enemyHealthAfter,
            Details = message,
            Timestamp = DateTime.UtcNow
        };
        _context.CombatLogs.Add(log);

        await _context.SaveChangesAsync();

        return new UseSpecialAbilityResult
        {
            Success = true,
            Message = message,
            DamageDealt = damageDealt,
            HealingDone = healingDone,
            CooldownRemaining = cooldown,
            UpdatedCombatSession = await GetCombatDetailAsync(combatSession.Id)
        };
    }

    // Sistema de Status Effects
    public async Task<StatusEffectResult> ApplyStatusEffectAsync(int combatSessionId, int? heroId, int? enemyId, StatusEffectType effectType, int duration, int intensity = 1)
    {
        var combatSession = await _context.CombatSessions.FindAsync(combatSessionId);
        if (combatSession == null)
        {
            throw new KeyNotFoundException("Sessão de combate não encontrada.");
        }

        // Verificar se já existe o mesmo efeito
        var allEffects = await _context.StatusEffects
            .Where(se => se.CombatSessionId == combatSessionId && 
                        se.HeroId == heroId && 
                        se.EnemyId == enemyId && 
                        se.Type == effectType)
            .ToListAsync();
            
        var existingEffect = allEffects.FirstOrDefault(se => se.IsActive);

        if (existingEffect != null)
        {
            // Renovar duração se for mais longa
            if (duration > existingEffect.Duration)
            {
                existingEffect.Duration = duration;
                existingEffect.Intensity = Math.Max(existingEffect.Intensity, intensity);
                existingEffect.ExpiresAt = DateTime.UtcNow.AddMinutes(duration * 2); // 2 minutos por turno
            }
        }
        else
        {
            // Criar novo efeito
            var statusEffect = new StatusEffect
            {
                CombatSessionId = combatSessionId,
                HeroId = heroId,
                EnemyId = enemyId,
                Type = effectType,
                Duration = duration,
                Intensity = intensity,
                Description = GetStatusEffectDescription(effectType, intensity),
                ExpiresAt = DateTime.UtcNow.AddMinutes(duration * 2)
            };

            _context.StatusEffects.Add(statusEffect);
        }

        await _context.SaveChangesAsync();

        return new StatusEffectResult
        {
            Success = true,
            Message = $"Efeito aplicado: {GetStatusEffectDescription(effectType, intensity)}",
            EffectType = effectType,
            Duration = duration,
            Intensity = intensity
        };
    }

    public async Task ProcessStatusEffectsAsync(int combatSessionId)
    {
        var allEffects = await _context.StatusEffects
            .Where(se => se.CombatSessionId == combatSessionId)
            .ToListAsync();
            
        var activeEffects = allEffects.Where(se => se.IsActive).ToList();

        var combatSession = await _context.CombatSessions
            .Include(cs => cs.Quest)
            .Include(cs => cs.CurrentEnemy)
            .FirstOrDefaultAsync(cs => cs.Id == combatSessionId);

        if (combatSession == null) return;

        foreach (var effect in activeEffects)
        {
            await ProcessStatusEffectAsync(combatSession, effect);
        }

        // Reduzir duração de todos os efeitos
        foreach (var effect in activeEffects)
        {
            effect.Duration = Math.Max(0, effect.Duration - 1);
            if (effect.Duration <= 0)
            {
                effect.ExpiresAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task ProcessStatusEffectAsync(CombatSession combatSession, StatusEffect effect)
    {
        var damage = 0;
        string message = "";

        switch (effect.Type)
        {
            case StatusEffectType.Poisoned:
                damage = 2 * effect.Intensity;
                message = $"☠️ Envenenamento causou {damage} de dano!";
                break;

            case StatusEffectType.Burning:
                damage = 3 * effect.Intensity;
                message = $"🔥 Queimadura causou {damage} de dano!";
                break;

            case StatusEffectType.Bleeding:
                damage = 1 * effect.Intensity;
                message = $"🩸 Sangramento causou {damage} de dano!";
                break;

            case StatusEffectType.Frozen:
                // Congelado pula o turno
                if (effect.HeroId.HasValue)
                {
                    combatSession.IsHeroTurn = false; // Pula turno do herói
                }
                message = $"❄️ Congelado! Turno pulado!";
                break;

            case StatusEffectType.Berserker:
                // Berserker aumenta dano do inimigo
                if (effect.EnemyId.HasValue)
                {
                    message = $"😡 {combatSession.CurrentEnemy?.Name} entrou em fúria! Dano aumentado!";
                }
                else if (effect.HeroId.HasValue)
                {
                    message = $"😡 Herói em fúria! Dano aumentado!";
                }
                break;

            case StatusEffectType.Shielded:
                // Escudo aumenta defesa
                if (effect.EnemyId.HasValue)
                {
                    message = $"🛡️ {combatSession.CurrentEnemy?.Name} se protegeu! Defesa aumentada!";
                }
                else if (effect.HeroId.HasValue)
                {
                    message = $"🛡️ Herói protegido! Defesa aumentada!";
                }
                break;

            case StatusEffectType.Blessed:
                // Bênção aumenta dano e defesa
                if (effect.HeroId.HasValue)
                {
                    message = $"✨ Herói abençoado! Poderes aumentados!";
                }
                break;

            case StatusEffectType.Weakened:
                // Enfraquecido reduz dano
                if (effect.HeroId.HasValue)
                {
                    message = $"😵 Herói enfraquecido! Dano reduzido!";
                }
                break;

            case StatusEffectType.Strengthened:
                // Fortalecido aumenta dano
                if (effect.HeroId.HasValue)
                {
                    message = $"💪 Herói fortalecido! Dano aumentado!";
                }
                break;
        }

        // Aplicar dano ou cura
        if (damage > 0)
        {
            if (effect.HeroId.HasValue)
            {
                // Dano ao herói
                var heroHealths = combatSession.GetHeroHealths();
                if (heroHealths.ContainsKey(effect.HeroId.Value))
                {
                    heroHealths[effect.HeroId.Value] = Math.Max(0, heroHealths[effect.HeroId.Value] - damage);
                    combatSession.SetHeroHealths(heroHealths);
                }
            }
            else if (effect.EnemyId.HasValue)
            {
                // Dano ao inimigo
                combatSession.CurrentEnemyHealth = Math.Max(0, combatSession.CurrentEnemyHealth - damage);
            }
        }

        // Criar log do efeito
        if (!string.IsNullOrEmpty(message))
        {
            var log = new CombatLog
        {
            CombatSessionId = combatSession.Id,
                HeroId = effect.HeroId,
                EnemyId = effect.EnemyId,
                Action = "STATUS_EFFECT",
                Details = message,
            Timestamp = DateTime.UtcNow
        };
            _context.CombatLogs.Add(log);
        }
    }

    private string GetStatusEffectDescription(StatusEffectType effectType, int intensity)
    {
        return effectType switch
        {
            StatusEffectType.Poisoned => $"Envenenado (Intensidade {intensity})",
            StatusEffectType.Burning => $"Queimado (Intensidade {intensity})",
            StatusEffectType.Frozen => $"Congelado (Intensidade {intensity})",
            StatusEffectType.Bleeding => $"Sangrando (Intensidade {intensity})",
            StatusEffectType.Berserker => $"Berserker (Intensidade {intensity})",
            StatusEffectType.Blessed => $"Abençoado (Intensidade {intensity})",
            StatusEffectType.Shielded => $"Protegido (Intensidade {intensity})",
            StatusEffectType.Weakened => $"Enfraquecido (Intensidade {intensity})",
            StatusEffectType.Strengthened => $"Fortalecido (Intensidade {intensity})",
            _ => "Efeito desconhecido"
        };
    }

    // Sistema de Combos
    private void UpdateComboSystem(CombatSession combatSession, bool success)
    {
        if (success)
        {
            // Sucesso consecutivo
            combatSession.ConsecutiveSuccesses++;
            combatSession.ConsecutiveFailures = 0;
            
            // Calcular multiplicador de combo
            if (combatSession.ConsecutiveSuccesses >= 3)
            {
                combatSession.ComboMultiplier = 2; // Combo Devastador
            }
            else if (combatSession.ConsecutiveSuccesses >= 5)
            {
                combatSession.ComboMultiplier = 3; // Fúria Berserker
            }
            else if (combatSession.ConsecutiveSuccesses >= 7)
            {
                combatSession.ComboMultiplier = 4; // Combo Épico
            }
            else
            {
                combatSession.ComboMultiplier = 1;
            }
        }
        else
        {
            // Falha reseta o combo
            combatSession.ConsecutiveSuccesses = 0;
            combatSession.ConsecutiveFailures++;
            combatSession.ComboMultiplier = 1;
        }
        
        combatSession.LastAction = success ? "SUCCESS" : "FAILURE";
    }

    // Sistema de Status Effects para Inimigos
    private async Task ApplyEnemyStatusEffects(CombatSession combatSession)
    {
        var enemy = combatSession.CurrentEnemy;
        if (enemy == null) return;

        // Determinar status effects baseado no tipo de inimigo
        var enemyType = enemy.Type.ToLower();
        var enemyName = enemy.Name.ToLower();
        
        // Aplicar status effects baseado no tipo de inimigo
        switch (enemyType)
        {
            case "goblin":
                // Goblins podem envenenar com suas armas sujas
                if (_random.Next(1, 101) <= 20) // 20% de chance
                {
                    var targetHeroId = GetRandomHeroId(combatSession);
                    if (targetHeroId.HasValue)
                    {
                        await ApplyStatusEffectAsync(combatSession.Id, targetHeroId, null, StatusEffectType.Poisoned, 2, 1);
                    }
                }
                break;

            case "orc":
                // Orcs podem causar sangramento com suas garras
                if (_random.Next(1, 101) <= 25) // 25% de chance
                {
                    var targetHeroId = GetRandomHeroId(combatSession);
                    if (targetHeroId.HasValue)
                    {
                        await ApplyStatusEffectAsync(combatSession.Id, targetHeroId, null, StatusEffectType.Bleeding, 3, 2);
                    }
                }
                break;

            case "troll":
                // Trolls podem enfraquecer com seus golpes brutais
                if (_random.Next(1, 101) <= 30) // 30% de chance
                {
                    var targetHeroId = GetRandomHeroId(combatSession);
                    if (targetHeroId.HasValue)
                    {
                        await ApplyStatusEffectAsync(combatSession.Id, targetHeroId, null, StatusEffectType.Weakened, 4, 2);
                    }
                }
                break;

            case "dragão":
            case "dragon":
                // Dragões podem queimar com seu fogo
                if (_random.Next(1, 101) <= 40) // 40% de chance
                {
                    var targetHeroId = GetRandomHeroId(combatSession);
                    if (targetHeroId.HasValue)
                    {
                        await ApplyStatusEffectAsync(combatSession.Id, targetHeroId, null, StatusEffectType.Burning, 5, 3);
                    }
                }
                break;

            case "lobo":
            case "wolf":
                // Lobos podem causar sangramento com suas presas
                if (_random.Next(1, 101) <= 35) // 35% de chance
                {
                    var targetHeroId = GetRandomHeroId(combatSession);
                    if (targetHeroId.HasValue)
                    {
                        await ApplyStatusEffectAsync(combatSession.Id, targetHeroId, null, StatusEffectType.Bleeding, 2, 1);
                    }
                }
                break;

            case "esqueleto":
            case "skeleton":
                // Esqueletos podem congelar com seu toque gélido
                if (_random.Next(1, 101) <= 25) // 25% de chance
                {
                    var targetHeroId = GetRandomHeroId(combatSession);
                    if (targetHeroId.HasValue)
                    {
                        await ApplyStatusEffectAsync(combatSession.Id, targetHeroId, null, StatusEffectType.Frozen, 1, 1);
                    }
                }
                break;

            case "mago":
            case "wizard":
                // Magos podem enfraquecer com magia
                if (_random.Next(1, 101) <= 30) // 30% de chance
                {
                    var targetHeroId = GetRandomHeroId(combatSession);
                    if (targetHeroId.HasValue)
                    {
                        await ApplyStatusEffectAsync(combatSession.Id, targetHeroId, null, StatusEffectType.Weakened, 3, 2);
                    }
                }
                break;

            case "demônio":
            case "demon":
                // Demônios podem queimar e enfraquecer
                if (_random.Next(1, 101) <= 35) // 35% de chance
                {
                    var targetHeroId = GetRandomHeroId(combatSession);
                    if (targetHeroId.HasValue)
                    {
                        var effectType = _random.Next(1, 3) == 1 ? StatusEffectType.Burning : StatusEffectType.Weakened;
                        await ApplyStatusEffectAsync(combatSession.Id, targetHeroId, null, effectType, 4, 2);
                    }
                }
                break;
        }

        // Aplicar status effects baseado no nome do inimigo (para casos específicos)
        if (enemyName.Contains("venenoso") || enemyName.Contains("poison"))
        {
            if (_random.Next(1, 101) <= 50) // 50% de chance
            {
                var targetHeroId = GetRandomHeroId(combatSession);
                if (targetHeroId.HasValue)
                {
                    await ApplyStatusEffectAsync(combatSession.Id, targetHeroId, null, StatusEffectType.Poisoned, 3, 2);
                }
            }
        }
        else if (enemyName.Contains("fogo") || enemyName.Contains("fire"))
        {
            if (_random.Next(1, 101) <= 45) // 45% de chance
            {
                var targetHeroId = GetRandomHeroId(combatSession);
                if (targetHeroId.HasValue)
                {
                    await ApplyStatusEffectAsync(combatSession.Id, targetHeroId, null, StatusEffectType.Burning, 4, 2);
                }
            }
        }
        else if (enemyName.Contains("gelo") || enemyName.Contains("ice"))
        {
            if (_random.Next(1, 101) <= 40) // 40% de chance
            {
                var targetHeroId = GetRandomHeroId(combatSession);
                if (targetHeroId.HasValue)
                {
                    await ApplyStatusEffectAsync(combatSession.Id, targetHeroId, null, StatusEffectType.Frozen, 2, 1);
                }
            }
        }
        else if (enemyName.Contains("berserker") || enemyName.Contains("furioso"))
        {
            // Inimigos berserker podem se fortalecer
            if (_random.Next(1, 101) <= 30) // 30% de chance
            {
                await ApplyStatusEffectAsync(combatSession.Id, null, enemy.Id, StatusEffectType.Berserker, 3, 2);
            }
        }
        else if (enemyName.Contains("guardião") || enemyName.Contains("guardian"))
        {
            // Guardiões podem se proteger
            if (_random.Next(1, 101) <= 35) // 35% de chance
            {
                await ApplyStatusEffectAsync(combatSession.Id, null, enemy.Id, StatusEffectType.Shielded, 4, 2);
            }
        }
    }

    private int? GetRandomHeroId(CombatSession combatSession)
    {
        var heroIds = combatSession.GetHeroIdsList();
        if (heroIds.Count == 0) return null;
        
        var randomIndex = _random.Next(0, heroIds.Count);
        return heroIds[randomIndex];
    }

    // Sistema de Condições Ambientais
    private async Task ApplyEnvironmentalConditions(CombatSession combatSession)
    {
        var quest = await _context.Quests.FindAsync(combatSession.QuestId);
        if (quest?.EnvironmentalCondition == null) return;

        var condition = quest.EnvironmentalCondition.Value;
        var intensity = quest.EnvironmentalIntensity;
        
        // Verificar se o inimigo é imune (nativo do habitat)
        var enemyType = combatSession.CurrentEnemy?.Type?.ToLower();
        var immuneEnemyTypes = GetImmuneEnemyTypes(quest);
        
        if (!string.IsNullOrEmpty(enemyType) && immuneEnemyTypes.Contains(enemyType))
        {
            // Inimigo é imune às condições ambientais
            return;
        }

        // Aplicar modificadores ambientais
        var modifiers = GetEnvironmentalModifiers(condition, intensity);
        
        // Aplicar dano ambiental aos heróis
        if (modifiers.ContainsKey("environmental_damage"))
        {
            var damage = (int)(modifiers["environmental_damage"] * intensity);
            var heroHealths = combatSession.GetHeroHealths();
            
            foreach (var heroId in combatSession.GetHeroIdsList())
            {
                if (heroHealths.ContainsKey(heroId))
                {
                    heroHealths[heroId] = Math.Max(0, heroHealths[heroId] - damage);
                }
            }
            
            combatSession.SetHeroHealths(heroHealths);
            
            // Criar log do dano ambiental
            var log = new CombatLog
            {
                CombatSessionId = combatSession.Id,
                Action = "ENVIRONMENTAL_DAMAGE",
                Details = $"🌍 Condição ambiental causou {damage} de dano!",
                Timestamp = DateTime.UtcNow
            };
            _context.CombatLogs.Add(log);
        }
    }

    private List<string> GetImmuneEnemyTypes(Quest quest)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(quest.ImmuneEnemyTypes) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private Dictionary<string, float> GetEnvironmentalModifiers(EnvironmentalConditionType condition, int intensity)
    {
        var modifiers = new Dictionary<string, float>();
        
        switch (condition)
        {
            case EnvironmentalConditionType.Rain:
                modifiers["accuracy"] = -0.10f * intensity;
                modifiers["water_damage"] = 0.15f * intensity;
                break;
                
            case EnvironmentalConditionType.Snow:
                modifiers["speed"] = -0.20f * intensity;
                modifiers["ice_damage"] = 0.20f * intensity;
                modifiers["environmental_damage"] = 2f * intensity; // Dano de frio
                break;
                
            case EnvironmentalConditionType.Desert:
                modifiers["fire_damage"] = 0.10f * intensity;
                modifiers["max_health"] = -0.15f * intensity;
                modifiers["environmental_damage"] = 1f * intensity; // Dano de calor
                break;
                
            case EnvironmentalConditionType.Forest:
                modifiers["stealth"] = 0.15f * intensity;
                modifiers["healing"] = 0.10f * intensity;
                break;
                
            case EnvironmentalConditionType.Night:
                modifiers["stealth"] = 0.25f * intensity;
                modifiers["accuracy"] = -0.10f * intensity;
                break;
                
            case EnvironmentalConditionType.Storm:
                modifiers["accuracy"] = -0.30f * intensity;
                modifiers["lightning_damage"] = 0.25f * intensity;
                modifiers["environmental_damage"] = 3f * intensity; // Dano de raio
                break;
                
            case EnvironmentalConditionType.Fog:
                modifiers["accuracy"] = -0.25f * intensity;
                modifiers["stealth"] = 0.20f * intensity;
                break;
                
            case EnvironmentalConditionType.Heat:
                modifiers["defense"] = -0.10f * intensity;
                modifiers["fire_damage"] = 0.15f * intensity;
                modifiers["environmental_damage"] = 2f * intensity; // Dano de calor
                break;
                
            case EnvironmentalConditionType.Cold:
                modifiers["speed"] = -0.15f * intensity;
                modifiers["defense"] = 0.20f * intensity;
                modifiers["environmental_damage"] = 1f * intensity; // Dano de frio
                break;
        }
        
        return modifiers;
    }

    // Sistema de Morale
    private async Task ApplyMoraleSystem(CombatSession combatSession)
    {
        // Inicializar morale para heróis se não existir
        foreach (var heroId in combatSession.GetHeroIdsList())
        {
            var existingMorale = await _context.MoraleStates
                .FirstOrDefaultAsync(m => m.CombatSessionId == combatSession.Id && m.HeroId == heroId);
                
            if (existingMorale == null)
            {
                var morale = new MoraleState
                {
                    CombatSessionId = combatSession.Id,
                    HeroId = heroId,
                    Level = MoraleLevel.Normal,
                    MoralePoints = 50
                };
                _context.MoraleStates.Add(morale);
            }
        }

        // Inicializar morale para inimigo se não existir
        if (combatSession.CurrentEnemy != null)
        {
            var existingEnemyMorale = await _context.MoraleStates
                .FirstOrDefaultAsync(m => m.CombatSessionId == combatSession.Id && m.EnemyId == combatSession.CurrentEnemy.Id);
                
            if (existingEnemyMorale == null)
            {
                var enemyMorale = new MoraleState
        {
            CombatSessionId = combatSession.Id,
                    EnemyId = combatSession.CurrentEnemy.Id,
                    Level = MoraleLevel.Normal,
                    MoralePoints = 50
                };
                _context.MoraleStates.Add(enemyMorale);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateMoraleAsync(int combatSessionId, int? heroId, int? enemyId, MoraleEventType eventType)
    {
        var morale = await _context.MoraleStates
            .FirstOrDefaultAsync(m => m.CombatSessionId == combatSessionId && 
                                    m.HeroId == heroId && 
                                    m.EnemyId == enemyId);
                                    
        if (morale != null)
        {
            morale.ApplyMoraleEvent(eventType);
            await _context.SaveChangesAsync();
        }
    }

    // Atualizar morale baseado nas ações do combate
    private async Task UpdateMoraleForAction(CombatSession combatSession, bool isHeroAction, bool isSuccess)
    {
        if (isHeroAction)
        {
            // Ação dos heróis
            foreach (var heroId in combatSession.GetHeroIdsList())
            {
                var morale = await _context.MoraleStates
                    .FirstOrDefaultAsync(m => m.CombatSessionId == combatSession.Id && m.HeroId == heroId);
                    
                if (morale != null)
                {
                    if (isSuccess)
                    {
                        morale.ApplyMoraleEvent(MoraleEventType.SuccessfulAttack);
                    }
                    else
                    {
                        morale.ApplyMoraleEvent(MoraleEventType.TakeDamage);
                    }
                }
            }
        }
        else
        {
            // Ação do inimigo
            if (combatSession.CurrentEnemy != null)
            {
                var enemyMorale = await _context.MoraleStates
                    .FirstOrDefaultAsync(m => m.CombatSessionId == combatSession.Id && m.EnemyId == combatSession.CurrentEnemy.Id);
                    
                if (enemyMorale != null)
                {
                    if (isSuccess)
                    {
                        enemyMorale.ApplyMoraleEvent(MoraleEventType.SuccessfulAttack);
                    }
                    else
                    {
                        enemyMorale.ApplyMoraleEvent(MoraleEventType.TakeDamage);
                    }
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    // Atualizar morale para críticos
    private async Task UpdateMoraleForCritical(CombatSession combatSession, bool isHeroCritical)
    {
        if (isHeroCritical)
        {
            foreach (var heroId in combatSession.GetHeroIdsList())
            {
                await UpdateMoraleAsync(combatSession.Id, heroId, null, MoraleEventType.CriticalHit);
            }
        }
        else
        {
            if (combatSession.CurrentEnemy != null)
            {
                await UpdateMoraleAsync(combatSession.Id, null, combatSession.CurrentEnemy.Id, MoraleEventType.CriticalHit);
            }
        }
    }

    // Atualizar morale para habilidades especiais
    private async Task UpdateMoraleForSpecialAbility(CombatSession combatSession, int heroId)
    {
        await UpdateMoraleAsync(combatSession.Id, heroId, null, MoraleEventType.SpecialAbility);
    }
}

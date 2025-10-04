using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class CombatService : ICombatService
{
    private readonly ApplicationDbContext _db;
    private readonly DiceService _diceService;
    private readonly MoraleService _moraleService;
    private readonly StatusEffectService _statusEffectService;
    private readonly LevelUpService _levelUpService;
    private readonly ISpecialAbilityService _specialAbilityService;
    private readonly IAchievementService _achievementService;
    private readonly QuestService _questService;
    private readonly DropService _dropService;
    private readonly RewardService _rewardService;
    private readonly InventoryService _inventoryService;

    public CombatService(
        ApplicationDbContext db, 
        DiceService diceService, 
        MoraleService moraleService, 
        StatusEffectService statusEffectService,
        LevelUpService levelUpService,
        ISpecialAbilityService specialAbilityService,
        IAchievementService achievementService,
        QuestService questService,
        DropService dropService,
        RewardService rewardService,
        InventoryService inventoryService)
    {
        _db = db;
        _diceService = diceService;
        _moraleService = moraleService;
        _statusEffectService = statusEffectService;
        _levelUpService = levelUpService;
        _specialAbilityService = specialAbilityService;
        _achievementService = achievementService;
        _questService = questService;
        _dropService = dropService;
        _rewardService = rewardService;
        _inventoryService = inventoryService;
    }

    public async Task<CombatResult> StartCombatAsync(int heroId, int monsterId)
    {
        var hero = await _db.Heroes.FirstOrDefaultAsync(x => x.Id == heroId) ?? throw new InvalidOperationException("Hero not found");
        var monster = await _db.Monsters.FirstOrDefaultAsync(x => x.Id == monsterId) ?? throw new InvalidOperationException("Monster not found");

        // Aplicar efeitos de status iniciais
        await _statusEffectService.ProcessStatusEffectsAsync(EffectTargetKind.Character, hero.Id);
        await _statusEffectService.ProcessStatusEffectsAsync(EffectTargetKind.Monster, monster.Id);

        var heroMoraleLevel = _moraleService.GetMoraleLevel(hero.Morale);
        
        return new CombatResult(
            hero, monster, 0, 0, false, false, false, false, 0, 
            "Combate iniciado!", new List<StatusEffectType>(), heroMoraleLevel, 0);
    }

    private async Task CompleteActiveQuestAsync(int heroId)
    {
        try
        {
            var activeQuest = await _questService.GetActiveQuestAsync(heroId);
            if (activeQuest != null)
            {
                await _questService.CompleteQuestAsync(activeQuest.Id, heroId);
                
                // Gerar recompensas de missão
                await _rewardService.GenerateQuestRewardsAsync(heroId, activeQuest.Id);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the combat
            Console.WriteLine($"Erro ao completar missão: {ex.Message}");
        }
    }

    public async Task<CombatResult> AttackAsync(int heroId, int monsterId)
    {
        var hero = await _db.Heroes.FirstOrDefaultAsync(x => x.Id == heroId) ?? throw new InvalidOperationException("Hero not found");
        var monster = await _db.Monsters.FirstOrDefaultAsync(x => x.Id == monsterId) ?? throw new InvalidOperationException("Monster not found");
        
        var heroEffects = await _statusEffectService.GetActiveEffectsAsync(EffectTargetKind.Hero, hero.Id);
        var monsterEffects = await _statusEffectService.GetActiveEffectsAsync(EffectTargetKind.Monster, monster.Id);

        // Processar efeitos de status no início do turno
        await ProcessStatusEffectsAtTurnStart(hero, monster, heroEffects, monsterEffects);

        // Verificar se herói está atordoado
        if (heroEffects.Any(e => e.Type == StatusEffectType.Stunned))
        {
            await _statusEffectService.ProcessStatusEffectsAsync(EffectTargetKind.Character, hero.Id);
            await _statusEffectService.ProcessStatusEffectsAsync(EffectTargetKind.Monster, monster.Id);
            
            var stunnedMoraleLevel = _moraleService.GetMoraleLevel(hero.Morale);
            return new CombatResult(hero, monster, 0, 0, false, false, false, false, 0, 
                "Herói está atordoado e perdeu o turno!", new List<StatusEffectType>(), stunnedMoraleLevel, 0);
        }

        // Calcular dano do herói
        var (damageToMonster, isCritical, isFumble, _) = await CalculateHeroDamage(hero, monster, heroEffects, monsterEffects);
        
        // Aplicar dano ao monstro
        monster.Health = Math.Max(0, monster.Health - damageToMonster);
        
        // Verificar se monstro morreu
        bool combatEnded = monster.Health <= 0;
        bool victory = combatEnded;
        int experienceGained = 0;
        
        int goldReward = 0;
        List<Item>? droppedItems = null;
        if (victory)
        {
            // Calcular e aplicar recompensas
            experienceGained = _levelUpService.CalculateExperienceReward(monster, hero.Level);
            goldReward = await _dropService.CalculateGoldRewardAsync(monster.Id, hero.Level);
            
            // Aplicar XP e verificar level up
            hero.Experience += experienceGained;
            await _levelUpService.CheckAndProcessLevelUpAsync(hero);
            
            // Aplicar ouro
            hero.Gold += goldReward;
            
            // Gerar drops de itens
            droppedItems = await _dropService.RollMonsterDropsAsync(monster.Id, hero.Level);
            foreach (var item in droppedItems)
            {
                await _inventoryService.AddItemAsync(hero.Id, item.Id, 1);
            }
            
            // Ajustar moral por vitória
            hero.Morale = _moraleService.AdjustMorale(hero.Morale, MoraleEvent.Victory);
            
            // Trigger achievements (notificações são criadas automaticamente)
            await _achievementService.UpdateAchievementProgressAsync(hero.UserId ?? 0, AchievementType.Combat, 1);
            
            // Gerar recompensas de combate (baseadas nos drops reais)
            var activeQuest = await _questService.GetActiveQuestAsync(hero.Id);
            if (activeQuest != null)
            {
                await _rewardService.GenerateCombatRewardsAsync(hero.Id, activeQuest.Id, monster.Id);
            }
            
            // Completar missão ativa se houver (gera recompensas de missão)
            await CompleteActiveQuestAsync(hero.Id);
        }
        else
        {
            // Monstro ataca de volta
            var (damageToHero, monsterAction) = await CalculateMonsterDamage(hero, monster, heroEffects, monsterEffects);
            hero.Health = Math.Max(0, hero.Health - damageToHero);
            
            // Verificar se herói morreu
            if (hero.Health <= 0)
            {
                combatEnded = true;
                victory = false;
                hero.Morale = _moraleService.AdjustMorale(hero.Morale, MoraleEvent.Death);
            }
            else if (damageToHero > 0)
            {
                hero.Morale = _moraleService.AdjustMorale(hero.Morale, MoraleEvent.TakeDamage);
            }
        }

        // Ajustar moral baseado em crítico/falha
        if (isCritical) hero.Morale = _moraleService.AdjustMorale(hero.Morale, MoraleEvent.CriticalHit);
        if (isFumble) hero.Morale = _moraleService.AdjustMorale(hero.Morale, MoraleEvent.AttackMiss);

        // Processar efeitos de status no fim do turno
        await _statusEffectService.ProcessStatusEffectsAsync(EffectTargetKind.Character, hero.Id);
        await _statusEffectService.ProcessStatusEffectsAsync(EffectTargetKind.Monster, monster.Id);

        await _db.SaveChangesAsync();

        var heroMoraleLevel = _moraleService.GetMoraleLevel(hero.Morale);
        
        var actionDescription = $"{hero.Name} causou {damageToMonster} de dano em {monster.Name}";
        if (isCritical) actionDescription += " 💥 CRÍTICO!";
        if (isFumble) actionDescription += " ❌ FALHA CRÍTICA!";
        
        return new CombatResult(hero, monster, damageToMonster, 0, isCritical, isFumble, 
            combatEnded, victory, experienceGained, actionDescription, new List<StatusEffectType>(), heroMoraleLevel, goldReward, droppedItems);
    }

    private Task ProcessStatusEffectsAtTurnStart(Hero hero, Monster monster, List<StatusEffectState> heroEffects, List<StatusEffectState> monsterEffects)
    {
        // Aplicar dano de veneno/sangramento
        foreach (var effect in heroEffects)
        {
            if (effect.Type == StatusEffectType.Poison || effect.Type == StatusEffectType.Bleeding)
            {
                var damage = _statusEffectService.GetStatusEffectDamage(effect.Type, hero.MaxHealth);
                hero.Health = Math.Max(0, hero.Health - damage);
            }
        }

        foreach (var effect in monsterEffects)
        {
            if (effect.Type == StatusEffectType.Poison || effect.Type == StatusEffectType.Bleeding)
            {
                var damage = _statusEffectService.GetStatusEffectDamage(effect.Type, monster.MaxHealth);
                monster.Health = Math.Max(0, monster.Health - damage);
            }
        }
        return Task.CompletedTask;
    }

    private Task<(int damage, bool isCritical, bool isFumble, string details)> CalculateHeroDamage(Hero hero, Monster monster, List<StatusEffectState> heroEffects, List<StatusEffectState> monsterEffects)
    {
        var heroMoraleLevel = _moraleService.GetMoraleLevel(hero.Morale);
        var (damageMultiplier, defenseMultiplier, criticalMultiplier) = _moraleService.GetMoraleModifiers(heroMoraleLevel);
        
        // Calcular dano base
        var heroStrength = hero.GetTotalStrength();
        var baseDamage = Math.Max(1, heroStrength - monster.Defense);
        var details = $"Ataque base: {heroStrength} - {monster.Defense} = {baseDamage}";
        
        // Aplicar modificadores de status effects
        var (statusAttackMultiplier, statusDefenseMultiplier) = _statusEffectService.GetStatusEffectModifiers(heroEffects);
        var statusModifiedDamage = (int)Math.Ceiling(baseDamage * statusAttackMultiplier);
        if (statusAttackMultiplier != 1.0)
        {
            details += $", Status Effects: {statusAttackMultiplier:F1}x = {statusModifiedDamage}";
        }
        baseDamage = statusModifiedDamage;
        
        // Aplicar modificadores de moral
        var moraleModifiedDamage = (int)Math.Ceiling(baseDamage * damageMultiplier);
        if (damageMultiplier != 1.0)
        {
            details += $", Moral ({heroMoraleLevel}): {damageMultiplier:F1}x = {moraleModifiedDamage}";
        }
        baseDamage = moraleModifiedDamage;
        
        // Adicionar elemento aleatório (±10%)
        var randomFactor = _diceService.RollDice(20); // D20 para 5% de precisão
        var randomMultiplier = 0.9 + (randomFactor * 0.01); // 0.9 a 1.1
        var randomModifiedDamage = (int)Math.Ceiling(baseDamage * randomMultiplier);
        details += $", D20: {randomFactor} ({randomMultiplier:F2}x) = {randomModifiedDamage}";
        baseDamage = randomModifiedDamage;
        
        // Verificar crítico
        var criticalChance = _diceService.RollCriticalChance(heroMoraleLevel);
        var criticalRoll = _diceService.RollDice(100);
        var isCritical = criticalRoll <= criticalChance;
        if (isCritical) 
        {
            baseDamage *= 2;
            details += $", CRÍTICO! ({criticalRoll}/{criticalChance}%) = {baseDamage}";
        }
        else
        {
            details += $", Crítico: {criticalRoll}/{criticalChance}% (falhou)";
        }
        
        // Verificar falha
        var fumbleChance = _diceService.RollFumbleChance(heroMoraleLevel);
        var fumbleRoll = _diceService.RollDice(100);
        var isFumble = fumbleRoll <= fumbleChance;
        if (isFumble) 
        {
            baseDamage = Math.Max(1, baseDamage / 2);
            details += $", FALHA! ({fumbleRoll}/{fumbleChance}%) = {baseDamage}";
        }
        else
        {
            details += $", Falha: {fumbleRoll}/{fumbleChance}% (falhou)";
        }
        
        return Task.FromResult((baseDamage, isCritical, isFumble, details));
    }

    private async Task<(int damage, string action)> CalculateMonsterDamage(Hero hero, Monster monster, List<StatusEffectState> heroEffects, List<StatusEffectState> monsterEffects)
    {
        // Verificar se monstro está atordoado
        if (monsterEffects.Any(e => e.Type == StatusEffectType.Stunned))
        {
            return (0, $"{monster.Name} está atordoado e não pode atacar!");
        }

        // Calcular dano base
        var baseDamage = Math.Max(1, monster.Attack - hero.Defense);
        var details = $"Ataque base: {monster.Attack} - {hero.Defense} = {baseDamage}";
        
        // Aplicar modificadores de status effects
        var (statusAttackMultiplier, statusDefenseMultiplier) = _statusEffectService.GetStatusEffectModifiers(heroEffects);
        var statusModifiedDamage = (int)Math.Ceiling(baseDamage * statusDefenseMultiplier);
        if (statusDefenseMultiplier != 1.0)
        {
            details += $", Status Effects: {statusDefenseMultiplier:F1}x = {statusModifiedDamage}";
        }
        baseDamage = statusModifiedDamage;
        
        // Adicionar dano ambiental
        var environmentalDamage = monster.Habitat switch
        {
            EnvironmentType.Volcano => 2,
            EnvironmentType.Swamp => 1,
            EnvironmentType.Sky => 1,
            _ => 0
        };
        if (environmentalDamage > 0)
        {
            baseDamage += environmentalDamage;
            details += $", Dano ambiental ({monster.Habitat}): +{environmentalDamage} = {baseDamage}";
        }
        
        // Aplicar efeitos especiais do monstro
        var appliedEffects = new List<StatusEffectType>();
        var specialEffectChance = monster.Type switch
        {
            MonsterType.Goblin => 20, // 20% chance de envenenar
            MonsterType.Dragon => 40, // 40% chance de queimar
            MonsterType.Demon => 30,  // 30% chance de causar medo
            MonsterType.Undead => 25, // 25% chance de congelar
            _ => 10
        };
        
        var effectRoll = _diceService.RollDice(100);
        var effectApplied = effectRoll <= specialEffectChance;
        details += $", Efeito especial: {effectRoll}/{specialEffectChance}%";
        
        if (effectApplied)
        {
            var effectType = monster.Type switch
            {
                MonsterType.Goblin => StatusEffectType.Poison,
                MonsterType.Dragon => StatusEffectType.Poison, // Queimar = Poison por simplicidade
                MonsterType.Demon => StatusEffectType.Fear,
                MonsterType.Undead => StatusEffectType.Weakened, // Congelar = Weakened por simplicidade
                _ => StatusEffectType.Poison
            };
            
            var duration = monster.Type switch
            {
                MonsterType.Goblin => 3,
                MonsterType.Dragon => 2,
                MonsterType.Demon => 2,
                MonsterType.Undead => 2,
                _ => 2
            };
            
            await _statusEffectService.ApplyStatusEffectAsync(EffectTargetKind.Character, hero.Id, effectType, duration);
            appliedEffects.Add(effectType);
            details += $" (APLICADO: {effectType} por {duration} turnos)";
        }
        else
        {
            details += " (falhou)";
        }
        
        var actionDescription = $"{monster.Name} atacou {hero.Name} causando {baseDamage} de dano!";
        if (appliedEffects.Any())
        {
            actionDescription += $" Aplicou efeito: {string.Join(", ", appliedEffects)}";
        }
        actionDescription += $" [{details}]";
        
        return (baseDamage, actionDescription);
    }

    public Task<CombatResult> UseAbilityAsync(int heroId, int monsterId, string abilityName)
    {
        // TODO: Implementar sistema de habilidades
        throw new NotImplementedException("Sistema de habilidades ainda não implementado");
    }

    public async Task<CombatResult> UseAbilityAsync(int heroId, int monsterId, int abilityId)
    {
        var hero = await _db.Heroes.FirstOrDefaultAsync(x => x.Id == heroId) ?? throw new InvalidOperationException("Hero not found");
        var monster = await _db.Monsters.FirstOrDefaultAsync(x => x.Id == monsterId) ?? throw new InvalidOperationException("Monster not found");

        var heroEffects = await _statusEffectService.GetActiveEffectsAsync(EffectTargetKind.Hero, hero.Id);
        var monsterEffects = await _statusEffectService.GetActiveEffectsAsync(EffectTargetKind.Monster, monster.Id);

        var heroMoraleLevel = _moraleService.GetMoraleLevel(hero.Morale);

        // Verificar se a habilidade está disponível
        var characterAbilities = await _specialAbilityService.GetCharacterAbilitiesAsync(heroId);
        var ability = characterAbilities.FirstOrDefault(ca => ca.AbilityId == abilityId && ca.IsEquipped);

        if (ability == null)
        {
            return new CombatResult(
                hero, monster, 0, 0, false, false, false, false, 0, 
                "Habilidade não encontrada ou não equipada!", heroEffects.Select(e => e.Type).ToList(), heroMoraleLevel, 0);
        }

        if (hero.Morale < ability.Ability.ManaCost)
        {
            return new CombatResult(
                hero, monster, 0, 0, false, false, false, false, 0, 
                "Mana insuficiente para usar esta habilidade!", heroEffects.Select(e => e.Type).ToList(), heroMoraleLevel, 0);
        }

        // Usar a habilidade
        var (success, message) = await _specialAbilityService.UseAbilityAsync(heroId, abilityId, monsterId);
        
        if (!success)
        {
            return new CombatResult(
                hero, monster, 0, 0, false, false, false, false, 0, 
                message, heroEffects.Select(e => e.Type).ToList(), heroMoraleLevel, 0);
        }

        // Aplicar efeitos da habilidade
        var abilityDamage = 0;
        var abilityHealing = 0;
        var newHeroEffects = new List<StatusEffectType>(heroEffects.Select(e => e.Type));

        if (ability.Ability.Damage > 0)
        {
            abilityDamage = ability.Ability.Damage;
            monster.Health = Math.Max(0, monster.Health - abilityDamage);
        }

        if (ability.Ability.Healing > 0)
        {
            abilityHealing = ability.Ability.Healing;
            hero.Health = Math.Min(hero.MaxHealth, hero.Health + abilityHealing);
        }

        // Aplicar efeitos de status
        if (ability.Ability.StatusEffects.Any())
        {
            newHeroEffects.AddRange(ability.Ability.StatusEffects);
        }

        // Verificar se o monstro morreu
        var monsterDied = monster.Health <= 0;
        var experienceGained = 0;

        if (monsterDied)
        {
            experienceGained = monster.ExperienceReward;
            hero.Experience += experienceGained;
            hero.Morale = Math.Min(100, hero.Morale + 10); // Bonus de moral por vitória

            // Trigger de conquista
            await _achievementService.UpdateAchievementProgressAsync(hero.UserId ?? 0, AchievementType.Combat, 1);

            // Verificar level up
            var leveledUp = await _levelUpService.CheckAndProcessLevelUpAsync(hero);
            if (leveledUp)
            {
                await _achievementService.UpdateAchievementProgressAsync(hero.UserId ?? 0, AchievementType.Progression, hero.Level);
            }
        }

        await _db.SaveChangesAsync();

        var resultMessage = $"{message}";
        if (abilityDamage > 0) resultMessage += $" Causou {abilityDamage} de dano!";
        if (abilityHealing > 0) resultMessage += $" Curou {abilityHealing} de vida!";
        if (monsterDied) resultMessage += $" {monster.Name} foi derrotado! +{experienceGained} XP";

        return new CombatResult(
            hero, monster, abilityDamage, 0, monsterDied, false, false, false, experienceGained, 
            resultMessage, newHeroEffects, heroMoraleLevel, 0);
    }

    public Task<CombatResult> UseItemAsync(int heroId, int monsterId, string itemName)
    {
        // TODO: Implementar sistema de itens
        throw new NotImplementedException("Sistema de itens ainda não implementado");
    }

    public async Task<bool> TryEscapeAsync(int heroId, int monsterId)
    {
        var hero = await _db.Heroes.FirstOrDefaultAsync(x => x.Id == heroId) ?? throw new InvalidOperationException("Hero not found");
        var monster = await _db.Monsters.FirstOrDefaultAsync(x => x.Id == monsterId) ?? throw new InvalidOperationException("Monster not found");
        
        // Chance de fuga baseada na moral e tipo de monstro
        var baseEscapeChance = 50;
        var moraleBonus = _moraleService.GetMoraleLevel(hero.Morale) switch
        {
            MoraleLevel.Despair => -20,
            MoraleLevel.Low => -10,
            MoraleLevel.Normal => 0,
            MoraleLevel.High => 10,
            MoraleLevel.Inspired => 20,
            _ => 0
        };
        
        var monsterPenalty = monster.Rank == MonsterRank.Boss ? -30 : 0;
        var escapeChance = Math.Max(10, Math.Min(90, baseEscapeChance + moraleBonus + monsterPenalty));
        
        var success = _diceService.RollPercentage(escapeChance);
        
        if (success)
        {
            hero.Morale = _moraleService.AdjustMorale(hero.Morale, MoraleEvent.AttackMiss); // Fuga reduz moral
        }
        
        await _db.SaveChangesAsync();
        return success;
    }
}



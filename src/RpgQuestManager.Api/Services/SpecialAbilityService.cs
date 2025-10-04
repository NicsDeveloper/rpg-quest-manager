using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface ISpecialAbilityService
{
    Task<List<SpecialAbility>> GetAllAbilitiesAsync();
    Task<List<SpecialAbility>> GetAbilitiesByTypeAsync(AbilityType type);
    Task<List<SpecialAbility>> GetAbilitiesByCategoryAsync(AbilityCategory category);
    Task<List<CharacterAbility>> GetCharacterAbilitiesAsync(int characterId);
    Task<(bool success, string message)> UnlockAbilityAsync(int characterId, int abilityId);
    Task<(bool success, string message)> EquipAbilityAsync(int characterId, int abilityId);
    Task<(bool success, string message)> UnequipAbilityAsync(int characterId, int abilityId);
    Task<(bool success, string message)> UseAbilityAsync(int characterId, int abilityId, int targetId);
    Task<List<Combo>> GetAvailableCombosAsync(int characterId);
    Task<List<CharacterCombo>> GetCharacterCombosAsync(int characterId);
    Task<(bool success, string message)> UnlockComboAsync(int characterId, int comboId);
    Task<(bool success, string message)> ExecuteComboStepAsync(int characterId, int comboId, int abilityId);
}

public class SpecialAbilityService : ISpecialAbilityService
{
    private readonly ApplicationDbContext _db;

    public SpecialAbilityService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<SpecialAbility>> GetAllAbilitiesAsync()
    {
        return await _db.SpecialAbilities
            .OrderBy(sa => sa.RequiredLevel)
            .ThenBy(sa => sa.Name)
            .ToListAsync();
    }

    public async Task<List<SpecialAbility>> GetAbilitiesByTypeAsync(AbilityType type)
    {
        return await _db.SpecialAbilities
            .Where(sa => sa.Type == type)
            .OrderBy(sa => sa.RequiredLevel)
            .ThenBy(sa => sa.Name)
            .ToListAsync();
    }

    public async Task<List<SpecialAbility>> GetAbilitiesByCategoryAsync(AbilityCategory category)
    {
        return await _db.SpecialAbilities
            .Where(sa => sa.Category == category)
            .OrderBy(sa => sa.RequiredLevel)
            .ThenBy(sa => sa.Name)
            .ToListAsync();
    }

    public async Task<List<CharacterAbility>> GetCharacterAbilitiesAsync(int characterId)
    {
        return await _db.CharacterAbilities
            .Include(ca => ca.Ability)
            .Where(ca => ca.CharacterId == characterId)
            .OrderBy(ca => ca.Ability.RequiredLevel)
            .ThenBy(ca => ca.Ability.Name)
            .ToListAsync();
    }

    public async Task<(bool success, string message)> UnlockAbilityAsync(int characterId, int abilityId)
    {
        try
        {
            var character = await _db.Characters.FindAsync(characterId);
            if (character == null)
            {
                return (false, "Personagem não encontrado");
            }

            var ability = await _db.SpecialAbilities.FindAsync(abilityId);
            if (ability == null)
            {
                return (false, "Habilidade não encontrada");
            }

            if (character.Level < ability.RequiredLevel)
            {
                return (false, $"Nível insuficiente. Necessário: {ability.RequiredLevel}");
            }

            if (character.Experience < ability.ExperienceCost)
            {
                return (false, $"Experiência insuficiente. Necessário: {ability.ExperienceCost}");
            }

            if (character.Gold < ability.GoldCost)
            {
                return (false, $"Ouro insuficiente. Necessário: {ability.GoldCost}");
            }

            // Verificar se a habilidade já foi desbloqueada
            var existingAbility = await _db.CharacterAbilities
                .FirstOrDefaultAsync(ca => ca.CharacterId == characterId && ca.AbilityId == abilityId);

            if (existingAbility != null)
            {
                return (false, "Habilidade já foi desbloqueada");
            }

            // Desbloquear a habilidade
            var characterAbility = new CharacterAbility
            {
                CharacterId = characterId,
                AbilityId = abilityId,
                Level = 1,
                IsUnlocked = true,
                IsEquipped = false,
                UnlockedAt = DateTime.UtcNow
            };

            _db.CharacterAbilities.Add(characterAbility);

            // Deduzir custos
            character.Experience -= ability.ExperienceCost;
            character.Gold -= ability.GoldCost;

            await _db.SaveChangesAsync();

            return (true, $"Habilidade '{ability.Name}' desbloqueada com sucesso");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao desbloquear habilidade: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> EquipAbilityAsync(int characterId, int abilityId)
    {
        try
        {
            var characterAbility = await _db.CharacterAbilities
                .Include(ca => ca.Ability)
                .FirstOrDefaultAsync(ca => ca.CharacterId == characterId && ca.AbilityId == abilityId);

            if (characterAbility == null)
            {
                return (false, "Habilidade não encontrada");
            }

            if (!characterAbility.IsUnlocked)
            {
                return (false, "Habilidade não foi desbloqueada");
            }

            // Verificar se já está equipada
            if (characterAbility.IsEquipped)
            {
                return (false, "Habilidade já está equipada");
            }

            // Verificar limite de habilidades equipadas (ex: máximo 4)
            var equippedCount = await _db.CharacterAbilities
                .CountAsync(ca => ca.CharacterId == characterId && ca.IsEquipped);

            if (equippedCount >= 4)
            {
                return (false, "Limite de habilidades equipadas atingido");
            }

            characterAbility.IsEquipped = true;
            await _db.SaveChangesAsync();

            return (true, $"Habilidade '{characterAbility.Ability.Name}' equipada com sucesso");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao equipar habilidade: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> UnequipAbilityAsync(int characterId, int abilityId)
    {
        try
        {
            var characterAbility = await _db.CharacterAbilities
                .Include(ca => ca.Ability)
                .FirstOrDefaultAsync(ca => ca.CharacterId == characterId && ca.AbilityId == abilityId);

            if (characterAbility == null)
            {
                return (false, "Habilidade não encontrada");
            }

            if (!characterAbility.IsEquipped)
            {
                return (false, "Habilidade não está equipada");
            }

            characterAbility.IsEquipped = false;
            await _db.SaveChangesAsync();

            return (true, $"Habilidade '{characterAbility.Ability.Name}' desequipada com sucesso");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao desequipar habilidade: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> UseAbilityAsync(int characterId, int abilityId, int targetId)
    {
        try
        {
            var characterAbility = await _db.CharacterAbilities
                .Include(ca => ca.Ability)
                .FirstOrDefaultAsync(ca => ca.CharacterId == characterId && ca.AbilityId == abilityId);

            if (characterAbility == null)
            {
                return (false, "Habilidade não encontrada");
            }

            if (!characterAbility.IsUnlocked || !characterAbility.IsEquipped)
            {
                return (false, "Habilidade não está disponível");
            }

            var character = await _db.Characters.FindAsync(characterId);
            if (character == null)
            {
                return (false, "Personagem não encontrado");
            }

            // Verificar se o personagem tem mana suficiente
            if (character.Morale < characterAbility.Ability.ManaCost)
            {
                return (false, "Mana insuficiente");
            }

            // Aplicar efeitos da habilidade
            if (characterAbility.Ability.Damage > 0)
            {
                // Lógica de dano seria implementada aqui
                // Por enquanto, apenas registramos o uso
            }

            if (characterAbility.Ability.Healing > 0)
            {
                character.Health = Math.Min(character.MaxHealth, character.Health + characterAbility.Ability.Healing);
            }

            // Deduzir mana
            character.Morale = Math.Max(0, character.Morale - characterAbility.Ability.ManaCost);

            // Atualizar último uso
            characterAbility.LastUsedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return (true, $"Habilidade '{characterAbility.Ability.Name}' usada com sucesso");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao usar habilidade: {ex.Message}");
        }
    }

    public async Task<List<Combo>> GetAvailableCombosAsync(int characterId)
    {
        var characterAbilities = await _db.CharacterAbilities
            .Where(ca => ca.CharacterId == characterId && ca.IsUnlocked)
            .Select(ca => ca.AbilityId)
            .ToListAsync();

        return await _db.Combos
            .Include(c => c.Steps)
            .ThenInclude(cs => cs.Ability)
            .Where(c => c.Steps.All(cs => characterAbilities.Contains(cs.AbilityId)))
            .OrderBy(c => c.RequiredLevel)
            .ToListAsync();
    }

    public async Task<List<CharacterCombo>> GetCharacterCombosAsync(int characterId)
    {
        return await _db.CharacterCombos
            .Include(cc => cc.Combo)
            .ThenInclude(c => c.Steps)
            .ThenInclude(cs => cs.Ability)
            .Where(cc => cc.CharacterId == characterId)
            .OrderBy(cc => cc.Combo.RequiredLevel)
            .ToListAsync();
    }

    public async Task<(bool success, string message)> UnlockComboAsync(int characterId, int comboId)
    {
        try
        {
            var character = await _db.Characters.FindAsync(characterId);
            if (character == null)
            {
                return (false, "Personagem não encontrado");
            }

            var combo = await _db.Combos
                .Include(c => c.Steps)
                .FirstOrDefaultAsync(c => c.Id == comboId);

            if (combo == null)
            {
                return (false, "Combo não encontrado");
            }

            if (character.Level < combo.RequiredLevel)
            {
                return (false, $"Nível insuficiente. Necessário: {combo.RequiredLevel}");
            }

            // Verificar se o combo já foi desbloqueado
            var existingCombo = await _db.CharacterCombos
                .FirstOrDefaultAsync(cc => cc.CharacterId == characterId && cc.ComboId == comboId);

            if (existingCombo != null)
            {
                return (false, "Combo já foi desbloqueado");
            }

            // Verificar se o personagem tem todas as habilidades necessárias
            var characterAbilities = await _db.CharacterAbilities
                .Where(ca => ca.CharacterId == characterId && ca.IsUnlocked)
                .Select(ca => ca.AbilityId)
                .ToListAsync();

            var requiredAbilities = combo.Steps.Select(cs => cs.AbilityId).ToList();
            if (!requiredAbilities.All(ra => characterAbilities.Contains(ra)))
            {
                return (false, "Habilidades necessárias não foram desbloqueadas");
            }

            // Desbloquear o combo
            var characterCombo = new CharacterCombo
            {
                CharacterId = characterId,
                ComboId = comboId,
                Progress = 0,
                LastStepCompleted = 0,
                LastStepTime = DateTime.UtcNow,
                IsUnlocked = true,
                UnlockedAt = DateTime.UtcNow
            };

            _db.CharacterCombos.Add(characterCombo);

            // Dar recompensas
            if (combo.ExperienceReward > 0)
            {
                character.Experience += combo.ExperienceReward;
            }

            if (combo.GoldReward > 0)
            {
                character.Gold += combo.GoldReward;
            }

            await _db.SaveChangesAsync();

            return (true, $"Combo '{combo.Name}' desbloqueado com sucesso");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao desbloquear combo: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> ExecuteComboStepAsync(int characterId, int comboId, int abilityId)
    {
        try
        {
            var characterCombo = await _db.CharacterCombos
                .Include(cc => cc.Combo)
                .ThenInclude(c => c.Steps)
                .ThenInclude(cs => cs.Ability)
                .FirstOrDefaultAsync(cc => cc.CharacterId == characterId && cc.ComboId == comboId);

            if (characterCombo == null)
            {
                return (false, "Combo não encontrado");
            }

            if (!characterCombo.IsUnlocked)
            {
                return (false, "Combo não foi desbloqueado");
            }

            var combo = characterCombo.Combo;
            var nextStep = combo.Steps
                .Where(cs => cs.StepOrder == characterCombo.LastStepCompleted + 1)
                .FirstOrDefault();

            if (nextStep == null)
            {
                return (false, "Combo já foi completado");
            }

            if (nextStep.AbilityId != abilityId)
            {
                return (false, "Habilidade incorreta para este passo do combo");
            }

            // Verificar se ainda está na janela de tempo
            var timeSinceLastStep = DateTime.UtcNow - characterCombo.LastStepTime;
            if (timeSinceLastStep.TotalSeconds > nextStep.TimeWindow)
            {
                // Resetar progresso do combo
                characterCombo.Progress = 0;
                characterCombo.LastStepCompleted = 0;
                characterCombo.LastStepTime = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return (false, "Combo expirado. Progresso resetado.");
            }

            // Avançar no combo
            characterCombo.Progress++;
            characterCombo.LastStepCompleted++;
            characterCombo.LastStepTime = DateTime.UtcNow;

            // Verificar se o combo foi completado
            if (characterCombo.LastStepCompleted >= combo.Steps.Count)
            {
                // Combo completado - aplicar efeitos especiais
                characterCombo.Progress = 0;
                characterCombo.LastStepCompleted = 0;
                await _db.SaveChangesAsync();
                return (true, $"Combo '{combo.Name}' executado com sucesso!");
            }

            await _db.SaveChangesAsync();
            return (true, $"Passo {characterCombo.LastStepCompleted} do combo executado");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao executar passo do combo: {ex.Message}");
        }
    }
}

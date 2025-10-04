using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class StatusEffectService
{
    private readonly ApplicationDbContext _db;

    public StatusEffectService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task ApplyStatusEffectAsync(EffectTargetKind targetKind, int targetId, StatusEffectType effectType, int duration)
    {
        // Remove efeito existente do mesmo tipo
        var existingEffect = await _db.StatusEffects
            .FirstOrDefaultAsync(e => e.TargetKind == targetKind && e.TargetId == targetId && e.Type == effectType);
        
        if (existingEffect != null)
        {
            _db.StatusEffects.Remove(existingEffect);
        }

        // Adiciona novo efeito
        _db.StatusEffects.Add(new StatusEffectState
        {
            TargetKind = targetKind,
            TargetId = targetId,
            Type = effectType,
            TurnsRemaining = duration
        });

        await _db.SaveChangesAsync();
    }

    public async Task<List<StatusEffectState>> GetActiveEffectsAsync(EffectTargetKind targetKind, int targetId)
    {
        return await _db.StatusEffects
            .Where(e => e.TargetKind == targetKind && e.TargetId == targetId)
            .ToListAsync();
    }

    public async Task ProcessStatusEffectsAsync(EffectTargetKind targetKind, int targetId)
    {
        var effects = await GetActiveEffectsAsync(targetKind, targetId);
        
        foreach (var effect in effects)
        {
            effect.TurnsRemaining--;
            if (effect.TurnsRemaining <= 0)
            {
                _db.StatusEffects.Remove(effect);
            }
        }

        await _db.SaveChangesAsync();
    }

    public int GetStatusEffectDamage(StatusEffectType effectType, int maxHealth)
    {
        return effectType switch
        {
            StatusEffectType.Poison => (int)Math.Ceiling(maxHealth * 0.05), // 5% do HP máximo
            StatusEffectType.Bleeding => (int)Math.Ceiling(maxHealth * 0.03), // 3% do HP máximo
            _ => 0
        };
    }

    public (double attackMultiplier, double defenseMultiplier) GetStatusEffectModifiers(List<StatusEffectState> effects)
    {
        double attackMultiplier = 1.0;
        double defenseMultiplier = 1.0;

        foreach (var effect in effects)
        {
            switch (effect.Type)
            {
                case StatusEffectType.StrengthBoost:
                    attackMultiplier *= 1.2; // +20% ataque
                    break;
                case StatusEffectType.Weakened:
                    attackMultiplier *= 0.8; // -20% ataque
                    defenseMultiplier *= 0.8; // -20% defesa
                    break;
                case StatusEffectType.Shielded:
                    defenseMultiplier *= 1.3; // +30% defesa
                    break;
            }
        }

        return (attackMultiplier, defenseMultiplier);
    }
}

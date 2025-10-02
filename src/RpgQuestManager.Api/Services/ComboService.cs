using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class ComboService : IComboService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ComboService> _logger;

    public ComboService(ApplicationDbContext context, ILogger<ComboService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Detecta se as classes dos heróis formam um combo especial
    /// </summary>
    public async Task<PartyCombo?> DetectComboAsync(List<string> heroClasses)
    {
        if (heroClasses.Count < 2) return null;

        var classes = heroClasses.OrderBy(c => c).ToList();

        // Busca combos que correspondem às classes fornecidas
        var combos = await _context.PartyCombos.ToListAsync();

        foreach (var combo in combos)
        {
            var requiredClasses = new List<string> { combo.RequiredClass1, combo.RequiredClass2 };
            if (!string.IsNullOrEmpty(combo.RequiredClass3))
            {
                requiredClasses.Add(combo.RequiredClass3);
            }

            requiredClasses = requiredClasses.OrderBy(c => c).ToList();

            // Verifica se as classes da party contêm todas as classes requeridas
            var hasAllRequired = requiredClasses.All(rc => classes.Contains(rc));
            
            if (hasAllRequired)
            {
                _logger.LogInformation("Combo detectado: {ComboName} com classes {Classes}", 
                    combo.Name, string.Join(", ", classes));
                return combo;
            }
        }

        return null;
    }

    /// <summary>
    /// Obtém a fraqueza de um boss para um combo específico
    /// </summary>
    public async Task<BossWeakness?> GetBossWeaknessAsync(int enemyId, int comboId)
    {
        return await _context.BossWeaknesses
            .Include(bw => bw.Combo)
            .Include(bw => bw.Enemy)
            .FirstOrDefaultAsync(bw => bw.EnemyId == enemyId && bw.ComboId == comboId);
    }

    /// <summary>
    /// Registra uma descoberta de combo/fraqueza pelo usuário
    /// </summary>
    public async Task<bool> RegisterDiscoveryAsync(int userId, int enemyId, int comboId)
    {
        var existing = await _context.ComboDiscoveries
            .FirstOrDefaultAsync(cd => cd.UserId == userId && cd.EnemyId == enemyId && cd.ComboId == comboId);

        if (existing != null)
        {
            existing.TimesUsed++;
            _logger.LogInformation("Usuário {UserId} usou combo {ComboId} contra boss {EnemyId} pela {Count}ª vez",
                userId, comboId, enemyId, existing.TimesUsed);
            await _context.SaveChangesAsync();
            return false; // Já tinha descoberto
        }

        var discovery = new ComboDiscovery
        {
            UserId = userId,
            EnemyId = enemyId,
            ComboId = comboId,
            DiscoveredAt = DateTime.UtcNow,
            TimesUsed = 1,
            TimesWon = 0
        };

        _context.ComboDiscoveries.Add(discovery);
        await _context.SaveChangesAsync();

        _logger.LogInformation("🎉 DESCOBERTA! Usuário {UserId} descobriu fraqueza do boss {EnemyId} ao combo {ComboId}",
            userId, enemyId, comboId);

        return true; // Nova descoberta!
    }

    /// <summary>
    /// Verifica se o usuário já descobriu uma fraqueza específica
    /// </summary>
    public async Task<bool> HasDiscoveredAsync(int userId, int enemyId, int comboId)
    {
        return await _context.ComboDiscoveries
            .AnyAsync(cd => cd.UserId == userId && cd.EnemyId == enemyId && cd.ComboId == comboId);
    }

    /// <summary>
    /// Obtém todas as descobertas de um usuário
    /// </summary>
    public async Task<List<ComboDiscovery>> GetUserDiscoveriesAsync(int userId)
    {
        return await _context.ComboDiscoveries
            .Include(cd => cd.Enemy)
            .Include(cd => cd.Combo)
            .Where(cd => cd.UserId == userId)
            .OrderByDescending(cd => cd.DiscoveredAt)
            .ToListAsync();
    }

    /// <summary>
    /// Obtém todas as fraquezas conhecidas de um boss
    /// </summary>
    public async Task<List<BossWeakness>> GetEnemyWeaknessesAsync(int enemyId)
    {
        return await _context.BossWeaknesses
            .Include(bw => bw.Combo)
            .Where(bw => bw.EnemyId == enemyId)
            .ToListAsync();
    }
}


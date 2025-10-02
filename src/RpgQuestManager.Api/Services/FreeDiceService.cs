using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class FreeDiceService : IFreeDiceService
{
    private readonly ApplicationDbContext _context;
    private readonly IDiceService _diceService;
    private readonly ILogger<FreeDiceService> _logger;

    public FreeDiceService(
        ApplicationDbContext context,
        IDiceService diceService,
        ILogger<FreeDiceService> logger)
    {
        _context = context;
        _diceService = diceService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém ou cria todos os grants de dados gratuitos para um usuário
    /// </summary>
    public async Task<List<FreeDiceGrant>> GetUserGrantsAsync(int userId)
    {
        var grants = new List<FreeDiceGrant>();

        foreach (DiceType diceType in Enum.GetValues(typeof(DiceType)))
        {
            var grant = await GetOrCreateGrantAsync(userId, diceType);
            grants.Add(grant);
        }

        return grants;
    }

    /// <summary>
    /// Obtém ou cria um grant específico
    /// </summary>
    public async Task<FreeDiceGrant> GetOrCreateGrantAsync(int userId, DiceType diceType)
    {
        var grant = await _context.FreeDiceGrants
            .FirstOrDefaultAsync(g => g.UserId == userId && g.DiceType == diceType);

        if (grant == null)
        {
            // Cria o primeiro grant (disponível imediatamente)
            grant = new FreeDiceGrant
            {
                UserId = userId,
                DiceType = diceType,
                LastClaimedAt = DateTime.UtcNow.AddHours(-FreeDiceGrant.GetCooldownHours(diceType)), // Permite resgate imediato
                NextAvailableAt = DateTime.UtcNow // Disponível agora
            };

            _context.FreeDiceGrants.Add(grant);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Grant de {DiceType} criado para usuário {UserId}", diceType, userId);
        }

        return grant;
    }

    /// <summary>
    /// Resgata um dado gratuito se disponível
    /// </summary>
    public async Task<bool> ClaimFreeDiceAsync(int userId, DiceType diceType)
    {
        var grant = await GetOrCreateGrantAsync(userId, diceType);

        if (!grant.IsAvailable())
        {
            _logger.LogWarning("Usuário {UserId} tentou resgatar {DiceType} mas ainda não está disponível", 
                userId, diceType);
            return false;
        }

        // Busca o herói principal do usuário para adicionar o dado
        var hero = await _context.Heroes
            .FirstOrDefaultAsync(h => h.UserId == userId && h.IsInActiveParty && h.PartySlot == 1);

        if (hero == null)
        {
            // Se não tem herói na party, busca qualquer herói do usuário
            hero = await _context.Heroes
                .FirstOrDefaultAsync(h => h.UserId == userId);
        }

        if (hero == null)
        {
            _logger.LogWarning("Usuário {UserId} tentou resgatar dado mas não tem heróis", userId);
            return false;
        }

        // Adiciona o dado ao inventário do herói
        var inventory = await _diceService.GetOrCreateInventoryAsync(hero.Id);
        inventory.AddDice(diceType, 1);

        // Atualiza o grant
        grant.LastClaimedAt = DateTime.UtcNow;
        grant.NextAvailableAt = DateTime.UtcNow.AddHours(FreeDiceGrant.GetCooldownHours(diceType));

        await _context.SaveChangesAsync();

        _logger.LogInformation("🎁 Usuário {UserId} resgatou 1x {DiceType} gratuito! Próximo em: {NextAvailable}",
            userId, diceType, grant.NextAvailableAt);

        return true;
    }
}


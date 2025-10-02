using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Services;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "📖 Bestiary")]
public class BestiaryController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IComboService _comboService;
    private readonly ILogger<BestiaryController> _logger;

    public BestiaryController(
        ApplicationDbContext context,
        IComboService comboService,
        ILogger<BestiaryController> logger)
    {
        _context = context;
        _comboService = comboService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todas as descobertas de combos do usuário logado
    /// </summary>
    [HttpGet("discoveries")]
    [ProducesResponseType(typeof(List<DiscoveryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DiscoveryDto>>> GetDiscoveries()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var discoveries = await _comboService.GetUserDiscoveriesAsync(userId);

        var dtos = discoveries.Select(d => new DiscoveryDto
        {
            Id = d.Id,
            EnemyId = d.EnemyId,
            EnemyName = d.Enemy.Name,
            ComboId = d.ComboId,
            ComboName = d.Combo.Name,
            ComboDescription = d.Combo.Description,
            ComboIcon = d.Combo.Icon,
            DiscoveredAt = d.DiscoveredAt,
            TimesUsed = d.TimesUsed,
            TimesWon = d.TimesWon
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Obtém todos os bosses e suas fraquezas descobertas pelo usuário
    /// </summary>
    [HttpGet("bosses")]
    [ProducesResponseType(typeof(List<BossInfoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BossInfoDto>>> GetBosses()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var bosses = await _context.Enemies
            .Where(e => e.IsBoss)
            .Include(e => e.BossDrops)
                .ThenInclude(bd => bd.Item)
            .ToListAsync();

        var userDiscoveries = await _comboService.GetUserDiscoveriesAsync(userId);

        var dtos = new List<BossInfoDto>();

        foreach (var boss in bosses)
        {
            var bossWeaknesses = await _comboService.GetEnemyWeaknessesAsync(boss.Id);
            var discoveredWeaknesses = bossWeaknesses
                .Where(bw => userDiscoveries.Any(ud => ud.EnemyId == boss.Id && ud.ComboId == bw.ComboId))
                .ToList();

            dtos.Add(new BossInfoDto
            {
                Id = boss.Id,
                Name = boss.Name,
                Type = boss.Type,
                Power = boss.Power,
                Health = boss.Health,
                RequiredDiceType = boss.RequiredDiceType.ToString(),
                MinimumRoll = boss.MinimumRoll,
                DiscoveredWeaknesses = discoveredWeaknesses.Select(bw => new WeaknessDto
                {
                    ComboName = bw.Combo.Name,
                    ComboDescription = bw.Combo.Description,
                    ComboIcon = bw.Combo.Icon,
                    RollReduction = bw.RollReduction,
                    DropMultiplier = bw.DropMultiplier,
                    ExpMultiplier = bw.ExpMultiplier,
                    FlavorText = bw.FlavorText
                }).ToList(),
                HasDiscoveries = discoveredWeaknesses.Any(),
                TotalWeaknesses = bossWeaknesses.Count,
                DiscoveredCount = discoveredWeaknesses.Count
            });
        }

        return Ok(dtos);
    }

    /// <summary>
    /// Obtém todos os combos disponíveis no jogo
    /// </summary>
    [HttpGet("combos")]
    [ProducesResponseType(typeof(List<ComboDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ComboDto>>> GetCombos()
    {
        var combos = await _context.PartyCombos.ToListAsync();

        var dtos = combos.Select(c => new ComboDto
        {
            Id = c.Id,
            Name = c.Name,
            RequiredClass1 = c.RequiredClass1,
            RequiredClass2 = c.RequiredClass2,
            RequiredClass3 = c.RequiredClass3,
            Description = c.Description,
            Effect = c.Effect,
            Icon = c.Icon
        }).ToList();

        return Ok(dtos);
    }
}

public class DiscoveryDto
{
    public int Id { get; set; }
    public int EnemyId { get; set; }
    public string EnemyName { get; set; } = string.Empty;
    public int ComboId { get; set; }
    public string ComboName { get; set; } = string.Empty;
    public string ComboDescription { get; set; } = string.Empty;
    public string ComboIcon { get; set; } = string.Empty;
    public DateTime DiscoveredAt { get; set; }
    public int TimesUsed { get; set; }
    public int TimesWon { get; set; }
}

public class BossInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Power { get; set; }
    public int Health { get; set; }
    public string RequiredDiceType { get; set; } = string.Empty;
    public int MinimumRoll { get; set; }
    public List<WeaknessDto> DiscoveredWeaknesses { get; set; } = new List<WeaknessDto>();
    public bool HasDiscoveries { get; set; }
    public int TotalWeaknesses { get; set; }
    public int DiscoveredCount { get; set; }
}

public class WeaknessDto
{
    public string ComboName { get; set; } = string.Empty;
    public string ComboDescription { get; set; } = string.Empty;
    public string ComboIcon { get; set; } = string.Empty;
    public int RollReduction { get; set; }
    public decimal DropMultiplier { get; set; }
    public decimal ExpMultiplier { get; set; }
    public string FlavorText { get; set; } = string.Empty;
}

public class ComboDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RequiredClass1 { get; set; } = string.Empty;
    public string RequiredClass2 { get; set; } = string.Empty;
    public string? RequiredClass3 { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Effect { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}


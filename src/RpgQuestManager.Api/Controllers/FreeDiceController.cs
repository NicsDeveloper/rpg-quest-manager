using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "🎁 Free Dice")]
public class FreeDiceController : ControllerBase
{
    private readonly IFreeDiceService _freeDiceService;
    private readonly ILogger<FreeDiceController> _logger;

    public FreeDiceController(IFreeDiceService freeDiceService, ILogger<FreeDiceController> logger)
    {
        _freeDiceService = freeDiceService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os grants de dados gratuitos do usuário
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<FreeDiceGrantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FreeDiceGrantDto>>> GetGrants()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var grants = await _freeDiceService.GetUserGrantsAsync(userId);

        var dtos = grants.Select(g => new FreeDiceGrantDto
        {
            DiceType = g.DiceType.ToString(),
            LastClaimedAt = g.LastClaimedAt,
            NextAvailableAt = g.NextAvailableAt,
            IsAvailable = g.IsAvailable(),
            CooldownHours = FreeDiceGrant.GetCooldownHours(g.DiceType),
            TimeUntilAvailable = g.TimeUntilAvailable()
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Resgata um dado gratuito
    /// </summary>
    [HttpPost("claim/{diceType}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ClaimDice(string diceType)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (!Enum.TryParse<DiceType>(diceType, out var parsedDiceType))
        {
            return BadRequest("Tipo de dado inválido.");
        }

        var success = await _freeDiceService.ClaimFreeDiceAsync(userId, parsedDiceType);

        if (!success)
        {
            return BadRequest("Dado ainda não disponível ou você não tem heróis.");
        }

        return Ok(new { message = $"✅ Dado {diceType} resgatado com sucesso!" });
    }
}

public class FreeDiceGrantDto
{
    public string DiceType { get; set; } = string.Empty;
    public DateTime LastClaimedAt { get; set; }
    public DateTime NextAvailableAt { get; set; }
    public bool IsAvailable { get; set; }
    public int CooldownHours { get; set; }
    public TimeSpan TimeUntilAvailable { get; set; }
}


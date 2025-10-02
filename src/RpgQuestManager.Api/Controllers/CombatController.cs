using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.DTOs.Combat;
using RpgQuestManager.Api.Services;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "⚔️ Combat")]
public class CombatController : ControllerBase
{
    private readonly ICombatService _combatService;
    private readonly ILogger<CombatController> _logger;

    public CombatController(ICombatService combatService, ILogger<CombatController> logger)
    {
        _combatService = combatService;
        _logger = logger;
    }

    /// <summary>
    /// Inicia uma nova sessão de combate com 1 a 3 heróis
    /// </summary>
    /// <remarks>
    /// - Máximo de 3 heróis por combate
    /// - Mais heróis = menor recompensa individual
    /// - Combos de classes podem gerar sinergias especiais
    /// </remarks>
    [HttpPost("start")]
    [ProducesResponseType(typeof(CombatSessionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CombatSessionDto>> StartCombat([FromBody] StartCombatRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var combatSession = await _combatService.StartCombatAsync(userId, request.HeroIds, request.QuestId);
            return CreatedAtAction(nameof(GetCombatSession), new { combatSessionId = combatSession.Id }, combatSession);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Obtém detalhes de uma sessão de combate
    /// </summary>
    [HttpGet("{combatSessionId}")]
    [ProducesResponseType(typeof(CombatSessionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CombatSessionDetailDto>> GetCombatSession(int combatSessionId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var combatSession = await _combatService.GetActiveCombatSessionAsync(userId, combatSessionId);
            return Ok(combatSession);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Realiza uma rolagem de dado durante uma sessão de combate
    /// </summary>
    [HttpPost("roll-dice")]
    [ProducesResponseType(typeof(RollDiceResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RollDiceResultDto>> RollDice([FromBody] RollDiceRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var result = await _combatService.RollDiceAsync(userId, request.CombatSessionId, request.DiceType);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Completa uma sessão de combate (após vencer todos os inimigos)
    /// </summary>
    [HttpPost("complete")]
    [ProducesResponseType(typeof(CompleteCombatResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompleteCombatResultDto>> CompleteCombat([FromBody] CompleteCombatRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var result = await _combatService.CompleteCombatAsync(userId, request.CombatSessionId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém a sessão de combate ativa de um herói (se existir)
    /// </summary>
    [HttpGet("active/{heroId}")]
    [ProducesResponseType(typeof(CombatSessionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CombatSessionDetailDto>> GetActiveCombat(int heroId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var activeCombat = await _combatService.GetActiveCombatByHeroIdAsync(userId, heroId);
            if (activeCombat == null)
            {
                return NotFound("Nenhuma sessão de combate ativa encontrada para este herói.");
            }
            return Ok(activeCombat);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Permite que a party fuja de uma sessão de combate
    /// </summary>
    [HttpPost("flee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> FleeCombat([FromBody] FleeCombatRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            await _combatService.FleeCombatAsync(userId, request.CombatSessionId);
            return Ok("A party fugiu do combate.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}

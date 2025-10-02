using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.DTOs.Combat;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CombatController : ControllerBase
{
    private readonly ICombatService _combatService;

    public CombatController(ICombatService combatService)
    {
        _combatService = combatService;
    }

    [HttpPost("start")]
    [ProducesResponseType(typeof(CombatDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CombatDetailDto>> StartCombat([FromBody] StartCombatRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _combatService.StartCombatAsync(userId, request);
            return CreatedAtAction(nameof(GetActiveCombat), new { userId }, result);
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

    [HttpGet("active/{userId}")]
    [ProducesResponseType(typeof(CombatDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CombatDetailDto>> GetActiveCombat(int userId)
    {
        try
        {
            var combat = await _combatService.GetActiveCombatAsync(userId);
            if (combat == null)
            {
                return NotFound("Nenhum combate ativo encontrado.");
            }
            return Ok(combat);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("clear-active/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ClearActiveCombat(int userId)
    {
        try
        {
            await _combatService.ClearActiveCombatAsync(userId);
            return Ok(new { message = "Combate ativo cancelado com sucesso." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpPost("roll-dice")]
    [ProducesResponseType(typeof(RollDiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RollDiceResult>> RollDice([FromBody] RollDiceRequest request)
    {
        try
        {
            var result = await _combatService.RollDiceAsync(request.CombatSessionId, request.DiceType);
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
    }

    [HttpPost("enemy-attack")]
    [ProducesResponseType(typeof(EnemyAttackResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnemyAttackResult>> EnemyAttack([FromBody] EnemyAttackRequest request)
    {
        try
        {
            var result = await _combatService.EnemyAttackAsync(request.CombatSessionId);
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
    }

    [HttpPost("{combatSessionId}/complete")]
    [ProducesResponseType(typeof(CombatDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CombatDetailDto>> CompleteCombat(int combatSessionId)
    {
        try
        {
            var result = await _combatService.CompleteCombatAsync(combatSessionId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{combatSessionId}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CancelCombat(int combatSessionId)
    {
        try
        {
            var result = await _combatService.CancelCombatAsync(combatSessionId);
            if (!result)
            {
                return NotFound("Combate não encontrado.");
            }
            return Ok(new { message = "Combate cancelado com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("use-special-ability")]
    [ProducesResponseType(typeof(UseSpecialAbilityResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UseSpecialAbilityResult>> UseSpecialAbility([FromBody] UseSpecialAbilityRequest request)
    {
        try
        {
            var result = await _combatService.UseSpecialAbilityAsync(request.CombatSessionId, request.HeroId);
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
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("Usuário não autenticado.");
    }
}

public class EnemyAttackRequest
{
    public int CombatSessionId { get; set; }
}

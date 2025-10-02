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
    /// Inicia uma nova sessão de combate para uma quest
    /// </summary>
    [HttpPost("start")]
    [ProducesResponseType(typeof(CombatSessionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CombatSessionDto>> StartCombat([FromBody] StartCombatRequest request)
    {
        try
        {
            var session = await _combatService.StartCombatAsync(request.HeroId, request.QuestId);
            
            var dto = new CombatSessionDto
            {
                Id = session.Id,
                HeroId = session.HeroId,
                QuestId = session.QuestId,
                Status = session.Status.ToString(),
                StartedAt = session.StartedAt,
                Message = $"Combate iniciado! Prepare-se para enfrentar os desafios da quest."
            };

            return CreatedAtAction(nameof(GetActiveCombat), new { heroId = request.HeroId }, dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtém a sessão de combate ativa do herói
    /// </summary>
    [HttpGet("active/{heroId}")]
    [ProducesResponseType(typeof(CombatSessionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CombatSessionDetailDto>> GetActiveCombat(int heroId)
    {
        var session = await _combatService.GetActiveCombatAsync(heroId);
        
        if (session == null)
        {
            return NotFound("Nenhuma sessão de combate ativa encontrada para este herói.");
        }

        var dto = new CombatSessionDetailDto
        {
            Id = session.Id,
            HeroId = session.HeroId,
            QuestId = session.QuestId,
            QuestName = session.Quest.Name,
            Status = session.Status.ToString(),
            StartedAt = session.StartedAt,
            Enemies = session.Quest.QuestEnemies.Select(qe => new EnemyInfoDto
            {
                Id = qe.Enemy.Id,
                Name = qe.Enemy.Name,
                Type = qe.Enemy.Type,
                RequiredDiceType = qe.Enemy.RequiredDiceType.ToString(),
                MinimumRoll = qe.Enemy.MinimumRoll,
                IsBoss = qe.Enemy.IsBoss
            }).ToList(),
            CombatLogs = session.CombatLogs.Select(log => new CombatLogDto
            {
                Id = log.Id,
                Action = log.Action,
                DiceUsed = log.DiceUsed?.ToString(),
                DiceResult = log.DiceResult,
                RequiredRoll = log.RequiredRoll,
                Success = log.Success,
                Details = log.Details,
                Timestamp = log.Timestamp
            }).ToList()
        };

        return Ok(dto);
    }

    /// <summary>
    /// Rola um dado contra um inimigo específico
    /// </summary>
    [HttpPost("roll-dice")]
    [ProducesResponseType(typeof(RollDiceResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RollDiceResultDto>> RollDice([FromBody] RollDiceRequest request)
    {
        try
        {
            var diceType = Enum.Parse<DiceType>(request.DiceType);
            var result = await _combatService.RollDiceAsync(
                request.CombatSessionId, 
                request.EnemyId, 
                diceType);

            return Ok(new RollDiceResultDto
            {
                Success = result.Success,
                RollResult = result.RollResult,
                Message = result.Message,
                DiceType = request.DiceType
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException)
        {
            return BadRequest("Tipo de dado inválido.");
        }
    }

    /// <summary>
    /// Completa a sessão de combate e processa drops
    /// </summary>
    [HttpPost("complete")]
    [ProducesResponseType(typeof(CompleteCombatResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CompleteCombatResultDto>> CompleteCombat([FromBody] CompleteCombatRequest request)
    {
        try
        {
            var result = await _combatService.CompleteCombatAsync(request.CombatSessionId);

            return Ok(new CompleteCombatResultDto
            {
                Status = result.Status.ToString(),
                DroppedItems = result.Drops.Select(item => new DroppedItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Rarity = item.GetFullRarityName(),
                    Type = item.Type,
                    BonusStrength = item.BonusStrength,
                    BonusIntelligence = item.BonusIntelligence,
                    BonusDexterity = item.BonusDexterity
                }).ToList(),
                Message = result.Status == CombatStatus.Victory 
                    ? $"🎉 Vitória! Você obteve {result.Drops.Count} item(ns)!"
                    : result.Status == CombatStatus.Fled 
                        ? "Você fugiu do combate."
                        : "Você foi derrotado."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Foge do combate atual
    /// </summary>
    [HttpPost("flee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Flee([FromBody] FleeCombatRequest request)
    {
        var success = await _combatService.FleeCombatAsync(request.CombatSessionId);
        
        if (!success)
        {
            return BadRequest("Não foi possível fugir do combate.");
        }

        return Ok(new { message = "Você fugiu do combate." });
    }
}


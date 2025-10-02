using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Rewards;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Controllers;

/// <summary>
/// Gerenciamento de recompensas vinculadas às quests
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "💰 Recompensas")]
public class RewardsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<RewardsController> _logger;
    
    public RewardsController(
        ApplicationDbContext context, 
        IMapper mapper,
        ILogger<RewardsController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }
    
    /// <summary>
    /// Obtém todas as recompensas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RewardDto>>> GetAll()
    {
        var rewards = await _context.Rewards.ToListAsync();
        return Ok(_mapper.Map<IEnumerable<RewardDto>>(rewards));
    }
    
    /// <summary>
    /// Obtém uma recompensa por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RewardDto>> GetById(int id)
    {
        var reward = await _context.Rewards.FindAsync(id);
        
        if (reward == null)
        {
            return NotFound($"Recompensa com ID {id} não encontrada");
        }
        
        return Ok(_mapper.Map<RewardDto>(reward));
    }
    
    /// <summary>
    /// Obtém recompensas por Quest ID
    /// </summary>
    [HttpGet("quest/{questId}")]
    public async Task<ActionResult<IEnumerable<RewardDto>>> GetByQuestId(int questId)
    {
        var rewards = await _context.Rewards
            .Where(r => r.QuestId == questId)
            .ToListAsync();
        
        return Ok(_mapper.Map<IEnumerable<RewardDto>>(rewards));
    }
    
    /// <summary>
    /// Cria uma nova recompensa (APENAS ADMIN)
    /// </summary>
    /// <response code="403">Usuário não tem permissão de administrador</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RewardDto>> Create([FromBody] CreateRewardRequest request)
    {
        // Verificar se a quest existe
        var questExists = await _context.Quests.AnyAsync(q => q.Id == request.QuestId);
        if (!questExists)
        {
            return NotFound($"Quest com ID {request.QuestId} não encontrada");
        }
        
        var reward = _mapper.Map<Reward>(request);
        
        _context.Rewards.Add(reward);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Recompensa criada com ID {Id} para Quest {QuestId}", reward.Id, reward.QuestId);
        
        var rewardDto = _mapper.Map<RewardDto>(reward);
        return CreatedAtAction(nameof(GetById), new { id = reward.Id }, rewardDto);
    }
    
    /// <summary>
    /// Deleta uma recompensa (APENAS ADMIN)
    /// </summary>
    /// <response code="403">Usuário não tem permissão de administrador</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var reward = await _context.Rewards.FindAsync(id);
        
        if (reward == null)
        {
            return NotFound($"Recompensa com ID {id} não encontrada");
        }
        
        _context.Rewards.Remove(reward);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Recompensa {Id} deletada", id);
        
        return NoContent();
    }
}


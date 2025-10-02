using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Enemies;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Controllers;

/// <summary>
/// Gerenciamento de inimigos e adversários do jogo
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "👹 Inimigos")]
public class EnemiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<EnemiesController> _logger;
    
    public EnemiesController(
        ApplicationDbContext context, 
        IMapper mapper,
        ILogger<EnemiesController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }
    
    /// <summary>
    /// Obtém todos os inimigos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnemyDto>>> GetAll()
    {
        var enemies = await _context.Enemies.ToListAsync();
        return Ok(_mapper.Map<IEnumerable<EnemyDto>>(enemies));
    }
    
    /// <summary>
    /// Obtém um inimigo por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EnemyDto>> GetById(int id)
    {
        var enemy = await _context.Enemies.FindAsync(id);
        
        if (enemy == null)
        {
            return NotFound($"Inimigo com ID {id} não encontrado");
        }
        
        return Ok(_mapper.Map<EnemyDto>(enemy));
    }
    
    /// <summary>
    /// Cria um novo inimigo (APENAS ADMIN)
    /// </summary>
    /// <response code="403">Usuário não tem permissão de administrador</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EnemyDto>> Create([FromBody] CreateEnemyRequest request)
    {
        var enemy = _mapper.Map<Enemy>(request);
        
        _context.Enemies.Add(enemy);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Inimigo {Name} criado com ID {Id}", enemy.Name, enemy.Id);
        
        var enemyDto = _mapper.Map<EnemyDto>(enemy);
        return CreatedAtAction(nameof(GetById), new { id = enemy.Id }, enemyDto);
    }
    
    /// <summary>
    /// Atualiza um inimigo existente (APENAS ADMIN)
    /// </summary>
    /// <response code="403">Usuário não tem permissão de administrador</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EnemyDto>> Update(int id, [FromBody] UpdateEnemyRequest request)
    {
        var enemy = await _context.Enemies.FindAsync(id);
        
        if (enemy == null)
        {
            return NotFound($"Inimigo com ID {id} não encontrado");
        }
        
        _mapper.Map(request, enemy);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Inimigo {Id} atualizado", id);
        
        return Ok(_mapper.Map<EnemyDto>(enemy));
    }
    
    /// <summary>
    /// Deleta um inimigo (APENAS ADMIN)
    /// </summary>
    /// <response code="403">Usuário não tem permissão de administrador</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var enemy = await _context.Enemies.FindAsync(id);
        
        if (enemy == null)
        {
            return NotFound($"Inimigo com ID {id} não encontrado");
        }
        
        _context.Enemies.Remove(enemy);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Inimigo {Id} deletado", id);
        
        return NoContent();
    }
}


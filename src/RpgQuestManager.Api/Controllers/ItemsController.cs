using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Items;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Controllers;

/// <summary>
/// Gerenciamento de itens equipáveis (espadas, poções, armaduras, etc)
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "🗡️ Itens")]
public class ItemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ItemsController> _logger;
    
    public ItemsController(
        ApplicationDbContext context, 
        IMapper mapper,
        ILogger<ItemsController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }
    
    /// <summary>
    /// Obtém todos os itens
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAll()
    {
        var items = await _context.Items.ToListAsync();
        return Ok(_mapper.Map<IEnumerable<ItemDto>>(items));
    }
    
    /// <summary>
    /// Obtém um item por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetById(int id)
    {
        var item = await _context.Items.FindAsync(id);
        
        if (item == null)
        {
            return NotFound($"Item com ID {id} não encontrado");
        }
        
        return Ok(_mapper.Map<ItemDto>(item));
    }
    
    /// <summary>
    /// Cria um novo item (APENAS ADMIN)
    /// </summary>
    /// <response code="403">Usuário não tem permissão de administrador</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ItemDto>> Create([FromBody] CreateItemRequest request)
    {
        var item = _mapper.Map<Item>(request);
        
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Item {Name} criado com ID {Id}", item.Name, item.Id);
        
        var itemDto = _mapper.Map<ItemDto>(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, itemDto);
    }
    
    /// <summary>
    /// Deleta um item (APENAS ADMIN)
    /// </summary>
    /// <response code="403">Usuário não tem permissão de administrador</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var item = await _context.Items.FindAsync(id);
        
        if (item == null)
        {
            return NotFound($"Item com ID {id} não encontrado");
        }
        
        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Item {Id} deletado", id);
        
        return NoContent();
    }
}


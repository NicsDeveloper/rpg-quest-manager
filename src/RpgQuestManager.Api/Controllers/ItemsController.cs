using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Items;
using RpgQuestManager.Api.Models;
using System.Security.Claims;

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
        var item = new Item
        {
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            Rarity = request.Rarity,
            BonusStrength = request.BonusStrength,
            BonusIntelligence = request.BonusIntelligence,
            BonusDexterity = request.BonusDexterity,
            Value = request.Value,
            CreatedAt = DateTime.UtcNow
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Item {ItemName} criado por admin", item.Name);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, _mapper.Map<ItemDto>(item));
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
        
        _logger.LogInformation("Item {ItemName} deletado por admin", item.Name);
        
        return NoContent();
    }

    /// <summary>
    /// Usa um item consumível (ex: poção de XP)
    /// </summary>
    [HttpPost("{itemId}/use")]
    public async Task<ActionResult<ItemUsageResultDto>> UseItem(int itemId, [FromQuery] int quantity = 1)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.UserId == userId);
        
        if (hero == null)
        {
            return BadRequest(new { message = "Você não possui um herói." });
        }
        
        var heroItem = await _context.HeroItems
            .Include(hi => hi.Item)
            .FirstOrDefaultAsync(hi => hi.HeroId == hero.Id && hi.ItemId == itemId);
            
        if (heroItem == null)
        {
            return NotFound(new { message = "Item não encontrado no inventário." });
        }
        
        if (heroItem.Quantity < quantity)
        {
            return BadRequest(new { message = $"Quantidade insuficiente. Disponível: {heroItem.Quantity}, Solicitado: {quantity}" });
        }
        
        if (!heroItem.Item.IsConsumable)
        {
            return BadRequest(new { message = "Este item não é consumível." });
        }
        
        // Aplicar efeitos do item
        var result = new ItemUsageResultDto
        {
            Success = true,
            ItemName = heroItem.Item.Name,
            QuantityUsed = quantity,
            Message = $"{heroItem.Item.Name} usado com sucesso!"
        };
        
        // Aplicar bônus de XP se for poção de XP
        if (heroItem.Item.PercentageXpBonus.HasValue && heroItem.Item.PercentageXpBonus > 0)
        {
            var xpNeeded = hero.GetExperienceForNextLevel();
            var xpGained = (int)(xpNeeded * heroItem.Item.PercentageXpBonus.Value * quantity);
            
            hero.Experience += xpGained;
            result.XpGained = xpGained;
            result.Message += $" Ganhou {xpGained} XP!";
            
            // Verificar level up
            var oldLevel = hero.Level;
            if (hero.CanLevelUp())
            {
                hero.LevelUp();
                if (hero.Level > oldLevel)
                {
                    result.LeveledUp = true;
                    result.NewLevel = hero.Level;
                    result.Message += $" Subiu para o nível {hero.Level}!";
                }
            }
        }
        
        // Remover item do inventário
        heroItem.Quantity -= quantity;
        if (heroItem.Quantity <= 0)
        {
            _context.HeroItems.Remove(heroItem);
        }
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🧪 Item {ItemName} usado por herói {HeroName}. XP ganho: {XpGained}, Level up: {LeveledUp}", 
            heroItem.Item.Name, hero.Name, result.XpGained, result.LeveledUp);
        
        return Ok(result);
    }
}
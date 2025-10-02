using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Heroes;
using RpgQuestManager.Api.DTOs.Items;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

/// <summary>
/// Gerenciamento de heróis com atributos, níveis e inventário
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "⚔️ Heróis")]
public class HeroesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ILogger<HeroesController> _logger;
    
    public HeroesController(
        ApplicationDbContext context, 
        IMapper mapper,
        ICacheService cacheService,
        ILogger<HeroesController> logger)
    {
        _context = context;
        _mapper = mapper;
        _cacheService = cacheService;
        _logger = logger;
    }
    
    /// <summary>
    /// Lista todos os heróis cadastrados no sistema
    /// </summary>
    /// <returns>Lista de heróis com atributos, nível, XP e ouro</returns>
    /// <remarks>
    /// Retorna todos os heróis sem paginação.
    /// 
    /// Cada herói contém:
    /// * Atributos: Força, Inteligência, Destreza
    /// * Progressão: Nível atual e experiência
    /// * Riqueza: Total de ouro acumulado
    /// </remarks>
    /// <response code="200">Lista de heróis retornada com sucesso</response>
    /// <response code="401">Token JWT inválido ou ausente</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HeroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<HeroDto>>> GetAll()
    {
        var heroes = await _context.Heroes.ToListAsync();
        return Ok(_mapper.Map<IEnumerable<HeroDto>>(heroes));
    }
    
    /// <summary>
    /// Obtém um herói por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<HeroDto>> GetById(int id)
    {
        // Tentar buscar do cache
        var cacheKey = $"hero:{id}";
        var cachedHero = await _cacheService.GetAsync<HeroDto>(cacheKey);
        
        if (cachedHero != null)
        {
            _logger.LogInformation("Herói {Id} obtido do cache", id);
            return Ok(cachedHero);
        }
        
        var hero = await _context.Heroes.FindAsync(id);
        
        if (hero == null)
        {
            return NotFound($"Herói com ID {id} não encontrado");
        }
        
        var heroDto = _mapper.Map<HeroDto>(hero);
        
        // Salvar no cache por 5 minutos
        await _cacheService.SetAsync(cacheKey, heroDto, TimeSpan.FromMinutes(5));
        
        return Ok(heroDto);
    }
    
    /// <summary>
    /// Obtém o ranking dos heróis mais fortes (CACHED por 10 minutos)
    /// </summary>
    /// <param name="limit">Quantidade de heróis no ranking (padrão: 10)</param>
    /// <returns>Lista dos heróis mais fortes ordenados por nível e XP</returns>
    /// <remarks>
    /// Este endpoint utiliza cache Redis para melhor performance.
    /// 
    /// Critérios de ordenação:
    /// 1. Nível (descendente)
    /// 2. Experiência (descendente)
    /// 
    /// Cache: 10 minutos
    /// 
    /// Exemplo:
    /// 
    ///     GET /api/v1/heroes/strongest?limit=5
    ///     
    /// </remarks>
    /// <response code="200">Ranking retornado com sucesso (pode vir do cache)</response>
    [HttpGet("strongest")]
    [ProducesResponseType(typeof(IEnumerable<HeroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HeroDto>>> GetStrongest([FromQuery] int limit = 10)
    {
        var cacheKey = $"heroes:strongest:{limit}";
        var cached = await _cacheService.GetAsync<IEnumerable<HeroDto>>(cacheKey);
        
        if (cached != null)
        {
            _logger.LogInformation("Heróis mais fortes obtidos do cache");
            return Ok(cached);
        }
        
        var heroes = await _context.Heroes
            .OrderByDescending(h => h.Level)
            .ThenByDescending(h => h.Experience)
            .Take(limit)
            .ToListAsync();
        
        var heroesDto = _mapper.Map<IEnumerable<HeroDto>>(heroes);
        
        // Salvar no cache por 10 minutos
        await _cacheService.SetAsync(cacheKey, heroesDto, TimeSpan.FromMinutes(10));
        
        return Ok(heroesDto);
    }
    
    /// <summary>
    /// Cria um novo herói no sistema
    /// </summary>
    /// <param name="request">Dados do herói a ser criado</param>
    /// <returns>Herói criado com ID gerado</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/v1/heroes
    ///     {
    ///         "name": "Aragorn",
    ///         "class": "Guerreiro",
    ///         "strength": 18,
    ///         "intelligence": 12,
    ///         "dexterity": 15
    ///     }
    ///     
    /// Classes disponíveis:
    /// * Guerreiro - Focado em força e resistência
    /// * Mago - Focado em inteligência e magia
    /// * Arqueiro - Focado em destreza e precisão
    /// * Paladino - Balanceado entre força e magia
    /// * Ladino - Focado em agilidade e furtividade
    /// 
    /// Atributos:
    /// * Valor mínimo: 1
    /// * Valor máximo: 100
    /// * Padrão: 10 se não informado
    /// 
    /// O herói começa no nível 1 com 0 XP e 0 ouro.
    /// </remarks>
    /// <response code="201">Herói criado com sucesso</response>
    /// <response code="400">Dados inválidos (classe inexistente ou atributos fora do range)</response>
    [HttpPost]
    [ProducesResponseType(typeof(HeroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HeroDto>> Create([FromBody] CreateHeroRequest request)
    {
        var hero = _mapper.Map<Hero>(request);
        
        _context.Heroes.Add(hero);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Herói {Name} criado com ID {Id}", hero.Name, hero.Id);
        
        var heroDto = _mapper.Map<HeroDto>(hero);
        return CreatedAtAction(nameof(GetById), new { id = hero.Id }, heroDto);
    }
    
    /// <summary>
    /// Atualiza um herói existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<HeroDto>> Update(int id, [FromBody] UpdateHeroRequest request)
    {
        var hero = await _context.Heroes.FindAsync(id);
        
        if (hero == null)
        {
            return NotFound($"Herói com ID {id} não encontrado");
        }
        
        _mapper.Map(request, hero);
        await _context.SaveChangesAsync();
        
        // Invalidar cache
        await _cacheService.RemoveAsync($"hero:{id}");
        
        _logger.LogInformation("Herói {Id} atualizado", id);
        
        return Ok(_mapper.Map<HeroDto>(hero));
    }
    
    /// <summary>
    /// Deleta um herói
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var hero = await _context.Heroes.FindAsync(id);
        
        if (hero == null)
        {
            return NotFound($"Herói com ID {id} não encontrado");
        }
        
        _context.Heroes.Remove(hero);
        await _context.SaveChangesAsync();
        
        // Invalidar cache
        await _cacheService.RemoveAsync($"hero:{id}");
        
        _logger.LogInformation("Herói {Id} deletado", id);
        
        return NoContent();
    }
    
    /// <summary>
    /// Obtém o inventário de um herói
    /// </summary>
    [HttpGet("{id}/inventory")]
    public async Task<ActionResult<IEnumerable<object>>> GetInventory(int id)
    {
        var heroExists = await _context.Heroes.AnyAsync(h => h.Id == id);
        
        if (!heroExists)
        {
            return NotFound($"Herói com ID {id} não encontrado");
        }
        
        var inventory = await _context.HeroItems
            .Include(hi => hi.Item)
            .Where(hi => hi.HeroId == id)
            .Select(hi => new
            {
                hi.Id,
                hi.ItemId,
                ItemName = hi.Item.Name,
                hi.Quantity,
                hi.IsEquipped,
                hi.AcquiredAt,
                Item = _mapper.Map<ItemDto>(hi.Item)
            })
            .ToListAsync();
        
        return Ok(inventory);
    }
    
    /// <summary>
    /// Adiciona um item ao inventário do herói
    /// </summary>
    [HttpPost("{id}/inventory/{itemId}")]
    public async Task<ActionResult> AddItemToInventory(int id, int itemId, [FromQuery] int quantity = 1)
    {
        var hero = await _context.Heroes.FindAsync(id);
        if (hero == null)
        {
            return NotFound($"Herói com ID {id} não encontrado");
        }
        
        var item = await _context.Items.FindAsync(itemId);
        if (item == null)
        {
            return NotFound($"Item com ID {itemId} não encontrado");
        }
        
        var existingHeroItem = await _context.HeroItems
            .FirstOrDefaultAsync(hi => hi.HeroId == id && hi.ItemId == itemId);
        
        if (existingHeroItem != null)
        {
            existingHeroItem.Quantity += quantity;
        }
        else
        {
            var heroItem = new HeroItem
            {
                HeroId = id,
                ItemId = itemId,
                Quantity = quantity
            };
            _context.HeroItems.Add(heroItem);
        }
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Item {ItemId} adicionado ao inventário do herói {HeroId}", itemId, id);
        
        return Ok(new { message = "Item adicionado ao inventário com sucesso" });
    }
    
    /// <summary>
    /// Equipa/desequipa um item
    /// </summary>
    [HttpPut("{id}/inventory/{heroItemId}/equip")]
    public async Task<ActionResult> ToggleEquipItem(int id, int heroItemId)
    {
        var heroItem = await _context.HeroItems
            .Include(hi => hi.Hero)
            .FirstOrDefaultAsync(hi => hi.Id == heroItemId && hi.HeroId == id);
        
        if (heroItem == null)
        {
            return NotFound("Item não encontrado no inventário do herói");
        }
        
        heroItem.IsEquipped = !heroItem.IsEquipped;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "Item {ItemId} {Action} do herói {HeroId}", 
            heroItem.ItemId, 
            heroItem.IsEquipped ? "equipado" : "desequipado",
            id
        );
        
        return Ok(new { isEquipped = heroItem.IsEquipped });
    }
}


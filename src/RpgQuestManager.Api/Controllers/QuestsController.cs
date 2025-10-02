using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Quests;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class QuestsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IQuestService _questService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<QuestsController> _logger;
    
    public QuestsController(
        ApplicationDbContext context, 
        IMapper mapper,
        IQuestService questService,
        ICacheService cacheService,
        ILogger<QuestsController> logger)
    {
        _context = context;
        _mapper = mapper;
        _questService = questService;
        _cacheService = cacheService;
        _logger = logger;
    }
    
    /// <summary>
    /// Obtém todas as quests
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestDto>>> GetAll()
    {
        var quests = await _context.Quests.ToListAsync();
        return Ok(_mapper.Map<IEnumerable<QuestDto>>(quests));
    }
    
    /// <summary>
    /// Obtém uma quest por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<QuestDto>> GetById(int id)
    {
        var quest = await _context.Quests.FindAsync(id);
        
        if (quest == null)
        {
            return NotFound($"Quest com ID {id} não encontrada");
        }
        
        return Ok(_mapper.Map<QuestDto>(quest));
    }
    
    /// <summary>
    /// Obtém as quests mais jogadas (cache)
    /// </summary>
    [HttpGet("most-played")]
    public async Task<ActionResult<IEnumerable<object>>> GetMostPlayed([FromQuery] int limit = 10)
    {
        var cacheKey = $"quests:most-played:{limit}";
        var cached = await _cacheService.GetAsync<IEnumerable<object>>(cacheKey);
        
        if (cached != null)
        {
            _logger.LogInformation("Quests mais jogadas obtidas do cache");
            return Ok(cached);
        }
        
        var mostPlayed = await _context.HeroQuests
            .GroupBy(hq => hq.Quest)
            .OrderByDescending(g => g.Count())
            .Take(limit)
            .Select(g => new
            {
                Quest = _mapper.Map<QuestDto>(g.Key),
                CompletionCount = g.Count(hq => hq.IsCompleted),
                AttemptCount = g.Count()
            })
            .ToListAsync();
        
        // Salvar no cache por 10 minutos
        await _cacheService.SetAsync(cacheKey, mostPlayed, TimeSpan.FromMinutes(10));
        
        return Ok(mostPlayed);
    }
    
    /// <summary>
    /// Cria uma nova quest
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<QuestDto>> Create([FromBody] CreateQuestRequest request)
    {
        var quest = _mapper.Map<Quest>(request);
        
        _context.Quests.Add(quest);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Quest {Name} criada com ID {Id}", quest.Name, quest.Id);
        
        var questDto = _mapper.Map<QuestDto>(quest);
        return CreatedAtAction(nameof(GetById), new { id = quest.Id }, questDto);
    }
    
    /// <summary>
    /// Atualiza uma quest existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<QuestDto>> Update(int id, [FromBody] UpdateQuestRequest request)
    {
        var quest = await _context.Quests.FindAsync(id);
        
        if (quest == null)
        {
            return NotFound($"Quest com ID {id} não encontrada");
        }
        
        _mapper.Map(request, quest);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Quest {Id} atualizada", id);
        
        return Ok(_mapper.Map<QuestDto>(quest));
    }
    
    /// <summary>
    /// Deleta uma quest
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var quest = await _context.Quests.FindAsync(id);
        
        if (quest == null)
        {
            return NotFound($"Quest com ID {id} não encontrada");
        }
        
        _context.Quests.Remove(quest);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Quest {Id} deletada", id);
        
        return NoContent();
    }
    
    /// <summary>
    /// Completa uma quest e recompensa o herói
    /// </summary>
    [HttpPost("complete")]
    public async Task<ActionResult<QuestDto>> CompleteQuest([FromBody] CompleteQuestRequest request)
    {
        var result = await _questService.CompleteQuestAsync(request.HeroId, request.QuestId);
        
        // Invalidar caches relevantes
        await _cacheService.RemoveAsync($"hero:{request.HeroId}");
        await _cacheService.RemoveAsync("heroes:strongest:10");
        
        return Ok(result);
    }
}


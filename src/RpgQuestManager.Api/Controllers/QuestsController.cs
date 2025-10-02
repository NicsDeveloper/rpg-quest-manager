using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Quests;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
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
    
    [HttpGet("catalog")]
    public async Task<ActionResult<List<QuestDto>>> GetCatalog()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        var quests = await _context.Quests.ToListAsync();
        var questDtos = _mapper.Map<List<QuestDto>>(quests);

        if (userRole != "Admin")
        {
            var hero = await _context.Heroes
                .FirstOrDefaultAsync(h => h.UserId == userId && h.PartySlot != null && !h.IsDeleted);
            
            if (hero != null)
            {
                var heroQuests = await _context.HeroQuests
                    .Where(hq => hq.HeroId == hero.Id)
                    .ToListAsync();

                var acceptedQuestIds = heroQuests
                    .Where(hq => !hq.IsCompleted)
                    .Select(hq => hq.QuestId)
                    .ToList();

                var completedQuestIds = heroQuests
                    .Where(hq => hq.IsCompleted)
                    .Select(hq => hq.QuestId)
                    .ToList();

                // Remove quests completadas da lista
                questDtos = questDtos.Where(q => !completedQuestIds.Contains(q.Id)).ToList();

                foreach (var quest in questDtos)
                {
                    quest.IsAccepted = acceptedQuestIds.Contains(quest.Id);
                    quest.CanAccept = !quest.IsAccepted.Value && 
                                     hero.Level >= quest.RequiredLevel &&
                                     (quest.RequiredClass == "Any" || quest.RequiredClass == hero.Class);
                }
            }
        }

        return Ok(questDtos);
    }

    [HttpGet("my-quests")]
    public async Task<ActionResult<List<QuestDto>>> GetMyQuests()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.UserId == userId);
        
        if (hero == null)
        {
            return Ok(new List<QuestDto>());
        }

        var heroQuestIds = await _context.HeroQuests
            .Where(hq => hq.HeroId == hero.Id)
            .Select(hq => hq.QuestId)
            .ToListAsync();

        var quests = await _context.Quests
            .Where(q => heroQuestIds.Contains(q.Id))
            .ToListAsync();

        var questDtos = _mapper.Map<List<QuestDto>>(quests);
        
        foreach (var quest in questDtos)
        {
            quest.IsAccepted = true;
        }

        return Ok(questDtos);
    }

    [HttpGet]
    public async Task<ActionResult<List<QuestDto>>> GetAll()
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (userRole == "Admin")
        {
            var quests = await _context.Quests.ToListAsync();
            return Ok(_mapper.Map<List<QuestDto>>(quests));
        }
        else
        {
            return await GetMyQuests();
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<QuestDto>> GetById(int id)
    {
        var quest = await _context.Quests.FirstOrDefaultAsync(q => q.Id == id);
        
        if (quest == null)
        {
            return NotFound($"Quest com ID {id} não encontrada");
        }
        
        return Ok(_mapper.Map<QuestDto>(quest));
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<QuestDto>> Create([FromBody] CreateQuestRequest request)
    {
        var quest = _mapper.Map<Quest>(request);
        _context.Quests.Add(quest);
        await _context.SaveChangesAsync();
        
        await _cacheService.RemoveAsync("quests:popular:10");
        
        return CreatedAtAction(nameof(GetById), new { id = quest.Id }, _mapper.Map<QuestDto>(quest));
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<QuestDto>> Update(int id, [FromBody] UpdateQuestRequest request)
    {
        var quest = await _context.Quests.FindAsync(id);
        
        if (quest == null)
        {
            return NotFound($"Quest com ID {id} não encontrada");
        }
        
        _mapper.Map(request, quest);
        await _context.SaveChangesAsync();
        
        await _cacheService.RemoveAsync("quests:popular:10");
        
        return Ok(_mapper.Map<QuestDto>(quest));
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var quest = await _context.Quests.FindAsync(id);
        
        if (quest == null)
        {
            return NotFound($"Quest com ID {id} não encontrada");
        }
        
        _context.Quests.Remove(quest);
        await _context.SaveChangesAsync();
        
        await _cacheService.RemoveAsync("quests:popular:10");
        
        return NoContent();
    }

    [HttpPost("{id}/accept")]
    public async Task<ActionResult> AcceptQuest(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.UserId == userId);
        
        if (hero == null)
        {
            return BadRequest(new { message = "Você não possui um herói." });
        }

        // ⚠️ NOVO: Verificar se já tem uma quest ativa (não completada)
        var hasActiveQuest = await _context.HeroQuests
            .AnyAsync(hq => hq.HeroId == hero.Id && !hq.IsCompleted);
        
        if (hasActiveQuest)
        {
            return BadRequest(new { message = "Você já tem uma missão ativa! Complete-a antes de aceitar outra." });
        }

        var quest = await _context.Quests.FindAsync(id);
        
        if (quest == null)
        {
            return NotFound(new { message = "Quest não encontrada." });
        }

        var alreadyAccepted = await _context.HeroQuests
            .AnyAsync(hq => hq.HeroId == hero.Id && hq.QuestId == id);
        
        if (alreadyAccepted)
        {
            return BadRequest(new { message = "Você já aceitou esta quest." });
        }

        if (hero.Level < quest.RequiredLevel)
        {
            return BadRequest(new { message = $"Seu herói precisa estar no nível {quest.RequiredLevel} para aceitar esta quest." });
        }

        if (quest.RequiredClass != "Any" && quest.RequiredClass != hero.Class)
        {
            return BadRequest(new { message = $"Esta quest é apenas para a classe {quest.RequiredClass}." });
        }

        var heroQuest = new HeroQuest
        {
            HeroId = hero.Id,
            QuestId = id,
            StartedAt = DateTime.UtcNow,
            IsCompleted = false
        };

        _context.HeroQuests.Add(heroQuest);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Herói {HeroName} aceitou a quest {QuestName}", hero.Name, quest.Name);

        return Ok(new { message = $"Quest '{quest.Name}' aceita com sucesso!" });
    }

    [HttpPost("complete")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<QuestDto>> CompleteQuest([FromBody] CompleteQuestRequest request)
    {
        var result = await _questService.CompleteQuestAsync(request.HeroId, request.QuestId);
        
        await _cacheService.RemoveAsync($"hero:{request.HeroId}");
        await _cacheService.RemoveAsync("heroes:strongest:10");
        await _cacheService.RemoveAsync("quests:popular:10");
        
        return Ok(_mapper.Map<QuestDto>(result));
    }
}

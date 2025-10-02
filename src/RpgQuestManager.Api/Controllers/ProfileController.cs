using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Heroes;
using RpgQuestManager.Api.DTOs.Items;
using RpgQuestManager.Api.DTOs.Quests;
using RpgQuestManager.Api.Models;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "👤 Profile")]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(ApplicationDbContext context, ILogger<ProfileController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém o perfil do primeiro herói na party ativa do usuário logado
    /// </summary>
    [HttpGet("my-hero")]
    [ProducesResponseType(typeof(HeroProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HeroProfileDto>> GetMyHeroProfile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Busca o primeiro herói da party ativa (PartySlot == 1)
        var hero = await _context.Heroes
            .Include(h => h.HeroItems)
                .ThenInclude(hi => hi.Item)
            .Include(h => h.HeroQuests)
                .ThenInclude(hq => hq.Quest)
            .FirstOrDefaultAsync(h => h.UserId == userId && h.IsInActiveParty && h.PartySlot == 1);

        if (hero == null)
        {
            return NotFound("Nenhum herói na party ativa. Adicione um herói à party primeiro.");
        }

        var heroProfile = new HeroProfileDto
        {
            Id = hero.Id,
            Name = hero.Name,
            Class = hero.Class,
            Level = hero.Level,
            Experience = hero.Experience,
            Strength = hero.Strength,
            Intelligence = hero.Intelligence,
            Dexterity = hero.Dexterity,
            Gold = hero.Gold,
            CreatedAt = hero.CreatedAt,
            Items = hero.HeroItems.Select(hi => new HeroItemDto
            {
                Id = hi.Id,
                ItemId = hi.ItemId,
                ItemName = hi.Item.Name,
                ItemDescription = hi.Item.Description,
                ItemType = hi.Item.Type,
                Quantity = hi.Quantity,
                IsEquipped = hi.IsEquipped,
                AcquiredAt = hi.AcquiredAt,
                BonusStrength = hi.Item.BonusStrength,
                BonusIntelligence = hi.Item.BonusIntelligence,
                BonusDexterity = hi.Item.BonusDexterity
            }).ToList(),
            CompletedQuests = hero.HeroQuests
                .Where(hq => hq.IsCompleted)
                .Select(hq => new QuestDto
                {
                    Id = hq.Quest.Id,
                    Name = hq.Quest.Name,
                    Description = hq.Quest.Description,
                    Difficulty = hq.Quest.Difficulty,
                    ExperienceReward = hq.Quest.ExperienceReward,
                    GoldReward = hq.Quest.GoldReward,
                    CreatedAt = hq.Quest.CreatedAt
                }).ToList()
        };

        return Ok(heroProfile);
    }

    /// <summary>
    /// Cria um novo herói para o usuário logado
    /// </summary>
    /// <remarks>
    /// Requisito: Ter pelo menos um herói nível 5 ou superior
    /// </remarks>
    [HttpPost("create-hero")]
    [ProducesResponseType(typeof(HeroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HeroDto>> CreateHero([FromBody] CreateHeroRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Verifica se já tem pelo menos um herói nível 5+
        var heroes = await _context.Heroes.Where(h => h.UserId == userId).ToListAsync();
        
        // Se já tem heróis, verifica se algum está nível 5+
        if (heroes.Count > 0)
        {
            var hasLevel5Hero = heroes.Any(h => h.Level >= 5);
            if (!hasLevel5Hero)
            {
                return BadRequest("Você precisa ter pelo menos um herói nível 5 ou superior para criar um novo herói.");
            }
        }

        var hero = new Hero
        {
            Name = request.Name,
            Class = request.Class,
            Level = 1,
            Experience = 0,
            Strength = request.Strength,
            Intelligence = request.Intelligence,
            Dexterity = request.Dexterity,
            Gold = 0,
            UserId = userId,
            IsInActiveParty = false, // Criado fora da party
            PartySlot = null,
            CreatedAt = DateTime.UtcNow
        };

        _context.Heroes.Add(hero);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Novo herói {HeroName} criado para o usuário {UserId}", hero.Name, userId);

        var heroDto = new HeroDto
        {
            Id = hero.Id,
            Name = hero.Name,
            Class = hero.Class,
            Level = hero.Level,
            Experience = hero.Experience,
            Strength = hero.Strength,
            Intelligence = hero.Intelligence,
            Dexterity = hero.Dexterity,
            Gold = hero.Gold,
            CreatedAt = hero.CreatedAt
        };

        return CreatedAtAction(nameof(GetMyHeroes), new { }, heroDto);
    }

    /// <summary>
    /// Lista todos os heróis do usuário logado
    /// </summary>
    [HttpGet("my-heroes")]
    [ProducesResponseType(typeof(List<HeroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<HeroDto>>> GetMyHeroes()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var heroes = await _context.Heroes
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.IsInActiveParty)
            .ThenBy(h => h.PartySlot)
            .ThenByDescending(h => h.Level)
            .Select(h => new HeroDto
            {
                Id = h.Id,
                Name = h.Name,
                Class = h.Class,
                Level = h.Level,
                Experience = h.Experience,
                Strength = h.Strength,
                Intelligence = h.Intelligence,
                Dexterity = h.Dexterity,
                Gold = h.Gold,
                CreatedAt = h.CreatedAt
            })
            .ToListAsync();

        return Ok(heroes);
    }

    /// <summary>
    /// Obtém os heróis na party ativa
    /// </summary>
    [HttpGet("active-party")]
    [ProducesResponseType(typeof(List<HeroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<HeroDto>>> GetActiveParty()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var partyHeroes = await _context.Heroes
            .Where(h => h.UserId == userId && h.IsInActiveParty)
            .OrderBy(h => h.PartySlot)
            .Select(h => new HeroDto
            {
                Id = h.Id,
                Name = h.Name,
                Class = h.Class,
                Level = h.Level,
                Experience = h.Experience,
                Strength = h.Strength,
                Intelligence = h.Intelligence,
                Dexterity = h.Dexterity,
                Gold = h.Gold,
                CreatedAt = h.CreatedAt
            })
            .ToListAsync();

        return Ok(partyHeroes);
    }

    /// <summary>
    /// Adiciona um herói à party ativa
    /// </summary>
    [HttpPost("add-to-party/{heroId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddToParty(int heroId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var hero = await _context.Heroes.FindAsync(heroId);
        if (hero == null || hero.UserId != userId)
        {
            return NotFound("Herói não encontrado.");
        }

        if (hero.IsInActiveParty)
        {
            return BadRequest("Este herói já está na party ativa.");
        }

        // Verifica se a party já está cheia (máximo 3)
        var partyCount = await _context.Heroes
            .CountAsync(h => h.UserId == userId && h.IsInActiveParty);

        if (partyCount >= 3)
        {
            return BadRequest("A party já está cheia! Máximo de 3 heróis. Remova um herói primeiro.");
        }

        // Encontra o próximo slot disponível (1, 2 ou 3)
        var usedSlots = await _context.Heroes
            .Where(h => h.UserId == userId && h.IsInActiveParty && h.PartySlot.HasValue)
            .Select(h => h.PartySlot!.Value)
            .ToListAsync();

        var nextSlot = Enumerable.Range(1, 3).First(s => !usedSlots.Contains(s));

        hero.IsInActiveParty = true;
        hero.PartySlot = nextSlot;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Herói {HeroName} (ID: {HeroId}) adicionado à party no slot {Slot}", 
            hero.Name, heroId, nextSlot);

        return Ok(new { message = $"Herói {hero.Name} adicionado à party no slot {nextSlot}." });
    }

    /// <summary>
    /// Remove um herói da party ativa
    /// </summary>
    [HttpPost("remove-from-party/{heroId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveFromParty(int heroId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var hero = await _context.Heroes.FindAsync(heroId);
        if (hero == null || hero.UserId != userId)
        {
            return NotFound("Herói não encontrado.");
        }

        if (!hero.IsInActiveParty)
        {
            return BadRequest("Este herói não está na party ativa.");
        }

        hero.IsInActiveParty = false;
        hero.PartySlot = null;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Herói {HeroName} (ID: {HeroId}) removido da party", hero.Name, heroId);

        return Ok(new { message = $"Herói {hero.Name} removido da party." });
    }
}

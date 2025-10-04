using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    /// Obtém os dados do usuário atual
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileDto>> GetCurrentUser()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        var heroCount = await _context.Heroes.CountAsync(h => h.UserId == userId && !h.IsDeleted);
        var activePartyCount = await _context.Heroes.CountAsync(h => h.UserId == userId && h.IsInActiveParty && !h.IsDeleted);

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Gold = user.Gold,
            HeroCount = heroCount,
            ActivePartyCount = activePartyCount,
            HasSeenTutorial = user.HasSeenTutorial,
            CreatedAt = user.CreatedAt
        });
    }

    /// <summary>
    /// Cria um novo herói
    /// </summary>
    [HttpPost("create-hero")]
    [ProducesResponseType(typeof(HeroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HeroDto>> CreateHero([FromBody] CreateHeroRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Verifica se já tem pelo menos um herói nível 5+ (não deletado)
        var heroes = await _context.Heroes.Where(h => h.UserId == userId && !h.IsDeleted).ToListAsync();
        
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
            Level = 1, // Começa no nível 1
            Experience = 0,
            UnallocatedAttributePoints = 0, // Começa sem pontos não alocados
            Gold = 0, // Ouro está no player, não no herói
            UserId = userId,
            IsInActiveParty = false, // Criado fora da party
            PartySlot = null,
            CreatedAt = DateTime.UtcNow
        };
        
        // Configurar atributos base baseados na classe
        hero.ConfigureInitialAttributes();

        _context.Heroes.Add(hero);
        await _context.SaveChangesAsync();

        // Se é o primeiro herói, adiciona automaticamente à party no slot 1
        var heroCount = await _context.Heroes.CountAsync(h => h.UserId == userId);
        if (heroCount == 1)
        {
            hero.IsInActiveParty = true;
            hero.PartySlot = 1;
            _logger.LogInformation("Primeiro herói {HeroName} adicionado automaticamente à party no slot 1", hero.Name);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Novo herói {HeroName} criado para o usuário {UserId}", hero.Name, userId);

        var heroDto = new HeroDto
        {
            Id = hero.Id,
            Name = hero.Name,
            Class = hero.Class,
            Level = hero.Level,
            Experience = hero.Experience,
            Strength = hero.GetTotalStrength(),
            Intelligence = hero.GetTotalIntelligence(),
            Dexterity = hero.GetTotalDexterity(),
            Gold = hero.Gold,
            MaxHealth = hero.MaxHealth,
            CurrentHealth = hero.CurrentHealth,
            CreatedAt = hero.CreatedAt,
            IsInActiveParty = hero.IsInActiveParty,
            PartySlot = hero.PartySlot
        };

        return CreatedAtAction(nameof(GetHero), new { id = hero.Id }, heroDto);
    }

    /// <summary>
    /// Obtém um herói específico
    /// </summary>
    [HttpGet("hero/{id}")]
    [ProducesResponseType(typeof(HeroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HeroDto>> GetHero(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var hero = await _context.Heroes
            .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId && !h.IsDeleted);

        if (hero == null)
        {
            return NotFound("Herói não encontrado.");
        }

        return Ok(new HeroDto
        {
            Id = hero.Id,
            Name = hero.Name,
            Class = hero.Class,
            Level = hero.Level,
            Experience = hero.Experience,
            Strength = hero.GetTotalStrength(),
            Intelligence = hero.GetTotalIntelligence(),
            Dexterity = hero.GetTotalDexterity(),
            Gold = hero.Gold,
            MaxHealth = hero.MaxHealth,
            CurrentHealth = hero.CurrentHealth,
            CreatedAt = hero.CreatedAt,
            IsInActiveParty = hero.IsInActiveParty,
            PartySlot = hero.PartySlot
        });
    }

    /// <summary>
    /// Obtém todos os heróis do usuário
    /// </summary>
    [HttpGet("my-heroes")]
    [ProducesResponseType(typeof(List<HeroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<HeroDto>>> GetMyHeroes()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var heroes = await _context.Heroes
            .Where(h => h.UserId == userId && !h.IsDeleted)
            .OrderByDescending(h => h.Level)
            .ThenBy(h => h.Name)
            .Select(h => new HeroDto
            {
                Id = h.Id,
                Name = h.Name,
                Class = h.Class,
                Level = h.Level,
                Experience = h.Experience,
                Strength = h.BaseStrength + h.BonusStrength,
                Intelligence = h.BaseIntelligence + h.BonusIntelligence,
                Dexterity = h.BaseDexterity + h.BonusDexterity,
                Gold = h.Gold,
                MaxHealth = h.MaxHealth,
                CurrentHealth = h.CurrentHealth,
                CreatedAt = h.CreatedAt,
                IsInActiveParty = h.IsInActiveParty,
                PartySlot = h.PartySlot
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
            .Where(h => h.UserId == userId && h.IsInActiveParty && !h.IsDeleted)
            .OrderBy(h => h.PartySlot)
            .Select(h => new HeroDto
            {
                Id = h.Id,
                Name = h.Name,
                Class = h.Class,
                Level = h.Level,
                Experience = h.Experience,
                Strength = h.BaseStrength + h.BonusStrength,
                Intelligence = h.BaseIntelligence + h.BonusIntelligence,
                Dexterity = h.BaseDexterity + h.BonusDexterity,
                Gold = h.Gold,
                MaxHealth = h.MaxHealth,
                CurrentHealth = h.CurrentHealth,
                CreatedAt = h.CreatedAt,
                IsInActiveParty = h.IsInActiveParty,
                PartySlot = h.PartySlot
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
        if (hero == null || hero.UserId != userId || hero.IsDeleted)
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

        // Se já tem 1+ herói na party, precisa ter pelo menos 1 herói nível 5+
        if (partyCount >= 1)
        {
            var hasLevel5Hero = await _context.Heroes
                .AnyAsync(h => h.UserId == userId && h.Level >= 5);

            if (!hasLevel5Hero)
            {
                return BadRequest("Você precisa ter pelo menos um herói nível 5 ou superior para adicionar mais heróis à party.");
            }
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
        if (hero == null || hero.UserId != userId || hero.IsDeleted)
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

        _logger.LogInformation("Herói {HeroName} (ID: {HeroId}) removido da party", 
            hero.Name, heroId);

        return Ok(new { message = $"Herói {hero.Name} removido da party." });
    }

    /// <summary>
    /// Marca o tutorial como visualizado
    /// </summary>
    [HttpPost("mark-tutorial-seen")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> MarkTutorialSeen()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.HasSeenTutorial = true;
            await _context.SaveChangesAsync();
        }

        return Ok(new { message = "Tutorial marcado como visualizado." });
    }

    /// <summary>
    /// Atualiza um herói
    /// </summary>
    [HttpPut("hero/{heroId}")]
    [ProducesResponseType(typeof(HeroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HeroDto>> UpdateHero(int heroId, [FromBody] UpdateHeroRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var hero = await _context.Heroes.FindAsync(heroId);
        if (hero == null || hero.UserId != userId || hero.IsDeleted)
        {
            return NotFound("Herói não encontrado");
        }

        // Atualizar campos permitidos
        if (request.Name != null) hero.Name = request.Name;
        if (request.Gold.HasValue) hero.Gold = request.Gold.Value;
        if (request.Experience.HasValue) hero.Experience = request.Experience.Value;
        if (request.Level.HasValue) hero.Level = request.Level.Value;
        if (request.CurrentHealth.HasValue) hero.CurrentHealth = request.CurrentHealth.Value;

        await _context.SaveChangesAsync();

        return Ok(new HeroDto
        {
            Id = hero.Id,
            Name = hero.Name,
            Class = hero.Class,
            Level = hero.Level,
            Experience = hero.Experience,
            Strength = hero.BaseStrength + hero.BonusStrength,
            Intelligence = hero.BaseIntelligence + hero.BonusIntelligence,
            Dexterity = hero.BaseDexterity + hero.BonusDexterity,
            Gold = hero.Gold,
            MaxHealth = hero.MaxHealth,
            CurrentHealth = hero.CurrentHealth,
            CreatedAt = hero.CreatedAt,
            IsInActiveParty = hero.IsInActiveParty,
            PartySlot = hero.PartySlot
        });
    }
}

// DTOs
public class UserProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Gold { get; set; }
    public int HeroCount { get; set; }
    public int ActivePartyCount { get; set; }
    public bool HasSeenTutorial { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HeroDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public int Dexterity { get; set; }
    public int Gold { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsInActiveParty { get; set; }
    public int? PartySlot { get; set; }
}

public class CreateHeroRequest
{
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
}

public class UpdateHeroRequest
{
    public string? Name { get; set; }
    public int? Gold { get; set; }
    public int? Experience { get; set; }
    public int? Level { get; set; }
    public int? CurrentHealth { get; set; }
}

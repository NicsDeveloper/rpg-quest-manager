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

        // Busca o herói principal: primeiro da party ativa, ou qualquer herói do player (não deletado)
        var hero = await _context.Heroes
            .Include(h => h.User) // Precisa incluir User para pegar o Gold
            .Include(h => h.HeroItems)
                .ThenInclude(hi => hi.Item)
            .Include(h => h.HeroQuests)
                .ThenInclude(hq => hq.Quest)
            .Where(h => h.UserId == userId && !h.IsDeleted)
            .OrderByDescending(h => h.IsInActiveParty)
            .ThenBy(h => h.PartySlot)
            .ThenByDescending(h => h.Level)
            .FirstOrDefaultAsync();

        if (hero == null)
        {
            return NotFound("Você ainda não tem um herói! Crie um herói para começar sua aventura.");
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
            Gold = hero.User?.Gold ?? 0, // Ouro vem do User, não do Hero
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
    /// Obtém estatísticas do perfil do usuário
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetMyStats()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var heroes = await _context.Heroes
            .Include(h => h.HeroQuests)
            .Include(h => h.HeroItems)
            .Include(h => h.User)
            .Where(h => h.UserId == userId && !h.IsDeleted)
            .ToListAsync();

        if (!heroes.Any())
        {
            return Ok(new
            {
                totalQuests = 0,
                completedQuests = 0,
                totalItems = 0,
                uniqueItems = 0,
                equippedItems = 0,
                totalGold = 0,
                currentLevel = 0,
                totalExperience = 0,
                experienceForNextLevel = 0,
                playDays = 0,
                powerRating = 0
            });
        }

        var mainHero = heroes.OrderByDescending(h => h.IsInActiveParty).ThenByDescending(h => h.Level).First();
        var user = await _context.Users.FindAsync(userId);

        var stats = new
        {
            totalQuests = heroes.Sum(h => h.HeroQuests.Count),
            completedQuests = heroes.Sum(h => h.HeroQuests.Count(hq => hq.IsCompleted)),
            totalItems = heroes.Sum(h => h.HeroItems.Sum(hi => hi.Quantity)),
            uniqueItems = heroes.SelectMany(h => h.HeroItems).Select(hi => hi.ItemId).Distinct().Count(),
            equippedItems = heroes.Sum(h => h.HeroItems.Count(hi => hi.IsEquipped)),
            totalGold = user?.Gold ?? 0, // Ouro vem do User
            currentLevel = mainHero.Level,
            totalExperience = heroes.Sum(h => h.Experience),
            experienceForNextLevel = mainHero.GetExperienceForNextLevel(),
            playDays = (DateTime.UtcNow - (user?.CreatedAt ?? DateTime.UtcNow)).Days,
            powerRating = heroes.Sum(h => h.Strength + h.Intelligence + h.Dexterity + (h.Level * 10))
        };

        return Ok(stats);
    }

    /// <summary>
    /// Obtém todas as quests do usuário (de todos os heróis)
    /// </summary>
    [HttpGet("my-quests")]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetMyQuests()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var heroQuests = await _context.HeroQuests
            .Include(hq => hq.Quest)
                .ThenInclude(q => q.Rewards)
                    .ThenInclude(r => r.Item)
            .Include(hq => hq.Hero)
            .Where(hq => hq.Hero.UserId == userId)
            .OrderByDescending(hq => hq.StartedAt)
            .Select(hq => new
            {
                id = hq.Id,
                quest = new
                {
                    id = hq.Quest.Id,
                    name = hq.Quest.Name,
                    description = hq.Quest.Description,
                    difficulty = hq.Quest.Difficulty,
                    goldReward = hq.Quest.GoldReward,
                    experienceReward = hq.Quest.ExperienceReward,
                    rewards = hq.Quest.Rewards.Select(r => new
                    {
                        id = r.Id,
                        itemId = r.ItemId,
                        itemName = r.Item != null ? r.Item.Name : null,
                        gold = r.Gold,
                        experience = r.Experience
                    }).ToList()
                },
                isCompleted = hq.IsCompleted,
                startedAt = hq.StartedAt,
                completedAt = hq.CompletedAt
            })
            .ToListAsync();

        return Ok(heroQuests);
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
            Level = 0,
            Experience = 0,
            Strength = request.Strength,
            Intelligence = request.Intelligence,
            Dexterity = request.Dexterity,
            Gold = 0, // Ouro está no player, não no herói
            UserId = userId,
            IsInActiveParty = false, // Criado fora da party
            PartySlot = null,
            CreatedAt = DateTime.UtcNow
        };

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
        
        // O inventário de dados é criado automaticamente quando o player compra/usa dados pela primeira vez
        // (já é compartilhado entre todos os heróis do player)

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
            .Where(h => h.UserId == userId && !h.IsDeleted)
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
            .Where(h => h.UserId == userId && h.IsInActiveParty && !h.IsDeleted)
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

    /// <summary>
    /// Soft delete de um herói (recuperável por 7 dias)
    /// </summary>
    [HttpDelete("delete-hero/{heroId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteHero(int heroId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var hero = await _context.Heroes
            .Where(h => h.Id == heroId && h.UserId == userId && !h.IsDeleted)
            .FirstOrDefaultAsync();
            
        if (hero == null)
        {
            return NotFound("Herói não encontrado.");
        }

        // Remove da party se estiver nela
        if (hero.IsInActiveParty)
        {
            hero.IsInActiveParty = false;
            hero.PartySlot = null;
        }

        // Soft delete
        hero.IsDeleted = true;
        hero.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Herói {HeroName} (ID: {HeroId}) marcado como deletado (soft delete)", hero.Name, heroId);

        return Ok(new { 
            message = $"Herói {hero.Name} foi deletado. Você tem 7 dias para recuperá-lo na área de recuperação.",
            heroId = hero.Id,
            deletedAt = hero.DeletedAt,
            recoveryDeadline = hero.DeletedAt.Value.AddDays(7)
        });
    }

    /// <summary>
    /// Lista heróis deletados (área de recuperação)
    /// </summary>
    [HttpGet("deleted-heroes")]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetDeletedHeroes()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var deletedHeroes = await _context.Heroes
            .Where(h => h.UserId == userId && h.IsDeleted)
            .OrderByDescending(h => h.DeletedAt)
            .Select(h => new
            {
                id = h.Id,
                name = h.Name,
                @class = h.Class,
                level = h.Level,
                experience = h.Experience,
                strength = h.Strength,
                intelligence = h.Intelligence,
                dexterity = h.Dexterity,
                deletedAt = h.DeletedAt,
                daysUntilPermanentDeletion = h.DeletedAt.HasValue 
                    ? Math.Max(0, 7 - (int)(DateTime.UtcNow - h.DeletedAt.Value).TotalDays)
                    : 0,
                canRecover = h.DeletedAt.HasValue && (DateTime.UtcNow - h.DeletedAt.Value).TotalDays < 7
            })
            .ToListAsync();

        return Ok(deletedHeroes);
    }

    /// <summary>
    /// Restaura um herói deletado
    /// </summary>
    [HttpPost("restore-hero/{heroId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RestoreHero(int heroId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var hero = await _context.Heroes
            .Where(h => h.Id == heroId && h.UserId == userId && h.IsDeleted)
            .FirstOrDefaultAsync();
            
        if (hero == null)
        {
            return NotFound("Herói deletado não encontrado.");
        }

        // Verifica se ainda está dentro do período de 7 dias
        if (hero.DeletedAt.HasValue && (DateTime.UtcNow - hero.DeletedAt.Value).TotalDays >= 7)
        {
            return BadRequest("O período de recuperação de 7 dias expirou. Este herói não pode mais ser restaurado.");
        }

        // Restaura o herói
        hero.IsDeleted = false;
        hero.DeletedAt = null;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Herói {HeroName} (ID: {HeroId}) restaurado com sucesso", hero.Name, heroId);

        return Ok(new { 
            message = $"Herói {hero.Name} restaurado com sucesso!",
            hero = new HeroDto
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
            }
        });
    }
}

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
    /// Obtém o perfil do herói ativo do usuário logado
    /// </summary>
    [HttpGet("my-hero")]
    [ProducesResponseType(typeof(HeroProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HeroProfileDto>> GetMyHeroProfile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var hero = await _context.Heroes
            .Include(h => h.HeroItems)
                .ThenInclude(hi => hi.Item)
            .Include(h => h.HeroQuests)
                .ThenInclude(hq => hq.Quest)
            .FirstOrDefaultAsync(h => h.UserId == userId && h.IsActive);

        if (hero == null)
        {
            return NotFound("Herói ativo não encontrado para o usuário logado.");
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
    /// Apenas um herói pode ser criado se:
    /// - O usuário não possui nenhum herói ainda, OU
    /// - O herói ativo atual atingiu o nível máximo (20)
    /// </remarks>
    [HttpPost("create-hero")]
    [ProducesResponseType(typeof(HeroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HeroDto>> CreateHero([FromBody] CreateHeroRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Verifica se já existe um herói ativo
        var existingHero = await _context.Heroes
            .FirstOrDefaultAsync(h => h.UserId == userId && h.IsActive);

        if (existingHero != null)
        {
            // Só pode criar novo herói se o ativo estiver no nível máximo
            if (!existingHero.IsMaxLevel())
            {
                return BadRequest($"Você só pode criar um novo herói quando seu herói atual ({existingHero.Name}) atingir o nível máximo (20). Nível atual: {existingHero.Level}");
            }

            // Desativa o herói antigo
            existingHero.IsActive = false;
            _logger.LogInformation("Herói {HeroName} desativado. Usuário {UserId} criou novo herói.", existingHero.Name, userId);
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
            IsActive = true,
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

        return CreatedAtAction(nameof(GetMyHeroProfile), new { }, heroDto);
    }

    /// <summary>
    /// Lista todos os heróis do usuário logado (ativos e inativos)
    /// </summary>
    [HttpGet("my-heroes")]
    [ProducesResponseType(typeof(List<HeroDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<HeroDto>>> GetMyHeroes()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var heroes = await _context.Heroes
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.IsActive)
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
}

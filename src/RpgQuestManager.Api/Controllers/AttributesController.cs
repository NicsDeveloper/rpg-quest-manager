using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Heroes;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "⚔️ Attributes")]
public class AttributesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AttributesController> _logger;

    public AttributesController(ApplicationDbContext context, ILogger<AttributesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém informações de atributos de um herói
    /// </summary>
    [HttpGet("hero/{heroId}")]
    public async Task<ActionResult<HeroAttributesDto>> GetHeroAttributes(int heroId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hero = await _context.Heroes
            .FirstOrDefaultAsync(h => h.Id == heroId && h.UserId == userId);

        if (hero == null)
        {
            return NotFound("Herói não encontrado ou não pertence ao usuário.");
        }

        var dto = new HeroAttributesDto
        {
            HeroId = hero.Id,
            HeroName = hero.Name,
            Class = hero.Class,
            Level = hero.Level,
            Strength = hero.Strength,
            Intelligence = hero.Intelligence,
            Dexterity = hero.Dexterity,
            BaseStrength = hero.BaseStrength,
            BaseIntelligence = hero.BaseIntelligence,
            BaseDexterity = hero.BaseDexterity,
            BonusStrength = hero.BonusStrength,
            BonusIntelligence = hero.BonusIntelligence,
            BonusDexterity = hero.BonusDexterity,
            UnallocatedPoints = hero.UnallocatedAttributePoints,
            TotalAttack = hero.GetTotalAttack(),
            TotalDefense = hero.GetTotalDefense(),
            TotalMagic = hero.GetTotalMagic()
        };

        return Ok(dto);
    }

    /// <summary>
    /// Distribui pontos de atributo não alocados
    /// </summary>
    [HttpPost("hero/{heroId}/allocate")]
    public async Task<ActionResult<AttributeAllocationResultDto>> AllocateAttributes(
        int heroId, 
        [FromBody] AllocateAttributesRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hero = await _context.Heroes
            .FirstOrDefaultAsync(h => h.Id == heroId && h.UserId == userId);

        if (hero == null)
        {
            return NotFound("Herói não encontrado ou não pertence ao usuário.");
        }

        if (hero.UnallocatedAttributePoints <= 0)
        {
            return BadRequest("Este herói não possui pontos de atributo para distribuir.");
        }

        var totalPoints = request.StrengthPoints + request.IntelligencePoints + request.DexterityPoints;
        
        if (totalPoints > hero.UnallocatedAttributePoints)
        {
            return BadRequest($"Você está tentando distribuir {totalPoints} pontos, mas só tem {hero.UnallocatedAttributePoints} disponíveis.");
        }

        if (totalPoints <= 0)
        {
            return BadRequest("Você deve distribuir pelo menos 1 ponto de atributo.");
        }

        if (request.StrengthPoints < 0 || request.IntelligencePoints < 0 || request.DexterityPoints < 0)
        {
            return BadRequest("Não é possível distribuir valores negativos.");
        }

        // Aplicar distribuição
        var success = hero.AllocateAttributePoints(
            request.StrengthPoints, 
            request.IntelligencePoints, 
            request.DexterityPoints
        );

        if (!success)
        {
            return BadRequest("Erro ao distribuir pontos de atributo.");
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("🎯 Herói {HeroName} distribuiu {TotalPoints} pontos de atributo: FOR+{Str}, INT+{Int}, DEX+{Dex}", 
            hero.Name, totalPoints, request.StrengthPoints, request.IntelligencePoints, request.DexterityPoints);

        var result = new AttributeAllocationResultDto
        {
            Success = true,
            Message = $"Pontos distribuídos com sucesso! {totalPoints} pontos alocados.",
            HeroId = hero.Id,
            NewStrength = hero.Strength,
            NewIntelligence = hero.Intelligence,
            NewDexterity = hero.Dexterity,
            RemainingPoints = hero.UnallocatedAttributePoints,
            NewTotalAttack = hero.GetTotalAttack(),
            NewTotalDefense = hero.GetTotalDefense(),
            NewTotalMagic = hero.GetTotalMagic()
        };

        return Ok(result);
    }

    /// <summary>
    /// Obtém informações sobre classes disponíveis e seus atributos base
    /// </summary>
    [HttpGet("classes")]
    public ActionResult<IEnumerable<ClassInfoDto>> GetAvailableClasses()
    {
        var classes = new List<ClassInfoDto>
        {
            new ClassInfoDto
            {
                Name = "Guerreiro",
                Description = "Especialista em combate corpo a corpo",
                BaseStrength = 18,
                BaseIntelligence = 12,
                BaseDexterity = 14,
                CombatFocus = "Físico",
                RecommendedFor = "Iniciantes, combate direto"
            },
            new ClassInfoDto
            {
                Name = "Mago",
                Description = "Mestre das artes arcanas",
                BaseStrength = 10,
                BaseIntelligence = 22,
                BaseDexterity = 16,
                CombatFocus = "Mágico",
                RecommendedFor = "Jogadores experientes, magia"
            },
            new ClassInfoDto
            {
                Name = "Arqueiro",
                Description = "Especialista em combate à distância",
                BaseStrength = 14,
                BaseIntelligence = 15,
                BaseDexterity = 20,
                CombatFocus = "Ágil",
                RecommendedFor = "Combate tático, precisão"
            },
            new ClassInfoDto
            {
                Name = "Ladino",
                Description = "Mestre da furtividade e agilidade",
                BaseStrength = 12,
                BaseIntelligence = 14,
                BaseDexterity = 18,
                CombatFocus = "Ágil",
                RecommendedFor = "Jogadores táticos, furtividade"
            },
            new ClassInfoDto
            {
                Name = "Paladino",
                Description = "Guerreiro sagrado equilibrado",
                BaseStrength = 16,
                BaseIntelligence = 18,
                BaseDexterity = 14,
                CombatFocus = "Físico/Mágico",
                RecommendedFor = "Jogadores versáteis"
            },
            new ClassInfoDto
            {
                Name = "Clérigo",
                Description = "Curandeiro e suporte divino",
                BaseStrength = 12,
                BaseIntelligence = 20,
                BaseDexterity = 12,
                CombatFocus = "Mágico",
                RecommendedFor = "Suporte, cura"
            },
            new ClassInfoDto
            {
                Name = "Bárbaro",
                Description = "Guerreiro selvagem e poderoso",
                BaseStrength = 20,
                BaseIntelligence = 10,
                BaseDexterity = 12,
                CombatFocus = "Físico",
                RecommendedFor = "Dano bruto, resistência"
            }
        };

        return Ok(classes);
    }
}

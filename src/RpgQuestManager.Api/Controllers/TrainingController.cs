using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Training;
using RpgQuestManager.Api.Models;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

/// <summary>
/// Sistema de treinamento diário para ganhar XP sem quests
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "🏋️ Treinamento")]
public class TrainingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TrainingController> _logger;
    
    public TrainingController(
        ApplicationDbContext context, 
        ILogger<TrainingController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Realiza treinamento diário para ganhar XP
    /// </summary>
    [HttpPost("daily")]
    public async Task<ActionResult<TrainingResultDto>> PerformDailyTraining()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.UserId == userId);
        
        if (hero == null)
        {
            return BadRequest(new { message = "Você não possui um herói." });
        }
        
        // Verificar se já treinou hoje
        var today = DateTime.UtcNow.Date;
        var todayTraining = await _context.HeroTrainings
            .FirstOrDefaultAsync(ht => ht.HeroId == hero.Id && ht.TrainingDate.Date == today);
            
        if (todayTraining != null)
        {
            return BadRequest(new { message = "Você já realizou o treinamento diário hoje. Tente novamente amanhã!" });
        }
        
        // Calcular XP baseado no nível do herói
        var baseXp = Math.Max(20, hero.Level * 10); // Mínimo 20 XP, escala com nível
        var randomBonus = new Random().Next(0, 21); // 0-20 XP aleatório
        var totalXp = baseXp + randomBonus;
        
        // Aplicar XP
        var oldLevel = hero.Level;
        hero.Experience += totalXp;
        
        // Verificar level up
        var leveledUp = false;
        var newLevel = hero.Level;
        if (hero.CanLevelUp())
        {
            hero.LevelUp();
            leveledUp = true;
            newLevel = hero.Level;
        }
        
        // Registrar treinamento
        var training = new HeroTraining
        {
            HeroId = hero.Id,
            TrainingDate = DateTime.UtcNow,
            XpGained = totalXp,
            TrainingType = "Daily"
        };
        _context.HeroTrainings.Add(training);
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("🏋️ Treinamento diário realizado! Herói {HeroName} ganhou {XpGained} XP. Level up: {LeveledUp}", 
            hero.Name, totalXp, leveledUp);
        
        return Ok(new TrainingResultDto
        {
            Success = true,
            Message = $"Treinamento concluído! Ganhou {totalXp} XP!",
            XpGained = totalXp,
            LeveledUp = leveledUp,
            NewLevel = leveledUp ? newLevel : null,
            RemainingXp = hero.GetExperienceForNextLevel() - (hero.Experience - totalXp)
        });
    }
    
    /// <summary>
    /// Obtém histórico de treinamentos do herói
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<TrainingHistoryDto>>> GetTrainingHistory()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.UserId == userId);
        
        if (hero == null)
        {
            return BadRequest(new { message = "Você não possui um herói." });
        }
        
        var trainings = await _context.HeroTrainings
            .Where(ht => ht.HeroId == hero.Id)
            .OrderByDescending(ht => ht.TrainingDate)
            .Take(30) // Últimos 30 treinamentos
            .Select(ht => new TrainingHistoryDto
            {
                Id = ht.Id,
                TrainingDate = ht.TrainingDate,
                XpGained = ht.XpGained,
                TrainingType = ht.TrainingType
            })
            .ToListAsync();
            
        return Ok(trainings);
    }
    
    /// <summary>
    /// Verifica se o herói pode treinar hoje
    /// </summary>
    [HttpGet("can-train")]
    public async Task<ActionResult<CanTrainDto>> CanTrainToday()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.UserId == userId);
        
        if (hero == null)
        {
            return BadRequest(new { message = "Você não possui um herói." });
        }
        
        var today = DateTime.UtcNow.Date;
        var todayTraining = await _context.HeroTrainings
            .FirstOrDefaultAsync(ht => ht.HeroId == hero.Id && ht.TrainingDate.Date == today);
            
        return Ok(new CanTrainDto
        {
            CanTrain = todayTraining == null,
            LastTrainingDate = todayTraining?.TrainingDate,
            Message = todayTraining == null 
                ? "Você pode treinar hoje!" 
                : "Você já treinou hoje. Tente novamente amanhã!"
        });
    }
}

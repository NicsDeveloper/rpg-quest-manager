using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DiceInventoryController : ControllerBase
{
    private readonly DiceInventoryService _diceInventoryService;
    private readonly ApplicationDbContext _db;

    public DiceInventoryController(DiceInventoryService diceInventoryService, ApplicationDbContext db)
    {
        _diceInventoryService = diceInventoryService;
        _db = db;
    }

    [HttpGet("{heroId}")]
    public async Task<IActionResult> GetHeroDiceInventory(int heroId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Verificar se o herói pertence ao usuário autenticado
            var hero = await _db.Heroes.FirstOrDefaultAsync(h => h.Id == heroId && h.UserId == userId);
            if (hero == null)
            {
                return Unauthorized(new { message = "Herói não encontrado ou não pertence ao usuário" });
            }
            
            var inventory = await _diceInventoryService.GetHeroDiceInventoryAsync(heroId);
            
            return Ok(new
            {
                heroId,
                inventory = inventory.ToDictionary(
                    kvp => kvp.Key.ToString().ToLower(),
                    kvp => kvp.Value
                )
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("{heroId}/consume/{diceType}")]
    public async Task<IActionResult> ConsumeDice(int heroId, string diceType)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Verificar se o herói pertence ao usuário autenticado
            var hero = await _db.Heroes.FirstOrDefaultAsync(h => h.Id == heroId && h.UserId == userId);
            if (hero == null)
            {
                return Unauthorized(new { message = "Herói não encontrado ou não pertence ao usuário" });
            }
            
            if (!Enum.TryParse<DiceType>(diceType.ToUpper(), out var diceTypeEnum))
            {
                return BadRequest(new { message = "Tipo de dado inválido" });
            }

            var success = await _diceInventoryService.ConsumeDiceAsync(heroId, diceTypeEnum);
            
            if (!success)
            {
                return BadRequest(new { message = "Dado não disponível no inventário" });
            }

            return Ok(new { message = "Dado consumido com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("{heroId}/add")]
    public async Task<IActionResult> AddDice(int heroId, [FromBody] AddDiceRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Verificar se o herói pertence ao usuário autenticado
            var hero = await _db.Heroes.FirstOrDefaultAsync(h => h.Id == heroId && h.UserId == userId);
            if (hero == null)
            {
                return Unauthorized(new { message = "Herói não encontrado ou não pertence ao usuário" });
            }
            
            if (!Enum.TryParse<DiceType>(request.DiceType.ToUpper(), out var diceTypeEnum))
            {
                return BadRequest(new { message = "Tipo de dado inválido" });
            }

            await _diceInventoryService.AddDiceAsync(heroId, diceTypeEnum, request.Quantity);
            
            return Ok(new { message = "Dados adicionados com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost("{heroId}/initialize")]
    public async Task<IActionResult> InitializeDiceInventory(int heroId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Verificar se o herói pertence ao usuário autenticado
            var hero = await _db.Heroes.FirstOrDefaultAsync(h => h.Id == heroId && h.UserId == userId);
            if (hero == null)
            {
                return Unauthorized(new { message = "Herói não encontrado ou não pertence ao usuário" });
            }
            
            await _diceInventoryService.InitializeHeroDiceInventoryAsync(heroId);
            
            return Ok(new { message = "Inventário de dados inicializado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    public record AddDiceRequest(string DiceType, int Quantity);
}

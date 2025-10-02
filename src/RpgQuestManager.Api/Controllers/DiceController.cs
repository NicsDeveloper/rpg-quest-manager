using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Dice;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "🎲 Dice")]
public class DiceController : ControllerBase
{
    private readonly IDiceService _diceService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DiceController> _logger;

    public DiceController(
        IDiceService diceService,
        ApplicationDbContext context,
        ILogger<DiceController> logger)
    {
        _diceService = diceService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém o inventário de dados do player (compartilhado entre todos os heróis)
    /// </summary>
    [HttpGet("inventory")]
    [ProducesResponseType(typeof(DiceInventoryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DiceInventoryDto>> GetInventory()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var inventory = await _diceService.GetOrCreateInventoryAsync(userId);
        
        var dto = new DiceInventoryDto
        {
            UserId = inventory.UserId,
            D6Count = inventory.D6Count,
            D10Count = inventory.D10Count,
            D12Count = inventory.D12Count,
            D20Count = inventory.D20Count
        };

        return Ok(dto);
    }

    /// <summary>
    /// Compra dados com ouro do herói
    /// </summary>
    [HttpPost("purchase")]
    [ProducesResponseType(typeof(PurchaseDiceResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseDiceResultDto>> PurchaseDice([FromBody] PurchaseDiceRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var diceType = Enum.Parse<DiceType>(request.DiceType);
            var price = await _diceService.GetDicePriceAsync(diceType);
            var totalPrice = price * request.Quantity;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return BadRequest("Usuário não encontrado.");
            }

            if (user.Gold < totalPrice)
            {
                return BadRequest($"Ouro insuficiente. Você precisa de {totalPrice} ouro, mas tem apenas {user.Gold}.");
            }

            var success = await _diceService.PurchaseDiceAsync(userId, diceType, request.Quantity);
            
            if (!success)
            {
                return BadRequest("Não foi possível realizar a compra.");
            }

            var inventory = await _diceService.GetOrCreateInventoryAsync(userId);
            
            // Recarrega para pegar o ouro atualizado
            await _context.Entry(user).ReloadAsync();

            return Ok(new PurchaseDiceResultDto
            {
                Success = true,
                Message = $"Você comprou {request.Quantity}x {request.DiceType} por {totalPrice} ouro!",
                RemainingGold = user.Gold,
                UpdatedInventory = new DiceInventoryDto
                {
                    UserId = inventory.UserId,
                    D6Count = inventory.D6Count,
                    D10Count = inventory.D10Count,
                    D12Count = inventory.D12Count,
                    D20Count = inventory.D20Count
                }
            });
        }
        catch (ArgumentException)
        {
            return BadRequest("Tipo de dado inválido.");
        }
    }

    /// <summary>
    /// Obtém os preços de todos os tipos de dados
    /// </summary>
    [HttpGet("prices")]
    [ProducesResponseType(typeof(DicePricesDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DicePricesDto>> GetPrices()
    {
        var prices = new DicePricesDto
        {
            D6 = await _diceService.GetDicePriceAsync(DiceType.D6),
            D10 = await _diceService.GetDicePriceAsync(DiceType.D10),
            D12 = await _diceService.GetDicePriceAsync(DiceType.D12),
            D20 = await _diceService.GetDicePriceAsync(DiceType.D20)
        };

        return Ok(prices);
    }
}


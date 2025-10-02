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
    /// Obtém o inventário de dados do herói do usuário logado
    /// </summary>
    [HttpGet("inventory/{heroId}")]
    [ProducesResponseType(typeof(DiceInventoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DiceInventoryDto>> GetInventory(int heroId)
    {
        var inventory = await _diceService.GetOrCreateInventoryAsync(heroId);
        
        var dto = new DiceInventoryDto
        {
            HeroId = inventory.HeroId,
            D6Count = inventory.D6Count,
            D8Count = inventory.D8Count,
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
            var diceType = Enum.Parse<DiceType>(request.DiceType);
            var price = await _diceService.GetDicePriceAsync(diceType);
            var totalPrice = price * request.Quantity;

            var hero = await _context.Heroes.FindAsync(request.HeroId);
            if (hero == null)
            {
                return BadRequest("Herói não encontrado.");
            }

            if (hero.Gold < totalPrice)
            {
                return BadRequest($"Ouro insuficiente. Você precisa de {totalPrice} ouro, mas tem apenas {hero.Gold}.");
            }

            var success = await _diceService.PurchaseDiceAsync(request.HeroId, diceType, request.Quantity);
            
            if (!success)
            {
                return BadRequest("Não foi possível realizar a compra.");
            }

            var inventory = await _diceService.GetOrCreateInventoryAsync(request.HeroId);

            return Ok(new PurchaseDiceResultDto
            {
                Success = true,
                Message = $"Você comprou {request.Quantity}x {request.DiceType} por {totalPrice} ouro!",
                RemainingGold = hero.Gold,
                UpdatedInventory = new DiceInventoryDto
                {
                    HeroId = inventory.HeroId,
                    D6Count = inventory.D6Count,
                    D8Count = inventory.D8Count,
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
            D8 = await _diceService.GetDicePriceAsync(DiceType.D8),
            D12 = await _diceService.GetDicePriceAsync(DiceType.D12),
            D20 = await _diceService.GetDicePriceAsync(DiceType.D20)
        };

        return Ok(prices);
    }
}


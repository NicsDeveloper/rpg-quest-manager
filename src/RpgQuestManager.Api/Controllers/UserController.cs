using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using System.Security.Claims;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserController> _logger;

    public UserController(ApplicationDbContext context, ILogger<UserController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("me")]
    public async Task<ActionResult> GetCurrentUser()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Role,
            user.HasSeenTutorial,
            user.Gold,
            user.CreatedAt
        });
    }

    [HttpPost("complete-tutorial")]
    public async Task<ActionResult> CompleteTutorial()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        user.HasSeenTutorial = true;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuário {Username} completou o tutorial", user.Username);

        return Ok(new { message = "Tutorial marcado como completo!" });
    }

    [HttpPost("skip-tutorial")]
    public async Task<ActionResult> SkipTutorial()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        user.HasSeenTutorial = true;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuário {Username} pulou o tutorial", user.Username);

        return Ok(new { message = "Tutorial pulado!" });
    }
}


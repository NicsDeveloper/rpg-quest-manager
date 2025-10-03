using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CombatController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICombatService _combat;
    public CombatController(ApplicationDbContext db, ICombatService combat) { _db = db; _combat = combat; }

    public record AttackRequest(int characterId, int monsterId);

    [HttpPost("attack")]
    public async Task<IActionResult> Attack([FromBody] AttackRequest request)
    {
        var result = await _combat.AttackAsync(request.characterId, request.monsterId);
        return Ok(new
        {
            hero = new { result.Hero.Id, result.Hero.Health, result.Hero.Morale },
            monster = new { result.Monster.Id, result.Monster.Health },
            damage = new { toMonster = result.DamageToMonster, toHero = result.DamageToHero },
            result.IsCritical,
            result.IsFumble
        });
    }
}



using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonstersController : ControllerBase
{
    private readonly MonsterService _monsterService;
    public MonstersController(MonsterService monsterService) { _monsterService = monsterService; }

    [HttpGet("by-environment/{env}")]
    public async Task<IActionResult> GetMonstersByEnvironment(string env)
    {
        if (!Enum.TryParse<EnvironmentType>(env, true, out var envType)) 
            return BadRequest("Invalid environment");
        
        var data = await _monsterService.GetMonstersByEnvironmentAsync(envType);
        return Ok(data);
    }

    [HttpGet("bosses-by-environment/{env}")]
    public async Task<IActionResult> GetBossesByEnvironment(string env)
    {
        if (!Enum.TryParse<EnvironmentType>(env, true, out var envType)) 
            return BadRequest("Invalid environment");
        
        var data = await _monsterService.GetBossesByEnvironmentAsync(envType);
        return Ok(data);
    }

    [HttpGet("by-type/{type}")]
    public async Task<IActionResult> GetMonstersByType(string type)
    {
        if (!Enum.TryParse<MonsterType>(type, true, out var monsterType)) 
            return BadRequest("Invalid monster type");
        
        var data = await _monsterService.GetMonstersByTypeAsync(monsterType);
        return Ok(data);
    }

    [HttpGet("by-level/{minLevel}/{maxLevel}")]
    public async Task<IActionResult> GetMonstersByLevelRange(int minLevel, int maxLevel)
    {
        var data = await _monsterService.GetMonstersByLevelRangeAsync(minLevel, maxLevel);
        return Ok(data);
    }

    [HttpGet("random/{env}/{characterLevel}")]
    public async Task<IActionResult> GetRandomMonster(string env, int characterLevel)
    {
        if (!Enum.TryParse<EnvironmentType>(env, true, out var envType)) 
            return BadRequest("Invalid environment");
        
        var monster = await _monsterService.GetRandomMonsterAsync(envType, characterLevel);
        if (monster == null) return NotFound("No suitable monster found");
        
        return Ok(monster);
    }

    [HttpGet("random-boss/{env}/{characterLevel}")]
    public async Task<IActionResult> GetRandomBoss(string env, int characterLevel)
    {
        if (!Enum.TryParse<EnvironmentType>(env, true, out var envType)) 
            return BadRequest("Invalid environment");
        
        var boss = await _monsterService.GetRandomBossAsync(envType, characterLevel);
        if (boss == null) return NotFound("No suitable boss found");
        
        return Ok(boss);
    }
}



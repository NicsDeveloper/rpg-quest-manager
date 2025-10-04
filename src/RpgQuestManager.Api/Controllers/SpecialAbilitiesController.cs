using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SpecialAbilitiesController : ControllerBase
{
    private readonly ISpecialAbilityService _specialAbilityService;

    public SpecialAbilitiesController(ISpecialAbilityService specialAbilityService)
    {
        _specialAbilityService = specialAbilityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAbilities()
    {
        var abilities = await _specialAbilityService.GetAllAbilitiesAsync();
        return Ok(abilities);
    }

    [HttpGet("by-type/{type}")]
    public async Task<IActionResult> GetAbilitiesByType(AbilityType type)
    {
        var abilities = await _specialAbilityService.GetAbilitiesByTypeAsync(type);
        return Ok(abilities);
    }

    [HttpGet("by-category/{category}")]
    public async Task<IActionResult> GetAbilitiesByCategory(AbilityCategory category)
    {
        var abilities = await _specialAbilityService.GetAbilitiesByCategoryAsync(category);
        return Ok(abilities);
    }

    [HttpGet("character/{characterId}")]
    public async Task<IActionResult> GetCharacterAbilities(int characterId)
    {
        var abilities = await _specialAbilityService.GetCharacterAbilitiesAsync(characterId);
        return Ok(abilities);
    }

    [HttpGet("character/{characterId}/combos")]
    public async Task<IActionResult> GetCharacterCombos(int characterId)
    {
        var combos = await _specialAbilityService.GetCharacterCombosAsync(characterId);
        return Ok(combos);
    }

    [HttpGet("character/{characterId}/available-combos")]
    public async Task<IActionResult> GetAvailableCombos(int characterId)
    {
        var combos = await _specialAbilityService.GetAvailableCombosAsync(characterId);
        return Ok(combos);
    }

    public record UnlockAbilityRequest(int CharacterId, int AbilityId);
    [HttpPost("unlock")]
    public async Task<IActionResult> UnlockAbility([FromBody] UnlockAbilityRequest request)
    {
        var (success, message) = await _specialAbilityService.UnlockAbilityAsync(request.CharacterId, request.AbilityId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record EquipAbilityRequest(int CharacterId, int AbilityId);
    [HttpPost("equip")]
    public async Task<IActionResult> EquipAbility([FromBody] EquipAbilityRequest request)
    {
        var (success, message) = await _specialAbilityService.EquipAbilityAsync(request.CharacterId, request.AbilityId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record UnequipAbilityRequest(int CharacterId, int AbilityId);
    [HttpPost("unequip")]
    public async Task<IActionResult> UnequipAbility([FromBody] UnequipAbilityRequest request)
    {
        var (success, message) = await _specialAbilityService.UnequipAbilityAsync(request.CharacterId, request.AbilityId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record UseAbilityRequest(int CharacterId, int AbilityId, int TargetId);
    [HttpPost("use")]
    public async Task<IActionResult> UseAbility([FromBody] UseAbilityRequest request)
    {
        var (success, message) = await _specialAbilityService.UseAbilityAsync(request.CharacterId, request.AbilityId, request.TargetId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record UnlockComboRequest(int CharacterId, int ComboId);
    [HttpPost("unlock-combo")]
    public async Task<IActionResult> UnlockCombo([FromBody] UnlockComboRequest request)
    {
        var (success, message) = await _specialAbilityService.UnlockComboAsync(request.CharacterId, request.ComboId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record ExecuteComboStepRequest(int CharacterId, int ComboId, int AbilityId);
    [HttpPost("execute-combo-step")]
    public async Task<IActionResult> ExecuteComboStep([FromBody] ExecuteComboStepRequest request)
    {
        var (success, message) = await _specialAbilityService.ExecuteComboStepAsync(request.CharacterId, request.ComboId, request.AbilityId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }
}

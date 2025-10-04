using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PartiesController : ControllerBase
{
    private readonly IPartyService _partyService;

    public PartiesController(IPartyService partyService)
    {
        _partyService = partyService;
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetPublicParties()
    {
        var parties = await _partyService.GetPublicPartiesAsync();
        return Ok(parties);
    }

    [HttpGet("{partyId}")]
    public async Task<IActionResult> GetPartyById(int partyId)
    {
        var party = await _partyService.GetPartyByIdAsync(partyId);
        if (party == null)
        {
            return NotFound(new { message = "Grupo não encontrado" });
        }
        return Ok(party);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserParty(int userId)
    {
        var party = await _partyService.GetUserPartyAsync(userId);
        if (party == null)
        {
            return NotFound(new { message = "Usuário não está em nenhum grupo" });
        }
        return Ok(party);
    }

    [HttpGet("user/{userId}/invites")]
    public async Task<IActionResult> GetUserInvites(int userId)
    {
        var invites = await _partyService.GetUserInvitesAsync(userId);
        return Ok(invites);
    }

    public record CreatePartyRequest(int UserId, string Name, string Description, bool IsPublic);
    [HttpPost("create")]
    public async Task<IActionResult> CreateParty([FromBody] CreatePartyRequest request)
    {
        var (success, message, party) = await _partyService.CreatePartyAsync(request.UserId, request.Name, request.Description, request.IsPublic);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message, party });
    }

    public record JoinPartyRequest(int UserId, int PartyId);
    [HttpPost("join")]
    public async Task<IActionResult> JoinParty([FromBody] JoinPartyRequest request)
    {
        var (success, message) = await _partyService.JoinPartyAsync(request.UserId, request.PartyId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record LeavePartyRequest(int UserId, int PartyId);
    [HttpPost("leave")]
    public async Task<IActionResult> LeaveParty([FromBody] LeavePartyRequest request)
    {
        var (success, message) = await _partyService.LeavePartyAsync(request.UserId, request.PartyId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record InviteToPartyRequest(int InviterId, int InviteeId, int PartyId, string Message);
    [HttpPost("invite")]
    public async Task<IActionResult> InviteToParty([FromBody] InviteToPartyRequest request)
    {
        var (success, message) = await _partyService.InviteToPartyAsync(request.InviterId, request.InviteeId, request.PartyId, request.Message);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record RespondToInviteRequest(int UserId, int InviteId, bool Accept);
    [HttpPost("respond-invite")]
    public async Task<IActionResult> RespondToInvite([FromBody] RespondToInviteRequest request)
    {
        var (success, message) = await _partyService.RespondToInviteAsync(request.UserId, request.InviteId, request.Accept);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record KickMemberRequest(int LeaderId, int MemberId, int PartyId);
    [HttpPost("kick-member")]
    public async Task<IActionResult> KickMember([FromBody] KickMemberRequest request)
    {
        var (success, message) = await _partyService.KickMemberAsync(request.LeaderId, request.MemberId, request.PartyId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record TransferLeadershipRequest(int CurrentLeaderId, int NewLeaderId, int PartyId);
    [HttpPost("transfer-leadership")]
    public async Task<IActionResult> TransferLeadership([FromBody] TransferLeadershipRequest request)
    {
        var (success, message) = await _partyService.TransferLeadershipAsync(request.CurrentLeaderId, request.NewLeaderId, request.PartyId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }

    public record DisbandPartyRequest(int LeaderId, int PartyId);
    [HttpPost("disband")]
    public async Task<IActionResult> DisbandParty([FromBody] DisbandPartyRequest request)
    {
        var (success, message) = await _partyService.DisbandPartyAsync(request.LeaderId, request.PartyId);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { message });
    }
}

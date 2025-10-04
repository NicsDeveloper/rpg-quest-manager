using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface IPartyService
{
    Task<List<Party>> GetPublicPartiesAsync();
    Task<Party?> GetPartyByIdAsync(int partyId);
    Task<Party?> GetUserPartyAsync(int userId);
    Task<(bool success, string message, Party? party)> CreatePartyAsync(int userId, string name, string description, bool isPublic);
    Task<(bool success, string message)> JoinPartyAsync(int userId, int partyId);
    Task<(bool success, string message)> LeavePartyAsync(int userId, int partyId);
    Task<(bool success, string message)> InviteToPartyAsync(int inviterId, int inviteeId, int partyId, string message);
    Task<(bool success, string message)> RespondToInviteAsync(int userId, int inviteId, bool accept);
    Task<List<PartyInvite>> GetUserInvitesAsync(int userId);
    Task<(bool success, string message)> KickMemberAsync(int leaderId, int memberId, int partyId);
    Task<(bool success, string message)> TransferLeadershipAsync(int currentLeaderId, int newLeaderId, int partyId);
    Task<(bool success, string message)> DisbandPartyAsync(int leaderId, int partyId);
}

public class PartyService : IPartyService
{
    private readonly ApplicationDbContext _db;

    public PartyService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Party>> GetPublicPartiesAsync()
    {
        return await _db.Parties
            .Include(p => p.Leader)
            .Include(p => p.Members)
            .ThenInclude(m => m.User)
            .Where(p => p.IsPublic && p.Status == PartyStatus.Active)
            .OrderBy(p => p.LastActivityAt)
            .ToListAsync();
    }

    public async Task<Party?> GetPartyByIdAsync(int partyId)
    {
        return await _db.Parties
            .Include(p => p.Leader)
            .Include(p => p.Members)
            .ThenInclude(m => m.User)
            .Include(p => p.Members)
            .ThenInclude(m => m.Character)
            .FirstOrDefaultAsync(p => p.Id == partyId);
    }

    public async Task<Party?> GetUserPartyAsync(int userId)
    {
        return await _db.Parties
            .Include(p => p.Leader)
            .Include(p => p.Members)
            .ThenInclude(m => m.User)
            .Include(p => p.Members)
            .ThenInclude(m => m.Character)
            .FirstOrDefaultAsync(p => p.Members.Any(m => m.UserId == userId) || p.LeaderId == userId);
    }

    public async Task<(bool success, string message, Party? party)> CreatePartyAsync(int userId, string name, string description, bool isPublic)
    {
        try
        {
            // Verificar se o usuário já está em um grupo
            var existingParty = await GetUserPartyAsync(userId);
            if (existingParty != null)
            {
                return (false, "Você já está em um grupo", null);
            }

            var character = await _db.Characters.FirstOrDefaultAsync(c => c.UserId == userId);
            if (character == null)
            {
                return (false, "Personagem não encontrado", null);
            }

            var party = new Party
            {
                Name = name,
                Description = description,
                LeaderId = userId,
                IsPublic = isPublic,
                Status = PartyStatus.Active,
                CreatedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow
            };

            _db.Parties.Add(party);
            await _db.SaveChangesAsync();

            // Adicionar o líder como membro
            var leaderMember = new PartyMember
            {
                PartyId = party.Id,
                UserId = userId,
                CharacterId = character.Id,
                Role = PartyRole.Leader,
                JoinedAt = DateTime.UtcNow,
                LastActiveAt = DateTime.UtcNow
            };

            _db.PartyMembers.Add(leaderMember);
            await _db.SaveChangesAsync();

            return (true, "Grupo criado com sucesso", party);
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao criar grupo: {ex.Message}", null);
        }
    }

    public async Task<(bool success, string message)> JoinPartyAsync(int userId, int partyId)
    {
        try
        {
            var party = await GetPartyByIdAsync(partyId);
            if (party == null)
            {
                return (false, "Grupo não encontrado");
            }

            if (party.Members.Count >= party.MaxMembers)
            {
                return (false, "Grupo está cheio");
            }

            // Verificar se o usuário já está em um grupo
            var existingParty = await GetUserPartyAsync(userId);
            if (existingParty != null)
            {
                return (false, "Você já está em um grupo");
            }

            var character = await _db.Characters.FirstOrDefaultAsync(c => c.UserId == userId);
            if (character == null)
            {
                return (false, "Personagem não encontrado");
            }

            var member = new PartyMember
            {
                PartyId = partyId,
                UserId = userId,
                CharacterId = character.Id,
                Role = PartyRole.Member,
                JoinedAt = DateTime.UtcNow,
                LastActiveAt = DateTime.UtcNow
            };

            _db.PartyMembers.Add(member);
            party.LastActivityAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return (true, "Você entrou no grupo com sucesso");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao entrar no grupo: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> LeavePartyAsync(int userId, int partyId)
    {
        try
        {
            var party = await GetPartyByIdAsync(partyId);
            if (party == null)
            {
                return (false, "Grupo não encontrado");
            }

            var member = party.Members.FirstOrDefault(m => m.UserId == userId);
            if (member == null)
            {
                return (false, "Você não está neste grupo");
            }

            // Se for o líder, transferir liderança ou dissolver o grupo
            if (member.Role == PartyRole.Leader)
            {
                var otherMembers = party.Members.Where(m => m.UserId != userId).ToList();
                if (otherMembers.Any())
                {
                    // Transferir liderança para o primeiro membro
                    var newLeader = otherMembers.First();
                    newLeader.Role = PartyRole.Leader;
                    party.LeaderId = newLeader.UserId;
                }
                else
                {
                    // Dissolver o grupo se não há outros membros
                    party.Status = PartyStatus.Disbanded;
                }
            }

            _db.PartyMembers.Remove(member);
            party.LastActivityAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return (true, "Você saiu do grupo");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao sair do grupo: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> InviteToPartyAsync(int inviterId, int inviteeId, int partyId, string message)
    {
        try
        {
            var party = await GetPartyByIdAsync(partyId);
            if (party == null)
            {
                return (false, "Grupo não encontrado");
            }

            var inviter = party.Members.FirstOrDefault(m => m.UserId == inviterId);
            if (inviter == null || (inviter.Role != PartyRole.Leader && inviter.Role != PartyRole.Officer))
            {
                return (false, "Você não tem permissão para convidar membros");
            }

            // Verificar se o convidado já está em um grupo
            var existingParty = await GetUserPartyAsync(inviteeId);
            if (existingParty != null)
            {
                return (false, "O usuário já está em um grupo");
            }

            // Verificar se já existe um convite pendente
            var existingInvite = await _db.PartyInvites
                .FirstOrDefaultAsync(pi => pi.PartyId == partyId && pi.InviteeId == inviteeId && pi.Status == InviteStatus.Pending);
            
            if (existingInvite != null)
            {
                return (false, "Já existe um convite pendente para este usuário");
            }

            var invite = new PartyInvite
            {
                PartyId = partyId,
                InviterId = inviterId,
                InviteeId = inviteeId,
                Message = message,
                Status = InviteStatus.Pending,
                SentAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            _db.PartyInvites.Add(invite);
            await _db.SaveChangesAsync();

            return (true, "Convite enviado com sucesso");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao enviar convite: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> RespondToInviteAsync(int userId, int inviteId, bool accept)
    {
        try
        {
            var invite = await _db.PartyInvites
                .Include(pi => pi.Party)
                .FirstOrDefaultAsync(pi => pi.Id == inviteId && pi.InviteeId == userId);

            if (invite == null)
            {
                return (false, "Convite não encontrado");
            }

            if (invite.Status != InviteStatus.Pending)
            {
                return (false, "Convite já foi respondido");
            }

            if (invite.ExpiresAt < DateTime.UtcNow)
            {
                invite.Status = InviteStatus.Expired;
                await _db.SaveChangesAsync();
                return (false, "Convite expirado");
            }

            if (accept)
            {
                var result = await JoinPartyAsync(userId, invite.PartyId);
                if (result.success)
                {
                    invite.Status = InviteStatus.Accepted;
                    invite.RespondedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                    return (true, "Convite aceito com sucesso");
                }
                else
                {
                    return result;
                }
            }
            else
            {
                invite.Status = InviteStatus.Declined;
                invite.RespondedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return (true, "Convite recusado");
            }
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao responder convite: {ex.Message}");
        }
    }

    public async Task<List<PartyInvite>> GetUserInvitesAsync(int userId)
    {
        return await _db.PartyInvites
            .Include(pi => pi.Party)
            .Include(pi => pi.Inviter)
            .Where(pi => pi.InviteeId == userId && pi.Status == InviteStatus.Pending && pi.ExpiresAt > DateTime.UtcNow)
            .OrderBy(pi => pi.SentAt)
            .ToListAsync();
    }

    public async Task<(bool success, string message)> KickMemberAsync(int leaderId, int memberId, int partyId)
    {
        try
        {
            var party = await GetPartyByIdAsync(partyId);
            if (party == null)
            {
                return (false, "Grupo não encontrado");
            }

            var leader = party.Members.FirstOrDefault(m => m.UserId == leaderId);
            if (leader == null || leader.Role != PartyRole.Leader)
            {
                return (false, "Você não tem permissão para expulsar membros");
            }

            var member = party.Members.FirstOrDefault(m => m.UserId == memberId);
            if (member == null)
            {
                return (false, "Membro não encontrado");
            }

            if (member.Role == PartyRole.Leader)
            {
                return (false, "Não é possível expulsar o líder do grupo");
            }

            _db.PartyMembers.Remove(member);
            party.LastActivityAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return (true, "Membro expulso com sucesso");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao expulsar membro: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> TransferLeadershipAsync(int currentLeaderId, int newLeaderId, int partyId)
    {
        try
        {
            var party = await GetPartyByIdAsync(partyId);
            if (party == null)
            {
                return (false, "Grupo não encontrado");
            }

            var currentLeader = party.Members.FirstOrDefault(m => m.UserId == currentLeaderId);
            if (currentLeader == null || currentLeader.Role != PartyRole.Leader)
            {
                return (false, "Você não é o líder do grupo");
            }

            var newLeader = party.Members.FirstOrDefault(m => m.UserId == newLeaderId);
            if (newLeader == null)
            {
                return (false, "Novo líder não encontrado no grupo");
            }

            currentLeader.Role = PartyRole.Member;
            newLeader.Role = PartyRole.Leader;
            party.LeaderId = newLeaderId;
            party.LastActivityAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return (true, "Liderança transferida com sucesso");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao transferir liderança: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> DisbandPartyAsync(int leaderId, int partyId)
    {
        try
        {
            var party = await GetPartyByIdAsync(partyId);
            if (party == null)
            {
                return (false, "Grupo não encontrado");
            }

            var leader = party.Members.FirstOrDefault(m => m.UserId == leaderId);
            if (leader == null || leader.Role != PartyRole.Leader)
            {
                return (false, "Você não tem permissão para dissolver o grupo");
            }

            party.Status = PartyStatus.Disbanded;
            party.LastActivityAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return (true, "Grupo dissolvido com sucesso");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao dissolver grupo: {ex.Message}");
        }
    }
}

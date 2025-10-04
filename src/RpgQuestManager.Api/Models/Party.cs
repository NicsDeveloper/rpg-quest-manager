namespace RpgQuestManager.Api.Models;

public class Party
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int LeaderId { get; set; }
    public int MaxMembers { get; set; } = 4;
    public bool IsPublic { get; set; } = true;
    public PartyStatus Status { get; set; } = PartyStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public User Leader { get; set; } = null!;
    public List<PartyMember> Members { get; set; } = new();
    public List<PartyInvite> Invites { get; set; } = new();
}

public class PartyMember
{
    public int Id { get; set; }
    public int PartyId { get; set; }
    public int UserId { get; set; }
    public int HeroId { get; set; }
    public PartyRole Role { get; set; } = PartyRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public Party Party { get; set; } = null!;
    public User User { get; set; } = null!;
    public Hero Hero { get; set; } = null!;
}

public class PartyInvite
{
    public int Id { get; set; }
    public int PartyId { get; set; }
    public int InviterId { get; set; }
    public int InviteeId { get; set; }
    public InviteStatus Status { get; set; } = InviteStatus.Pending;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);

    // Relacionamentos
    public Party Party { get; set; } = null!;
    public User Inviter { get; set; } = null!;
    public User Invitee { get; set; } = null!;
}

public enum PartyStatus
{
    Active,         // Grupo ativo
    Inactive,       // Grupo inativo
    Disbanded       // Grupo dissolvido
}

public enum PartyRole
{
    Leader,         // Líder do grupo
    Officer,        // Oficial (pode convidar)
    Member          // Membro comum
}

public enum InviteStatus
{
    Pending,        // Convite pendente
    Accepted,       // Convite aceito
    Declined,       // Convite recusado
    Expired         // Convite expirado
}

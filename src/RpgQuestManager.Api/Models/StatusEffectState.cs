namespace RpgQuestManager.Api.Models;

public enum EffectTargetKind
{
    Character = 0,
    Monster = 1,
    Hero = 2
}

public class StatusEffectState
{
    public int Id { get; set; }
    public EffectTargetKind TargetKind { get; set; }
    public int TargetId { get; set; }
    public StatusEffectType Type { get; set; }
    public int TurnsRemaining { get; set; } = 1;
}



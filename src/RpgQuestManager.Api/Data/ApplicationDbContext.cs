using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    
    // Entidades principais
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Monster> Monsters => Set<Monster>();
    public DbSet<Quest> Quests => Set<Quest>();
    public DbSet<StatusEffectState> StatusEffects => Set<StatusEffectState>();
    
    // Sistema de itens
    public DbSet<Item> Items => Set<Item>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<CharacterEquipment> CharacterEquipment => Set<CharacterEquipment>();
    
    // Sistema de conquistas
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    
    // Sistema de grupos
    public DbSet<Party> Parties => Set<Party>();
    public DbSet<PartyMember> PartyMembers => Set<PartyMember>();
    public DbSet<PartyInvite> PartyInvites => Set<PartyInvite>();
    
    // Sistema de habilidades especiais
    public DbSet<SpecialAbility> SpecialAbilities => Set<SpecialAbility>();
    public DbSet<CharacterAbility> CharacterAbilities => Set<CharacterAbility>();
    public DbSet<Combo> Combos => Set<Combo>();
    public DbSet<ComboStep> ComboSteps => Set<ComboStep>();
    public DbSet<CharacterCombo> CharacterCombos => Set<CharacterCombo>();
    
    // Sistema de notificações
    public DbSet<Notification> Notifications => Set<Notification>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurações de relacionamentos
        modelBuilder.Entity<Character>()
            .HasOne(c => c.User)
            .WithMany(u => u.Characters)
            .HasForeignKey(c => c.UserId);
            
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Character)
            .WithMany(c => c.Inventory)
            .HasForeignKey(ii => ii.CharacterId);
            
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Item)
            .WithMany()
            .HasForeignKey(ii => ii.ItemId);
            
        modelBuilder.Entity<CharacterEquipment>()
            .HasOne(ce => ce.Character)
            .WithOne(c => c.Equipment)
            .HasForeignKey<CharacterEquipment>(ce => ce.CharacterId);
            
        modelBuilder.Entity<UserSession>()
            .HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId);
            
        // Índices para performance
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
            
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
            
        modelBuilder.Entity<UserSession>()
            .HasIndex(us => us.Token)
            .IsUnique();
            
        modelBuilder.Entity<InventoryItem>()
            .HasIndex(ii => new { ii.CharacterId, ii.ItemId })
            .IsUnique();
            
        // Configurações de conquistas
        modelBuilder.Entity<UserAchievement>()
            .HasOne(ua => ua.User)
            .WithMany()
            .HasForeignKey(ua => ua.UserId);
            
        modelBuilder.Entity<UserAchievement>()
            .HasOne(ua => ua.Achievement)
            .WithMany(a => a.UserAchievements)
            .HasForeignKey(ua => ua.AchievementId);
            
        modelBuilder.Entity<UserAchievement>()
            .HasIndex(ua => new { ua.UserId, ua.AchievementId })
            .IsUnique();
            
        // Configurações de grupos
        modelBuilder.Entity<Party>()
            .HasOne(p => p.Leader)
            .WithMany()
            .HasForeignKey(p => p.LeaderId);
            
        modelBuilder.Entity<PartyMember>()
            .HasOne(pm => pm.Party)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.PartyId);
            
        modelBuilder.Entity<PartyMember>()
            .HasOne(pm => pm.User)
            .WithMany()
            .HasForeignKey(pm => pm.UserId);
            
        modelBuilder.Entity<PartyMember>()
            .HasOne(pm => pm.Character)
            .WithMany()
            .HasForeignKey(pm => pm.CharacterId);
            
        modelBuilder.Entity<PartyMember>()
            .HasIndex(pm => new { pm.PartyId, pm.UserId })
            .IsUnique();
            
        modelBuilder.Entity<PartyInvite>()
            .HasOne(pi => pi.Party)
            .WithMany(p => p.Invites)
            .HasForeignKey(pi => pi.PartyId);
            
        modelBuilder.Entity<PartyInvite>()
            .HasOne(pi => pi.Inviter)
            .WithMany()
            .HasForeignKey(pi => pi.InviterId);
            
        modelBuilder.Entity<PartyInvite>()
            .HasOne(pi => pi.Invitee)
            .WithMany()
            .HasForeignKey(pi => pi.InviteeId);
            
        // Configurações de habilidades especiais
        modelBuilder.Entity<CharacterAbility>()
            .HasOne(ca => ca.Character)
            .WithMany()
            .HasForeignKey(ca => ca.CharacterId);
            
        modelBuilder.Entity<CharacterAbility>()
            .HasOne(ca => ca.Ability)
            .WithMany(sa => sa.CharacterAbilities)
            .HasForeignKey(ca => ca.AbilityId);
            
        modelBuilder.Entity<CharacterAbility>()
            .HasIndex(ca => new { ca.CharacterId, ca.AbilityId })
            .IsUnique();
            
        modelBuilder.Entity<ComboStep>()
            .HasOne(cs => cs.Combo)
            .WithMany(c => c.Steps)
            .HasForeignKey(cs => cs.ComboId);
            
        modelBuilder.Entity<ComboStep>()
            .HasOne(cs => cs.Ability)
            .WithMany(sa => sa.ComboSteps)
            .HasForeignKey(cs => cs.AbilityId);
            
        modelBuilder.Entity<CharacterCombo>()
            .HasOne(cc => cc.Character)
            .WithMany()
            .HasForeignKey(cc => cc.CharacterId);
            
        modelBuilder.Entity<CharacterCombo>()
            .HasOne(cc => cc.Combo)
            .WithMany(c => c.CharacterCombos)
            .HasForeignKey(cc => cc.ComboId);
            
        modelBuilder.Entity<CharacterCombo>()
            .HasIndex(cc => new { cc.CharacterId, cc.ComboId })
            .IsUnique();
            
        // Configurações de notificações
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId);
            
        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.UserId, n.CreatedAt });
            
        // Configurações para propriedades List<T> - conversão para JSON
        modelBuilder.Entity<Item>()
            .Property(i => i.StatusEffects)
            .HasConversion(
                v => v.Select(x => (int)x).ToArray(),
                v => v.Select(x => (StatusEffectType)x).ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<StatusEffectType>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
            
        modelBuilder.Entity<Item>()
            .Property(i => i.RequiredClasses)
            .HasConversion(
                v => v.ToArray(),
                v => v.ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
            
        modelBuilder.Entity<Item>()
            .Property(i => i.DroppedBy)
            .HasConversion(
                v => v.Select(x => (int)x).ToArray(),
                v => v.Select(x => (MonsterType)x).ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<MonsterType>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
            
        modelBuilder.Entity<Item>()
            .Property(i => i.FoundIn)
            .HasConversion(
                v => v.Select(x => (int)x).ToArray(),
                v => v.Select(x => (EnvironmentType)x).ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<EnvironmentType>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
            
        modelBuilder.Entity<Item>()
            .Property(i => i.ShopTypes)
            .HasConversion(
                v => v.ToArray(),
                v => v.ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
            
        modelBuilder.Entity<Monster>()
            .Property(m => m.StatusEffects)
            .HasConversion(
                v => v.Select(x => (int)x).ToArray(),
                v => v.Select(x => (StatusEffectType)x).ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<StatusEffectType>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
            
        modelBuilder.Entity<Monster>()
            .Property(m => m.SpecialAbilities)
            .HasConversion(
                v => v.Select(x => (int)x).ToArray(),
                v => v.Select(x => (StatusEffectType)x).ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<StatusEffectType>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
            
        modelBuilder.Entity<Quest>()
            .Property(q => q.Prerequisites)
            .HasConversion(
                v => v.ToArray(),
                v => v.ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
            
        modelBuilder.Entity<Quest>()
            .Property(q => q.Rewards)
            .HasConversion(
                v => v.ToArray(),
                v => v.ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
            
        modelBuilder.Entity<SpecialAbility>()
            .Property(sa => sa.StatusEffects)
            .HasConversion(
                v => v.Select(x => (int)x).ToArray(),
                v => v.Select(x => (StatusEffectType)x).ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<StatusEffectType>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
            
        modelBuilder.Entity<SpecialAbility>()
            .Property(sa => sa.StatusEffectsToRemove)
            .HasConversion(
                v => v.Select(x => (int)x).ToArray(),
                v => v.Select(x => (StatusEffectType)x).ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<StatusEffectType>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
    }
}



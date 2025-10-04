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
    public DbSet<Hero> Heroes => Set<Hero>();
    public DbSet<HeroQuest> HeroQuests => Set<HeroQuest>();
    public DbSet<Monster> Monsters => Set<Monster>();
    public DbSet<Quest> Quests => Set<Quest>();
    public DbSet<StatusEffectState> StatusEffects => Set<StatusEffectState>();
    
    // Sistema de itens
    public DbSet<Item> Items => Set<Item>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<HeroEquipment> HeroEquipment => Set<HeroEquipment>();
    
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
    
    // Sistema de dados
    public DbSet<DiceInventory> DiceInventories => Set<DiceInventory>();
    public DbSet<DiceShopItem> DiceShopItems => Set<DiceShopItem>();
    
    // Sistema de recompensas
    public DbSet<CombatRewards> CombatRewards => Set<CombatRewards>();
    public DbSet<QuestRewards> QuestRewards => Set<QuestRewards>();
    public DbSet<CombatReward> CombatRewardItems => Set<CombatReward>();
    public DbSet<QuestReward> QuestRewardItems => Set<QuestReward>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurações de relacionamentos
        modelBuilder.Entity<Character>()
            .HasOne(c => c.User)
            .WithMany(u => u.Characters)
            .HasForeignKey(c => c.UserId);
            
        modelBuilder.Entity<Hero>()
            .HasOne(h => h.User)
            .WithMany(u => u.Heroes)
            .HasForeignKey(h => h.UserId);
            
        modelBuilder.Entity<HeroQuest>()
            .HasOne(hq => hq.Hero)
            .WithMany(h => h.HeroQuests)
            .HasForeignKey(hq => hq.HeroId);
            
        modelBuilder.Entity<HeroQuest>()
            .HasOne(hq => hq.Quest)
            .WithMany()
            .HasForeignKey(hq => hq.QuestId);
            
        modelBuilder.Entity<HeroQuest>()
            .HasIndex(hq => new { hq.HeroId, hq.QuestId })
            .IsUnique();
            
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Hero)
            .WithMany(h => h.InventoryItems)
            .HasForeignKey(ii => ii.HeroId);
            
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Item)
            .WithMany()
            .HasForeignKey(ii => ii.ItemId);
            
        modelBuilder.Entity<HeroEquipment>()
            .HasOne(he => he.Hero)
            .WithOne(h => h.Equipment)
            .HasForeignKey<HeroEquipment>(he => he.HeroId);
            
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
            .HasIndex(ii => new { ii.HeroId, ii.ItemId })
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
            .HasOne(pm => pm.Hero)
            .WithMany()
            .HasForeignKey(pm => pm.HeroId);
            
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
                
        // Configurações do sistema de dados
        modelBuilder.Entity<DiceInventory>()
            .HasOne(di => di.Hero)
            .WithMany()
            .HasForeignKey(di => di.HeroId);
            
        modelBuilder.Entity<DiceInventory>()
            .HasIndex(di => new { di.HeroId, di.DiceType })
            .IsUnique();
            
        // Configurações do sistema de recompensas
        modelBuilder.Entity<CombatRewards>()
            .HasOne(cr => cr.Hero)
            .WithMany()
            .HasForeignKey(cr => cr.HeroId);
            
        modelBuilder.Entity<CombatRewards>()
            .HasOne(cr => cr.Quest)
            .WithMany()
            .HasForeignKey(cr => cr.QuestId);
            
        modelBuilder.Entity<CombatRewards>()
            .HasOne(cr => cr.Monster)
            .WithMany()
            .HasForeignKey(cr => cr.MonsterId);
            
        modelBuilder.Entity<CombatRewards>()
            .HasMany(cr => cr.Rewards)
            .WithOne()
            .HasForeignKey(cr => cr.CombatRewardsId);
            
        modelBuilder.Entity<QuestRewards>()
            .HasOne(qr => qr.Hero)
            .WithMany()
            .HasForeignKey(qr => qr.HeroId);
            
        modelBuilder.Entity<QuestRewards>()
            .HasOne(qr => qr.Quest)
            .WithMany()
            .HasForeignKey(qr => qr.QuestId);
            
        modelBuilder.Entity<QuestRewards>()
            .HasMany(qr => qr.Rewards)
            .WithOne()
            .HasForeignKey(qr => qr.QuestRewardsId);
            
        // Configurações para propriedades JSON
        modelBuilder.Entity<CombatReward>()
            .Property(cr => cr.Type)
            .HasConversion<int>();
            
        modelBuilder.Entity<CombatReward>()
            .Property(cr => cr.DiceType)
            .HasConversion<int?>();
            
        modelBuilder.Entity<CombatReward>()
            .Property(cr => cr.Rarity)
            .HasConversion<int?>();
            
        modelBuilder.Entity<QuestReward>()
            .Property(qr => qr.Type)
            .HasConversion<int>();
            
        modelBuilder.Entity<QuestReward>()
            .Property(qr => qr.DiceType)
            .HasConversion<int?>();
            
        modelBuilder.Entity<QuestReward>()
            .Property(qr => qr.Rarity)
            .HasConversion<int?>();
    }
}



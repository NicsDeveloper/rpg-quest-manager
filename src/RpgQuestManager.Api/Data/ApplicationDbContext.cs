using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Hero> Heroes { get; set; }
    public DbSet<Quest> Quests { get; set; }
    public DbSet<Enemy> Enemies { get; set; }
    public DbSet<Reward> Rewards { get; set; }
    public DbSet<HeroQuest> HeroQuests { get; set; }
    public DbSet<QuestEnemy> QuestEnemies { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<HeroItem> HeroItems { get; set; }
    public DbSet<HeroTraining> HeroTrainings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    // Sistema de Dados
    public DbSet<DiceInventory> DiceInventories { get; set; }
    public DbSet<BossDropTable> BossDropTables { get; set; }
    
        // Sistema de Combate
        public DbSet<CombatSession> CombatSessions { get; set; }
        public DbSet<CombatLog> CombatLogs { get; set; }
        public DbSet<StatusEffect> StatusEffects { get; set; }
        
    // Sistema de Condições Ambientais e Morale
    public DbSet<EnvironmentalCondition> EnvironmentalConditions { get; set; }
    public DbSet<MoraleState> MoraleStates { get; set; }
    
    // Sistema de Missões Redesenhado
    public DbSet<QuestCategory> QuestCategories { get; set; }
    public DbSet<Monster> Monsters { get; set; }
    
    // Sistema de Combos e Party
    public DbSet<PartyCombo> PartyCombos { get; set; }
    public DbSet<BossWeakness> BossWeaknesses { get; set; }
    public DbSet<ComboDiscovery> ComboDiscoveries { get; set; }
    public DbSet<FreeDiceGrant> FreeDiceGrants { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configuração User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        });
        
        // Configuração Hero
        modelBuilder.Entity<Hero>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Class).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Configuração Quest
        modelBuilder.Entity<Quest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(5000); // Aumentado para suportar lore rica
            entity.Property(e => e.Difficulty).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RequiredClass).IsRequired().HasMaxLength(50);
            
            // Relacionamento com Monster principal
            entity.HasOne<Monster>()
                .WithMany()
                .HasForeignKey(e => e.MainMonsterId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Configuração Enemy
        modelBuilder.Entity<Enemy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
        });
        
        // Configuração Reward
        modelBuilder.Entity<Reward>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Quest)
                .WithMany(q => q.Rewards)
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Configuração HeroQuest (tabela de relacionamento)
        modelBuilder.Entity<HeroQuest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Hero)
                .WithMany(h => h.HeroQuests)
                .HasForeignKey(e => e.HeroId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Quest)
                .WithMany(q => q.HeroQuests)
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configuração QuestEnemy (tabela de relacionamento)
        modelBuilder.Entity<QuestEnemy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Quest)
                .WithMany(q => q.QuestEnemies)
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Enemy)
                .WithMany(en => en.QuestEnemies)
                .HasForeignKey(e => e.EnemyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configuração Item
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
        });
        
        // Configuração HeroItem (inventário)
        modelBuilder.Entity<HeroItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Hero)
                .WithMany(h => h.HeroItems)
                .HasForeignKey(e => e.HeroId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Item)
                .WithMany(i => i.HeroItems)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configuração HeroTraining
        modelBuilder.Entity<HeroTraining>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TrainingType).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Hero)
                .WithMany()
                .HasForeignKey(e => e.HeroId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configuração Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configuração DiceInventory
        modelBuilder.Entity<DiceInventory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<DiceInventory>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configuração CombatSession
        modelBuilder.Entity<CombatSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HeroIds).IsRequired();
            entity.Property(e => e.HeroHealths).IsRequired();
            entity.Property(e => e.MaxHeroHealths).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Quest)
                .WithMany()
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.CurrentEnemy)
                .WithMany()
                .HasForeignKey(e => e.CurrentEnemyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Configuração CombatLog
        modelBuilder.Entity<CombatLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.HasOne(e => e.CombatSession)
                .WithMany(cs => cs.CombatLogs)
                .HasForeignKey(e => e.CombatSessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Enemy)
                .WithMany()
                .HasForeignKey(e => e.EnemyId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuração StatusEffect
        modelBuilder.Entity<StatusEffect>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.HasOne(e => e.CombatSession)
                .WithMany()
                .HasForeignKey(e => e.CombatSessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Hero)
                .WithMany()
                .HasForeignKey(e => e.HeroId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Enemy)
                .WithMany()
                .HasForeignKey(e => e.EnemyId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuração EnvironmentalCondition
        modelBuilder.Entity<EnvironmentalCondition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
            entity.HasOne(e => e.Quest)
                .WithMany(q => q.EnvironmentalConditions)
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração MoraleState
        modelBuilder.Entity<MoraleState>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Level).IsRequired();
            entity.HasOne(e => e.CombatSession)
                .WithMany()
                .HasForeignKey(e => e.CombatSessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Hero)
                .WithMany()
                .HasForeignKey(e => e.HeroId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Enemy)
                .WithMany()
                .HasForeignKey(e => e.EnemyId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuração QuestCategory
        modelBuilder.Entity<QuestCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(10);
            entity.Property(e => e.Color).HasMaxLength(7);
        });

        // Configuração Monster
        modelBuilder.Entity<Monster>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(10);
            entity.Property(e => e.Color).HasMaxLength(7);
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.Lore).HasMaxLength(1000);
            entity.Property(e => e.Origin).HasMaxLength(200);
            entity.Property(e => e.Weakness).HasMaxLength(200);
        });

        // Configuração Quest atualizada
        modelBuilder.Entity<Quest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Difficulty).HasMaxLength(50);
            entity.Property(e => e.RequiredClass).HasMaxLength(50);
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Quests)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Configuração BossDropTable
        modelBuilder.Entity<BossDropTable>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Enemy)
                .WithMany(en => en.BossDrops)
                .HasForeignKey(e => e.EnemyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Item)
                .WithMany(i => i.BossDrops)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.DropChance).HasPrecision(5, 2);
        });
        
        // Configuração PartyCombo
        modelBuilder.Entity<PartyCombo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RequiredClass1).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RequiredClass2).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RequiredClass3).HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Effect).IsRequired().HasMaxLength(500);
        });
        
        // Configuração BossWeakness
        modelBuilder.Entity<BossWeakness>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Enemy)
                .WithMany()
                .HasForeignKey(e => e.EnemyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Combo)
                .WithMany(c => c.BossWeaknesses)
                .HasForeignKey(e => e.ComboId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.DropMultiplier).HasPrecision(5, 2);
            entity.Property(e => e.ExpMultiplier).HasPrecision(5, 2);
            entity.Property(e => e.FlavorText).HasMaxLength(500);
        });
        
        // Configuração ComboDiscovery
        modelBuilder.Entity<ComboDiscovery>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Enemy)
                .WithMany()
                .HasForeignKey(e => e.EnemyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Combo)
                .WithMany(c => c.ComboDiscoveries)
                .HasForeignKey(e => e.ComboId)
                .OnDelete(DeleteBehavior.Cascade);
            // Índice único: usuário só pode descobrir um combo por boss uma vez
            entity.HasIndex(e => new { e.UserId, e.EnemyId, e.ComboId }).IsUnique();
        });
        
        // Configuração FreeDiceGrant
        modelBuilder.Entity<FreeDiceGrant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // Índice único: um grant por tipo de dado por usuário
            entity.HasIndex(e => new { e.UserId, e.DiceType }).IsUnique();
        });
    }
}


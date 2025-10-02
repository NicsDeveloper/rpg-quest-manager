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
    public DbSet<Notification> Notifications { get; set; }
    
    // Sistema de Combate e Dados
    public DbSet<DiceInventory> DiceInventories { get; set; }
    public DbSet<CombatSession> CombatSessions { get; set; }
    public DbSet<CombatLog> CombatLogs { get; set; }
    public DbSet<BossDropTable> BossDropTables { get; set; }
    
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
            entity.HasOne(e => e.Hero)
                .WithOne()
                .HasForeignKey<DiceInventory>(e => e.HeroId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configuração CombatSession
        modelBuilder.Entity<CombatSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HeroIds).IsRequired();
            entity.HasOne(e => e.Quest)
                .WithMany()
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.CurrentEnemy)
                .WithMany()
                .HasForeignKey(e => e.CurrentEnemyId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Combo)
                .WithMany()
                .HasForeignKey(e => e.ComboId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Configuração CombatLog
        modelBuilder.Entity<CombatLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.CombatSession)
                .WithMany(cs => cs.CombatLogs)
                .HasForeignKey(e => e.CombatSessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Enemy)
                .WithMany()
                .HasForeignKey(e => e.EnemyId)
                .OnDelete(DeleteBehavior.SetNull);
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


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
        });
        
        // Configuração Quest
        modelBuilder.Entity<Quest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Difficulty).IsRequired().HasMaxLength(50);
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
    }
}


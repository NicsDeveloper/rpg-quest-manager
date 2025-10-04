using Microsoft.EntityFrameworkCore;
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
    }
}



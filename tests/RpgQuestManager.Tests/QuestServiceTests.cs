using AutoMapper;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Events;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Services;

namespace RpgQuestManager.Tests;

public class QuestServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<ILogger<QuestService>> _loggerMock;
    private readonly QuestService _questService;
    
    public QuestServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new ApplicationDbContext(options);
        _mapperMock = new Mock<IMapper>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _loggerMock = new Mock<ILogger<QuestService>>();
        
        _questService = new QuestService(
            _context,
            _mapperMock.Object,
            _publishEndpointMock.Object,
            _loggerMock.Object
        );
    }
    
    [Fact]
    public async Task CompleteQuest_Should_Give_Rewards_To_Hero()
    {
        // Arrange
        var hero = new Hero
        {
            Id = 1,
            Name = "Test Hero",
            Class = "Guerreiro",
            Level = 1,
            Experience = 0,
            Gold = 0
        };
        
        var quest = new Quest
        {
            Id = 1,
            Name = "Kill Goblins",
            Description = "Defeat 10 goblins",
            Difficulty = "Fácil",
            ExperienceReward = 50,
            GoldReward = 100
        };
        
        _context.Heroes.Add(hero);
        _context.Quests.Add(quest);
        await _context.SaveChangesAsync();
        
        // Act
        await _questService.CompleteQuestAsync(hero.Id, quest.Id);
        
        // Assert
        var updatedHero = await _context.Heroes.FindAsync(hero.Id);
        updatedHero!.Experience.Should().Be(50);
        updatedHero.Gold.Should().Be(100);
        
        var heroQuest = await _context.HeroQuests
            .FirstOrDefaultAsync(hq => hq.HeroId == hero.Id && hq.QuestId == quest.Id);
        
        heroQuest.Should().NotBeNull();
        heroQuest!.IsCompleted.Should().BeTrue();
    }
    
    [Fact]
    public async Task CompleteQuest_Should_Level_Up_Hero_If_Enough_Experience()
    {
        // Arrange
        var hero = new Hero
        {
            Id = 1,
            Name = "Test Hero",
            Class = "Mago",
            Level = 1,
            Experience = 60, // Já tem 60, vai ganhar mais 50
            Strength = 10,
            Intelligence = 10,
            Dexterity = 10
        };
        
        var quest = new Quest
        {
            Id = 1,
            Name = "Collect Herbs",
            Description = "Collect 20 magic herbs",
            Difficulty = "Médio",
            ExperienceReward = 50, // Total: 110 XP (suficiente para level 2)
            GoldReward = 50
        };
        
        _context.Heroes.Add(hero);
        _context.Quests.Add(quest);
        await _context.SaveChangesAsync();
        
        // Act
        await _questService.CompleteQuestAsync(hero.Id, quest.Id);
        
        // Assert
        var updatedHero = await _context.Heroes.FindAsync(hero.Id);
        updatedHero!.Level.Should().Be(2); // Subiu de nível
        updatedHero.Experience.Should().Be(10); // 110 - 100 = 10 XP restantes
        updatedHero.Strength.Should().Be(12); // +2 por level up
    }
    
    [Fact]
    public async Task CompleteQuest_Should_Throw_Exception_If_Hero_Not_Found()
    {
        // Arrange
        var quest = new Quest
        {
            Id = 1,
            Name = "Test Quest",
            Difficulty = "Fácil"
        };
        
        _context.Quests.Add(quest);
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await _questService.CompleteQuestAsync(999, quest.Id)
        );
    }
    
    [Fact]
    public async Task CompleteQuest_Should_Throw_Exception_If_Quest_Already_Completed()
    {
        // Arrange
        var hero = new Hero
        {
            Id = 1,
            Name = "Test Hero",
            Class = "Guerreiro"
        };
        
        var quest = new Quest
        {
            Id = 1,
            Name = "Already Completed Quest",
            Difficulty = "Fácil",
            ExperienceReward = 50,
            GoldReward = 100
        };
        
        var heroQuest = new HeroQuest
        {
            HeroId = hero.Id,
            QuestId = quest.Id,
            IsCompleted = true,
            CompletedAt = DateTime.UtcNow
        };
        
        _context.Heroes.Add(hero);
        _context.Quests.Add(quest);
        _context.HeroQuests.Add(heroQuest);
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await _questService.CompleteQuestAsync(hero.Id, quest.Id)
        );
    }
    
    [Fact]
    public async Task CompleteQuest_Should_Publish_QuestCompletedEvent()
    {
        // Arrange
        var hero = new Hero
        {
            Id = 1,
            Name = "Event Hero",
            Class = "Paladino",
            Level = 5
        };
        
        var quest = new Quest
        {
            Id = 1,
            Name = "Event Quest",
            Difficulty = "Épico",
            ExperienceReward = 200,
            GoldReward = 500
        };
        
        _context.Heroes.Add(hero);
        _context.Quests.Add(quest);
        await _context.SaveChangesAsync();
        
        // Act
        await _questService.CompleteQuestAsync(hero.Id, quest.Id);
        
        // Assert
        _publishEndpointMock.Verify(
            x => x.Publish(
                It.Is<QuestCompletedEvent>(e => 
                    e.HeroId == hero.Id &&
                    e.QuestId == quest.Id &&
                    e.ExperienceGained == 200 &&
                    e.GoldGained == 500
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}


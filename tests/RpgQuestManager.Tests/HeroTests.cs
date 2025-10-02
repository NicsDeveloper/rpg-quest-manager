using FluentAssertions;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Tests;

public class HeroTests
{
    [Fact]
    public void Hero_Should_Have_Initial_Level_One()
    {
        // Arrange & Act
        var hero = new Hero
        {
            Name = "Conan",
            Class = "Guerreiro"
        };
        
        // Assert
        hero.Level.Should().Be(1);
        hero.Experience.Should().Be(0);
    }
    
    [Fact]
    public void Hero_Should_Calculate_Experience_For_Next_Level_Correctly()
    {
        // Arrange
        var hero = new Hero
        {
            Name = "Gandalf",
            Class = "Mago",
            Level = 5
        };
        
        // Act
        var expNeeded = hero.GetExperienceForNextLevel();
        
        // Assert
        expNeeded.Should().Be(500); // Level 5 * 100 = 500
    }
    
    [Fact]
    public void Hero_Should_Level_Up_When_Has_Enough_Experience()
    {
        // Arrange
        var hero = new Hero
        {
            Name = "Legolas",
            Class = "Arqueiro",
            Level = 1,
            Experience = 0,
            Strength = 10,
            Intelligence = 10,
            Dexterity = 10
        };
        
        // Act
        hero.Experience = 100; // Level 1 precisa de 100 XP
        var canLevelUp = hero.CanLevelUp();
        
        // Assert
        canLevelUp.Should().BeTrue();
        
        // Act - Level Up
        hero.LevelUp();
        
        // Assert
        hero.Level.Should().Be(2);
        hero.Experience.Should().Be(0); // XP restante após level up
        hero.Strength.Should().Be(12); // +2 por level
        hero.Intelligence.Should().Be(12);
        hero.Dexterity.Should().Be(12);
    }
    
    [Fact]
    public void Hero_Should_Level_Up_Multiple_Times_If_Has_Enough_Experience()
    {
        // Arrange
        var hero = new Hero
        {
            Name = "Arthur",
            Class = "Paladino",
            Level = 1,
            Experience = 0
        };
        
        // Act - Ganha 350 XP (Level 1->2 = 100, Level 2->3 = 200, sobra 50)
        hero.Experience = 350;
        hero.LevelUp();
        
        // Assert
        hero.Level.Should().Be(3);
        hero.Experience.Should().Be(50); // Sobrou 50 XP
    }
    
    [Fact]
    public void Hero_Should_Not_Level_Up_Without_Enough_Experience()
    {
        // Arrange
        var hero = new Hero
        {
            Name = "Frodo",
            Class = "Ladino",
            Level = 1,
            Experience = 50
        };
        
        // Act
        var initialLevel = hero.Level;
        hero.LevelUp();
        
        // Assert
        hero.Level.Should().Be(initialLevel);
        hero.Experience.Should().Be(50);
    }
}


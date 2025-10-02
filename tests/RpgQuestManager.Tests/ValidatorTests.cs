using FluentAssertions;
using RpgQuestManager.Api.DTOs.Auth;
using RpgQuestManager.Api.DTOs.Heroes;
using RpgQuestManager.Api.DTOs.Quests;
using RpgQuestManager.Api.Validators;

namespace RpgQuestManager.Tests;

public class ValidatorTests
{
    [Fact]
    public void RegisterRequest_Should_Be_Valid_With_Valid_Data()
    {
        // Arrange
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };
        
        // Act
        var result = validator.Validate(request);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void RegisterRequest_Should_Be_Invalid_With_Short_Username()
    {
        // Arrange
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Username = "ab", // Menos de 3 caracteres
            Email = "test@example.com",
            Password = "password123"
        };
        
        // Act
        var result = validator.Validate(request);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }
    
    [Fact]
    public void RegisterRequest_Should_Be_Invalid_With_Invalid_Email()
    {
        // Arrange
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "invalid-email",
            Password = "password123"
        };
        
        // Act
        var result = validator.Validate(request);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }
    
    [Fact]
    public void CreateHeroRequest_Should_Be_Valid_With_Valid_Class()
    {
        // Arrange
        var validator = new CreateHeroRequestValidator();
        var request = new CreateHeroRequest
        {
            Name = "Aragorn",
            Class = "Guerreiro",
            Strength = 15,
            Intelligence = 12,
            Dexterity = 14
        };
        
        // Act
        var result = validator.Validate(request);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void CreateHeroRequest_Should_Be_Invalid_With_Invalid_Class()
    {
        // Arrange
        var validator = new CreateHeroRequestValidator();
        var request = new CreateHeroRequest
        {
            Name = "Test Hero",
            Class = "InvalidClass",
            Strength = 10,
            Intelligence = 10,
            Dexterity = 10
        };
        
        // Act
        var result = validator.Validate(request);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Class");
    }
    
    [Theory]
    [InlineData(0)]  // Menor que mínimo
    [InlineData(101)] // Maior que máximo
    public void CreateHeroRequest_Should_Be_Invalid_With_Attribute_Out_Of_Range(int attributeValue)
    {
        // Arrange
        var validator = new CreateHeroRequestValidator();
        var request = new CreateHeroRequest
        {
            Name = "Test Hero",
            Class = "Mago",
            Strength = attributeValue,
            Intelligence = 10,
            Dexterity = 10
        };
        
        // Act
        var result = validator.Validate(request);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Strength");
    }
    
    [Fact]
    public void CreateQuestRequest_Should_Be_Valid_With_Valid_Difficulty()
    {
        // Arrange
        var validator = new CreateQuestRequestValidator();
        var request = new CreateQuestRequest
        {
            Name = "Dragon Slayer",
            Description = "Defeat the ancient dragon",
            Difficulty = "Épico",
            ExperienceReward = 1000,
            GoldReward = 5000
        };
        
        // Act
        var result = validator.Validate(request);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void CreateQuestRequest_Should_Be_Invalid_With_Invalid_Difficulty()
    {
        // Arrange
        var validator = new CreateQuestRequestValidator();
        var request = new CreateQuestRequest
        {
            Name = "Test Quest",
            Description = "Test description",
            Difficulty = "VeryHard", // Dificuldade inválida
            ExperienceReward = 100,
            GoldReward = 50
        };
        
        // Act
        var result = validator.Validate(request);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Difficulty");
    }
}


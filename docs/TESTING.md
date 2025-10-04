# 🧪 Testing - RPG Quest Manager

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Estratégia de Testes](#-estratégia-de-testes)
- [Testes Unitários](#-testes-unitários)
- [Testes de Integração](#-testes-de-integração)
- [Testes de API](#-testes-de-api)
- [Testes E2E](#-testes-e2e)
- [Testes de Performance](#-testes-de-performance)
- [Testes de Segurança](#-testes-de-segurança)
- [Testes de Acessibilidade](#-testes-de-acessibilidade)
- [Cobertura de Código](#-cobertura-de-código)
- [CI/CD](#-cicd)
- [Ferramentas](#-ferramentas)

## 🎯 Visão Geral

Este documento detalha a estratégia de testes do RPG Quest Manager, incluindo testes unitários, integração, API, E2E, performance, segurança e acessibilidade.

### Princípios de Testes
- **Test Pyramid**: Mais testes unitários, menos testes E2E
- **AAA Pattern**: Arrange, Act, Assert
- **FIRST**: Fast, Independent, Repeatable, Self-validating, Timely
- **TDD**: Test-Driven Development
- **BDD**: Behavior-Driven Development

## 🏗️ Estratégia de Testes

### Pirâmide de Testes
```
                    ┌─────────────────┐
                    │   E2E Tests     │ ← Poucos, lentos, caros
                    │   (10-20%)      │
                    └─────────────────┘
                ┌─────────────────────────┐
                │   Integration Tests     │ ← Alguns, médios
                │      (20-30%)           │
                └─────────────────────────┘
        ┌─────────────────────────────────────────┐
        │         Unit Tests                      │ ← Muitos, rápidos, baratos
        │           (60-70%)                     │
        └─────────────────────────────────────────┘
```

### Tipos de Testes
- **Unit Tests**: Testam unidades individuais de código
- **Integration Tests**: Testam integração entre componentes
- **API Tests**: Testam endpoints da API
- **E2E Tests**: Testam fluxos completos do usuário
- **Performance Tests**: Testam performance e carga
- **Security Tests**: Testam vulnerabilidades de segurança
- **Accessibility Tests**: Testam acessibilidade

## 🔬 Testes Unitários

### Configuração do xUnit
```csharp
// RpgQuestManager.Tests.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
    <PackageReference Include="Moq" Version="4.20.69" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="AutoFixture" Version="4.18.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\RpgQuestManager.Api\RpgQuestManager.Api.csproj" />
  </ItemGroup>
</Project>
```

### Exemplo de Teste Unitário
```csharp
// Tests/Services/CombatServiceTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using AutoFixture;
using RpgQuestManager.Api.Services;
using RpgQuestManager.Api.Models;
using RpgQuestManager.Api.Data;

namespace RpgQuestManager.Tests.Services;

public class CombatServiceTests
{
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<IDiceService> _mockDiceService;
    private readonly Mock<IMoraleService> _mockMoraleService;
    private readonly Mock<IStatusEffectService> _mockStatusEffectService;
    private readonly Mock<ILevelUpService> _mockLevelUpService;
    private readonly Fixture _fixture;
    private readonly CombatService _sut;

    public CombatServiceTests()
    {
        _mockContext = new Mock<ApplicationDbContext>();
        _mockDiceService = new Mock<IDiceService>();
        _mockMoraleService = new Mock<IMoraleService>();
        _mockStatusEffectService = new Mock<IStatusEffectService>();
        _mockLevelUpService = new Mock<ILevelUpService>();
        _fixture = new Fixture();
        _sut = new CombatService(
            _mockContext.Object,
            _mockDiceService.Object,
            _mockMoraleService.Object,
            _mockStatusEffectService.Object,
            _mockLevelUpService.Object
        );
    }

    [Fact]
    public async Task AttackAsync_WhenHeroAttacksMonster_ShouldReturnCombatResult()
    {
        // Arrange
        var hero = _fixture.Create<Character>();
        var monster = _fixture.Create<Monster>();
        var diceResult = 15;
        var moraleLevel = MoraleLevel.Normal;

        _mockDiceService.Setup(x => x.RollDice(20))
            .Returns(diceResult);
        _mockMoraleService.Setup(x => x.GetMoraleLevel(hero.Morale))
            .Returns(moraleLevel);
        _mockStatusEffectService.Setup(x => x.GetActiveEffectsAsync(EffectTargetKind.Character, hero.Id))
            .ReturnsAsync(new List<StatusEffectState>());

        // Act
        var result = await _sut.AttackAsync(hero.Id, monster.Id);

        // Assert
        result.Should().NotBeNull();
        result.Hero.Should().BeEquivalentTo(hero);
        result.Monster.Should().BeEquivalentTo(monster);
        result.CombatEnded.Should().BeFalse();
    }

    [Theory]
    [InlineData(1, 5, 10, 15)] // Hero level, monster level, expected damage range
    [InlineData(5, 10, 20, 30)]
    [InlineData(10, 15, 40, 60)]
    public async Task AttackAsync_WithDifferentLevels_ShouldCalculateCorrectDamage(
        int heroLevel, int monsterLevel, int minDamage, int maxDamage)
    {
        // Arrange
        var hero = _fixture.Build<Character>()
            .With(x => x.Level, heroLevel)
            .With(x => x.Attack, heroLevel * 5)
            .Create();
        
        var monster = _fixture.Build<Monster>()
            .With(x => x.Level, monsterLevel)
            .With(x => x.Defense, monsterLevel * 3)
            .Create();

        _mockDiceService.Setup(x => x.RollDice(20))
            .Returns(15);
        _mockMoraleService.Setup(x => x.GetMoraleLevel(hero.Morale))
            .Returns(MoraleLevel.Normal);
        _mockStatusEffectService.Setup(x => x.GetActiveEffectsAsync(EffectTargetKind.Character, hero.Id))
            .ReturnsAsync(new List<StatusEffectState>());

        // Act
        var result = await _sut.AttackAsync(hero.Id, monster.Id);

        // Assert
        result.DamageToMonster.Should().BeInRange(minDamage, maxDamage);
    }

    [Fact]
    public async Task AttackAsync_WhenMonsterDies_ShouldGrantExperience()
    {
        // Arrange
        var hero = _fixture.Create<Character>();
        var monster = _fixture.Build<Monster>()
            .With(x => x.Health, 1)
            .With(x => x.ExperienceReward, 100)
            .Create();

        _mockDiceService.Setup(x => x.RollDice(20))
            .Returns(20); // Critical hit
        _mockMoraleService.Setup(x => x.GetMoraleLevel(hero.Morale))
            .Returns(MoraleLevel.Normal);
        _mockStatusEffectService.Setup(x => x.GetActiveEffectsAsync(EffectTargetKind.Character, hero.Id))
            .ReturnsAsync(new List<StatusEffectState>());

        // Act
        var result = await _sut.AttackAsync(hero.Id, monster.Id);

        // Assert
        result.CombatEnded.Should().BeTrue();
        result.Victory.Should().BeTrue();
        result.ExperienceGained.Should().Be(monster.ExperienceReward);
    }
}
```

### Testes de Modelos
```csharp
// Tests/Models/CharacterTests.cs
using Xunit;
using FluentAssertions;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Tests.Models;

public class CharacterTests
{
    [Fact]
    public void Character_WhenCreated_ShouldHaveValidProperties()
    {
        // Arrange & Act
        var character = new Character
        {
            Id = 1,
            Name = "Test Hero",
            Level = 5,
            Health = 100,
            MaxHealth = 100,
            Attack = 20,
            Defense = 15,
            Morale = 80,
            Experience = 500
        };

        // Assert
        character.Id.Should().Be(1);
        character.Name.Should().Be("Test Hero");
        character.Level.Should().Be(5);
        character.Health.Should().Be(100);
        character.MaxHealth.Should().Be(100);
        character.Attack.Should().Be(20);
        character.Defense.Should().Be(15);
        character.Morale.Should().Be(80);
        character.Experience.Should().Be(500);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(50, true)]
    [InlineData(100, true)]
    [InlineData(101, false)]
    public void Character_Health_ShouldBeWithinValidRange(int health, bool isValid)
    {
        // Arrange
        var character = new Character { MaxHealth = 100 };

        // Act
        var result = character.Health >= 0 && character.Health <= character.MaxHealth;

        // Assert
        result.Should().Be(isValid);
    }
}
```

## 🔗 Testes de Integração

### Configuração do TestServer
```csharp
// Tests/Integration/IntegrationTestBase.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RpgQuestManager.Api.Data;

namespace RpgQuestManager.Tests.Integration;

public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;

    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remover o DbContext real
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Adicionar DbContext em memória
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });

        Client = Factory.CreateClient();
    }

    protected async Task<ApplicationDbContext> GetDbContextAsync()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}
```

### Exemplo de Teste de Integração
```csharp
// Tests/Integration/CombatIntegrationTests.cs
using System.Net.Http.Json;
using FluentAssertions;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Tests.Integration;

public class CombatIntegrationTests : IntegrationTestBase
{
    public CombatIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task StartCombat_WithValidCharacterAndMonster_ShouldReturnCombatState()
    {
        // Arrange
        var request = new
        {
            characterId = 1,
            monsterId = 1
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/combat/start", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var combatResult = await response.Content.ReadFromJsonAsync<CombatResult>();
        combatResult.Should().NotBeNull();
        combatResult.Hero.Should().NotBeNull();
        combatResult.Monster.Should().NotBeNull();
    }

    [Fact]
    public async Task Attack_WithValidCombat_ShouldReturnUpdatedCombatState()
    {
        // Arrange
        var startRequest = new { characterId = 1, monsterId = 1 };
        await Client.PostAsJsonAsync("/api/combat/start", startRequest);

        var attackRequest = new { characterId = 1, monsterId = 1 };

        // Act
        var response = await Client.PostAsJsonAsync("/api/combat/attack", attackRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var combatResult = await response.Content.ReadFromJsonAsync<CombatResult>();
        combatResult.Should().NotBeNull();
        combatResult.DamageToMonster.Should().BeGreaterThan(0);
    }
}
```

## 🌐 Testes de API

### Configuração do Postman/Newman
```json
// tests/postman/collection.json
{
  "info": {
    "name": "RPG Quest Manager API Tests",
    "description": "Testes automatizados da API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Authentication",
      "item": [
        {
          "name": "Register User",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"username\": \"testuser\",\n  \"email\": \"test@example.com\",\n  \"password\": \"TestPassword123!\"\n}"
            },
            "url": {
              "raw": "{{baseUrl}}/api/auth/register",
              "host": ["{{baseUrl}}"],
              "path": ["api", "auth", "register"]
            }
          },
          "event": [
            {
              "listen": "test",
              "script": {
                "exec": [
                  "pm.test('Status code is 200', function () {",
                  "    pm.response.to.have.status(200);",
                  "});",
                  "",
                  "pm.test('Response has token', function () {",
                  "    var jsonData = pm.response.json();",
                  "    pm.expect(jsonData.token).to.exist;",
                  "});"
                ]
              }
            }
          ]
        }
      ]
    }
  ]
}
```

### Testes com RestSharp
```csharp
// Tests/API/AuthApiTests.cs
using RestSharp;
using FluentAssertions;
using Newtonsoft.Json;

namespace RpgQuestManager.Tests.API;

public class AuthApiTests
{
    private readonly RestClient _client;
    private readonly string _baseUrl = "https://localhost:7001";

    public AuthApiTests()
    {
        _client = new RestClient(_baseUrl);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnToken()
    {
        // Arrange
        var request = new RestRequest("/api/auth/register", Method.Post);
        request.AddJsonBody(new
        {
            username = "testuser",
            email = "test@example.com",
            password = "TestPassword123!"
        });

        // Act
        var response = await _client.ExecuteAsync(request);

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var content = JsonConvert.DeserializeObject<dynamic>(response.Content);
        ((string)content.token).Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var request = new RestRequest("/api/auth/login", Method.Post);
        request.AddJsonBody(new
        {
            username = "testuser",
            password = "TestPassword123!"
        });

        // Act
        var response = await _client.ExecuteAsync(request);

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var content = JsonConvert.DeserializeObject<dynamic>(response.Content);
        ((string)content.token).Should().NotBeNullOrEmpty();
    }
}
```

## 🎭 Testes E2E

### Configuração do Playwright
```typescript
// tests/e2e/playwright.config.ts
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  use: {
    baseURL: 'http://localhost:3000',
    trace: 'on-first-retry',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },
  ],
  webServer: {
    command: 'npm run start',
    url: 'http://localhost:3000',
    reuseExistingServer: !process.env.CI,
  },
});
```

### Exemplo de Teste E2E
```typescript
// tests/e2e/auth.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Authentication', () => {
  test('should register new user', async ({ page }) => {
    await page.goto('/register');
    
    await page.fill('[data-testid="username"]', 'testuser');
    await page.fill('[data-testid="email"]', 'test@example.com');
    await page.fill('[data-testid="password"]', 'TestPassword123!');
    await page.fill('[data-testid="confirmPassword"]', 'TestPassword123!');
    
    await page.click('[data-testid="register-button"]');
    
    await expect(page).toHaveURL('/dashboard');
    await expect(page.locator('[data-testid="welcome-message"]')).toContainText('Bem-vindo, testuser!');
  });

  test('should login existing user', async ({ page }) => {
    await page.goto('/login');
    
    await page.fill('[data-testid="username"]', 'testuser');
    await page.fill('[data-testid="password"]', 'TestPassword123!');
    
    await page.click('[data-testid="login-button"]');
    
    await expect(page).toHaveURL('/dashboard');
    await expect(page.locator('[data-testid="welcome-message"]')).toContainText('Bem-vindo, testuser!');
  });
});
```

### Teste de Fluxo Completo
```typescript
// tests/e2e/quest-flow.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Quest Flow', () => {
  test('should complete quest flow', async ({ page }) => {
    // Login
    await page.goto('/login');
    await page.fill('[data-testid="username"]', 'testuser');
    await page.fill('[data-testid="password"]', 'TestPassword123!');
    await page.click('[data-testid="login-button"]');
    
    // Navigate to quests
    await page.click('[data-testid="quests-nav"]');
    await expect(page).toHaveURL('/quests');
    
    // Start quest
    await page.click('[data-testid="quest-card"]:first-child');
    await page.click('[data-testid="start-quest-button"]');
    
    // Navigate to combat
    await page.click('[data-testid="combat-nav"]');
    await expect(page).toHaveURL('/combat');
    
    // Start combat
    await page.click('[data-testid="monster-card"]:first-child');
    await page.click('[data-testid="start-combat-button"]');
    
    // Attack monster
    await page.click('[data-testid="attack-button"]');
    
    // Check combat result
    await expect(page.locator('[data-testid="combat-log"]')).toContainText('atacou');
    
    // Complete quest
    await page.click('[data-testid="quests-nav"]');
    await page.click('[data-testid="complete-quest-button"]');
    
    // Verify quest completion
    await expect(page.locator('[data-testid="quest-status"]')).toContainText('Completada');
  });
});
```

## ⚡ Testes de Performance

### Configuração do NBomber
```csharp
// Tests/Performance/PerformanceTests.cs
using NBomber;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace RpgQuestManager.Tests.Performance;

public class PerformanceTests
{
    [Fact]
    public void ApiPerformanceTest()
    {
        var scenario = Scenario.Create("api_performance", async context =>
        {
            var response = await context.Client.GetAsync("https://localhost:7001/api/characters");
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromMinutes(1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    [Fact]
    public void DatabasePerformanceTest()
    {
        var scenario = Scenario.Create("database_performance", async context =>
        {
            // Simular operações de banco de dados
            await Task.Delay(10); // Simular query
            return Response.Ok();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromMinutes(2))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
```

### Testes de Carga
```csharp
// Tests/Performance/LoadTests.cs
using NBomber;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace RpgQuestManager.Tests.Performance;

public class LoadTests
{
    [Fact]
    public void HighLoadTest()
    {
        var scenario = Scenario.Create("high_load", async context =>
        {
            var response = await context.Client.GetAsync("https://localhost:7001/api/characters");
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 1000, during: TimeSpan.FromMinutes(5))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    [Fact]
    public void StressTest()
    {
        var scenario = Scenario.Create("stress_test", async context =>
        {
            var response = await context.Client.PostAsJsonAsync("https://localhost:7001/api/combat/start", 
                new { characterId = 1, monsterId = 1 });
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 500, during: TimeSpan.FromMinutes(10))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
```

## 🔒 Testes de Segurança

### Testes de Vulnerabilidades
```csharp
// Tests/Security/SecurityTests.cs
using Xunit;
using FluentAssertions;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace RpgQuestManager.Tests.Security;

public class SecurityTests
{
    private readonly HttpClient _client;

    public SecurityTests()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://localhost:7001");
    }

    [Fact]
    public async Task SqlInjection_ShouldBePrevented()
    {
        // Arrange
        var maliciousInput = "'; DROP TABLE Characters; --";
        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"/api/characters?name={maliciousInput}");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        // Verificar se a tabela ainda existe
        var charactersResponse = await _client.GetAsync("/api/characters");
        charactersResponse.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task XssAttack_ShouldBePrevented()
    {
        // Arrange
        var maliciousScript = "<script>alert('XSS')</script>";
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/characters");
        request.Content = new StringContent(JsonConvert.SerializeObject(new
        {
            name = maliciousScript,
            level = 1
        }), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotContain("<script>");
    }

    [Fact]
    public async Task UnauthorizedAccess_ShouldBeDenied()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/users");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
```

### Testes de Autenticação
```csharp
// Tests/Security/AuthenticationTests.cs
using Xunit;
using FluentAssertions;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace RpgQuestManager.Tests.Security;

public class AuthenticationTests
{
    private readonly HttpClient _client;

    public AuthenticationTests()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://localhost:7001");
    }

    [Fact]
    public async Task InvalidToken_ShouldBeRejected()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/characters");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", "invalid-token");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExpiredToken_ShouldBeRejected()
    {
        // Arrange
        var expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjE1MTYyMzkwMjJ9.4Adcj3UFYzPUVaVF43FmMab6RlaQD8A9V8wFzzht-KQ";
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/characters");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", expiredToken);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
```

## ♿ Testes de Acessibilidade

### Configuração do axe-core
```typescript
// tests/accessibility/accessibility.spec.ts
import { test, expect } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

test.describe('Accessibility', () => {
  test('should not have accessibility violations', async ({ page }) => {
    await page.goto('/dashboard');
    
    const accessibilityScanResults = await new AxeBuilder({ page }).analyze();
    
    expect(accessibilityScanResults.violations).toEqual([]);
  });

  test('should be keyboard navigable', async ({ page }) => {
    await page.goto('/dashboard');
    
    // Test tab navigation
    await page.keyboard.press('Tab');
    await expect(page.locator(':focus')).toBeVisible();
    
    await page.keyboard.press('Tab');
    await expect(page.locator(':focus')).toBeVisible();
  });

  test('should have proper ARIA labels', async ({ page }) => {
    await page.goto('/dashboard');
    
    const buttons = page.locator('button');
    const buttonCount = await buttons.count();
    
    for (let i = 0; i < buttonCount; i++) {
      const button = buttons.nth(i);
      const ariaLabel = await button.getAttribute('aria-label');
      const textContent = await button.textContent();
      
      expect(ariaLabel || textContent).toBeTruthy();
    }
  });
});
```

## 📊 Cobertura de Código

### Configuração do Coverlet
```xml
<!-- RpgQuestManager.Tests.csproj -->
<ItemGroup>
  <PackageReference Include="coverlet.collector" Version="6.0.0" />
  <PackageReference Include="coverlet.msbuild" Version="6.0.0" />
</ItemGroup>

<PropertyGroup>
  <CollectCoverage>true</CollectCoverage>
  <CoverletOutputFormat>cobertura</CoverletOutputFormat>
  <CoverletOutput>./coverage/</CoverletOutput>
  <Exclude>[*.Tests]*</Exclude>
</PropertyGroup>
```

### Relatório de Cobertura
```bash
# Gerar relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório HTML
reportgenerator -reports:"coverage/coverage.cobertura.xml" -targetdir:"coverage/report" -reporttypes:Html
```

### Metas de Cobertura
- **Unit Tests**: 80%+
- **Integration Tests**: 70%+
- **API Tests**: 90%+
- **E2E Tests**: 60%+

## 🚀 CI/CD

### GitHub Actions
```yaml
# .github/workflows/test.yml
name: Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: postgres
          POSTGRES_DB: rpgquestmanager_test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
      env:
        ConnectionStrings__Default: "Host=localhost;Database=rpgquestmanager_test;Username=postgres;Password=postgres"
    
    - name: Upload coverage to Codecov
      uses: codecov/codecov-action@v3
      with:
        file: ./coverage/coverage.cobertura.xml
```

### Testes E2E no CI
```yaml
# .github/workflows/e2e.yml
name: E2E Tests

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  e2e:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '18'
    
    - name: Install dependencies
      run: npm ci
    
    - name: Install Playwright Browsers
      run: npx playwright install --with-deps
    
    - name: Build application
      run: npm run build
    
    - name: Start application
      run: npm start &
      env:
        NODE_ENV: production
    
    - name: Wait for application
      run: npx wait-on http://localhost:3000
    
    - name: Run E2E tests
      run: npx playwright test
    
    - name: Upload test results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: playwright-report
        path: playwright-report/
```

## 🛠️ Ferramentas

### Backend
- **xUnit**: Framework de testes
- **Moq**: Mocking framework
- **FluentAssertions**: Assertions fluentes
- **AutoFixture**: Geração automática de dados
- **TestServer**: Testes de integração
- **NBomber**: Testes de performance
- **Coverlet**: Cobertura de código

### Frontend
- **Jest**: Framework de testes
- **React Testing Library**: Testes de componentes
- **Playwright**: Testes E2E
- **axe-core**: Testes de acessibilidade
- **Cypress**: Testes E2E alternativo

### API
- **Postman/Newman**: Testes de API
- **RestSharp**: Cliente HTTP para testes
- **Swagger/OpenAPI**: Documentação e testes

### Performance
- **NBomber**: Testes de carga
- **Artillery**: Testes de performance
- **JMeter**: Testes de carga

### Segurança
- **OWASP ZAP**: Testes de segurança
- **Burp Suite**: Testes de segurança
- **SonarQube**: Análise de código

## 📋 Checklist de Testes

### Desenvolvimento
- [ ] **Testes unitários**: Cobertura mínima de 80%
- [ ] **Testes de integração**: Cobertura mínima de 70%
- [ ] **Testes de API**: Todos os endpoints testados
- [ ] **Testes E2E**: Fluxos críticos testados
- [ ] **Testes de performance**: Carga e stress testados
- [ ] **Testes de segurança**: Vulnerabilidades testadas
- [ ] **Testes de acessibilidade**: WCAG 2.1 AA
- [ ] **Cobertura de código**: Relatórios gerados

### CI/CD
- [ ] **Testes automáticos**: Executados em cada commit
- [ ] **Testes de regressão**: Executados em cada PR
- [ ] **Testes de performance**: Executados em releases
- [ ] **Testes de segurança**: Executados semanalmente
- [ ] **Relatórios**: Gerados automaticamente
- [ ] **Notificações**: Alertas de falhas
- [ ] **Deploy**: Apenas com testes passando
- [ ] **Rollback**: Automático em caso de falha

---

**Este documento é atualizado regularmente com novas estratégias e ferramentas de teste.**

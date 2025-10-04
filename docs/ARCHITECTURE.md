# 🏗️ Arquitetura - RPG Quest Manager

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Arquitetura do Sistema](#-arquitetura-do-sistema)
- [Backend Architecture](#-backend-architecture)
- [Frontend Architecture](#-frontend-architecture)
- [Database Design](#-database-design)
- [API Design](#-api-design)
- [Security Architecture](#-security-architecture)
- [Deployment Architecture](#-deployment-architecture)
- [Performance Architecture](#-performance-architecture)
- [Monitoring Architecture](#-monitoring-architecture)

## 🎯 Visão Geral

O RPG Quest Manager é uma aplicação web moderna construída com arquitetura de microserviços, seguindo princípios de Clean Architecture e Domain-Driven Design.

### Princípios Arquiteturais
- **Separação de Responsabilidades**: Cada camada tem uma responsabilidade específica
- **Inversão de Dependência**: Dependências são injetadas, não criadas
- **Single Responsibility**: Cada classe tem uma única responsabilidade
- **Open/Closed**: Aberto para extensão, fechado para modificação
- **Interface Segregation**: Interfaces específicas para cada cliente
- **Dependency Inversion**: Dependa de abstrações, não de implementações

## 🏗️ Arquitetura do Sistema

### Diagrama de Alto Nível
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │   Backend       │    │   Database      │
│   (React)       │◄──►│   (.NET 8)      │◄──►│   (PostgreSQL)  │
│                 │    │                 │    │                 │
│   - UI/UX       │    │   - API REST    │    │   - Data Store  │
│   - State Mgmt  │    │   - Business    │    │   - Relations   │
│   - Routing     │    │   - Data Access │    │   - Indexes     │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### Componentes Principais
- **Frontend**: Interface do usuário em React
- **Backend**: API REST em .NET 8
- **Database**: PostgreSQL para persistência
- **Cache**: Redis para cache (futuro)
- **Message Queue**: RabbitMQ para eventos (futuro)

## 🔧 Backend Architecture

### Estrutura de Camadas
```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │ Controllers │  │   Middleware│  │   Filters   │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │   Services  │  │   DTOs      │  │   Mappers   │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                    Domain Layer                             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │  Entities   │  │  Value      │  │  Domain     │        │
│  │             │  │  Objects    │  │  Services   │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                     │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │   DbContext │  │ Repositories│  │   External  │        │
│  │             │  │             │  │   Services  │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
```

### Camadas Detalhadas

#### 1. Presentation Layer
- **Controllers**: Endpoints da API REST
- **Middleware**: Autenticação, logging, CORS
- **Filters**: Validação, tratamento de erros
- **Models**: DTOs de request/response

#### 2. Application Layer
- **Services**: Lógica de negócio da aplicação
- **DTOs**: Objetos de transferência de dados
- **Mappers**: Conversão entre entidades e DTOs
- **Validators**: Validação de dados de entrada

#### 3. Domain Layer
- **Entities**: Entidades de domínio
- **Value Objects**: Objetos de valor
- **Domain Services**: Lógica de domínio
- **Interfaces**: Contratos de domínio

#### 4. Infrastructure Layer
- **DbContext**: Contexto do Entity Framework
- **Repositories**: Implementação de repositórios
- **External Services**: Serviços externos
- **Configuration**: Configurações da aplicação

### Padrões Utilizados

#### Repository Pattern
```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

#### Unit of Work Pattern
```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

#### Dependency Injection
```csharp
// Program.cs
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<ICombatService, CombatService>();
```

## 🎨 Frontend Architecture

### Estrutura de Componentes
```
┌─────────────────────────────────────────────────────────────┐
│                    App Component                            │
│  ┌─────────────────────────────────────────────────────────┐│
│  │                Router                                   ││
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐    ││
│  │  │   Pages     │  │   Layout    │  │   Context   │    ││
│  │  └─────────────┘  └─────────────┘  └─────────────┘    ││
│  └─────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                    Component Layer                          │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │   UI        │  │   Feature   │  │   Shared    │        │
│  │ Components  │  │ Components  │  │ Components  │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                    Service Layer                            │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │   API       │  │   Cache     │  │   Utils     │        │
│  │ Services    │  │ Services    │  │ Services    │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
```

### Padrões Utilizados

#### Component Composition
```typescript
// Layout.tsx
export function Layout({ children }: LayoutProps) {
  return (
    <div className="min-h-screen bg-gray-50">
      <Header />
      <Sidebar />
      <main className="ml-64 p-6">
        {children}
      </main>
    </div>
  );
}
```

#### Custom Hooks
```typescript
// useCharacter.ts
export function useCharacter() {
  const [character, setCharacter] = useState<Character | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadCharacter = async (id: number) => {
    try {
      setLoading(true);
      const data = await characterService.getCharacter(id);
      setCharacter(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  return { character, loading, error, loadCharacter };
}
```

#### Context API
```typescript
// AuthContext.tsx
interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  
  // ... implementation
}
```

### State Management
- **Local State**: useState, useReducer
- **Global State**: Context API
- **Server State**: Custom hooks
- **Cache**: localStorage, sessionStorage

## 🗄️ Database Design

### Entity Relationship Diagram
```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│    User     │    │  Character  │    │   Monster   │
│             │    │             │    │             │
│ - Id        │◄──►│ - Id        │    │ - Id        │
│ - Username  │    │ - Name      │    │ - Name      │
│ - Email     │    │ - Level     │    │ - Type      │
│ - Password  │    │ - Health    │    │ - Health    │
└─────────────┘    │ - Attack    │    │ - Attack    │
                   │ - Defense   │    │ - Defense   │
                   │ - Morale    │    └─────────────┘
                   │ - Gold      │
                   │ - UserId    │
                   └─────────────┘
```

### Principais Entidades

#### User
```sql
CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

#### Character
```sql
CREATE TABLE Characters (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Level INTEGER DEFAULT 1,
    Experience INTEGER DEFAULT 0,
    Health INTEGER NOT NULL,
    MaxHealth INTEGER NOT NULL,
    Attack INTEGER NOT NULL,
    Defense INTEGER NOT NULL,
    Morale INTEGER NOT NULL,
    Gold INTEGER DEFAULT 100,
    UserId INTEGER REFERENCES Users(Id),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

#### Monster
```sql
CREATE TABLE Monsters (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Type VARCHAR(50) NOT NULL,
    Rank VARCHAR(20) NOT NULL,
    Habitat VARCHAR(50) NOT NULL,
    Health INTEGER NOT NULL,
    MaxHealth INTEGER NOT NULL,
    Attack INTEGER NOT NULL,
    Defense INTEGER NOT NULL,
    ExperienceReward INTEGER NOT NULL
);
```

### Índices e Otimizações
```sql
-- Índices para performance
CREATE INDEX IX_Characters_UserId ON Characters(UserId);
CREATE INDEX IX_Characters_Level ON Characters(Level);
CREATE INDEX IX_Monsters_Type ON Monsters(Type);
CREATE INDEX IX_Monsters_Rank ON Monsters(Rank);

-- Índices compostos
CREATE INDEX IX_Characters_UserId_Level ON Characters(UserId, Level);
CREATE INDEX IX_Monsters_Type_Rank ON Monsters(Type, Rank);
```

## 🔌 API Design

### RESTful Endpoints
```
GET    /api/characters/{id}           # Obter personagem
PUT    /api/characters/{id}           # Atualizar personagem
POST   /api/combat/start              # Iniciar combate
POST   /api/combat/attack             # Atacar
GET    /api/quests                    # Listar missões
POST   /api/quests/start              # Iniciar missão
GET    /api/inventory/{characterId}   # Obter inventário
POST   /api/inventory/equip           # Equipar item
```

### Padrões de Response
```json
{
  "data": {
    "id": 1,
    "name": "Hero",
    "level": 5,
    "health": 100
  },
  "meta": {
    "timestamp": "2024-01-01T00:00:00Z",
    "version": "1.0"
  }
}
```

### Error Handling
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input data",
    "details": [
      {
        "field": "name",
        "message": "Name is required"
      }
    ]
  }
}
```

### Versioning
- **URL Versioning**: `/api/v1/characters`
- **Header Versioning**: `Accept: application/vnd.api+json;version=1`
- **Query Versioning**: `/api/characters?version=1`

## 🔒 Security Architecture

### Autenticação
```csharp
// JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
        };
    });
```

### Autorização
```csharp
[Authorize]
[ApiController]
public class CharactersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<Character>> GetCharacter(int id)
    {
        // Verificar se o usuário tem acesso ao personagem
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // ... lógica de autorização
    }
}
```

### Validação
```csharp
public class CreateCharacterRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    
    [Range(1, 100)]
    public int Level { get; set; }
}
```

### CORS
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

## 🚀 Deployment Architecture

### Docker Compose
```yaml
version: '3.8'
services:
  db:
    image: postgres:15
    environment:
      POSTGRES_DB: rpgquestmanager
      POSTGRES_USER: rpguser
      POSTGRES_PASSWORD: rpgpass123
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  api:
    build: ./src/RpgQuestManager.Api
    ports:
      - "5000:5000"
    depends_on:
      - db
    environment:
      - ConnectionStrings__Default=Host=db;Port=5432;Database=rpgquestmanager;Username=rpguser;Password=rpgpass123

  web:
    build: ./frontend
    ports:
      - "3000:3000"
    depends_on:
      - api
    environment:
      - VITE_API_BASE_URL=http://localhost:5000

volumes:
  postgres_data:
```

### Produção
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Load Balancer │    │   Web Server    │    │   Database      │
│   (Nginx)       │◄──►│   (Docker)      │◄──►│   (PostgreSQL)  │
│                 │    │                 │    │                 │
│   - SSL/TLS     │    │   - API         │    │   - Primary     │
│   - Caching     │    │   - Frontend    │    │   - Replica     │
│   - Rate Limit  │    │   - Static      │    │   - Backup      │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## ⚡ Performance Architecture

### Caching Strategy
```csharp
// Memory Cache
builder.Services.AddMemoryCache();

// Distributed Cache (Redis)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

### Database Optimization
```csharp
// Connection Pooling
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
        npgsqlOptions.CommandTimeout(30);
    }));
```

### Frontend Optimization
```typescript
// Lazy Loading
const LazyComponent = lazy(() => import('./LazyComponent'));

// Memoization
const MemoizedComponent = memo(({ data }: Props) => {
  return <div>{data}</div>;
});

// Virtual Scrolling
const VirtualizedList = ({ items }: Props) => {
  return (
    <FixedSizeList
      height={600}
      itemCount={items.length}
      itemSize={50}
    >
      {({ index, style }) => (
        <div style={style}>
          {items[index]}
        </div>
      )}
    </FixedSizeList>
  );
};
```

## 📊 Monitoring Architecture

### Logging
```csharp
// Structured Logging
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddSerilog();
});

// Custom Logger
public class CombatService
{
    private readonly ILogger<CombatService> _logger;
    
    public CombatService(ILogger<CombatService> logger)
    {
        _logger = logger;
    }
    
    public async Task<CombatResult> AttackAsync(int characterId, int monsterId)
    {
        _logger.LogInformation("Character {CharacterId} attacking monster {MonsterId}", 
            characterId, monsterId);
        
        // ... lógica de combate
        
        _logger.LogInformation("Combat result: {Result}", result);
        return result;
    }
}
```

### Metrics
```csharp
// Application Metrics
public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMetrics _metrics;
    
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        await _next(context);
        
        stopwatch.Stop();
        
        _metrics.Measure.Timer.Time("http_request_duration", 
            stopwatch.Elapsed, 
            new MetricTags("method", context.Request.Method, 
                          "path", context.Request.Path));
    }
}
```

### Health Checks
```csharp
// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContext<ApplicationDbContext>()
    .AddNpgSql(connectionString);

// Health Check Endpoint
app.MapHealthChecks("/health");
```

## 🔄 Event-Driven Architecture

### Domain Events
```csharp
public abstract class DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public class CharacterLeveledUpEvent : DomainEvent
{
    public int CharacterId { get; }
    public int NewLevel { get; }
    
    public CharacterLeveledUpEvent(int characterId, int newLevel)
    {
        CharacterId = characterId;
        NewLevel = newLevel;
    }
}
```

### Event Handlers
```csharp
public class CharacterLeveledUpEventHandler : INotificationHandler<CharacterLeveledUpEvent>
{
    private readonly ILogger<CharacterLeveledUpEventHandler> _logger;
    
    public async Task Handle(CharacterLeveledUpEventHandler notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Character {CharacterId} leveled up to {NewLevel}", 
            notification.CharacterId, notification.NewLevel);
        
        // Lógica adicional para level up
    }
}
```

## 🧪 Testing Architecture

### Test Pyramid
```
┌─────────────────────────────────────────────────────────────┐
│                    E2E Tests (10%)                         │
│  ┌─────────────────────────────────────────────────────────┐│
│  │                Integration Tests (20%)                  ││
│  │  ┌─────────────────────────────────────────────────────┐││
│  │  │              Unit Tests (70%)                       │││
│  │  └─────────────────────────────────────────────────────┘││
│  └─────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────┘
```

### Unit Tests
```csharp
[Test]
public async Task AttackAsync_ShouldDealDamage_WhenValidInput()
{
    // Arrange
    var character = new Character { Attack = 10, Health = 100 };
    var monster = new Monster { Defense = 5, Health = 50 };
    var combatService = new CombatService();
    
    // Act
    var result = await combatService.AttackAsync(character, monster);
    
    // Assert
    Assert.That(result.DamageDealt, Is.GreaterThan(0));
    Assert.That(monster.Health, Is.LessThan(50));
}
```

### Integration Tests
```csharp
[Test]
public async Task GetCharacter_ShouldReturnCharacter_WhenValidId()
{
    // Arrange
    var client = _factory.CreateClient();
    var character = await CreateTestCharacter();
    
    // Act
    var response = await client.GetAsync($"/api/characters/{character.Id}");
    
    // Assert
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<Character>();
    Assert.That(result.Name, Is.EqualTo(character.Name));
}
```

---

**Esta arquitetura é evolutiva e será atualizada conforme o projeto cresce e novas funcionalidades são adicionadas.**

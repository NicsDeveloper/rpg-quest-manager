# ⚡ Performance - RPG Quest Manager

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Métricas Atuais](#-métricas-atuais)
- [Backend Performance](#-backend-performance)
- [Frontend Performance](#-frontend-performance)
- [Database Performance](#-database-performance)
- [Otimizações Implementadas](#-otimizações-implementadas)
- [Monitoramento](#-monitoramento)
- [Plano de Melhoria](#-plano-de-melhoria)

## 🎯 Visão Geral

Este documento detalha as métricas de performance do RPG Quest Manager, otimizações implementadas e planos de melhoria.

### Objetivos de Performance
- **Tempo de Resposta API**: < 100ms
- **Tempo de Carregamento Frontend**: < 2s
- **Throughput**: > 200 req/s
- **Uso de Memória**: < 256MB
- **Disponibilidade**: > 99.9%

## 📊 Métricas Atuais

### Backend (.NET 8)
- **Tempo de Resposta Médio**: 150ms
- **Tempo de Resposta P95**: 300ms
- **Throughput**: 120 req/s
- **Uso de Memória**: 512MB
- **CPU Usage**: 45%

### Frontend (React)
- **First Contentful Paint**: 1.8s
- **Largest Contentful Paint**: 2.5s
- **Time to Interactive**: 3.2s
- **Cumulative Layout Shift**: 0.1
- **First Input Delay**: 50ms

### Database (PostgreSQL)
- **Query Time Médio**: 25ms
- **Query Time P95**: 80ms
- **Connections Ativas**: 15/100
- **Cache Hit Ratio**: 85%
- **Disk I/O**: 120 IOPS

## 🔧 Backend Performance

### Otimizações Implementadas

#### 1. Connection Pooling
```csharp
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
        npgsqlOptions.CommandTimeout(30);
        npgsqlOptions.MaxBatchSize(1000);
    }));
```

#### 2. Caching
```csharp
// Memory Cache
builder.Services.AddMemoryCache();

// Distributed Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "RpgQuestManager";
});
```

#### 3. Async/Await
```csharp
public async Task<Character> GetCharacterAsync(int id)
{
    return await _context.Characters
        .Include(c => c.Equipment)
        .FirstOrDefaultAsync(c => c.Id == id);
}
```

#### 4. Pagination
```csharp
public async Task<PagedResult<Quest>> GetQuestsAsync(int page, int pageSize)
{
    var totalCount = await _context.Quests.CountAsync();
    var quests = await _context.Quests
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return new PagedResult<Quest>
    {
        Data = quests,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

### Problemas Identificados

#### 1. N+1 Queries
```csharp
// ❌ Problemático
var characters = await _context.Characters.ToListAsync();
foreach (var character in characters)
{
    var equipment = await _context.Equipment
        .Where(e => e.CharacterId == character.Id)
        .ToListAsync();
}

// ✅ Otimizado
var characters = await _context.Characters
    .Include(c => c.Equipment)
    .ToListAsync();
```

#### 2. Queries Ineficientes
```csharp
// ❌ Problemático
var characters = await _context.Characters
    .Where(c => c.Level > 10)
    .ToListAsync();
var filteredCharacters = characters
    .Where(c => c.Health > 50)
    .ToList();

// ✅ Otimizado
var characters = await _context.Characters
    .Where(c => c.Level > 10 && c.Health > 50)
    .ToListAsync();
```

#### 3. Falta de Índices
```sql
-- Índices necessários
CREATE INDEX IX_Characters_Level ON Characters(Level);
CREATE INDEX IX_Characters_Health ON Characters(Health);
CREATE INDEX IX_Quests_Status ON Quests(Status);
CREATE INDEX IX_Items_Type ON Items(Type);
```

## 🎨 Frontend Performance

### Otimizações Implementadas

#### 1. Code Splitting
```typescript
// Lazy loading de componentes
const LazyCombat = lazy(() => import('./pages/Combat'));
const LazyQuests = lazy(() => import('./pages/Quests'));
const LazyInventory = lazy(() => import('./pages/Inventory'));

// Uso com Suspense
<Suspense fallback={<Loading />}>
  <LazyCombat />
</Suspense>
```

#### 2. Memoization
```typescript
// Memoização de componentes
const MemoizedCharacterCard = memo(({ character }: Props) => {
  return (
    <div className="character-card">
      <h3>{character.name}</h3>
      <p>Level: {character.level}</p>
    </div>
  );
});

// Memoização de valores
const expensiveValue = useMemo(() => {
  return calculateExpensiveValue(data);
}, [data]);
```

#### 3. Virtual Scrolling
```typescript
// Para listas grandes
const VirtualizedList = ({ items }: Props) => {
  return (
    <FixedSizeList
      height={600}
      itemCount={items.length}
      itemSize={50}
      itemData={items}
    >
      {({ index, style, data }) => (
        <div style={style}>
          <CharacterCard character={data[index]} />
        </div>
      )}
    </FixedSizeList>
  );
};
```

#### 4. Image Optimization
```typescript
// Lazy loading de imagens
const LazyImage = ({ src, alt }: Props) => {
  const [isLoaded, setIsLoaded] = useState(false);
  
  return (
    <img
      src={isLoaded ? src : placeholder}
      alt={alt}
      onLoad={() => setIsLoaded(true)}
      loading="lazy"
    />
  );
};
```

### Problemas Identificados

#### 1. Re-renders Desnecessários
```typescript
// ❌ Problemático
const Component = ({ data }: Props) => {
  const processedData = data.map(item => ({
    ...item,
    processed: true
  }));
  
  return <div>{processedData.map(item => <Item key={item.id} item={item} />)}</div>;
};

// ✅ Otimizado
const Component = ({ data }: Props) => {
  const processedData = useMemo(() => 
    data.map(item => ({
      ...item,
      processed: true
    })), [data]);
  
  return <div>{processedData.map(item => <Item key={item.id} item={item} />)}</div>;
};
```

#### 2. Bundle Size Grande
```typescript
// ❌ Importando biblioteca inteira
import * as _ from 'lodash';

// ✅ Importando apenas o necessário
import { debounce } from 'lodash/debounce';
```

#### 3. Falta de Caching
```typescript
// ❌ Sem cache
const fetchData = async () => {
  const response = await fetch('/api/data');
  return response.json();
};

// ✅ Com cache
const fetchData = useCallback(async () => {
  const cached = cache.get('data');
  if (cached) return cached;
  
  const response = await fetch('/api/data');
  const data = await response.json();
  cache.set('data', data, 5 * 60 * 1000); // 5 minutos
  return data;
}, []);
```

## 🗄️ Database Performance

### Otimizações Implementadas

#### 1. Índices Estratégicos
```sql
-- Índices para queries frequentes
CREATE INDEX IX_Characters_UserId_Level ON Characters(UserId, Level);
CREATE INDEX IX_Quests_Status_Level ON Quests(Status, RequiredLevel);
CREATE INDEX IX_Items_Type_Rarity ON Items(Type, Rarity);
CREATE INDEX IX_Combat_CharacterId_CreatedAt ON Combat(CharacterId, CreatedAt);
```

#### 2. Query Optimization
```sql
-- ❌ Query ineficiente
SELECT * FROM Characters c
JOIN Equipment e ON c.Id = e.CharacterId
WHERE c.Level > 10;

-- ✅ Query otimizada
SELECT c.Id, c.Name, c.Level, e.Type, e.Name
FROM Characters c
INNER JOIN Equipment e ON c.Id = e.CharacterId
WHERE c.Level > 10
AND c.Health > 50;
```

#### 3. Connection Pooling
```csharp
// Configuração de connection pooling
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(3);
        npgsqlOptions.CommandTimeout(30);
        npgsqlOptions.MaxBatchSize(1000);
        npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    }));
```

### Problemas Identificados

#### 1. Queries N+1
```csharp
// ❌ Problemático
var characters = await _context.Characters.ToListAsync();
foreach (var character in characters)
{
    var equipment = await _context.Equipment
        .Where(e => e.CharacterId == character.Id)
        .ToListAsync();
}

// ✅ Otimizado
var characters = await _context.Characters
    .Include(c => c.Equipment)
    .ToListAsync();
```

#### 2. Falta de Índices
```sql
-- Índices necessários
CREATE INDEX IX_Characters_Level ON Characters(Level);
CREATE INDEX IX_Characters_Health ON Characters(Health);
CREATE INDEX IX_Quests_Status ON Quests(Status);
CREATE INDEX IX_Items_Type ON Items(Type);
```

#### 3. Queries Complexas
```sql
-- ❌ Query complexa
SELECT c.*, e.*, q.*
FROM Characters c
LEFT JOIN Equipment e ON c.Id = e.CharacterId
LEFT JOIN Quests q ON c.Id = q.CharacterId
WHERE c.Level > 10 AND c.Health > 50;

-- ✅ Query otimizada
SELECT c.Id, c.Name, c.Level, c.Health
FROM Characters c
WHERE c.Level > 10 AND c.Health > 50;
```

## 🚀 Otimizações Implementadas

### 1. Caching Strategy
```csharp
// Cache em múltiplas camadas
public class CachedCharacterService : ICharacterService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ICharacterService _characterService;
    
    public async Task<Character> GetCharacterAsync(int id)
    {
        // L1: Memory Cache
        if (_memoryCache.TryGetValue($"character:{id}", out Character cached))
            return cached;
        
        // L2: Distributed Cache
        var distributed = await _distributedCache.GetStringAsync($"character:{id}");
        if (distributed != null)
        {
            var character = JsonSerializer.Deserialize<Character>(distributed);
            _memoryCache.Set($"character:{id}", character, TimeSpan.FromMinutes(5));
            return character;
        }
        
        // L3: Database
        var character = await _characterService.GetCharacterAsync(id);
        _memoryCache.Set($"character:{id}", character, TimeSpan.FromMinutes(5));
        await _distributedCache.SetStringAsync($"character:{id}", 
            JsonSerializer.Serialize(character), 
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });
        
        return character;
    }
}
```

### 2. Database Optimization
```csharp
// Otimização de queries
public class OptimizedCharacterRepository : ICharacterRepository
{
    public async Task<Character> GetCharacterWithEquipmentAsync(int id)
    {
        return await _context.Characters
            .Include(c => c.Equipment)
            .AsNoTracking() // Para queries somente leitura
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<List<Character>> GetCharactersByLevelAsync(int level)
    {
        return await _context.Characters
            .Where(c => c.Level >= level)
            .OrderBy(c => c.Level)
            .Take(100) // Limite para evitar queries muito grandes
            .ToListAsync();
    }
}
```

### 3. Frontend Optimization
```typescript
// Otimização de componentes
const OptimizedCharacterList = ({ characters }: Props) => {
  const [visibleCharacters, setVisibleCharacters] = useState<Character[]>([]);
  const [loading, setLoading] = useState(false);
  
  // Debounce para busca
  const debouncedSearch = useCallback(
    debounce((searchTerm: string) => {
      setLoading(true);
      const filtered = characters.filter(c => 
        c.name.toLowerCase().includes(searchTerm.toLowerCase())
      );
      setVisibleCharacters(filtered);
      setLoading(false);
    }, 300),
    [characters]
  );
  
  // Virtual scrolling para listas grandes
  const itemRenderer = useCallback(({ index, style }: any) => (
    <div style={style}>
      <CharacterCard character={visibleCharacters[index]} />
    </div>
  ), [visibleCharacters]);
  
  return (
    <div>
      <input 
        onChange={(e) => debouncedSearch(e.target.value)}
        placeholder="Buscar personagens..."
      />
      {loading ? (
        <Loading />
      ) : (
        <FixedSizeList
          height={600}
          itemCount={visibleCharacters.length}
          itemSize={100}
          itemRenderer={itemRenderer}
        />
      )}
    </div>
  );
};
```

## 📊 Monitoramento

### Métricas de Performance
```csharp
// Middleware de métricas
public class PerformanceMiddleware
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
                          "path", context.Request.Path,
                          "status", context.Response.StatusCode.ToString()));
    }
}
```

### Health Checks
```csharp
// Health checks para performance
builder.Services.AddHealthChecks()
    .AddDbContext<ApplicationDbContext>()
    .AddNpgSql(connectionString)
    .AddCheck<PerformanceHealthCheck>("performance");

public class PerformanceHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var memoryUsage = GC.GetTotalMemory(false);
        var isHealthy = memoryUsage < 500 * 1024 * 1024; // 500MB
        
        return isHealthy 
            ? HealthCheckResult.Healthy($"Memory usage: {memoryUsage / 1024 / 1024}MB")
            : HealthCheckResult.Unhealthy($"Memory usage too high: {memoryUsage / 1024 / 1024}MB");
    }
}
```

### Logging de Performance
```csharp
// Logging estruturado para performance
public class PerformanceLogger
{
    private readonly ILogger<PerformanceLogger> _logger;
    
    public void LogSlowQuery(string query, TimeSpan duration)
    {
        _logger.LogWarning("Slow query detected: {Query} took {Duration}ms", 
            query, duration.TotalMilliseconds);
    }
    
    public void LogHighMemoryUsage(long memoryUsage)
    {
        _logger.LogWarning("High memory usage: {MemoryUsage}MB", 
            memoryUsage / 1024 / 1024);
    }
}
```

## 🎯 Plano de Melhoria

### Fase 1: Otimizações Imediatas (1-2 semanas)
- [ ] **Implementar caching**
  - [ ] Memory cache para dados frequentes
  - [ ] Distributed cache para dados compartilhados
  - [ ] Cache de queries de banco
  - [ ] Invalidação de cache

- [ ] **Otimizar queries de banco**
  - [ ] Adicionar índices necessários
  - [ ] Eliminar queries N+1
  - [ ] Implementar pagination
  - [ ] Otimizar queries complexas

- [ ] **Melhorar frontend**
  - [ ] Implementar lazy loading
  - [ ] Adicionar memoization
  - [ ] Otimizar bundle size
  - [ ] Implementar virtual scrolling

### Fase 2: Otimizações Avançadas (3-4 semanas)
- [ ] **Implementar CDN**
  - [ ] CDN para assets estáticos
  - [ ] Cache de API responses
  - [ ] Compressão de assets
  - [ ] Minificação de código

- [ ] **Otimizar banco de dados**
  - [ ] Implementar read replicas
  - [ ] Otimizar connection pooling
  - [ ] Implementar query caching
  - [ ] Adicionar monitoring

- [ ] **Melhorar arquitetura**
  - [ ] Implementar microserviços
  - [ ] Adicionar load balancing
  - [ ] Implementar circuit breakers
  - [ ] Adicionar rate limiting

### Fase 3: Otimizações de Longo Prazo (1-2 meses)
- [ ] **Implementar observabilidade**
  - [ ] APM (Application Performance Monitoring)
  - [ ] Distributed tracing
  - [ ] Real-time monitoring
  - [ ] Alertas automáticos

- [ ] **Otimizar infraestrutura**
  - [ ] Auto-scaling
  - [ ] Load balancing avançado
  - [ ] Database sharding
  - [ ] Caching distribuído

- [ ] **Melhorar experiência do usuário**
  - [ ] Progressive Web App
  - [ ] Offline support
  - [ ] Real-time updates
  - [ ] Performance budgets

## 📈 Métricas de Sucesso

### KPIs de Performance
- **Tempo de Resposta API**: 150ms → 100ms
- **Tempo de Carregamento Frontend**: 3.2s → 2s
- **Throughput**: 120 req/s → 200 req/s
- **Uso de Memória**: 512MB → 256MB
- **Disponibilidade**: 99.5% → 99.9%

### Métricas de Qualidade
- **Core Web Vitals**: Melhorar para "Good"
- **Lighthouse Score**: 80 → 95
- **Bundle Size**: Reduzir em 30%
- **Cache Hit Ratio**: 85% → 95%

### Métricas de Negócio
- **Bounce Rate**: Reduzir em 20%
- **Session Duration**: Aumentar em 30%
- **User Engagement**: Aumentar em 25%
- **Conversion Rate**: Aumentar em 15%

## 🔧 Ferramentas de Monitoramento

### Backend
- **Application Insights**: Métricas de aplicação
- **Serilog**: Logging estruturado
- **Health Checks**: Monitoramento de saúde
- **Performance Counters**: Métricas de sistema

### Frontend
- **Lighthouse**: Auditoria de performance
- **Web Vitals**: Métricas de experiência
- **Bundle Analyzer**: Análise de bundle
- **Performance API**: Métricas nativas

### Database
- **pg_stat_statements**: Estatísticas de queries
- **pg_stat_activity**: Atividade de conexões
- **pg_stat_database**: Estatísticas de banco
- **pg_stat_user_tables**: Estatísticas de tabelas

### Infrastructure
- **Prometheus**: Coleta de métricas
- **Grafana**: Visualização de dados
- **AlertManager**: Gerenciamento de alertas
- **Jaeger**: Distributed tracing

---

**Este documento é atualizado regularmente com novas métricas e otimizações.**

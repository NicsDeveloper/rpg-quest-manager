using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Middleware;
using RpgQuestManager.Api.Services;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Redis
var redisConnection = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection!));
builder.Services.AddScoped<ICacheService, CacheService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

builder.Services.AddAuthorization();

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IQuestService, QuestService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDiceService, DiceService>();
builder.Services.AddScoped<ICombatService, CombatService>();
builder.Services.AddScoped<IDropService, DropService>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<IFreeDiceService, FreeDiceService>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "🐉 RPG Quest Manager API", 
        Version = "v1.0",
        Description = @"
# API para Gerenciamento de RPG - O Livro de Eldoria

Esta API permite gerenciar um sistema completo de RPG, incluindo:

## ⚔️ Funcionalidades Principais
* **Heróis**: Crie e gerencie heróis com classes, atributos e progressão de nível
* **Quests**: Crie missões com diferentes dificuldades e recompensas
* **Inimigos**: Cadastre adversários com poder e vida
* **Recompensas**: Configure prêmios em ouro, XP e itens
* **Inventário**: Sistema completo de itens equipáveis
* **Sistema de Progressão**: Level up automático baseado em XP

## 🎯 Recursos Especiais
* ✅ Autenticação JWT
* ✅ Cache com Redis (heróis mais fortes, quests mais jogadas)
* ✅ Eventos assíncronos com RabbitMQ
* ✅ Validações com FluentValidation
* ✅ Logs estruturados com Serilog

## 🚀 Como Usar
1. Registre-se em `/api/v1/auth/register`
2. Faça login em `/api/v1/auth/login` e obtenha o token JWT
3. Use o token no botão 'Authorize' acima
4. Explore os endpoints disponíveis!

---
**Desenvolvido com ⚔️ por Eldoria Dev Team**
",
        Contact = new OpenApiContact
        {
            Name = "Eldoria Dev Team",
            Email = "dev@eldoria.com",
            Url = new Uri("https://github.com/seu-usuario/rpg-quest-manager")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    
    // Incluir comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    
    // Configuração JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando o esquema Bearer.
                      
Entre com 'Bearer' [espaço] e então seu token.
                      
Exemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
    // Tags organizadas
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    c.DocInclusionPredicate((name, api) => true);
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString!)
    .AddRedis(redisConnection!);

var app = builder.Build();

// Middleware de erro global
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RPG Quest Manager API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// Aplicar migrations e seed automaticamente em desenvolvimento
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.Migrate();
        Log.Information("Database migrations aplicadas com sucesso");
        
        // Executar seed
        var seeder = new DatabaseSeeder(
            dbContext, 
            scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>()
        );
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Erro ao aplicar migrations ou seed do banco de dados");
    }
}

Log.Information("🐉 RPG Quest Manager API iniciada!");

app.Run();

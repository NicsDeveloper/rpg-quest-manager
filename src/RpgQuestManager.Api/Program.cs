using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configuração JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "RpgQuestManager",
            ValidAudience = "RpgQuestManager",
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "default-key-that-should-be-changed-in-production"))
        };
    });

builder.Services.AddAuthorization();

var conn = builder.Configuration.GetConnectionString("Default")
           ?? "Host=localhost;Port=5432;Database=rpgquestmanager;Username=rpguser;Password=rpgpass123";

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(conn));

// Serviços de combate
builder.Services.AddScoped<DiceService>();
builder.Services.AddScoped<MoraleService>();
builder.Services.AddScoped<StatusEffectService>();
builder.Services.AddScoped<LevelUpService>();
builder.Services.AddScoped<MonsterService>();
builder.Services.AddScoped<QuestService>();
builder.Services.AddScoped<ICombatService, CombatService>();

// Serviços de sistema
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<ShopService>();
builder.Services.AddScoped<DropService>();
builder.Services.AddScoped<DiceInventoryService>();
builder.Services.AddScoped<RewardService>();

// Serviços de conquistas e grupos
builder.Services.AddScoped<IAchievementService, AchievementService>();
builder.Services.AddScoped<IPartyService, PartyService>();
builder.Services.AddScoped<ISpecialAbilityService, SpecialAbilityService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usar CORS antes de Authentication
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    var maxRetries = 10;
    var delay = TimeSpan.FromSeconds(5);
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            logger.LogInformation("Tentando conectar ao banco de dados... Tentativa {Attempt}/{MaxRetries}", i + 1, maxRetries);
            db.Database.Migrate();
            DbSeeder.Seed(db);
            logger.LogInformation("Banco de dados inicializado com sucesso!");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Falha na conexão com banco de dados (tentativa {Attempt}/{MaxRetries}): {Error}", i + 1, maxRetries, ex.Message);
            
            if (i == maxRetries - 1)
            {
                logger.LogError("Não foi possível conectar ao banco de dados após {MaxRetries} tentativas", maxRetries);
                throw;
            }
            
            await Task.Delay(delay);
        }
    }
}

app.MapGet("/health", () => Results.Ok("Healthy"));
app.MapControllers();

app.Run();

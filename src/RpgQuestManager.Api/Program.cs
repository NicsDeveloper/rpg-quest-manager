using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

app.MapGet("/health", () => Results.Ok("Healthy"));
app.MapControllers();

app.Run();

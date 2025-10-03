using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var conn = builder.Configuration.GetConnectionString("Default")
           ?? "Host=localhost;Port=5432;Database=rpgquestmanager;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(conn));
builder.Services.AddScoped<ICombatService, CombatService>();
builder.Services.AddScoped<IQuestService, QuestService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

app.MapGet("/health", () => Results.Ok("Healthy"));
app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RpgQuestManager.Api.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Connection string temporária para design time (migrations)
        // Em produção, virá do appsettings.json
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=rpgquestmanager;Username=postgres;Password=postgres123");
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}


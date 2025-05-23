using JWTAuthApi.Users.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthApi.DB;

public static class MigrationExtension
{
    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();

        var context = serviceScope?.ServiceProvider.GetRequiredService<AppDbContext>();
        var hashingService = serviceScope?.ServiceProvider.GetRequiredService<IHashingService>();
        
        context?.Database.Migrate();
        DataSeeder.SeedData(context!, hashingService!);

        return app;
    }
}

using JWTAuthApi.DB;
using JWTAuthApi.Users.Configs;
using JWTAuthApi.Users.Services;
using JWTAuthApi.Users.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthApi;

public static class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JWTConfig>(configuration.GetSection("JWTSettings"));

        var dbConnectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<AppDbContext>(option => 
            option.UseSqlite(dbConnectionString));

        services.AddScoped<IHashingService, HashingService>();
        services.AddScoped<ITokenService, TokenService>();
        
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}

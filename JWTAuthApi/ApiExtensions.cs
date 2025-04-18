using JWTAuthApi.Users.Configs;
using JWTAuthApi.Users.Services;
using JWTAuthApi.Users.Services.Interfaces;

namespace JWTAuthApi;

public static class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JWTConfig>(configuration.GetSection("JWTSettings"));

        services.AddScoped<IHashingService, HashingService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}

using System.Text;
using JWTAuthApi.DB;
using JWTAuthApi.Users.Configs;
using JWTAuthApi.Users.Services;
using JWTAuthApi.Users.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuthApi;

public static class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JWTConfig>(configuration.GetSection("JWTSettings"));

        var dbConnectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<AppDbContext>(option => 
            option.UseSqlite(dbConnectionString));
        
        var jwtConfig = services.BuildServiceProvider().GetService<IOptions<JWTConfig>>()?.Value;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = jwtConfig?.Audience,
                    ValidIssuer = jwtConfig?.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig?.Key!))
                };
            });

        services.AddScoped<IHashingService, HashingService>();
        services.AddScoped<ITokenService, TokenService>();
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}

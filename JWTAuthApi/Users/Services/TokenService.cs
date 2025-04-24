using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JWTAuthApi.Users.Configs;
using JWTAuthApi.Users.Entities;
using JWTAuthApi.Users.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuthApi.Users.Services;

public class TokenService(IOptions<JWTConfig> jwtOptions) : ITokenService
{
    private readonly JWTConfig _jwtConfig = jwtOptions.Value;
    private const string SecurityAlgorithm = SecurityAlgorithms.HmacSha512;

    public string GenerateAccessToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_jwtConfig.Key);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithm);
        var expiresIn = DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpiresInMin);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name,user.Name),
            new(ClaimTypes.Email,user.Email),
            new("UserName", user.Username),
        };
        
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: expiresIn,
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        return Convert.ToBase64String(randomNumber); 
    }
}

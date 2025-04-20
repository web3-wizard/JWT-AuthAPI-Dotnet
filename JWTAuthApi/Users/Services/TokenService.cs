using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    private readonly string _securityAlgorithm = SecurityAlgorithms.HmacSha512;
 
    public string GenerateToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_jwtConfig.Key);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, _securityAlgorithm);
        var expiresIn = DateTime.UtcNow.AddDays(_jwtConfig.ExpiresInDays);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name,user.Name),
            new(ClaimTypes.Email,user.Email),
            new("UserName", user.Username),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: expiresIn,
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

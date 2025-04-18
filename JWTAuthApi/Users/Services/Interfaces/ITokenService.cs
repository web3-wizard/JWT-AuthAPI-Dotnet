using JWTAuthApi.Users.Entities;

namespace JWTAuthApi.Users.Services.Interfaces;

public interface ITokenService
{
    public string GenerateToken(User user);
}

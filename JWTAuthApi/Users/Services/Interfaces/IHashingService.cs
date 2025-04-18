using JWTAuthApi.Users.Entities;

namespace JWTAuthApi.Users.Services.Interfaces;

public interface IHashingService
{
    public string HashPassword(User user, string password);
    public bool IsValidPassword(User user, string password);
}

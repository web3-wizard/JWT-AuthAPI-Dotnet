using JWTAuthApi.Users.Entities;
using JWTAuthApi.Users.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace JWTAuthApi.Users.Services;

public class HashingService : IHashingService
{
    public string HashPassword(User user, string password)
    {
        var passwordHasher = new PasswordHasher<User>();

        return passwordHasher.HashPassword(
            user: user,
            password: password);
    }

    public bool IsValidPassword(User user, string password)
    {
        var passwordHasher = new PasswordHasher<User>();

        var result = passwordHasher.VerifyHashedPassword(
            user: user,
            hashedPassword: user.PasswordHash,
            providedPassword: password);

        return result != PasswordVerificationResult.Failed;
    }
}

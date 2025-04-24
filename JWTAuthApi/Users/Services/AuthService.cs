using System.Net;
using JWTAuthApi.DB;
using JWTAuthApi.Users.Configs;
using JWTAuthApi.Users.Entities;
using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.Requests;
using JWTAuthApi.Users.Models.Responses;
using JWTAuthApi.Users.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JWTAuthApi.Users.Services;

public class AuthService(
    AppDbContext dbContext,
    IHashingService hashingService,
    ITokenService tokenService,
    IOptions<JWTConfig> jwtOptions,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly JWTConfig _jwtConfig = jwtOptions.Value;

    public async Task<ServiceResult> Register(RegisterRequest request)
    {
        try
        {
            var (name, username, email, password) = request;
            
            logger.LogInformation("Checking for existing user");
            var isExistingUser = await CheckIfUserExists(email, username);
            
            if (isExistingUser)
            {
                logger.LogInformation("User already exists");
                return new ServiceResult(HttpStatusCode.Conflict, "User already exists!");
            }
            
            logger.LogInformation("Creating new user");
            var user = User.CreateGuestUser(name, username, email);

            var hashPassword = hashingService.HashPassword(user, password);
            user.UpdatePassword(hashPassword);
            
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("User created successfully.");
            
            return new ServiceResult(HttpStatusCode.Created, "User register successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: $"Failed to register user. Error: {ex.Message}");
            return new ServiceResult(HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }

    public async Task<ServiceResult<LoginResponse>> Login(LoginRequest request)
    {
        try
        {
            logger.LogInformation("Checking for existing user");
            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == request.Username.ToLower());

            if (user is null)
            {
                logger.LogWarning("User not found.");
                return new ServiceResult<LoginResponse>(HttpStatusCode.BadRequest, "Invalid Credentials!");
            }
            
            logger.LogInformation("Validating password");
            var isValidPassword = hashingService.IsValidPassword(user, request.Password);

            if (isValidPassword == false)
            {
                logger.LogWarning("Invalid Password.");
                return new ServiceResult<LoginResponse>(HttpStatusCode.BadRequest, "Invalid Credentials!");
            }

            var accessToken = tokenService.GenerateAccessToken(user);
            logger.LogInformation("Access Token generated successfully.");

            var refreshToken = await GenerateAndSaveRefreshToken(user);
            logger.LogInformation("Refresh Token generated successfully");
            
            return new ServiceResult<LoginResponse>(
                HttpStatusCode.OK,
                "Logged in successfully",
                new LoginResponse(AccessToken: accessToken, RefreshToken: refreshToken));
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: $"Failed to login user. Error: {ex.Message}");
            return new ServiceResult<LoginResponse>(HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }

    public async Task<ServiceResult> ConfirmedEmail(VerifyEmailRequest request)
    {
        try
        {
            var user = await dbContext.Users.FindAsync(request.UserId);

            if (user is null)
            {
                return new ServiceResult(HttpStatusCode.NotFound, "User not exists!");
            }

            if (user.Email.Equals(request.Email) == false)
            {
                return new ServiceResult(HttpStatusCode.Forbidden, "You don't have required accessed!");
            }

            if (user.Roles.Contains(nameof(UserRoles.Guest)))
            {
                user.AddUserRole();
            }

            await dbContext.SaveChangesAsync();

            return new ServiceResult(HttpStatusCode.OK, "Email confirmed!");
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: $"Failed to confirmed user email. Error: {ex.Message}");
            return new ServiceResult<LoginResponse>(HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user)
    {
        string refreshToken = tokenService.GenerateRefreshToken();
        DateTime expiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiresInDays);

        user.UpdateRefreshToken(refreshToken, expiryTime);
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();

        return refreshToken;
    }

    private async Task<bool> CheckIfUserExists(string email, string username)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username.ToLower() 
                                      || x.Email == email.ToLower());

        return user is not null;
    }
}
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

    public async Task<ServiceResult> Register(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var (name, username, email, password) = request;

            logger.LogInformation("Checking for existing user");
            var isExistingUser = await CheckIfUserExists(email, username, cancellationToken);

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
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User created successfully.");

            return new ServiceResult(HttpStatusCode.Created, "User register successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: $"Failed to register user. Error: {ex.Message}");
            return new ServiceResult(HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }

    public async Task<ServiceResult<TokenResponseDTO>> Login(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Checking for existing user");
            User? user = await FindUserByUserName(request.Username, cancellationToken);

            if (user is null)
            {
                logger.LogWarning("User not found.");
                return new ServiceResult<TokenResponseDTO>(HttpStatusCode.BadRequest, "Invalid Credentials!");
            }

            logger.LogInformation("Validating password");
            var isValidPassword = hashingService.IsValidPassword(user, request.Password);

            if (isValidPassword == false)
            {
                logger.LogWarning("Invalid Password.");
                return new ServiceResult<TokenResponseDTO>(HttpStatusCode.BadRequest, "Invalid Credentials!");
            }

            var tokenResponseDTO = await GenerateTokens(user);

            return new ServiceResult<TokenResponseDTO>(
                statusCode: HttpStatusCode.OK,
                message: "Logged in successfully",
                data: tokenResponseDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: $"Failed to login user. Error: {ex.Message}");
            return new ServiceResult<TokenResponseDTO>(HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }

    public async Task<ServiceResult> ConfirmedEmail(VerifyEmailRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Checking for existing user");
            var user = await FindUserById(request.UserId, cancellationToken);

            if (user is null)
            {
                logger.LogWarning("User not found.");
                return new ServiceResult(HttpStatusCode.NotFound, "User not exists!");
            }

            if (user.Email.Equals(request.Email) == false)
            {
                return new ServiceResult(HttpStatusCode.Forbidden, "You don't have required accessed!");
            }

            if (user.Roles.Contains(nameof(UserRoles.Guest)))
            {
                user.AddUserRole();
                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return new ServiceResult(HttpStatusCode.OK, "Email confirmed!");
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: $"Failed to confirmed user email. Error: {ex.Message}");
            return new ServiceResult<TokenResponseDTO>(HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }

    public async Task<ServiceResult<TokenResponseDTO>> RefreshTokens(RefreshTokensRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Checking for existing user");
            var user = await FindUserById(request.UserId, cancellationToken);

            if (user is null)
            {
                logger.LogWarning("User not found.");
                return new ServiceResult<TokenResponseDTO>(HttpStatusCode.NotFound, "User not exists!");
            }

            if (CheckValidRefreshToken(user: user, refreshToken: request.RefreshToken))
            {
                logger.LogWarning("Invalid Refresh Token");
                return new ServiceResult<TokenResponseDTO>(HttpStatusCode.Unauthorized, "Please logged in again!");
            }

            var tokenResponseDTO = await GenerateTokens(user);

            return new ServiceResult<TokenResponseDTO>(
                statusCode: HttpStatusCode.OK,
                message: "Tokens refreshed successfully",
                data: tokenResponseDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: $"Failed to refresh tokens. Error: {ex.Message}");
            return new ServiceResult<TokenResponseDTO>(HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }

    private static bool CheckValidRefreshToken(User user, string refreshToken)
    {
        return string.IsNullOrEmpty(user.RefreshToken)
                || user.RefreshToken.Equals(refreshToken) == false
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow;
    }

    private async Task<TokenResponseDTO> GenerateTokens(User user)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        logger.LogInformation("Access Token generated successfully.");

        var refreshToken = await GenerateAndSaveRefreshToken(user);
        logger.LogInformation("Refresh Token generated successfully");

        return new TokenResponseDTO(AccessToken: accessToken, RefreshToken: refreshToken);
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user)
    {
        string refreshToken = tokenService.GenerateRefreshToken();
        DateTime expiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiresInDays);

        user.UpdateRefreshToken(refreshToken, expiryTime);
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Refresh Token updated successfully.");

        return refreshToken;
    }

    private async Task<bool> CheckIfUserExists(string email, string username,CancellationToken cancellationToken )
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username.ToLower()
                                      || u.Email == email.ToLower(), cancellationToken: cancellationToken);

        return user is not null;
    }

    private async Task<User?> FindUserByUserName(string username, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username.ToLower(), cancellationToken: cancellationToken);
    }

    private async Task<User?> FindUserById(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);
    }
}
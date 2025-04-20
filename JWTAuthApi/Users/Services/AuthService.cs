using System.Net;
using JWTAuthApi.DB;
using JWTAuthApi.Users.Entities;
using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.Requests;
using JWTAuthApi.Users.Models.Responses;
using JWTAuthApi.Users.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthApi.Users.Services;

public class AuthService(
    AppDbContext dbContext,
    IHashingService hashingService,
    ITokenService tokenService,
    ILogger<AuthService> logger) : IAuthService
{
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
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == request.Username.ToLower());

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

            var token = tokenService.GenerateToken(user);
            logger.LogInformation("Token generated successfully.");
            
            return new ServiceResult<LoginResponse>(
                HttpStatusCode.OK,
                "Logged in successfully",
                new LoginResponse(token));
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: $"Failed to login user. Error: {ex.Message}");
            return new ServiceResult<LoginResponse>(HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }

    private async Task<bool> CheckIfUserExists(string email, string username)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(
            x => x.Username == username.ToLower() 
                 || x.Email == email.ToLower());

        return existingUser is not null;
    }
}
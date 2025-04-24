using System.Net;
using JWTAuthApi.DB;
using JWTAuthApi.Users.Entities;
using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.DTOs;
using JWTAuthApi.Users.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthApi.Users.Services;

public class UserService(
    AppDbContext dbContext,
    ILogger<UserService> logger) : IUserService
{
    public async Task<ServiceResult<List<UserDTO>>> GetAllUser(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Fetching users data...");
            var users = await dbContext.Users
                .AsNoTracking()
                .OrderByDescending(u => u.UpdatedAt)
                .ToListAsync(cancellationToken: cancellationToken);

            logger.LogInformation("Users data fetched successfully");

            return new ServiceResult<List<UserDTO>>(
                statusCode: HttpStatusCode.OK,
                message: "Users fetched successfully.",
                data: users.ToDTOList());
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: $"Failed to get users. Error: {ex.Message}");
            return new ServiceResult<List<UserDTO>>(HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }

    public async Task<ServiceResult<UserDTO>> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Fetching User Details... Id: {id}", userId);
            var user = await FindUserById(userId, cancellationToken);

            if (user is null)
            {
                logger.LogWarning("User not found. Id: {id}", userId);
                return new ServiceResult<UserDTO>(HttpStatusCode.NotFound, "User details not found");
            }

            logger.LogInformation("User details fetched successfully. Id: {id}", user.Id);

            return new ServiceResult<UserDTO>(
                statusCode: HttpStatusCode.OK,
                message: "User details fetched successfully",
                data: user.ToDTO());
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: $"Failed to get user.Id: {userId}, Error: {ex.Message}");
            return new ServiceResult<UserDTO>(HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }

    private async Task<User?> FindUserById(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);
    }
}

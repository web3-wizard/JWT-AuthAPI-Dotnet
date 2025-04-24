using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.DTOs;

namespace JWTAuthApi.Users.Services.Interfaces;

public interface IUserService
{
    Task<ServiceResult<List<UserDTO>>> GetAllUser(CancellationToken cancellationToken);
    Task<ServiceResult<UserDTO>> GetUser(Guid userId, CancellationToken cancellationToken);
}

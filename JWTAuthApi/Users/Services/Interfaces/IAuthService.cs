using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.Requests;
using JWTAuthApi.Users.Models.Responses;

namespace JWTAuthApi.Users.Services.Interfaces;

public interface IAuthService
{
    public Task<ServiceResult> Register(RegisterRequest request);
    public Task<ServiceResult<LoginResponse>> Login(LoginRequest request);
}
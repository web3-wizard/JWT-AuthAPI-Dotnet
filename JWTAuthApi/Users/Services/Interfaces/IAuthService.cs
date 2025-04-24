using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.Requests;
using JWTAuthApi.Users.Models.Responses;

namespace JWTAuthApi.Users.Services.Interfaces;

public interface IAuthService
{
    public Task<ServiceResult> Register(RegisterRequest request);
    public Task<ServiceResult<TokenResponseDTO>> Login(LoginRequest request);
    public Task<ServiceResult> ConfirmedEmail(VerifyEmailRequest request);
    public Task<ServiceResult<TokenResponseDTO>> RefreshTokens(RefreshTokensRequest request);
}
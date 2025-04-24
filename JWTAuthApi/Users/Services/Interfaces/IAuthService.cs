using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.DTOs;
using JWTAuthApi.Users.Models.Requests;

namespace JWTAuthApi.Users.Services.Interfaces;

public interface IAuthService
{
    public Task<ServiceResult> Register(RegisterRequest request, CancellationToken cancellationToken);
    public Task<ServiceResult<TokenDTO>> Login(LoginRequest request, CancellationToken cancellationToken);
    public Task<ServiceResult> ConfirmedEmail(VerifyEmailRequest request, CancellationToken cancellationToken);
    public Task<ServiceResult<TokenDTO>> RefreshTokens(RefreshTokensRequest request, CancellationToken cancellationToken);
}
using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.Requests;
using JWTAuthApi.Users.Models.Responses;

namespace JWTAuthApi.Users.Services.Interfaces;

public interface IAuthService
{
    public Task<ServiceResult> Register(RegisterRequest request, CancellationToken cancellationToken);
    public Task<ServiceResult<TokenResponseDTO>> Login(LoginRequest request, CancellationToken cancellationToken);
    public Task<ServiceResult> ConfirmedEmail(VerifyEmailRequest request, CancellationToken cancellationToken);
    public Task<ServiceResult<TokenResponseDTO>> RefreshTokens(RefreshTokensRequest request, CancellationToken cancellationToken);
}
using System.ComponentModel.DataAnnotations;

namespace JWTAuthApi.Users.Models.Requests;

public record RefreshTokensRequest(
    [Required] Guid UserId,
    [Required] string RefreshToken
);

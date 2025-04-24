namespace JWTAuthApi.Users.Models.Responses;

public record TokenResponseDTO(string AccessToken, string RefreshToken);
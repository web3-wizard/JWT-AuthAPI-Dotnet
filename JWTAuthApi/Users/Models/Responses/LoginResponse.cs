namespace JWTAuthApi.Users.Models.Responses;

public record LoginResponse(string AccessToken, string RefreshToken);
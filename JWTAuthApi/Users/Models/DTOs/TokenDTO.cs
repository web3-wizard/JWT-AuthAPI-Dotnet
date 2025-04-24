namespace JWTAuthApi.Users.Models.DTOs;

public record TokenDTO(string AccessToken, string RefreshToken);
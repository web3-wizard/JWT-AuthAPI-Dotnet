namespace JWTAuthApi.Users.Models.Responses;

public record VerifyResponse(
    Guid Id,
    string Name,
    string Username,
    string Email,
    List<string> Roles);
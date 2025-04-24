namespace JWTAuthApi.Users.Models.DTOs;

public record UserDTO(
    Guid Id,
    string Name,
    string UserName,
    string Email,
    List<string> Roles,
    bool IsEmailConfirmed,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt
);

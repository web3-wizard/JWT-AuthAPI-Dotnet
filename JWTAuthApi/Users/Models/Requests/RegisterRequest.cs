using System.ComponentModel.DataAnnotations;

namespace JWTAuthApi.Users.Models.Requests;

public record RegisterRequest(
    [Required] string Name,
    [Required] string Username,
    [Required][EmailAddress] string Email,
    [Required] string Password
);
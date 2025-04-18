using System.ComponentModel.DataAnnotations;

namespace JWTAuthApi.Users.Models.Requests;

public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);

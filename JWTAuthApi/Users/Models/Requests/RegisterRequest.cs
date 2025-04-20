using System.ComponentModel.DataAnnotations;
using JWTAuthApi.Users.Configs;

namespace JWTAuthApi.Users.Models.Requests;

public record RegisterRequest(
    [Required, MinLength(UserConstraints.NameMinLength), MaxLength(UserConstraints.NameMaxLength)] string Name,
    [Required, MinLength(UserConstraints.UsernameMinLength), MaxLength(UserConstraints.UsernameMaxLength)] string Username,
    [Required, EmailAddress, MaxLength(UserConstraints.EmailMaxLength)] string Email,
    [Required, MinLength(UserConstraints.PasswordMinLength), MaxLength(UserConstraints.PasswordMaxLength)] string Password
);
using System.ComponentModel.DataAnnotations;
using JWTAuthApi.Users.Configs;

namespace JWTAuthApi.Users.Models.Requests;

public record VerifyEmailRequest(
    [Required] Guid UserId,
    [Required, EmailAddress, MaxLength(UserConstraints.EmailMaxLength)] string Email);
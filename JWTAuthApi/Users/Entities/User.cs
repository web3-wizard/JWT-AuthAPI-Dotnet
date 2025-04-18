using System.ComponentModel.DataAnnotations;

namespace JWTAuthApi.Users.Entities;

public class User
{
    [Key]
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
}

using System.ComponentModel.DataAnnotations;
using JWTAuthApi.Users.Configs;

namespace JWTAuthApi.Users.Entities;

public class User
{
    [Key]
    public Guid Id { get; init; }
    
    [Required]
    [MinLength(UserConstraints.NameMinLength)]
    [MaxLength(UserConstraints.NameMaxLength)]
    public string Name { get; init; }
    
    [Required]
    [MinLength(UserConstraints.UsernameMinLength)]
    [MaxLength(UserConstraints.UsernameMaxLength)]
    public string Username { get; init; }
    
    [Required]
    [MaxLength(UserConstraints.EmailMaxLength)]
    [EmailAddress]
    public string Email { get; init; }
    
    [Required]
    [MaxLength(UserConstraints.PasswordHashMaxLength)]
    public string PasswordHash { get; private set; }
    
    public List<string> Roles { get; private set; }

    private User(string name, string username, string email, List<string> roles)
    {
        Id = Guid.NewGuid();
        Name = name;
        Username = username.ToLower();
        Email = email.ToLower();
        PasswordHash = string.Empty;
        Roles = roles;
    }

    public static User CreateGuestUser(string name, string username, string email)
    {
        List<string> roles = [nameof(UserRoles.Guest)];
        
        return new User(
            name: name,
            username: username,
            email: email,
            roles: roles);
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void AddRole(string role)
    {
        Roles.Add(role);
    }
}

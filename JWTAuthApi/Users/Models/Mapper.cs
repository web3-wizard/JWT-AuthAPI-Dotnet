using JWTAuthApi.Users.Entities;
using JWTAuthApi.Users.Models.DTOs;

namespace JWTAuthApi.Users.Models;

public static class Mapper
{
    public static UserDTO ToDTO(this User user)
    {
        return new UserDTO(
            Id: user.Id,
            Name: user.Name,
            UserName: user.Username,
            Email: user.Email,
            Roles: user.Roles,
            IsEmailConfirmed: user.IsEmailConfirmed,
            CreatedAt: user.CreatedAt,
            LastUpdatedAt: user.UpdatedAt
        );
    }

    public static List<UserDTO> ToDTOList(this IEnumerable<User> users)
    {
        return users.Select(ToDTO).ToList();
    }
}

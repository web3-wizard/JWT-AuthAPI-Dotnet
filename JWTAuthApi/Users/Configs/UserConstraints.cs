namespace JWTAuthApi.Users.Configs;

public static class UserConstraints
{
    public const int NameMinLength = 3;
    public const int NameMaxLength = 50;
    public const int UsernameMinLength = 3;
    public const int UsernameMaxLength = 25;
    public const int EmailMaxLength = 100;
    public const int PasswordMinLength = 6;
    public const int PasswordMaxLength = 20;
    public const int PasswordHashMaxLength = 500;
}
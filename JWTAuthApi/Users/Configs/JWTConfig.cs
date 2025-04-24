namespace JWTAuthApi.Users.Configs;

public class JWTConfig
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpiresInMin { get; set; } = 30;
    public int RefreshTokenExpiresInDays {get; set; } = 3;
}

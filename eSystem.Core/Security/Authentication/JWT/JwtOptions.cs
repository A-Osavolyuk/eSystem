namespace eSystem.Core.Security.Authentication.JWT;

public class JwtOptions
{
    public int AccessTokenExpirationMinutes { get; init; }
    public int RefreshTokenExpirationDays { get; init; }
    public string Secret { get; init; } = string.Empty;
    public List<string> Audience { get; init; } = [];
    public string Issuer { get; init; } = string.Empty;
}
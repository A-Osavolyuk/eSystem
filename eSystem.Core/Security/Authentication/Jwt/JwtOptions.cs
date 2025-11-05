namespace eSystem.Core.Security.Authentication.Jwt;

public class JwtOptions
{
    public int AccessTokenExpirationMinutes { get; init; }
    public string Secret { get; init; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
}
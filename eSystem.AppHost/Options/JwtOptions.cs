namespace eSystem.AppHost.Options;

public class JwtOptions
{
    public int AccessTokenExpirationMinutes { get; init; }
    public string Secret { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
}
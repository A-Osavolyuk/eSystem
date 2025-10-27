namespace eSystem.AppHost.Options;

public class JwtOptions
{
    public int AccessTokenExpirationMinutes { get; init; }
    public int RefreshTokenExpirationDays { get; init; }
    public string Secret { get; init; } = string.Empty;
    public List<string> Audiences { get; init; } = [];
    public string Issuer { get; init; } = string.Empty;
}
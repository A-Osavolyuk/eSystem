namespace eSecurity.Server.Security.Cryptography.Tokens.Jwt;

public class TokenOptions
{
    public string Issuer { get; set; } = string.Empty;
    public TimeSpan AccessTokenLifetime { get; set; }
    public TimeSpan IdTokenLifetime { get; set; }
}
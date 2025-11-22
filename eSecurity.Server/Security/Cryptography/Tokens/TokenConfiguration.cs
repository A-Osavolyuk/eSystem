namespace eSecurity.Server.Security.Cryptography.Tokens;

public class TokenOptions
{
    public string Issuer { get; set; } = string.Empty;
    public TimeSpan AccessTokenLifetime { get; set; }
    public TimeSpan IdTokenLifetime { get; set; }
    public TimeSpan OpaqueTokenLifetime { get; set; }
    public int OpaqueTokenLength { get; set; }
    public int RefreshTokenLength { get; set; }
}
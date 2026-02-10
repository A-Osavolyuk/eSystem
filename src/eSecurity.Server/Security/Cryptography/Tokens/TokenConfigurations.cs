namespace eSecurity.Server.Security.Cryptography.Tokens;

public class TokenConfigurations
{
    public string Issuer { get; set; } = string.Empty;
    public int OpaqueTokenLength { get; set; }
    public TimeSpan DefaultAccessTokenLifetime { get; set; }
    public TimeSpan DefaultIdTokenLifetime { get; set; }
    public TimeSpan DefaultLoginTokenLifetime { get; set; }
    public TimeSpan DefaultRefreshTokenLifetime { get; set; }
}
namespace eSecurity.Server.Security.Cryptography.Tokens;

public class TokenConfigurations
{
    public string Issuer { get; set; } = string.Empty;
    public int OpaqueTokenLength { get; set; }
    public TimeSpan DefaultAccessTokenLifetime { get; set; }
    public TimeSpan MinAccessTokenLifetime { get; set; }
    public TimeSpan MaxAccessTokenLifetime { get; set; }
    
    public TimeSpan DefaultIdTokenLifetime { get; set; }
    public TimeSpan MinIdTokenLifetime { get; set; }
    public TimeSpan MaxIdTokenLifetime { get; set; }
    
    public TimeSpan DefaultLoginTokenLifetime { get; set; }
    public TimeSpan MinLoginTokenLifetime { get; set; }
    public TimeSpan MaxLoginTokenLifetime { get; set; }
    
    public TimeSpan DefaultRefreshTokenLifetime { get; set; }
    public TimeSpan MinRefreshTokenLifetime { get; set; }
    public TimeSpan MaxRefreshTokenLifetime { get; set; }
    
    public TimeSpan DefaultLogoutTokenLifetime { get; set; }
    public TimeSpan MinLogoutTokenLifetime { get; set; }
    public TimeSpan MaxLogoutTokenLifetime { get; set; }
}
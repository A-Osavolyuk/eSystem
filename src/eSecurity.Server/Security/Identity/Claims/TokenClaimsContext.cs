namespace eSecurity.Server.Security.Identity.Claims;

public abstract class TokenClaimsContext
{
    public required string Subject { get; set; }
    public required DateTimeOffset Expiration { get; set; }
    public DateTimeOffset? NotBefore { get; set; }
    public DateTimeOffset? IssuedAt { get; set; }
    public IEnumerable<string> Scopes { get; set; } = [];
}
namespace eSecurity.Server.Security.Identity.Claims;

public abstract class TokenClaimsContext
{
    public string? Nonce { get; set; }

    public DateTimeOffset? Nbf { get; set; }
    public IEnumerable<string> Scopes { get; set; } = [];
}
namespace eSecurity.Server.Security.Identity.Claims;

public abstract class TokenClaimsContext
{
    public required IEnumerable<string> Scopes { get; set; }
    public required string Aud { get; set; }
    public string? Nonce { get; set; }

    public DateTimeOffset? Nbf { get; set; }
}
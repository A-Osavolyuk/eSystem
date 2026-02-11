namespace eSecurity.Server.Security.Identity.Claims;

public abstract class TokenClaimsContext
{
    public IEnumerable<string> Scopes { get; set; } = [];
    public required DateTimeOffset Exp { get; set; }
    public DateTimeOffset? Nbf { get; set; }
    public DateTimeOffset? Iat { get; set; }
}
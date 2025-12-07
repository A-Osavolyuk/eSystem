using System.Security.Claims;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Identity.Claims;

public abstract class ClaimsContext
{
    public required List<string> Scopes { get; set; }
    public required string Aud { get; set; }
    public required string Sid { get; set; }

    public DateTimeOffset? Nbf { get; set; }
}

public interface IClaimFactory<in TContext> where TContext : ClaimsContext
{
    public ValueTask<List<Claim>> GetClaimsAsync(UserEntity user, TContext context, 
        CancellationToken cancellationToken);
}
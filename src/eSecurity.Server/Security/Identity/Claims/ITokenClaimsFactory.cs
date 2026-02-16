using System.Security.Claims;

namespace eSecurity.Server.Security.Identity.Claims;

public interface ITokenClaimsFactory<in TContext, in TSource> where TContext : TokenClaimsContext
{
    public ValueTask<List<Claim>> GetClaimsAsync(TSource client, TContext context, CancellationToken cancellationToken);
}
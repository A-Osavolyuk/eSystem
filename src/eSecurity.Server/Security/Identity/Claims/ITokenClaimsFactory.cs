using System.Security.Claims;

namespace eSecurity.Server.Security.Identity.Claims;

public interface ITokenClaimsFactory<in TContext, in TSource> where TContext : TokenClaimsContext
{
    public ValueTask<List<Claim>> GetClaimsAsync(TSource source, TContext context, CancellationToken cancellationToken);
}
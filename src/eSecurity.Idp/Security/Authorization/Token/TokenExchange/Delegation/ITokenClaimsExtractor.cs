using System.Security.Claims;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Token.TokenExchange.Delegation;

public interface ITokenClaimsExtractor
{
    public ValueTask<TypedResult<IEnumerable<Claim>>> ExtractAsync(string source, CancellationToken cancellationToken);
}
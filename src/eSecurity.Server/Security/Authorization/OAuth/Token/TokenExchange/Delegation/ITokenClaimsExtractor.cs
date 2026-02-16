using System.Security.Claims;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Delegation;

public interface ITokenClaimsExtractor
{
    public ValueTask<TypedResult<IEnumerable<Claim>>> ExtractAsync(string source, CancellationToken cancellationToken);
}
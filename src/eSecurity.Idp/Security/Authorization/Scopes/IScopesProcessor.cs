using System.Security.Claims;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Scopes;

public interface IScopesProcessor
{
    ValueTask<TypedResult<List<Claim>>> ProcessAsync(ScopeContext context, IEnumerable<string> scopes,
        CancellationToken cancellationToken = default);
}
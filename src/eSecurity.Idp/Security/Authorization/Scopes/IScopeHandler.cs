using System.Security.Claims;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Scopes;

public interface IScopeHandler
{
    bool CanHandle(string scope);
    
    ValueTask<TypedResult<List<Claim>>> HandleAsync(ScopeContext context, CancellationToken cancellationToken = default);
}
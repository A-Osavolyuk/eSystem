using System.Security.Claims;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Idp.Security.Authorization.Scopes.Handlers;

public sealed class PhoneScopeHandler : IScopeHandler
{
    public bool CanHandle(string scope) => scope == ScopeTypes.Phone;

    public async ValueTask<TypedResult<List<Claim>>> HandleAsync(ScopeContext context, 
        CancellationToken cancellationToken = default)
    {
        //TODO: Implement
        return TypedResult<List<Claim>>.Success([]);
    }
}
using System.Security.Claims;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Scopes;

public sealed class ScopesProcessor(IEnumerable<IScopeHandler> handlers) : IScopesProcessor
{
    private readonly IEnumerable<IScopeHandler> _handlers = handlers;

    public async ValueTask<TypedResult<List<Claim>>> ProcessAsync(ScopeContext context, IEnumerable<string> scopes,
        CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>();
        foreach (var scope in scopes)
        {
            var handler = _handlers.FirstOrDefault(x => x.CanHandle(scope));
            if (handler is null)
                continue;

            var result = await handler.HandleAsync(context, cancellationToken);
            if (!result.Succeeded)
                return result;

            var resultClaim = result.GetValue();
            claims.AddRange(resultClaim);
        }

        return TypedResult<List<Claim>>.Success(claims);
    }
}
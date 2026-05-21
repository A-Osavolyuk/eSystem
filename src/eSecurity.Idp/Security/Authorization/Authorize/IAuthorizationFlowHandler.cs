using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Authorize;

public interface IAuthorizationFlowHandler
{
    public ValueTask<Result> HandleAsync(AuthorizationRequest request, CancellationToken cancellationToken = default);
}
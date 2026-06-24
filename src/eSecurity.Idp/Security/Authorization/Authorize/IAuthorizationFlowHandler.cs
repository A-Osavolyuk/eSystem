using eSecurity.Idp.Features.Connect;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Authorize;

public interface IAuthorizationFlowHandler
{
    ValueTask<Result> HandleAsync(AuthorizationCommand command, 
        CancellationToken cancellationToken = default);
}
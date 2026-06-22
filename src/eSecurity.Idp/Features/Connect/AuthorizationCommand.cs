using eSecurity.Idp.Security.Authorization;
using eSecurity.Idp.Security.Authorization.Authorize;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Features.Connect;

public sealed record AuthorizationCommand(AuthorizationRequest Request) : IRequest<Result>;

public sealed class AuthorizationCommandHandler(
    IAuthorizationFlowHandlerProvider authorizationFlowHandlerProvider) : IRequestHandler<AuthorizationCommand, Result>
{
    private readonly IAuthorizationFlowHandlerProvider _authorizationFlowHandlerProvider = authorizationFlowHandlerProvider;

    public async Task<Result> Handle(AuthorizationCommand request, CancellationToken cancellationToken = default)
    {
        AuthorizationFlow flow;
        if (!string.IsNullOrEmpty(request.Request.Request))
        {
            flow = AuthorizationFlow.JwtSecuredAuthorizationRequest;
        }
        else if (!string.IsNullOrEmpty(request.Request.RequestUri))
        {
            flow = AuthorizationFlow.PushedAuthorizationRequest;
        }
        else
        {
            flow = AuthorizationFlow.Manual;
        }

        var handler = _authorizationFlowHandlerProvider.GetHandler(flow);
        return await handler.HandleAsync(request.Request, cancellationToken);
    }
}
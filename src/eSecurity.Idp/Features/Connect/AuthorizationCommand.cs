using eSecurity.Idp.Security.Authorization.Authorize;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Features.Connect;

public sealed record AuthorizationCommand : IRequest<Result>
{
    [FromQuery(Name = "response_type")]
    public string? ResponseType { get; set; }
    
    [FromQuery(Name = "client_id")]
    public string? ClientId { get; set; }
    
    [FromQuery(Name = "redirect_uri")]
    public string? RedirectUri { get; set; }
    
    [FromQuery(Name = "request_uri")]
    public string? RequestUri { get; set; }
    
    [FromQuery(Name = "request")]
    public string? Request { get; set; }
    
    [FromQuery(Name = "scope")]
    public string? Scope { get; set; }
    
    [FromQuery(Name = "nonce")]
    public string? Nonce { get; set; }
    
    [FromQuery(Name = "state")]
    public string? State { get; set; }
    
    [FromQuery(Name = "code_challenge")]
    public string? CodeChallenge { get; set; }
    
    [FromQuery(Name = "code_challenge_method")]
    public string? CodeChallengeMethod { get; set; }
    
    [FromQuery(Name = "prompt")]
    public string? Prompt { get; set; }
}

public sealed class AuthorizationCommandHandler(
    IAuthorizationFlowHandlerProvider authorizationFlowHandlerProvider) : IRequestHandler<AuthorizationCommand, Result>
{
    private readonly IAuthorizationFlowHandlerProvider _authorizationFlowHandlerProvider = authorizationFlowHandlerProvider;

    public async Task<Result> Handle(AuthorizationCommand request, CancellationToken cancellationToken = default)
    {
        AuthorizationFlow flow;
        if (!string.IsNullOrEmpty(request.Request))
        {
            flow = AuthorizationFlow.JwtSecuredAuthorizationRequest;
        }
        else if (!string.IsNullOrEmpty(request.RequestUri))
        {
            flow = AuthorizationFlow.PushedAuthorizationRequest;
        }
        else
        {
            flow = AuthorizationFlow.Manual;
        }

        var handler = _authorizationFlowHandlerProvider.GetHandler(flow);
        return await handler.HandleAsync(request, cancellationToken);
    }
}
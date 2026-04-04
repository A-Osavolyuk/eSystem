using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authentication.Session;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;

namespace eSecurity.Server.Features.Account.Queries;

public sealed record GetAuthenticationSessionQuery(Guid Sid) : IRequest<Result>;

public sealed class GetAuthenticationSessionQueryHandler(
    IAuthenticationSessionManager authenticationSessionManager) : IRequestHandler<GetAuthenticationSessionQuery, Result>
{
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;

    public async Task<Result> Handle(GetAuthenticationSessionQuery request, 
        CancellationToken cancellationToken = default)
    {
        var authenticationSession = await _authenticationSessionManager.FindByIdAsync(request.Sid, cancellationToken);
        if (authenticationSession is null) return Results.NotFound("Session not found");
        if (authenticationSession.ExpiredAt < DateTimeOffset.UtcNow)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorCode.ExpiredAuthenticationSession,
                Description = "Session is already expired"
            });
        }

        var nextMethod = authenticationSession
            .GetMethods(AuthenticationMethodType.Required)
            .FirstOrDefault();
        
        var allowedMfaMethods = authenticationSession
            .GetMethods(AuthenticationMethodType.AllowedMfa)
            .Select(x => x.Method);
        
        var response = new AuthenticationSessionDto()
        {
            SessionId = authenticationSession.SessionId,
            OAuthFlow =  authenticationSession.OAuthFlow,
            IsCompleted = authenticationSession.GetMethods(AuthenticationMethodType.Required).Count == 0,
            NextMethod = nextMethod?.Method,
            AllowedMfaMethods = allowedMfaMethods
        };
        
        return Results.Ok(response);
    }
}
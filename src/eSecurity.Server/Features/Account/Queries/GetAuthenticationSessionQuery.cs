using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.Session;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Server.Features.Account.Queries;

public sealed record GetAuthenticationSessionQuery(Guid Sid) : IRequest<Result>;

public sealed class GetAuthenticationSessionQueryHandler(
    IAuthenticationSessionManager authenticationSessionManager,
    ISessionManager sessionManager,
    ISessionCookieFactory sessionCookieFactory) : IRequestHandler<GetAuthenticationSessionQuery, Result>
{
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ISessionCookieFactory _sessionCookieFactory = sessionCookieFactory;

    public async Task<Result> Handle(GetAuthenticationSessionQuery request, 
        CancellationToken cancellationToken = default)
    {
        var authenticationSession = await _authenticationSessionManager.FindByIdAsync(request.Sid, cancellationToken);
        if (authenticationSession is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Session not found"
            });
        }
        
        if (authenticationSession.ExpiredAt < DateTimeOffset.UtcNow)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
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
            .Select(x => x.MethodReference);
        
        var response = new AuthenticationSessionDto()
        {
            SessionId = authenticationSession.SessionId,
            OAuthFlow =  authenticationSession.OAuthFlow,
            IsCompleted = authenticationSession.GetMethods(AuthenticationMethodType.Required).Count == 0,
            NextMethod = nextMethod?.MethodReference,
            AllowedMfaMethods = allowedMfaMethods
        };

        if (authenticationSession.SessionId.HasValue)
        {
            var session = await _sessionManager.FindByIdAsync(authenticationSession.SessionId.Value, cancellationToken);
            if (session is null)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.InvalidSession,
                    Description = "Invalid session"
                });
            }
            
            response.SessionCookie = _sessionCookieFactory.CreateCookie(session);
        }
        
        return Results.Success(SuccessCodes.Ok, response);
    }
}
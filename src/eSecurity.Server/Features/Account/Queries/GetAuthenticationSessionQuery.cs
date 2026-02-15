using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authentication.Session;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Mediator;

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
                Code = ErrorTypes.Common.ExpiredAuthenticationSession,
                Description = "Session is already expired"
            });
        }

        var response = new AuthenticationSessionDto()
        {
            SessionId = authenticationSession.SessionId,
            OAuthFlow =  authenticationSession.OAuthFlow,
            IsCompleted = authenticationSession.RequiredAuthenticationMethods.Length == 0,
            NextMethod = authenticationSession.RequiredAuthenticationMethods.First(),
        };
        
        return Results.Ok(response);
    }
}
using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authorization.OAuth.Session;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.OAuth.Queries;

public sealed record GetOAuthSessionQuery(Guid Id) : IRequest<Result>;

public sealed class GetOAuthSessionQueryHandler(
    IOAuthSessionManager sessionManager) : IRequestHandler<GetOAuthSessionQuery, Result>
{
    private readonly IOAuthSessionManager _sessionManager = sessionManager;

    public async Task<Result> Handle(GetOAuthSessionQuery request, CancellationToken cancellationToken)
    {
        var session = await _sessionManager.FindByIdAsync(request.Id, cancellationToken);
        if (session is null) return Results.NotFound("Session was not found");

        if (!session.IsValid || session.Flow is null || session.UserId is null || session.RequireTwoFactor is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.InvalidSession,
                Description = "Invalid session"
            });
        }
        
        return Results.Ok(new OAuthSessionDto()
        {
            Flow = session.Flow.Value,
            UserId = session.UserId.Value,
            Provider =  session.Provider,
            RequireTwoFactor = session.RequireTwoFactor.Value,
            AuthenticationMethods = session.AuthenticationMethods
        });
    }
}
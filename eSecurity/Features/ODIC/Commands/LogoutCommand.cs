using eSecurity.Security.Authentication.Odic.Session;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.ODIC.Commands;

public class LogoutCommand() : IRequest<Result>
{
    public required Guid SessionId { get; set; }
}

public class LogoutCommandHandler(ISessionManager sessionManager) : IRequestHandler<LogoutCommand, Result>
{
    private readonly ISessionManager sessionManager = sessionManager;

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var session = await sessionManager.FindByIdAsync(request.SessionId, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");

        var sessionRemoveResult = await sessionManager.RemoveAsync(session, cancellationToken);
        return sessionRemoveResult;
    }
}
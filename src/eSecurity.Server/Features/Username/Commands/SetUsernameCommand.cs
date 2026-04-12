using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Identity.User;
using eSecurity.Server.Security.Identity.User.Username;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Server.Features.Username.Commands;

public sealed record SetUsernameCommand(SetUsernameRequest Request) : IRequest<Result>;

public sealed class SetUsernameCommandHandler(
    ISessionManager sessionManager,
    IUserManager userManager,
    IUsernameManager usernameManager) : IRequestHandler<SetUsernameCommand, Result>
{
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IUsernameManager _usernameManager = usernameManager;

    public async Task<Result> Handle(SetUsernameCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionManager.FindByIdAsync(request.Request.SessionId, cancellationToken);
        if (session is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Session not found"
            });
        }
        
        var user = await _userManager.FindByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });
        }

        if (await _usernameManager.IsTakenAsync(request.Request.Username, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UsernameTaken,
                Description = "The username is already taken."
            });
        }

        return await _usernameManager.SetAsync(user, request.Request.Username, cancellationToken);
    }
}
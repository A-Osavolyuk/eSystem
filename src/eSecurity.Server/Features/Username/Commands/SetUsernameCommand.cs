using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Identity.User;
using eSecurity.Server.Security.Identity.User.Username;
using eSystem.Core.Http.Constants;
using eSystem.Core.Mediator;

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
        var session = await _sessionManager.FindByIdAsync(request.Request.Sid, cancellationToken);
        if (session is null) return Results.NotFound("Session not found");
        
        var user = await _userManager.FindByIdAsync(session.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found");

        if (await _usernameManager.IsTakenAsync(request.Request.Username, cancellationToken))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.UsernameTaken,
                Description = "The username is already taken."
            });
        }

        return await _usernameManager.SetAsync(user, request.Request.Username, cancellationToken);
    }
}
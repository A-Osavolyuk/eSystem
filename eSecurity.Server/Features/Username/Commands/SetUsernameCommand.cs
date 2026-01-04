using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Identity.User;
using eSecurity.Server.Security.Identity.User.Username;

namespace eSecurity.Server.Features.Username.Commands;

public sealed record SetUsernameCommand(SetUsernameRequest Request) : IRequest<Result>;

public sealed class SetUsernameCommandHandler(
    IUserManager userManager,
    IUsernameManager usernameManager) : IRequestHandler<SetUsernameCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IUsernameManager _usernameManager = usernameManager;

    public async Task<Result> Handle(SetUsernameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found");

        if (await _usernameManager.IsTakenAsync(request.Request.Username, cancellationToken))
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.UsernameTaken,
                Description = "The username is already taken."
            });
        }

        return await _usernameManager.SetAsync(user, request.Request.Username, cancellationToken);
    }
}
using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Username.Commands;

public sealed record SetUsernameCommand(SetUsernameRequest Request) : IRequest<Result>;

public sealed class SetUsernameCommandHandler(IUserManager userManager) : IRequestHandler<SetUsernameCommand, Result>
{
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(SetUsernameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found");

        if (await _userManager.IsUsernameTakenAsync(request.Request.Username, cancellationToken))
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.UsernameTaken,
                Description = "The username is already taken."
            });
        }

        return await _userManager.SetUsernameAsync(user, request.Request.Username, cancellationToken);
    }
}
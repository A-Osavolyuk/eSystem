using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSecurity.Server.Security.Identity.User.Username;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Username.Commands;

public record ChangeUsernameCommand(ChangeUsernameRequest Request) : IRequest<Result>;

public class ChangeUsernameCommandHandler(
    IUserManager userManager,
    IOptions<AccountOptions> options,
    IUsernameManager usernameManager) : IRequestHandler<ChangeUsernameCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IUsernameManager _usernameManager = usernameManager;
    private readonly AccountOptions _options = options.Value;

    public async Task<Result> Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        if (user.Username == request.Request.Username) 
            return Results.BadRequest("Username must be unique.");

        if (_options.RequireUniqueUsername &&
            await _usernameManager.IsTakenAsync(request.Request.Username, cancellationToken))
        {
            return Results.NotFound(new Error
            {
                Code = ErrorTypes.Common.UsernameTaken,
                Description = "Username is already taken"
            });
        }

        var result = await _usernameManager.ChangeAsync(user, request.Request.Username, cancellationToken);
        return result;
    }
}
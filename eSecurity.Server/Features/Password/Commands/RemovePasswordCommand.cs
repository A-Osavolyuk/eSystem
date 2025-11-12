using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Password.Commands;

public record RemovePasswordCommand(RemovePasswordRequest Request) : IRequest<Result>;

public class RemovePasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager) : IRequestHandler<RemovePasswordCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public async Task<Result> Handle(RemovePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        if (!user.HasPassword())
            return Results.BadRequest("User doesn't have a password.");

        if (!user.HasLinkedAccounts() && !user.HasPasskeys())
            return Results.BadRequest("You need to configure sign-in with passkey or linked external account.");

        var result = await _passwordManager.RemoveAsync(user, cancellationToken);
        return result;
    }
}
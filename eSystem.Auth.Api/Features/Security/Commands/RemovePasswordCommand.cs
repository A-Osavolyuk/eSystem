using eSystem.Auth.Api.Security.Authentication.Password;
using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSystem.Auth.Api.Features.Security.Commands;

public record RemovePasswordCommand(RemovePasswordRequest Request) : IRequest<Result>;

public class RemovePasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager) : IRequestHandler<RemovePasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasswordManager passwordManager = passwordManager;

    public async Task<Result> Handle(RemovePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        if (!user.HasPassword())
            return Results.BadRequest("User doesn't have a password.");

        if (!user.HasLinkedAccounts() && !user.HasPasskeys())
            return Results.BadRequest("You need to configure sign-in with passkey or linked external account.");

        var result = await passwordManager.RemoveAsync(user, cancellationToken);
        return result;
    }
}
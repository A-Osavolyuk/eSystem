using eSecurity.Security.Authentication.Password;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.Security.Commands;

public sealed record ChangePasswordCommand(ChangePasswordRequest Request) : IRequest<Result>;

public sealed class ChangePasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasswordManager passwordManager = passwordManager;

    async Task<Result> IRequestHandler<ChangePasswordCommand, Result>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        if (!user.HasPassword()) return Results.BadRequest("User does not have a password.");

        var isCorrectPassword = passwordManager.Check(user, request.Request.CurrentPassword);
        if (!isCorrectPassword) return Results.BadRequest($"Wrong password.");
        
        var result = await passwordManager.ChangeAsync(user, request.Request.NewPassword, cancellationToken);

        return result;
    }
}
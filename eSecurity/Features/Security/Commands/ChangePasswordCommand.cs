using eSecurity.Security.Authentication.Password;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.Security.Commands;

public sealed record ChangePasswordCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public sealed class ChangePasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasswordManager passwordManager = passwordManager;

    async Task<Result> IRequestHandler<ChangePasswordCommand, Result>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        if (!user.HasPassword()) return Results.BadRequest("User does not have a password.");

        var isCorrectPassword = passwordManager.Check(user, request.CurrentPassword);
        if (!isCorrectPassword) return Results.BadRequest($"Wrong password.");
        
        var result = await passwordManager.ChangeAsync(user, request.NewPassword, cancellationToken);

        return result;
    }
}
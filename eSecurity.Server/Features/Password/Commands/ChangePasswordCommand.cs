using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Password.Commands;

public sealed record ChangePasswordCommand(ChangePasswordRequest Request) : IRequest<Result>;

public sealed class ChangePasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;

    async Task<Result> IRequestHandler<ChangePasswordCommand, Result>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        if (!await _passwordManager.HasAsync(user, cancellationToken))
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidPassword,
                Description = "User does not have a password."
            });
        }

        if (!await _passwordManager.CheckAsync(user, request.Request.CurrentPassword, cancellationToken))
        {
            return Results.BadRequest(new Error()
            {
                Code = "Invalid password",
                Description = "Invalid password."
            });
        }
        
        var result = await _passwordManager.ChangeAsync(user, request.Request.NewPassword, cancellationToken);

        return result;
    }
}
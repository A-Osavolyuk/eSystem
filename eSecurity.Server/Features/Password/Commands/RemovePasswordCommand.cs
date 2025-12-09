using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Password.Commands;

public record RemovePasswordCommand(RemovePasswordRequest Request) : IRequest<Result>;

public class RemovePasswordCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ILinkedAccountManager linkedAccountManager,
    IPasswordManager passwordManager) : IRequestHandler<RemovePasswordCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public async Task<Result> Handle(RemovePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        if (!await _passwordManager.HasAsync(user, cancellationToken))
            return Results.BadRequest("User doesn't have a password.");

        if (!await _linkedAccountManager.HasAsync(user, cancellationToken) && 
            !await _passkeyManager.HasAsync(user, cancellationToken))
            return Results.BadRequest("You need to configure sign-in with passkey or linked external account.");

        var result = await _passwordManager.RemoveAsync(user, cancellationToken);
        return result;
    }
}
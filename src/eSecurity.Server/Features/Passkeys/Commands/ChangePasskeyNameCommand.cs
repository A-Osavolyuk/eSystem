using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Passkeys.Commands;

public record ChangePasskeyNameCommand(ChangePasskeyNameRequest Request) : IRequest<Result>;

public class ChangePasskeyNameCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager) : IRequestHandler<ChangePasskeyNameCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;

    public async Task<Result> Handle(ChangePasskeyNameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        if (!await _passkeyManager.HasAsync(user, cancellationToken)) 
            return Results.BadRequest("User does not have any passkeys.");

        var passkey = await _passkeyManager.FindByIdAsync(request.Request.PasskeyId, cancellationToken);
        if (passkey is null) return Results.NotFound("Passkey not found.");

        passkey.DisplayName = request.Request.DisplayName;

        var result = await _passkeyManager.UpdateAsync(passkey, cancellationToken);
        return result;
    }
}
using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.User;

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
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (!user.HasPasskeys()) return Results.BadRequest("User does not have any passkeys.");

        var passkey = user.Devices
            .Where(x => x.Passkey is not null)
            .Select(x => x.Passkey!)
            .FirstOrDefault(x => x.Id == request.Request.PasskeyId);
        
        if (passkey is null) return Results.NotFound("Cannot find user's passkey.");

        passkey.DisplayName = request.Request.DisplayName;
        passkey.UpdateDate = DateTimeOffset.UtcNow;

        var result = await _passkeyManager.UpdateAsync(passkey, cancellationToken);
        return result;
    }
}
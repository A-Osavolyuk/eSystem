using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.Common.Results;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Features.Passkeys.Commands;

public record ChangePasskeyNameCommand(ChangePasskeyNameRequest Request) : IRequest<Result>;

public class ChangePasskeyNameCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager) : IRequestHandler<ChangePasskeyNameCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;

    public async Task<Result> Handle(ChangePasskeyNameCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (!user.HasPasskeys()) return Results.BadRequest("User does not have any passkeys.");

        var passkey = user.Devices
            .Where(x => x.Passkey is not null)
            .Select(x => x.Passkey!)
            .FirstOrDefault(x => x.Id == request.Request.PasskeyId);
        
        if (passkey is null) return Results.NotFound("Cannot find user's passkey.");

        passkey.DisplayName = request.Request.DisplayName;
        passkey.UpdateDate = DateTimeOffset.UtcNow;

        var result = await passkeyManager.UpdateAsync(passkey, cancellationToken);
        return result;
    }
}
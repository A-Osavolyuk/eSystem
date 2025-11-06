using eSecurity.Security.Credentials.PublicKey;
using eSecurity.Security.Identity.User;

namespace eSecurity.Features.Passkeys.Commands;

public class ChangePasskeyNameCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required Guid PasskeyId { get; set; }
    public required string DisplayName { get; set; }
}

public class ChangePasskeyNameCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager) : IRequestHandler<ChangePasskeyNameCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;

    public async Task<Result> Handle(ChangePasskeyNameCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        if (!user.HasPasskeys()) return Results.BadRequest("User does not have any passkeys.");

        var passkey = user.Devices
            .Where(x => x.Passkey is not null)
            .Select(x => x.Passkey!)
            .FirstOrDefault(x => x.Id == request.PasskeyId);
        
        if (passkey is null) return Results.NotFound("Cannot find user's passkey.");

        passkey.DisplayName = request.DisplayName;
        passkey.UpdateDate = DateTimeOffset.UtcNow;

        var result = await passkeyManager.UpdateAsync(passkey, cancellationToken);
        return result;
    }
}
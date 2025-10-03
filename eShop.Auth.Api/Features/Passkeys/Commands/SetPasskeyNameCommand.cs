using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record SetPasskeyNameCommand(SetPasskeyNameRequest Request) : IRequest<Result>;

public class SetPasskeyNameCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager) : IRequestHandler<SetPasskeyNameCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;

    public async Task<Result> Handle(SetPasskeyNameCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var passkey = user.Passkeys.FirstOrDefault(x => x.Id == request.Request.PasskeyId);
        if (passkey is null) return Results.NotFound($"Cannot find user's passkey.");

        passkey.DisplayName = request.Request.DisplayName;
        passkey.UpdateDate = DateTimeOffset.UtcNow;

        var result = await passkeyManager.UpdateAsync(passkey, cancellationToken);
        return result;
    }
}
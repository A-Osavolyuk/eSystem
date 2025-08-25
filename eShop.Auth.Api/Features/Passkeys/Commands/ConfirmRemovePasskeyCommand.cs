using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record ConfirmRemovePasskeyCommand(ConfirmRemovePasskeyRequest Request) : IRequest<Result>;

public class ConfirmRemovePasskeyCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ICodeManager codeManager) : IRequestHandler<ConfirmRemovePasskeyCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ConfirmRemovePasskeyCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var passkey = await passkeyManager.FindByIdAsync(request.Request.KeyId, cancellationToken);
        if (passkey is null) return Results.NotFound($"Cannot find credential with ID {request.Request.KeyId}.");

        var code = request.Request.Code;
        var codeResult = await codeManager.VerifyAsync(user, code, SenderType.Email,
            CodeType.Remove, CodeResource.Passkey, cancellationToken);

        if (!codeResult.Succeeded) return codeResult;

        var result = await passkeyManager.DeleteAsync(passkey, cancellationToken);
        return result;
    }
}
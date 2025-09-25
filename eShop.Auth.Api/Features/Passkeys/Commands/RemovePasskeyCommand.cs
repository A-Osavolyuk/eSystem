using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record RemovePasskeyCommand(RemovePasskeyRequest Request) : IRequest<Result>;

public class RemovePasskeyCommandHandler(
    IPasskeyManager passkeyManager,
    IUserManager userManager,
    IVerificationManager verificationManager,
    ILoginMethodManager loginMethodManager,
    IdentityOptions identityOptions) : IRequestHandler<RemovePasskeyCommand, Result>
{
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly ILoginMethodManager loginMethodManager = loginMethodManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(RemovePasskeyCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var passkey = await passkeyManager.FindByIdAsync(request.Request.KeyId, cancellationToken);
        if (passkey is null) return Results.NotFound($"Cannot find passkey with ID {request.Request.KeyId}.");

        if (identityOptions.SignIn.RequireConfirmedEmail && !user.HasEmail(EmailType.Primary) || !user.HasPassword())
            return Results.BadRequest("You need to enable another authentication method first.");

        var verificationResult = await verificationManager.VerifyAsync(user, 
            CodeResource.Passkey, CodeType.Remove, cancellationToken);
        
        if (!verificationResult.Succeeded) return verificationResult;

        if (user.Passkeys.Count == 1)
        {
            var method = user.GetLoginMethod(LoginType.Passkey);
            var methodResult = await loginMethodManager.RemoveAsync(method, cancellationToken);
            if (!methodResult.Succeeded) return methodResult;
        }

        var result = await passkeyManager.DeleteAsync(passkey, cancellationToken);
        return result;
    }
}
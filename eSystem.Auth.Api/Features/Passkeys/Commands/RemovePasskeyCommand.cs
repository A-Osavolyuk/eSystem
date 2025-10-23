using eSystem.Auth.Api.Interfaces;
using eSystem.Auth.Api.Security.Identity.Options;
using eSystem.Domain.Common.Results;
using eSystem.Domain.Requests.Auth;
using eSystem.Domain.Security.Authentication.TwoFactor;
using eSystem.Domain.Security.Verification;

namespace eSystem.Auth.Api.Features.Passkeys.Commands;

public record RemovePasskeyCommand(RemovePasskeyRequest Request) : IRequest<Result>;

public class RemovePasskeyCommandHandler(
    IPasskeyManager passkeyManager,
    IUserManager userManager,
    IVerificationManager verificationManager,
    ITwoFactorManager twoFactorManager,
    IOptions<SignInOptions> options) : IRequestHandler<RemovePasskeyCommand, Result>
{
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    private readonly SignInOptions options = options.Value;

    public async Task<Result> Handle(RemovePasskeyCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var passkey = await passkeyManager.FindByIdAsync(request.Request.KeyId, cancellationToken);
        if (passkey is null) return Results.NotFound($"Cannot find passkey with ID {request.Request.KeyId}.");

        if (options.RequireConfirmedEmail && !user.HasEmail(EmailType.Primary) || !user.HasPassword())
            return Results.BadRequest("You need to enable another authentication method first.");

        var verificationResult = await verificationManager.VerifyAsync(user, 
            PurposeType.Passkey, ActionType.Remove, cancellationToken);
        
        if (!verificationResult.Succeeded) return verificationResult;

        if (user.CountPasskeys() == 1)
        {
            if (user.HasTwoFactor(TwoFactorMethod.Passkey))
            {
                var twoFactorMethod = user.GetTwoFactorMethod(TwoFactorMethod.Passkey)!;

                if (twoFactorMethod.Preferred)
                {
                    var preferredResult = await twoFactorManager.PreferAsync(user, 
                        TwoFactorMethod.AuthenticatorApp, cancellationToken);
                    if (!preferredResult.Succeeded) return preferredResult;
                }
                
                var twoFactorResult = await twoFactorManager.UnsubscribeAsync(twoFactorMethod, cancellationToken);
                if (!twoFactorResult.Succeeded) return twoFactorResult;
            }

            if (user.HasVerification(VerificationMethod.Passkey))
            {
                var method = user.GetVerificationMethod(VerificationMethod.Passkey)!;

                if (method.Preferred)
                {
                    var preferredMethod = user.HasVerification(VerificationMethod.AuthenticatorApp) 
                        ? VerificationMethod.AuthenticatorApp 
                        : VerificationMethod.Email;
                
                    var methodResult = await verificationManager.PreferAsync(user, preferredMethod, cancellationToken);
                    if (!methodResult.Succeeded) return methodResult;
                }
                
                var unsubscribeResult = await verificationManager.UnsubscribeAsync(method, cancellationToken);
                if (!unsubscribeResult.Succeeded) return unsubscribeResult;
            }
        }

        var result = await passkeyManager.DeleteAsync(passkey, cancellationToken);
        return result;
    }
}
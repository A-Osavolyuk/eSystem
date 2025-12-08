using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Features.Passkeys.Commands;

public record RemovePasskeyCommand(RemovePasskeyRequest Request) : IRequest<Result>;

public class RemovePasskeyCommandHandler(
    IPasskeyManager passkeyManager,
    IUserManager userManager,
    IVerificationManager verificationManager,
    ITwoFactorManager twoFactorManager,
    IOptions<SignInOptions> options) : IRequestHandler<RemovePasskeyCommand, Result>
{
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly SignInOptions _options = options.Value;

    public async Task<Result> Handle(RemovePasskeyCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var passkey = await _passkeyManager.FindByIdAsync(request.Request.PasskeyId, cancellationToken);
        if (passkey is null) return Results.NotFound($"Cannot find passkey with ID {request.Request.PasskeyId}.");

        if (_options.RequireConfirmedEmail && !user.HasEmail(EmailType.Primary) || !user.HasPassword())
            return Results.BadRequest("You need to enable another authentication method first.");

        var verificationResult = await _verificationManager.VerifyAsync(user, 
            PurposeType.Passkey, ActionType.Remove, cancellationToken);
        
        if (!verificationResult.Succeeded) return verificationResult;
        
        var passkeys = await _passkeyManager.GetAllAsync(user, cancellationToken);
        if (passkeys.Count == 1)
        {
            if (user.HasTwoFactor(TwoFactorMethod.Passkey))
            {
                var twoFactorMethod = user.GetTwoFactorMethod(TwoFactorMethod.Passkey)!;

                if (twoFactorMethod.Preferred)
                {
                    var preferredResult = await _twoFactorManager.PreferAsync(user, 
                        TwoFactorMethod.AuthenticatorApp, cancellationToken);
                    if (!preferredResult.Succeeded) return preferredResult;
                }
                
                var twoFactorResult = await _twoFactorManager.UnsubscribeAsync(twoFactorMethod, cancellationToken);
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
                
                    var methodResult = await _verificationManager.PreferAsync(user, preferredMethod, cancellationToken);
                    if (!methodResult.Succeeded) return methodResult;
                }
                
                var unsubscribeResult = await _verificationManager.UnsubscribeAsync(method, cancellationToken);
                if (!unsubscribeResult.Succeeded) return unsubscribeResult;
            }
        }

        var result = await _passkeyManager.DeleteAsync(passkey, cancellationToken);
        return result;
    }
}
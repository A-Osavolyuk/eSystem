using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record EnableTwoFactorCommand(EnableTwoFactorRequest Request) : IRequest<Result>;

public class EnableTwoFactorCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IVerificationManager verificationManager) : IRequestHandler<EnableTwoFactorCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (user.TwoFactorEnabled) return Results.BadRequest("2FA already enabled.");

        var enableVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.TwoFactor, ActionType.Enable, cancellationToken);

        if (!enableVerificationResult.Succeeded) return enableVerificationResult;

        var subscribeVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.AuthenticatorApp, ActionType.Subscribe, cancellationToken);

        if (!subscribeVerificationResult.Succeeded) return subscribeVerificationResult;

        if (user.HasPasskeys() && !user.HasTwoFactor(TwoFactorMethod.Passkey))
        {
            var passkeyResult = await _twoFactorManager.SubscribeAsync(user,
                TwoFactorMethod.Passkey, cancellationToken: cancellationToken);

            if (!passkeyResult.Succeeded) return passkeyResult;

            if (!user.HasVerification(VerificationMethod.Passkey))
            {
                var passkeySubscriptionResult = await _verificationManager.SubscribeAsync(user,
                    VerificationMethod.Passkey, cancellationToken: cancellationToken);

                if (!passkeySubscriptionResult.Succeeded) return passkeySubscriptionResult;
            }
        }

        var authenticatorSubscriptionResult = await _verificationManager.SubscribeAsync(user,
            VerificationMethod.AuthenticatorApp, true, cancellationToken);

        if (!authenticatorSubscriptionResult.Succeeded) return authenticatorSubscriptionResult;

        var authenticatorResult = await _twoFactorManager.SubscribeAsync(user,
            TwoFactorMethod.AuthenticatorApp, true, cancellationToken);

        if (!authenticatorResult.Succeeded) return authenticatorResult;

        var recoveryCodesResult = await _twoFactorManager.SubscribeAsync(user,
            TwoFactorMethod.RecoveryCode, cancellationToken: cancellationToken);

        return recoveryCodesResult;
    }
}
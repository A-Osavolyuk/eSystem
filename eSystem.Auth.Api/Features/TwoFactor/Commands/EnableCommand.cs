using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.Common.Results;
using eSystem.Domain.Requests.Auth;
using eSystem.Domain.Security.Authentication.TwoFactor;
using eSystem.Domain.Security.Verification;

namespace eSystem.Auth.Api.Features.TwoFactor.Commands;

public record EnableCommand(EnableTwoFactorRequest Request) : IRequest<Result>;

public class EnableTwoFactorCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IVerificationManager verificationManager) : IRequestHandler<EnableCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(EnableCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (user.TwoFactorEnabled) return Results.BadRequest("2FA already enabled.");

        var enableVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.TwoFactor, ActionType.Enable, cancellationToken);

        if (!enableVerificationResult.Succeeded) return enableVerificationResult;

        var subscribeVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.AuthenticatorApp, ActionType.Subscribe, cancellationToken);

        if (!subscribeVerificationResult.Succeeded) return subscribeVerificationResult;

        if (user.HasPasskeys() && !user.HasTwoFactor(TwoFactorMethod.Passkey))
        {
            var passkeyResult = await twoFactorManager.SubscribeAsync(user,
                TwoFactorMethod.Passkey, cancellationToken: cancellationToken);

            if (!passkeyResult.Succeeded) return passkeyResult;

            if (!user.HasVerification(VerificationMethod.Passkey))
            {
                var passkeySubscriptionResult = await verificationManager.SubscribeAsync(user,
                    VerificationMethod.Passkey, cancellationToken: cancellationToken);

                if (!passkeySubscriptionResult.Succeeded) return passkeySubscriptionResult;
            }
        }

        var authenticatorSubscriptionResult = await verificationManager.SubscribeAsync(user,
            VerificationMethod.AuthenticatorApp, true, cancellationToken);

        if (!authenticatorSubscriptionResult.Succeeded) return authenticatorSubscriptionResult;

        var authenticatorResult = await twoFactorManager.SubscribeAsync(user,
            TwoFactorMethod.AuthenticatorApp, true, cancellationToken);

        if (!authenticatorResult.Succeeded) return authenticatorResult;

        var recoveryCodesResult = await twoFactorManager.SubscribeAsync(user,
            TwoFactorMethod.RecoveryCode, cancellationToken: cancellationToken);

        return recoveryCodesResult;
    }
}
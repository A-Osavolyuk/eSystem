using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record EnableTwoFactorCommand(EnableTwoFactorRequest Request) : IRequest<Result>;

public class EnableTwoFactorCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IVerificationManager verificationManager) : IRequestHandler<EnableTwoFactorCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (user.TwoFactorEnabled) return Results.BadRequest("2FA already enabled.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.TwoFactor, ActionType.Enable, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        if (user.HasPasskeys() && !user.HasTwoFactor(TwoFactorMethod.Passkey))
        {
            var passkeyResult = await twoFactorManager.SubscribeAsync(user,
                TwoFactorMethod.Passkey, cancellationToken: cancellationToken);

            if (!passkeyResult.Succeeded) return passkeyResult;

            var passkeySubscriptionResult = await verificationManager.SubscribeAsync(user,
                VerificationMethod.Passkey, preferred: false, cancellationToken);

            if (!passkeySubscriptionResult.Succeeded) return passkeySubscriptionResult;
        }

        var authenticatorSubscriptionResult = await verificationManager.SubscribeAsync(user, 
            VerificationMethod.AuthenticatorApp, true, cancellationToken);
        
        if (!authenticatorSubscriptionResult.Succeeded) return authenticatorSubscriptionResult;
        
        var authenticatorResult = await twoFactorManager.SubscribeAsync(user,
            TwoFactorMethod.AuthenticatorApp, true, cancellationToken);

        return authenticatorResult;
    }
}
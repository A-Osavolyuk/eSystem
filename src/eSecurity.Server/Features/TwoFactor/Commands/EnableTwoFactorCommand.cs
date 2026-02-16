using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;

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
        if (user is null) return Results.NotFound("User not found.");
        if (await _twoFactorManager.IsEnabledAsync(user, cancellationToken)) 
            return Results.BadRequest("2FA already enabled.");

        var enableVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.TwoFactor, ActionType.Enable, cancellationToken);

        if (!enableVerificationResult.Succeeded) return enableVerificationResult;

        var subscribeVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.AuthenticatorApp, ActionType.Subscribe, cancellationToken);

        if (!subscribeVerificationResult.Succeeded) return subscribeVerificationResult;

        var authenticatorResult = await _twoFactorManager.SubscribeAsync(user,
            TwoFactorMethod.AuthenticatorApp, true, cancellationToken);

        if (!authenticatorResult.Succeeded) return authenticatorResult;

        var recoveryCodesResult = await _twoFactorManager.SubscribeAsync(user,
            TwoFactorMethod.RecoveryCode, cancellationToken: cancellationToken);

        return recoveryCodesResult;
    }
}
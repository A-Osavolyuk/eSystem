using eSecurity.Security.Authentication.TwoFactor;
using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Features.TwoFactor.Commands;

public record DisableCommand(DisableTwoFactorRequest Request) : IRequest<Result>;

public class DisableTwoFactorCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IVerificationManager verificationManager) : IRequestHandler<DisableCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(DisableCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (!user.TwoFactorEnabled) return Results.BadRequest("2FA already disabled.");
        
        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.TwoFactor, ActionType.Disable, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        var preferredMethod = user.HasVerification(VerificationMethod.Passkey)
            ? VerificationMethod.Passkey
            : VerificationMethod.Email;
        
        var result = await verificationManager.PreferAsync(user, preferredMethod, cancellationToken);
        if (!result.Succeeded) return result;

        var authenticatorMethod = user.GetVerificationMethod(VerificationMethod.AuthenticatorApp)!;
        var authenticatorResult = await verificationManager.UnsubscribeAsync(authenticatorMethod, cancellationToken);
        if (!authenticatorResult.Succeeded) return authenticatorResult;
        
        var methodResult = await twoFactorManager.UnsubscribeAsync(user, cancellationToken);
        return methodResult;
    }
}
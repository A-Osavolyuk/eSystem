using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record DisableTwoFactorCommand(DisableTwoFactorRequest Request) : IRequest<Result>;

public class DisableTwoFactorCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IVerificationManager verificationManager) : IRequestHandler<DisableTwoFactorCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (!user.TwoFactorEnabled) return Results.BadRequest("2FA already disabled.");
        
        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.TwoFactor, ActionType.Disable, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        var preferredMethod = user.HasVerification(VerificationMethod.Passkey)
            ? VerificationMethod.Passkey
            : VerificationMethod.Email;
        
        var result = await _verificationManager.PreferAsync(user, preferredMethod, cancellationToken);
        if (!result.Succeeded) return result;

        var authenticatorMethod = await _verificationManager.GetAsync(user, VerificationMethod.AuthenticatorApp, cancellationToken);
        if (authenticatorMethod is null) return Results.NotFound("Method not found");
        
        var authenticatorResult = await _verificationManager.UnsubscribeAsync(authenticatorMethod, cancellationToken);
        if (!authenticatorResult.Succeeded) return authenticatorResult;
        
        var methodResult = await _twoFactorManager.UnsubscribeAsync(user, cancellationToken);
        return methodResult;
    }
}
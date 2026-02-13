using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;
using eSystem.Core.Mediator;

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
        if (user is null) return Results.NotFound("User not found.");
        if (await _twoFactorManager.IsEnabledAsync(user, cancellationToken)) 
            return Results.BadRequest("2FA already disabled.");
        
        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.TwoFactor, ActionType.Disable, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        var methodResult = await _twoFactorManager.UnsubscribeAsync(user, cancellationToken);
        return methodResult;
    }
}
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record DisableTwoFactorCommand(DisableTwoFactorRequest Request) : IRequest<Result>;

public class DisableTwoFactorCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IVerificationManager verificationManager) : IRequestHandler<DisableTwoFactorCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (!user.TwoFactorEnabled) return Results.BadRequest("2FA already disabled.");
        
        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.TwoFactor, ActionType.Disable, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        var providerResult = await twoFactorManager.UnsubscribeAsync(user, cancellationToken);
        return providerResult;
    }
}
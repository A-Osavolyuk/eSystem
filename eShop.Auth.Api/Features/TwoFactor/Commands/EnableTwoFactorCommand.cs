using eShop.Domain.Requests.API.Auth;

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
            CodeResource.TwoFactor, CodeType.Enable, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        if (user.HasPasskeys() && !user.HasTwoFactor(MethodType.Passkey))
        {
            var passkeyResult = await twoFactorManager.SubscribeAsync(user, 
                MethodType.Passkey, cancellationToken: cancellationToken);

            if (!passkeyResult.Succeeded) return passkeyResult;
        }
        
        var providerResult = await twoFactorManager.SubscribeAsync(user, 
            MethodType.AuthenticatorApp, true, cancellationToken);

        return providerResult;
    }
}
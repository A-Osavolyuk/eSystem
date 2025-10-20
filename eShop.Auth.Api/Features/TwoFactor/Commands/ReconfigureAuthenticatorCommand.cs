using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record ReconfigureAuthenticatorCommand(ReconfigureAuthenticatorRequest Request) : IRequest<Result>;

public class ReconfigureAuthenticatorCommandHandler(
    IUserManager userManager,
    ISecretManager secretManager,
    IVerificationManager verificationManager) : IRequestHandler<ReconfigureAuthenticatorCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISecretManager secretManager = secretManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(ReconfigureAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var verificationResult = await verificationManager.VerifyAsync(user, PurposeType.AuthenticatorApp,
            ActionType.Reconfigure, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var protectedSecret = secretManager.Protect(request.Request.Secret);
        var userSecret = user.Secret!;

        userSecret.Secret = protectedSecret;
        userSecret.UpdateDate = DateTimeOffset.UtcNow;

        var result = await secretManager.SaveAsync(userSecret, cancellationToken);
        return result;
    }
}
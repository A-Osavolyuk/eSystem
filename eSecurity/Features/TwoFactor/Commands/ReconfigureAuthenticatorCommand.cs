using eSecurity.Security.Authentication.TwoFactor.Secret;
using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Features.TwoFactor.Commands;

public record ReconfigureAuthenticatorCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string Secret { get; set; }
}

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
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var verificationResult = await verificationManager.VerifyAsync(user, PurposeType.AuthenticatorApp,
            ActionType.Reconfigure, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var protectedSecret = secretManager.Protect(request.Secret);
        var userSecret = user.Secret!;

        userSecret.Secret = protectedSecret;
        userSecret.UpdateDate = DateTimeOffset.UtcNow;

        var result = await secretManager.UpdateAsync(userSecret, cancellationToken);
        return result;
    }
}
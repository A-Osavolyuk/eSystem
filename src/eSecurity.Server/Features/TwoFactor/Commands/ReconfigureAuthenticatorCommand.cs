using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authentication.TwoFactor.Secret;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record ReconfigureAuthenticatorCommand(ReconfigureAuthenticatorRequest Request) : IRequest<Result>;

public class ReconfigureAuthenticatorCommandHandler(
    IUserManager userManager,
    ISecretManager secretManager,
    IVerificationManager verificationManager,
    IDataProtectionProvider protectionProvider) : IRequestHandler<ReconfigureAuthenticatorCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ISecretManager _secretManager = secretManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;

    public async Task<Result> Handle(ReconfigureAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var verificationResult = await _verificationManager.VerifyAsync(user, PurposeType.AuthenticatorApp,
            ActionType.Reconfigure, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        var protectedSecret = protector.Protect(request.Request.Secret);
        var userSecret = await _secretManager.GetAsync(user, cancellationToken);
        if (userSecret is null) return Results.NotFound("Secret not found");
        
        userSecret.Secret = protectedSecret;

        var result = await _secretManager.UpdateAsync(userSecret, cancellationToken);
        return result;
    }
}
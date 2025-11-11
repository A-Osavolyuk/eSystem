using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Server.Security.Authentication.TwoFactor.Secret;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.User;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Features.Verification.Commands;

public record VerifyAuthenticatorCodeCommand(VerifyAuthenticatorCodeRequest Request) : IRequest<Result>;

public class VerifyAuthenticatorCodeCommandHandler(
    IUserManager userManager,
    ISecretManager secretManager,
    IVerificationManager verificationManager,
    IDataProtectionProvider protectionProvider) : IRequestHandler<VerifyAuthenticatorCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISecretManager secretManager = secretManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly IDataProtectionProvider protectionProvider = protectionProvider;

    public async Task<Result> Handle(VerifyAuthenticatorCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var userSecret = await secretManager.FindAsync(user, cancellationToken);
        if (userSecret is null) return Results.NotFound("Not found user secret");

        var protector = protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        var unprotectedSecret = protector.Unprotect(userSecret.Secret);
        var verified = AuthenticatorUtils.VerifyCode(request.Request.Code, unprotectedSecret);
        if (!verified) return Results.BadRequest("Invalid authenticator code");
        
        var purpose = request.Request.Purpose;
        var action = request.Request.Action;
        var result = await verificationManager.CreateAsync(user, purpose, action, cancellationToken);
        return result;
    }
}
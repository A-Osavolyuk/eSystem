using eSecurity.Idp.Security.Authentication.TwoFactor.Secret;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Cryptography.Protection.Constants;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Idp.Features.TwoFactor.Commands;

public record ReconfigureAuthenticatorCommand(ReconfigureAuthenticatorRequest Request) : IRequest<Result>;

public class ReconfigureAuthenticatorCommandHandler(
    ISecretManager secretManager,
    IVerificationManager verificationManager,
    ICurrentUserAccessor currentUserAccessor,
    IDataProtectionProvider protectionProvider) : IRequestHandler<ReconfigureAuthenticatorCommand, Result>
{
    private readonly ISecretManager _secretManager = secretManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;

    public async Task<Result> Handle(ReconfigureAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var verification = await _verificationManager.FindByIdAsync(request.Request.VerificationId, cancellationToken);
        if (verification?.Status is not VerificationStatus.Approved)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Unverified request."
            });
        }

        var verificationResult = await _verificationManager.ConsumeAsync(verification, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        var protectedSecret = protector.Protect(request.Request.Secret);
        var userSecret = await _secretManager.GetAsync(user, cancellationToken);
        if (userSecret is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Secret not found"
            });
        }

        userSecret.ProtectedSecret = protectedSecret;

        var result = await _secretManager.UpdateAsync(userSecret, cancellationToken);
        return result;
    }
}
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Idp.Security.Authentication.TwoFactor.Secret;
using eSecurity.Idp.Security.Cryptography.Protection.Constants;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Responses;
using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Idp.Security.Authorization.Verification.AuthenticationApp;

public sealed class AuthenticationAppVerificationStrategy(
    IUserManager userManager,
    IVerificationManager verificationManager,
    ISecretManager secretManager,
    IDataProtectionProvider protectionProvider,
    IOptions<VerificationConfiguration> options) : IVerificationStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly ISecretManager _secretManager = secretManager;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly VerificationConfiguration _configuration = options.Value;

    public async ValueTask<Result> ExecuteAsync(VerificationContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is not AuthenticatorAppVerificationContext authenticationContext)
            throw new InvalidOperationException("Invalid context type");
        
        var userResult = await _userManager.GetUserAsync(cancellationToken);
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

        var secret = await _secretManager.GetAsync(user, cancellationToken);
        if (secret is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Authenticator app is not set up for this user."
            });
        }

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        var unprotectedSecret = protector.Unprotect(secret.ProtectedSecret);
        var verified = AuthenticatorUtils.VerifyCode(authenticationContext.Code, unprotectedSecret);
        if (!verified)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid code."
            });
        }

        var requestEntity = new VerificationRequestEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Action = authenticationContext.Action,
            Purpose = authenticationContext.Purpose,
            Method = VerificationMethod.AuthenticatorApp,
            Status = VerificationStatus.Approved,
            ApprovedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configuration.Timestamp)
        };

        var verificationResult = await _verificationManager.CreateAsync(requestEntity, cancellationToken);
        if (!verificationResult.Succeeded) 
            return verificationResult;

        var response = new VerificationResponse { VerificationId = requestEntity.Id };
        return Results.Success(SuccessCodes.Ok, response);
    }
}
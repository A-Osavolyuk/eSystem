using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Server.Security.Authentication.TwoFactor.Secret;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authorization.Verification.AuthenticationApp;

public sealed class AuthenticationAppVerificationStrategy(
    IHttpContextAccessor httpContextAccessor,
    IUserManager userManager,
    IVerificationManager verificationManager,
    ISecretManager secretManager,
    IDataProtectionProvider protectionProvider,
    IOptions<VerificationConfiguration> options) : IVerificationStrategy<AuthenticatorAppVerificationContext>
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly IUserManager _userManager = userManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly ISecretManager _secretManager = secretManager;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly VerificationConfiguration _configuration = options.Value;

    public async ValueTask<Result> ExecuteAsync(AuthenticatorAppVerificationContext context, 
        CancellationToken cancellationToken = default)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");

        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User was not found");

        var secret = await _secretManager.GetAsync(user, cancellationToken);
        if (secret is null) return Results.BadRequest("Authenticator app is not set up for this user.");

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        var unprotectedSecret = protector.Unprotect(secret.ProtectedSecret);
        var verified = AuthenticatorUtils.VerifyCode(context.Code, unprotectedSecret);
        if (!verified) return Results.BadRequest("Invalid code.");

        var requestEntity = new VerificationRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Action = context.Action,
            Purpose = context.Purpose,
            Method = VerificationMethod.AuthenticatorApp,
            Status = VerificationStatus.Approved,
            ApprovedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configuration.Timestamp)
        };
        
        var verificationResult = await _verificationManager.CreateAsync(requestEntity, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var response = new VerificationResponse { VerificationId = requestEntity.Id };
        return Results.Ok(response);
    }
}
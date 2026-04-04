using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Core.Security.Credentials.PublicKey.Constants;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Credentials.PublicKey.Credentials;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authorization.Verification.Passkey;

public sealed class PasskeyVerificationStrategy(
    IHttpContextAccessor httpContextAccessor,
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    IVerificationManager verificationManager,
    IOptions<VerificationConfiguration> options) : IVerificationStrategy<PasskeyVerificationContext>
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly VerificationConfiguration _configuration = options.Value;

    public async ValueTask<Result> ExecuteAsync(PasskeyVerificationContext context, 
        CancellationToken cancellationToken = default)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");

        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User was not found");
        
        var credential = context.Credential;
        var credentialId = CredentialUtils.ToBase64String(credential.Id);

        var passkey = await _passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorCode.InvalidCredentials,
                Description = "Invalid credential"
            });
        }
        
        var savedChallenge = _httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorCode.InvalidChallenge,
                Description = "Invalid challenge"
            });
        }

        var passkeyResult = await _passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!passkeyResult.Succeeded) return passkeyResult;

        var requestEntity = new VerificationRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Action = context.Action,
            Purpose = context.Purpose,
            Status = VerificationStatus.Approved,
            Method = VerificationMethod.Passkey,
            ApprovedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configuration.Timestamp),
        };
        
        var verificationResult = await _verificationManager.CreateAsync(requestEntity, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var response = new VerificationResponse() { VerificationId = requestEntity.Id };
        return Results.Ok(response);
    }
}
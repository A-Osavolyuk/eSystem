using eSecurity.Core.Requests.Verification;
using eSecurity.Core.Responses;
using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Credentials.PublicKey.Credentials;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.WebAuthN.Constants;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Verification.SoftwareKey;

public sealed record VerifySoftwareKeyCommand(VerifySoftwareKeyRequest Request) : IRequest<Result>;

public sealed class VerifySoftwareKeyCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    IHttpContextAccessor httpContextAccessor,
    IVerificationManager verificationManager,
    IOptions<VerificationConfiguration> options) : IRequestHandler<VerifySoftwareKeyCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly VerificationConfiguration _configuration = options.Value;

    public async Task<Result> Handle(VerifySoftwareKeyCommand request, CancellationToken cancellationToken = default)
    {
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

        var credential = request.Request.Credential;
        var credentialId = CredentialUtils.ToBase64String(credential.Id);

        var passkey = await _passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidCredentials,
                Description = "Invalid credential"
            });
        }

        var savedChallenge = _httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidChallenge,
                Description = "Invalid challenge"
            });
        }

        var passkeyResult = await _passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!passkeyResult.Succeeded) return passkeyResult;

        var requestEntity = new VerificationRequestEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Action = request.Request.Action,
            Purpose = request.Request.Purpose,
            Status = VerificationStatus.Approved,
            Method = VerificationMethod.Passkey,
            ApprovedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configuration.Timestamp),
        };

        var verificationResult = await _verificationManager.CreateAsync(requestEntity, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var response = new VerificationResponse { VerificationId = requestEntity.Id };
        return Results.Success(SuccessCodes.Ok, response);
    }
}
using System.Text.Json.Serialization;
using eSecurity.Core.Requests.Verification;
using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Credentials.PublicKey.Credentials;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.WebAuthN;
using eSecurity.WebAuthN.Constants;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Verification.SoftwareKey;

public sealed record VerifySoftwareKeyCommand : IRequest<Result>
{
    [JsonPropertyName("credential")]
    public required PublicKeyCredential Credential { get; set; }
    
    [JsonPropertyName("operation_type")]
    public OperationType OperationType { get; set; }
}

public sealed class VerifySoftwareKeyCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IPasskeyManager passkeyManager,
    IHttpContextAccessor httpContextAccessor,
    IVerificationCommandService verificationCommandService,
    IOptions<VerificationConfiguration> options) : IRequestHandler<VerifySoftwareKeyCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly VerificationConfiguration _configuration = options.Value;

    public async Task<Result> Handle(VerifySoftwareKeyCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var credential = request.Credential;
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
            Operation = request.OperationType,
            Status = VerificationStatus.Approved,
            Method = VerificationMethod.Passkey,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configuration.Timestamp),
        };

        var verificationResult = await _verificationCommandService.CreateAsync(requestEntity, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var response = new VerificationResponse { VerificationId = requestEntity.Id };
        return Results.Success(SuccessCodes.Ok, response);
    }
}
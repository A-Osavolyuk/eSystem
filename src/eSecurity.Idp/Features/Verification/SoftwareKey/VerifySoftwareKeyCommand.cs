using System.Text.Json.Serialization;
using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Credentials.PublicKey.Challenge;
using eSecurity.Idp.Security.Credentials.PublicKey.Credentials;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.WebAuthN;
using eSecurity.WebAuthN.Constants;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.Verification.SoftwareKey;

public sealed record VerifySoftwareKeyCommand : IRequest<Result>
{
    [JsonPropertyName("credential")] 
    public PublicKeyCredential? Credential { get; set; } = null!;
    
    [JsonPropertyName("operation_type")]
    public OperationType OperationType { get; set; }
}

public sealed class VerifySoftwareKeyCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IHttpContextAccessor httpContextAccessor,
    IVerificationCommandService verificationCommandService,
    ISoftwareKeyQueryService softwareKeyQueryService,
    ISoftwareKeyCommandService softwareKeyCommandService,
    IOptions<VerificationConfiguration> options) : IRequestHandler<VerifySoftwareKeyCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly ISoftwareKeyQueryService _softwareKeyQueryService = softwareKeyQueryService;
    private readonly ISoftwareKeyCommandService _softwareKeyCommandService = softwareKeyCommandService;
    private readonly IOptions<VerificationConfiguration> _options = options;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly VerificationConfiguration _configuration = options.Value;

    public async Task<Result> Handle(VerifySoftwareKeyCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var credential = request.Credential;
        
        if (credential is null)
            throw new ValidationException("Credential is required");
        
        var credentialId = CredentialUtils.ToBase64String(credential.Id);
        var passkey = await _softwareKeyQueryService.GetByCredentialIdAsync(credentialId, cancellationToken);
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

        var passkeyResult = await _softwareKeyCommandService.VerifyAsync(passkey, 
            credential, savedChallenge, cancellationToken);
        
        if (!passkeyResult.Succeeded) 
            return passkeyResult;

        var requestEntity = new VerificationRequestEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Operation = request.OperationType,
            Status = VerificationStatus.Approved,
            Method = VerificationMethod.SoftwareKey,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configuration.Timestamp),
        };

        var verificationResult = await _verificationCommandService.CreateAsync(requestEntity, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var response = new VerificationResponse { VerificationId = requestEntity.Id };
        return Results.Success(SuccessCodes.Ok, response);
    }
}

public sealed class VerifySoftwareKeyCommandValidator : IRequestValidator<VerifySoftwareKeyCommand>
{
    public async ValueTask<Result> Validate(VerifySoftwareKeyCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.OperationType == OperationType.None)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'operation_type' is invalid"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}
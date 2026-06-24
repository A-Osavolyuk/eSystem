using System.Text.Json.Serialization;
using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Verification.EmailOtp;

public sealed record VerifyEmailOtpCommand : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
    
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("operation_type")]
    public required OperationType OperationType { get; set; }
}

public sealed class VerifyEmailOtpCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    ICodeManager codeManager,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService,
    IOptions<VerificationConfiguration> options) : IRequestHandler<VerifyEmailOtpCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly IOptions<VerificationConfiguration> _options = options;
    private readonly VerificationConfiguration _configuration = options.Value;

    public async Task<Result> Handle(VerifyEmailOtpCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var verificationRequest = await _verificationQueryService.GetByIdAsync(user.Id,
            request.VerificationId, cancellationToken);

        if (verificationRequest is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'verification_id' is invalid"
            });
        }

        if (verificationRequest.Status != VerificationStatus.Pending ||
            string.IsNullOrWhiteSpace(verificationRequest.Target))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid verification request"
            });
        }
        
        var code = await _codeManager.FindByCodeAsync(user, request.Code, cancellationToken);
        if (code is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Code is invalid"
            });
        }

        var codeResult = await _codeManager.ConsumeAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var verificationResult = await _verificationCommandService.ApproveAsync(verificationRequest, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var response = new VerificationResponse
        {
            VerificationId = verificationRequest.Id,
            ExpiresAt = verificationRequest.ExpiredAt
        };
        
        return Results.Success(SuccessCodes.Ok, response);
    }
}

public sealed class VerifyEmailOtpCommandValidator : IRequestValidator<VerifyEmailOtpCommand>
{
    public async ValueTask<Result> Validate(VerifyEmailOtpCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'code' is required"
            });
        }

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
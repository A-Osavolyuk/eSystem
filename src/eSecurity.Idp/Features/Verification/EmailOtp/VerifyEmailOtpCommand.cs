using eSecurity.Core.Requests.Verification;
using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Verification.EmailOtp;

public sealed record VerifyEmailOtpCommand(VerifyEmailOtpRequest Request) : IRequest<Result>;

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
        if (string.IsNullOrEmpty(request.Request.Code))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'code' is required"
            });
        }
        
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        var user = userResult.GetValue();
        var verificationRequest = await _verificationQueryService.GetByIdAsync(user.Id,
            request.Request.VerificationId, cancellationToken);

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
        
        var code = await _codeManager.FindByCodeAsync(user, request.Request.Code, cancellationToken);
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
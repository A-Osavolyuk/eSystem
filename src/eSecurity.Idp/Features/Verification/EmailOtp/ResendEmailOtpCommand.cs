using System.Text.Json.Serialization;
using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Verification.EmailOtp;

public sealed record ResendEmailOtpCommand : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
}

public sealed class ResendEmailOtpCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IUserResendAttemptsService resendAttemptsService,
    IEmailQueryService emailQueryService,
    IEmailService emailService,
    IVerificationQueryService verificationQueryService,
    ICodeManager codeManager) : IRequestHandler<ResendEmailOtpCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IUserResendAttemptsService _resendAttemptsService = resendAttemptsService;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly ICodeManager _codeManager = codeManager;

    public async Task<Result> Handle(ResendEmailOtpCommand request, CancellationToken cancellationToken = default)
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
        
        if (user.ResendAttempts >= 5)
        {
            return Results.Success(SuccessCodes.Ok, new ResendEmailOtpResponse()
            {
                VerificationId = verificationRequest.Id,
                ExpiresAt = verificationRequest.ExpiredAt,
                IsResendAvailable = false
            });
        }

        if (user.ResendAvailableAt > DateTimeOffset.UtcNow)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.SlowDown,
                Description = "Code resend request was sent to early"
            });
        }
        
        var targetEmail = await _emailQueryService.GetByEmailAsync(user.Id, 
            verificationRequest.Target, cancellationToken);
        
        if (targetEmail is null)
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
        
        var codeResult = await _codeManager.CreateAsync(user, SenderType.Email, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var error = codeResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }
        
        var emailContext = new EmailVerificationContext
        {
            Code = codeResult.GetValue(),
            Subject = "Access confirmation",
            To = targetEmail.Email
        };

        await _emailService.SendAsync(emailContext, cancellationToken);
        
        var updateResult = await _resendAttemptsService.ResetAttemptsAsync(user, 
            TimeSpan.FromMinutes(2), cancellationToken);
        
        if (!updateResult.Succeeded) 
            return updateResult;

        ResendEmailOtpResponse response;
        if (user.ResendAttempts == 5)
        {
            response = new ResendEmailOtpResponse()
            {
                VerificationId = verificationRequest.Id,
                ExpiresAt = verificationRequest.ExpiredAt,
                IsResendAvailable = false
            };
        }
        else
        {
            response = new ResendEmailOtpResponse()
            {
                VerificationId = verificationRequest.Id,
                ExpiresAt = verificationRequest.ExpiredAt,
                IsResendAvailable = true,
                ResendAvailableAt = user.ResendAvailableAt
            };
        }

        return Results.Success(SuccessCodes.Ok, response);
    }
}
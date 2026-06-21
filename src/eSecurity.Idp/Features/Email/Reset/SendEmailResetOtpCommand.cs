using eSecurity.Core.Requests.Email.Reset;
using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email.Reset;

public sealed record SendEmailResetOtpCommand(SendEmailResetOtpRequest Request) : IRequest<Result>;

public sealed class SendEmailResetCommandOtpHandler(
    ICurrentUserAccessor currentUserAccessor,
    IEmailQueryService emailQueryService,
    IEmailService emailService,
    ICodeManager codeManager,
    IUserResendAttemptsService resendAttemptsService,
    IEmailPolicy emailPolicy,
    IVerificationCommandService verificationCommandService) : IRequestHandler<SendEmailResetOtpCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IUserResendAttemptsService _resendAttemptsService = resendAttemptsService;
    private readonly IEmailPolicy _emailPolicy = emailPolicy;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(SendEmailResetOtpCommand request, CancellationToken cancellationToken = default)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var user = userResult.GetValue();
        if (string.IsNullOrWhiteSpace(request.Request.CurrentEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'current_email' is required"
            });
        }
        
        if (string.IsNullOrWhiteSpace(request.Request.NewEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'new_email' is required"
            });
        }

        if (await _emailQueryService.ExistsAsync(request.Request.NewEmail, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.EmailTaken,
                Description = "New email address is already taken"
            });
        }

        var currentEmail = await _emailQueryService.GetByEmailAsync(
            user.Id, request.Request.CurrentEmail, cancellationToken);

        if (currentEmail is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.EmailTaken,
                Description = "'current_email' is invalid"
            });
        }

        var canResetResult = _emailPolicy.CanReset(user.Id, currentEmail);
        if (!canResetResult.Succeeded) 
            return canResetResult;
        
        var resetAttemptsResult = await _resendAttemptsService.ResetAttemptsAsync(
            user, TimeSpan.FromMinutes(2), cancellationToken);

        if (!resetAttemptsResult.Succeeded)
            return resetAttemptsResult;

        var codeResult = await _codeManager.CreateAsync(user, SenderType.Email, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var error = codeResult.GetError();
            return Results.ServerError(ServerErrorCode.InsufficientStorage, error);
        }

        var verificationRequest = new VerificationRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Target = request.Request.NewEmail,
            Operation = OperationType.ResetEmail,
            Method = VerificationMethod.EmailOtp,
            Status = VerificationStatus.Pending,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        var verificationResult = await _verificationCommandService.CreateAsync(verificationRequest, cancellationToken);
        if (!verificationResult.Succeeded)
            return verificationResult;

        var emailContext = new EmailVerificationContext()
        {
            To = request.Request.NewEmail,
            Subject = "Email reset",
            Code = codeResult.GetValue()
        };

        await _emailService.SendAsync(emailContext, cancellationToken);

        var response = new SendEmailOtpResponse()
        {
            VerificationId = verificationRequest.Id,
            ExpiresAt = verificationRequest.ExpiredAt,
            MaxResendAttempts = 5,
            ResendInterval = (int)TimeSpan.FromMinutes(2).TotalSeconds
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}
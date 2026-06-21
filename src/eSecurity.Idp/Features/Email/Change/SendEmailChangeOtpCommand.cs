using eSecurity.Core.Requests.Email.Change;
using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Common.Validation;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email.Change;

public sealed record SendEmailChangeOtpCommand(SendEmailChangeOtpRequest OtpRequest) : IRequest<Result>;

public sealed class SendEmailChangeCommandOtpHandler(
    ICurrentUserAccessor currentUserAccessor,
    IEmailService emailService,
    IEmailQueryService emailQueryService,
    IUserResendAttemptsService resendAttemptsService,
    IVerificationCommandService verificationCommandService,
    ICodeManager codeManager) : IRequestHandler<SendEmailChangeOtpCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailService _emailService = emailService;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IUserResendAttemptsService _resendAttemptsService = resendAttemptsService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly ICodeManager _codeManager = codeManager;

    public async Task<Result> Handle(SendEmailChangeOtpCommand request, CancellationToken cancellationToken = default)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }

        if (string.IsNullOrEmpty(request.OtpRequest.NewEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "new_email is required"
            });
        }

        var newEmail = request.OtpRequest.NewEmail;
        var normalizedNewEmail = Normalizer.Normalize(newEmail);
        var userEmails = await _emailQueryService.ListByUserAsync(user.Id, cancellationToken);
        if (userEmails.Any(x => x.NormalizedEmail == normalizedNewEmail))
        {
            var email = userEmails.FirstOrDefault(x => x.NormalizedEmail == normalizedNewEmail);
            if (email is null)
            {
                return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
                {
                    Code = ErrorCode.ServerError,
                    Description = "Server error"
                });
            }

            if (email.Type != EmailType.Secondary)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "You can change your primary or recovery email only with your secondary email"
                });
            }

            if (!email.IsVerified)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.UnverifiedEmail,
                    Description = "New email address is not verified yet"
                });
            }
        }
        else
        {
            if (await _emailQueryService.ExistsAsync(newEmail, cancellationToken))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.EmailTaken,
                    Description = "This email address is already taken"
                });
            }
        }
        
        var codeResult = await _codeManager.CreateAsync(user, SenderType.Email, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        if (!codeResult.TryGetValue(out var code))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }

        var result = await _resendAttemptsService.ResetAttemptsAsync(user, TimeSpan.FromMinutes(2), cancellationToken);
        if (!result.Succeeded) return result;

        var verificationRequest = new VerificationRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Target = request.OtpRequest.NewEmail,
            Operation = OperationType.ChangeEmail,
            Method = VerificationMethod.EmailOtp,
            Status = VerificationStatus.Pending,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        var verificationResult = await _verificationCommandService.CreateAsync(verificationRequest, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var emailContext = new EmailVerificationContext()
        {
            Code = code,
            Subject = "New email verification",
            To = newEmail
        };

        await _emailService.SendAsync(emailContext, cancellationToken);
        
        var response = new SendEmailOtpResponse
        {
            VerificationId = verificationRequest.Id,
            ExpiresAt = verificationRequest.ExpiredAt,
            MaxResendAttempts = 5,
            ResendInterval = (int)TimeSpan.FromMinutes(2).TotalSeconds
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}
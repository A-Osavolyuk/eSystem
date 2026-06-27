using System.Text.Json.Serialization;
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

namespace eSecurity.Idp.Features.Email;

public sealed record SendEmailChangeOtpCommand : IRequest<Result>
{
    [JsonPropertyName("new_email")]
    public string? NewEmail { get; set; }
}

public sealed class SendEmailChangeCommandOtpHandler(
    ICurrentUserAccessor currentUserAccessor,
    IEmailService emailService,
    IEmailQueryService emailQueryService,
    ICodeCommandService codeCommandService,
    IUserResendAttemptsService resendAttemptsService,
    IVerificationCommandService verificationCommandService) : IRequestHandler<SendEmailChangeOtpCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailService _emailService = emailService;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly ICodeCommandService _codeCommandService = codeCommandService;
    private readonly IUserResendAttemptsService _resendAttemptsService = resendAttemptsService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(SendEmailChangeOtpCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(request.NewEmail))
            throw new ValidationException("NewEmail is required");
        
        var normalizedNewEmail = Normalizer.Normalize(request.NewEmail);
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
            if (await _emailQueryService.ExistsAsync(request.NewEmail, cancellationToken))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.EmailTaken,
                    Description = "This email address is already taken"
                });
            }
        }
        
        var codeResult = await _codeCommandService.CreateAsync(user.Id, SenderType.Email, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var error = codeResult.GetError();
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
            Target = request.NewEmail,
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
            To = request.NewEmail
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

public sealed class SendEmailChangeOtpCommandValidator : IRequestValidator<SendEmailChangeOtpCommand>
{
    public async ValueTask<Result> Validate(SendEmailChangeOtpCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.NewEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'new_email' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}
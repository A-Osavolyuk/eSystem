using System.Text.Json.Serialization;
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
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.Email;

public sealed record SendEmailResetOtpCommand : IRequest<Result>
{
    [JsonPropertyName("current_email")]
    public string? CurrentEmail { get; set; }
    
    [JsonPropertyName("new_email")]
    public string? NewEmail { get; set; }
}

public sealed class SendEmailResetCommandOtpHandler(
    ICurrentUserAccessor currentUserAccessor,
    IEmailQueryService emailQueryService,
    IEmailService emailService,
    ICodeCommandService codeCommandService,
    IUserResendAttemptsService resendAttemptsService,
    IVerificationCommandService verificationCommandService) : IRequestHandler<SendEmailResetOtpCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly ICodeCommandService _codeCommandService = codeCommandService;
    private readonly IUserResendAttemptsService _resendAttemptsService = resendAttemptsService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(SendEmailResetOtpCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(request.NewEmail))
            throw new ValidationException("NewEmail is required");
        
        if (await _emailQueryService.ExistsAsync(request.NewEmail, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.EmailTaken,
                Description = "New email address is already taken"
            });
        }

        if (string.IsNullOrWhiteSpace(request.CurrentEmail))
            throw new ValidationException("CurrentEmail is require");
        
        var currentEmail = await _emailQueryService.GetByEmailAsync(user.Id, request.CurrentEmail, cancellationToken);
        if (currentEmail is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.EmailTaken,
                Description = "'current_email' is invalid"
            });
        }
        
        var resetAttemptsResult = await _resendAttemptsService.ResetAttemptsAsync(
            user, TimeSpan.FromMinutes(2), cancellationToken);

        if (!resetAttemptsResult.Succeeded)
            return resetAttemptsResult;

        var codeResult = await _codeCommandService.CreateAsync(user.Id, SenderType.Email, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var error = codeResult.GetError();
            return Results.ServerError(ServerErrorCode.InsufficientStorage, error);
        }

        var verificationRequest = new VerificationRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Target = request.NewEmail,
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
            To = request.NewEmail,
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

public sealed class SendEmailResetOtpCommandValidator : IRequestValidator<SendEmailResetOtpCommand>
{
    public async ValueTask<Result> Validate(SendEmailResetOtpCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.CurrentEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'current_email' is required"
            });
        }
        
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
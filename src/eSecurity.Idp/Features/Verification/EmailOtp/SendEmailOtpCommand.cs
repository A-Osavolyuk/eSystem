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

namespace eSecurity.Idp.Features.Verification.EmailOtp;

public sealed record SendEmailOtpCommand : IRequest<Result>
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("operation_type")]
    public required OperationType OperationType { get; set; }
}

public sealed class SendEmailOtpCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IUserResendAttemptsService resendAttemptsService,
    IEmailService emailService,
    IEmailQueryService emailQueryService,
    IVerificationCommandService verificationCommandService,
    ICodeManager codeManager) : IRequestHandler<SendEmailOtpCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IUserResendAttemptsService _resendAttemptsService = resendAttemptsService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly ICodeManager _codeManager = codeManager;

    public async Task<Result> Handle(SendEmailOtpCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required");
        
        var userEmail = await _emailQueryService.GetByEmailAsync(user.Id, request.Email, cancellationToken);
        if (userEmail is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "Invalid email address"
            });
        }

        var updateResult = await _resendAttemptsService.CleanAttemptsAsync(user, cancellationToken);
        if (!updateResult.Succeeded) return updateResult;

        var codeResult = await _codeManager.CreateAsync(user, SenderType.Email, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var error = codeResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        var verificationRequest = new VerificationRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Operation = request.OperationType,
            Target = request.Email,
            Status = VerificationStatus.Pending,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        var verificationResult = await _verificationCommandService.CreateAsync(verificationRequest, cancellationToken);
        if (!verificationResult.Succeeded)
            return verificationResult;
        
        var emailContext = new EmailVerificationContext
        {
            Code = codeResult.GetValue(),
            Subject = "Access verification",
            To = userEmail.Email
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

public sealed class SendEmailOtpCommandValidator : IRequestValidator<SendEmailOtpCommand>
{
    public async ValueTask<Result> Validate(SendEmailOtpCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'email' is required"
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
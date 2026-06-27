using System.Text.Json.Serialization;
using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email;

public sealed record SendEmailVerificationOtpCommand : IRequest<Result>
{
    [JsonPropertyName("email")] 
    public string? Email { get; set; }
}

public sealed class SendEmailVerificationCommandOtpHandler(
    ICurrentUserAccessor currentUserAccessor,
    IUserResendAttemptsService userResendAttemptsService,
    IVerificationCommandService verificationCommandService,
    ICodeCommandService codeCommandService,
    IEmailService emailService) : IRequestHandler<SendEmailVerificationOtpCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IUserResendAttemptsService _userResendAttemptsService = userResendAttemptsService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly ICodeCommandService _codeCommandService = codeCommandService;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result> Handle(SendEmailVerificationOtpCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var result = await _userResendAttemptsService.ResetAttemptsAsync(user, 
            TimeSpan.FromMinutes(2), cancellationToken);
        
        if (!result.Succeeded) 
            return result;
        
        var codeResult = await _codeCommandService.CreateAsync(user.Id, SenderType.Email, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var error = codeResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        var verificationRequest = new VerificationRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Target = request.Email,
            Operation = OperationType.VerifyEmail,
            Method = VerificationMethod.EmailOtp,
            Status = VerificationStatus.Pending,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        var verificationResult = await _verificationCommandService.CreateAsync(verificationRequest, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required");
        
        var emailContext = new EmailVerificationContext()
        {
            Code = codeResult.GetValue(),
            Subject = "Email verification",
            To = request.Email
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

public sealed class SendEmailVerificationOtpCommandValidator : IRequestValidator<SendEmailVerificationOtpCommand>
{
    public async ValueTask<Result> Validate(SendEmailVerificationOtpCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'current_email' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}
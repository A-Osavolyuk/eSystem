using System.Text.Json.Serialization;
using eSecurity.Core.Requests.Email.Verification;
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

namespace eSecurity.Idp.Features.Email.Verification;

public sealed record SendEmailVerificationOtpCommand : IRequest<Result>
{
    [JsonPropertyName("email")] 
    public required string Email { get; set; }
}

public sealed class SendEmailVerificationCommandOtpHandler(
    ICurrentUserAccessor currentUserAccessor,
    IUserResendAttemptsService userResendAttemptsService,
    ICodeManager codeManager,
    IVerificationCommandService verificationCommandService,
    IEmailService emailService) : IRequestHandler<SendEmailVerificationOtpCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IUserResendAttemptsService _userResendAttemptsService = userResendAttemptsService;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result> Handle(SendEmailVerificationOtpCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var result = await _userResendAttemptsService.ResetAttemptsAsync(user, 
            TimeSpan.FromMinutes(2), cancellationToken);
        
        if (!result.Succeeded) 
            return result;
        
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
            Target = request.Email,
            Operation = OperationType.VerifyEmail,
            Method = VerificationMethod.EmailOtp,
            Status = VerificationStatus.Pending,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        var verificationResult = await _verificationCommandService.CreateAsync(verificationRequest, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;
        
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
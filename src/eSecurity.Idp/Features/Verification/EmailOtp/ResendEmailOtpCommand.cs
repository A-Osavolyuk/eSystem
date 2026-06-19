using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Verification.EmailOtp;

public sealed record ResendEmailOtpCommand : IRequest<Result>;

public sealed class ResendEmailOtpCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IUserResendAttemptsService resendAttemptsService,
    IEmailQueryService emailQueryService,
    IEmailService emailService,
    ICodeManager codeManager) : IRequestHandler<ResendEmailOtpCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IUserResendAttemptsService _resendAttemptsService = resendAttemptsService;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly ICodeManager _codeManager = codeManager;

    public async Task<Result> Handle(ResendEmailOtpCommand request, CancellationToken cancellationToken = default)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        if (user.ResendAttempts >= 5)
        {
            return Results.Success(SuccessCodes.Ok, new ResendEmailOtpResponse()
            {
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
        
        var primaryEmail = await _emailQueryService.GetByTypeAsync(user.Id, EmailType.Primary, cancellationToken);
        if (primaryEmail is null)
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

        if (!codeResult.TryGetValue(out var code))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }

        var emailContext = new EmailVerificationContext
        {
            Code = code,
            Subject = "Access confirmation",
            To = primaryEmail.Email
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
                IsResendAvailable = false
            };
        }
        else
        {
            response = new ResendEmailOtpResponse()
            {
                IsResendAvailable = true,
                ResendAvailableAt = user.ResendAvailableAt
            };
        }

        return Results.Success(SuccessCodes.Ok, response);
    }
}
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

public sealed class SendEmailOtpCommand : IRequest<Result>;

public sealed class SendEmailOtpCommandHandler(
    IUserManager userManager,
    IEmailService emailService,
    IEmailManager emailManager,
    ICodeManager codeManager) : IRequestHandler<SendEmailOtpCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailService _emailService = emailService;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ICodeManager _codeManager = codeManager;

    public async Task<Result> Handle(SendEmailOtpCommand request, CancellationToken cancellationToken = default)
    {
        var userResult = await _userManager.GetUserAsync(cancellationToken);
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

        var primaryEmail = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (primaryEmail is null)
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }

        user.ResendAttempts = 0;
        user.ResendAvailableAt = null;

        var updateResult = await _userManager.UpdateAsync(user, cancellationToken);
        if (!updateResult.Succeeded) return updateResult;

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
            Subject = "Access verification",
            To = primaryEmail.Email
        };

        await _emailService.SendAsync(emailContext, cancellationToken);

        var response = new SendEmailOtpResponse
        {
            MaxResendAttempts = 5,
            ResendInterval = (int)TimeSpan.FromMinutes(2).TotalSeconds
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}
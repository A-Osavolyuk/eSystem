using eSecurity.Core.Requests.Email.Verification;
using eSecurity.Core.Responses;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email.Verification;

public sealed record ResendEmailVerificationCommand(ResendEmailVerificationRequest Request) : IRequest<Result>;

public sealed class ResendEmailVerificationCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IEmailService emailService,
    ICodeManager codeManager) : IRequestHandler<ResendEmailVerificationCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IEmailService _emailService = emailService;
    private readonly ICodeManager _codeManager = codeManager;

    public async Task<Result> Handle(ResendEmailVerificationCommand request,
        CancellationToken cancellationToken = default)
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

        var email = request.Request.Email;
        if (string.IsNullOrEmpty(email))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Email is required"
            });
        }

        var ownEmail = await _emailManager.OwnAsync(user, email, cancellationToken);
        if (!ownEmail)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Email is invalid"
            });
        }
        
        if (user.ResendAttempts >= 5)
        {
            return Results.Success(SuccessCodes.Ok, new CodeResendResponse()
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

        var emailContext = new EmailVerificationContext()
        {
            Code = code,
            Subject = "Email verification",
            To = email
        };

        await _emailService.SendAsync(emailContext, cancellationToken);

        var result = await _userManager.AddResendAttemptAsync(user, TimeSpan.FromMinutes(2), cancellationToken);
        if (!result.Succeeded) return result;

        CodeResendResponse response;
        if (user.ResendAttempts == 5)
        {
            response = new CodeResendResponse
            {
                IsResendAvailable = false
            };
        }
        else
        {
            response = new CodeResendResponse
            {
                IsResendAvailable = true,
                ResendAvailableAt = user.ResendAvailableAt
            };
        }

        return Results.Success(SuccessCodes.Ok, response);
    }
}
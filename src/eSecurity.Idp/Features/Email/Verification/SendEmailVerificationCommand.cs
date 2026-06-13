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

public sealed record SendEmailVerificationCommand(SendEmailVerificationRequest Request) : IRequest<Result>;

public sealed class SendEmailVerificationCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    ICodeManager codeManager,
    IEmailService emailService) : IRequestHandler<SendEmailVerificationCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result> Handle(SendEmailVerificationCommand request,
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

        var result = await _userManager.ResetResendAttemptsAsync(user, TimeSpan.FromMinutes(2), cancellationToken);
        if (!result.Succeeded) return result;

        var emailContext = new EmailVerificationContext()
        {
            Code = code,
            Subject = "Email verification",
            To = email
        };

        await _emailService.SendAsync(emailContext, cancellationToken);

        var response = new CodeSendResponse()
        {
            MaxResendAttempts = 5,
            ResendInterval = (int)TimeSpan.FromMinutes(2).TotalSeconds
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}
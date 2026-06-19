using eSecurity.Core.Requests.Email.Change;
using eSecurity.Core.Responses;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Common.Validation;
using eSecurity.Idp.Data.Seeding;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email.Change;

public sealed record SendEmailChangeCommand(SendEmailChangeRequest Request) : IRequest<Result>;

public sealed class SendEmailChangeCommandHandler(
    IUserManager userManager,
    IEmailService emailService,
    IEmailQueryService emailQueryService,
    ICodeManager codeManager) : IRequestHandler<SendEmailChangeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailService _emailService = emailService;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly ICodeManager _codeManager = codeManager;

    public async Task<Result> Handle(SendEmailChangeCommand request, CancellationToken cancellationToken = default)
    {
        var userResult = await _userManager.GetUserAsync(cancellationToken);
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

        if (string.IsNullOrEmpty(request.Request.NewEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "new_email is required"
            });
        }

        var newEmail = request.Request.NewEmail;
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

        var result = await _userManager.ResetResendAttemptsAsync(user, TimeSpan.FromMinutes(2), cancellationToken);
        if (!result.Succeeded) return result;

        var emailContext = new EmailVerificationContext()
        {
            Code = code,
            Subject = "New email verification",
            To = newEmail
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
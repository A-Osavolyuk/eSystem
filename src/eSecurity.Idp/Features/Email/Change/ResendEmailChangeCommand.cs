using eSecurity.Core.Requests.Email.Change;
using eSecurity.Core.Responses.Email.Change;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email.Change;

public sealed record ResendEmailChangeCommand(ResendEmailChangeRequest Request) : IRequest<Result>;

public sealed class ResendEmailChangeCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IEmailService emailService,
    ICodeManager codeManager) : IRequestHandler<ResendEmailChangeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IEmailService _emailService = emailService;
    private readonly ICodeManager _codeManager = codeManager;

    public async Task<Result> Handle(ResendEmailChangeCommand request, CancellationToken cancellationToken = default)
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
        if (await _emailManager.OwnAsync(user, request.Request.NewEmail, cancellationToken))
        {
            var email = await _emailManager.FindByEmailAsync(user, request.Request.NewEmail, cancellationToken);
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
            if (await _emailManager.IsTakenAsync(request.Request.NewEmail, cancellationToken))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.EmailTaken,
                    Description = "This email address is already taken"
                });
            }
        }

        if (user.ResendAttempts >= 5)
        {
            return Results.Success(SuccessCodes.Ok, new ResendEmailChangeResponse()
            {
                IsResendAvailable = false
            });
        }

        if (user.ResendAvailableAt > DateTimeOffset.UtcNow)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.SlowDown,
                Description = "Resend request was sent to early"
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
            Subject = "New email verification",
            To = newEmail
        };

        await _emailService.SendAsync(emailContext, cancellationToken);

        var result = await _userManager.AddResendAttemptAsync(user, TimeSpan.FromMinutes(2), cancellationToken);
        if (!result.Succeeded) return result;

        ResendEmailChangeResponse response;
        if (user.ResendAttempts == 5)
        {
            response = new ResendEmailChangeResponse
            {
                IsResendAvailable = false
            };
        }
        else
        {
            response = new ResendEmailChangeResponse
            {
                IsResendAvailable = true,
                ResendAvailableAt = user.ResendAvailableAt
            };
        }

        return Results.Success(SuccessCodes.Ok, response);
    }
}
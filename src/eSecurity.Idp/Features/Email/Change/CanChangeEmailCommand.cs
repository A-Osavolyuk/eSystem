using eSecurity.Core.Requests.Email.Change;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email.Change;

public sealed record CanChangeEmailCommand(CanChangeEmailRequest Request) : IRequest<Result>;

public sealed class CanChangeEmailCommandHandler(
    IUserManager userManager, 
    IEmailManager emailManager,
    ILinkedAccountManager linkedAccountManager) : IRequestHandler<CanChangeEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;

    public async Task<Result> Handle(CanChangeEmailCommand request, CancellationToken cancellationToken = default)
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

        if (string.IsNullOrEmpty(request.Request.CurrentEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'current_email' is required"
            });
        }
        
        if (string.IsNullOrEmpty(request.Request.NewEmail))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'new_email' is required"
            });
        }

        var currentEmail = await _emailManager.FindByEmailAsync(user, request.Request.CurrentEmail, cancellationToken);
        if (currentEmail is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'current_email' is invalid"
            });
        }

        if (!currentEmail.IsVerified)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.UnverifiedEmail,
                Description = "Current email address is not verified yet"
            });
        }

        if (currentEmail.Type == EmailType.Primary && await _linkedAccountManager.HasAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "You cannot change primary email, while you have linked external accounts"
            });
        }

        if (currentEmail.Type == EmailType.Recovery)
        {
            if (!await _emailManager.HasAsync(user, EmailType.Primary, cancellationToken))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.BadRequest,
                    Description = "You cannot change recovery email if you do not have primary one"
                });
            }
        }

        if (currentEmail.Type == EmailType.Secondary)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "You cannot change secondary email"
            });
        }

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
        }
        else
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "User does not own this email"
            });
        }

        return Results.Success(SuccessCodes.Ok);
    }
}
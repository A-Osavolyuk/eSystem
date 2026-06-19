using eSecurity.Core.Requests.Email.Change;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Validation;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email.Change;

public sealed record CanChangeEmailCommand(CanChangeEmailRequest Request) : IRequest<Result>;

public sealed class CanChangeEmailCommandHandler(
    IEmailQueryService emailQueryService,
    ICurrentUserAccessor currentUserAccessor,
    ILinkedAccountManager linkedAccountManager) : IRequestHandler<CanChangeEmailCommand, Result>
{
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;

    public async Task<Result> Handle(CanChangeEmailCommand request, CancellationToken cancellationToken = default)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
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

        var userEmails = await _emailQueryService.ListByUserAsync(user.Id, cancellationToken);
        var currentNormalizedEmail = Normalizer.Normalize(request.Request.CurrentEmail);
        var currentEmail = userEmails.FirstOrDefault(x => x.NormalizedEmail == currentNormalizedEmail);
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
            if (userEmails.All(x => x.Type != EmailType.Primary))
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

        var newNormalizedEmail = Normalizer.Normalize(request.Request.NewEmail);
        if (userEmails.Any(x => x.NormalizedEmail == newNormalizedEmail))
        {
            var email = userEmails.FirstOrDefault(x => x.NormalizedEmail == newNormalizedEmail);
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
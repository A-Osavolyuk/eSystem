using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Account.Commands;

public record CheckAccountCommand(CheckAccountRequest Request) : IRequest<Result>;

public class CheckAccountCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    ILockoutManager lockoutManager) : IRequestHandler<CheckAccountCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;

    public async Task<Result> Handle(CheckAccountCommand request, CancellationToken cancellationToken)
    {
        CheckAccountResponse? response;

        var user = await _userManager.FindByLoginAsync(request.Request.Login, cancellationToken);
        if (user is null)
        {
            response = new CheckAccountResponse { Exists = false };
            return Results.Ok(response);
        }

        var lockoutState = await _lockoutManager.GetAsync(user, cancellationToken);
        if (lockoutState is null)
        {
            return Results.NotFound(new Error
            {
                Code = ErrorTypes.Common.InvalidLockoutState,
                Description = "Invalid state"
            });
        }

        if (lockoutState.Enabled)
        {
            response = new CheckAccountResponse
            {
                Exists = true,
                UserId = user.Id,
                IsLockedOut = lockoutState.Enabled,
            };

            return Results.Ok(response);
        }

        var email = await _emailManager.FindByTypeAsync(user, EmailType.Recovery, cancellationToken);
        if (email is null)
        {
            response = new CheckAccountResponse
            {
                Exists = true,
                UserId = user.Id,
                HasRecoveryEmail = false,
            };

            return Results.Ok(response);
        }

        response = new CheckAccountResponse
        {
            Exists = true,
            UserId = user.Id,
            HasRecoveryEmail = true,
            RecoveryEmail = email.Email
        };

        return Results.Ok(response);
    }
}
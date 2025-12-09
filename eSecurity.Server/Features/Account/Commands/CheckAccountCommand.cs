using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Account.Commands;

public record CheckAccountCommand(CheckAccountRequest Request) : IRequest<Result>;

public class CheckAccountCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager) : IRequestHandler<CheckAccountCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;

    public async Task<Result> Handle(CheckAccountCommand request, CancellationToken cancellationToken)
    {
        CheckAccountResponse? response;
        
        var user = await _userManager.FindByUsernameAsync(request.Request.Username, cancellationToken);

        if (user is null)
        {
            response = new CheckAccountResponse { Exists = false };
            return Results.Ok(response);
        }

        if (user.LockoutState.Enabled)
        {
            response = new CheckAccountResponse
            {
                Exists = true,
                UserId = user.Id,
                IsLockedOut = user.LockoutState.Enabled,
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
using eSecurity.Common.Responses;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Identity.Email;

namespace eSecurity.Features.Security.Commands;

public record CheckAccountCommand() : IRequest<Result>
{
    public string Username { get; set; } = string.Empty;
}

public class CheckAccountCommandHandler(IUserManager userManager) : IRequestHandler<CheckAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(CheckAccountCommand request, CancellationToken cancellationToken)
    {
        CheckAccountResponse? response;
        
        var user = await userManager.FindByUsernameAsync(request.Username, cancellationToken);

        if (user is null)
        {
            response = new CheckAccountResponse { Exists = false };
            return Result.Success(response);
        }

        if (user.LockoutState.Enabled)
        {
            response = new CheckAccountResponse
            {
                Exists = true,
                UserId = user.Id,
                IsLockedOut = user.LockoutState.Enabled,
            };
            
            return Result.Success(response);
        }
        
        var email = user.Emails.FirstOrDefault(x => x is { Type: EmailType.Recovery, IsVerified: true });

        if (email is null)
        {
            response = new CheckAccountResponse
            {
                Exists = true,
                UserId = user.Id,
                HasRecoveryEmail = false,
            };
            
            return Result.Success(response);
        }
        
        response = new CheckAccountResponse
        {
            Exists = true,
            UserId = user.Id,
            HasRecoveryEmail = true,
            RecoveryEmail = email.Email
        };
        
        return Result.Success(response);
    }
}
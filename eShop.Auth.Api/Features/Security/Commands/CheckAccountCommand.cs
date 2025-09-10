using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record CheckAccountCommand(CheckAccountRequest Request) : IRequest<Result>;

public class CheckAccountCommandHandler(IUserManager userManager) : IRequestHandler<CheckAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(CheckAccountCommand request, CancellationToken cancellationToken)
    {
        CheckAccountResponse? response;
        
        var user = await userManager.FindByUsernameAsync(request.Request.Username, cancellationToken);

        if (user is null)
        {
            response = new CheckAccountResponse { Exists = false };
            return Result.Success(response);
        }
        
        response = new CheckAccountResponse
        {
            Exists = true,
            UserId = user.Id,
            IsLockedOut = user.LockoutState.Enabled
        };
        return Result.Success(response);
    }
}
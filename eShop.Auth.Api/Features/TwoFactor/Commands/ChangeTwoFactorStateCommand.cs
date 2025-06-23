using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public sealed record ChangeTwoFactorStateCommand(ChangeTwoFactorStateRequest Request)
    : IRequest<Result>;

public sealed class ChangeTwoFactorAuthenticationStateCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager)
    : IRequestHandler<ChangeTwoFactorStateCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(
        ChangeTwoFactorStateCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        if (user.TwoFactorEnabled)
        {
            await twoFactorManager.DisableAsync(user, cancellationToken);
            
            return Result.Success(new ChangeTwoFactorStateResponse()
            {
                Message = $"Two factor authentication was successfully disabled.",
                TwoFactorAuthenticationState = false
            });
        }
        else
        {
            await twoFactorManager.EnableAsync(user, cancellationToken);
            
            return Result.Success(new ChangeTwoFactorStateResponse()
            {
                Message = $"Two factor authentication was successfully enabled.",
                TwoFactorAuthenticationState = true,
            });
        }
    }
}
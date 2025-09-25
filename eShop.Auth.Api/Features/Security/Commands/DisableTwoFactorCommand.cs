using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record DisableTwoFactorCommand(DisableTwoFactorRequest Request) : IRequest<Result>;

public class DisableTwoFactorCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager) : IRequestHandler<DisableTwoFactorCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (!user.TwoFactorEnabled) return Results.BadRequest("2FA already disabled.");

        user.TwoFactorEnabled = false;
        var userResult = await userManager.UpdateAsync(user, cancellationToken);
        if (!userResult.Succeeded) return userResult;

        var providers = user.TwoFactorProviders.ToList();
        var providerResult = await twoFactorManager.UnsubscribeAsync(providers, cancellationToken);
        return providerResult;
    }
}
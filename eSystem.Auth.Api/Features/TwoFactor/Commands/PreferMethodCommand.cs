using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Features.TwoFactor.Commands;

public record PreferMethodCommand(PreferTwoFactorMethodRequest Request) : IRequest<Result>;

public class PreferMethodCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager) : IRequestHandler<PreferMethodCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(PreferMethodCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        
        var result = await twoFactorManager.PreferAsync(user, request.Request.PreferredMethod, cancellationToken);
        return result;
    }
}
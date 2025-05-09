using eShop.Domain.Requests.API.TwoFactor;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record UnsubscribeProviderCommand(UnsubscribeProviderRequest Request) : IRequest<Result>;

public class UnsubscribeProviderCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager) : IRequestHandler<UnsubscribeProviderCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    
    public async Task<Result> Handle(UnsubscribeProviderCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }
        
        var provider = await twoFactorManager.GetProviderAsync(request.Request.Provider, cancellationToken);
        
        if (provider is null)
        {
            return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        }
        
        var result = await twoFactorManager.UnsubscribeAsync(user, provider, cancellationToken);
        return result;
    }
}
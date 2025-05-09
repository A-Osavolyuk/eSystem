using eShop.Domain.Requests.API.TwoFactor;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record UnsubscribeProviderCommand(UnsubscribeProviderRequest Request) : IRequest<Result>;

public class UnsubscribeProviderCommandHandler(
    IUserManager userManager,
    IProviderManager providerManager) : IRequestHandler<UnsubscribeProviderCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IProviderManager providerManager = providerManager;

    public async Task<Result> Handle(UnsubscribeProviderCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }
        
        var provider = await providerManager.FindAsync(request.Request.Provider, cancellationToken);
        
        if (provider is null)
        {
            return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        }
        
        var result = await providerManager.UnsubscribeAsync(user, provider, cancellationToken);
        return result;
    }
}
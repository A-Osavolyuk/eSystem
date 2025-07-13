using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Providers.Commands;

public record SubscribeProviderCommand(SubscribeProviderRequest Request) : IRequest<Result>;

public class SubscribeProviderCommandHandler(
    IUserManager userManager,
    IProviderManager providerManager) : IRequestHandler<SubscribeProviderCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IProviderManager providerManager = providerManager;

    public async Task<Result> Handle(SubscribeProviderCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var provider = await providerManager.FindAsync(request.Request.Provider, cancellationToken);

        if (provider is null)
        {
            return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        }
        
        var result = await providerManager.SubscribeAsync(user, provider, cancellationToken);

        return result;
    }
}
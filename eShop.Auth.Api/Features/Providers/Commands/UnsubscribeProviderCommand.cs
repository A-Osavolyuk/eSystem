using eShop.Domain.Common.Security;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Providers.Commands;

public record UnsubscribeProviderCommand(UnsubscribeProviderRequest Request) : IRequest<Result>;

public class UnsubscribeProviderCommandHandler(
    IUserManager userManager,
    IProviderManager providerManager,
    ISecretManager secretManager) : IRequestHandler<UnsubscribeProviderCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IProviderManager providerManager = providerManager;
    private readonly ISecretManager secretManager = secretManager;

    public async Task<Result> Handle(UnsubscribeProviderCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");

        if (provider.Name == ProviderTypes.Authenticator)
        {
            var secretResult = await secretManager.RemoveAsync(user, cancellationToken);
            if (!secretResult.Succeeded) return secretResult;
        }
        
        var result = await providerManager.UnsubscribeAsync(user, provider, cancellationToken);
        return result;
    }
}
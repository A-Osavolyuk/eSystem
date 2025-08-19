using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.OAuth.Commands;

public record DisconnectLinkedAccountCommand(DisconnectLinkedAccountRequest Request) : IRequest<Result>;

public class DisconnectLinkedAccountCommandHandler(
    IUserManager userManager,
    IOAuthProviderManager providerManager) : IRequestHandler<DisconnectLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthProviderManager providerManager = providerManager;

    public async Task<Result> Handle(DisconnectLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var provider = await providerManager.FindByIdAsync(request.Request.ProviderId, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with ID {request.Request.ProviderId}.");

        var result = await providerManager.DisconnectAsync(user, provider, cancellationToken);
        return result;
    }
}
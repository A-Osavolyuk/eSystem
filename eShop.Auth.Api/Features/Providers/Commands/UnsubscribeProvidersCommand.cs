using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Providers.Commands;

public record UnsubscribeProvidersCommand(UnsubscribeProvidersRequest Request) : IRequest<Result>;

public class UnsubscribeProvidersCommandHandler(
    IUserManager userManager,
    IProviderManager providerManager) : IRequestHandler<UnsubscribeProvidersCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IProviderManager providerManager = providerManager;

    public async Task<Result> Handle(UnsubscribeProvidersCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }
        
        var result = await providerManager.UnsubscribeAsync(user, cancellationToken);
        
        return result;
    }
}
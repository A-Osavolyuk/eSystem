using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.OAuth.Commands;

public record AllowLinkedAccountCommand(AllowLinkedAccountRequest Request) : IRequest<Result>;

public class AllowLinkedAccountCommandHandler(
    IUserManager userManager,
    IOAuthProviderManager providerManager) : IRequestHandler<AllowLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthProviderManager providerManager = providerManager;

    public async Task<Result> Handle(AllowLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var provider = await providerManager.FindByIdAsync(request.Request.ProviderId, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with ID {request.Request.ProviderId}.");

        var result = await providerManager.AllowAsync(user, provider, cancellationToken);
        return result;
    }
}
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Providers.Commands;

public record VerifyProviderCommand(VerifyProviderRequest Request) : IRequest<Result>;

public class VerifyProviderCommandHandler(
    IUserManager userManager,
    IProviderManager providerManager,
    ILoginCodeManager loginCodeManager) : IRequestHandler<VerifyProviderCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IProviderManager providerManager = providerManager;
    private readonly ILoginCodeManager loginCodeManager = loginCodeManager;

    public async Task<Result> Handle(VerifyProviderCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");

        var code = request.Request.Code;

        var verifyResult = await loginCodeManager.VerifyAsync(user, provider, code, cancellationToken);
        if (!verifyResult.Succeeded) return verifyResult;

        var result = await providerManager.SubscribeAsync(user, provider, cancellationToken);

        return result;
    }
}
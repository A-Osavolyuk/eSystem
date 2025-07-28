using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record VerifyTwoFactorCodeCommand(VerifyTwoFactorCodeRequest Request) : IRequest<Result>;

public class VerifyTwoFactorCodeCommandHandler(
    IUserManager userManager,
    ILoginTokenManager loginTokenManager,
    IProviderManager providerManager) : IRequestHandler<VerifyTwoFactorCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILoginTokenManager loginTokenManager = loginTokenManager;
    private readonly IProviderManager providerManager = providerManager;

    public async Task<Result> Handle(VerifyTwoFactorCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);

        if (provider is null)
        {
            return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        }
        
        var result = await loginTokenManager.VerifyAsync(user, provider, request.Request.Code, cancellationToken);
        
        return result;
    }
}
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Providers.Commands;

public record UnsubscribeProviderCommand(UnsubscribeProviderRequest Request) : IRequest<Result>;

public class UnsubscribeProviderCommandHandler(
    IUserManager userManager,
    ITwoFactorProviderManager twoFactorProviderManager,
    IVerificationManager verificationManager,
    ISecretManager secretManager) : IRequestHandler<UnsubscribeProviderCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorProviderManager twoFactorProviderManager = twoFactorProviderManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly ISecretManager secretManager = secretManager;

    public async Task<Result> Handle(UnsubscribeProviderCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var provider = await twoFactorProviderManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");

        var verificationResult = await verificationManager.VerifyAsync(user, 
            CodeResource.Provider, CodeType.Unsubscribe, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        if (provider.Name == ProviderTypes.Authenticator)
        {
            var secretResult = await secretManager.RemoveAsync(user, cancellationToken);
            if (!secretResult.Succeeded) return secretResult;
        }
        
        var result = await twoFactorProviderManager.UnsubscribeAsync(user, provider, cancellationToken);
        return result;
    }
}
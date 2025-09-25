using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record UnsubscribeProviderCommand(UnsubscribeProviderRequest Request) : IRequest<Result>;

public class UnsubscribeProviderCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IVerificationManager verificationManager,
    ILoginMethodManager loginMethodManager,
    ISecretManager secretManager) : IRequestHandler<UnsubscribeProviderCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly ILoginMethodManager loginMethodManager = loginMethodManager;
    private readonly ISecretManager secretManager = secretManager;

    public async Task<Result> Handle(UnsubscribeProviderCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var provider = await twoFactorManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");

        var verificationResult = await verificationManager.VerifyAsync(user, 
            CodeResource.Provider, CodeType.Unsubscribe, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        if (provider.Name == ProviderTypes.Authenticator)
        {
            var secretResult = await secretManager.RemoveAsync(user, cancellationToken);
            if (!secretResult.Succeeded) return secretResult;
        }

        if (user.TwoFactorProviders.Count == 1)
        {
            var method = user.GetLoginMethod(LoginType.OAuth);
            var methodResult = await loginMethodManager.RemoveAsync(method, cancellationToken);
            if (!methodResult.Succeeded) return methodResult;
        }
        
        var userProvider = user.TwoFactorProviders.First(
            x => x.TwoFactorProvider.Name == provider.Name);
        
        var result = await twoFactorManager.UnsubscribeAsync([userProvider], cancellationToken);
        return result;
    }
}
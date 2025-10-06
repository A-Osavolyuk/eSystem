using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public record DisallowLinkedAccountCommand(DisallowLinkedAccountRequest Request) : IRequest<Result>;

public class DisallowLinkedAccountCommandHandler(
    IUserManager userManager,
    IOAuthProviderManager providerManager,
    IVerificationManager verificationManager) : IRequestHandler<DisallowLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(DisallowLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with ID {request.Request.Provider}.");

        var verificationResult = await verificationManager.VerifyAsync(user, 
            CodeResource.LinkedAccount, CodeType.Disallow, cancellationToken);
        
        if(!verificationResult.Succeeded) return verificationResult;

        var result = await providerManager.DisallowAsync(user, provider, cancellationToken);
        return result;
    }
}
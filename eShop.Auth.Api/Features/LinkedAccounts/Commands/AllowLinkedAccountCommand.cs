using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public record AllowLinkedAccountCommand(AllowLinkedAccountRequest Request) : IRequest<Result>;

public class AllowLinkedAccountCommandHandler(
    IUserManager userManager,
    IOAuthProviderManager providerManager,
    IVerificationManager verificationManager) : IRequestHandler<AllowLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(AllowLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider {request.Request.Provider}.");

        var verificationResult = await verificationManager.VerifyAsync(user, 
            CodeResource.LinkedAccount, CodeType.Allow, cancellationToken);
        
        if(!verificationResult.Succeeded) return verificationResult;

        var result = await providerManager.AllowAsync(user, provider, cancellationToken);
        return result;
    }
}
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public record DisconnectLinkedAccountCommand(DisconnectLinkedAccountRequest Request) : IRequest<Result>;

public class DisconnectLinkedAccountCommandHandler(
    IUserManager userManager,
    IOAuthProviderManager providerManager,
    IVerificationManager verificationManager) : IRequestHandler<DisconnectLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(DisconnectLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with ID {request.Request.Provider}.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.LinkedAccount, ActionType.Disconnect, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var result = await providerManager.DisconnectAsync(user, provider, cancellationToken);
        return result;
    }
}
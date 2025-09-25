using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public record DisconnectLinkedAccountCommand(DisconnectLinkedAccountRequest Request) : IRequest<Result>;

public class DisconnectLinkedAccountCommandHandler(
    IUserManager userManager,
    IOAuthProviderManager providerManager,
    IVerificationManager verificationManager,
    ILoginMethodManager loginMethodManager) : IRequestHandler<DisconnectLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly ILoginMethodManager loginMethodManager = loginMethodManager;

    public async Task<Result> Handle(DisconnectLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with ID {request.Request.Provider}.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            CodeResource.LinkedAccount, CodeType.Disconnect, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        if (user.LinkedAccounts.Count == 1)
        {
            var method = user.GetLoginMethod(LoginType.OAuth);
            var methodResult = await loginMethodManager.RemoveAsync(method, cancellationToken);
            if (!methodResult.Succeeded) return methodResult;
        }

        var result = await providerManager.DisconnectAsync(user, provider, cancellationToken);
        return result;
    }
}
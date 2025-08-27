using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public record ConfirmDisconnectLinkedAccountCommand(ConfirmDisconnectLinkedAccountRequest Request) : IRequest<Result>;

public class ConfirmDisconnectLinkedAccountCommandHandler(
    IUserManager userManager,
    IOAuthProviderManager providerManager,
    ICodeManager codeManager) : IRequestHandler<ConfirmDisconnectLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ConfirmDisconnectLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with ID {request.Request.Provider}.");

        var code = request.Request.Code;
        var codeResult = await codeManager.VerifyAsync(user, code, SenderType.Email,
            CodeType.Disconnect, CodeResource.LinkedAccount, cancellationToken);

        if (!codeResult.Succeeded) return codeResult;
        
        var result = await providerManager.DisconnectAsync(user, provider, cancellationToken);
        return result;
    }
}
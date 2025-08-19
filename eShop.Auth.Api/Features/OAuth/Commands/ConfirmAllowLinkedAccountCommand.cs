using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.OAuth.Commands;

public record ConfirmAllowLinkedAccountCommand(ConfirmAllowLinkedAccountRequest Request) : IRequest<Result>;

public class ConfirmAllowLinkedAccountCommandHandler(
    IUserManager userManager,
    IOAuthProviderManager providerManager,
    ICodeManager codeManager) : IRequestHandler<ConfirmAllowLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ConfirmAllowLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider {request.Request.Provider}.");

        var code = request.Request.Code;
        var codeResult = await codeManager.VerifyAsync(user, code, SenderType.Email,
            CodeType.Allow, CodeResource.LinkedAccount, cancellationToken);

        if (!codeResult.Succeeded) return codeResult;
        
        var result = await providerManager.DisconnectAsync(user, provider, cancellationToken);
        return result;
    }
}
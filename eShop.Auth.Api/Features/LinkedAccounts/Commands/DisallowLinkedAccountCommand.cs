using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public record DisallowLinkedAccountCommand(DisallowLinkedAccountRequest Request) : IRequest<Result>;

public class DisallowLinkedAccountCommandHandler(
    IUserManager userManager,
    IOAuthProviderManager providerManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<DisallowLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(DisallowLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with ID {request.Request.Provider}.");

        var code = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.Disallow,
            CodeResource.LinkedAccount, cancellationToken);

        var message = new DisallowLinkedAccountMessage()
        {
            Credentials = new()
            {
                { "To", user!.Email },
                { "Subject", $"Disallow {provider.Name} linked account" }
            },
            Payload = new()
            {
                { "Code", code },
                { "Provider", provider.Name },
                { "UserName", user.UserName }
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        return Result.Success();
    }
}
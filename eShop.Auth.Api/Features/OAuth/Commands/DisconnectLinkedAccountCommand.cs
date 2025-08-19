using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.OAuth.Commands;

public record DisconnectLinkedAccountCommand(DisconnectLinkedAccountRequest Request) : IRequest<Result>;

public class DisconnectLinkedAccountCommandHandler(
    IUserManager userManager,
    IOAuthProviderManager providerManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<DisconnectLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(DisconnectLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with ID {request.Request.Provider}.");

        var code = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.Disconnect,
            CodeResource.LinkedAccount, cancellationToken);

        var message = new DisconnectLinkedAccountMessage()
        {
            Payload = new()
            {
                { "To", user!.Email },
                { "Subject", $"Disconnect {provider.Name} linked account" }
            },
            Credentials = new()
            {
                { "Code", code },
                { "Provider", provider.Name },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        return Result.Success();
    }
}
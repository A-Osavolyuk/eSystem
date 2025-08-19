using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.OAuth.Commands;

public record AllowLinkedAccountCommand(AllowLinkedAccountRequest Request) : IRequest<Result>;

public class AllowLinkedAccountCommandHandler(
    IUserManager userManager,
    IOAuthProviderManager providerManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<AllowLinkedAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(AllowLinkedAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider {request.Request.Provider}.");

        var code = await codeManager.GenerateAsync(user, SenderType.Email, CodeType.Allow,
            CodeResource.LinkedAccount, cancellationToken);

        var message = new AllowLinkedAccountMessage()
        {
            Payload = new()
            {
                { "To", user!.Email },
                { "Subject", $"Allow {provider.Name} linked account" }
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
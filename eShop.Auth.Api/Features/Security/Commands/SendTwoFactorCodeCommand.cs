using eShop.Auth.Api.Messages.Email;
using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Common.Security;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record SendTwoFactorCodeCommand(SendTwoFactorCodeRequest Request) : IRequest<Result>;

public class SendTwoFactorCodeCommandHandler(
    IUserManager userManager,
    ILoginCodeManager loginCodeManager,
    IProviderManager providerManager,
    IMessageService messageService) : IRequestHandler<SendTwoFactorCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILoginCodeManager loginCodeManager = loginCodeManager;
    private readonly IMessageService messageService = messageService;
    private readonly IProviderManager providerManager = providerManager;

    public async Task<Result> Handle(SendTwoFactorCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        
        var code = await loginCodeManager.GenerateAsync(user, provider, cancellationToken);

        var sender = provider.Name switch
        {
            ProviderTypes.Email => SenderType.Email,
            ProviderTypes.Sms => SenderType.Sms,
            _ => throw new NotSupportedException($"Provider type {provider.Name} is not supported.")
        };

        Message message = provider.Name switch
        {
            ProviderTypes.Email => new TwoFactorCodeEmailMessage()
            {
                Credentials = new ()
                {
                    { "To", user!.Email },
                    { "Subject", "Two-factor authentication" }
                }, 
                Payload = new()
                {
                    { "UserName", user.UserName },
                    { "Code", code },
                },
            },
            ProviderTypes.Sms => new TwoFactorCodeSmsMessage()
            {
                Credentials = new ()
                {
                    { "PhoneNumber", user.PhoneNumber! },
                }, 
                Payload = new()
                {
                    { "Code", code },
                },
            },
            _ => throw new NotSupportedException($"Provider type {provider.Name} is not supported.")
        };
        
        await messageService.SendMessageAsync(sender, message, cancellationToken);
        
        return Result.Success("Two-factor authentication code has been successfully sent");
    }
}
using eShop.Auth.Api.Messages.Email;
using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Common.Security;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record SendTwoFactorTokenCommand(SendTwoFactorTokenRequest Request) : IRequest<Result>;

public class SendTwoFactorTokenCommandHandler(
    IUserManager userManager,
    ILoginTokenManager loginTokenManager,
    IProviderManager providerManager,
    IMessageService messageService) : IRequestHandler<SendTwoFactorTokenCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILoginTokenManager loginTokenManager = loginTokenManager;
    private readonly IMessageService messageService = messageService;
    private readonly IProviderManager providerManager = providerManager;

    public async Task<Result> Handle(SendTwoFactorTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var provider = await providerManager.FindAsync(request.Request.Provider, cancellationToken);

        if (provider is null)
        {
            return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        }
        
        var token = await loginTokenManager.GenerateAsync(user, provider, cancellationToken);

        var sender = provider.Name switch
        {
            ProviderTypes.Email => SenderType.Email,
            ProviderTypes.Sms => SenderType.Sms,
            _ => throw new NotSupportedException($"Provider type {provider.Name} is not supported.")
        };

        Message message = provider.Name switch
        {
            ProviderTypes.Email => new TwoFactorTokenEmailMessage()
            {
                Credentials = new ()
                {
                    { "To", user!.Email },
                    { "Subject", "Two-factor authentication" }
                }, 
                Payload = new()
                {
                    { "UserName", user.UserName },
                    { "Code", token },
                },
            },
            ProviderTypes.Sms => new TwoFactorTokenSmsMessage()
            {
                Credentials = new ()
                {
                    { "PhoneNumber", user!.PhoneNumber },
                }, 
                Payload = new()
                {
                    { "Code", token },
                },
            },
            _ => throw new NotSupportedException($"Provider type {provider.Name} is not supported.")
        };
        
        await messageService.SendMessageAsync(sender, message, cancellationToken);
        
        return Result.Success("Two-factor authentication token has been successfully sent");
    }
}
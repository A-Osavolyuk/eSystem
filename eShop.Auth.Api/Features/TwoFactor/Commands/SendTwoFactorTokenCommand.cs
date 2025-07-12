using eShop.Auth.Api.Messaging.Email;
using eShop.Auth.Api.Messaging.Sms;
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
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var provider = await providerManager.FindAsync(request.Request.Provider, cancellationToken);

        if (provider is null)
        {
            return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        }
        
        var token = await loginTokenManager.GenerateAsync(user, provider, cancellationToken);

        switch (provider.Name)
        {
            case ProviderTypes.Email:
            {
                var message = new TwoFactorTokenEmailMessage()
                {
                    Credentials = new ()
                    {
                        { "To", user!.Email },
                        { "Subject", "Two-factor authentication" },
                        { "UserName", user.Email },
                    }, 
                    UserName = user.UserName,
                    Token = token
                };
        
                await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);
                
                break;
            }
            case ProviderTypes.Sms:
            {
                var message = new TwoFactorTokenSmsMessage()
                {
                    Credentials = new ()
                    {
                        { "PhoneNumber", user!.PhoneNumber },
                    }, 
                    Token = token
                };
        
                await messageService.SendMessageAsync(SenderType.Sms, message, cancellationToken);
                
                break;
            }
        }
        
        return Result.Success("Two-factor authentication token has been successfully sent");
    }
}
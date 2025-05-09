using eShop.Domain.Common.Security;
using eShop.Domain.Messages.Email;
using eShop.Domain.Messages.Sms;
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
        var deliveryType = string.Empty;

        switch (provider.Name)
        {
            case Providers.Email:
            {
                var message = new TwoFactorTokenEmailMessage()
                {
                    Token = token,
                    To = user.Email,
                    UserName = user.UserName,
                    Subject = "Two-factor authentication token"
                };
                
                deliveryType = "email address";
                await messageService.SendMessageAsync("email:two-factor-token", message, cancellationToken);
                break;
            }
            case Providers.Sms:
            {
                var message = new TwoFactorTokenSmsMessage()
                {
                    Token = token,
                    PhoneNumber = user.PhoneNumber
                };
                
                deliveryType = "phone number";
                await messageService.SendMessageAsync("sms:two-factor-token", message, cancellationToken);
                break;
            }
        }
        
        return Result.Success($"Two-factor authentication token has been successfully sent. Please check your {deliveryType}");
    }
}
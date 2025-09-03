using eShop.Auth.Api.Messages.Email;
using eShop.Auth.Api.Messages.Sms;
using eShop.Auth.Api.Security.Protection;
using eShop.Domain.Common.Security;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Providers.Commands;

public record SubscribeProviderCommand(SubscribeProviderRequest Request) : IRequest<Result>;

public class SubscribeProviderCommandHandler(
    IUserManager userManager,
    IProviderManager providerManager,
    ILoginCodeManager loginCodeManager,
    IMessageService messageService,
    ISecretManager secretManager,
    SecretProtector protector,
    IdentityOptions identityOptions) : IRequestHandler<SubscribeProviderCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IProviderManager providerManager = providerManager;
    private readonly ILoginCodeManager loginCodeManager = loginCodeManager;
    private readonly ISecretManager secretManager = secretManager;
    private readonly IMessageService messageService = messageService;
    private readonly SecretProtector protector = protector;
    private readonly IdentityOptions identityOptions = identityOptions;
    private const string QrCodeIssuer = "eShop";

    public async Task<Result> Handle(SubscribeProviderCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        
        if (provider.Name == ProviderTypes.Email)
        {
            if (identityOptions.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
                return Results.BadRequest("You need to confirm your email first.");

            if (!user.HasEmail())
                Results.BadRequest("You need to provide a verified email first.");
        }

        if (provider.Name == ProviderTypes.Sms)
        {
            if (identityOptions.SignIn.RequireConfirmedPhoneNumber && !user.PhoneNumberConfirmed) 
                return Results.BadRequest("You need to confirm your phone number first.");
            
            if (!user.HasPhoneNumber())
                Results.BadRequest("You need to provide a verified phone number first.");
        }

        if (provider.Name == ProviderTypes.Authenticator)
        {
            var secret = await secretManager.FindAsync(user, cancellationToken);
            if (secret is null) secret = await secretManager.GenerateAsync(user, cancellationToken);

            var unprotectedSecret = protector.Unprotect(secret.Secret);
            var qrCode = QrCodeGenerator.Generate(user.Email, unprotectedSecret, QrCodeIssuer);

            var response = new SubscribeProviderResponse() { QrCode = qrCode };

            return Result.Success(response);
        }

        var code = await loginCodeManager.GenerateAsync(user, provider, cancellationToken);

        var sender = provider.Name switch
        {
            ProviderTypes.Email => SenderType.Email,
            ProviderTypes.Sms => SenderType.Sms,
            _ => throw new NotSupportedException($"Provider type {provider.Name} is not supported.")
        };

        Message message = provider.Name switch
        {
            ProviderTypes.Email => new VerifyProviderEmailMessage()
            {
                Credentials = new()
                {
                    { "To", user!.Email },
                    { "Subject", "Two-factor authentication" }
                },
                Payload = new()
                {
                    { "UserName", user.Username },
                    { "Code", code },
                },
            },
            ProviderTypes.Sms => new VerifyProviderSmsMessage()
            {
                Credentials = new()
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

        return Result.Success();
    }
}
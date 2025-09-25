using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Providers.Commands;

public record SubscribeProviderCommand(SubscribeProviderRequest Request) : IRequest<Result>;

public class SubscribeProviderCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IVerificationManager verificationManager,
    ISecretManager secretManager,
    ILoginMethodManager loginMethodManager,
    IdentityOptions identityOptions) : IRequestHandler<SubscribeProviderCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly ISecretManager secretManager = secretManager;
    private readonly ILoginMethodManager loginMethodManager = loginMethodManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(SubscribeProviderCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var provider = await twoFactorManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        
        if (provider.Name == ProviderTypes.Email)
        {
            if (identityOptions.SignIn.RequireConfirmedEmail 
                && user.Emails.Any(x => x is { Type: EmailType.Primary, IsVerified: false }))
                return Results.BadRequest("You need to confirm your email first.");

            if (!user.HasEmail(EmailType.Primary))
                Results.BadRequest("You need to provide a verified email first.");
        }

        if (provider.Name == ProviderTypes.Sms)
        {
            if (identityOptions.SignIn.RequireConfirmedPhoneNumber 
                && user.PhoneNumbers.Any(x => x is { Type: PhoneNumberType.Primary, IsVerified: false })) 
                return Results.BadRequest("You need to confirm your phone number first.");
            
            if (!user.HasPhoneNumber(PhoneNumberType.Primary))
                Results.BadRequest("You need to provide a verified phone number first.");
        }

        if (provider.Name == ProviderTypes.Authenticator)
        {
            var secret = await secretManager.FindAsync(user, cancellationToken);
            if (secret is null) return Results.NotFound($"Cannot find authenticator secret");
        }
        
        var verificationResult = await verificationManager.VerifyAsync(user, 
            CodeResource.Provider, CodeType.Subscribe, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        if (!user.HasLoginMethod(LoginType.TwoFactor))
        {
            var methodResult = await loginMethodManager.CreateAsync(user, LoginType.OAuth,  cancellationToken);
            if (!methodResult.Succeeded) return methodResult;
        }
        
        var result = await twoFactorManager.SubscribeAsync(user, provider, cancellationToken);
        return result;
    }
}
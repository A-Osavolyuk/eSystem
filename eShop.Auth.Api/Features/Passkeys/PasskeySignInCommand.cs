using eShop.Domain.Constants;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Types;
using OtpNet;

namespace eShop.Auth.Api.Features.Passkeys;

public record PasskeySignInCommand(PasskeySignInRequest Request, HttpContext Context) : IRequest<Result>;

public class PasskeySignInCommandHandler(
    IUserManager userManager,
    IdentityOptions identityOptions) : IRequestHandler<PasskeySignInCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(PasskeySignInCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByUsernameAsync(request.Request.Username, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with name {request.Request.Username}.");

        var challengeBytes = KeyGeneration.GenerateRandomKey(32);
        var challenge = Convert.ToBase64String(challengeBytes);
        var allowedCredentials = user.Credentials
            .Select(x => new AllowedCredential()
            {
                Type = KeyType.PublicKey,
                Id = x.CredentialId
            }).ToList();

        var options = new PublicKeyCredentialRequestOptions()
        {
            Challenge = challenge,
            Timeout = 60000,
            Domain = identityOptions.Credentials.Domain,
            UserVerification = UserVerifications.Preferred,
            AllowedCredentials = allowedCredentials
        };

        request.Context.Session.SetString("webauthn_assertion_challenge", challenge);

        return Result.Success(options);
    }
}
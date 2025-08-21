using eShop.Domain.Constants;
using eShop.Domain.Types;
using OtpNet;

namespace eShop.Auth.Api.Features.WebAuthN;

public record CreateCredentialRequestOptionsCommand(HttpContext Context) : IRequest<Result>;

public class CreateRequestCredentialOptionsCommandHandler : IRequestHandler<CreateCredentialRequestOptionsCommand, Result>
{
    public async Task<Result> Handle(CreateCredentialRequestOptionsCommand credentialRequest, CancellationToken cancellationToken)
    {
        var challengeBytes = KeyGeneration.GenerateRandomKey(32);
        var challenge = Convert.ToBase64String(challengeBytes);

        var options = new CredentialRequestOptions()
        {
            Challenge = challenge,
            Timeout = 60000,
            Domain = "localhost",
            UserVerification = UserVerifications.Preferred
        };
        
        credentialRequest.Context.Session.SetString("webauthn_assertion_challenge", challenge);

        return Result.Success(options);
    }
}
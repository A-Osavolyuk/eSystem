using eShop.Domain.Constants;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Types;
using OtpNet;

namespace eShop.Auth.Api.Features.WebAuthN;

public record CreatePublicKeyCredentialCommand(
    CreatePublicKeyCredentialRequest Request, HttpContext HttpContext) : IRequest<Result>;

public class CreatePublicKeyCredentialCommandHandler(
    IUserManager userManager) : IRequestHandler<CreatePublicKeyCredentialCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(CreatePublicKeyCredentialCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var challengeBytes = KeyGeneration.GenerateRandomKey(32);
        var challenge = Convert.ToBase64String(challengeBytes);

        var userIdBytes = user.Id.ToByteArray();
        var userIdBase64 = Convert.ToBase64String(userIdBytes);
        
        var options = new PublicKeyCredentialCreationOptions()
        {
            Challenge = challenge,
            PublicKeyCredentialUser = new()
            {
                Id = userIdBase64,
                Name = user.UserName,
                DisplayName = request.Request.DisplayName,
            },
            AuthenticatorSelection = new()
            {
                AuthenticatorAttachment = AuthenticatorAttachments.Platform,
                UserVerification = UserVerifications.Preferred,
                ResidentKey = ResidentKeys.Preferred
            },
            PublicKeyCredentialParameters =
            [
                new() { Algorithm = Algorithms.Es256, Type = KeyType.PublicKey },
                new() { Algorithm = Algorithms.Rs256, Type = KeyType.PublicKey },
            ],
            ReplyingParty = new ReplyingParty()
            {
                Domain = "localhost",
                Name = "eAccount",
            },
            Attestation = Attestations.None,
            Timeout = 60000
        };
        
        request.HttpContext.Session.SetString("webauthn_attestation_challenge", challenge);
        
        return Result.Success(options);
    }
}
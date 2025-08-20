using System.Text.Json;
using System.Text.Json.Serialization;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Types;
using OtpNet;

namespace eShop.Auth.Api.Features.Security.Commands;

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
            User = new()
            {
                Id = userIdBase64,
                Name = user.UserName,
                DisplayName = request.Request.DisplayName,
            },
            AuthenticatorSelection = new()
            {
                AuthenticatorAttachment = AuthenticatorAttachment.Platform,
                UserVerification = UserVerification.Preferred,
                ResidentKey = ResidentKey.Preferred
            },
            PublicKeyCredentialParameters =
            [
                new() { Algorithm = Algorithm.Es256, Type = KeyType.PublicKey },
                new() { Algorithm = Algorithm.Rs256, Type = KeyType.PublicKey },
            ],
            ReplyingParty = new ReplyingParty()
            {
                Domain = "localhost",
                Name = "eAccount",
            },
            Attestation = Attestation.None,
            Timeout = 60000
        };
        
        request.HttpContext.Session.SetString("webauthn_challenge", challenge);
        
        return Result.Success(options);
    }
}